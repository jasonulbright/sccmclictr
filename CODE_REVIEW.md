# Maintenance recovery: architecture and validation

## Current source

The WPF application and 14 plugins target .NET Framework 4.8. The automation
library is compiled from Roger Zander's original public source, pinned at
`1c875c00ab04144741247873cea1b69cb25ef1ea`, with narrow maintenance patches.
It is not downloaded as a precompiled NuGet dependency. See
[source provenance](sccmclictr.automation/UPSTREAM.md) for the local changes.

Original model queries use Get-WmiObject and WMI method calls inside a remote
Windows PowerShell runspace over WinRM. System.Management is intentional.
The original separate GetCimObjects helper remains for callers such as the WMI
browser; its results must not be passed indiscriminately to WMI model constructors.

## Why recovery was necessary

The prior fork reconstructed a NuGet DLL and broadly changed WMI operations to
CIM without preserving the full result/model contract. Missing WMI metadata and
native CIM date types caused update-model regressions. Synthetic repairs did not
establish full live parity. The claim that no current original source existed
was incorrect. The old state is preserved on `codex/pre-recovery-8014407`.

We are not the only maintained community fork. [drummachine24's fork](https://github.com/drummachine24/sccmclictr)
also builds from original library source, on .NET 10, and contains useful fixes.
Our scope remains conservative Framework 4.8 maintenance and runtime servicing
aligned with the supported host OS lifecycle, not a feature/runtime migration.

## Security and distribution

Application-side TLS validation, removal of saved/command-line passwords, retired
self-update removal, signing and installer/portable infrastructure are retained.
Library changes retain DPAPI compatibility, remove unused TripleDES, harden IPC
password marshalling, validate MSI product codes, and avoid Invoke-Expression.
This is not a complete security audit. Upstream error-swallowing and legacy
administrative operations still require feature-by-feature assessment. The
bare-catch baseline is a regression guard, not a statement that catches are safe.

The application is MS-PL; the automation library is LGPL-3.0-or-later. Recovery
packages include corresponding library source and license texts in library-source.
No release has been published from this recovery. Public v1.3.0.1 binaries still
contain the earlier implementation; local recovery binaries are unsigned.

## Validation boundaries

- Local recovery validation: all 14 plugins and the application built successfully;
  10 offline Pester tests and 2 MSTest tests passed. The 2 live parity tests were
  skipped because no authenticated test session was supplied. Original-source
  compiler warnings remain; this is not a warning-free or complete audit.
- Build checks cover the main application and all 14 plugins against the restored API.
- Recovery Pester tests exercise compiled update models with serialized WMI-shaped
  data, real local WMI metadata, DPAPI, query errors, refresh caching, source
  packaging, and mandatory-install command construction without executing it.
- Read-only live tests compare pending/available identities from the compiled
  library with direct queries against the same client. Set SCCM_TEST_HOST; use
  integrated authentication or optional environment-provided credentials.
- Roger's installed 1.0.6.1 was shown displaying two pending updates and populated
  All Updates on CLIENT01 after the lab synchronization issue was fixed.
- The restored build has **not yet been validated live** against CLIENT01.
  Update installation, policy actions, and other remote writes remain unverified.
  No live update installation or reboot is performed by these tests.

Do not publish the recovery as functionally restored until live read/action parity
has been demonstrated. Compilation and skipped integration tests are insufficient.

The [old review](docs/history/PRE_RECOVERY_REVIEW.md) is retained solely as history;
its source-availability and completed-migration claims are superseded.
