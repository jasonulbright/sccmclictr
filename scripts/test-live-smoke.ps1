[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$ComputerName,
    [string]$StageDirectory = '',
    [pscredential]$Credential,
    [switch]$PromptForCredential,
    [string]$CredentialUserName = '',
    [string]$OutputPath = ''
)

# Read-only: no policy triggers, service changes, installs, cache deletion or reboot.
$ErrorActionPreference = 'Stop'
if (-not $StageDirectory) { $StageDirectory = Join-Path $PSScriptRoot '..\artifacts\recovery-stage' }
if (-not $OutputPath) { $OutputPath = Join-Path $PSScriptRoot '..\artifacts\diagnostics\live-smoke.json' }
$report = [ordered]@{ Computer=$ComputerName; CheckedAt=(Get-Date).ToString('o'); Checks=@(); Error=$null }
$agent = $null
try {
    if ($PromptForCredential) {
        $credentialPrompt = @{ Message="Read-only smoke test for ${ComputerName}: enter the credentials that worked in Client Center." }
        if ($CredentialUserName) { $credentialPrompt.UserName=$CredentialUserName }
        $Credential = Get-Credential @credentialPrompt
        if (-not $Credential) { throw 'Credential entry cancelled.' }
    }
    $dll = (Resolve-Path (Join-Path $StageDirectory 'sccmclictr.automation.dll')).Path
    $report.LibrarySHA256 = (Get-FileHash -LiteralPath $dll -Algorithm SHA256).Hash
    Add-Type -Path $dll
    if ($Credential) { $agent = [sccmclictr.automation.SCCMAgent]::new($ComputerName, $Credential) }
    else { $agent = [sccmclictr.automation.SCCMAgent]::new($ComputerName) }

    $probes = @(
        @{ Name='Pending updates'; Namespace='root\ccm\ClientSDK'; Class='CCM_SoftwareUpdate'; Key='UpdateID'; Read={ $agent.Client.SoftwareUpdates.GetSoftwareUpdate($true) } },
        @{ Name='All updates'; Namespace='root\ccm\SoftwareUpdates\UpdatesStore'; Class='CCM_UpdateStatus'; Key='UniqueId'; Read={ $agent.Client.SoftwareUpdates.GetUpdateStatus($true) } },
        @{ Name='Applications'; Namespace='root\ccm\ClientSDK'; Class='CCM_Application'; Key='Id'; Read={ $agent.Client.SoftwareDistribution.Applications_($true) } },
        @{ Name='Services'; Namespace='root\cimv2'; Class='Win32_Service'; Key='Name'; Read={ $agent.Client.Services.Win32_Services } },
        @{ Name='Client cache'; Namespace='root\ccm\SoftMgmtAgent'; Class='CacheInfoEx'; Key='CacheId'; Read={ $agent.Client.SWCache.CachedContent } },
        @{ Name='Installed software'; Namespace='root\CIMV2\sms'; Class='SMS_InstalledSoftware'; Key='SoftwareCode'; Read={ $agent.Client.Inventory.InstalledSoftware } }
    )
    foreach ($probe in $probes) {
        try {
            $raw = @($agent.Client.GetObjects($probe.Namespace, ('SELECT * FROM ' + $probe.Class), $true))
            $models = @(& $probe.Read)
            $rawKeys = @($raw | ForEach-Object { [string]$_.($probe.Key) } | Sort-Object)
            $modelKeys = @($models | ForEach-Object { [string]$_.($probe.Key) } | Sort-Object)
            if ($raw.Count -gt 0 -and @($rawKeys | Where-Object { $_ }).Count -eq 0) { throw "Provider returned no usable $($probe.Key) keys; parity is not established." }
            $matches = $raw.Count -eq $models.Count -and ($rawKeys -join '|') -ceq ($modelKeys -join '|')
            $report.Checks += [pscustomobject]@{ Name=$probe.Name; RawCount=$raw.Count; ModelCount=$models.Count; Matches=$matches; Empty=($raw.Count -eq 0); Error=$null }
        } catch {
            $report.Checks += [pscustomobject]@{ Name=$probe.Name; Matches=$false; Error=$_.Exception.Message }
        }
    }
    # Read logs through WinRM instead of depending on Explorer/SMB permissions.
    $remote = @{ ComputerName=$ComputerName; ErrorAction='Stop' }
    if ($Credential) { $remote.Credential=$Credential }
    $report.Logs = @(Invoke-Command @remote -ScriptBlock {
        foreach ($name in @('UpdatesDeployment.log','UpdatesHandler.log','WUAHandler.log')) {
            $path = Join-Path $env:windir "CCM\Logs\$name"
            try {
                [pscustomobject]@{ Name=$name; Readable=$true; Lines=@(Get-Content -LiteralPath $path -Tail 12 -ErrorAction Stop) }
            } catch { [pscustomobject]@{ Name=$name; Readable=$false; Error=$_.Exception.Message } }
        }
    })
} catch { $report.Error=$_.Exception.Message }
finally {
    if ($agent) { $agent.Dispose() }
    New-Item -ItemType Directory -Path (Split-Path ([IO.Path]::GetFullPath($OutputPath)) -Parent) -Force | Out-Null
    $report | ConvertTo-Json -Depth 7 | Set-Content -LiteralPath $OutputPath -Encoding UTF8
}
if ($report.Error -or @($report.Checks | Where-Object { -not $_.Matches }).Count -or @($report.Logs | Where-Object { -not $_.Readable }).Count) {
    Write-Error "Smoke test failed; details saved to $OutputPath"
    exit 1
}
Write-Host "Read-only smoke test complete: $OutputPath. Empty providers do not validate populated-row parsing."
