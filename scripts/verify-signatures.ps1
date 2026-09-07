[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string[]]$Path,

    [Parameter(Mandatory = $true)]
    [string]$ExpectedSubjectCommonName
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 2.0

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
        throw "Signature-verification target was not found: $item"
    }
}

if ($files.Count -eq 0) { throw 'No signature-verification targets were found.' }

foreach ($file in $files | Sort-Object FullName -Unique) {
    $signature = Get-AuthenticodeSignature -LiteralPath $file.FullName
    if ($signature.Status -ne 'Valid') {
        throw "Authenticode signature is not valid on $($file.FullName): $($signature.Status) $($signature.StatusMessage)"
    }

    $subjectCommonName = $signature.SignerCertificate.GetNameInfo(
        [Security.Cryptography.X509Certificates.X509NameType]::SimpleName,
        $false
    )
    if ($subjectCommonName -cne $ExpectedSubjectCommonName) {
        throw "Unexpected signer on $($file.FullName): '$subjectCommonName' (expected '$ExpectedSubjectCommonName')."
    }
    if ($null -eq $signature.TimeStamperCertificate) {
        throw "The signature on $($file.FullName) does not contain a trusted timestamp."
    }

    Write-Host "Verified $($file.Name): signer '$subjectCommonName', timestamp '$($signature.TimeStamperCertificate.Subject)'."
}
