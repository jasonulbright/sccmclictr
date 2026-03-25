# SCCMCliCtr Code Review — 2026-03-25

## Fork Details
- **Upstream**: https://github.com/rzander/sccmclictr
- **Fork**: https://github.com/jasonulbright/sccmclictr
- **Fork base**: Latest upstream commit (includes all changes through v1.0.7.2)

## Project Status
Original project effectively abandoned. Maintainer (rzander) stated in Jan 2026:
- "I currently don't have access to any test environments"
- "there are no plans to refactor ClientCenter... In worst case it will die with Get-WMIObject"
- Removed from Microsoft Store and winget
- Hangs on Windows 25H2

**Fork status (v1.1.0):** Automation library integrated as source, SSL vulnerability fixed, freezing plugins removed, all 16 projects build clean. See [CHANGELOG.md](CHANGELOG.md) for full details.

## Architecture
- WPF app, .NET Framework 4.8, 14 plugins (2 removed in v1.1.0)
- `sccmclictr.automation` library (formerly closed-source NuGet) now integrated as a source project — 38 C# files, ~15,600 lines
- WinRM on port 5985 (HTTP) or 5986 (HTTPS)
- Plugin architecture via dynamic assembly loading

## Security Issues

### Fixed in v1.1.0
1. **SSL cert validation globally disabled**: `ServerCertificateValidationCallback = delegate { return true; }` — replaced with TLS 1.2/1.3 enforcement

### Fixed in v1.1.1
2. **Credential handling hardened** — removed persistent `string Password` property from `SCCMAgent`. Credentials now stored only as `PSCredential`. IPC connection uses `Marshal.SecureStringToGlobalAllocUnicode` with immediate `ZeroFreeGlobalAllocUnicode` cleanup, minimizing plaintext exposure to the P/Invoke call boundary.
3. **ConnectIPC() no longer accepts plaintext password** — signature changed to `ConnectIPC(string, SecureString)` and `ConnectIPC(PSCredential)`. Caller updated to pass `SecurePassword` instead of `Password`.
4. **Invoke-Expression removed** — 4 call sites in `inventory.cs` and `agentactions.cs` replaced with direct `& msiexec.exe` invocation. Eliminates code injection surface.

### Open
1. **238 bare `catch { }` blocks** — silent exception swallowing across 55 files. Most are defensive property probes against remote clients (expected to fail when WMI class/registry key doesn't exist on that client version). Needs categorized review: leave defensive probes silent, surface actual operation failures.
2. **Saved password encryption weak** — the WPF app encrypts saved passwords with SHA1 using the assembly name as the key (`common.Encrypt`/`common.Decrypt` in MainPage.xaml.cs). This is in the UI layer, not the automation library.

## Outdated Dependencies
- `WPFToolkit` v3.5 — unmaintained since 2012, blocks .NET 10 migration
- `NavigationPane` v2.1 — unmaintained, blocks .NET 10 migration
- `System.Management.Automation` v3.0 — PS 3.0 era, hardcoded DLL path
- Code signing references Sectigo cert with hardcoded path (non-functional)

## Upstream Fixes (already included)
The fork base includes all 27 upstream commits through v1.0.7.2. These are already in our codebase:

| Fix | Commit | Status |
|-----|--------|--------|
| Keyboard debounce (100ms guard on Enter) | `3b5ae59` | Included |
| PSScripts UI freeze (DoEvents + Sleep) | `e72f9e0` | Included |
| IPC credential fix | `3b5ae59` | Included, further hardened in v1.1.0 |
| Log access buttons (execmgr/smsts) | `d191bc1` | Included |
| System lock action | `d556efa` | Included |
| AppImport removed | `4677c80` | Included |

## GitHub Issues (60 open on upstream) -- Key Themes
1. **SSL/HTTPS #1 pain point** — FQDN, cert validation, proxy broken (#199, #200, #203, #212). Partially addressed by v1.1.0 TLS fix.
2. **WMI deprecation existential** — Get-WmiObject to Get-CimInstance requested since 2018 (#58, #208)
3. **User-targeted deployments invisible** — #82 (26 comments), queries only IsMachineTarget
4. **Application Groups unsupported** — #144, #207, #183
5. **Connectivity brittle** — Surface, VPN, proxy, alt creds all break
6. **Resource leak** — CPU climbs over time (#80)

## WS-MAN Migration Assessment
- **Request**: Issue #216 — port from direct WMI (Get-WmiObject) to WS-MAN with PSSessionConfiguration tunneling
- **Previous blocker**: sccmclictrlib was a closed-source NuGet with no access to the WMI call sites
- **Current status**: Blocker removed. The automation library is now full source. All WMI calls are visible and modifiable in `sccmclictr.automation/functions/`.
- **Scope**: The library uses `System.Management.ManagementScope` / `ManagementObjectSearcher` throughout (classic WMI, not CIM). A port would replace these with `CimSession` / `CimInstance` (MI-based, WS-MAN native) or route queries through PowerShell remoting sessions.
- **Effort estimate**: 4-6 weeks. The 25 function files in `functions/` plus 3 in `policy/` contain all WMI call sites. `baseInit.cs` and `WSMan.cs` handle connection/runspace management.
- **Risk**: High. Every WMI query pattern changes. The 238 defensive catch blocks complicate testing — silent failures make it hard to distinguish "works" from "silently broken."
- **Recommendation**: Worth pursuing now that source access removes the primary blocker. Start with a single function file (e.g., `agentproperties.cs`) as a proof-of-concept before committing to full port.

## WMI Namespaces Used
- `root\cimv2` — Standard Windows classes (services, processes, OS info)
- `Root\CCM\SoftMgmtAgent` — Software execution state
- `Root\CCM\Policy\Machine\ActualConfig` — Client policy/variables
- `Root\CCM\Scheduler` — Schedule history
- `root\ccm\Events` — Real-time event watching
- `root\sms` — Site provider (admin server)
- `root\wmi` — WMI operational namespace

## .NET 10 Migration Path
.NET 10 is current LTS (EOL Nov 2028). WPF, System.Management, and COM interop are all supported.

**Key blockers:**
- `NavigationPane` and `WPFToolkit` must be replaced (dead .NET 3.5 era packages with no .NET 10 equivalent)
- PowerShell hosting shifts from 5.1 to 7.x (remote WSMan still works to 5.1 endpoints)

**Supported as-is:**
- `System.Management` (WMI) — available in .NET 10 via Windows Compatibility Pack
- WPF — first-class support in .NET 10
- COM interop — supported
