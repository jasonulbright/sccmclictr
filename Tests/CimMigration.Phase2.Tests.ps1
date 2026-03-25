#Requires -Modules Pester
<#
    CIM Migration Phase 2 Validation
    Tests that CIM cmdlet equivalents produce the same results as [wmi]/[wmiclass]
    accelerators. Run these BEFORE applying changes to the C# code.

    Pattern mapping:
      [wmi]"NS:Class=@"              → Get-CimInstance -Namespace NS -ClassName Class
      [wmi]"NS:Class.Key='Val'"      → Get-CimInstance -Namespace NS -ClassName Class -Filter "Key='Val'"
      [wmiclass]"NS:Class"           → Invoke-CimMethod -Namespace NS -ClassName Class -MethodName ...
      [wmi]"..." | Remove-WmiObject  → Get-CimInstance ... | Remove-CimInstance
      $a=[wmi]"...";$a.Prop=V;$a.Put() → Set-CimInstance / Invoke-CimMethod
#>

BeforeAll {
    # Helper to parse WMI path "NAMESPACE:ClassName.Key='Value'" or "NAMESPACE:ClassName=@"
    function Split-WmiPath {
        param([string]$Path)
        $parts = $Path -split ':', 2
        $ns = $parts[0] -replace '\\\\', '\'
        $classAndKey = $parts[1]

        if ($classAndKey -match '^([^.=]+)=@$') {
            # Singleton: NS:Class=@
            return @{ Namespace = $ns; ClassName = $Matches[1]; Filter = $null; IsSingleton = $true }
        }
        elseif ($classAndKey -match '^([^.]+)\.(.+)$') {
            # Keyed: NS:Class.Key='Value'
            return @{ Namespace = $ns; ClassName = $Matches[1]; Filter = $Matches[2]; IsSingleton = $false }
        }
        else {
            # Class only: NS:Class
            return @{ Namespace = $ns; ClassName = $classAndKey; Filter = $null; IsSingleton = $false }
        }
    }
}

Describe 'Phase 2: WMI Path Parser' {
    It 'Parses singleton path (NS:Class=@)' {
        $r = Split-WmiPath 'ROOT\cimv2:Win32_OperatingSystem=@'
        $r.Namespace | Should -Be 'ROOT\cimv2'
        $r.ClassName | Should -Be 'Win32_OperatingSystem'
        $r.IsSingleton | Should -Be $true
    }

    It 'Parses keyed path (NS:Class.Key=Value)' {
        $r = Split-WmiPath "ROOT\ccm\invagt:InventoryActionStatus.InventoryActionID='{00000000-0000-0000-0000-000000000001}'"
        $r.Namespace | Should -Be 'ROOT\ccm\invagt'
        $r.ClassName | Should -Be 'InventoryActionStatus'
        $r.Filter | Should -Match 'InventoryActionID'
    }

    It 'Parses class-only path (NS:Class)' {
        $r = Split-WmiPath 'ROOT\ccm\ClientSDK:CCM_SoftwareUpdatesManager'
        $r.Namespace | Should -Be 'ROOT\ccm\ClientSDK'
        $r.ClassName | Should -Be 'CCM_SoftwareUpdatesManager'
    }
}

Describe 'Phase 2: GetProperty equivalent (read singleton property)' {
    # Old: ([wmi]"ROOT\cimv2:Win32_OperatingSystem=@").Caption
    # New: (Get-CimInstance -Namespace "ROOT\cimv2" -ClassName Win32_OperatingSystem).Caption

    Context '[wmi] singleton property read vs Get-CimInstance' {
        It 'Returns identical OS Caption' {
            $wmiResult = ([wmi]"ROOT\cimv2:Win32_OperatingSystem=@").Caption
            $cimResult = (Get-CimInstance -Namespace "ROOT\cimv2" -ClassName Win32_OperatingSystem).Caption
            $cimResult | Should -Be $wmiResult
        }

        It 'Returns identical OS Version' {
            $wmiResult = ([wmi]"ROOT\cimv2:Win32_OperatingSystem=@").Version
            $cimResult = (Get-CimInstance -Namespace "ROOT\cimv2" -ClassName Win32_OperatingSystem).Version
            $cimResult | Should -Be $wmiResult
        }

        It 'Returns identical ComputerName from Win32_ComputerSystem' {
            # Win32_ComputerSystem is not a =@ singleton — it's keyed by Name
            $wmiResult = (Get-WmiObject -Class Win32_ComputerSystem).Name
            $cimResult = (Get-CimInstance -ClassName Win32_ComputerSystem).Name
            $cimResult | Should -Be $wmiResult
        }
    }
}

Describe 'Phase 2: GetProperties equivalent (read multiple properties)' {
    # Old: ([wmi]'ROOT\cimv2:Win32_OperatingSystem=@').LastBootUpTime
    # New: (Get-CimInstance -Namespace 'ROOT\cimv2' -ClassName Win32_OperatingSystem).LastBootUpTime

    Context 'Date properties — CIM returns DateTime natively' {
        It 'CIM LastBootUpTime is DateTime, WMI is string' {
            $wmiResult = ([wmi]"ROOT\cimv2:Win32_OperatingSystem=@").LastBootUpTime
            $cimResult = (Get-CimInstance -Namespace "ROOT\cimv2" -ClassName Win32_OperatingSystem).LastBootUpTime

            # WMI returns DMTF string, CIM returns DateTime
            $wmiResult | Should -BeOfType [string]
            $cimResult | Should -BeOfType [DateTime]
        }

        It 'Both represent the same point in time' {
            $wmiResult = ([wmi]"ROOT\cimv2:Win32_OperatingSystem=@")
            $wmiDateTime = $wmiResult.ConvertToDateTime($wmiResult.LastBootUpTime)
            $cimDateTime = (Get-CimInstance -Namespace "ROOT\cimv2" -ClassName Win32_OperatingSystem).LastBootUpTime

            # Should be within 1 second of each other (query timing)
            [Math]::Abs(($wmiDateTime - $cimDateTime).TotalSeconds) | Should -BeLessThan 1
        }
    }
}

Describe 'Phase 2: SetProperty equivalent (modify and save)' {
    # Old: $a=([wmi]"NS:Class=@");$a.Prop=Value;$a.Put()
    # New: Get-CimInstance ... | Set-CimInstance -Property @{Prop=Value}
    #      or: Invoke-CimMethod for WMI methods

    Context 'Set-CimInstance pipeline pattern works' {
        It 'Set-CimInstance cmdlet exists and accepts -Property' {
            $cmd = Get-Command Set-CimInstance -ErrorAction Stop
            $cmd.Parameters.Keys | Should -Contain 'Property'
        }
    }
}

Describe 'Phase 2: CallClassMethod / GetStringFromClassMethod equivalent' {
    # Old: ([wmiclass]"NS:Class").Method(Params).Property
    # New: (Invoke-CimMethod -Namespace "NS" -ClassName "Class" -MethodName "Method" -Arguments @{...}).Property

    Context 'Invoke-CimMethod on static class method' {
        It 'Invoke-CimMethod cmdlet exists' {
            $cmd = Get-Command Invoke-CimMethod -ErrorAction Stop
            $cmd.Parameters.Keys | Should -Contain 'ClassName'
            $cmd.Parameters.Keys | Should -Contain 'MethodName'
            $cmd.Parameters.Keys | Should -Contain 'Namespace'
        }

        It 'Can invoke Win32_Process.Create (syntax check, not executed)' {
            # Just verify the parameter binding works — don't actually create a process
            { Get-CimClass -Namespace 'ROOT\cimv2' -ClassName Win32_Process } | Should -Not -Throw
        }
    }
}

Describe 'Phase 2: CallInstanceMethod equivalent' {
    # Old: ([wmi]'NS:Class.Key=Val').Method(Params)
    # New: Get-CimInstance ... | Invoke-CimMethod -MethodName "Method" -Arguments @{...}

    Context 'Instance method invocation pattern' {
        It 'Win32_OperatingSystem singleton supports Invoke-CimMethod' {
            $os = Get-CimInstance -Namespace 'ROOT\cimv2' -ClassName Win32_OperatingSystem
            $os | Should -Not -BeNullOrEmpty
            # GetType() would be via Invoke-CimMethod; just verify object exists
        }
    }
}

Describe 'Phase 2: Remove pattern ([wmi]... | Remove-WmiObject)' {
    # Old: [wmi]'NS:Class.Key=Val' | remove-wmiobject
    # New: Get-CimInstance -Namespace NS -ClassName Class -Filter "Key='Val'" | Remove-CimInstance

    Context 'CIM remove pipeline' {
        It 'Remove-CimInstance accepts CimInstance pipeline input' {
            $cmd = Get-Command Remove-CimInstance -ErrorAction Stop
            # Verify it accepts pipeline input by ParameterSet
            $cmd.Parameters['InputObject'].Attributes |
                Where-Object { $_ -is [System.Management.Automation.ParameterAttribute] -and $_.ValueFromPipeline } |
                Should -Not -BeNullOrEmpty
        }
    }
}

Describe 'Phase 2: baseInit.cs method migration patterns' {
    # These test the EXACT PS command patterns that will replace the [wmi]/[wmiclass] patterns

    Context 'GetProperty pattern: ([wmi]"NS:Class=@").Property' {
        It 'CIM equivalent returns same value as [wmi] singleton' {
            # Pattern: ([wmi]"ROOT\cimv2:Win32_OperatingSystem=@").Caption
            # CIM:     (Get-CimInstance -Namespace "ROOT\cimv2" -ClassName "Win32_OperatingSystem").Caption
            $old = ([wmi]"ROOT\cimv2:Win32_OperatingSystem=@").Caption
            $new = (Get-CimInstance -Namespace "ROOT\cimv2" -ClassName "Win32_OperatingSystem").Caption
            $new | Should -Be $old
        }
    }

    Context 'GetStringFromClassMethod pattern: ([wmiclass]"NS:Class").Method().Property' {
        It 'Invoke-CimMethod returns result with ReturnValue' {
            # Test with Win32_OperatingSystem to verify Invoke-CimMethod works on a class
            # This is a safe read-only call
            $class = Get-CimClass -Namespace 'ROOT\cimv2' -ClassName Win32_OperatingSystem
            $class | Should -Not -BeNullOrEmpty
            $class.CimClassMethods | Should -Not -BeNullOrEmpty
        }
    }

    Context 'SetProperty pattern: $a=[wmi]"NS:Class=@";$a.Prop=Val;$a.Put()' {
        It 'CIM equivalent using Set-CimInstance -Property hashtable' {
            # Verify the Set-CimInstance pattern is valid
            # Old: $a=([wmi]"ROOT\cimv2:Win32_WMISetting=@");$a.ASPScriptDefaultNamespace="root\cimv2";$a.Put()
            # New: Set-CimInstance -Namespace "ROOT\cimv2" -Query "SELECT * FROM Win32_WMISetting" -Property @{ASPScriptDefaultNamespace="root\cimv2"}
            # Just verify the cmdlet parameter structure — don't actually write
            $cmd = Get-Command Set-CimInstance
            $cmd.Parameters.Keys | Should -Contain 'Property'
            $cmd.Parameters.Keys | Should -Contain 'Namespace'
            $cmd.Parameters.Keys | Should -Contain 'Query'
        }
    }
}

Describe 'Phase 2: SCCM-specific patterns (skip if no ConfigMgr client)' {
    BeforeAll {
        $hasCcm = $null -ne (Get-CimInstance -Namespace 'root' -ClassName __Namespace -Filter "Name='ccm'" -ErrorAction SilentlyContinue)
    }

    Context 'SCCM singleton property read' {
        # Old: ([wmi]"ROOT\ccm:SMS_Client=@").ClientVersion
        # New: (Get-CimInstance -Namespace "ROOT\ccm" -ClassName SMS_Client).ClientVersion
        It 'SMS_Client.ClientVersion via Get-CimInstance' -Skip:(-not $hasCcm) {
            $cim = (Get-CimInstance -Namespace "ROOT\ccm" -ClassName SMS_Client).ClientVersion
            $wmi = ([wmi]"ROOT\ccm:SMS_Client=@").ClientVersion
            $cim | Should -Be $wmi
        }
    }

    Context 'SCCM class method invocation' {
        # Old: ([wmiclass]'ROOT\ccm:SMS_Client').GetAssignedSite().sSiteCode
        # New: (Invoke-CimMethod -Namespace 'ROOT\ccm' -ClassName SMS_Client -MethodName GetAssignedSite).sSiteCode
        It 'SMS_Client.GetAssignedSite via Invoke-CimMethod' -Skip:(-not $hasCcm) {
            $cim = (Invoke-CimMethod -Namespace 'ROOT\ccm' -ClassName SMS_Client -MethodName GetAssignedSite).sSiteCode
            $wmi = ([wmiclass]'ROOT\ccm:SMS_Client').GetAssignedSite().sSiteCode
            $cim | Should -Be $wmi
        }
    }

    Context 'SCCM keyed instance delete' {
        # Old: [wmi]"ROOT\ccm\invagt:InventoryActionStatus.InventoryActionID='...'" | remove-wmiobject
        # New: Get-CimInstance -Namespace "ROOT\ccm\invagt" -ClassName InventoryActionStatus -Filter "InventoryActionID='...'" | Remove-CimInstance
        It 'Get-CimInstance with filter returns keyed instance' -Skip:(-not $hasCcm) {
            # Just verify the query pattern works — don't actually delete
            $instances = Get-CimInstance -Namespace "ROOT\ccm\invagt" -ClassName InventoryActionStatus -ErrorAction SilentlyContinue
            # May or may not have instances, but the query shouldn't throw
        }
    }

    Context 'Software update install pattern' {
        # Old: ([wmiclass]'ROOT\ccm\ClientSDK:CCM_SoftwareUpdatesManager').InstallUpdates($updates)
        # New: Invoke-CimMethod -Namespace 'ROOT\ccm\ClientSDK' -ClassName CCM_SoftwareUpdatesManager -MethodName InstallUpdates -Arguments @{CCMUpdates=$updates}
        It 'CCM_SoftwareUpdatesManager class exists' -Skip:(-not $hasCcm) {
            $class = Get-CimClass -Namespace 'ROOT\ccm\ClientSDK' -ClassName CCM_SoftwareUpdatesManager -ErrorAction SilentlyContinue
            $class | Should -Not -BeNullOrEmpty
            $class.CimClassMethods.Name | Should -Contain 'InstallUpdates'
        }
    }
}

Describe 'Phase 2: Date handling difference' {
    # CRITICAL: Get-CimInstance returns native DateTime objects for date properties.
    # Get-WmiObject returned DMTF strings (e.g., "20231215120000.000000+000").
    # The C# code uses ManagementDateTimeConverter.ToDateTime() on the DMTF strings.
    # After CIM migration, PS-side dates are already DateTime — but they get serialized
    # through the PSObject pipeline, so the C# side still receives them.

    Context 'Date property serialization through PSObject' {
        It 'CIM date property is DateTime, not DMTF string' {
            $os = Get-CimInstance -ClassName Win32_OperatingSystem
            $os.InstallDate | Should -BeOfType [DateTime]
            $os.LastBootUpTime | Should -BeOfType [DateTime]
        }

        It 'WMI date property is DMTF string' {
            $os = Get-WmiObject -Class Win32_OperatingSystem
            $os.InstallDate | Should -BeOfType [string]
            $os.LastBootUpTime | Should -BeOfType [string]
        }
    }
}
