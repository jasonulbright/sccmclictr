# Automation library source provenance

This directory is built from Roger Zander's public source, not decompiled output
and not a downloaded NuGet DLL.

- Repository: https://github.com/rzander/sccmclictrlib
- Pinned revision: `1c875c00ab04144741247873cea1b69cb25ef1ea` (Update to .NET 4.8).
- Imported: 2026-09-09, from the upstream `sccmclictr.automation` directory.
- Original assembly/file version: 1.0.1.0; application version remains 1.3.0.1.
- License: LGPL-3.0-or-later, as stated in the original source headers.
  See LICENSE.md and COPYING. The application's MS-PL license does not replace this license.

Matching the NuGet assembly version does not establish byte-for-byte equivalence
with either the NuGet binary or Roger's installed application. Live parity must
be tested separately.

## Local changes from that revision

- Project integration: retain this application's project GUID; resolve Windows
  PowerShell from the installed Framework environment; remove machine-specific
  signing, source-control bindings, and unused project-item references.
- Define CM2012 in both configurations. Retain original WMI model/query behavior,
  System.Management, resources, and the existing separate CIM query helper.
- Remove two unused TripleDES allocations; retain the DPAPI encrypted-data format.
- Keep credentials as PSCredential. IPC uses a Unicode unmanaged password buffer,
  clears it in finally, avoids a managed plaintext copy, and checks native error
  codes instead of claiming that a failed connection succeeded.
- MSI operations validate product codes as GUIDs and invoke msiexec directly,
  rather than constructing Invoke-Expression commands.
- WMI/CIM object-query helpers surface provider errors, replace cached results
  on refresh, and use the requested cache lifetime.
- Mandatory-update installation now passes a typed WMI update collection filtered
  to instances with a deadline, and does nothing when that collection is empty.
  This call still needs live installation validation; offline tests only inspect
  the generated command.
- Retain the bare-catch regression guard with a new original-source baseline.
  Existing upstream catches are not thereby certified safe.

No blanket CIM conversion, modern .NET retarget, or new NuGet dependency is part
of this recovery. The source files keep their upstream notices. The former
reconstruction is preserved in Git on `codex/pre-recovery-8014407`.

## Build / replace the library

On Windows with Visual Studio Build Tools, the .NET Framework 4.8 targeting pack,
and Windows PowerShell 5.1, run from a Developer PowerShell:

```powershell
msbuild .\sccmclictr.automation\sccmclictr.automation.csproj /t:Rebuild /p:Configuration=Release /p:Platform=AnyCPU
```

In a release's `library-source` directory, this same command builds the accompanying
source. The guard script and its baseline are included under `tools`.
Output is `sccmclictr.automation\bin\Release\sccmclictr.automation.dll`.
With Client Center closed, an API-compatible rebuilt DLL can replace the DLL
beside the executable; retain a backup of the original. A modified DLL will not
retain the release's Authenticode signature.

The main repository's `scripts/build.ps1` builds this project, the application,
and plugins together. Release signing signs the resulting first-party binaries;
there is no automation-library NuGet download step.
