# Read-only live comparison. Never requests policy, installs updates or reboots.
# Set SCCM_TEST_HOST; use integrated authentication (including runas /netonly),
# or optionally supply SCCM_TEST_USERNAME and SCCM_TEST_PASSWORD in the process environment.
$configured = -not [string]::IsNullOrWhiteSpace($env:SCCM_TEST_HOST)
BeforeAll {
    $agent = $null
    if ($env:SCCM_TEST_HOST) {
        $stage = if ($env:SCCM_TEST_STAGE) { $env:SCCM_TEST_STAGE } else { Join-Path $PSScriptRoot '..\artifacts\stage' }
        Add-Type -Path (Join-Path $stage 'sccmclictr.automation.dll')
        $params = @{ ComputerName=$env:SCCM_TEST_HOST; ErrorAction='Stop' }
        if ($env:SCCM_TEST_USERNAME) {
            if (-not $env:SCCM_TEST_PASSWORD) { throw 'SCCM_TEST_PASSWORD is required with SCCM_TEST_USERNAME.' }
            $credential = [pscredential]::new($env:SCCM_TEST_USERNAME, (ConvertTo-SecureString $env:SCCM_TEST_PASSWORD -AsPlainText -Force))
            $params.Credential = $credential
            $agent = [sccmclictr.automation.SCCMAgent]::new($env:SCCM_TEST_HOST, $credential)
        } else {
            $agent = [sccmclictr.automation.SCCMAgent]::new($env:SCCM_TEST_HOST)
        }
        $raw = Invoke-Command @params -ScriptBlock {
            [pscustomobject]@{
                Pending = @(Get-WmiObject -Namespace root\ccm\ClientSDK -Class CCM_SoftwareUpdate -ErrorAction Stop | Select-Object -ExpandProperty UpdateID)
                Available = @(Get-WmiObject -Namespace root\ccm\SoftwareUpdates\UpdatesStore -Class CCM_UpdateStatus -ErrorAction Stop | Select-Object -ExpandProperty UniqueId)
            }
        }
    }
}
AfterAll { if ($agent) { $agent.Dispose() } }
Describe 'Live update model parity with direct WMI queries' -Skip:(-not $configured) {
    It 'matches pending update identities, not just a successful connection' {
        $actual = @($agent.Client.SoftwareUpdates.GetSoftwareUpdate($true) | ForEach-Object UpdateID | Sort-Object)
        ($actual -join '|') | Should -Be (@($raw.Pending | Sort-Object) -join '|')
    }
    It 'matches available update identities without silently dropping rows' {
        $actual = @($agent.Client.SoftwareUpdates.GetUpdateStatus($true) | ForEach-Object UniqueId | Sort-Object)
        ($actual -join '|') | Should -Be (@($raw.Available | Sort-Object) -join '|')
    }
}
