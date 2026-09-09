[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [string]$OutputDirectory = '',

    [string]$Version = ''
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 2.0

$repoRoot = Split-Path $PSScriptRoot -Parent

function Resolve-MSBuild {
    $fromPath = Get-Command msbuild.exe -ErrorAction SilentlyContinue
    if ($fromPath) { return $fromPath.Source }

    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    if (-not (Test-Path -LiteralPath $vswhere)) {
        throw 'MSBuild was not found. Install Visual Studio 2022 Build Tools with the .NET desktop build workload.'
    }

    $installationPath = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
    if ([string]::IsNullOrWhiteSpace($installationPath)) {
        throw 'Visual Studio with MSBuild was not found.'
    }

    $candidate = Join-Path $installationPath 'MSBuild\Current\Bin\MSBuild.exe'
    if (-not (Test-Path -LiteralPath $candidate)) {
        throw "MSBuild was not found at $candidate"
    }

    return $candidate
}

function Invoke-MSBuild {
    param(
        [Parameter(Mandatory = $true)][string]$Project,
        [Parameter(Mandatory = $true)][string]$Platform
    )

    Write-Host "Building $Project ($Configuration)..."
    & $script:msbuild $Project /m /t:Rebuild "/p:Configuration=$Configuration" "/p:Platform=$Platform" /p:ContinuousIntegrationBuild=true /v:minimal /nologo
    if ($LASTEXITCODE -ne 0) {
        throw "MSBuild failed for $Project with exit code $LASTEXITCODE."
    }
}

function Copy-RequiredFile {
    param(
        [Parameter(Mandatory = $true)][string]$Source,
        [Parameter(Mandatory = $true)][string]$Destination
    )

    if (-not (Test-Path -LiteralPath $Source -PathType Leaf)) {
        throw "Required build output is missing: $Source"
    }
    Copy-Item -LiteralPath $Source -Destination $Destination -Force
}

function ConvertTo-FourPartVersion {
    param([Parameter(Mandatory = $true)][string]$Value)

    if ($Value -notmatch '^\d+\.\d+\.\d+(?:\.\d+)?$') {
        throw "Version must contain three or four numeric parts: $Value"
    }
    $parts = @($Value.Split('.'))
    while ($parts.Count -lt 4) { $parts += '0' }
    return ($parts -join '.')
}

Push-Location $repoRoot
try {
    $script:msbuild = Resolve-MSBuild
    Invoke-MSBuild -Project 'SCCMCliCtrWPF\SCCMCliCtrWPF.sln' -Platform 'Any CPU'

    $pluginProjects = Get-ChildItem (Join-Path $repoRoot 'Plugins') -Recurse -Filter '*.csproj' |
        Where-Object { $_.Name -notlike '*-NUC1.csproj' } |
        Sort-Object FullName

    foreach ($project in $pluginProjects) {
        Invoke-MSBuild -Project $project.FullName -Platform 'AnyCPU'
    }

    $mainOutput = Join-Path $repoRoot "SCCMCliCtrWPF\SCCMCliCtrWPF\bin\$Configuration"
    $appPath = Join-Path $mainOutput 'SCCMCliCtrWPF.exe'
    if (-not (Test-Path -LiteralPath $appPath -PathType Leaf)) {
        throw "Main executable was not produced: $appPath"
    }

    $fileVersion = (Get-Item -LiteralPath $appPath).VersionInfo.FileVersion
    if (-not [string]::IsNullOrWhiteSpace($Version)) {
        $expectedVersion = ConvertTo-FourPartVersion -Value $Version
        if ($fileVersion -ne $expectedVersion) {
            throw "Release version $expectedVersion does not match SCCMCliCtrWPF.exe file version $fileVersion."
        }
    }

    if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
        $OutputDirectory = Join-Path $repoRoot 'artifacts\stage'
    }
    elseif (-not [IO.Path]::IsPathRooted($OutputDirectory)) {
        $OutputDirectory = Join-Path $repoRoot $OutputDirectory
    }
    $OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)

    $artifactsRoot = [IO.Path]::GetFullPath((Join-Path $repoRoot 'artifacts'))
    $artifactsPrefix = $artifactsRoot.TrimEnd([IO.Path]::DirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
    if (-not $OutputDirectory.StartsWith($artifactsPrefix, [StringComparison]::OrdinalIgnoreCase)) {
        throw "OutputDirectory must be a child of $artifactsRoot because its previous contents are removed: $OutputDirectory"
    }
    if (Test-Path -LiteralPath $OutputDirectory) {
        Remove-Item -LiteralPath $OutputDirectory -Recurse -Force
    }
    New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null

    $runtimeFiles = @(
        'SCCMCliCtrWPF.exe',
        'SCCMCliCtrWPF.exe.config',
        'Customization.dll',
        'sccmclictr.automation.dll',
        'NavigationPane.dll',
        'System.Windows.Controls.Input.Toolkit.dll',
        'System.Windows.Controls.Layout.Toolkit.dll',
        'WPFToolkit.dll'
    )
    foreach ($name in $runtimeFiles) {
        Copy-RequiredFile -Source (Join-Path $mainOutput $name) -Destination (Join-Path $OutputDirectory $name)
    }
    Copy-RequiredFile -Source (Join-Path $repoRoot 'SCCMCliCtrWPF\SCCMCliCtrWPF\Icon16.ico') -Destination (Join-Path $OutputDirectory 'Icon16.ico')

    $pluginOutputs = @(
        @{ Project = 'Plugin_AppV46'; Directory = 'Plugin_AppV46'; File = 'Plugin_AppV46.dll' },
        @{ Project = 'Plugin_CompMgmt'; Directory = 'Plugin_CompMgmt'; File = 'Plugin_CompMgmt.dll' },
        @{ Project = 'Plugin_CustomTools_AMTTools'; Directory = 'Plugin_CustomTools_AMTTools'; File = 'Plugin_AMTTools.dll' },
        @{ Project = 'Plugin_EnablePSRemoting'; Directory = 'Plugin_EnablePSRemoting'; File = 'Plugin_EnablePSRemoting.dll' },
        @{ Project = 'Plugin_Explorer'; Directory = 'Plugin_Explorer'; File = 'Plugin_Explorer.dll' },
        @{ Project = 'Plugin_FEP'; Directory = 'Plugin_FEP'; File = 'Plugin_FEP.dll' },
        @{ Project = 'Plugin_MSInfo32'; Directory = 'Plugin_MSInfo32'; File = 'Plugin_MSInfo32.dll' },
        @{ Project = 'Plugin_MSRA'; Directory = 'Plugin_MSRA'; File = 'Plugin_MSRA.dll' },
        @{ Project = 'Plugin_PSScripts'; Directory = 'Plugin_PSScripts'; File = 'Plugin_PSScripts.dll' },
        @{ Project = 'Plugin_RDP'; Directory = 'Plugin_RDP'; File = 'Plugin_RDP.dll' },
        @{ Project = 'Plugin_Regedit'; Directory = 'Plugin_Regedit'; File = 'Plugin_Regedit.dll' },
        @{ Project = 'Plugin_CMRemote'; Directory = 'Plugin_RemoteTools'; File = 'Plugin_CMRemote.dll' },
        @{ Project = 'Plugin_CMResourceExplorer'; Directory = 'Plugin_ResourceExplorer'; File = 'Plugin_CMResourceExplorer.dll' },
        @{ Project = 'Plugin_StatusMessageViewer'; Directory = 'Plugin_StatusMessageViewer'; File = 'Plugin_StatusMessageViewer.dll' }
    )
    foreach ($plugin in $pluginOutputs) {
        $source = Join-Path $repoRoot "Plugins\$($plugin.Directory)\bin\$Configuration\$($plugin.File)"
        Copy-RequiredFile -Source $source -Destination (Join-Path $OutputDirectory $plugin.File)
    }

    Copy-RequiredFile -Source (Join-Path $repoRoot 'SCCMCliCtrWPF\SCCMCliCtrWPF\Plugin_Explorer.dll.config') -Destination (Join-Path $OutputDirectory 'Plugin_Explorer.dll.config')
    Copy-Item -LiteralPath (Join-Path $repoRoot 'Plugins\Plugin_PSScripts\PSScripts') -Destination (Join-Path $OutputDirectory 'PSScripts') -Recurse -Force

    foreach ($document in @('LICENSE.md', 'README.md', 'CHANGELOG.md')) {
        Copy-RequiredFile -Source (Join-Path $repoRoot $document) -Destination (Join-Path $OutputDirectory $document)
    }

    # Ship corresponding LGPL library source and build instructions with both
    # installer and portable output. Never fetch a precompiled automation DLL.
    $sourceBundle = Join-Path $OutputDirectory 'library-source'
    $libraryRoot = Join-Path $repoRoot 'sccmclictr.automation'
    Get-ChildItem -LiteralPath $libraryRoot -Recurse -File |
        Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' -and
            ($_.Extension -in '.cs', '.csproj', '.resx', '.settings', '.txt' -or
             $_.Name -in 'LICENSE.md', 'UPSTREAM.md', 'COPYING') } |
        ForEach-Object {
            $relative = $_.FullName.Substring($repoRoot.Length + 1)
            $destination = Join-Path $sourceBundle $relative
            New-Item -ItemType Directory -Path (Split-Path $destination -Parent) -Force | Out-Null
            Copy-RequiredFile -Source $_.FullName -Destination $destination
        }
    New-Item -ItemType Directory -Path (Join-Path $sourceBundle 'tools') -Force | Out-Null
    foreach ($file in @('check-bare-catches.ps1', 'bare-catches.baseline')) {
        Copy-RequiredFile -Source (Join-Path $repoRoot "tools\$file") -Destination (Join-Path $sourceBundle "tools\$file")
    }

    $commit = 'unknown'
    try { $commit = (& git rev-parse HEAD 2>$null).Trim() } catch { }
    $buildInfo = [ordered]@{
        version = $fileVersion
        commit = $commit
        configuration = $Configuration
        builtAtUtc = [DateTime]::UtcNow.ToString('o')
        targetFramework = '.NET Framework 4.8'
        automationSource = 'rzander/sccmclictrlib@1c875c00ab04144741247873cea1b69cb25ef1ea + documented maintenance patches'
    }
    $buildInfo | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $OutputDirectory 'build-info.json') -Encoding UTF8

    Write-Host "Build and staging complete: $OutputDirectory"
}
finally {
    Pop-Location
}
