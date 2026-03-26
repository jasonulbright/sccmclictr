# Code Review & Technical Reference

## Fork Details
- **Upstream**: https://github.com/rzander/sccmclictr
- **Fork**: https://github.com/jasonulbright/sccmclictr
- **Fork base**: Latest upstream commit (includes all 27 commits through v1.0.7.2)
- **Current version**: v1.2.0

Original project effectively abandoned. Maintainer (rzander) stated in Jan 2026: "I currently don't have access to any test environments" and "there are no plans to refactor ClientCenter... In worst case it will die with Get-WMIObject." Removed from Microsoft Store and winget. Hangs on Windows 25H2.

## Architecture
- WPF app, .NET Framework 4.8, 14 plugins (2 removed: RuckZuck, SelfUpdate)
- `sccmclictr.automation` library (formerly closed-source NuGet) integrated as source -- 38 C# files, ~15,600 lines
- All WMI access is PS-mediated: PowerShell commands sent through a remote WinRM runspace. Zero direct C# WMI calls to remote machines.
- WinRM on port 5985 (HTTP) or 5986 (HTTPS)
- Plugin architecture via dynamic assembly loading

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
| 1 | 308 bare `catch { }` blocks | Audited | Categorized across 57 files. 40 SILENT-OK, 91 DEBUG, 114 SURFACE, 61 UNVERIFIED. See `CATCH_BLOCK_AUDIT.md`. |
| 2 | Outdated vendored dependencies | Open | NavigationPane (2016) and WPFToolkit (2012) are dead projects. Vendored in `lib/`. Replacement planned for .NET 10 migration. |

## Dependencies (all vendored -- zero NuGet)

All external dependencies are vendored in `lib/`. No `nuget restore` required.

| Dependency | Version | Location | Notes |
|-----------|---------|----------|-------|
| NavigationPane | 2.1.0 | `lib/NavigationPane/` | Dead project (.NET 3.5 era). Replace in .NET 10 migration. |
| WPFToolkit | 3.5.50211.1 | `lib/WPFToolkit/` | 3 DLLs. Dead since 2012. Most controls built into modern WPF. |
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
| WMI deprecation | #58, #208 | CIM migration in progress |
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

## CIM Migration -- COMPLETE (v1.2.0)

All deprecated `System.Management` / WMI usage removed. `System.Management` assembly reference removed from csproj. Only `System.Management.Automation` (PowerShell runspace) remains.

**Key finding**: All WMI access was PS-mediated (PowerShell commands through remote runspace). Zero direct C# WMI calls to remote machines.

| Phase | Status | Scope |
|-------|--------|-------|
| 1: `Get-WmiObject`/`gwmi` string replacement | **Complete** | 8 files, ~20 replacements |
| 2a: Property reads/writes | **Complete** | `baseInit.cs` + 3 function files |
| 2b: No-arg method calls | **Complete** | `baseInit.cs` |
| 2c: Parameterized method calls | **Complete** | Dynamic param discovery via `Get-CimClass` |
| 3: Replace `ManagementDateTimeConverter` | **Complete** | Custom `DmtfToDateTime` parser, 51 call sites |
| 4: Remove `System.Management` reference | **Complete** | Removed from csproj, all `using` statements removed |

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
- [x] Audit catch blocks (308 across 57 files)

### Phase 2: CIM Migration -- COMPLETE (v1.2.0)
`System.Management` dependency removed. See CIM Migration section above.

### Phase 3: Catch Block Implementation
Apply fixes from `CATCH_BLOCK_AUDIT.md`: 40 silent-ok (leave), 91 debug (`Debug.WriteLine`), 114 surface (`Listener?.WriteError`), 61 unverified (manual review).

### Phase 4: .NET 10 Migration
Target: .NET 10 LTS (EOL November 2028).

| Blocker | Resolution |
|---------|------------|
| NavigationPane 2.1 | Replace (dead .NET 3.5 project) |
| WPFToolkit 3.5 | Replace with built-in WPF controls |
| ClickOnce | Replace with MSIX or self-contained publish |
| 14 plugins | Recompile for .NET 10 |
| PowerShell hosting | 5.1 to 7.x (remote WSMan to 5.1 still works) |

Supported as-is: WPF, System.Management (via Windows Compatibility Pack), COM interop, P/Invoke.
