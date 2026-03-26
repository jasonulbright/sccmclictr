// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.functions.inventory
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

#nullable disable
namespace sccmclictr.automation.functions;

/// <summary>Inventory Class</summary>
public class inventory : baseInit
{
  internal Runspace remoteRunspace;
  internal TraceSource pSCode;
  internal ccm baseClient;

  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.inventory" /> class.
  /// </summary>
  /// <param name="RemoteRunspace">The remote runspace.</param>
  /// <param name="PSCode">The PowerShell code.</param>
  /// <param name="oClient">A CCM Client object.</param>
  public inventory(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
    : base(RemoteRunspace, PSCode)
  {
    this.remoteRunspace = RemoteRunspace;
    this.pSCode = PSCode;
    this.baseClient = oClient;
  }

  /// <summary>Show all Installed Software (like AddRemove Programs)</summary>
  public List<inventory.AI_InstalledSoftwareCache> InstalledSoftware
  {
    get
    {
      List<inventory.AI_InstalledSoftwareCache> installedSoftware = new List<inventory.AI_InstalledSoftwareCache>();
      foreach (PSObject cimObject in this.GetCimObjects("root\\CIMV2\\sms", "SELECT * FROM SMS_InstalledSoftware", true))
      {
        try
        {
          installedSoftware.Add(new inventory.AI_InstalledSoftwareCache(cimObject, this.remoteRunspace, this.pSCode)
          {
            remoteRunspace = this.remoteRunspace,
            pSCode = this.pSCode
          });
        }
        catch
        {
        }
      }
      return installedSoftware;
    }
  }

  /// <summary>Status of Inventory Actions</summary>
  public List<inventory.InventoryActionStatus> InventoryActionStatusList
  {
    get
    {
      List<inventory.InventoryActionStatus> actionStatusList = new List<inventory.InventoryActionStatus>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\invagt", "SELECT * FROM InventoryActionStatus"))
      {
        try
        {
          actionStatusList.Add(new inventory.InventoryActionStatus(WMIObject, this.remoteRunspace, this.pSCode)
          {
            remoteRunspace = this.remoteRunspace,
            pSCode = this.pSCode
          });
        }
        catch
        {
        }
      }
      return actionStatusList;
    }
  }

  /// <summary>Get System Power Management Capabilities</summary>
  /// <returns></returns>
  public inventory.CCM_PwrMgmtSystemPowerCapabilities PwrMgmtSystemPowerCapabilities()
  {
    using (List<PSObject>.Enumerator enumerator = this.GetObjects("ROOT\\ccm\\PowerManagementAgent", "SELECT * FROM CCM_PwrMgmtSystemPowerCapabilities", false, new TimeSpan(0, 1, 0)).GetEnumerator())
    {
      if (enumerator.MoveNext())
        return new inventory.CCM_PwrMgmtSystemPowerCapabilities(enumerator.Current, this.remoteRunspace, this.pSCode);
    }
    return (inventory.CCM_PwrMgmtSystemPowerCapabilities) null;
  }

  /// <summary>List of Daily PowerMgmt data</summary>
  public List<inventory.CCM_PwrMgmtActualDay> PwrMgmtActualDay
  {
    get
    {
      List<inventory.CCM_PwrMgmtActualDay> pwrMgmtActualDay1 = new List<inventory.CCM_PwrMgmtActualDay>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\PowerManagementAgent", "SELECT * FROM CCM_PwrMgmtActualDay", false, new TimeSpan(0, 1, 0)))
      {
        inventory.CCM_PwrMgmtActualDay pwrMgmtActualDay2 = new inventory.CCM_PwrMgmtActualDay(WMIObject, this.remoteRunspace, this.pSCode);
        pwrMgmtActualDay1.Add(pwrMgmtActualDay2);
      }
      return pwrMgmtActualDay1;
    }
  }

  /// <summary>Monthly PwrMgmt data</summary>
  public List<inventory.CCM_PwrMgmtMonth> PwrMgmtMonth
  {
    get
    {
      List<inventory.CCM_PwrMgmtMonth> pwrMgmtMonth = new List<inventory.CCM_PwrMgmtMonth>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\PowerManagementAgent", "SELECT * FROM CCM_PwrMgmtMonth", false, new TimeSpan(0, 5, 0)))
      {
        inventory.CCM_PwrMgmtMonth ccmPwrMgmtMonth = new inventory.CCM_PwrMgmtMonth(WMIObject, this.remoteRunspace, this.pSCode);
        pwrMgmtMonth.Add(ccmPwrMgmtMonth);
      }
      return pwrMgmtMonth;
    }
  }

  /// <summary>Current Power Settings on the System</summary>
  /// <param name="Reload"></param>
  /// <returns></returns>
  public List<inventory.SMS_PowerSettings> PowerSettings(bool Reload)
  {
    List<inventory.SMS_PowerSettings> smsPowerSettingsList = new List<inventory.SMS_PowerSettings>();
    foreach (PSObject WMIObject in this.GetObjects("ROOT\\cimv2\\sms", "SELECT * FROM SMS_PowerSettings", Reload))
    {
      try
      {
        inventory.SMS_PowerSettings smsPowerSettings = new inventory.SMS_PowerSettings(WMIObject, this.remoteRunspace, this.pSCode);
        smsPowerSettingsList.Add(smsPowerSettings);
      }
      catch
      {
      }
    }
    return smsPowerSettingsList;
  }

  /// <summary>Return OS Architecture (x64 or x86)</summary>
  public string OSArchitecture
  {
    get
    {
      TimeSpan cacheTime = this.cacheTime;
      this.cacheTime = new TimeSpan(0, 15, 0);
      string stringFromPs = this.GetStringFromPS("(Get-CimInstance Win32_Processor | where {$_.DeviceID -eq 'CPU0'}).AddressWidth");
      this.cacheTime = cacheTime;
      return string.Compare("64", stringFromPs, true) == 0 ? "x64" : "x86";
    }
  }

  /// <summary>Return OS version e.g. "10.0.14393"</summary>
  public string OSVersion
  {
    get
    {
      TimeSpan cacheTime = this.cacheTime;
      this.cacheTime = new TimeSpan(0, 15, 0);
      string stringFromPs = this.GetStringFromPS("(Get-CimInstance Win32_OperatingSystem).Version");
      this.cacheTime = cacheTime;
      return stringFromPs;
    }
  }

  /// <summary>Return True if OS is x64 Architecture</summary>
  public bool isx64OS => this.OSArchitecture == "x64";

  /// <summary>Source:ROOT\ccm\invagt</summary>
  public class AI_InstalledSoftwareCache
  {
    internal baseInit oNewBase;
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.inventory.AI_InstalledSoftwareCache" /> class.
    /// </summary>
    /// <param name="WMIObject">A WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public AI_InstalledSoftwareCache(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.oNewBase = new baseInit(this.remoteRunspace, this.pSCode);
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)] == null ? WMIObject.Properties["CimClass"].Value as string : WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__CLASS = WMIObject.Properties[nameof (__NAMESPACE)] == null ? "" : WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__CLASS = WMIObject.Properties[nameof (__RELPATH)] == null ? "" : WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.ARPDisplayName = WMIObject.Properties[nameof (ARPDisplayName)].Value as string;
      this.ChannelCode = WMIObject.Properties[nameof (ChannelCode)].Value as string;
      this.ChannelID = WMIObject.Properties[nameof (ChannelID)].Value as string;
      this.CM_DSLID = WMIObject.Properties[nameof (CM_DSLID)].Value as string;
      this.EvidenceSource = WMIObject.Properties[nameof (EvidenceSource)].Value as string;
      try
      {
        DateTime? nullable = WMIObject.Properties[nameof (InstallDate)].Value as DateTime?;
        this.InstallDate = !nullable.HasValue ? (WMIObject.Properties[nameof (InstallDate)].Value == null ? "" : WMIObject.Properties[nameof (InstallDate)].Value.ToString()) : nullable.Value.ToString("u");
      }
      catch
      {
      }
      this.InstallDirectoryValidation = WMIObject.Properties[nameof (InstallDirectoryValidation)].Value as string;
      this.InstalledLocation = WMIObject.Properties[nameof (InstalledLocation)].Value as string;
      this.InstallSource = WMIObject.Properties[nameof (InstallSource)].Value as string;
      this.InstallType = WMIObject.Properties[nameof (InstallType)].Value as string;
      this.Language = WMIObject.Properties[nameof (Language)].Value as string;
      this.LocalPackage = WMIObject.Properties[nameof (LocalPackage)].Value as string;
      this.MPC = WMIObject.Properties[nameof (MPC)].Value as string;
      this.OsComponent = WMIObject.Properties[nameof (OsComponent)].Value as string;
      this.PackageCode = WMIObject.Properties[nameof (PackageCode)].Value as string;
      this.ProductID = WMIObject.Properties[nameof (ProductID)].Value as string;
      this.ProductName = WMIObject.Properties[nameof (ProductName)].Value as string;
      this.ProductVersion = WMIObject.Properties[nameof (ProductVersion)].Value as string;
      this.Publisher = WMIObject.Properties[nameof (Publisher)].Value as string;
      this.RegisteredUser = WMIObject.Properties[nameof (RegisteredUser)].Value as string;
      this.ServicePack = WMIObject.Properties[nameof (ServicePack)].Value as string;
      this.SoftwareCode = WMIObject.Properties[nameof (SoftwareCode)].Value as string;
      this.SoftwarePropertiesHash = WMIObject.Properties[nameof (SoftwarePropertiesHash)].Value as string;
      this.SoftwarePropertiesHashEx = WMIObject.Properties[nameof (SoftwarePropertiesHashEx)].Value as string;
      this.UninstallString = WMIObject.Properties[nameof (UninstallString)].Value as string;
      this.UpgradeCode = WMIObject.Properties[nameof (UpgradeCode)].Value as string;
      this.VersionMajor = WMIObject.Properties[nameof (VersionMajor)].Value as string;
      this.VersionMinor = WMIObject.Properties[nameof (VersionMinor)].Value as string;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    public string ARPDisplayName { get; set; }

    public string ChannelCode { get; set; }

    public string ChannelID { get; set; }

    public string CM_DSLID { get; set; }

    public string EvidenceSource { get; set; }

    public string InstallDate { get; set; }

    public string InstallDirectoryValidation { get; set; }

    public string InstalledLocation { get; set; }

    public string InstallSource { get; set; }

    public string InstallType { get; set; }

    public string Language { get; set; }

    public string LocalPackage { get; set; }

    public string MPC { get; set; }

    public string OsComponent { get; set; }

    public string PackageCode { get; set; }

    public string ProductID { get; set; }

    public string ProductName { get; set; }

    public string ProductVersion { get; set; }

    public string Publisher { get; set; }

    public string RegisteredUser { get; set; }

    public string ServicePack { get; set; }

    public string SoftwareCode { get; set; }

    public string SoftwarePropertiesHash { get; set; }

    public string SoftwarePropertiesHashEx { get; set; }

    public string UninstallString { get; set; }

    public string UpgradeCode { get; set; }

    public string VersionMajor { get; set; }

    public string VersionMinor { get; set; }

    /// <summary>Uninstalls this instance.</summary>
    /// <returns>System.String.</returns>
    public string Uninstall()
    {
      return this.SoftwareCode.StartsWith("{") ? this.oNewBase.GetStringFromPS($"& msiexec.exe /x '{this.SoftwareCode}' REBOOT=ReallySuppress /q") : (string) null;
    }

    /// <summary>Repairs this instance.</summary>
    /// <returns>System.String.</returns>
    public string Repair()
    {
      return this.SoftwareCode.StartsWith("{") ? this.oNewBase.GetStringFromPS($"& msiexec.exe /fpecmsu '{this.SoftwareCode}' REBOOT=ReallySuppress REINSTALL=ALL /q") : (string) null;
    }
  }

  /// <summary>Source:ROOT\ccm\invagt</summary>
  public class InventoryActionStatus
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.inventory.InventoryActionStatus" /> class.
    /// </summary>
    /// <param name="WMIObject">A WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public InventoryActionStatus(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.InventoryActionID = WMIObject.Properties[nameof (InventoryActionID)].Value as string;
      string dmtfDate1 = WMIObject.Properties[nameof (LastCycleStartedDate)].Value as string;
      this.LastCycleStartedDate = !string.IsNullOrEmpty(dmtfDate1) ? new DateTime?(common.DmtfToDateTime(dmtfDate1)) : new DateTime?();
      this.LastMajorReportVersion = WMIObject.Properties[nameof (LastMajorReportVersion)].Value as uint?;
      this.LastMinorReportVersion = WMIObject.Properties[nameof (LastMinorReportVersion)].Value as uint?;
      string dmtfDate2 = WMIObject.Properties[nameof (LastReportDate)].Value as string;
      if (string.IsNullOrEmpty(dmtfDate2))
        this.LastReportDate = new DateTime?();
      else
        this.LastReportDate = new DateTime?(common.DmtfToDateTime(dmtfDate2));
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    public string InventoryActionID { get; set; }

    public DateTime? LastCycleStartedDate { get; set; }

    public uint? LastMajorReportVersion { get; set; }

    public uint? LastMinorReportVersion { get; set; }

    public DateTime? LastReportDate { get; set; }
  }

  /// <summary>Source:ROOT\ccm\PowerManagementAgent</summary>
  public class CCM_PwrMgmtActualDay
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    public CCM_PwrMgmtActualDay(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      string dmtfDate = WMIObject.Properties[nameof (Date)].Value as string;
      this.Date = !string.IsNullOrEmpty(dmtfDate) ? new DateTime?(common.DmtfToDateTime(dmtfDate)) : new DateTime?();
      this.hr0_1 = WMIObject.Properties[nameof (hr0_1)].Value as uint?;
      this.hr10_11 = WMIObject.Properties[nameof (hr10_11)].Value as uint?;
      this.hr11_12 = WMIObject.Properties[nameof (hr11_12)].Value as uint?;
      this.hr12_13 = WMIObject.Properties[nameof (hr12_13)].Value as uint?;
      this.hr13_14 = WMIObject.Properties[nameof (hr13_14)].Value as uint?;
      this.hr14_15 = WMIObject.Properties[nameof (hr14_15)].Value as uint?;
      this.hr15_16 = WMIObject.Properties[nameof (hr15_16)].Value as uint?;
      this.hr16_17 = WMIObject.Properties[nameof (hr16_17)].Value as uint?;
      this.hr17_18 = WMIObject.Properties[nameof (hr17_18)].Value as uint?;
      this.hr18_19 = WMIObject.Properties[nameof (hr18_19)].Value as uint?;
      this.hr19_20 = WMIObject.Properties[nameof (hr19_20)].Value as uint?;
      this.hr1_2 = WMIObject.Properties[nameof (hr1_2)].Value as uint?;
      this.hr20_21 = WMIObject.Properties[nameof (hr20_21)].Value as uint?;
      this.hr21_22 = WMIObject.Properties[nameof (hr21_22)].Value as uint?;
      this.hr22_23 = WMIObject.Properties[nameof (hr22_23)].Value as uint?;
      this.hr23_0 = WMIObject.Properties[nameof (hr23_0)].Value as uint?;
      this.hr2_3 = WMIObject.Properties[nameof (hr2_3)].Value as uint?;
      this.hr3_4 = WMIObject.Properties[nameof (hr3_4)].Value as uint?;
      this.hr4_5 = WMIObject.Properties[nameof (hr4_5)].Value as uint?;
      this.hr5_6 = WMIObject.Properties[nameof (hr5_6)].Value as uint?;
      this.hr6_7 = WMIObject.Properties[nameof (hr6_7)].Value as uint?;
      this.hr7_8 = WMIObject.Properties[nameof (hr7_8)].Value as uint?;
      this.hr8_9 = WMIObject.Properties[nameof (hr8_9)].Value as uint?;
      this.hr9_10 = WMIObject.Properties[nameof (hr9_10)].Value as uint?;
      this.minutesTotal = WMIObject.Properties[nameof (minutesTotal)].Value as uint?;
      this.TypeOfEvent = WMIObject.Properties[nameof (TypeOfEvent)].Value as string;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    public DateTime? Date { get; set; }

    public uint? hr0_1 { get; set; }

    public uint? hr10_11 { get; set; }

    public uint? hr11_12 { get; set; }

    public uint? hr12_13 { get; set; }

    public uint? hr13_14 { get; set; }

    public uint? hr14_15 { get; set; }

    public uint? hr15_16 { get; set; }

    public uint? hr16_17 { get; set; }

    public uint? hr17_18 { get; set; }

    public uint? hr18_19 { get; set; }

    public uint? hr19_20 { get; set; }

    public uint? hr1_2 { get; set; }

    public uint? hr20_21 { get; set; }

    public uint? hr21_22 { get; set; }

    public uint? hr22_23 { get; set; }

    public uint? hr23_0 { get; set; }

    public uint? hr2_3 { get; set; }

    public uint? hr3_4 { get; set; }

    public uint? hr4_5 { get; set; }

    public uint? hr5_6 { get; set; }

    public uint? hr6_7 { get; set; }

    public uint? hr7_8 { get; set; }

    public uint? hr8_9 { get; set; }

    public uint? hr9_10 { get; set; }

    public uint? minutesTotal { get; set; }

    public string TypeOfEvent { get; set; }
  }

  /// <summary>Source:ROOT\ccm\PowerManagementAgent</summary>
  public class CCM_PwrMgmtInternalAgentState
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    public CCM_PwrMgmtInternalAgentState(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      string dmtfDate = WMIObject.Properties[nameof (LKGTime)].Value as string;
      if (string.IsNullOrEmpty(dmtfDate))
        this.LKGTime = new DateTime?();
      else
        this.LKGTime = new DateTime?(common.DmtfToDateTime(dmtfDate));
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    public DateTime? LKGTime { get; set; }
  }

  /// <summary>Source:ROOT\ccm\PowerManagementAgent</summary>
  public class CCM_PwrMgmtInternalEventStateCookie
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    public CCM_PwrMgmtInternalEventStateCookie(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.ClientID = WMIObject.Properties[nameof (ClientID)].Value as string;
      this.ComputerActiveState = WMIObject.Properties[nameof (ComputerActiveState)].Value as uint?;
      this.ComputerOnState = WMIObject.Properties[nameof (ComputerOnState)].Value as uint?;
      this.ComputerShutdownState = WMIObject.Properties[nameof (ComputerShutdownState)].Value as uint?;
      this.ComputerSleepState = WMIObject.Properties[nameof (ComputerSleepState)].Value as uint?;
      string dmtfDate = WMIObject.Properties[nameof (LastRecordedDate)].Value as string;
      this.LastRecordedDate = !string.IsNullOrEmpty(dmtfDate) ? new DateTime?(common.DmtfToDateTime(dmtfDate)) : new DateTime?();
      this.MonitorOnState = WMIObject.Properties[nameof (MonitorOnState)].Value as uint?;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    public string ClientID { get; set; }

    public uint? ComputerActiveState { get; set; }

    public uint? ComputerOnState { get; set; }

    public uint? ComputerShutdownState { get; set; }

    public uint? ComputerSleepState { get; set; }

    public DateTime? LastRecordedDate { get; set; }

    public uint? MonitorOnState { get; set; }
  }

  /// <summary>Source:ROOT\ccm\PowerManagementAgent</summary>
  public class CCM_PwrMgmtInternalRawEvent
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    public CCM_PwrMgmtInternalRawEvent(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.eventID = WMIObject.Properties[nameof (eventID)].Value as uint?;
      this.GUID = WMIObject.Properties[nameof (GUID)].Value as string;
      string dmtfDate = WMIObject.Properties[nameof (time)].Value as string;
      if (string.IsNullOrEmpty(dmtfDate))
        this.time = new DateTime?();
      else
        this.time = new DateTime?(common.DmtfToDateTime(dmtfDate));
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    public uint? eventID { get; set; }

    public string GUID { get; set; }

    public DateTime? time { get; set; }
  }

  /// <summary>Source:ROOT\ccm\PowerManagementAgent</summary>
  public class CCM_PwrMgmtLastSuspendError
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    public CCM_PwrMgmtLastSuspendError(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.AdditionalCode = WMIObject.Properties[nameof (AdditionalCode)].Value as uint?;
      this.AdditionalInfo = WMIObject.Properties[nameof (AdditionalInfo)].Value as string;
      this.Requester = WMIObject.Properties[nameof (Requester)].Value as string;
      this.RequesterInfo = WMIObject.Properties[nameof (RequesterInfo)].Value as string;
      this.RequesterType = WMIObject.Properties[nameof (RequesterType)].Value as string;
      this.RequestType = WMIObject.Properties[nameof (RequestType)].Value as string;
      string dmtfDate = WMIObject.Properties[nameof (Time)].Value as string;
      this.Time = !string.IsNullOrEmpty(dmtfDate) ? new DateTime?(common.DmtfToDateTime(dmtfDate)) : new DateTime?();
      this.UnknownRequester = WMIObject.Properties[nameof (UnknownRequester)].Value as bool?;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    public uint? AdditionalCode { get; set; }

    public string AdditionalInfo { get; set; }

    public string Requester { get; set; }

    public string RequesterInfo { get; set; }

    public string RequesterType { get; set; }

    public string RequestType { get; set; }

    public DateTime? Time { get; set; }

    public bool? UnknownRequester { get; set; }
  }

  /// <summary>Source:ROOT\ccm\PowerManagementAgent</summary>
  public class CCM_PwrMgmtMonth
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    public CCM_PwrMgmtMonth(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.minutesComputerActive = WMIObject.Properties[nameof (minutesComputerActive)].Value as uint?;
      this.minutesComputerOn = WMIObject.Properties[nameof (minutesComputerOn)].Value as uint?;
      this.minutesComputerShutdown = WMIObject.Properties[nameof (minutesComputerShutdown)].Value as uint?;
      this.minutesComputerSleep = WMIObject.Properties[nameof (minutesComputerSleep)].Value as uint?;
      this.minutesMonitorOn = WMIObject.Properties[nameof (minutesMonitorOn)].Value as uint?;
      this.minutesTotal = WMIObject.Properties[nameof (minutesTotal)].Value as uint?;
      string dmtfDate = WMIObject.Properties[nameof (MonthStart)].Value as string;
      if (string.IsNullOrEmpty(dmtfDate))
        this.MonthStart = new DateTime?();
      else
        this.MonthStart = new DateTime?(common.DmtfToDateTime(dmtfDate));
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    public uint? minutesComputerActive { get; set; }

    public uint? minutesComputerOn { get; set; }

    public uint? minutesComputerShutdown { get; set; }

    public uint? minutesComputerSleep { get; set; }

    public uint? minutesMonitorOn { get; set; }

    public uint? minutesTotal { get; set; }

    public DateTime? MonthStart { get; set; }
  }

  /// <summary>Source:ROOT\ccm\PowerManagementAgent</summary>
  public class CCM_PwrMgmtSystemPowerCapabilities
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    public CCM_PwrMgmtSystemPowerCapabilities(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.ApmPresent = WMIObject.Properties[nameof (ApmPresent)].Value as bool?;
      this.BatteriesAreShortTerm = WMIObject.Properties[nameof (BatteriesAreShortTerm)].Value as bool?;
      this.FullWake = WMIObject.Properties[nameof (FullWake)].Value as bool?;
      this.LidPresent = WMIObject.Properties[nameof (LidPresent)].Value as bool?;
      this.MinDeviceWakeState = WMIObject.Properties[nameof (MinDeviceWakeState)].Value as string;
      this.PreferredPMProfile = WMIObject.Properties[nameof (PreferredPMProfile)].Value as uint?;
      this.ProcessorThrottle = WMIObject.Properties[nameof (ProcessorThrottle)].Value as bool?;
      this.RtcWake = WMIObject.Properties[nameof (RtcWake)].Value as string;
      this.SystemBatteriesPresent = WMIObject.Properties[nameof (SystemBatteriesPresent)].Value as bool?;
      this.SystemS1 = WMIObject.Properties[nameof (SystemS1)].Value as bool?;
      this.SystemS2 = WMIObject.Properties[nameof (SystemS2)].Value as bool?;
      this.SystemS3 = WMIObject.Properties[nameof (SystemS3)].Value as bool?;
      this.SystemS4 = WMIObject.Properties[nameof (SystemS4)].Value as bool?;
      this.SystemS5 = WMIObject.Properties[nameof (SystemS5)].Value as bool?;
      this.UpsPresent = WMIObject.Properties[nameof (UpsPresent)].Value as bool?;
      this.VideoDimPresent = WMIObject.Properties[nameof (VideoDimPresent)].Value as bool?;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    public bool? ApmPresent { get; set; }

    public bool? BatteriesAreShortTerm { get; set; }

    public bool? FullWake { get; set; }

    public bool? LidPresent { get; set; }

    public string MinDeviceWakeState { get; set; }

    public uint? PreferredPMProfile { get; set; }

    public bool? ProcessorThrottle { get; set; }

    public string RtcWake { get; set; }

    public bool? SystemBatteriesPresent { get; set; }

    public bool? SystemS1 { get; set; }

    public bool? SystemS2 { get; set; }

    public bool? SystemS3 { get; set; }

    public bool? SystemS4 { get; set; }

    public bool? SystemS5 { get; set; }

    public bool? UpsPresent { get; set; }

    public bool? VideoDimPresent { get; set; }
  }

  /// <summary>Source:ROOT\ccm\PowerManagementAgent</summary>
  public class CCM_PwrMgmtUserClientSettings
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    public CCM_PwrMgmtUserClientSettings(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.IsClientOptOut = WMIObject.Properties[nameof (IsClientOptOut)].Value as bool?;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    public bool? IsClientOptOut { get; set; }
  }

  /// <summary>Source:ROOT\cimv2\sms</summary>
  public class SMS_PowerSettings
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    public SMS_PowerSettings(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.ACSettingIndex = WMIObject.Properties[nameof (ACSettingIndex)].Value as string;
      this.ACValue = WMIObject.Properties[nameof (ACValue)].Value as string;
      this.DCSettingIndex = WMIObject.Properties[nameof (DCSettingIndex)].Value as string;
      this.DCValue = WMIObject.Properties[nameof (DCValue)].Value as string;
      this.GUID = WMIObject.Properties[nameof (GUID)].Value as string;
      this.Name = WMIObject.Properties[nameof (Name)].Value as string;
      this.UnitSpecifier = WMIObject.Properties[nameof (UnitSpecifier)].Value as string;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    public string ACSettingIndex { get; set; }

    public string ACValue { get; set; }

    public string DCSettingIndex { get; set; }

    public string DCValue { get; set; }

    public string GUID { get; set; }

    public string Name { get; set; }

    public string UnitSpecifier { get; set; }

    public string ACDisplayvalue
    {
      get
      {
        return this.Name == this.ACValue ? $"{this.ACSettingIndex} {this.UnitSpecifier}" : this.ACValue;
      }
    }

    public string DCDisplayvalue
    {
      get
      {
        return this.Name == this.DCValue ? $"{this.DCSettingIndex} {this.UnitSpecifier}" : this.DCValue;
      }
    }
  }
}
