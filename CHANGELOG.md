# Changelog

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
- 76 decompiler-generated variable names remain across 4 files (cosmetic, does not affect functionality):
  - `softwareupdates.cs` -- 39 instances (`num1`-`num19` in state-checking methods)
  - `ScheduleDecoding.cs` -- 22 instances (`num1`-`num16` in bit-packing methods)
  - `softwaredistribution.cs` -- 9 instances (`str1`-`str3` in XML/string operations)
  - `agentactions.cs` -- 6 instances (mixed `num`/`str` in service operations)

### Project Reference Fixes
- **Customization.dll**: was referencing `C:\Users\RogerZander\OneDrive\Dokumente\...`. Converted to a `ProjectReference` within the solution.
- **sccmclictr.automation**: all 17 projects converted from NuGet `PackageReference` to either `ProjectReference` (main app) or local `HintPath` (plugins).
- **Plugin_Explorer**: Customization HintPath pointed to `Release` build; fixed to `Debug`.

### Security
- **SSL/TLS**: Removed global `ServerCertificateValidationCallback` override. Added `SecurityProtocol = Tls12 | Tls13`.
- **Credential handling hardened**: Removed persistent `string Password` property from `SCCMAgent`. Credentials now stored only as `PSCredential`. `ConnectIPC()` signature changed from `(string, string)` to `(string, SecureString)` and `(PSCredential)`. IPC P/Invoke uses `Marshal.SecureStringToGlobalAllocUnicode` with immediate `ZeroFreeGlobalAllocUnicode` cleanup.
- **Invoke-Expression removed**: 4 call sites in `inventory.cs` and `agentactions.cs` replaced with direct `& msiexec.exe` invocation, eliminating code injection surface.
- **Remaining**: 238 bare `catch { }` blocks (mostly intentional defensive probes), weak saved-password encryption in UI layer (SHA1 + assembly name key).

### Build
- All NuGet packages restored from nuget.org (WPFToolkit, NavigationPane, MSTest)
- 14 of 14 remaining plugins build successfully
- Main solution builds: `SCCMCliCtrWPF.exe` + `sccmclictr.automation.dll` + `Customization.dll` + all plugin DLLs
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
