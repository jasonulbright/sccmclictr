[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path $PSScriptRoot -Parent
$testAssembly = Join-Path $repoRoot "SCCMCliCtrWPF\SCCMCliCtrTests\bin\$Configuration\SCCMCliCtrTests.dll"
if (-not (Test-Path -LiteralPath $testAssembly -PathType Leaf)) {
    throw "Test assembly was not found. Run scripts\build.ps1 first: $testAssembly"
}

$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (-not (Test-Path -LiteralPath $vswhere -PathType Leaf)) {
    throw 'vswhere.exe was not found.'
}
$installationPath = & $vswhere -latest -products * -requires Microsoft.VisualStudio.Workload.ManagedDesktop -property installationPath
if ([string]::IsNullOrWhiteSpace($installationPath)) {
    $installationPath = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
}

$candidates = @(
    (Join-Path $installationPath 'Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe'),
    (Join-Path $installationPath 'Common7\IDE\Extensions\TestPlatform\vstest.console.exe')
)
$vstest = $candidates | Where-Object { Test-Path -LiteralPath $_ -PathType Leaf } | Select-Object -First 1
if (-not $vstest) { throw 'vstest.console.exe was not found in the Visual Studio installation.' }

& $vstest $testAssembly /Platform:x64 /Logger:Console
if ($LASTEXITCODE -ne 0) { throw "Tests failed with exit code $LASTEXITCODE." }
