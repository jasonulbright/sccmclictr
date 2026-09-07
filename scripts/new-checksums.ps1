[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$Directory
)

$ErrorActionPreference = 'Stop'
$Directory = [IO.Path]::GetFullPath($Directory)
if (-not (Test-Path -LiteralPath $Directory -PathType Container)) {
    throw "Release directory was not found: $Directory"
}

$checksumPath = Join-Path $Directory 'checksums.txt'
$lines = Get-ChildItem -LiteralPath $Directory -File |
    Where-Object { $_.Name -ne 'checksums.txt' } |
    Sort-Object Name |
    ForEach-Object {
        $hash = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
        "$hash  $($_.Name)"
    }
$lines | Set-Content -LiteralPath $checksumPath -Encoding ASCII
Write-Host "Checksums written: $checksumPath"
