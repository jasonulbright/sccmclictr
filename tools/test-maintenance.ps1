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

$integrationTests = Get-Content -LiteralPath (Join-Path $repoRoot 'Tests\Recovery.Integration.Tests.ps1') -Raw
Assert-True ($integrationTests -notmatch "ConvertTo-SecureString\s+'[^']+'\s+-AsPlainText") 'Integration tests contain a plaintext password.'

$commonHelpers = Get-Content -LiteralPath (Join-Path $repoRoot 'sccmclictr.automation\common.cs') -Raw
Assert-True ($commonHelpers -notmatch 'TripleDESCryptoServiceProvider') 'The common helpers instantiate the obsolete TripleDES provider.'

$libraryProject = Get-Content -LiteralPath (Join-Path $repoRoot 'sccmclictr.automation\sccmclictr.automation.csproj') -Raw
Assert-True ($libraryProject -match '<TargetFrameworkVersion>v4.8</TargetFrameworkVersion>') 'Automation must target .NET Framework 4.8.'
Assert-True ($mainProject -match 'ProjectReference Include="[^"]*sccmclictr.automation.csproj"') 'The application must build automation from source.'
Assert-True ($libraryProject -notmatch 'PackageReference|PostBuildEvent') 'The library must not fetch a binary package or use workstation signing.'
$agent = Get-Content -LiteralPath (Join-Path $repoRoot 'sccmclictr.automation\SCCMAgent.cs') -Raw
Assert-True ($agent -notmatch 'private string Password|PtrToStringUni') 'IPC credentials must not be copied into a persistent or temporary managed plaintext string.'
foreach ($file in @('AgentActions.cs', 'Inventory.cs')) {
    Assert-True ((Get-Content -LiteralPath (Join-Path $repoRoot "sccmclictr.automation\$file") -Raw) -notmatch 'Invoke-Expression') "$file regressed to expression-based MSI invocation."
}

$releaseWorkflow = Get-Content -LiteralPath (Join-Path $repoRoot '.github\workflows\release.yml') -Raw
Assert-True ($releaseWorkflow -match 'azure/artifact-signing-action@v2') 'The release workflow does not require Azure Artifact Signing.'
Assert-True ($releaseWorkflow -match 'verify-signatures\.ps1') 'The release workflow does not verify Authenticode signatures before publication.'
Assert-True ($releaseWorkflow -notmatch 'SIGNING_CERTIFICATE_(?:BASE64|PASSWORD)') 'The release workflow contains obsolete exportable-certificate secrets.'

& powershell -NoProfile -File (Join-Path $repoRoot 'tools\check-bare-catches.ps1')
Assert-True ($LASTEXITCODE -eq 0) 'Bare-catch regression guard failed.'

if ($failures.Count -gt 0) {
    $failures | ForEach-Object { Write-Error $_ }
    throw "$($failures.Count) maintenance check(s) failed."
}

Write-Host 'Maintenance checks passed.'
