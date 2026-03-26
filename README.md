# Client Center for Configuration Manager (Community Fork)

A WPF desktop tool for IT professionals to troubleshoot ConfigMgr/MECM agent issues on remote workstations. Connect via WinRM to view client settings, running services, software deployments, update status, agent configuration, and execute administrative actions -- all from a single pane.

**This is a maintained community fork** of [rzander/sccmclictr](https://github.com/rzander/sccmclictr), which was effectively abandoned in 2023. The original tool was removed from the Microsoft Store and winget, hangs on Windows 25H2, and the maintainer confirmed he has no test environment and no plans to update.

## Installation

Download the latest release zip from [Releases](https://github.com/jasonulbright/sccmclictr/releases), extract, and run `SCCMCliCtrWPF.exe`.

The executable is **not signed** -- you may need to unblock it via file properties or:
```powershell
Get-ChildItem .\*.dll, .\*.exe | Unblock-File
```

### Requirements

**Host machine** (running the tool):

| Component | Version |
|-----------|---------|
| OS | Windows 10 1607+ / Windows Server 2016+ |
| .NET Framework | 4.8 |
| PowerShell | 5.1 (Windows PowerShell) |

**Target machine** (being managed):

| Component | Version |
|-----------|---------|
| WinRM | Enabled (`winrm quickconfig`) |
| PowerShell | 4.0+ (5.1 recommended) |
| ConfigMgr Agent | Any supported version |
| Admin rights | Required on target |

## Usage

### Connecting

Launch the application and enter the target hostname. Authentication options:

- **Integrated auth** (recommended): Launch the app elevated (Run as Administrator or via PAM) and connect with no credentials. Uses the current user's Kerberos token.
- **Explicit credentials**: Enter `DOMAIN\username` in the Username field and type the password. Passwords are not saved between sessions.

### Command Line

```
SCCMCliCtrWPF.exe <Hostname> [/Username:DOMAIN\user]
```

### Plugins

The tool includes 14 plugin extensions:

| Plugin | Description |
|--------|-------------|
| Computer Management | Remote computer management snap-in |
| Explorer | Browse remote file shares and log folders |
| PSScripts | Execute PowerShell scripts on the remote client |
| RDP | Launch Remote Desktop to the target |
| Registry Editor | Browse and edit the remote registry |
| Enable PSRemoting | Enable WinRM on a target (via WMI) |
| Status Message Viewer | View SCCM status messages |
| Resource Explorer | Browse SCCM resource inventory |
| Remote Tools | Launch CM Remote Control |
| MSInfo32 | Remote system information |
| MSRA | Microsoft Remote Assistance |
| FEP | Endpoint Protection status |
| AppV 4.6 | App-V 4.6 virtual application management |
| AMT Tools | Intel vPro/AMT remote management |

## What This Fork Changes

- **Open-sourced the automation library** -- the closed-source NuGet engine is now full source (38 C# files, ~15,600 lines)
- **Fixed startup freeze** -- removed Plugin_SelfUpdate (synchronous HTTP on UI thread to defunct API)
- **Fixed SSL vulnerability** -- re-enabled certificate validation, enforced TLS 1.2/1.3
- **Hardened credentials** -- no plaintext password storage, SecureString throughout
- **Removed code injection** -- replaced `Invoke-Expression` with direct invocation
- **Removed saved passwords** -- `/Password:` command-line argument removed
- **CIM migration complete** -- all `Get-WmiObject` / `[wmi]` / `[wmiclass]` replaced with CIM cmdlets. `System.Management` dependency removed. 43 Pester tests. ([details](CODE_REVIEW.md))

## Security

| Issue | Status |
|-------|--------|
| SSL cert validation disabled | **Fixed** |
| Plaintext password storage | **Fixed** |
| Invoke-Expression code injection | **Fixed** |
| Saved-password persistence | **Fixed** |
| WMI deprecation (`System.Management`) | **Fixed** — CIM migration complete |
| 308 bare `catch { }` blocks | [Audited](CATCH_BLOCK_AUDIT.md) |
| Outdated vendored dependencies | Open |

This tool is designed for trusted administrators on internal networks. See [CODE_REVIEW.md](CODE_REVIEW.md) for detailed security analysis.

## Building from Source

No external package managers required -- all dependencies are vendored in `lib/`.

**Prerequisites**: Visual Studio 2022+ (or MSBuild 17+) and .NET Framework 4.8 targeting pack.

```
msbuild SCCMCliCtrWPF\SCCMCliCtrWPF.sln -p:Configuration=Debug
for /d %p in (Plugins\Plugin_*) do msbuild "%p\*.sln" -p:Configuration=Debug -verbosity:quiet
copy Plugins\*\bin\Debug\Plugin_*.dll SCCMCliCtrWPF\SCCMCliCtrWPF\bin\Debug\
```

Output: `SCCMCliCtrWPF\SCCMCliCtrWPF\bin\Debug\SCCMCliCtrWPF.exe`

## License

[MS-PL (Microsoft Public License)](LICENSE.md) -- inherited from the original project.

## Credits

- Original project by [Roger Zander](https://github.com/rzander/sccmclictr)
- Community fork maintenance and modernization
