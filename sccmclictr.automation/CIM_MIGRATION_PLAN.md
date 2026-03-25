# CIM Migration Plan: System.Management to Microsoft.Management.Infrastructure

## Current Architecture

### Connection Model
The application connects to remote SCCM clients via **WinRM/PSRemoting** (PowerShell runspaces), NOT direct WMI/DCOM:

1. `SCCMAgent` creates a `WSManConnectionInfo` with target hostname, credentials, and port (default 5985)
2. `WSMan.openRunspace()` creates a `Runspace` via `RunspaceFactory.CreateRunspace(connectionInfo)`
3. The runspace is passed to `ccm` (which extends `baseInit`)
4. All WMI operations flow through `WSMan.RunPSScript()` which creates a `PowerShell` instance attached to the remote runspace and calls `AddScript()` + `Invoke()`

**Key insight**: There are NO direct C# WMI calls using `ManagementScope`, `ManagementObjectSearcher`, or `ManagementObject` to connect to remote machines. ALL WMI access is PS-mediated -- PowerShell commands sent through the remote runspace.

### baseInit.cs Helper Methods

| Method | PS Command Pattern | WMI Mechanism |
|--------|-------------------|---------------|
| `GetStringFromClassMethod()` | `([wmiclass]"PATH").Method().Property` | PS `[wmiclass]` accelerator |
| `GetStringFromMethod()` | `([wmi]"PATH").Method()(Property)` | PS `[wmi]` accelerator |
| `CallClassMethod()` | `([wmiclass]'PATH').Method(Params)` | PS `[wmiclass]` accelerator |
| `CallInstanceMethod()` | `([wmi]'PATH').Method(Params)` | PS `[wmi]` accelerator |
| `GetProperty()` | `([wmi]"PATH").Property` | PS `[wmi]` accelerator |
| `GetProperties()` | `([wmi]'PATH').Property` | PS `[wmi]` accelerator |
| `SetProperty()` | `$a=([wmi]"PATH");$a.Prop=Val;$a.Put()` | PS `[wmi]` accelerator |
| `GetObjects()` | `get-wmiobject -query "WQL" -namespace "NS"` | PS `Get-WmiObject` cmdlet |
| `GetCimObjects()` | `Get-CimInstance -query "WQL" -namespace "NS"` | PS `Get-CimInstance` cmdlet (already migrated!) |
| `GetStringFromPS()` | Arbitrary PS code | Varies -- caller-supplied |
| `GetObjectsFromPS()` | Arbitrary PS code | Varies -- caller-supplied |

### System.Management Usage (C# side)

The `System.Management` assembly reference is used for exactly ONE thing on the C# side:
- **`ManagementDateTimeConverter.ToDateTime()`** -- converts DMTF datetime strings (e.g., `20231215120000.000000+000`) to `DateTime` objects. Used in ~40 locations across model constructors in:
  - `common.cs`, `appv4.cs`, `CIM_ManagedSystemElement.cs`, `CIM_Process.cs`
  - `dcm.cs`, `inventory.cs`, `softwaredistribution.cs`, `swcache.cs`, `softwareupdates.cs`

There are NO `ManagementScope`, `ManagementObjectSearcher`, or `ManagementObject` instances created in C# code.

### PS-Mediated WMI (PowerShell command strings)

These are string literals in C# that get executed on the remote machine via the runspace:

#### 1. `Get-WmiObject` / `gwmi` cmdlet calls (PHASE 1 -- simple string replacement)

| File | Usage |
|------|-------|
| `baseInit.cs:610` | `get-wmiobject -query ... -namespace ...` in `GetObjects()` |
| `agentproperties.cs:467,476,485` | `Get-WmiObject -Class Win32_OperatingSystem` (reboot info) |
| `agentproperties.cs:802` | `Get-WmiObject -Class CCM_InstalledProduct` |
| `agentproperties.cs:820` | `gwmi -query ... \| Remove-WmiObject` |
| `agentproperties.cs:837` | `get-wmiobject -query ... -namespace "root\\cimv2"` |
| `agentproperties.cs:846` | `get-wmiobject -query ... -namespace "ROOT\\ccm"` |
| `inventory.cs:156` | `Get-WmiObject Win32_Processor` |
| `inventory.cs:169` | `Get-WmiObject Win32_OperatingSystem` |
| `health.cs:144` | `gwmi -query ... \| Remove-WmiObject` |
| `softwareupdates.cs:133` | `get-wmiobject -query ... -namespace ...` (inside type cast) |
| `softwareupdates.cs:235` | `get-wmiobject -query ...` (inside type cast) |
| `softwareupdates.cs:1268` | `get-wmiobject -query ...` |
| `requestedConfig.cs:72` | `Get-WMIObject -Namespace ...` |
| `Settings.cs:26` | `Get-WmiObject -Namespace root/cimv2 -Class __SystemSecurity` |
| `Resources.resx:129-131` | `get-wmiobject -query ...` (HealthCheck script, 3 occurrences) |
| `Resources.resx:143` | `gwmi win32_process` (HealthCheck script) |
| `Resources.resx:324,330` | `get-wmiobject -query ...` (CacheCleanup script, 2 occurrences) |

#### 2. `Remove-WmiObject` calls (PHASE 1b -- replace with `Remove-CimInstance`)

| File | Usage |
|------|-------|
| `agentproperties.cs:820` | `gwmi ... \| Remove-WmiObject` |
| `agentactions.cs:51` | `[wmi]"..." \| remove-wmiobject` |
| `health.cs:144` | `gwmi ... \| Remove-WmiObject` |
| `locationservices.cs:87` | `[wmi]'...' \| remove-wmiobject` |
| `requestedConfig.cs:72` | `Get-WMIObject ... \| Remove-WmiObject` |
| `swcache.cs:293` | `[wmi]'...' \| remove-wmiobject` |

#### 3. `[System.Management.ManagementObject[]]` type casts in PS strings (PHASE 2 -- needs CIM equivalent)

| File | Usage |
|------|-------|
| `softwareupdates.cs:133` | Cast for `InstallUpdates()` parameter |
| `softwareupdates.cs:235` | Cast for `InstallUpdates()` parameter |

These cast `Get-WmiObject` results to `[System.Management.ManagementObject[]]` for passing to the `CCM_SoftwareUpdatesManager.InstallUpdates()` WMI method. With `Get-CimInstance`, the return type is `CimInstance` not `ManagementObject`, so the cast needs to change to `[CimInstance[]]`.

#### 4. `[wmi]` / `[wmiclass]` PS type accelerators (PHASE 3 -- needs CIM equivalent)

These are deeply embedded in `baseInit.cs` helper methods and throughout function files. The CIM equivalents are:
- `[wmi]"PATH"` -> `Get-CimInstance` with appropriate parameters
- `[wmiclass]"PATH"` -> `Get-CimClass` or `Invoke-CimMethod`
- `.Put()` -> `Set-CimInstance`

#### 5. `system.management.ManagementClass` in PS strings (Settings.cs)

`Settings.cs:26` uses `new-object system.management.ManagementClass Win32_SecurityDescriptorHelper` in a PS command. This is a PS-side .NET type usage, not C# side.

## Migration Phases

### Phase 1: Replace `Get-WmiObject`/`gwmi` with `Get-CimInstance` in PS command strings (**LOW RISK**)
- Simple string replacement in C# string literals
- `Get-WmiObject` -> `Get-CimInstance`
- `gwmi` -> `Get-CimInstance`
- `Get-WMIObject` -> `Get-CimInstance`
- Also replace `Remove-WmiObject` -> `Remove-CimInstance`
- Also replace `[System.Management.ManagementObject[]]` -> `[CimInstance[]]` in PS command strings
- Update `Resources.resx` (HealthCheck and CacheCleanup scripts)
- Update `Settings.cs` (PSGetDCOMPerm)
- Note: `Resources.cs` is auto-generated from `Resources.resx`, so only update `.resx`; rebuild regenerates `.cs`

### Phase 2: Replace `[wmi]`/`[wmiclass]` PS accelerators with CIM cmdlets (**MEDIUM RISK**)
These accelerators use `System.Management` under the hood. CIM equivalents:

| Old Pattern | New Pattern |
|-------------|-------------|
| `([wmi]"NS:Class=@").Property` | `(Get-CimInstance -Namespace "NS" -ClassName "Class").Property` |
| `([wmiclass]"NS:Class").Method(params)` | `Invoke-CimMethod -Namespace "NS" -ClassName "Class" -MethodName "Method" -Arguments @{...}` |
| `$a=([wmi]"NS:Class=@");$a.Prop=Val;$a.Put()` | `Set-CimInstance` or `Invoke-CimMethod` |
| `[wmi]"path" \| remove-wmiobject` | `Get-CimInstance ... \| Remove-CimInstance` |

This affects ALL methods in `baseInit.cs` except `GetObjects()`, `GetCimObjects()`, `GetStringFromPS()`, and `GetObjectsFromPS()`.

### Phase 3: Replace `ManagementDateTimeConverter.ToDateTime()` in C# code (**LOW RISK**)
Write a small static helper method that parses DMTF datetime strings without depending on `System.Management`:
```csharp
// Replace ManagementDateTimeConverter.ToDateTime(dmtf) with:
public static DateTime DmtfToDateTime(string dmtfDate)
{
    // DMTF format: yyyyMMddHHmmss.ffffff+UUU
    // Parse manually or use regex
}
```
Then remove `using System.Management;` from all files and remove the `System.Management` reference from the `.csproj`.

### Phase 4: Remove `System.Management` assembly reference (**CLEANUP**)
- Remove `<Reference Include="System.Management" />` from `.csproj`
- Remove all `using System.Management;` statements
- Verify build succeeds

## Class Mapping (for future Phase 3)

| System.Management | Microsoft.Management.Infrastructure |
|-------------------|--------------------------------------|
| `ManagementScope` | `CimSession` |
| `ManagementObjectSearcher` | `CimSession.QueryInstances()` |
| `ManagementObject` | `CimInstance` |
| `ManagementClass` | `CimClass` / `CimSession.GetClass()` |
| `ManagementDateTimeConverter` | Custom helper (DMTF parsing) |
| `ObjectQuery` | WQL string passed to `QueryInstances()` |

## Risks and Testing Considerations

1. **`Get-CimInstance` returns `CimInstance` not `ManagementObject`**: Property access syntax is the same (`.PropertyName`), but type casts like `[System.Management.ManagementObject[]]` must change to `[CimInstance[]]`.

2. **`Get-CimInstance` date handling**: Returns `DateTime` objects directly instead of DMTF strings. However, since these PS commands run remotely and results come back as `PSObject`, the serialization behavior should be consistent.

3. **`Remove-CimInstance` vs `Remove-WmiObject`**: Same pipeline behavior (`| Remove-CimInstance` works like `| Remove-WmiObject`).

4. **`[wmi]`/`[wmiclass]` accelerators**: These still work in PS 5.1 but depend on `System.Management` being loaded in the remote session. They are deprecated and should be replaced in Phase 2, but will continue to work for now.

5. **`Get-CimInstance` parameter differences**:
   - `-query` and `-namespace` parameters work identically
   - `-Class` becomes `-ClassName` (though `-Class` works as alias)
   - No `-Filter` syntax differences

6. **CCM client WMI provider compatibility**: The SCCM client WMI provider exposes the same classes regardless of whether they're accessed via `Get-WmiObject` or `Get-CimInstance`. The WQL queries are identical.

7. **Testing priority**:
   - Health check script (Resources.resx HealthCheck)
   - Software updates install (`InstallUpdates` methods with type casts)
   - Reboot detection (`agentproperties.cs` Win32_OperatingSystem queries)
   - Cache cleanup script (Resources.resx CacheCleanup)

## Notes

- `Resources.cs` is auto-generated from `Resources.resx`. Only edit the `.resx` file; the `.cs` will be regenerated on build.
- The `Settings.cs` `DefaultSettingValue` attribute contains a PS script with `Get-WmiObject`. This is a compile-time constant in a settings attribute -- it defines a default value that gets embedded. It should be updated.
- The `GetCimObjects()` method in `baseInit.cs` already uses `Get-CimInstance` -- this was apparently a parallel implementation added alongside `GetObjects()`. After migration, `GetObjects()` and `GetCimObjects()` will produce identical PS commands.
