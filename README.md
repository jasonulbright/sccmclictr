# Client Center for Configuration Manager

[![CI](https://github.com/jasonulbright/sccmclictr/actions/workflows/ci.yml/badge.svg)](https://github.com/jasonulbright/sccmclictr/actions/workflows/ci.yml)
[![Latest release](https://img.shields.io/github/v/release/jasonulbright/sccmclictr?label=release)](https://github.com/jasonulbright/sccmclictr/releases/latest)
[![Downloads](https://img.shields.io/github/downloads/jasonulbright/sccmclictr/total?label=downloads)](https://github.com/jasonulbright/sccmclictr/releases)
[![Platform](https://img.shields.io/badge/platform-Windows-0078D4)](#requirements)
[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-512BD4)](#requirements)
[![License](https://img.shields.io/github/license/jasonulbright/sccmclictr)](LICENSE.md)
[![Maintenance](https://img.shields.io/badge/status-maintenance-2ea44f)](#project-scope)

A WPF desktop tool for IT professionals to troubleshoot Microsoft Configuration Manager (ConfigMgr) clients on remote Windows computers. It uses PowerShell remoting over WinRM to inspect client settings, services, deployments, updates, policy, logs, and inventory, and to run administrative actions from one place.

![Client Center for Configuration Manager v1.3.0.1 About screen](screenshot.png)

This is a maintenance-focused community continuation of [Roger Zander's original Client Center](https://github.com/rzander/sccmclictr). It preserves the established .NET Framework application and plugin model while adding reproducible builds, security fixes, and supported release packaging. It is an independent community project and is not supported by Microsoft.

## Install

Download the [latest release](https://github.com/jasonulbright/sccmclictr/releases/latest) and choose one of these assets:

- `ClientCenterForConfigMgr-Setup.exe` — recommended. Installs the application, registers it in Apps & Features, and creates Start menu and optional desktop shortcuts.
- `ClientCenterForConfigMgr-Portable.zip` — extract anywhere and run `SCCMCliCtrWPF.exe`; no installation or registry changes.
- `checksums.txt` — SHA-256 hashes for both release assets.

First-party application binaries and the installer are Authenticode-signed through Azure Artifact Signing. The release workflow verifies the signer and timestamp before publication, and GitHub publishes build-provenance attestations for the installer and portable archive. The vendored `NavigationPane.dll` retains its upstream unsigned binary. To verify the checksum in PowerShell:

```powershell
Get-FileHash .\ClientCenterForConfigMgr-Setup.exe -Algorithm SHA256
```

Compare the result with the matching line in `checksums.txt`.

### Requirements

Host machine (running Client Center):

| Component | Version |
|---|---|
| OS | Windows 10 1607+ / Windows Server 2016+ (installer-enforced minimum) |
| .NET Framework | 4.8 |
| PowerShell | Windows PowerShell 5.1 |

Target machine (being managed):

| Component | Requirement |
|---|---|
| WinRM | Enabled and reachable |
| PowerShell | Windows PowerShell 3.0+ |
| ConfigMgr client | Installed on an OS supported by the site's ConfigMgr version |
| Permissions | Local administrator rights on the target |

See Microsoft's documentation for [PowerShell remoting requirements](https://learn.microsoft.com/powershell/module/microsoft.powershell.core/about/about_remote_requirements) and the [ConfigMgr client OS support matrix](https://learn.microsoft.com/intune/configmgr/core/plan-design/configs/supported-operating-systems-for-clients-and-devices). Individual Client Center features can also depend on the ConfigMgr client version and locally installed administrative tools.

## Usage

Launch Client Center and enter the target hostname.

- Integrated authentication (recommended): leave the username blank to use the current Windows identity through WinRM. Domain hostnames normally use Kerberos; other scenarios follow the host's WinRM authentication configuration.
- Explicit credentials: enter `DOMAIN\username` and the password. The username can be remembered, but the password is not persisted between sessions or accepted on the command line.

Command line:

```text
SCCMCliCtrWPF.exe <Hostname> [/Username:DOMAIN\user]
```

## Included plugins

| Plugin | Purpose |
|---|---|
| Computer Management | Open remote Computer Management |
| Explorer | Browse remote shares and log folders |
| PowerShell Scripts | Run the included or custom scripts on the connected client |
| RDP | Launch Remote Desktop |
| Registry Editor | Browse and edit the remote registry |
| Enable PSRemoting | Enable WinRM through WMI where needed |
| Status Message Viewer | View ConfigMgr status messages |
| Resource Explorer | Launch the ConfigMgr console Resource Explorer (console required) |
| Remote Tools | Launch the ConfigMgr Remote Control viewer (console required) |
| MSInfo32 | Open remote system information |
| MSRA | Launch Microsoft Remote Assistance |
| FEP | Inspect Endpoint Protection state |
| App-V 4.6 | Manage legacy App-V 4.6 applications |
| AMT Tools | Intel vPro/AMT remote actions |

## Community fork changes

- Reconstructed the formerly DLL-only `sccmclictr.automation` NuGet dependency as a buildable .NET Framework 4.8 C# project included in this repository.
- Removed the dead self-update and RuckZuck plugins that could block startup against retired services.
- Restored TLS certificate validation and removed the global trust-all callback.
- Removed plaintext password persistence and the `/Password:` command-line argument.
- Replaced `Invoke-Expression` installer calls with direct process invocation.
- Migrated the core automation helpers and primary client queries from deprecated WMI PowerShell syntax to CIM.
- Added a repeatable Release build for the application and all 14 plugins.
- Added signed installer and portable release assets, SHA-256 checksums, CI, and provenance attestations.

Some optional legacy PowerShell scripts intentionally retain WMI syntax because this maintenance fork continues to target Windows PowerShell 5.1 and old ConfigMgr environments. The exact migration status and remaining compatibility work are documented in [CODE_REVIEW.md](CODE_REVIEW.md).

## Security

| Area | Status |
|---|---|
| TLS certificate validation | Fixed |
| Plaintext password persistence | Removed |
| `Invoke-Expression` command injection | Removed |
| Password on the command line | Removed |
| WMI-to-CIM migration | Substantial; compatibility paths and optional scripts remain |
| Bare catch blocks | Legacy instances remain; a regression baseline prevents new ones |
| Vendored UI dependencies | Accepted for the .NET Framework maintenance lifecycle |
| Release integrity | Authenticode, SHA-256 checksums, and GitHub attestations |

Client Center is an administrative tool intended for trusted operators on managed internal networks. See [CODE_REVIEW.md](CODE_REVIEW.md) for the security and architecture review.

## Build from source

Prerequisites:

- Visual Studio 2022 or newer Build Tools with the .NET desktop build workload
- .NET Framework 4.8 targeting pack
- Windows PowerShell 5.1

Build the application, all plugins, and a portable staging directory:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\build.ps1 -Configuration Release
```

Output: `artifacts\stage\SCCMCliCtrWPF.exe`

Run the MSTest suite after building:

```powershell
.\scripts\test.ps1 -Configuration Release
```

Run the CIM migration and optional live-environment Pester checks with Pester 5.7.1 or newer:

```powershell
.\scripts\test-pester.ps1
```

No NuGet restore is required; the stable third-party controls and MSTest assemblies are vendored under `lib\`. To compile the installer locally, install Inno Setup 6 or 7 and run:

```powershell
.\scripts\new-release-assets.ps1 -StageDirectory .\artifacts\stage -Version 1.3.0.1
```

Release signing and publication are documented in [docs/RELEASING.md](docs/RELEASING.md).

## Project scope

This is a maintenance-focused fork. Changes should improve compatibility, reliability, security, build repeatability, or distribution without rewriting the established .NET Framework 4.8 application. The experimental modern-WPF spike is not part of the shipping build.

## License and credits

Licensed under the [Microsoft Public License (MS-PL)](LICENSE.md), inherited from the original project.

- Original project and copyright: [Roger Zander](https://github.com/rzander/sccmclictr)
- Community maintenance and modernization: project contributors
