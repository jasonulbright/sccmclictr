[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path $PSScriptRoot -Parent
$failures = [System.Collections.Generic.List[string]]::new()

function Assert-True {
    param([bool]$Condition, [string]$Message)
    if (-not $Condition) { $script:failures.Add($Message) }
}

$activeProjects = Get-ChildItem (Join-Path $repoRoot 'Plugins') -Recurse -Filter '*.csproj' |
    Where-Object { $_.Name -notlike '*-NUC1.csproj' }
Assert-True ($activeProjects.Count -eq 14) "Expected 14 active plugin projects; found $($activeProjects.Count)."

foreach ($project in $activeProjects) {
    $content = Get-Content -LiteralPath $project.FullName -Raw
    Assert-True ($content -notmatch 'bin\\Debug\\(?:Customization|sccmclictr\.automation)\.dll') "$($project.FullName) has a configuration-specific dependency pinned to Debug."
    Assert-True ($content -notmatch '<PostBuildEvent>') "$($project.FullName) contains a workstation-specific post-build event."
}

$mainProject = Get-Content -LiteralPath (Join-Path $repoRoot 'SCCMCliCtrWPF\SCCMCliCtrWPF\SCCMCliCtr.csproj') -Raw
Assert-True ($mainProject -notmatch '<PostBuildEvent>') 'The main project contains a workstation-specific post-build event.'
Assert-True ($mainProject -notmatch 'azurewebsites\.net/ClickOnce') 'The retired ClickOnce endpoint is still configured.'

$about = Get-Content -LiteralPath (Join-Path $repoRoot 'SCCMCliCtrWPF\SCCMCliCtrWPF\Controls\About.xaml') -Raw
Assert-True ($about -match 'Copyright \(C\) 2023 Roger Zander') 'The original copyright attribution is missing from About.'
Assert-True ($about -notmatch 'Copyright[^\r\n]*Jason Ulbright') 'Jason Ulbright must be listed as a contributor, not a copyright holder.'
Assert-True ($about -match 'Contributors:[^\r\n]*Jason Ulbright') 'Jason Ulbright is missing from the contributor list.'

$integrationTests = Get-Content -LiteralPath (Join-Path $repoRoot 'Tests\CimMigration.Integration.Tests.ps1') -Raw
Assert-True ($integrationTests -notmatch "ConvertTo-SecureString\s+'[^']+'\s+-AsPlainText") 'Integration tests contain a plaintext password.'

& (Join-Path $repoRoot 'tools\check-bare-catches.ps1')

if ($failures.Count -gt 0) {
    $failures | ForEach-Object { Write-Error $_ }
    throw "$($failures.Count) maintenance check(s) failed."
}

Write-Host 'Maintenance checks passed.'
