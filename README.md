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
```
# 1. Restore NuGet packages (WPFToolkit, NavigationPane, MSTest)
nuget restore SCCMCliCtrWPF\SCCMCliCtrWPF.sln

# 2. Build the automation library (produces the DLL plugins reference)
msbuild sccmclictr.automation\sccmclictr.automation.csproj -p:Configuration=Debug

# 3. Build plugin solutions (14 separate .sln files in Plugins\)
for /d %p in (Plugins\Plugin_*) do msbuild "%p\*.sln" -p:Configuration=Debug -verbosity:quiet

# 4. Copy plugin DLLs into the main project folder
copy Plugins\*\bin\Debug\Plugin_*.dll SCCMCliCtrWPF\SCCMCliCtrWPF\

# 5. Build the main solution
msbuild SCCMCliCtrWPF\SCCMCliCtrWPF.sln -p:Configuration=Debug
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
| 238 bare `catch { }` blocks | Open | Silent exception swallowing across 55 files. Most are intentional defensive probes against remote clients with varying configurations. Needs categorized review. |
| Weak saved-password encryption (SHA1 + assembly name as key) | Open | UI layer (`common.cs`) uses assembly name as encryption key for saved credentials |
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

### Phase 1: Stabilization (current)
- [x] Integrate decompiled automation library as source
- [x] Remove dead plugins (RuckZuck, SelfUpdate)
- [x] Fix SSL certificate validation
- [x] Fix broken project references
- [x] Harden credential handling (remove plaintext password storage, SecureString marshaling for IPC)
- [x] Remove `Invoke-Expression` command injection surface (4 call sites)
- [x] Rename decompiler-generated variables (41 across 2 files)
- [ ] Categorize bare `catch { }` blocks (238 across 55 files)

### Phase 2: .NET 10 Migration
Target: .NET 10 LTS (supported through November 2028).

| Component | Migration Path |
|-----------|---------------|
| WPF | Supported on .NET 10 (`<UseWPF>true</UseWPF>`) |
| System.Management (WMI) | NuGet package `System.Management` v10.x |
| System.Management.Automation (PS) | NuGet `Microsoft.PowerShell.SDK` 7.6+ (hosts PS 7.x, remote WSMan to PS 5.1 still works) |
| COM Interop (CPAPPLET) | Fully supported on .NET 10 |
| P/Invoke (mpr.dll) | Works unchanged |
| System.Runtime.Caching | NuGet package v10.x |
| TripleDES/SHA1 | Built-in; use `TripleDES.Create()` / `SHA1.Create()` factory methods |
| NavigationPane 2.1.0 | **Must be replaced** (dead project, .NET 3.5 era) |
| WPFToolkit 3.5 | **Must be replaced** (most controls now built into WPF) |
| ClickOnce | Not supported on .NET 10; replace with MSIX or self-contained publish |
| Plugin architecture | All plugins must be recompiled for .NET 10 (cannot load .NET Framework DLLs) |

**Key constraint**: .NET 10 hosts PowerShell 7.x, not Windows PowerShell 5.1. Remote WSMan operations to PS 5.1 endpoints still work (protocol-level), but locally-executed scripts run under PS 7.x.

## License

[MS-PL (Microsoft Public License)](LICENSE.md) -- inherited from the original project.

## Credits

- Original project by [Roger Zander](https://github.com/rzander/sccmclictr)
- Decompilation and fork maintenance by the community
