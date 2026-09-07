# Contributing

Client Center is a maintenance-focused continuation of a mature administrative tool. Contributions should be small enough to validate and should prioritize current Windows/ConfigMgr compatibility, reliability, security, build repeatability, or operator usability.

## Before opening a change

- Search this repository and the [original issue history](https://github.com/rzander/sccmclictr/issues).
- Open an issue for behavior that needs live ConfigMgr design discussion.
- Never include credentials, real hostnames, site codes, internal URLs, or unsanitized logs.
- Keep the shipping application on .NET Framework 4.8 and Windows PowerShell 5.1 unless repository scope changes explicitly.

## Build and test

From Windows PowerShell:

```powershell
.\tools\test-maintenance.ps1
.\scripts\build.ps1 -Configuration Release
.\scripts\test.ps1 -Configuration Release
.\scripts\test-pester.ps1
```

If a change affects remote client behavior, state the Windows version, ConfigMgr client version, authentication method, and relevant namespace used for validation. Live integration tests read `SCCM_TEST_HOST`, `SCCM_TEST_USERNAME`, and `SCCM_TEST_PASSWORD` from the environment; never commit those values.

## Pull requests

Explain the failure mode, keep compatibility changes narrow, add or update tests when practical, and update `CHANGELOG.md` for user-visible fixes. Do not commit `bin`, `obj`, `artifacts`, PFX files, or locally assembled plugin DLLs.
