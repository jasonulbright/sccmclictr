# Releasing

> Recovery is unreleased. Live client parity is a release gate; see the recovery
> release gate below. Do not overwrite public v1.3.0.1 assets with an unvalidated build.

The `Release` GitHub Actions workflow builds every .NET Framework 4.8 project, Authenticode-signs the first-party executables and DLLs, creates an Inno Setup installer and portable ZIP, generates SHA-256 checksums, adds GitHub build-provenance attestations, and publishes the assets to a GitHub release.

Unsigned releases are deliberately blocked. Signing uses the same Azure Artifact Signing account and public certificate profile as Spectra-PDF; no exportable code-signing certificate is stored in GitHub.

## One-time signing setup

Create a GitHub environment named `release`, then add a federated credential to the existing Azure signing app registration for this exact subject:

```text
repo:jasonulbright/sccmclictr:environment:release
```

The app registration must have the Artifact Signing Certificate Profile Signer role on `signalridgelabs` / `SRL-Public`. Add the same three repository secrets used by Spectra-PDF:

- `AZURE_CLIENT_ID`
- `AZURE_TENANT_ID`
- `AZURE_SUBSCRIPTION_ID`

GitHub exchanges its short-lived OIDC identity for Azure access; there is no client secret. The workflow uses SHA-256 Authenticode signatures and an RFC 3161 timestamp, then verifies the signer common name and timestamp on every first-party executable and DLL before it can publish.

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
# Recovery release gate

The source recovery is not yet published. Before tagging, validate the rebuilt
Framework 4.8 app against a live ConfigMgr client, including pending/all-update
queries, policy retrieval, and deliberately authorized update actions. A green
offline build alone is insufficient.

Verify that both packages include `library-source` with the original-source
automation project, local patches, LGPL/GPL license texts, and build instructions.
The automation DLL is compiled from checked-in source, never fetched from NuGet.
Keep the source commit and build provenance aligned with the shipped binary.
