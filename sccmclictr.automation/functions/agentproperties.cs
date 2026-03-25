// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.functions.agentproperties
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

#nullable disable
namespace sccmclictr.automation.functions;

/// <summary>Class agentproperties.</summary>
public class agentproperties : baseInit
{
  internal Runspace remoteRunspace;
  internal TraceSource pSCode;
  internal ccm baseClient;

  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.agentproperties" /> class.
  /// </summary>
  /// <param name="RemoteRunspace">The remote runspace.</param>
  /// <param name="PSCode">The ps code.</param>
  /// <param name="oClient">The CCM client object.</param>
  public agentproperties(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
    : base(RemoteRunspace, PSCode)
  {
    this.remoteRunspace = RemoteRunspace;
    this.pSCode = PSCode;
    this.baseClient = oClient;
  }

  /// <summary>Return the ActiveDirectory Site-Name (if exist).</summary>
  public string ADSiteName
  {
    get
    {
      return this.GetStringFromPS("$a=New-Object -comObject 'CPAPPLET.CPAppletMgr';($a.GetClientProperties() | Where-Object { $_.Name -eq 'ADSiteName' }).Value");
    }
  }

  /// <summary>Return the Agent CommunicationMode (if exist).</summary>
  public string CommunicationMode
  {
    get
    {
      return this.GetStringFromPS("$a=New-Object -comObject 'CPAPPLET.CPAppletMgr';($a.GetClientProperties() | Where-Object { $_.Name -eq 'CommunicationMode' }).Value");
    }
  }

  /// <summary>Return the Agent CertKeyType (if exist).</summary>
  public string CertKeyType
  {
    get
    {
      return this.GetStringFromPS("$a=New-Object -comObject 'CPAPPLET.CPAppletMgr';($a.GetClientProperties() | Where-Object { $_.Name -eq 'CertKeyType' }).Value");
    }
  }

  /// <summary>Gets the branding title.</summary>
  /// <value>The branding title.</value>
  public string BrandingTitle
  {
    get
    {
      return this.GetProperty("ROOT\\ccm\\ClientSDK:CCM_ClientAgentSettings=@", nameof (BrandingTitle));
    }
  }

  /// <summary>Gets the day reminder interval.</summary>
  /// <value>The day reminder interval.</value>
  public uint DayReminderInterval
  {
    get
    {
      return uint.Parse(this.GetProperty("ROOT\\ccm\\ClientSDK:CCM_ClientAgentSettings=@", nameof (DayReminderInterval)));
    }
  }

  /// <summary>Gets the display new program notification.</summary>
  /// <value>True if the display new program notification is enabled, otherewise false</value>
  public bool DisplayNewProgramNotification
  {
    get
    {
      return bool.Parse(this.GetProperty("ROOT\\ccm\\ClientSDK:CCM_ClientAgentSettings=@", nameof (DisplayNewProgramNotification)));
    }
  }

  /// <summary>Gets the enable third party orchestration.</summary>
  /// <value>The enable third party orchestration.</value>
  public uint EnableThirdPartyOrchestration
  {
    get
    {
      return uint.Parse(this.GetProperty("ROOT\\ccm\\ClientSDK:CCM_ClientAgentSettings=@", nameof (EnableThirdPartyOrchestration)));
    }
  }

  /// <summary>Gets the hour reminder interval.</summary>
  /// <value>The hour reminder interval.</value>
  public uint HourReminderInterval
  {
    get
    {
      return uint.Parse(this.GetProperty("ROOT\\ccm\\ClientSDK:CCM_ClientAgentSettings=@", nameof (HourReminderInterval)));
    }
  }

  /// <summary>Gets the install restriction.</summary>
  /// <value>The install restriction.</value>
  public uint InstallRestriction
  {
    get
    {
      return uint.Parse(this.GetProperty("ROOT\\ccm\\ClientSDK:CCM_ClientAgentSettings=@", nameof (InstallRestriction)));
    }
  }

  /// <summary>Gets the osd branding subtitle.</summary>
  /// <value>The osd branding subtitle.</value>
  public string OSDBrandingSubtitle
  {
    get
    {
      return this.GetProperty("ROOT\\ccm\\ClientSDK:CCM_ClientAgentSettings=@", nameof (OSDBrandingSubtitle));
    }
  }

  /// <summary>Gets the reminder interval.</summary>
  /// <value>The reminder interval.</value>
  public uint ReminderInterval
  {
    get
    {
      return uint.Parse(this.GetProperty("ROOT\\ccm\\ClientSDK:CCM_ClientAgentSettings=@", nameof (ReminderInterval)));
    }
  }

  /// <summary>Gets the sum branding subtitle.</summary>
  /// <value>The sum branding subtitle.</value>
  public string SUMBrandingSubtitle
  {
    get
    {
      return this.GetProperty("ROOT\\ccm\\ClientSDK:CCM_ClientAgentSettings=@", nameof (SUMBrandingSubtitle));
    }
  }

  /// <summary>Gets the suspend bit locker.</summary>
  /// <value>The suspend bit locker.</value>
  public uint SuspendBitLocker
  {
    get
    {
      return uint.Parse(this.GetProperty("ROOT\\ccm\\ClientSDK:CCM_ClientAgentSettings=@", nameof (SuspendBitLocker)));
    }
  }

  /// <summary>Gets the SWD branding subtitle.</summary>
  /// <value>The SWD branding subtitle.</value>
  public string SWDBrandingSubtitle
  {
    get
    {
      return this.GetProperty("ROOT\\ccm\\ClientSDK:CCM_ClientAgentSettings=@", "SWDBarndingSubtitle");
    }
  }

  /// <summary>Gets the system restart turnaround time.</summary>
  /// <value>The system restart turnaround time.</value>
  public uint SystemRestartTurnaroundTime
  {
    get
    {
      return uint.Parse(this.GetProperty("ROOT\\ccm\\ClientSDK:CCM_ClientAgentSettings=@", nameof (SystemRestartTurnaroundTime)));
    }
  }

  /// <summary>Gets the are multi users logged on.</summary>
  /// <value>True if there are multiple users logged on, otherwise false.</value>
  public bool AreMultiUsersLoggedOn
  {
    get
    {
      return bool.Parse(this.GetStringFromMethod("ROOT\\ccm\\ClientSDK:CCM_ClientInternalUtilities=@", nameof (AreMultiUsersLoggedOn), "MultiUsersLoggedOn"));
    }
  }

  /// <summary>Notifies the presentation mode changed.</summary>
  /// <returns>UInt32.</returns>
  public uint NotifyPresentationModeChanged()
  {
    return uint.Parse(this.GetStringFromMethod("ROOT\\ccm\\ClientSDK:CCM_ClientInternalUtilities=@", nameof (NotifyPresentationModeChanged), "ReturnValue"));
  }

  /// <summary>Raises the event.</summary>
  /// <param name="ActionType">Type of the action.</param>
  /// <param name="ClassName">Name of the class.</param>
  /// <param name="MessageLevel">The message level.</param>
  /// <param name="SessionID">The session identifier.</param>
  /// <param name="TargetInstancePath">The target instance path.</param>
  /// <param name="UserSID">The user sid.</param>
  /// <param name="Value">The value.</param>
  /// <param name="Verbosity">The verbosity.</param>
  /// <returns>UInt32.</returns>
  public uint RaiseEvent(
    uint ActionType,
    string ClassName,
    uint MessageLevel,
    uint SessionID,
    string TargetInstancePath,
    string UserSID,
    string Value,
    uint Verbosity)
  {
    return uint.Parse(this.GetStringFromMethod("ROOT\\ccm\\ClientSDK:CCM_ClientInternalUtilities=@", $"RaiseEvent({ClassName},{TargetInstancePath},{ActionType},{UserSID},{SessionID},{MessageLevel},{Value},{Verbosity}", "ReturnValue"));
  }

  /// <summary>Gets the business hours.</summary>
  /// <returns>PSObject.</returns>
  public PSObject GetBusinessHours()
  {
    return this.CallClassMethod("ROOT\\ccm\\ClientSDK:CCM_ClientUXSettings", nameof (GetBusinessHours), "");
  }

  /// <summary>Gets the business hours.</summary>
  /// <param name="EndTime">The end time.</param>
  /// <param name="StartTime">The start time.</param>
  /// <param name="WorkingDays">The working days.</param>
  /// <returns>PSObject.</returns>
  public PSObject GetBusinessHours(out uint EndTime, out uint StartTime, out uint WorkingDays)
  {
    PSObject businessHours = this.CallClassMethod("ROOT\\ccm\\ClientSDK:CCM_ClientUXSettings", nameof (GetBusinessHours), "");
    EndTime = uint.Parse(businessHours.Properties[nameof (EndTime)].Value.ToString());
    StartTime = uint.Parse(businessHours.Properties[nameof (StartTime)].Value.ToString());
    WorkingDays = uint.Parse(businessHours.Properties[nameof (WorkingDays)].Value.ToString());
    return businessHours;
  }

  /// <summary>Sets the business hours.</summary>
  /// <param name="EndTime">The end time.</param>
  /// <param name="StartTime">The start time.</param>
  /// <param name="WorkingDays">The working days.</param>
  /// <returns>0 = Success, non zero for failure</returns>
  public uint SetBusinessHours(uint EndTime, uint StartTime, uint WorkingDays)
  {
    return uint.Parse(this.CallClassMethod("ROOT\\ccm\\ClientSDK:CCM_ClientUXSettings", "GetBusinessHours", $"{WorkingDays}, {StartTime}, {EndTime}").Properties["ReturnValue"].Value.ToString());
  }

  /// <summary>
  /// Sets the automatic install required software to non business hours.
  /// </summary>
  /// <param name="AutomaticallyInstallSoftware">True to enable this otherwise false</param>
  /// <returns>0 = Success, non zero for failure</returns>
  public uint SetAutoInstallRequiredSoftwaretoNonBusinessHours(bool AutomaticallyInstallSoftware)
  {
    return uint.Parse(this.GetStringFromClassMethod("ROOT\\ccm\\ClientSDK:CCM_ClientUXSettings", $"SetAutoInstallRequiredSoftwaretoNonBusinessHours({AutomaticallyInstallSoftware.ToString()})", "ReturnValue"));
  }

  /// <summary>
  /// Gets the automatic install required software to non business hours setting.
  /// </summary>
  /// <param name="AutomaticallyInstallSoftware">Gets the settings.</param>
  /// <returns>0 = Success, non zero for failure</returns>
  public uint GetAutoInstallRequiredSoftwaretoNonBusinessHours(out bool AutomaticallyInstallSoftware)
  {
    PSObject psObject = this.CallClassMethod("ROOT\\ccm\\ClientSDK:CCM_ClientUXSettings", nameof (GetAutoInstallRequiredSoftwaretoNonBusinessHours), "");
    AutomaticallyInstallSoftware = bool.Parse(psObject.Properties[nameof (AutomaticallyInstallSoftware)].Value.ToString());
    return uint.Parse(psObject.Properties["ReturnValue"].Value.ToString());
  }

  /// <summary>
  /// Sets the suppress computer activity in presentation mode.
  /// </summary>
  /// <param name="SuppressComputerActivityInPresentationMode">The suppress computer activity in presentation mode.</param>
  /// <returns>0 = Success, non zero for failure</returns>
  public uint SetSuppressComputerActivityInPresentationMode(
    bool SuppressComputerActivityInPresentationMode)
  {
    return uint.Parse(this.GetStringFromClassMethod("ROOT\\ccm\\ClientSDK:CCM_ClientUXSettings", $"SetSuppressComputerActivityInPresentationMode({SuppressComputerActivityInPresentationMode.ToString()})", "ReturnValue"));
  }

  /// <summary>
  /// Gets the suppress computer activity in presentation mode.
  /// </summary>
  /// <param name="SuppressComputerActivityInPresentationMode">Gets the setting. True  means suppress computer activity in presentation mode. </param>
  /// <returns>0 = Success, non zero for failure</returns>
  public uint GetSuppressComputerActivityInPresentationMode(
    out bool SuppressComputerActivityInPresentationMode)
  {
    PSObject psObject = this.CallClassMethod("ROOT\\ccm\\ClientSDK:CCM_ClientUXSettings", nameof (GetSuppressComputerActivityInPresentationMode), "");
    SuppressComputerActivityInPresentationMode = bool.Parse(psObject.Properties[nameof (SuppressComputerActivityInPresentationMode)].Value.ToString());
    return uint.Parse(psObject.Properties["ReturnValue"].Value.ToString());
  }

  /// <summary>
  /// Restart Computer from CM12 CCM_ClientUtilities function
  /// </summary>
  /// <returns>Return Code</returns>
  public uint RestartComputer()
  {
    try
    {
      return uint.Parse(this.GetStringFromClassMethod("ROOT\\ccm\\ClientSDK:CCM_ClientUtilities", "RestartComputer()", "ReturnValue"));
    }
    catch (Exception ex)
    {
      Trace.TraceError("RestartComputer: " + ex.Message);
    }
    return 1;
  }

  public uint GetMachinePolicy()
  {
    try
    {
      return uint.Parse(this.GetStringFromClassMethod("ROOT\\ccm\\ClientSDK:CCM_ClientUtilities", "GetMachinePolicy()", "ReturnValue"));
    }
    catch (Exception ex)
    {
      Trace.TraceError("GetMachinePolicy: " + ex.Message);
    }
    return 1;
  }

  public uint GetUserPolicy()
  {
    try
    {
      return uint.Parse(this.GetStringFromClassMethod("ROOT\\ccm\\ClientSDK:CCM_ClientUtilities", "GetUserPolicy()", "ReturnValue"));
    }
    catch (Exception ex)
    {
      Trace.TraceError("GetUserPolicy: " + ex.Message);
    }
    return 1;
  }

  /// <summary>Determine pending reboots (from ConfigMgr. !)</summary>
  /// <returns></returns>
  public PSObject DetermineIfRebootPending()
  {
    return this.CallClassMethod("ROOT\\ccm\\ClientSDK:CCM_ClientUtilities", nameof (DetermineIfRebootPending), "");
  }

  /// <summary>Determine pending reboots (from ConfigMgr. !)</summary>
  /// <param name="DisableHideTime"></param>
  /// <param name="InGracePeriod"></param>
  /// <param name="IsHardRebootPending"></param>
  /// <param name="RebootDeadline"></param>
  /// <param name="RebootPending"></param>
  /// <returns></returns>
  public PSObject DetermineIfRebootPending(
    out DateTime DisableHideTime,
    out bool InGracePeriod,
    out bool IsHardRebootPending,
    out DateTime RebootDeadline,
    out bool RebootPending)
  {
    PSObject ifRebootPending = this.CallClassMethod("ROOT\\ccm\\ClientSDK:CCM_ClientUtilities", nameof (DetermineIfRebootPending), "");
    DisableHideTime = DateTime.Parse(ifRebootPending.Properties[nameof (DisableHideTime)].Value.ToString());
    InGracePeriod = bool.Parse(ifRebootPending.Properties[nameof (InGracePeriod)].Value.ToString());
    IsHardRebootPending = bool.Parse(ifRebootPending.Properties[nameof (IsHardRebootPending)].Value.ToString());
    RebootDeadline = DateTime.Parse(ifRebootPending.Properties[nameof (RebootDeadline)].Value.ToString());
    RebootPending = bool.Parse(ifRebootPending.Properties[nameof (RebootPending)].Value.ToString());
    return ifRebootPending;
  }

  /// <summary>Check if ConfigMgr. requires a reboot;</summary>
  /// <returns></returns>
  public bool RebootPending()
  {
    PSObject psObject = this.CallClassMethod("ROOT\\ccm\\ClientSDK:CCM_ClientUtilities", "DetermineIfRebootPending", "");
    return bool.Parse(psObject.Properties["IsHardRebootPending"].Value.ToString()) | bool.Parse(psObject.Properties[nameof (RebootPending)].Value.ToString());
  }

  /// <summary>t.b.d</summary>
  /// <param name="Feature"></param>
  /// <param name="Value"></param>
  /// <returns></returns>
  public PSObject GetUserCapability(uint Feature, out uint Value)
  {
    PSObject userCapability = this.CallClassMethod("ROOT\\ccm\\ClientSDK:CCM_ClientUtilities", "DetermineIfRebootPending", Feature.ToString());
    Value = uint.Parse(userCapability.Properties[nameof (Value)].Value.ToString());
    return userCapability;
  }

  /// <summary>
  /// Get/Set the option if an Administrator can Override Agent Settings from the ControlPanel Applet
  /// </summary>
  public bool AllowLocalAdminOverride
  {
    get => bool.Parse(this.GetProperty("ROOT\\ccm:SMS_Client=@", nameof (AllowLocalAdminOverride)));
    set
    {
      this.SetProperty("ROOT\\ccm:SMS_Client=@", nameof (AllowLocalAdminOverride), "$" + value.ToString());
    }
  }

  /// <summary>Return the SCCM Agent GUID</summary>
  public string ClientId => this.GetProperty("ROOT\\ccm:CCM_Client=@", nameof (ClientId));

  /// <summary>Return the previous SCCM Agent GUID</summary>
  public string PreviousClientId
  {
    get => this.GetProperty("ROOT\\ccm:CCM_Client=@", nameof (PreviousClientId));
  }

  /// <summary>Return the full SCCM Agent ClientVersion</summary>
  public string ClientVersion
  {
    get
    {
      string clientVersion = "";
      try
      {
        TimeSpan cacheTime = this.cacheTime;
        this.cacheTime = new TimeSpan(0, 5, 0);
        clientVersion = this.GetProperty("ROOT\\ccm:SMS_Client=@", nameof (ClientVersion));
        this.cacheTime = cacheTime;
      }
      catch
      {
      }
      return clientVersion;
    }
  }

  /// <summary>
  /// Return the SCCM Agent GUID creation/change date as string
  /// </summary>
  public string ClientIdChangeDate
  {
    get => this.GetProperty("ROOT\\ccm:CCM_Client=@", nameof (ClientIdChangeDate));
  }

  /// <summary>Return the SCCM Client Type. 0=Desktop;1=Remote</summary>
  public uint ClientType
  {
    get => uint.Parse(this.GetProperty("ROOT\\ccm:SMS_Client=@", nameof (ClientType)));
  }

  /// <summary>
  /// Enable Site Code Auto Assignment on next Agent Restart
  /// </summary>
  public bool EnableAutoAssignment
  {
    get => bool.Parse(this.GetProperty("ROOT\\ccm:SMS_Client=@", nameof (EnableAutoAssignment)));
    set
    {
      this.SetProperty("ROOT\\ccm:SMS_Client=@", nameof (EnableAutoAssignment), "$" + value.ToString());
    }
  }

  /// <summary>Return Days Since last reboot</summary>
  public int DaysSinceLastReboot
  {
    get
    {
      return int.Parse(this.GetStringFromPS("$wmi = Get-WmiObject -Class Win32_OperatingSystem;$a = New-TimeSpan $wmi.ConvertToDateTime($wmi.LastBootUpTime) $(Get-Date);$a.Days"));
    }
  }

  /// <summary>DateTime of last reboot</summary>
  public DateTime LastReboot
  {
    get
    {
      return DateTime.ParseExact(this.GetStringFromPS("$wmi = Get-WmiObject -Class Win32_OperatingSystem;$a = $wmi.ConvertToDateTime($wmi.LastBootUpTime);$a.ToString(\"yyyy-MM-dd HH:mm\")"), "yyyy-MM-dd HH:mm", (IFormatProvider) null);
    }
  }

  /// <summary>Get TimeSpan of LastReboot</summary>
  public TimeSpan LastRebootTimeSpan
  {
    get
    {
      return TimeSpan.FromSeconds(double.Parse(this.GetStringFromPS("$wmi = Get-WmiObject -Class Win32_OperatingSystem;$a = New-TimeSpan $wmi.ConvertToDateTime($wmi.LastBootUpTime) $(Get-Date);$a.TotalSeconds").ToString()));
    }
  }

  /// <summary>Assigned SCCM Agent Site Code</summary>
  public string AssignedSite
  {
    get
    {
      string assignedSite = "";
      try
      {
        TimeSpan cacheTime = this.cacheTime;
        this.cacheTime = new TimeSpan(1, 0, 0);
        assignedSite = this.GetStringFromClassMethod("ROOT\\ccm:SMS_Client", "GetAssignedSite()", "sSiteCode");
        this.cacheTime = cacheTime;
      }
      catch
      {
      }
      return assignedSite;
    }
    set
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "SetAssignedSite", $"\"{value}\"");
      this.Cache.Remove(this.CreateHash("ROOT\\ccm:SMS_ClientGetAssignedSite().sSiteCode"), (string) null);
    }
  }

  /// <summary>Get the assigned Management Point</summary>
  public string ManagementPoint
  {
    get
    {
      string managementPoint = "";
      try
      {
        TimeSpan cacheTime = this.cacheTime;
        this.cacheTime = new TimeSpan(1, 0, 0);
        managementPoint = this.GetProperty($"ROOT\\ccm:SMS_Authority.Name='SMS:{this.AssignedSite}'", "CurrentManagementPoint");
        this.cacheTime = cacheTime;
      }
      catch
      {
      }
      return managementPoint;
    }
  }

  /// <summary>Configure Internet Management Point</summary>
  public string ManagementPointInternet
  {
    get
    {
      try
      {
        return this.baseClient.Inventory.isx64OS & !this.baseClient.AgentProperties.isSCCM2012 ? this.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\SMS\\Client\\Internet Facing\")).$(\"Internet MP Hostname\")") : this.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Microsoft\\SMS\\Client\\Internet Facing\")).$(\"Internet MP Hostname\")");
      }
      catch
      {
      }
      return "";
    }
    set
    {
      if (this.baseClient.Inventory.isx64OS)
      {
        this.GetStringFromPS($"Set-ItemProperty -path \"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\SMS\\Client\\Internet Facing\" -name \"Internet MP Hostname\" -value \"{value}\"");
        this.Cache.Remove(this.CreateHash("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\SMS\\Client\\Internet Facing\")).$(\"Internet MP Hostname\")"), (string) null);
      }
      else
      {
        this.GetStringFromPS($"Set-ItemProperty -path \"HKLM:\\SOFTWARE\\Microsoft\\SMS\\Client\\Internet Facing\" -name \"Internet MP Hostname\" -value \"{value}\"");
        this.Cache.Remove(this.CreateHash("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Microsoft\\SMS\\Client\\Internet Facing\")).$(\"Internet MP Hostname\")"), (string) null);
      }
    }
  }

  /// <summary>Get the assigned Proxy Management Point</summary>
  public string ManagementPointProxy
  {
    get
    {
      TimeSpan cacheTime = this.cacheTime;
      this.cacheTime = new TimeSpan(0, 1, 0);
      string managementPointProxy = "";
      try
      {
        foreach (PSObject psObject in this.GetObjects("ROOT\\CCM", "SELECT * FROM SMS_MPProxyInformation Where State = 'Active'"))
          managementPointProxy = psObject.Properties["Name"].Value.ToString();
      }
      catch
      {
      }
      this.cacheTime = cacheTime;
      return managementPointProxy;
    }
  }

  /// <summary>
  /// determine if SCCM Agent is from SCCM2012(TRUE) otherwise it's SCCM2007(FALSE)
  /// </summary>
  public bool isSCCM2012
  {
    get => this.ClientVersion.StartsWith("5.", StringComparison.CurrentCultureIgnoreCase);
  }

  /// <summary>
  /// Get the local Path of the SCCM Agent (e.g. C:\Windows\CCM )
  /// </summary>
  public string LocalSCCMAgentPath
  {
    get
    {
      try
      {
        return this.baseClient.Inventory.isx64OS & !this.baseClient.AgentProperties.isSCCM2012 ? this.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\SMS\\Client\\Configuration\\Client Properties\")).$(\"Local SMS Path\")") : this.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Microsoft\\SMS\\Client\\Configuration\\Client Properties\")).$(\"Local SMS Path\")");
      }
      catch
      {
      }
      return "";
    }
  }

  /// <summary>
  /// Get the local Path of the SCCM Agent Log Files (e.g. C:\windows\ccm\logs)
  /// </summary>
  public string LocalSCCMAgentLogPath => Path.Combine(this.LocalSCCMAgentPath, "Logs");

  /// <summary>Get all Log Files from the SCCM Agent Log Folder</summary>
  public List<string> LocalSCCMAgentLogFiles
  {
    get
    {
      List<string> sccmAgentLogFiles = new List<string>();
      try
      {
        foreach (PSObject psObject in this.GetObjectsFromPS($"(get-item '{this.LocalSCCMAgentLogPath}\\*.log').Name", false, new TimeSpan(0, 5, 0)))
          sccmAgentLogFiles.Add(psObject.ToString());
      }
      catch
      {
      }
      return sccmAgentLogFiles;
    }
  }

  /// <summary>
  /// Get or Set the Server Locator Point (in CM12 it's the Management Point)
  /// </summary>
  public string ServerLocatorPoint
  {
    get
    {
      try
      {
        return this.baseClient.Inventory.isx64OS & !this.baseClient.AgentProperties.isSCCM2012 ? this.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\CCM\")).$(\"SMSSLP\")") : this.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Microsoft\\CCM\")).$(\"SMSSLP\")");
      }
      catch
      {
      }
      return "";
    }
    set
    {
      if (this.baseClient.Inventory.isx64OS & !this.baseClient.AgentProperties.isSCCM2012)
      {
        this.GetStringFromPS($"New-ItemProperty -path \"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\CCM\" -name \"SMSSLP\" -PropertyType String -Force -value {value}");
        this.Cache.Remove(this.CreateHash("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\CCM\")).$(\"SMSSLP\")"), (string) null);
      }
      else
      {
        this.GetStringFromPS($"New-ItemProperty -path \"HKLM:\\SOFTWARE\\Microsoft\\CCM\" -name \"SMSSLP\" -PropertyType String -Force -value {value}");
        this.Cache.Remove(this.CreateHash("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Microsoft\\CCM\")).$(\"SMSSLP\")"), (string) null);
      }
    }
  }

  /// <summary>Get or Set the DNS Suffix</summary>
  public string DNSSuffix
  {
    get
    {
      try
      {
        return this.baseClient.Inventory.isx64OS & !this.baseClient.AgentProperties.isSCCM2012 ? this.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\CCM\\LocationServices\")).$(\"DnsSuffix\")") : this.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Microsoft\\CCM\\LocationServices\")).$(\"DnsSuffix\")");
      }
      catch
      {
      }
      return "";
    }
    set
    {
      if (this.baseClient.Inventory.isx64OS & !this.baseClient.AgentProperties.isSCCM2012)
      {
        this.GetStringFromPS($"New-ItemProperty -path \"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\CCM\\LocationServices\" -name \"DnsSuffix\" -PropertyType String -Force -value {value}");
        this.Cache.Remove(this.CreateHash("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\CCM\\LocationServices\")).$(\"DnsSuffix\")"), (string) null);
      }
      else
      {
        this.GetStringFromPS($"New-ItemProperty -path \"HKLM:\\SOFTWARE\\Microsoft\\CCM\\LocationServices\" -name \"DnsSuffix\" -PropertyType String -Force -value {value}");
        this.Cache.Remove(this.CreateHash("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Microsoft\\CCM\\LocationServices\")).$(\"DnsSuffix\")"), (string) null);
      }
    }
  }

  /// <summary>Get or Set the HTTP Port from the Agent.</summary>
  public int? HTTPPort
  {
    get
    {
      try
      {
        if (this.baseClient.Inventory.isx64OS & !this.baseClient.AgentProperties.isSCCM2012)
        {
          string stringFromPs = this.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\CCM\")).$(\"HttpPort\")");
          return !string.IsNullOrEmpty(stringFromPs) ? new int?(int.Parse(stringFromPs)) : new int?();
        }
        string stringFromPs1 = this.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Microsoft\\CCM\")).$(\"HttpPort\")");
        return !string.IsNullOrEmpty(stringFromPs1) ? new int?(int.Parse(stringFromPs1)) : new int?();
      }
      catch
      {
      }
      return new int?();
    }
    set
    {
      if (this.baseClient.Inventory.isx64OS & !this.baseClient.AgentProperties.isSCCM2012)
      {
        this.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\CCM\")).$(\"HttpPort\")");
        this.Cache.Remove(this.CreateHash("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\CCM\")).$(\"HttpPort\")"), (string) null);
      }
      else
      {
        this.GetStringFromPS($"New-ItemProperty -path \"HKLM:\\SOFTWARE\\Microsoft\\CCM\" -name \"HttpPort\" -Type DWORD -force -value {value.ToString()}");
        this.Cache.Remove(this.CreateHash($"New-ItemProperty -path \"HKLM:\\SOFTWARE\\Microsoft\\CCM\" -name \"HttpPort\" -Type DWORD -force -value {value.ToString()}"), (string) null);
      }
    }
  }

  /// <summary>Get or Set the HTTPS Port from the Agent.</summary>
  public int? HTTPSPort
  {
    get
    {
      try
      {
        if (this.baseClient.Inventory.isx64OS & !this.baseClient.AgentProperties.isSCCM2012)
        {
          string stringFromPs = this.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\CCM\")).$(\"HttpsPort\")");
          return !string.IsNullOrEmpty(stringFromPs) ? new int?(int.Parse(stringFromPs)) : new int?();
        }
        string stringFromPs1 = this.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Microsoft\\CCM\")).$(\"HttpsPort\")");
        return !string.IsNullOrEmpty(stringFromPs1) ? new int?(int.Parse(stringFromPs1)) : new int?();
      }
      catch
      {
      }
      return new int?();
    }
    set
    {
      if (this.baseClient.Inventory.isx64OS & !this.baseClient.AgentProperties.isSCCM2012)
      {
        this.GetStringFromPS($"New-ItemProperty -path \"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\CCM\" -name \"HttpsPort\" -Type DWORD -force -value {value.ToString()}");
        this.Cache.Remove(this.CreateHash("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\CCM\")).$(\"HttpsPort\")"), (string) null);
      }
      else
      {
        this.GetStringFromPS($"New-ItemProperty -path \"HKLM:\\SOFTWARE\\Microsoft\\CCM\" -name \"HttpsPort\" -Type DWORD -force -value {value.ToString()}");
        this.Cache.Remove(this.CreateHash("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Microsoft\\CCM\")).$(\"HttpsPort\")"), (string) null);
      }
    }
  }

  /// <summary>Determine if pending FileRenameOperations exists...</summary>
  public bool FileRenameOperationsPending
  {
    get
    {
      try
      {
        return this.GetObjectsFromPS("(Get-ItemProperty(\"HKLM:\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\")).$(\"PendingFileRenameOperations\")").Count > 0;
      }
      catch
      {
      }
      return false;
    }
  }

  /// <summary>Determin if component chnage requires a reboot</summary>
  public bool ComponentServicingRebootPending
  {
    get
    {
      try
      {
        return this.GetObjectsFromPS("if(test-path \"HKLM:\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Component Based Servicing\\RebootPending\"){ $true }").Count > 0;
      }
      catch
      {
      }
      return false;
    }
  }

  /// <summary>Get the MSI Product Code of the SCCM/CM12 Agent</summary>
  public string ProductCode
  {
    get
    {
      try
      {
        return this.GetStringFromPS("(Get-WmiObject -Class CCM_InstalledProduct -Namespace \"root\\ccm\").ProductCode");
      }
      catch
      {
      }
      return "";
    }
  }

  /// <summary>
  /// Delete root\ccm Namespace
  /// Query from rchiav (https://sccmclictrlib.codeplex.com/discussions/349818)
  /// </summary>
  /// <returns></returns>
  public string DeleteCCMNamespace()
  {
    try
    {
      return this.GetStringFromPS("gwmi -query \"SELECT * FROM __Namespace WHERE Name='CCM'\" -Namespace \"root\" | Remove-WmiObject");
    }
    catch
    {
    }
    return "";
  }

  /// <summary>Get the SID's of all logged on Users</summary>
  /// <returns></returns>
  public List<string> GetLoggedOnUserSIDs()
  {
    List<string> loggedOnUserSiDs = new List<string>();
    if (this.ClientVersion.StartsWith("5.00.78"))
    {
      try
      {
        string PSCode = "$username = (get-wmiobject -query \"SELECT Username FROM Win32_ComputerSystem\" -namespace \"root\\cimv2\").Username;$user = New-Object System.Security.Principal.NTAccount($username.split('\\')[0],$username.split('\\')[1]);$sid = $user.Translate([System.Security.Principal.SecurityIdentifier]);$sid.Value";
        loggedOnUserSiDs.Add(this.GetStringFromPS(PSCode));
      }
      catch
      {
      }
    }
    else
    {
      foreach (PSObject psObject in this.GetObjectsFromPS("get-wmiobject -query \"SELECT UserSID FROM CCM_UserLogonEvents WHERE LogoffTime = NULL\" -namespace \"ROOT\\ccm\""))
      {
        try
        {
          loggedOnUserSiDs.Add(psObject.Properties["UserSID"].Value.ToString());
        }
        catch
        {
        }
      }
    }
    return loggedOnUserSiDs;
  }

  /// <summary>
  /// Get the DeviceID from ROOT\ccm\ClientSdk:CCM_SoftwareCatalogUtilities
  /// </summary>
  /// <returns>DeviceID Object with ClientID and SignedClientId</returns>
  public agentproperties.DeviceId GetDeviceId
  {
    get
    {
      try
      {
        PSObject psObject = this.CallClassMethod("ROOT\\ccm\\ClientSdk:CCM_SoftwareCatalogUtilities", nameof (GetDeviceId), "", true);
        psObject.BaseObject.ToString();
        return new agentproperties.DeviceId()
        {
          ClientId = psObject.Properties["ClientId"].Value.ToString(),
          SignedClientId = psObject.Properties["SignedClientId"].Value.ToString(),
          ReturnValue = (uint) psObject.Properties["ReturnValue"].Value
        };
      }
      catch
      {
      }
      return new agentproperties.DeviceId();
    }
  }

  /// <summary>get ApplicationCatalog URL</summary>
  public string PortalURL
  {
    get
    {
      try
      {
        return this.CallClassMethod("ROOT\\ccm\\ClientSdk:CCM_SoftwareCatalogUtilities", "GetPortalUrlValue", "").Properties["PortalUrl"].Value.ToString();
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }
  }

  /// <summary>
  /// DeviceID from ROOT\ccm\ClientSdk:CCM_SoftwareCatalogUtilities
  /// </summary>
  public class DeviceId
  {
    public string ClientId;
    public string SignedClientId;
    public uint ReturnValue;
  }
}
