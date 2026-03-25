# Bare Catch Block Audit

Status: Categorized, not yet implemented.

## Summary

| Category | Count | Action |
|----------|-------|--------|
| SILENT-OK | 40 | Leave as-is -- defensive probes, dispose, cleanup |
| DEBUG | 91 | Add `Debug.WriteLine(ex.ToString())` |
| SURFACE | 114 | Add `Listener?.WriteError(ex.Message)` or trace |
| SURFACE-ALREADY | 2 | Already has inline error handling in catch body |
| UNVERIFIED | 61 | Need manual review (agent couldn't read full context) |

Total: 308 bare catch blocks across 57 files.

## Categories

### SILENT-OK
Defensive property probes against remote clients, dispose/cleanup, config loading with fallbacks. Failure is expected and the return value (null/empty/false) is correct behavior.

### DEBUG
Plugin loading, grid binding, WMI enumeration, XML parsing, date conversion. Not critical but useful for troubleshooting. `System.Diagnostics.Debug.WriteLine` -- only visible with debugger attached.

### SURFACE
User-initiated actions: button click handlers (install/repair/trigger), console registration, remote process launches, service operations, schedule triggers. Should surface via `Listener?.WriteError(ex.Message)` or `myTrace.WriteError()`.

### SURFACE-ALREADY
Catch blocks that already contain meaningful error handling inline (e.g., `myTrace.WriteError(...)` or `rStatus.AppendText(...)`). No changes needed.

### UNVERIFIED
Blocks where the surrounding context was not fully reviewed. Need manual inspection to determine correct category.

---

## Automation Library (sccmclictr.automation/)

Note: The automation library catch blocks are primarily in decompiled code. Most follow a pattern of returning a default value (empty string, false, null, empty list) on failure. The library has `tsPSCode.TraceInformation` infrastructure but bare catches bypass it.

### baseInit.cs (3 blocks)
| Line | Category | Context |
|------|----------|---------|
| 50 | SILENT-OK | Dispose -- `tsPSCode.Close()` inside Dispose method |
| 63 | SILENT-OK | Dispose -- `Cache.Dispose()` inside Dispose method |
| 67 | SILENT-OK | Dispose -- outer catch wrapping entire Dispose body |

### SCCMAgent.cs (5 blocks)
| Line | Category | Context |
|------|----------|---------|
| 46 | SILENT-OK | Dispose -- `disconnect()` inside Dispose method |
| 72 | SILENT-OK | Property probe -- `TargetHostname` getter, returns "" on failure |
| 95 | SILENT-OK | Property probe -- `isConnected` getter, returns false on failure |
| 402 | SILENT-OK | IPC connect attempt -- sets `ipcconnected = false` on failure |
| 418 | SILENT-OK | IPC connect attempt -- sets `ipcconnected = false`, returns false |

### WSMan.cs (3 blocks)
| Line | Category | Context |
|------|----------|---------|
| 61 | DEBUG | PS command execution -- returns null collection on failure |
| 81 | DEBUG | PSObject.ToString() in string builder loop |
| 150 | DEBUG | PSObject.ToString() in string builder loop |

### common.cs (1 block)
| Line | Category | Context |
|------|----------|---------|
| 107 | SILENT-OK | DateTime parse -- `ManagementDateTimeConverter.ToDateTime`, returns null DateTime |

### agentactions.cs (62 blocks)
| Line | Category | Context |
|------|----------|---------|
| 53 | SURFACE | `SMSDelInvHist` -- delete inventory history, void method |
| 69 | SURFACE | `TriggerSchedule` HW inventory -- returns false on failure |
| 87 | SURFACE | `TriggerSchedule` SW inventory -- returns false on failure |
| 105 | SURFACE | `TriggerSchedule` data discovery -- returns false on failure |
| 123 | SURFACE | `TriggerSchedule` file collection -- returns false on failure |
| 141 | SURFACE | `TriggerSchedule` IDMIF collection -- returns false on failure |
| 159 | SURFACE | `TriggerSchedule` SW metering -- returns false on failure |
| 174 | SURFACE | `TriggerSchedule` machine policy eval -- returns false on failure |
| 189 | SURFACE | `TriggerSchedule` machine policy assignments -- returns false on failure |
| 204 | SURFACE | `TriggerSchedule` user policy assignments -- returns false on failure |
| 219 | SURFACE | `TriggerSchedule` force user policy -- returns false on failure |
| 234 | SURFACE | `TriggerSchedule` machine policy cleanup -- returns false on failure |
| 253 | DEBUG | Inner catch -- SetProperty for user policy trigger, non-critical |
| 258 | SURFACE | Outer catch -- user policy request cycle, returns false |
| 277 | DEBUG | Inner catch -- SetProperty for user policy trigger, non-critical |
| 282 | SURFACE | Outer catch -- user policy eval cycle, returns false |
| 297 | SURFACE | `TriggerSchedule` SW dist -- returns false on failure |
| 312 | SURFACE | `TriggerSchedule` SW dist eval -- returns false on failure |
| 327 | SURFACE | `TriggerSchedule` initial program run -- returns false on failure |
| 342 | SURFACE | `TriggerSchedule` retrying/pending actions -- returns false on failure |
| 357 | SURFACE | `TriggerSchedule` peer DP status -- returns false on failure |
| 372 | SURFACE | `TriggerSchedule` peer DP pending -- returns false on failure |
| 387 | SURFACE | `TriggerSchedule` update scan -- returns false on failure |
| 402 | SURFACE | `TriggerSchedule` update store -- returns false on failure |
| 417 | SURFACE | `TriggerSchedule` state msg cache cleanup -- returns false on failure |
| 432 | SURFACE | `TriggerSchedule` update source scan -- returns false on failure |
| 447 | SURFACE | `TriggerSchedule` update deploy eval -- returns false on failure |
| 462 | SURFACE | `TriggerSchedule` SW update policy -- returns false on failure |
| 477 | SURFACE | `TriggerSchedule` send unsent state msg -- returns false on failure |
| 492 | SURFACE | `TriggerSchedule` state system -- returns false on failure |
| 507 | SURFACE | `TriggerSchedule` force full scan -- returns false on failure |
| 522 | SURFACE | `TriggerSchedule` settings compliance -- returns false on failure |
| 537 | SURFACE | `TriggerSchedule` file collection eval -- returns false on failure |
| 552 | SURFACE | `TriggerSchedule` endpoint deploy eval -- returns false on failure |
| 567 | SURFACE | `TriggerSchedule` endpoint policy eval -- returns false on failure |
| 582 | SURFACE | `TriggerSchedule` external events -- returns false on failure |
| 597 | SURFACE | `TriggerSchedule` SW update eval -- returns false on failure |
| 612 | SURFACE | `TriggerSchedule` policy agent validate -- returns false on failure |
| 627 | SURFACE | `TriggerSchedule` policy agent host -- returns false on failure |
| 642 | SURFACE | `TriggerSchedule` scan update source -- returns false on failure |
| 657 | SURFACE | `TriggerSchedule` update content deploy -- returns false on failure |
| 672 | SURFACE | `TriggerSchedule` state msg fwd -- returns false on failure |
| 687 | SURFACE | `TriggerSchedule` compliance store scan -- returns false on failure |
| 702 | SURFACE | `TriggerSchedule` source update msg -- returns false on failure |
| 721 | DEBUG | Inner catch -- SetProperty for user policy trigger, non-critical |
| 726 | SURFACE | Outer catch -- app global eval, returns false |
| 741 | SURFACE | `PerformAction` app mgmt -- returns false on failure |
| 756 | SURFACE | `TriggerSchedule` DCMCI assignment -- returns false on failure |
| 771 | SURFACE | `TriggerSchedule` SW update agent -- returns false on failure |
| 786 | SURFACE | `TriggerSchedule` send unsent msg -- returns false on failure |
| 801 | SURFACE | `TriggerSchedule` data transfer -- returns false on failure |
| 819 | SURFACE | ClearMessageQueue -- stop/cleanup/start CcmExec, returns false |
| 840 | SURFACE | ResetPolicy -- calls WMI ResetPolicy, returns false |
| 856 | SURFACE | SetClientProvisioningMode -- returns false on failure |
| 895 | SURFACE | ResetPausedSWDist -- registry fix, returns false on failure |
| 911 | SURFACE | ResetProvisioningMode -- registry fix, returns false on failure |
| 926 | SURFACE | SystemTaskExclude -- registry fix, returns false on failure |
| 941 | SURFACE | IsCacheCopyNeededCallBack -- registry cleanup, returns false |
| 958 | SURFACE | ApplyPolicyEx -- returns false on failure |
| 1052 | DEBUG | XML attribute parsing in rule class builder loop |
| 1058 | DEBUG | Outer catch around rule class builder |
| 1063 | DEBUG | Outer catch around XML/rule processing |

### agentproperties.cs (18 blocks)
| Line | Category | Context |
|------|----------|---------|
| 429 | SILENT-OK | Property probe -- `ClientVersion` getter, returns cached/empty |
| 502 | SILENT-OK | Property probe -- `AssignedSite` getter, returns empty |
| 527 | SILENT-OK | Property probe -- `ManagementPoint` getter, returns empty |
| 543 | SILENT-OK | Property probe -- `InternetManagementPoint` getter, returns "" |
| 576 | SILENT-OK | Property probe -- `ManagementPointProxy` getter, returns empty |
| 603 | SILENT-OK | Property probe -- `LocalSCCMAgentPath` getter, returns "" |
| 626 | DEBUG | Property probe -- `SCCMAgentLogFiles` list, returns empty list |
| 644 | SILENT-OK | Property probe -- `SMSSLP` getter, returns "" |
| 673 | SILENT-OK | Property probe -- `DnsSuffix` getter, returns "" |
| 708 | SILENT-OK | Property probe -- `HttpPort` getter, returns null int |
| 743 | SILENT-OK | Property probe -- `HttpsPort` getter, returns null int |
| 772 | SILENT-OK | Property probe -- `PendingFileRenameOperations`, returns false |
| 788 | SILENT-OK | Property probe -- `RebootPending` (CBS), returns false |
| 804 | DEBUG | Property probe -- `ProductCode`, returns "" |
| 822 | SURFACE | `RemoveWMINamespaceCCM` -- destructive WMI cleanup |
| 840 | DEBUG | `LoggedOnUserSiDs` -- get user SID, continues on failure |
| 852 | DEBUG | Inner loop -- `UserSID` property access, continues |
| 879 | DEBUG | `DeviceId` -- property parsing, returns default struct |

### components.cs (1 block)
| Line | Category | Context |
|------|----------|---------|
| 108 | DEBUG | Component Enabled check -- sets Enabled=false on failure |

### dcm.cs (4 blocks)
| Line | Category | Context |
|------|----------|---------|
| 94 | SILENT-OK | DateTime parse -- `LastEvalTime` DMTF conversion |
| 150 | SURFACE | `TriggerEvaluation` -- DCM baseline eval, returns 1 on failure |
| 175 | DEBUG | XML attribute parse -- `IsDetected`, defaults to true |
| 203 | DEBUG | Config item list parsing, returns partial list |

### health.cs (5 blocks)
| Line | Category | Context |
|------|----------|---------|
| 135 | DEBUG | `HealthCheck` PS script result, returns "" |
| 154 | SURFACE | `DeleteSMSCertificates` -- registry cleanup, void |
| 174 | DEBUG | `LastCCMEval` DateTime parse, returns default DateTime |
| 201 | DEBUG | `CCMEvalStatus` loop -- individual status parsing, continues |
| 217 | SURFACE | `RefreshServerComplianceState` -- COM object call, void |

### inventory.cs (4 blocks)
| Line | Category | Context |
|------|----------|---------|
| 55 | DEBUG | `InstalledSoftware` loop -- WMI object construction, continues |
| 79 | DEBUG | `InventoryActionStatus` loop -- WMI object construction, continues |
| 142 | DEBUG | `SMS_PowerSettings` loop -- WMI object construction, continues |
| 214 | DEBUG | `InstallDate` property parse -- nullable DateTime handling |

### locationservices.cs (1 block)
| Line | Category | Context |
|------|----------|---------|
| 90 | DEBUG | `Delete` boundary group cache -- returns false on failure |

### monitoring.cs (4 blocks)
| Line | Category | Context |
|------|----------|---------|
| 135 | DEBUG | Type conversion fallback in async PS output handling |
| 146 | DEBUG | Hashtable property enumeration fallback |
| 153 | DEBUG | Outer fallback for async output conversion |
| 254 | SILENT-OK | Cleanup -- pipeline/runspace close in monitoring teardown |

### processes.cs (1 block)
| Line | Category | Context |
|------|----------|---------|
| 118 | SURFACE | `StartProcess` -- remote process launch, returns null uint |

### softwaredistribution.cs (22 blocks)
| Line | Category | Context |
|------|----------|---------|
| 80 | DEBUG | `CCM_Application` list loop -- WMI construction, continues |
| 102 | DEBUG | `CCM_ApplicationActions` -- single object construction, returns null |
| 201 | DEBUG | `SoftwareStatus` list loop -- WMI construction, continues |
| 264 | DEBUG | XML field reflection -- `SetValue` in loop, continues |
| 320 | DEBUG | `CCM_ApplicationCIAssignment` loop -- WMI construction, continues |
| 722 | DEBUG | Enforcement deadline override -- SetProperty in loop, continues |
| 745 | DEBUG | Enforcement deadline restore -- SetProperty, continues |
| 798 | DEBUG | `AppDTs` array construction -- property parsing |
| 825 | SILENT-OK | DateTime parse -- `LastEvalTime` DMTF conversion |
| 841 | SILENT-OK | DateTime parse -- `LastInstallTime` DMTF conversion |
| 860 | SILENT-OK | DateTime parse -- `ReleaseDate` DMTF conversion |
| 879 | SILENT-OK | DateTime parse -- `StartTime` DMTF conversion |
| 1211 | DEBUG | `CCM_Scheduler_ScheduledMessage` construction in loop |
| 1238 | SURFACE | `SetProperty` ADV_RepeatRunBehavior -- schedule override |
| 1245 | SURFACE | `SetProperty` ADV_MandatoryAssignments -- schedule override |
| 1818 | SURFACE | `Remove-Item` registry path -- execution history delete |
| 2042 | DEBUG | Update status switch case -- assignment with break |
| 2159 | DEBUG | WMI property parsing for SW update status icon/status |
| 2167 | DEBUG | `ErrorCode` uint parse |
| 2174 | DEBUG | `PercentComplete` uint parse |
| 2178 | DEBUG | Outer catch wrapping entire SW update constructor |
| 2723 | DEBUG | `AssignedCIs` array conversion |

### swcache.cs (6 blocks)
| Line | Category | Context |
|------|----------|---------|
| 166 | DEBUG | `ContentFlags` property probe -- sets null on failure |
| 176 | DEBUG | `ContentSize` property probe -- sets null on failure |
| 186 | DEBUG | `ExcludeFileList` property probe -- sets "" on failure |
| 197 | DEBUG | `PeerCaching` property probe -- sets false on failure |
| 205 | DEBUG | `PersistInCache` property probe -- sets null on failure |
| 213 | DEBUG | `ReferenceCount` property probe -- sets null on failure |

### Win32_Service.cs (3 blocks)
| Line | Category | Context |
|------|----------|---------|
| 132 | SURFACE | `SetStartupManual` -- Set-Service call, returns false |
| 148 | SURFACE | `SetStartupAutomatic` -- Set-Service call, returns false |
| 164 | SURFACE | `SetStartupDisabled` -- Set-Service call, returns false |

### policy/localpolicy.cs (1 block)
| Line | Category | Context |
|------|----------|---------|
| 34 | DEBUG | Policy XML decompression -- returns raw XML on failure |

### schedule/ScheduleDecoding.cs (1 block)
| Line | Category | Context |
|------|----------|---------|
| 257 | DEBUG | Schedule ID decoding -- returns null on parse failure |

---

## WPF Application (SCCMCliCtrWPF/)

### MainPage.xaml.cs (28 blocks)
| Line | Category | Context |
|------|----------|---------|
| 73 | DEBUG | Window_Loaded -- visibility settings for install/repair panel |
| 118 | DEBUG | Plugin loading -- `Activator.CreateInstance` for AgentAction tools |
| 150 | DEBUG | Plugin loading -- `Activator.CreateInstance` for CustomTools |
| 182 | DEBUG | Plugin loading -- `Activator.CreateInstance` for MainMenu items |
| 187 | DEBUG | Plugin loading -- inner type loop catch |
| 190 | DEBUG | Plugin loading -- assembly type enumeration |
| 194 | DEBUG | Plugin loading -- outer assembly loading catch |
| 265 | UNVERIFIED | Command-line args processing |
| 293 | SURFACE | Console extension registration (`/RegisterConsole` arg) |
| 304 | SURFACE | Console extension unregistration (`/UnRegisterConsole` arg) |
| 338 | UNVERIFIED | Connection parameter setup |
| 435 | SILENT-OK | `Current_Exit` -- close monitoring script |
| 441 | SILENT-OK | `Current_Exit` -- disconnect agent |
| 465 | SURFACE | `bt_Connect_Click` -- stop monitoring and disconnect |
| 549 | SURFACE | `bt_Connect_Click` -- save settings after connect |
| 684 | UNVERIFIED | Tab switching / agent action invocation |
| 688 | UNVERIFIED | Outer catch around tab switching |
| 1081 | DEBUG | AutoComplete populate -- ItemsSource binding |
| 1114 | SURFACE-ALREADY | `bt_Ping_Click` -- catch appends "Unable to ping" to rStatus |
| 1130 | SURFACE-ALREADY | `bt_RegConsole_Click` -- catch calls `myTrace.WriteError(...)` |
| 1142 | SURFACE | `bt_UnRegConsole_Click` -- catch calls `myTrace.WriteError(...)` (already has error msg but using bare catch) |
| 1159 | SURFACE | `btResetPausedSWDist_Click` -- reset paused SW dist |
| 1171 | SURFACE | `btResetProvisioningMode_Click` -- reset provisioning mode |
| 1183 | SURFACE | `btResetSystemTaskExclude_Click` -- reset system task exclude |
| 1195 | SURFACE | `btDeleteIsCacheCopyNeededCallBack_Click` -- delete callback |
| 1206 | SURFACE | `btRefreshServerComplianceState_Click` -- already has `myTrace.WriteError` in catch body |
| 1223 | SURFACE | `bt_CleanBoundaryGroupCache_Click` -- already has `myTrace.WriteError` in catch body |
| 1430 | SILENT-OK | File.Delete for console extension XML -- cleanup |

### Logs.cs (2 blocks)
| Line | Category | Context |
|------|----------|---------|
| 33 | DEBUG | Log line parsing -- CMTrace/text format DateTime parse |
| 56 | DEBUG | Log line parsing -- SCCM log format DateTime parse |

### Controls/About.xaml.cs (1 block)
| Line | Category | Context |
|------|----------|---------|
| 47 | DEBUG | `Process.Start` PayPal donation URL |

### Controls/AdvertisementGrid.xaml.cs (7 blocks)
| Line | Category | Context |
|------|----------|---------|
| 75 | UNVERIFIED | Grid data binding in Dispatcher callback |
| 113 | UNVERIFIED | Grid reload with sort info |
| 132 | UNVERIFIED | Detail grid binding on selection changed |
| 201 | DEBUG | Date parse fallback -- tries DMTF then falls back to string escaping |
| 249 | SURFACE | `TriggerSchedule` from advertisement right-click action |
| 269 | UNVERIFIED | Status reload grid rebinding |
| 412 | UNVERIFIED | Value converter -- returns null on failure |

### Controls/AgentComponents.xaml.cs (2 blocks)
| Line | Category | Context |
|------|----------|---------|
| 54 | UNVERIFIED | Grid binding for installed components |
| 58 | UNVERIFIED | Outer catch wrapping component load |

### Controls/AgentSettingItem.xaml.cs (3 blocks)
| Line | Category | Context |
|------|----------|---------|
| 77 | UNVERIFIED | Agent setting load -- version/MP refresh |
| 285 | DEBUG | Property probe -- `LocalSCCMAgentLogPath` for Explorer launch |
| 379 | DEBUG | Property probe -- `CachePath` for Explorer launch |

### Controls/ApplicationGrid.xaml.cs (4 blocks)
| Line | Category | Context |
|------|----------|---------|
| 85 | UNVERIFIED | Grid binding for applications in Dispatcher callback |
| 115 | UNVERIFIED | Grid reload with sort info |
| 381 | UNVERIFIED | Image converter -- BitmapImage from URI, returns null |
| 406 | UNVERIFIED | Image converter -- BitmapImage from stream, returns null |

### Controls/CCMEvalGrid.xaml.cs (3 blocks)
| Line | Category | Context |
|------|----------|---------|
| 66 | UNVERIFIED | Grid binding for CCM eval results |
| 99 | UNVERIFIED | Grid reload with sort and last date |
| 154 | SURFACE | `RunCCMEval` button click handler |

### Controls/CacheGrid.xaml.cs (5 blocks)
| Line | Category | Context |
|------|----------|---------|
| 68 | UNVERIFIED | Cache grid load -- grid binding and size calculation |
| 107 | DEBUG | Property probe -- `CachePath` for Explorer launch |
| 179 | UNVERIFIED | Cache reload -- grid binding and size calculation |
| 203 | DEBUG | Cache item Location property probe for Explorer open |
| 312 | UNVERIFIED | Cache delete/reload -- grid rebinding |

### Controls/CollectionVariables.xaml.cs (2 blocks)
| Line | Category | Context |
|------|----------|---------|
| 54 | UNVERIFIED | `dataGrid1.Items.Clear()` on agent change |
| 105 | UNVERIFIED | Collection variable decoding/display |

### Controls/EventMonitoring.xaml.cs (6 blocks)
| Line | Category | Context |
|------|----------|---------|
| 61 | SURFACE | Stop monitoring -- close async script, reset buttons |
| 64 | UNVERIFIED | Outer catch wrapping monitoring stop |
| 183 | UNVERIFIED | Tree view item add for monitoring event |
| 198 | SURFACE | `bt_StopMonitoring_Click` -- close async script |
| 209 | DEBUG | `bt_ClearMonitoring_Click` -- `treeView1.Items.Clear()` |
| 221 | DEBUG | Clipboard.SetData in KeyDown handler |

### Controls/ExecHistoryGrid.xaml.cs (5 blocks)
| Line | Category | Context |
|------|----------|---------|
| 55 | UNVERIFIED | Grid binding for execution history |
| 73 | SURFACE | Delete execution history registry entry |
| 82 | UNVERIFIED | Grid rebinding after delete |
| 104 | UNVERIFIED | Grid rebinding after SID resolution |
| 118 | UNVERIFIED | KeyDown handler -- calls delete on Delete key |

### Controls/InstallAgent.xaml.cs (2 blocks)
| Line | Category | Context |
|------|----------|---------|
| 47 | DEBUG | ScrollViewer height calculation from text line count |
| 58 | DEBUG | `RefreshMPandSiteCode` -- site code/MP text update |

### Controls/InstalledSoftwareGrid.xaml.cs (3 blocks)
| Line | Category | Context |
|------|----------|---------|
| 61 | UNVERIFIED | Grid binding for installed software |
| 85 | UNVERIFIED | Grid reload with rebinding |
| 181 | UNVERIFIED | Image converter -- software icon, returns null |

### Controls/InstallRepair.xaml.cs (2 blocks)
| Line | Category | Context |
|------|----------|---------|
| 48 | UNVERIFIED | Agent property setter |
| 280 | SURFACE | Install/repair dialog setup -- site code and MP refresh |

### Controls/LogGrid.xaml.cs (1 block)
| Line | Category | Context |
|------|----------|---------|
| 52 | DEBUG | Row formatting -- conditional yellow background for warnings |

### Controls/LogViewer.xaml.cs (8 blocks)
| Line | Category | Context |
|------|----------|---------|
| 57 | UNVERIFIED | Tab/menu items clear on agent reset |
| 60 | UNVERIFIED | Outer catch wrapping tab clear |
| 110 | UNVERIFIED | Log tab creation from file list |
| 119 | UNVERIFIED | Tab selection after load |
| 122 | UNVERIFIED | Outer catch wrapping log viewer load |
| 145 | UNVERIFIED | Checkbox menu population from log files |
| 163 | UNVERIFIED | Checkbox uncheck loop in clear handler |
| 168 | UNVERIFIED | Outer catch wrapping clear handler |

### Controls/PowerSettings.xaml.cs (1 block)
| Line | Category | Context |
|------|----------|---------|
| 60 | UNVERIFIED | Grid binding for power settings |

### Controls/ProcessGrid.xaml.cs (3 blocks)
| Line | Category | Context |
|------|----------|---------|
| 61 | UNVERIFIED | Grid binding for processes |
| 64 | UNVERIFIED | Outer catch wrapping process load |
| 133 | UNVERIFIED | Grid reload with sort |

### Controls/ServicesGrid.xaml.cs (1 block)
| Line | Category | Context |
|------|----------|---------|
| 58 | UNVERIFIED | Grid binding for services |

### Controls/ServiceWindowGrid.xaml.cs (6 blocks)
| Line | Category | Context |
|------|----------|---------|
| 108 | UNVERIFIED | Service window schedule enumeration |
| 295 | UNVERIFIED | Schedule type switch handling |
| 305 | SURFACE | `bt_Reload_Click` -- delete/reload service windows |
| 343 | SURFACE | `bt_NewServiceWindow_Click` -- create new service window |
| 365 | SURFACE | `bt_CleanServiceWindow_Click` -- delete expired windows |
| 372 | SURFACE | Outer catch wrapping clean operation |

### Controls/SettingsMgmt.xaml.cs (5 blocks)
| Line | Category | Context |
|------|----------|---------|
| 62 | UNVERIFIED | Grid binding for DCM baselines |
| 88 | UNVERIFIED | Grid reload with rebinding |
| 104 | UNVERIFIED | Detail grid binding on selection changed |
| 120 | SURFACE | `TriggerEvaluation` for selected baseline |
| 123 | SURFACE | Outer catch wrapping baseline evaluation loop |

### Controls/SWAllUpdatesGrid.xaml.cs (2 blocks)
| Line | Category | Context |
|------|----------|---------|
| 68 | UNVERIFIED | Grid binding for all software updates |
| 71 | UNVERIFIED | Outer catch wrapping update load |

### Controls/SWStatusGrid.xaml.cs (3 blocks)
| Line | Category | Context |
|------|----------|---------|
| 62 | UNVERIFIED | Grid binding for software status |
| 91 | UNVERIFIED | Grid reload with sort |
| 110 | UNVERIFIED | Grid reload active items with sort |

### Controls/SWUpdatesGrid.xaml.cs (2 blocks)
| Line | Category | Context |
|------|----------|---------|
| 60 | UNVERIFIED | Grid binding for software updates |
| 63 | UNVERIFIED | Outer catch wrapping update load |

### Controls/WMIBrowser.xaml.cs (4 blocks)
| Line | Category | Context |
|------|----------|---------|
| 55 | UNVERIFIED | Adhoc inventory menu population |
| 85 | UNVERIFIED | WMI class enumeration for combobox |
| 161 | UNVERIFIED | WMI object property display in tree view |
| 217 | DEBUG | Control clear -- `tvObjects.Items.Clear()` |

---

## Plugins

### Plugin_AppV46/AppVForm.xaml-NUC1.cs (11 blocks)
| Line | Category | Context |
|------|----------|---------|
| 107 | DEBUG | AppV registry probe -- cache size / drive letter |
| 113 | SURFACE | Outer catch wrapping AppV form load |
| 134 | SURFACE | `bt_AppReload_Click` -- grid binding |
| 155 | SURFACE | `bt_PkgReload_Click` -- grid binding |
| 174 | SURFACE | `miDeletePackage_Click` -- delete package via PS |
| 192 | SURFACE | `miUnloadPackage_Click` -- unload package via PS |
| 210 | SURFACE | `miLoadPackage_Click` -- load package via PS |
| 228 | SURFACE | `miDeleteApp_Click` -- delete app via PS |
| 246 | SURFACE | `miUnloadApp_Click` -- unload app via PS |
| 264 | SURFACE | `miLoadApp_Click` -- load app via PS |
| 282 | SURFACE | `miRepairApp_Click` -- repair app via PS |

### Plugin_AppV46/AppVForm.xaml.cs (11 blocks)
| Line | Category | Context |
|------|----------|---------|
| 100 | DEBUG | AppV registry probe -- cache size / drive letter |
| 106 | SURFACE | Outer catch wrapping AppV form load |
| 127 | SURFACE | `bt_AppReload_Click` -- grid binding |
| 148 | SURFACE | `bt_PkgReload_Click` -- grid binding |
| 167 | SURFACE | `miDeletePackage_Click` -- delete package via PS |
| 185 | SURFACE | `miUnloadPackage_Click` -- unload package via PS |
| 203 | SURFACE | `miLoadPackage_Click` -- load package via PS |
| 221 | SURFACE | `miDeleteApp_Click` -- delete app via PS |
| 239 | SURFACE | `miUnloadApp_Click` -- unload app via PS |
| 257 | SURFACE | `miLoadApp_Click` -- load app via PS |
| 275 | SURFACE | `miRepairApp_Click` -- repair app via PS |

### Plugin_CustomTools_AMTTools/AgentActionTool_AMTTools.xaml.cs (5 blocks)
| Line | Category | Context |
|------|----------|---------|
| 146 | DEBUG | vPro tool download -- `_RunPS` for sWget command |
| 162 | DEBUG | vPro tool unzip -- `_RunPS` for unzip command |
| 170 | DEBUG | vPro MSI unpack -- `_RunPS` for msiexec command |
| 180 | DEBUG | vPro file copy -- `_RunPS` for Copy-Item command |
| 195 | SURFACE | Outer catch wrapping entire vPro tool install process |

### Plugin_Explorer/CustomTools_Explorer.xaml.cs (3 blocks)
| Line | Category | Context |
|------|----------|---------|
| 36 | DEBUG | Ribbon button creation -- BitmapImage/tooltip in loop |
| 92 | SURFACE | `btC_Click` -- Explorer.exe launch to remote share |
| 121 | DEBUG | XML config parse -- folder list with fallback to Settings |

### Plugin_Explorer/Explorer.xaml.cs (3 blocks)
| Line | Category | Context |
|------|----------|---------|
| 42 | DEBUG | Ribbon button creation -- BitmapImage/tooltip in loop |
| 97 | SURFACE | `btC_Click` -- Explorer.exe launch to remote share |
| 126 | DEBUG | XML config parse -- folder list with fallback to Settings |

### Plugin_PSScripts/Plugin_PSScripts.xaml.cs (4 blocks)
| Line | Category | Context |
|------|----------|---------|
| 49 | DEBUG | Script directory load -- `LoadPSFolders` init |
| 68 | DEBUG | PS script menu item creation in loop |
| 85 | DEBUG | PS subfolder menu creation in loop |
| 109 | SURFACE | `PSFolder_MouseDown` -- Explorer.exe open to script folder |

### Plugin_Regedit/AgentActionTool_Regedit.xaml.cs (1 block)
| Line | Category | Context |
|------|----------|---------|
| 28 | DEBUG | Button tooltip -- check if regedit feature is enabled |

### Plugin_RemoteTools/AgentActionTool_CMRemote.xaml.cs (1 block)
| Line | Category | Context |
|------|----------|---------|
| 64 | SILENT-OK | Property probe -- SCCM console UI path from registry, returns "" |

### Plugin_RemoteTools/CustomTools_CMRemote.xaml.cs (1 block)
| Line | Category | Context |
|------|----------|---------|
| 86 | SILENT-OK | Property probe -- SCCM console UI path from registry, returns "" |

### Plugin_ResourceExplorer/AgentActionTool_CMRemote.xaml.cs (3 blocks)
| Line | Category | Context |
|------|----------|---------|
| 65 | SILENT-OK | Property probe -- SCCM console UI path from registry, returns "" |
| 99 | SILENT-OK | Property probe -- SCCM server name from registry, returns "" |
| 114 | DEBUG | WMI query -- `SiteCode` from ManagementObject, returns "" |

### Plugin_ResourceExplorer/CustomTools_CMResource.xaml.cs (3 blocks)
| Line | Category | Context |
|------|----------|---------|
| 94 | SILENT-OK | Property probe -- SCCM console UI path from registry, returns "" |
| 128 | SILENT-OK | Property probe -- SCCM server name from registry, returns "" |
| 143 | DEBUG | WMI query -- `SiteCode` from ManagementObject, returns "" |

### Plugin_StatusMessageViewer/AgentActionTool_StatusMessage.xaml.cs (3 blocks)
| Line | Category | Context |
|------|----------|---------|
| 65 | SILENT-OK | Property probe -- SCCM console UI path from registry, returns "" |
| 99 | SILENT-OK | Property probe -- SCCM server name from registry, returns "" |
| 114 | DEBUG | WMI query -- `SiteCode` from ManagementObject, returns "" |

### Plugin_StatusMessageViewer/CustomTools_StatusMessage.xaml.cs (3 blocks)
| Line | Category | Context |
|------|----------|---------|
| 95 | SILENT-OK | Property probe -- SCCM console UI path from registry, returns "" |
| 129 | SILENT-OK | Property probe -- SCCM server name from registry, returns "" |
| 144 | DEBUG | WMI query -- `SiteCode` from ManagementObject, returns "" |
