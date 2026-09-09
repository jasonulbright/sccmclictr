[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path $PSScriptRoot -Parent
$testFiles = @(
    (Join-Path $repoRoot 'Tests\Recovery.Tests.ps1'),
    (Join-Path $repoRoot 'Tests\Recovery.Integration.Tests.ps1')
)

$pester = Get-Module Pester -ListAvailable |
    Where-Object { $_.Version -ge [version]'5.7.1' } |
    Sort-Object Version -Descending |
    Select-Object -First 1
if (-not $pester) {
    throw 'Pester 5.7.1 or newer is required. Install it with: Install-Module Pester -MinimumVersion 5.7.1 -Scope CurrentUser'
}

Import-Module $pester.Path -Force
$configuration = [PesterConfiguration]::Default
$configuration.Run.Path = $testFiles
$configuration.Run.PassThru = $true
$configuration.Output.Verbosity = 'Detailed'
$result = Invoke-Pester -Configuration $configuration
if ($result.Result -ne 'Passed') {
    throw "Pester did not pass: $($result.FailedCount) failed test(s), $($result.FailedContainersCount) failed container(s)."
}
