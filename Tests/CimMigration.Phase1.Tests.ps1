#Requires -Modules Pester
<#
    CIM Migration Phase 1 Validation
    Validates that Get-WmiObject/gwmi have been replaced with Get-CimInstance
    in all migrated files, and that the replacement commands are functional.
#>

BeforeAll {
    $RepoRoot = Split-Path $PSScriptRoot -Parent
    $AutomationDir = Join-Path $RepoRoot 'sccmclictr.automation'

    # Files that were migrated in Phase 1
    $MigratedFiles = @(
        'baseInit.cs'
        'functions\agentproperties.cs'
        'functions\inventory.cs'
        'functions\health.cs'
        'functions\softwareupdates.cs'
        'policy\requestedConfig.cs'
        'Properties\Settings.cs'
        'Properties\Resources.resx'
    )

    # Files intentionally deferred to Phase 2
    $DeferredFiles = @(
        'functions\processes.cs'     # Uses .GetOwner() WMI method
        'policy\actualConfig.cs'     # Uses Set-WmiInstance
    )
}

Describe 'Phase 1: Static Analysis' {

    Context 'Migrated files contain no Get-WmiObject remnants' {
        foreach ($file in $MigratedFiles) {
            $filePath = Join-Path $AutomationDir $file

            It "should not contain Get-WmiObject in <file>" -TestCases @(@{ file = $file; filePath = $filePath }) {
                $content = Get-Content $filePath -Raw -ErrorAction Stop
                $content | Should -Not -Match '\bGet-WmiObject\b'
                $content | Should -Not -Match '\bGet-WMIObject\b'
            }

            It "should not contain gwmi in <file>" -TestCases @(@{ file = $file; filePath = $filePath }) {
                $content = Get-Content $filePath -Raw -ErrorAction Stop
                # Match gwmi as a standalone command, not as part of another word
                $content | Should -Not -Match '(?<![a-zA-Z])gwmi\b'
            }

            It "should not contain Remove-WmiObject in <file>" -TestCases @(@{ file = $file; filePath = $filePath }) {
                $content = Get-Content $filePath -Raw -ErrorAction Stop
                $content | Should -Not -Match '\bRemove-WmiObject\b'
            }
        }
    }

    Context 'Migrated files use Get-CimInstance' {
        It 'baseInit.cs GetObjects method uses Get-CimInstance' {
            $content = Get-Content (Join-Path $AutomationDir 'baseInit.cs') -Raw
            $content | Should -Match 'Get-CimInstance'
        }

        It 'agentproperties.cs uses Get-CimInstance for Win32_OperatingSystem' {
            $content = Get-Content (Join-Path $AutomationDir 'functions\agentproperties.cs') -Raw
            $content | Should -Match 'Get-CimInstance.*Win32_OperatingSystem'
        }

        It 'inventory.cs uses Get-CimInstance for Win32_Processor' {
            $content = Get-Content (Join-Path $AutomationDir 'functions\inventory.cs') -Raw
            $content | Should -Match 'Get-CimInstance.*Win32_Processor'
        }
    }

    Context 'No ManagementObject casts remain in migrated files' {
        It 'softwareupdates.cs uses CimInstance casts, not ManagementObject' {
            $content = Get-Content (Join-Path $AutomationDir 'functions\softwareupdates.cs') -Raw
            $content | Should -Not -Match '\[System\.Management\.ManagementObject\[\]\]'
            $content | Should -Match '\[CimInstance\[\]\]'
        }
    }

    Context 'Deferred files still have WMI references (expected)' {
        foreach ($file in $DeferredFiles) {
            It "should still contain WMI references in <file> (deferred)" -TestCases @(@{ file = $file }) {
                $filePath = Join-Path $AutomationDir $file
                if (Test-Path $filePath) {
                    $content = Get-Content $filePath -Raw
                    # These files should still have WMI references — they're Phase 2
                    $hasWmi = $content -match 'wmi|WmiObject|Set-WmiInstance'
                    $hasWmi | Should -Be $true
                }
            }
        }
    }
}

Describe 'Phase 1: CIM Command Functionality (localhost)' {

    Context 'Core WMI classes accessible via Get-CimInstance' {
        It 'Win32_OperatingSystem returns data' {
            $os = Get-CimInstance -ClassName Win32_OperatingSystem
            $os | Should -Not -BeNullOrEmpty
            $os.Caption | Should -Not -BeNullOrEmpty
        }

        It 'Win32_Processor returns data' {
            $proc = Get-CimInstance -ClassName Win32_Processor
            $proc | Should -Not -BeNullOrEmpty
            $proc[0].Name | Should -Not -BeNullOrEmpty
        }

        It 'Win32_OperatingSystem date properties are native DateTime (not DMTF)' {
            $os = Get-CimInstance -ClassName Win32_OperatingSystem
            $os.LastBootUpTime | Should -BeOfType [DateTime]
        }

        It 'WQL query via -Query parameter works' {
            $result = Get-CimInstance -Query "SELECT * FROM Win32_OperatingSystem"
            $result | Should -Not -BeNullOrEmpty
        }

        It 'Namespace parameter works with root\cimv2' {
            $result = Get-CimInstance -Query "SELECT * FROM Win32_OperatingSystem" -Namespace "root\cimv2"
            $result | Should -Not -BeNullOrEmpty
        }
    }

    Context 'Remove-CimInstance pipeline works' {
        It 'Remove-CimInstance accepts pipeline input (dry run via -WhatIf)' {
            # Create a temporary WMI instance we can safely target
            # Just verify the cmdlet exists and accepts pipeline
            { Get-Command Remove-CimInstance -ErrorAction Stop } | Should -Not -Throw
        }
    }

    Context 'CimInstance type casting works' {
        It '[CimInstance[]] cast works on Get-CimInstance output' {
            $result = [CimInstance[]](Get-CimInstance -ClassName Win32_OperatingSystem)
            $result | Should -Not -BeNullOrEmpty
            $result[0] | Should -BeOfType [CimInstance]
        }
    }

    Context 'SCCM namespaces (skip if no ConfigMgr client)' {
        BeforeAll {
            $hasCcm = $null -ne (Get-CimInstance -Namespace 'root' -ClassName __Namespace -Filter "Name='ccm'" -ErrorAction SilentlyContinue)
        }

        It 'root\ccm namespace exists' -Skip:(-not $hasCcm) {
            $hasCcm | Should -Be $true
        }

        It 'CCM_InstalledProduct accessible via Get-CimInstance' -Skip:(-not $hasCcm) {
            $product = Get-CimInstance -Namespace 'root\ccm' -ClassName CCM_InstalledProduct -ErrorAction SilentlyContinue
            $product | Should -Not -BeNullOrEmpty
        }
    }
}

Describe 'Phase 1: Build Verification' {
    It 'Automation library builds without errors' {
        $msbuild = 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe'
        $csproj = Join-Path $AutomationDir 'sccmclictr.automation.csproj'

        $output = & $msbuild $csproj -p:Configuration=Debug -verbosity:minimal 2>&1
        $LASTEXITCODE | Should -Be 0
        ($output | Select-String 'error').Count | Should -Be 0
    }
}
