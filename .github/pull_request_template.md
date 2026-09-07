## Summary

Describe the maintenance problem and the change.

## Validation

- [ ] `tools/test-maintenance.ps1`
- [ ] `scripts/build.ps1 -Configuration Release`
- [ ] `scripts/test.ps1 -Configuration Release`
- [ ] `scripts/test-pester.ps1`
- [ ] Tested against a ConfigMgr client where behavior changed

## Compatibility and security

- [ ] Preserves .NET Framework 4.8 and Windows PowerShell 5.1 compatibility
- [ ] Does not add credentials, environment names, private endpoints, or build artifacts
- [ ] Updates README/CHANGELOG when operator behavior changes
