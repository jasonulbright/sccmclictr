# Releasing

The `Release` GitHub Actions workflow builds every .NET Framework 4.8 project, Authenticode-signs the first-party executables and DLLs, creates an Inno Setup installer and portable ZIP, generates SHA-256 checksums, adds GitHub build-provenance attestations, and publishes the assets to a GitHub release.

Unsigned releases are deliberately blocked.

## One-time signing setup

Add these GitHub Actions repository secrets:

- `SIGNING_CERTIFICATE_BASE64`: the Base64 representation of a trusted code-signing PFX certificate.
- `SIGNING_CERTIFICATE_PASSWORD`: the PFX password.

From PowerShell, an owner can populate them without writing the Base64 value to disk:

```powershell
[Convert]::ToBase64String([IO.File]::ReadAllBytes('C:\secure\code-signing.pfx')) | gh secret set SIGNING_CERTIFICATE_BASE64
gh secret set SIGNING_CERTIFICATE_PASSWORD
```

The workflow uses SHA-256 Authenticode signatures and an RFC 3161 timestamp. The PFX is decoded only into the ephemeral runner's temporary directory and is deleted in an `always()` cleanup step.

## Publish a release

1. Update `AssemblyFileVersion` and `AssemblyVersion` in `SCCMCliCtrWPF/SCCMCliCtrWPF/Properties/AssemblyInfo.cs`.
2. Add the release notes to `CHANGELOG.md`.
3. Run `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\build.ps1 -Configuration Release -Version X.Y.Z`.
4. Run `.\scripts\test.ps1 -Configuration Release` and `.\scripts\test-pester.ps1`.
5. Commit and push the changes.
6. Create and push an annotated `vX.Y.Z` tag.

The tag version must match the executable's four-part file version (a three-part tag is normalized with a final zero). A mismatch or missing signing secret stops the workflow before publication.

Published assets:

- `ClientCenterForConfigMgr-Setup.exe`
- `ClientCenterForConfigMgr-Portable.zip`
- `checksums.txt`
