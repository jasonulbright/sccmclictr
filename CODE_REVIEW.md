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

### Fixed in v1.1.0 (cont.)
5. **Saved-password storage removed** — password is no longer persisted to settings between sessions. `/Password:` command-line argument removed (was visible in process listings). Previously saved passwords are cleared on first connect. Use integrated auth (launch elevated via PAM/runas) or type credentials each session.

### Open
1. **308 bare `catch { }` blocks** — audited and categorized across 57 files (see `CATCH_BLOCK_AUDIT.md`). 40 SILENT-OK, 91 DEBUG, 114 SURFACE, 61 UNVERIFIED. Implementation pending.

## Dependencies (all vendored — zero NuGet)
All external dependencies are vendored in `lib/`. No `nuget restore` required. No external package manager dependencies.

| Dependency | Version | Location | Status |
|-----------|---------|----------|--------|
| NavigationPane | 2.1.0 | `lib/NavigationPane/` | Vendored DLL. Dead project (.NET 3.5 era). Replace in .NET 10 migration. |
| WPFToolkit | 3.5.50211.1 | `lib/WPFToolkit/` | Vendored DLL (3 files). Dead since 2012. Most controls built into modern WPF. |
| MSTest | 1.2.0 | `lib/MSTest/` | Vendored DLL + build props/targets. Test infrastructure only. |
| System.Management.Automation | 3.0.0 | GAC / hardcoded path | PS 3.0 era. Ships with Windows. |
| Code signing cert | Sectigo | Hardcoded path | Non-functional reference from original author. |

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

## CIM Migration (in progress — `cim-migration` branch)
- **Request**: Upstream issues #58, #208, #216 — replace deprecated `Get-WmiObject` / `[wmi]` / `[wmiclass]` with CIM cmdlets
- **Key finding**: All WMI access is PS-mediated (PowerShell commands through remote runspace). Zero direct C# WMI calls to remote machines. Only C#-side `System.Management` usage is `ManagementDateTimeConverter.ToDateTime()` (~40 sites).
- **Phases 1, 2a, 2b complete**: `Get-WmiObject`→`Get-CimInstance`, property reads/writes→CIM, no-arg methods→`Invoke-CimMethod`. Validated with Pester tests (30/30 pass).
- **Phase 2c pending**: 6 remaining `[wmi]`/`[wmiclass]` references with parameterized method calls. Requires WMI method parameter name mapping for `Invoke-CimMethod -Arguments`.
- **Phase 3 pending**: Replace `ManagementDateTimeConverter.ToDateTime()` with custom DMTF parser.
- **Phase 4 pending**: Remove `System.Management` assembly reference entirely.
- **Test lab**: Home lab operational (CM 2509, site code MCM, `192.168.50.20`). Integration tests can now validate against live SCCM client.
- See `CIM_MIGRATION_PLAN.md` on the `cim-migration` branch for full details.

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
