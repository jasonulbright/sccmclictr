BeforeAll {
    $stage = if ($env:SCCM_TEST_STAGE) { $env:SCCM_TEST_STAGE } else { Join-Path $PSScriptRoot '..\artifacts\stage' }
    Add-Type -Path (Join-Path $stage 'sccmclictr.automation.dll')
    $trace = [Diagnostics.TraceSource]::new('RecoveryTests')
    $runspace = [Management.Automation.Runspaces.RunspaceFactory]::CreateRunspace([Management.Automation.Runspaces.WSManConnectionInfo]::new())
    $expectedUtc = [datetime]::new(2026, 9, 8, 12, 0, 0, [DateTimeKind]::Utc)
    function New-UpdateRow([string]$ClassName) {
        $properties = @{ __CLASS=$ClassName; __NAMESPACE='root\ccm\ClientSDK'; __RELPATH=($ClassName + '.UpdateID="test-update"') }
        $type = if ($ClassName -eq 'CCM_UpdateStatus') { [sccmclictr.automation.functions.softwareupdates+CCM_UpdateStatus] } else { [sccmclictr.automation.functions.softwareupdates+CCM_SoftwareUpdate] }
        foreach ($property in $type.GetProperties()) {
            $underlying = [Nullable]::GetUnderlyingType($property.PropertyType)
            if ($property.PropertyType -eq [string]) { $properties[$property.Name] = '' }
            elseif ($underlying -eq [uint32]) { $properties[$property.Name] = [uint32]0 }
            elseif ($underlying -eq [bool]) { $properties[$property.Name] = $false }
            elseif ($underlying -eq [datetime]) { $properties[$property.Name] = '20260908120000.000000+000' }
        }
        if ($ClassName -eq 'CCM_UpdateStatus') { $properties.Article = '123456' } else { $properties.ArticleID = '123456'; $properties.UpdateID = 'test-update' }
        [Management.Automation.PSSerializer]::Deserialize([Management.Automation.PSSerializer]::Serialize([pscustomobject]$properties))
    }
}
AfterAll { $runspace.Dispose(); $trace.Close() }

Describe 'Recovered compiled automation library' {
    It 'targets Framework 4.8 and retains the 1.0.1.0 library identity' {
        $assembly = [sccmclictr.automation.common].Assembly
        $assembly.GetName().Version.ToString() | Should -Be '1.0.1.0'
        $assembly.ImageRuntimeVersion | Should -Be 'v4.0.30319'
        @($assembly.GetReferencedAssemblies().Name) | Should -Contain 'System.Management'
    }
    It 'loads available update rows with WMI metadata and DMTF ScanTime' {
        $row = New-UpdateRow CCM_UpdateStatus
        $model = [sccmclictr.automation.functions.softwareupdates+CCM_UpdateStatus]::new($row, $runspace, $trace)
        $model.Article | Should -Be '123456'
        $model.ScanTime.ToUniversalTime() | Should -Be $expectedUtc
    }
    It 'loads pending update rows with deadlines and numeric state' {
        $row = New-UpdateRow CCM_SoftwareUpdate
        $model = [sccmclictr.automation.functions.softwareupdates+CCM_SoftwareUpdate]::new($row, $runspace, $trace)
        $model.UpdateID | Should -Be 'test-update'
        $model.Deadline.ToUniversalTime() | Should -Be $expectedUtc
        $model.EvaluationState | Should -Be 0
    }
    It 'keeps a missing scan date null' {
        $row = New-UpdateRow CCM_UpdateStatus
        $row.ScanTime = $null
        $model = [sccmclictr.automation.functions.softwareupdates+CCM_UpdateStatus]::new($row, $runspace, $trace)
        $model.ScanTime | Should -BeNullOrEmpty
    }
    It 'retains DPAPI round-trip compatibility after removing unused TripleDES' {
        $encrypted = [sccmclictr.automation.common]::Encrypt('test only', 'test entropy')
        $encrypted | Should -Not -BeNullOrEmpty
        [sccmclictr.automation.common]::Decrypt($encrypted, 'test entropy') | Should -Be 'test only'
    }
    It 'surfaces provider errors and distinguishes empty success' {
        $local = [Management.Automation.Runspaces.RunspaceFactory]::CreateRunspace()
        $local.Open()
        try {
            $method = [sccmclictr.automation.common].Assembly.GetType('sccmclictr.automation.WSMan').GetMethod('RunPSScript', [Reflection.BindingFlags]'NonPublic,Static')
            { $method.Invoke($null, @("Write-Error 'provider unavailable'", $local, $true)) } | Should -Throw '*provider unavailable*'
            { $method.Invoke($null, @("throw 'connection failed'", $local, $true)) } | Should -Throw '*connection failed*'
            $method.Invoke($null, @('$null', $local, $true)).Count | Should -Be 0
        } finally { $local.Dispose() }
    }
    It 'uses WMI for model queries and replaces cached results on refresh' {
        $local = [Management.Automation.Runspaces.RunspaceFactory]::CreateRunspace()
        $local.Open()
        $helper = [sccmclictr.automation.baseInit]::new($runspace, $trace)
        try {
            [sccmclictr.automation.baseInit].GetProperty('remoteRunspace', [Reflection.BindingFlags]'NonPublic,Instance').SetValue($helper, $local)
            $ps = [powershell]::Create()
            try {
                $ps.Runspace = $local
                $null = $ps.AddScript('function global:Get-WmiObject { param($query,$namespace) $global:counter++; [pscustomobject]@{Value=$global:counter} }; function global:Get-CimInstance { throw "Wrong query transport" }; $global:counter=0').Invoke()
            } finally { $ps.Dispose() }
            $helper.GetObjects('root\test', 'SELECT * FROM Example', $false, [timespan]::FromMinutes(1))[0].Value | Should -Be 1
            $helper.GetObjects('root\test', 'SELECT * FROM Example', $true, [timespan]::FromMinutes(1))[0].Value | Should -Be 2
            $helper.GetObjects('root\test', 'SELECT * FROM Example', $false, [timespan]::FromMinutes(1))[0].Value | Should -Be 2
        } finally { $helper.Dispose(); $local.Dispose() }
    }
    It 'retains original WMI metadata on a real locally serialized WMI object' {
        $original = Get-WmiObject Win32_Process -Filter "ProcessId=$PID"
        $row = [Management.Automation.PSSerializer]::Deserialize([Management.Automation.PSSerializer]::Serialize($original))
        $model = [sccmclictr.automation.functions._PublicClass]::new($row, $runspace, $trace)
        $model.GetType().GetProperty('__CLASS', [Reflection.BindingFlags]'NonPublic,Instance').GetValue($model) | Should -Be 'Win32_Process'
    }
    It 'ships the library source and licenses alongside the binary' {
        foreach ($file in @('sccmclictr.automation\sccmclictr.automation.csproj', 'sccmclictr.automation\UPSTREAM.md', 'sccmclictr.automation\LICENSE.md', 'sccmclictr.automation\COPYING', 'tools\check-bare-catches.ps1')) {
            Join-Path $stage "library-source\$file" | Should -Exist
        }
    }
    It 'constructs a mandatory-update call with a typed collection and deadline filter without executing it' {
        $writer = [IO.StringWriter]::new()
        $listener = [Diagnostics.TextWriterTraceListener]::new($writer)
        $trace.Switch.Level = [Diagnostics.SourceLevels]::All
        $null = $trace.Listeners.Add($listener)
        $client = [Activator]::CreateInstance([sccmclictr.automation.ccm], [Reflection.BindingFlags]'Instance,NonPublic,Public', $null, @($runspace, $trace), $null)
        try {
            [sccmclictr.automation.baseInit].GetField('bShowPSCodeOnly', [Reflection.BindingFlags]'NonPublic,Instance').SetValue($client, $true)
            $updates = [sccmclictr.automation.functions.softwareupdates]::new($runspace, $trace, $client)
            $updates.InstallAllRequiredUpdates()
            $trace.Flush()
            $script = $writer.ToString()
            $script | Should -Match 'Deadline'
            $script | Should -Match 'InstallUpdates\(\[System.Management.ManagementObject\[\]\]\$updates\)'
            $script | Should -Match '\$updates.Count -gt 0'
        } finally { $trace.Listeners.Remove($listener); $listener.Dispose(); $writer.Dispose() }
    }
}
