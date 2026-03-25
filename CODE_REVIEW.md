# SCCMCliCtr Code Review — 2026-03-25

## Fork Details
- **Source**: https://github.com/rzander/sccmclictr (v1.0.6.1)
- **Fork**: https://github.com/jasonulbright/sccmclictr
- **Latest upstream release**: 1.0.7.2 (source at `c:\projects\sccmclictr-1.0.7.2\`)

## Project Status
Original project effectively abandoned. Maintainer (rzander) stated in Jan 2026:
- "I currently don't have access to any test environments"
- "there are no plans to refactor ClientCenter... In worst case it will die with Get-WMIObject"
- Removed from Microsoft Store and winget
- Hangs on Windows 25H2

## Architecture
- WPF app, .NET Framework 4.8, 17 plugins
- All WMI/PowerShell communication through external `sccmclictrlib` NuGet (v1.0.1) — **black box, no source**
- WinRM on port 5985 (HTTP) or 5986 (HTTPS)
- Plugin architecture via dynamic assembly loading

## Security Issues (Critical)
1. **SSL cert validation globally disabled**: `ServerCertificateValidationCallback = delegate { return true; }`
2. **107+ bare `catch { }` blocks** — silent exception swallowing
3. **Weak credential handling** — assembly name as encryption key, SecureString→plaintext conversion
4. **ConnectIPC()** passes plaintext password

## Outdated Dependencies
- `WPFToolkit` v3.5 — unmaintained since 2012
- `System.Management.Automation` v3.0 — PS 3.0 era, hardcoded DLL path
- Code signing references Sectigo cert with hardcoded path

## Version Diff (1.0.6.1 → 1.0.7.2)
27 commits. Cherry-pick candidates:

| Priority | Fix | Description |
|----------|-----|-------------|
| HIGH | Keyboard debounce | Rapid Enter triggered multiple connections |
| HIGH | PSScripts UI freeze | Long scripts locked dropdown |
| HIGH | IPC credential fix | Remote auth with explicit creds broken |
| MEDIUM | Log access buttons | Quick open execmgr.log / smsts.log |
| MEDIUM | System lock action | New agent action |
| LOW | AppImport removed | Catalog import deprecated |

## GitHub Issues (60 open) — Key Themes
1. **SSL/HTTPS #1 pain point** — FQDN, cert validation, proxy broken (#199, #200, #203, #212)
2. **WMI deprecation existential** — Get-WmiObject→Get-CimInstance requested since 2018 (#58, #208)
3. **User-targeted deployments invisible** — #82 (26 comments), queries only IsMachineTarget
4. **Application Groups unsupported** — #144, #207, #183
5. **Connectivity brittle** — Surface, VPN, proxy, alt creds all break
6. **Resource leak** — CPU climbs over time (#80)

## WS-MAN Port Assessment
- Request from issue #216: port to WS-MAN with PSSessionConfiguration tunneling
- Blocker: sccmclictrlib black box owns all WMI comms
- Effort: 6-8 weeks full port, 4-5 weeks adapter approach
- Recommendation: Not worth standalone effort; apply learnings to LWC instead

## WMI Namespaces Used (reference for LWC)
- `root\cimv2` — Standard Windows classes
- `Root\CCM\SoftMgmtAgent` — Software execution state
- `Root\CCM\Policy\Machine\ActualConfig` — Client policy/variables
- `Root\CCM\Scheduler` — Schedule history
- `root\ccm\Events` — Real-time event watching
- `root\sms` — Site provider (admin server)
- `root\wmi` — WMI operational namespace
