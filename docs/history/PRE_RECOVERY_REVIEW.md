# Historical review — superseded 2026-09-09

This record is not the current architecture or assurance statement. Its claims
that the library was closed-source and migration was complete were incorrect.
Roger's public source includes a Framework 4.8 project and assembly version
1.0.1.0. See ../../CODE_REVIEW.md for current status.

# Code Review & Technical Reference

## Fork Details
- **Upstream**: https://github.com/rzander/sccmclictr
- **Fork**: https://github.com/jasonulbright/sccmclictr
- **Fork base**: Latest upstream commit (includes all 27 commits through v1.0.7.2)
- **Current version**: v1.3.0.1

The original project is no longer actively developed. Its maintainer has noted that he no longer has a ConfigMgr test environment and does not plan a broad refactor away from the legacy WMI compatibility paths. This fork therefore focuses on conservative maintenance, supported Windows compatibility, and dependable distribution rather than feature expansion.

## Architecture
- WPF app, .NET Framework 4.8, 14 plugins (2 removed: RuckZuck, SelfUpdate)
- `sccmclictr.automation` library (formerly closed-source NuGet) integrated as source -- 38 C# files, ~15,600 lines
- All WMI access is PS-mediated: PowerShell commands sent through a remote WinRM runspace. Zero direct C# WMI calls to remote machines.
- WinRM on port 5985 (HTTP) or 5986 (HTTPS)
- Plugin architecture via dynamic assembly loading
- Reproducible GitHub Actions build with Authenticode-signed Inno Setup and portable releases

## Security

### Fixed
| # | Issue | Version | Details |
|---|-------|---------|---------|
| 1 | SSL cert validation globally disabled | v1.1.0 | `ServerCertificateValidationCallback = delegate { return true; }` replaced with TLS 1.2/1.3 enforcement |
| 2 | Credential handling | v1.1.0 | Removed `string Password` property. Credentials stored as `PSCredential` only. IPC P/Invoke uses `SecureStringToGlobalAllocUnicode` with immediate `ZeroFreeGlobalAllocUnicode` cleanup. |
| 3 | ConnectIPC plaintext password | v1.1.0 | Signature changed to `ConnectIPC(string, SecureString)` and `ConnectIPC(PSCredential)`. |
| 4 | Invoke-Expression code injection | v1.1.0 | 4 call sites in `inventory.cs` and `agentactions.cs` replaced with direct `& msiexec.exe` invocation. |
| 5 | Saved-password persistence | v1.1.0 | Password no longer persisted to settings. `/Password:` command-line argument removed (was visible in process listings). Previously saved passwords cleared on first connect. |

### Open
| # | Issue | Status | Details |
|---|-------|--------|---------|
| 1 | 307 bare `catch { }` blocks | Audited | The inherited blocks are categorized and protected by a no-growth regression baseline. See `CATCH_BLOCK_AUDIT.md`. |
| 2 | Outdated vendored dependencies | Accepted | NavigationPane (2016) and WPFToolkit (2012) are dead projects. Vendored in `lib/`. Functionally stable on .NET Framework 4.8, no known CVEs. Replacement would require UI rework — not justified for a maintenance fork. |

## Dependencies (all vendored -- zero NuGet)

All external dependencies are vendored in `lib/`. No `nuget restore` required.

| Dependency | Version | Location | Notes |
|-----------|---------|----------|-------|
| NavigationPane | 2.1.0 | `lib/NavigationPane/` | Dead project (.NET 3.5 era). Functionally stable on 4.8, no CVEs. |
| WPFToolkit | 3.5.50211.1 | `lib/WPFToolkit/` | 3 DLLs. Dead since 2012. Functionally stable on 4.8, no CVEs. |
| MSTest | 1.2.0 | `lib/MSTest/` | DLLs + build props/targets. Test infrastructure only. |
| System.Management.Automation | 3.0.0 | GAC | PS 3.0 era. Ships with Windows. |

## Decompiled Automation Library

The `sccmclictr.automation` project was reverse-engineered from the closed-source `sccmclictrlib` NuGet package v1.0.1 (March 2023) using JetBrains dotPeek.

- NuGet v1.0.1 is **5 years newer** than the last public source (v1.0.0.32, Sept 2018). Contains all unpublished changes from 2018-2023.
- 3 decompiler artifacts fixed (invalid ref syntax, unassigned locals).
- 41 decompiler-generated variable names cleaned up across 2 files.
- Zero external NuGet dependencies. All references are .NET Framework system assemblies.

## Upstream Fixes (already included)

The fork base includes all upstream commits through v1.0.7.2:

| Fix | Commit | Notes |
|-----|--------|-------|
| Keyboard debounce (100ms Enter guard) | `3b5ae59` | |
| PSScripts UI freeze (DoEvents + Sleep) | `e72f9e0` | |
| IPC credential fix | `3b5ae59` | Further hardened in v1.1.0 |
| Log access buttons (execmgr/smsts) | `d191bc1` | |
| System lock action | `d556efa` | |
| AppImport removed | `4677c80` | |

## Upstream Issues (60 open on rzander/sccmclictr)

| Theme | Issues | Fork Status |
|-------|--------|-------------|
| SSL/HTTPS | #199, #200, #203, #212 | Partially addressed (TLS fix) |
| WMI deprecation | #58, #208 | Substantially addressed in the core helpers; compatibility paths and optional scripts remain |
| User-targeted deployments invisible | #82 (26 comments) | Open -- queries only IsMachineTarget |
| Application Groups unsupported | #144, #207, #183 | Open |
| Connectivity (Surface, VPN, proxy) | Various | Open |
| Resource leak / high CPU | #80 | Open |

## WMI Namespaces Referenced

| Namespace | Purpose |
|-----------|---------|
| `root\cimv2` | Standard Windows classes (OS, disk, BIOS, network) |
| `root\ccm\clientsdk` | Client-side app deployment, install/uninstall triggers |
| `root\ccm\SoftMgmtAgent` | Software distribution execution state |
| `root\ccm\Policy\Machine\ActualConfig` | Applied client policy and variables |
| `root\ccm\Scheduler` | Schedule history |
| `root\ccm\Events` | Real-time event watching |
| `root\sms` | Site provider discovery (admin server) |
| `root\wmi` | WMI operational namespace |

## CIM Migration -- SUBSTANTIAL, NOT GLOBAL

Version 1.2.0 migrated the central automation helpers and many primary client operations to CIM. The `sccmclictr.automation` project no longer has a compile-time `System.Management` reference; it continues to host Windows PowerShell through `System.Management.Automation`.

The earlier review overstated this as a repository-wide completion. Remaining PowerShell-mediated WMI compatibility paths include process-owner lookup, service-window creation, policy instance creation, embedded health/cache scripts, application settings, and the optional script library. These paths still work under the supported Windows PowerShell 5.1 runtime and should be migrated only with ConfigMgr integration coverage.

**Key finding**: All WMI access was PS-mediated (PowerShell commands through remote runspace). Zero direct C# WMI calls to remote machines.

| Phase | Status | Scope |
|-------|--------|-------|
| 1: Primary `Get-WmiObject`/`gwmi` replacement | **Mostly complete** | Central helpers and common queries migrated; compatibility paths remain |
| 2a: Property reads/writes | **Complete** | `baseInit.cs` + 3 function files |
| 2b: No-arg method calls | **Complete** | `baseInit.cs` |
| 2c: Parameterized method calls | **Complete** | Dynamic param discovery via `Get-CimClass` |
| 3: Replace `ManagementDateTimeConverter` | **Complete** | Custom `DmtfToDateTime` parser, 51 call sites |
| 4: Remove `System.Management` from automation compile references | **Complete** | Removed from the automation project; the main app and some plugins retain it for compatibility |

43 Pester tests (30 unit + 13 integration against live CM 2509). See `CIM_MIGRATION_PLAN.md`.

**Phase 5 (future)**: Replace PS-string architecture with native C# `CimSession` API. Currently all WMI/CIM operations are PowerShell command strings sent through a remote runspace. Direct C# `CimSession` calls would be faster and type-safe. Natural fit for .NET 10 migration.

## Project Structure

```
sccmclictr\
+-- sccmclictr.automation\         # Automation library (38 files, 15.6K lines)
|   +-- SCCMAgent.cs               # Entry: WSMan/PowerShell runspace management
|   +-- ccm.cs                     # Aggregator: wires up all functional modules
|   +-- baseInit.cs                # Base: cache, PS execution, tracing, WMI path parsing
|   +-- WSMan.cs                   # Remote runspace creation (Kerberos auth)
|   +-- common.cs                  # Utilities: encryption, SHA1, Base64, WMI datetime
|   +-- functions\                 # 25 WMI class wrappers for SCCM client namespaces
|   +-- policy\                    # Policy retrieval, decompression, config evaluation
|   +-- schedule\                  # SCCM schedule token encoding/decoding
+-- SCCMCliCtrWPF\                 # Main WPF application + MSTest project
+-- Customization\                 # Licensing/customization placeholder
+-- Plugins\                       # 14 plugin projects (separate .sln each)
+-- lib\                           # Vendored dependencies (NavigationPane, WPFToolkit, MSTest)
+-- Tests\                         # Pester tests for CIM migration validation
```

## Roadmap

### Phase 1: Stabilization -- COMPLETE
- [x] Integrate decompiled automation library as source
- [x] Remove dead plugins (RuckZuck, SelfUpdate)
- [x] Fix SSL, credentials, Invoke-Expression, saved passwords
- [x] Vendor all external dependencies (zero NuGet)
- [x] Clean decompiler variables (41 across 2 files)
- [x] Audit catch blocks (307 across 56 files; no-growth guard enabled)

### Phase 2: CIM Migration -- MAJORITY COMPLETE (v1.2.0+)
The central automation layer is migrated. Remaining compatibility paths are listed in the CIM Migration section above and require live ConfigMgr validation.

### Phase 3: Catch Block Implementation (optional)
Apply fixes from `CATCH_BLOCK_AUDIT.md`: 40 silent-ok (leave), 91 debug (`Debug.WriteLine`), 114 surface (`Listener?.WriteError`), 61 unverified (manual review). Low priority — cosmetic improvement, not a functional issue.

### Phase 4: .NET 10 Migration (non-shipping experiment)
Would require UI rework (NavigationPane, WPFToolkit replacements), ClickOnce to MSIX, 14 plugin recompiles, and PS 5.1 to 7.x hosting migration. .NET Framework 4.8 is supported through 2032+ and ships with Windows. This fork's value is "it works when the original doesn't" — a full rewrite is not justified unless there's a compelling functional reason.

## Build and Release Review (v1.3.0.1)

The former AppVeyor file targeted Visual Studio 2017, built only the main solution, did not assemble current plugin outputs, and could not publish a complete release. Active project files also contained Roger's retired local SignTool path and certificate identity, while plugin Release builds referenced `bin\Debug` dependencies.

Version 1.3.0.1 replaces that path with:

- `scripts/build.ps1` for repeatable main-app and 14-plugin Release builds and staging.
- Explicit runtime-file validation so missing or stale plugin DLLs cannot be silently packaged.
- An Inno Setup installer plus a portable ZIP with the same signed payload.
- SHA-256 checksums, GitHub artifact attestations, and a tag-driven GitHub release.
- Mandatory Authenticode signing through repository secrets; unsigned tag releases fail before publication.

See `docs/RELEASING.md` for certificate setup and the release checklist.
