# Changelog

## v1.2.0 -- CIM Migration: System.Management Removed (2026-03-26)

### CIM Migration (complete)
All deprecated `System.Management` / WMI usage removed from the automation library. The `System.Management` assembly reference is no longer needed.

- **Get-WmiObject/gwmi** replaced with `Get-CimInstance` across 8 files (~20 call sites)
- **[wmi]/[wmiclass]** PS accelerators replaced with `Get-CimInstance`, `Invoke-CimMethod`, `Set-CimInstance` in all `baseInit.cs` helper methods and 6 direct call sites in function files
- **ManagementDateTimeConverter.ToDateTime()** replaced with custom `common.DmtfToDateTime()` parser across 51 call sites in 9 files
- **System.Management** assembly reference removed from `sccmclictr.automation.csproj`
- **Dynamic parameter discovery**: `CallClassMethod`/`CallInstanceMethod` use `Get-CimClass` at runtime to map positional arguments to `Invoke-CimMethod -Arguments` hashtable
- **43 Pester tests**: 30 unit tests + 13 integration tests against live CM 2509 (site code MCM). All pass.
- See `CIM_MIGRATION_PLAN.md` for full technical details.

---

## v1.1.1 -- Vendor Dependencies & Catch Block Audit (2026-03-25)

### Dependencies
- **All NuGet dependencies vendored to `lib/`** — NavigationPane, WPFToolkit (3 DLLs), and MSTest (adapter + framework) are now referenced via direct HintPath. `packages.config` files removed. No `nuget restore` required. The project is fully self-contained with zero external package manager dependencies.

### Documentation
- **`CATCH_BLOCK_AUDIT.md`** — comprehensive audit of all 308 bare `catch { }` blocks across 57 files. Categorized as SILENT-OK (40), DEBUG (91), SURFACE (114), UNVERIFIED (61). Implementation pending.
- **Roadmap updated** — Phase 1 (stabilization) complete. CIM migration (Phase 2) in progress on `cim-migration` branch with sub-phase tracking. Catch block implementation (Phase 3) and .NET 10 migration (Phase 4) documented.
- **CODE_REVIEW.md updated** — dependency table, CIM migration status, corrected catch block counts.
- **Build instructions simplified** — single `msbuild` command, no NuGet step.

---

## v1.1.0 -- Community Fork: Source Integration & Security Fixes (2026-03-25)

### Breaking Changes
- **Plugin_SelfUpdate removed.** Called `CheckForUpdateAsync(...).Result` synchronously in its constructor, deadlocking the UI thread when the RuckZuck API was unreachable. This was the root cause of the startup freeze reported by multiple users. The plugin depended on the external `ruckzuck` repo which is not part of this project.
- **Plugin_RuckZuck removed.** Same external `ruckzuck` dependency. Non-essential for SCCM administration.
- **SSL certificate validation re-enabled.** The original code globally disabled all SSL/TLS certificate validation via `ServerCertificateValidationCallback = delegate { return true; }` and `CheckCertificateRevocationList = false`. This accepted any certificate including fraudulent ones, enabling man-in-the-middle attacks on all HTTPS connections (including WinRM over HTTPS). Replaced with TLS 1.2/1.3 enforcement. If you connect to endpoints with self-signed certificates, you may need to add them to the trusted certificate store.

### Automation Library (sccmclictr.automation)
The closed-source `sccmclictrlib` NuGet package v1.0.1 has been decompiled via JetBrains dotPeek and integrated as a source project. The NuGet dependency has been removed from all 17 projects (main app + 16 plugins).

- 38 C# source files, ~15,600 lines of code
- .NET Framework 4.8, zero external NuGet dependencies
- `LangVersion=latest` to support decompiler output (file-scoped namespaces, nullable annotations)
- 3 decompiler artifacts fixed:
  - `SCCMAgent.cs`: `ref new NETRESOURCE()` -- extracted to local variable
  - `ScheduleDecoding.cs`: `DateTime& local3` (invalid C# ref syntax) -- rewritten as clean DateTime assignments
  - `ScheduleDecoding.cs`: unassigned locals via `ref DateTime` indirection -- simplified to direct initialization
- All decompiler-generated variable names cleaned up: 41 renames across `softwaredistribution.cs` (38) and `softwareupdates.cs` (3). `ScheduleDecoding.cs` and `agentactions.cs` were already clean.

### Project Reference Fixes
- **Customization.dll**: was referencing `C:\Users\RogerZander\OneDrive\Dokumente\...`. Converted to a `ProjectReference` within the solution.
- **sccmclictr.automation**: all 17 projects converted from NuGet `PackageReference` to either `ProjectReference` (main app) or local `HintPath` (plugins).
- **Plugin_Explorer**: Customization HintPath pointed to `Release` build; fixed to `Debug`.

### Security
- **SSL/TLS**: Removed global `ServerCertificateValidationCallback` override. Added `SecurityProtocol = Tls12 | Tls13`.
- **Credential handling hardened**: Removed persistent `string Password` property from `SCCMAgent`. Credentials now stored only as `PSCredential`. `ConnectIPC()` signature changed from `(string, string)` to `(string, SecureString)` and `(PSCredential)`. IPC P/Invoke uses `Marshal.SecureStringToGlobalAllocUnicode` with immediate `ZeroFreeGlobalAllocUnicode` cleanup.
- **Invoke-Expression removed**: 4 call sites in `inventory.cs` and `agentactions.cs` replaced with direct `& msiexec.exe` invocation, eliminating code injection surface.
- **Saved-password storage removed**: Password is no longer persisted to settings between sessions. `/Password:` command-line argument removed (plaintext visible in process listings). Previously saved passwords are cleared on first connect. Use integrated auth (launch elevated) or type credentials each session.
- **Remaining**: 308 bare `catch { }` blocks audited in v1.1.1 (see `CATCH_BLOCK_AUDIT.md`).

### Build
- 14 of 14 remaining plugins build successfully
- Main solution builds: `SCCMCliCtrWPF.exe` + `sccmclictr.automation.dll` + `Customization.dll` + all plugin DLLs
- NuGet dependencies vendored to `lib/` in v1.1.1 (no `nuget restore` required)
- ClickOnce publish warnings remain (plugin DLLs listed as Content items) -- harmless, does not affect Debug builds

---

## v1.0.6.1 -- Last Upstream Release (2022)

Final release from [rzander/sccmclictr](https://github.com/rzander/sccmclictr). Includes:
- .NET Framework 4.8 target
- Keyboard debounce fix (100ms guard on Enter key connect)
- PSScripts UI freeze fix (DoEvents + Sleep pattern)
- IPC credential handling fix (sccmclictr.automation v1.0.1)
- 17 plugins (including RuckZuck and SelfUpdate)
- Closed-source `sccmclictrlib` v1.0.1 NuGet dependency

### Known Issues at Time of Fork
- **Startup freeze**: Plugin_SelfUpdate blocks UI thread with synchronous HTTP call to defunct RuckZuck API
- **Windows 25H2 hang**: Reported by multiple users, no fix from upstream
- **SSL globally disabled**: MITM vulnerability on all HTTPS connections
- **107+ silent catch blocks**: All errors suppressed without logging
- **User-targeted deployments invisible**: WMI queries only check `IsMachineTarget` (upstream issue #82, 26 comments)
- **High CPU over time**: Resource leak reported in upstream issue #80
- **Removed from Microsoft Store and winget**: No longer distributed by original author
