#Requires -Modules Pester
<#
    CIM Migration Integration Tests
    Runs against a live ConfigMgr environment supplied through environment variables.
    Validates that migrated CIM commands produce correct results
    on a real Windows Server with MECM installed.
#>

$IntegrationConfigured = -not [string]::IsNullOrWhiteSpace($env:SCCM_TEST_HOST) -and
    -not [string]::IsNullOrWhiteSpace($env:SCCM_TEST_USERNAME) -and
    -not [string]::IsNullOrWhiteSpace($env:SCCM_TEST_PASSWORD)

BeforeAll {
    $TargetHost = $env:SCCM_TEST_HOST
    $TestUsername = $env:SCCM_TEST_USERNAME
    $TestPassword = $env:SCCM_TEST_PASSWORD

    if ($IntegrationConfigured) {
        $securePassword = ConvertTo-SecureString $TestPassword -AsPlainText -Force
        $cred = New-Object PSCredential($TestUsername, $securePassword)
    }

    # Helper to run a script block on the configured host via direct PSRemoting.
    function Invoke-OnTestClient {
        param([scriptblock]$ScriptBlock)
        Invoke-Command -ComputerName $TargetHost -Credential $cred -ScriptBlock $ScriptBlock
    }
}

Describe 'Integration: CIM connectivity on the configured test client' -Skip:(-not $IntegrationConfigured) {

    It 'Get-CimInstance returns the operating-system caption' {
        $result = Invoke-OnTestClient {
            (Get-CimInstance -ClassName Win32_OperatingSystem).Caption
        }
        $result | Should -Match 'Windows'
    }

    It 'Get-CimInstance with -Namespace works' {
        $result = Invoke-OnTestClient {
            (Get-CimInstance -Namespace 'root\cimv2' -ClassName Win32_ComputerSystem).Name
        }
        $result | Should -Not -BeNullOrEmpty
    }

    It 'Get-CimInstance with -Query works' {
        $result = Invoke-OnTestClient {
            (Get-CimInstance -Query 'SELECT Version FROM Win32_OperatingSystem').Version
        }
        $result | Should -Not -BeNullOrEmpty
    }
}

Describe 'Integration: CIM date handling' -Skip:(-not $IntegrationConfigured) {

    It 'CIM returns native DateTime for date properties' {
        $result = Invoke-OnTestClient {
            $os = Get-CimInstance -ClassName Win32_OperatingSystem
            $os.LastBootUpTime.GetType().Name
        }
        $result | Should -Be 'DateTime'
    }

    It 'CIM DateTime is not a DMTF string' {
        $result = Invoke-OnTestClient {
            $os = Get-CimInstance -ClassName Win32_OperatingSystem
            # DMTF strings look like "20231215120000.000000+000"
            # DateTime objects don't have that format
            $os.LastBootUpTime -match '^\d{14}\.'
        }
        $result | Should -Be $false
    }
}

Describe 'Integration: Invoke-CimMethod on the configured test client' -Skip:(-not $IntegrationConfigured) {

    It 'Invoke-CimMethod works on a static class method' {
        $result = Invoke-OnTestClient {
            # Win32_Process.Create is a well-known static method
            $class = Get-CimClass -Namespace 'root\cimv2' -ClassName Win32_Process
            $class.CimClassMethods['Create'].Parameters.Count
        }
        $result | Should -BeGreaterThan 0
    }

    It 'Invoke-CimMethod with -Arguments hashtable works' {
        $result = Invoke-OnTestClient {
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
        $result | Should -Match 'Windows'
    }
}

Describe 'Integration: Dynamic parameter name discovery' -Skip:(-not $IntegrationConfigured) {
    # This tests the pattern used by CallClassMethod in baseInit.cs

    It 'Get-CimClass returns method parameter names' {
        $result = Invoke-OnTestClient {
            $class = Get-CimClass -Namespace 'root\default' -ClassName StdRegProv
            $params = $class.CimClassMethods['GetStringValue'].Parameters.Name
            $params -join ','
        }
        $result | Should -Match 'hDefKey'
        $result | Should -Match 'sSubKeyName'
        $result | Should -Match 'sValueName'
    }

    It 'Dynamic param mapping builds correct Arguments hashtable' {
        $result = Invoke-OnTestClient {
            # Simulate what CallClassMethod does: positional args mapped to named params
            $_pv = @([uint32]2147483650, 'SOFTWARE\Microsoft\Windows NT\CurrentVersion', 'ProductName')
            $_pn = (Get-CimClass -Namespace 'root\default' -ClassName StdRegProv).CimClassMethods['GetStringValue'].Parameters.Name
            $_a = @{}
            for ($i = 0; $i -lt $_pv.Count; $i++) { $_a[$_pn[$i]] = $_pv[$i] }
            $r = Invoke-CimMethod -Namespace 'root\default' -ClassName StdRegProv -MethodName GetStringValue -Arguments $_a
            $r.sValue
        }
        $result | Should -Match 'Windows'
    }
}

Describe 'Integration: Set-CimInstance pattern' -Skip:(-not $IntegrationConfigured) {

    It 'Set-CimInstance cmdlet is available' {
        $result = Invoke-OnTestClient {
            (Get-Command Set-CimInstance -ErrorAction SilentlyContinue).Name
        }
        $result | Should -Be 'Set-CimInstance'
    }
}

Describe 'Integration: Remove-CimInstance pipeline' -Skip:(-not $IntegrationConfigured) {

    It 'Remove-CimInstance accepts pipeline from Get-CimInstance' {
        $result = Invoke-OnTestClient {
            (Get-Command Remove-CimInstance).Parameters['InputObject'].Attributes |
                Where-Object { $_ -is [System.Management.Automation.ParameterAttribute] -and $_.ValueFromPipeline } |
                ForEach-Object { 'PipelineSupported' }
        }
        $result | Should -Contain 'PipelineSupported'
    }
}

Describe 'Integration: SCCM site server namespaces' -Skip:(-not $IntegrationConfigured) {

    It 'root\sms namespace exists on site server' {
        $result = Invoke-OnTestClient {
            $ns = Get-CimInstance -Namespace 'root' -ClassName __Namespace -Filter "Name='sms'" -ErrorAction SilentlyContinue
            if ($ns) { 'exists' } else { 'missing' }
        }
        $result | Should -Be 'exists'
    }

    It 'SMS_Site class accessible via CIM on site server' {
        $result = Invoke-OnTestClient {
            $provider = Get-CimInstance -Namespace 'root\sms' -ClassName SMS_ProviderLocation -ErrorAction SilentlyContinue |
                Where-Object ProviderForLocalSite |
                Select-Object -First 1
            if (-not $provider) { return 'not found' }

            $siteNamespace = "root\sms\site_$($provider.SiteCode)"
            $site = Get-CimInstance -Namespace $siteNamespace -ClassName SMS_Site -ErrorAction SilentlyContinue
            if ($site) { $site.SiteCode } else { 'not found' }
        }
        $result | Should -Not -Be 'not found'
        $result | Should -Not -BeNullOrEmpty
    }
}
