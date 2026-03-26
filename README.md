# Client Center for Configuration Manager (Community Fork)

A WPF desktop tool for IT professionals to troubleshoot ConfigMgr/MECM agent issues. Provides a quick overview of client settings, running services, software deployments, update status, and agent configuration with 14 plugin extensions.

**This is a maintained community fork** of [rzander/sccmclictr](https://github.com/rzander/sccmclictr), which was effectively abandoned in 2023. The original maintainer confirmed he has no test environment and no plans to update the project. The tool was removed from the Microsoft Store and winget, and hangs on Windows 25H2.

## What This Fork Changes

- **Open-sourced the automation library.** The closed-source `sccmclictr.automation` NuGet package (the engine behind all WMI/PowerShell communication) has been decompiled, integrated as source, and is now buildable and maintainable. The NuGet dependency is removed from all 17 projects.
- **Removed the SelfUpdate freeze.** The `Plugin_SelfUpdate` plugin called `CheckForUpdateAsync(...).Result` in its constructor on the UI thread, causing the app to hang on startup when the RuckZuck API was unreachable (which it always is now). Plugin removed entirely.
- **Removed Plugin_RuckZuck.** Depended on an external `ruckzuck` repo not included in the project. Non-essential for SCCM administration.
- **Fixed SSL certificate validation.** The original code globally disabled all SSL/TLS certificate validation (`ServerCertificateValidationCallback = delegate { return true; }`), enabling MITM attacks on every HTTPS connection. Replaced with proper TLS 1.2/1.3 enforcement.
- **Fixed broken project references.** The Customization project referenced Roger Zander's personal OneDrive path. Converted to a proper ProjectReference.

## Requirements

### Host Machine (running the tool)
| Component | Minimum Version |
|-----------|----------------|
| OS | Windows 10 1607+ / Windows Server 2016+ |
| .NET Framework | 4.8 |
| PowerShell | 5.1 (Windows PowerShell) |
| Visual C++ Runtime | Not required (managed code only) |

### Target Machine (being managed)
| Component | Minimum Version |
|-----------|----------------|
| WinRM | Enabled and configured (`winrm quickconfig`) |
| PowerShell | 4.0+ (5.1 recommended) |
| ConfigMgr Agent | Any supported version |
| Admin rights | Required on target |

## Building from Source

### Prerequisites
- Visual Studio 2022 (or MSBuild 17+)
- .NET Framework 4.8 targeting pack

### Build Steps
No NuGet restore required -- all dependencies are vendored in `lib/`.
```
# 1. Build the main solution (includes automation library, app, and tests)
msbuild SCCMCliCtrWPF\SCCMCliCtrWPF.sln -p:Configuration=Debug

# 2. Build plugin solutions (14 separate .sln files in Plugins\)
for /d %p in (Plugins\Plugin_*) do msbuild "%p\*.sln" -p:Configuration=Debug -verbosity:quiet

# 3. Copy plugin DLLs into the main project output folder
copy Plugins\*\bin\Debug\Plugin_*.dll SCCMCliCtrWPF\SCCMCliCtrWPF\bin\Debug\
```

Output: `SCCMCliCtrWPF\SCCMCliCtrWPF\bin\Debug\SCCMCliCtrWPF.exe`

## Project Structure

```
sccmclictr\
+-- sccmclictr.automation\         # Decompiled automation library (38 files, 15.6K lines)
|   +-- SCCMAgent.cs               # Entry point: WSMan/PowerShell runspace management
|   +-- ccm.cs                     # Aggregator: wires up all functional modules
|   +-- baseInit.cs                # Base class: cache, PS execution, tracing, disposal
|   +-- WSMan.cs                   # Remote PowerShell runspace creation (Kerberos auth)
|   +-- common.cs                  # Utilities: encryption, SHA1, Base64, WMI datetime
|   +-- functions\                 # 25 WMI class wrappers for SCCM client namespaces
|   |   +-- softwaredistribution.cs  # App deployment engine (126 KB, largest file)
|   |   +-- softwareupdates.cs       # Patch management (58 KB)
|   |   +-- inventory.cs             # Hardware/software inventory (35 KB)
|   |   +-- agentactions.cs          # Client action triggers
|   |   +-- health.cs               # Client health checks
|   |   +-- ...
|   +-- policy\                    # Policy retrieval, decompression, config evaluation
|   |   +-- requestedConfig.cs       # Requested policy (74 KB)
|   |   +-- actualConfig.cs          # Applied configuration (66 KB)
|   +-- schedule\                  # SCCM schedule token encoding/decoding
+-- SCCMCliCtrWPF\                 # Main WPF application
|   +-- SCCMCliCtrWPF\             # WPF project (MainPage.xaml, 20+ UserControls)
|   +-- SCCMCliCtrTests\           # MSTest test project
+-- Customization\                 # Licensing/customization placeholder
+-- Plugins\                       # 14 plugin projects (separate .sln each)
    +-- Plugin_CompMgmt\           # Computer Management
    +-- Plugin_Explorer\           # File Explorer (shares, logs)
    +-- Plugin_PSScripts\          # PowerShell script runner
    +-- Plugin_RDP\                # Remote Desktop
    +-- Plugin_Regedit\            # Remote Registry
    +-- Plugin_EnablePSRemoting\   # WinRM enablement
    +-- Plugin_StatusMessageViewer\ # SCCM status messages
    +-- Plugin_ResourceExplorer\   # Resource Explorer
    +-- Plugin_RemoteTools\        # CM Remote Control
    +-- ...
```

## Known Security Issues

These are inherited from the original project. Some have been fixed; others are documented for awareness.

| Issue | Status | Details |
|-------|--------|---------|
| SSL cert validation globally disabled | **Fixed** | Replaced with TLS 1.2/1.3 enforcement |
| SecureString to plaintext conversion | **Fixed** | `SCCMAgent` no longer stores `string Password`; credentials stored as `PSCredential` only. IPC P/Invoke uses `SecureStringToGlobalAllocUnicode` with immediate zero-free. |
| Unescaped strings in Invoke-Expression | **Fixed** | 4 call sites replaced with direct `& msiexec.exe` invocation |
| 308 bare `catch { }` blocks | Audited | Categorized across 57 files (see `CATCH_BLOCK_AUDIT.md`). 40 silent-ok, 91 debug, 114 surface, 61 unverified. Implementation pending. |
| Saved-password storage removed | **Fixed** | Password is no longer persisted between sessions. `/Password:` command-line argument removed. Previously saved passwords are cleared on first connect. |
| Outdated dependencies (WPFToolkit 2012, NavigationPane 2016) | Open | Both unmaintained; will block .NET 10 migration |

**This tool is designed for use by trusted administrators on internal networks.** It is not suitable for untrusted or internet-facing environments without addressing the open security items above.

## Decompiled Automation Library

The `sccmclictr.automation` project was reverse-engineered from the closed-source `sccmclictrlib` NuGet package v1.0.1 (March 2023) using JetBrains dotPeek. Key facts:

- The NuGet v1.0.1 is **5 years newer** than the last public source on the `rzander/sccmclictrlib` GitHub repo (v1.0.0.32, Sept 2018). It contains all unpublished changes from 2018-2023.
- **76 decompiler-generated variable names** remain across 4 files (`softwareupdates.cs`, `ScheduleDecoding.cs`, `softwaredistribution.cs`, `agentactions.cs`). These are cosmetic — the code compiles and functions correctly.
- Zero external NuGet dependencies. All references are .NET Framework system assemblies.
- `LangVersion=latest` set in the `.csproj` to accommodate decompiler output (file-scoped namespaces, nullable annotations).

## WMI Namespaces Referenced

For reference when building SCCM client tooling:

| Namespace | Purpose |
|-----------|---------|
| `root\ccm\clientsdk` | Client-side app deployment, install/uninstall triggers |
| `root\ccm\SoftMgmtAgent` | Software distribution execution state |
| `root\ccm\Policy\Machine\ActualConfig` | Applied client policy and variables |
| `root\ccm\Scheduler` | Schedule history |
| `root\ccm\Events` | Real-time event watching |
| `root\cimv2` | Standard Windows classes (OS, disk, BIOS, network) |
| `root\sms` | Site provider discovery (admin server) |

## Roadmap

### Phase 1: Stabilization (complete)
- [x] Integrate decompiled automation library as source
- [x] Remove dead plugins (RuckZuck, SelfUpdate)
- [x] Fix SSL certificate validation
- [x] Fix broken project references
- [x] Harden credential handling (remove plaintext password storage, SecureString marshaling for IPC)
- [x] Remove `Invoke-Expression` command injection surface (4 call sites)
- [x] Remove saved-password storage and `/Password:` command-line argument
- [x] Rename decompiler-generated variables (41 across 2 files)
- [x] Audit bare `catch { }` blocks (308 across 57 files — see `CATCH_BLOCK_AUDIT.md`)

### Phase 2: CIM Migration (in progress, `cim-migration` branch)
Replace deprecated `Get-WmiObject` / `[wmi]` / `[wmiclass]` with CIM cmdlets (`Get-CimInstance`, `Invoke-CimMethod`, `Set-CimInstance`). Runs on .NET Framework 4.8 — no .NET 10 required.

| Sub-phase | Status | Scope |
|-----------|--------|-------|
| Phase 1: `Get-WmiObject`/`gwmi` string replacement | **Complete** | 8 files, ~20 replacements |
| Phase 2a: Property reads/writes (`GetProperty`, `SetProperty`) | **Complete** | `baseInit.cs` + 3 function files |
| Phase 2b: No-arg method calls (`GetStringFromClassMethod`) | **Complete** | `baseInit.cs` |
| Phase 2c: Parameterized method calls (`CallClassMethod`) | Pending | 6 remaining `[wmi]`/`[wmiclass]` references |
| Phase 3: Replace `ManagementDateTimeConverter` (C# side) | Pending | ~40 call sites |
| Phase 4: Remove `System.Management` reference | Pending | Cleanup |

Validated with Pester tests at each phase (30/30 passing). See `CIM_MIGRATION_PLAN.md` on the `cim-migration` branch for details.

### Phase 3: Catch Block Implementation
Apply the categorized fixes from `CATCH_BLOCK_AUDIT.md`:

| Category | Count | Action |
|----------|-------|--------|
| SILENT-OK | 40 | Leave as-is — defensive probes, dispose, cleanup |
| DEBUG | 91 | Add `Debug.WriteLine` (visible with debugger only) |
| SURFACE | 114 | Add `Listener?.WriteError` / trace (visible in UI) |
| UNVERIFIED | 61 | Manual review needed |

### Phase 4: .NET 10 Migration
Target: .NET 10 LTS (EOL November 2028). Blocked by NavigationPane and WPFToolkit replacement.

| Blocker | Issue |
|---------|-------|
| NavigationPane 2.1 | Dead .NET 3.5 project, no .NET 10 equivalent — must be replaced |
| WPFToolkit 3.5 | Most controls now built into WPF — remove package, use built-in |
| ClickOnce | Not supported on .NET 10 — replace with MSIX or self-contained publish |
| Plugin architecture | All 14 plugins must be recompiled for .NET 10 |
| PowerShell hosting | Shifts from 5.1 to 7.x (remote WSMan to 5.1 endpoints still works) |

## License

[MS-PL (Microsoft Public License)](LICENSE.md) -- inherited from the original project.

## Credits

- Original project by [Roger Zander](https://github.com/rzander/sccmclictr)
- Decompilation and fork maintenance by the community
