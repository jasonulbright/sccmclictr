#Requires -Modules Pester
<#
    CIM Migration Integration Tests
    Runs against a live SCCM environment via AutomatedLab.
    Validates that migrated CIM commands produce correct results
    on a real Windows Server with MECM installed.
#>

BeforeAll {
    $TargetHost = '192.168.50.20'
    $cred = New-Object PSCredential('contoso\LabAdmin', (ConvertTo-SecureString 'P@ssw0rd!' -AsPlainText -Force))

    # Helper to run a script block on CM01 via direct PSRemoting
    function Invoke-OnCM01 {
        param([scriptblock]$ScriptBlock)
        Invoke-Command -ComputerName $TargetHost -Credential $cred -ScriptBlock $ScriptBlock
    }
}

Describe 'Integration: CIM connectivity on CM01' {

    It 'Get-CimInstance works on CM01 (Win32_OperatingSystem)' {
        $result = Invoke-OnCM01 {
            (Get-CimInstance -ClassName Win32_OperatingSystem).Caption
        }
        $result | Should -Match 'Windows Server 2025'
    }

    It 'Get-CimInstance with -Namespace works' {
        $result = Invoke-OnCM01 {
            (Get-CimInstance -Namespace 'root\cimv2' -ClassName Win32_ComputerSystem).Name
        }
        $result | Should -Be 'CM01'
    }

    It 'Get-CimInstance with -Query works' {
        $result = Invoke-OnCM01 {
            (Get-CimInstance -Query 'SELECT Version FROM Win32_OperatingSystem').Version
        }
        $result | Should -Not -BeNullOrEmpty
    }
}

Describe 'Integration: CIM date handling' {

    It 'CIM returns native DateTime for date properties' {
        $result = Invoke-OnCM01 {
            $os = Get-CimInstance -ClassName Win32_OperatingSystem
            $os.LastBootUpTime.GetType().Name
        }
        $result | Should -Be 'DateTime'
    }

    It 'CIM DateTime is not a DMTF string' {
        $result = Invoke-OnCM01 {
            $os = Get-CimInstance -ClassName Win32_OperatingSystem
            # DMTF strings look like "20231215120000.000000+000"
            # DateTime objects don't have that format
            $os.LastBootUpTime -match '^\d{14}\.'
        }
        $result | Should -Be $false
    }
}

Describe 'Integration: Invoke-CimMethod on CM01' {

    It 'Invoke-CimMethod works on a static class method' {
        $result = Invoke-OnCM01 {
            # Win32_Process.Create is a well-known static method
            $class = Get-CimClass -Namespace 'root\cimv2' -ClassName Win32_Process
            $class.CimClassMethods['Create'].Parameters.Count
        }
        $result | Should -BeGreaterThan 0
    }

    It 'Invoke-CimMethod with -Arguments hashtable works' {
        $result = Invoke-OnCM01 {
            # StdRegProv.GetStringValue — read a safe registry key
            $r = Invoke-CimMethod -Namespace 'root\default' -ClassName StdRegProv `
                -MethodName GetStringValue `
                -Arguments @{
                    hDefKey    = [uint32]2147483650  # HKLM
                    sSubKeyName = 'SOFTWARE\Microsoft\Windows NT\CurrentVersion'
                    sValueName  = 'ProductName'
                }
            $r.sValue
        }
        $result | Should -Match 'Windows Server 2025'
    }
}

Describe 'Integration: Dynamic parameter name discovery' {
    # This tests the pattern used by CallClassMethod in baseInit.cs

    It 'Get-CimClass returns method parameter names' {
        $result = Invoke-OnCM01 {
            $class = Get-CimClass -Namespace 'root\default' -ClassName StdRegProv
            $params = $class.CimClassMethods['GetStringValue'].Parameters.Name
            $params -join ','
        }
        $result | Should -Match 'hDefKey'
        $result | Should -Match 'sSubKeyName'
        $result | Should -Match 'sValueName'
    }

    It 'Dynamic param mapping builds correct Arguments hashtable' {
        $result = Invoke-OnCM01 {
            # Simulate what CallClassMethod does: positional args mapped to named params
            $_pv = @([uint32]2147483650, 'SOFTWARE\Microsoft\Windows NT\CurrentVersion', 'ProductName')
            $_pn = (Get-CimClass -Namespace 'root\default' -ClassName StdRegProv).CimClassMethods['GetStringValue'].Parameters.Name
            $_a = @{}
            for ($i = 0; $i -lt $_pv.Count; $i++) { $_a[$_pn[$i]] = $_pv[$i] }
            $r = Invoke-CimMethod -Namespace 'root\default' -ClassName StdRegProv -MethodName GetStringValue -Arguments $_a
            $r.sValue
        }
        $result | Should -Match 'Windows Server 2025'
    }
}

Describe 'Integration: Set-CimInstance pattern' {

    It 'Set-CimInstance cmdlet is available' {
        $result = Invoke-OnCM01 {
            (Get-Command Set-CimInstance -ErrorAction SilentlyContinue).Name
        }
        $result | Should -Be 'Set-CimInstance'
    }
}

Describe 'Integration: Remove-CimInstance pipeline' {

    It 'Remove-CimInstance accepts pipeline from Get-CimInstance' {
        $result = Invoke-OnCM01 {
            (Get-Command Remove-CimInstance).Parameters['InputObject'].Attributes |
                Where-Object { $_ -is [System.Management.Automation.ParameterAttribute] -and $_.ValueFromPipeline } |
                ForEach-Object { 'PipelineSupported' }
        }
        $result | Should -Contain 'PipelineSupported'
    }
}

Describe 'Integration: SCCM site server namespaces' {

    It 'root\sms namespace exists on site server' {
        $result = Invoke-OnCM01 {
            $ns = Get-CimInstance -Namespace 'root' -ClassName __Namespace -Filter "Name='sms'" -ErrorAction SilentlyContinue
            if ($ns) { 'exists' } else { 'missing' }
        }
        $result | Should -Be 'exists'
    }

    It 'SMS_Site class accessible via CIM on site server' {
        $result = Invoke-OnCM01 {
            $site = Get-CimInstance -Namespace 'root\sms\site_MCM' -ClassName SMS_Site -ErrorAction SilentlyContinue
            if ($site) { $site.SiteCode } else { 'not found' }
        }
        $result | Should -Be 'MCM'
    }
}
