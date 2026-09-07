[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string[]]$Path,

    [Parameter(Mandatory = $true)]
    [string]$CertificatePath,

    [Parameter(Mandatory = $true)]
    [string]$CertificatePassword,

    [string]$TimestampUrl = 'http://timestamp.digicert.com'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 2.0

if (-not (Test-Path -LiteralPath $CertificatePath -PathType Leaf)) {
    throw "Signing certificate was not found: $CertificatePath"
}

$kitsRoot = Join-Path ${env:ProgramFiles(x86)} 'Windows Kits\10\bin'
$signTool = Get-ChildItem $kitsRoot -Filter signtool.exe -Recurse -ErrorAction SilentlyContinue |
    Where-Object { $_.FullName -match '\\x64\\signtool\.exe$' } |
    Sort-Object { [version]$_.Directory.Parent.Name } -Descending |
    Select-Object -First 1
if (-not $signTool) {
    throw 'SignTool.exe was not found in the Windows 10 SDK.'
}

$files = [System.Collections.Generic.List[IO.FileInfo]]::new()
foreach ($item in $Path) {
    if (Test-Path -LiteralPath $item -PathType Container) {
        Get-ChildItem -LiteralPath $item -File |
            Where-Object {
                $_.Name -eq 'SCCMCliCtrWPF.exe' -or
                $_.Name -eq 'Customization.dll' -or
                $_.Name -eq 'sccmclictr.automation.dll' -or
                $_.Name -like 'Plugin_*.dll'
            } |
            ForEach-Object { $files.Add($_) }
    }
    elseif (Test-Path -LiteralPath $item -PathType Leaf) {
        $files.Add((Get-Item -LiteralPath $item))
    }
    else {
        throw "Signing target was not found: $item"
    }
}

if ($files.Count -eq 0) { throw 'No signing targets were found.' }

foreach ($file in $files | Sort-Object FullName -Unique) {
    Write-Host "Signing $($file.Name)..."
    & $signTool.FullName sign /fd SHA256 /f $CertificatePath /p $CertificatePassword /tr $TimestampUrl /td SHA256 $file.FullName
    if ($LASTEXITCODE -ne 0) { throw "SignTool failed for $($file.FullName)." }
    & $signTool.FullName verify /pa /q $file.FullName
    if ($LASTEXITCODE -ne 0) { throw "Signature verification failed for $($file.FullName)." }
}
