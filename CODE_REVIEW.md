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
Release 1.3.0.2 contains this recovery; v1.3.0.1 contains the earlier implementation.
Published first-party binaries are signed by the release pipeline; ordinary local
builds are unsigned.

## Validation boundaries

- Local recovery validation: all 14 plugins and the application built successfully;
  12 offline Pester tests and 2 MSTest tests passed. The 2 live parity tests were
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
- User-observed live validation on CLIENT01 (2026-09-09): restored update lists
  populated; right-click installation of .NET update KB5126052 progressed through
  Downloading to PendingSoftReboot with error 0. The app's forced-restart action
  rebooted the client; after reconnect/refresh the update displayed Installed.
  This validates that update/restart path, not every administrative feature.
- The maintainer reports completing all manual checks available from the workstation.
  This is manual validation, not a completed automated feature-by-feature parity run.
  Log access and additional verification must be tested inside the target domain;
  cross-domain credential attempts did not establish a library or UI defect.
- Bulk update-install actions and individual policy triggers have no separately
  recorded results. Automated live tests remain uncompleted; the install and restart
  above were user-run.

The maintainer authorized release 1.3.0.2 after the manual checks above. Release
notes must retain these boundaries; compilation and skipped integration tests
alone do not establish full functional parity.

The [old review](docs/history/PRE_RECOVERY_REVIEW.md) is retained solely as history;
its source-availability and completed-migration claims are superseded.
