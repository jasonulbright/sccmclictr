#requires -Version 5.1
<#
.SYNOPSIS
    Bare-catch regression guard. Fails the build if any tracked .cs file
    introduces NEW bare `catch { }` blocks beyond what the baseline records.

.DESCRIPTION
    Scans all .cs files (excluding bin/obj/Properties/Settings.Designer.cs and
    packages/) for bare catch clauses (no exception variable). Per-file counts
    are compared against tools/bare-catches.baseline.

    Decreases are permitted (and ignored — the baseline can be re-written via
    -Update once the work is committed). Increases or new files with bare
    catches cause exit 1.

    Wired into sccmclictr.automation as a BeforeBuild target. Run manually with
    -Update to re-baseline after a C-phase commit.

.PARAMETER Update
    Rewrites the baseline file with current per-file counts. Use after a
    C-phase commit reduces the bare-catch surface.

.EXAMPLE
    powershell -File tools/check-bare-catches.ps1
    powershell -File tools/check-bare-catches.ps1 -Update
#>
param(
    [switch]$Update
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$baselinePath = Join-Path $PSScriptRoot 'bare-catches.baseline'

function Get-BareCatchCount {
    param($filePath)
    $count = 0
    foreach ($line in Get-Content $filePath) {
        if ($line -match '^\s*catch\s*\{' -or $line -match '^\s*catch\s*$') {
            $count++
        }
    }
    return $count
}

# Build current per-file map
$current = [ordered]@{}
Get-ChildItem $root -Recurse -Filter '*.cs' -File | Where-Object {
    $_.FullName -notmatch '\\bin\\|\\obj\\|\\Properties\\Settings\.Designer\.cs$|\\packages\\|\\tools\\'
} | Sort-Object FullName | ForEach-Object {
    $rel = $_.FullName.Substring($root.Length + 1) -replace '\\', '/'
    $count = Get-BareCatchCount $_.FullName
    if ($count -gt 0) { $current[$rel] = $count }
}

if ($Update) {
    $lines = $current.Keys | ForEach-Object { "${_}:$($current[$_])" }
    [System.IO.File]::WriteAllLines($baselinePath, $lines, (New-Object System.Text.UTF8Encoding($false)))
    $total = ($current.Values | Measure-Object -Sum).Sum
    Write-Host "Baseline updated: $($current.Count) files, $total bare catches total."
    exit 0
}

# Load baseline
if (-not (Test-Path $baselinePath)) {
    Write-Host "ERROR: baseline file not found at $baselinePath. Run with -Update to create it." -ForegroundColor Red
    exit 1
}

$baseline = [ordered]@{}
foreach ($line in Get-Content $baselinePath) {
    if ($line -match '^(.+):(\d+)$') { $baseline[$matches[1]] = [int]$matches[2] }
}

# Compare
$violations = @()
foreach ($file in $current.Keys) {
    $cur = $current[$file]
    $base = if ($baseline.Contains($file)) { $baseline[$file] } else { 0 }
    if ($cur -gt $base) {
        $violations += [pscustomobject]@{ File = $file; Baseline = $base; Current = $cur; Delta = ($cur - $base) }
    }
}

if ($violations.Count -gt 0) {
    Write-Host ""
    Write-Host "BARE-CATCH REGRESSION DETECTED" -ForegroundColor Red
    Write-Host "==============================" -ForegroundColor Red
    Write-Host "New bare 'catch { }' blocks were added beyond the audited baseline."
    Write-Host "Either fix them (use 'catch (Exception ex)' with logging) OR update"
    Write-Host "the baseline via 'pwsh tools/check-bare-catches.ps1 -Update' after"
    Write-Host "an intentional C-phase commit."
    Write-Host ""
    $violations | Format-Table -AutoSize
    exit 1
}

# Optional: surface decreases as info (good news)
$decreases = @()
foreach ($file in $baseline.Keys) {
    $cur = if ($current.Contains($file)) { $current[$file] } else { 0 }
    $base = $baseline[$file]
    if ($cur -lt $base) { $decreases += "$file : $base -> $cur" }
}
if ($decreases.Count -gt 0 -and $env:VERBOSE_BARE_CATCH) {
    Write-Host "Bare-catch decreases (run -Update to refresh baseline):" -ForegroundColor Cyan
    $decreases | ForEach-Object { Write-Host "  $_" }
}

$total = ($current.Values | Measure-Object -Sum).Sum
Write-Host "Bare-catch guard OK: $($current.Count) files, $total total (baseline holds)."
