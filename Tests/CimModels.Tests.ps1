BeforeAll {
    Add-Type -Path (Join-Path $PSScriptRoot '..\artifacts\stage\sccmclictr.automation.dll')
    $trace = [Diagnostics.TraceSource]::new('CimModelTests')
    $expectedUtc = [datetime]::new(2026, 9, 8, 12, 0, 0, [DateTimeKind]::Utc)
    # Create connection metadata only; no connection to a managed computer is opened.
    $runspace = [Management.Automation.Runspaces.RunspaceFactory]::CreateRunspace([Management.Automation.Runspaces.WSManConnectionInfo]::new())
    function New-UpdateRow([string]$ClassName) {
        $properties = @{}
        $type = if ($ClassName -eq 'CCM_UpdateStatus') { [sccmclictr.automation.functions.softwareupdates+CCM_UpdateStatus] } else { [sccmclictr.automation.functions.softwareupdates+CCM_SoftwareUpdate] }
        foreach ($property in $type.GetProperties()) {
            $underlying = [Nullable]::GetUnderlyingType($property.PropertyType)
            if ($property.PropertyType -eq [string]) { $properties[$property.Name] = '' }
            elseif ($underlying -eq [uint32]) { $properties[$property.Name] = [uint32]0 }
            elseif ($underlying -eq [bool]) { $properties[$property.Name] = $false }
            elseif ($underlying -eq [datetime]) { $properties[$property.Name] = [datetime]'2026-09-08T12:00:00Z' }
        }
        if ($ClassName -eq 'CCM_UpdateStatus') { $properties.Article = '123456' } else { $properties.ArticleID = '123456'; $properties.UpdateID = 'test-update' }
        $row = New-CimInstance -Namespace root/ccm/ClientSDK -ClassName $ClassName -ClientOnly -Property $properties
        [Management.Automation.PSSerializer]::Deserialize([Management.Automation.PSSerializer]::Serialize($row))
    }
}
AfterAll { $runspace.Dispose(); $trace.Close() }

Describe 'Compiled update models consume remoted CIM results' {
    It 'loads available updates without WMI metadata and preserves ScanTime' {
        $row = New-UpdateRow CCM_UpdateStatus
        $row.PSObject.Properties['__CLASS'] | Should -BeNullOrEmpty
        $model = [sccmclictr.automation.functions.softwareupdates+CCM_UpdateStatus]::new($row, $runspace, $trace)
        $model.Article | Should -Be '123456'
        $model.ScanTime.ToUniversalTime() | Should -Be $expectedUtc
    }
    It 'loads pending updates and preserves deadlines and numeric state' {
        $row = New-UpdateRow CCM_SoftwareUpdate
        $model = [sccmclictr.automation.functions.softwareupdates+CCM_SoftwareUpdate]::new($row, $runspace, $trace)
        $model.UpdateID | Should -Be 'test-update'
        $model.Deadline | Should -Be $expectedUtc
        $model.RestartDeadline | Should -Be $expectedUtc
        $model.EvaluationState | Should -Be 0
    }
    It 'keeps null update dates null' {
        $row = New-UpdateRow CCM_UpdateStatus
        $row.ScanTime = $null
        $model = [sccmclictr.automation.functions.softwareupdates+CCM_UpdateStatus]::new($row, $runspace, $trace)
        $model.ScanTime | Should -BeNullOrEmpty
    }
    It 'still reads legacy DMTF date strings' {
        [sccmclictr.automation.common]::DmtfToDateTime([object]'20260908120000.000000+000').ToUniversalTime() | Should -Be $expectedUtc
    }
    It 'preserves actual CIM class, namespace and keyed instance path after serialization' {
        $original = Get-CimInstance Win32_Process -Filter "ProcessId=$PID"
        $row = [Management.Automation.PSSerializer]::Deserialize([Management.Automation.PSSerializer]::Serialize($original))
        $model = [sccmclictr.automation.functions._PublicClass]::new($row, $runspace, $trace)
        $flags = [Reflection.BindingFlags]'NonPublic,Instance'
        $model.GetType().GetProperty('__CLASS', $flags).GetValue($model) | Should -Be 'Win32_Process'
        $model.GetType().GetProperty('__NAMESPACE', $flags).GetValue($model) | Should -Be 'root\cimv2'
        $model.GetType().GetProperty('__RELPATH', $flags).GetValue($model) | Should -Be ('Win32_Process.Handle="' + $PID + '"')
    }
    It 'retains WMI metadata for legacy callers' {
        $row = [pscustomobject]@{ __CLASS = 'Legacy'; __NAMESPACE = 'root\test'; __RELPATH = 'Legacy.Id="1"' }
        $model = [sccmclictr.automation.functions._PublicClass]::new($row, $runspace, $trace)
        $model.GetType().GetProperty('__RELPATH', [Reflection.BindingFlags]'NonPublic,Instance').GetValue($model) | Should -Be 'Legacy.Id="1"'
    }
    It 'surfaces query errors instead of returning them as model rows' {
        $local = [Management.Automation.Runspaces.RunspaceFactory]::CreateRunspace()
        $local.Open()
        try {
            $method = [sccmclictr.automation.common].Assembly.GetType('sccmclictr.automation.WSMan').GetMethod('RunPSScript', [Reflection.BindingFlags]'NonPublic,Static')
            { $method.Invoke($null, @("Write-Error 'provider unavailable'", $local, $true)) } | Should -Throw '*provider unavailable*'
            { $method.Invoke($null, @("throw 'connection failed'", $local, $true)) } | Should -Throw '*connection failed*'
            $result = $method.Invoke($null, @('$null', $local, $true))
            $result.Count | Should -Be 0
        } finally { $local.Dispose() }
    }
    It 'refresh replaces cached query results' {
        $local = [Management.Automation.Runspaces.RunspaceFactory]::CreateRunspace()
        $local.Open()
        $helper = [sccmclictr.automation.baseInit]::new($runspace, $trace)
        try {
            [sccmclictr.automation.baseInit].GetProperty('remoteRunspace', [Reflection.BindingFlags]'NonPublic,Instance').SetValue($helper, $local)
            $ps = [powershell]::Create()
            try {
                $ps.Runspace = $local
                $null = $ps.AddScript('function global:Get-CimInstance { param($query,$namespace) $global:counter++; [pscustomobject]@{Value=$global:counter} }; $global:counter=0').Invoke()
            } finally { $ps.Dispose() }
            $helper.GetObjects('root\test', 'SELECT * FROM Example', $false, [timespan]::FromMinutes(1))[0].Value | Should -Be 1
            $helper.GetObjects('root\test', 'SELECT * FROM Example', $true, [timespan]::FromMinutes(1))[0].Value | Should -Be 2
            $helper.GetObjects('root\test', 'SELECT * FROM Example', $false, [timespan]::FromMinutes(1))[0].Value | Should -Be 2
        } finally { $helper.Dispose(); $local.Dispose() }
    }
}
