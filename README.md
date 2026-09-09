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

This is **one of several community forks**, not the only maintained fork. See also [drummachine24's fork](https://github.com/drummachine24/sccmclictr), which takes a .NET 10 modernization approach.

**Recovery status:** current source restores Roger's original automation-library source and WMI behavior after regressions in our reconstructed WMI-to-CIM migration. The user has verified the single-update installation and restart workflow on a live client. Broader feature checks remain pending. The published v1.3.0.1 assets predate this recovery; see [CODE_REVIEW.md](CODE_REVIEW.md) for validation status.

## Install

Download the [latest release](https://github.com/jasonulbright/sccmclictr/releases/latest) and choose one of these assets:

- `ClientCenterForConfigMgr-Setup.exe` — recommended. Installs the application, registers it in Apps & Features, and creates Start menu and optional desktop shortcuts.
- `ClientCenterForConfigMgr-Portable.zip` — extract and run `SCCMCliCtrWPF.exe`; no installer required. Administrative actions can still change local or remote system state.
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
| .NET Framework | 4.8 (also runs on the in-place 4.8.1 runtime) |
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

- Builds `sccmclictr.automation` from Roger's original public C# source, checked into this repository and targeting .NET Framework 4.8. No precompiled automation DLL is fetched from NuGet.
- Removed the dead self-update and RuckZuck plugins that could block startup against retired services.
- Restored TLS certificate validation and removed the global trust-all callback.
- Removed plaintext password persistence and the `/Password:` command-line argument.
- Replaced `Invoke-Expression` installer calls with direct process invocation.
- Restored the original WMI query/model contract over Windows PowerShell remoting. The broad CIM migration is withdrawn; the existing CIM browser helper remains.
- Added a repeatable Release build for the application and all 14 plugins.
- Added signed installer and portable release assets, SHA-256 checksums, CI, and provenance attestations.

The original library was not closed-source: it is available at [rzander/sccmclictrlib](https://github.com/rzander/sccmclictrlib). Earlier documentation incorrectly described it as a black box. The pinned source revision and local modifications are recorded in [UPSTREAM.md](sccmclictr.automation/UPSTREAM.md).

## Security

| Area | Status |
|---|---|
| TLS certificate validation | Fixed |
| Plaintext password persistence | Removed |
| `Invoke-Expression` command injection | Removed |
| Password on the command line | Removed |
| Query compatibility | Original WMI behavior restored; single-update workflow live-validated, broader checks pending |
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

Run the recovery regression and optional read-only live-parity checks with Pester 5.7.1 or newer in Windows PowerShell 5.1:

```powershell
.\scripts\test-pester.ps1
```

No NuGet restore is required; the automation library is a project reference and the stable third-party controls and MSTest assemblies are vendored under `lib\`. Recovery builds include the automation library's corresponding source, build instructions, and LGPL/GPL license texts under `library-source\` in both installer and portable staging. To compile the installer locally, install Inno Setup 6 or 7 and run:

```powershell
.\scripts\new-release-assets.ps1 -StageDirectory .\artifacts\stage -Version 1.3.0.1
```

Release signing and publication are documented in [docs/RELEASING.md](docs/RELEASING.md).

## Project scope

This is a maintenance-focused fork. Changes should improve compatibility, reliability, security, build repeatability, or distribution without rewriting the established .NET Framework 4.8 application. The experimental modern-WPF spike is not part of the shipping build.

We intentionally remain on **.NET Framework 4.8** to align runtime servicing with the supported host Windows lifecycle, rather than adopting modern .NET's recurring major-runtime upgrade schedule. Framework 4.8.1 ships with Server 2025, whose extended support ends November 14, 2034. This is not a promise that every older Windows version or ConfigMgr release is supported until then: each host OS, runtime combination, and ConfigMgr version must remain supported. See [Microsoft's Framework lifecycle policy](https://learn.microsoft.com/en-us/lifecycle/faq/dotnet-framework) and [Server 2025 lifecycle](https://learn.microsoft.com/en-us/lifecycle/products/windows-server-2025).

Functional fixes can be shared with other forks without adopting their runtime or feature roadmap. Preserve original behavior first; require evidence and regression tests for changes.

## License and credits

The application retains the [Microsoft Public License (MS-PL)](LICENSE.md). The separately built automation library is **LGPL-3.0-or-later**, with Roger's notices retained; see its [license](sccmclictr.automation/LICENSE.md), accompanying [GPL text](sccmclictr.automation/COPYING), and [source/build provenance](sccmclictr.automation/UPSTREAM.md). Third-party dependencies retain their own licenses.

- Original project and copyright: [Roger Zander](https://github.com/rzander/sccmclictr)
- Community maintenance: project contributors
