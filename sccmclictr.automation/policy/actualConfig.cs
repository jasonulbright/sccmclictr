// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.policy.actualConfig
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using sccmclictr.automation.Properties;
using sccmclictr.automation.schedule;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

#nullable disable
namespace sccmclictr.automation.policy;

/// <summary>Class actualConfig.</summary>
public class actualConfig : baseInit
{
  internal Runspace remoteRunspace;
  internal TraceSource pSCode;
  internal ccm baseClient;

  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.actualConfig" /> class.
  /// </summary>
  /// <param name="RemoteRunspace">The remote runspace.</param>
  /// <param name="PSCode">The PowerShell code.</param>
  /// <param name="oClient">A CCM Client object.</param>
  public actualConfig(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
    : base(RemoteRunspace, PSCode)
  {
    this.remoteRunspace = RemoteRunspace;
    this.pSCode = PSCode;
    this.baseClient = oClient;
  }

  /// <summary>Creates the service window.</summary>
  /// <param name="Schedules">The schedules.</param>
  /// <param name="ServiceWindowType">Type of the service window.</param>
  /// <returns>System.String.</returns>
  public string CreateServiceWindow(string Schedules, uint ServiceWindowType)
  {
    foreach (PSObject psObject in this.baseClient.GetObjectsFromPS($"$a = Set-WmiInstance -Class CCM_ServiceWindow -Namespace 'ROOT\\ccm\\Policy\\Machine\\ActualConfig' -PutType 'CreateOnly';$a.ServiceWindowType = {ServiceWindowType.ToString()};$a.Schedules = '{Schedules}';$a.Put() | Out-Null;$a.ServiceWindowID"))
    {
      if (psObject != null && psObject.ToString().Length == 38)
        return psObject.ToString();
    }
    return (string) null;
  }

  /// <summary>Gets a list of component client configurations.</summary>
  /// <value>A list of component client configurations.</value>
  public List<actualConfig.CCM_ComponentClientConfig> ComponentClientConfig
  {
    get
    {
      List<actualConfig.CCM_ComponentClientConfig> componentClientConfig1 = new List<actualConfig.CCM_ComponentClientConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\ActualConfig", "SELECT * FROM CCM_ComponentClientConfig"))
      {
        actualConfig.CCM_ComponentClientConfig componentClientConfig2 = new actualConfig.CCM_ComponentClientConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        componentClientConfig2.remoteRunspace = this.remoteRunspace;
        componentClientConfig2.pSCode = this.pSCode;
        componentClientConfig1.Add(componentClientConfig2);
      }
      return componentClientConfig1;
    }
  }

  /// <summary>
  /// Gets a list of software updates client configurations.
  /// </summary>
  /// <value>A list of software updates client configuration.</value>
  public List<actualConfig.CCM_SoftwareUpdatesClientConfig> SoftwareUpdatesClientConfig
  {
    get
    {
      List<actualConfig.CCM_SoftwareUpdatesClientConfig> updatesClientConfig1 = new List<actualConfig.CCM_SoftwareUpdatesClientConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\ActualConfig", "SELECT * FROM CCM_SoftwareUpdatesClientConfig"))
      {
        actualConfig.CCM_SoftwareUpdatesClientConfig updatesClientConfig2 = new actualConfig.CCM_SoftwareUpdatesClientConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        updatesClientConfig2.remoteRunspace = this.remoteRunspace;
        updatesClientConfig2.pSCode = this.pSCode;
        updatesClientConfig1.Add(updatesClientConfig2);
      }
      return updatesClientConfig1;
    }
  }

  /// <summary>Gets a list of root ca certificates.</summary>
  /// <value>A list of root ca certificates.</value>
  public List<actualConfig.CCM_RootCACertificates> RootCACertificates
  {
    get
    {
      List<actualConfig.CCM_RootCACertificates> rootCaCertificates1 = new List<actualConfig.CCM_RootCACertificates>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\ActualConfig", "SELECT * FROM CCM_RootCACertificates"))
      {
        actualConfig.CCM_RootCACertificates rootCaCertificates2 = new actualConfig.CCM_RootCACertificates(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        rootCaCertificates2.remoteRunspace = this.remoteRunspace;
        rootCaCertificates2.pSCode = this.pSCode;
        rootCaCertificates1.Add(rootCaCertificates2);
      }
      return rootCaCertificates1;
    }
  }

  /// <summary>Gets a list of source update client configuration.</summary>
  /// <value>A list of source update client configuration.</value>
  public List<actualConfig.CCM_SourceUpdateClientConfig> SourceUpdateClientConfig
  {
    get
    {
      List<actualConfig.CCM_SourceUpdateClientConfig> updateClientConfig1 = new List<actualConfig.CCM_SourceUpdateClientConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\ActualConfig", "SELECT * FROM CCM_SourceUpdateClientConfig"))
      {
        actualConfig.CCM_SourceUpdateClientConfig updateClientConfig2 = new actualConfig.CCM_SourceUpdateClientConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        updateClientConfig2.remoteRunspace = this.remoteRunspace;
        updateClientConfig2.pSCode = this.pSCode;
        updateClientConfig1.Add(updateClientConfig2);
      }
      return updateClientConfig1;
    }
  }

  /// <summary>Gets a list of software center settings.</summary>
  /// <value>A list of software center settings.</value>
  public List<actualConfig.CCM_SoftwareCenterSettings> SoftwareCenterSettings
  {
    get
    {
      List<actualConfig.CCM_SoftwareCenterSettings> softwareCenterSettings1 = new List<actualConfig.CCM_SoftwareCenterSettings>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\ActualConfig", "SELECT * CCM_SoftwareCenterSettings"))
      {
        actualConfig.CCM_SoftwareCenterSettings softwareCenterSettings2 = new actualConfig.CCM_SoftwareCenterSettings(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        softwareCenterSettings2.remoteRunspace = this.remoteRunspace;
        softwareCenterSettings2.pSCode = this.pSCode;
        softwareCenterSettings1.Add(softwareCenterSettings2);
      }
      return softwareCenterSettings1;
    }
  }

  /// <summary>
  /// Gets A list of software inventory client configurations.
  /// </summary>
  /// <value>A list of software inventory client configurations.</value>
  public List<actualConfig.CCM_SoftwareInventoryClientConfig> SoftwareInventoryClientConfig
  {
    get
    {
      List<actualConfig.CCM_SoftwareInventoryClientConfig> inventoryClientConfig1 = new List<actualConfig.CCM_SoftwareInventoryClientConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\ActualConfig", "SELECT * CCM_SoftwareInventoryClientConfig"))
      {
        actualConfig.CCM_SoftwareInventoryClientConfig inventoryClientConfig2 = new actualConfig.CCM_SoftwareInventoryClientConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        inventoryClientConfig2.remoteRunspace = this.remoteRunspace;
        inventoryClientConfig2.pSCode = this.pSCode;
        inventoryClientConfig1.Add(inventoryClientConfig2);
      }
      return inventoryClientConfig1;
    }
  }

  /// <summary>Gets a list of targeting settings.</summary>
  /// <value>A list of targeting settings.</value>
  public List<actualConfig.CCM_TargetingSettings> TargetingSettings
  {
    get
    {
      List<actualConfig.CCM_TargetingSettings> targetingSettings1 = new List<actualConfig.CCM_TargetingSettings>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\ActualConfig", "SELECT * CCM_TargetingSettings"))
      {
        actualConfig.CCM_TargetingSettings targetingSettings2 = new actualConfig.CCM_TargetingSettings(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        targetingSettings2.remoteRunspace = this.remoteRunspace;
        targetingSettings2.pSCode = this.pSCode;
        targetingSettings1.Add(targetingSettings2);
      }
      return targetingSettings1;
    }
  }

  /// <summary>Gets a list of multicast configurations.</summary>
  /// <value>A list of multicast configurations.</value>
  public List<actualConfig.CCM_MulticastConfig> MulticastConfig
  {
    get
    {
      List<actualConfig.CCM_MulticastConfig> multicastConfig = new List<actualConfig.CCM_MulticastConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\ActualConfig", "SELECT * CCM_MulticastConfig"))
      {
        actualConfig.CCM_MulticastConfig ccmMulticastConfig = new actualConfig.CCM_MulticastConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        ccmMulticastConfig.remoteRunspace = this.remoteRunspace;
        ccmMulticastConfig.pSCode = this.pSCode;
        multicastConfig.Add(ccmMulticastConfig);
      }
      return multicastConfig;
    }
  }

  /// <summary>
  /// Gets a list of software distribution client configuration.
  /// </summary>
  /// <value>A list of software distribution client configuration.</value>
  public List<actualConfig.CCM_SoftwareDistributionClientConfig> SoftwareDistributionClientConfig
  {
    get
    {
      List<actualConfig.CCM_SoftwareDistributionClientConfig> distributionClientConfig1 = new List<actualConfig.CCM_SoftwareDistributionClientConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\ActualConfig", "SELECT * CCM_SoftwareDistributionClientConfig"))
      {
        actualConfig.CCM_SoftwareDistributionClientConfig distributionClientConfig2 = new actualConfig.CCM_SoftwareDistributionClientConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        distributionClientConfig2.remoteRunspace = this.remoteRunspace;
        distributionClientConfig2.pSCode = this.pSCode;
        distributionClientConfig1.Add(distributionClientConfig2);
      }
      return distributionClientConfig1;
    }
  }

  /// <summary>
  /// Gets a list of configuration management client configurations.
  /// </summary>
  /// <value>A list of configuration management client configurations.</value>
  public List<actualConfig.CCM_ConfigurationManagementClientConfig> ConfigurationManagementClientConfig
  {
    get
    {
      List<actualConfig.CCM_ConfigurationManagementClientConfig> managementClientConfig1 = new List<actualConfig.CCM_ConfigurationManagementClientConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\ActualConfig", "SELECT * CCM_ConfigurationManagementClientConfig"))
      {
        actualConfig.CCM_ConfigurationManagementClientConfig managementClientConfig2 = new actualConfig.CCM_ConfigurationManagementClientConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        managementClientConfig2.remoteRunspace = this.remoteRunspace;
        managementClientConfig2.pSCode = this.pSCode;
        managementClientConfig1.Add(managementClientConfig2);
      }
      return managementClientConfig1;
    }
  }

  /// <summary>Gets a list of client agent configurations.</summary>
  /// <value>A list of client agent configurations.</value>
  public List<actualConfig.CCM_ClientAgentConfig> ClientAgentConfig
  {
    get
    {
      List<actualConfig.CCM_ClientAgentConfig> clientAgentConfig1 = new List<actualConfig.CCM_ClientAgentConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\ActualConfig", "SELECT * CCM_ClientAgentConfig"))
      {
        actualConfig.CCM_ClientAgentConfig clientAgentConfig2 = new actualConfig.CCM_ClientAgentConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        clientAgentConfig2.remoteRunspace = this.remoteRunspace;
        clientAgentConfig2.pSCode = this.pSCode;
        clientAgentConfig1.Add(clientAgentConfig2);
      }
      return clientAgentConfig1;
    }
  }

  /// <summary>Gets a list of system health client configurations.</summary>
  /// <value>A list of system health client configurations.</value>
  public List<actualConfig.CCM_SystemHealthClientConfig> SystemHealthClientConfig
  {
    get
    {
      List<actualConfig.CCM_SystemHealthClientConfig> healthClientConfig1 = new List<actualConfig.CCM_SystemHealthClientConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\ActualConfig", "SELECT * CCM_SystemHealthClientConfig"))
      {
        actualConfig.CCM_SystemHealthClientConfig healthClientConfig2 = new actualConfig.CCM_SystemHealthClientConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        healthClientConfig2.remoteRunspace = this.remoteRunspace;
        healthClientConfig2.pSCode = this.pSCode;
        healthClientConfig1.Add(healthClientConfig2);
      }
      return healthClientConfig1;
    }
  }

  /// <summary>
  /// Gets a list of power management client configurations.
  /// </summary>
  /// <value>A list of power management client configurations.</value>
  public List<actualConfig.CCM_PowerManagementClientConfig> PowerManagementClientConfig
  {
    get
    {
      List<actualConfig.CCM_PowerManagementClientConfig> managementClientConfig1 = new List<actualConfig.CCM_PowerManagementClientConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\ActualConfig", "SELECT * CCM_PowerManagementClientConfig"))
      {
        actualConfig.CCM_PowerManagementClientConfig managementClientConfig2 = new actualConfig.CCM_PowerManagementClientConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        managementClientConfig2.remoteRunspace = this.remoteRunspace;
        managementClientConfig2.pSCode = this.pSCode;
        managementClientConfig1.Add(managementClientConfig2);
      }
      return managementClientConfig1;
    }
  }

  /// <summary>
  /// Gets a list of software metering client configurations.
  /// </summary>
  /// <value>A list of software metering client configurations.</value>
  public List<actualConfig.CCM_SoftwareMeteringClientConfig> SoftwareMeteringClientConfig
  {
    get
    {
      List<actualConfig.CCM_SoftwareMeteringClientConfig> meteringClientConfig1 = new List<actualConfig.CCM_SoftwareMeteringClientConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\ActualConfig", "SELECT * CCM_SoftwareMeteringClientConfig"))
      {
        actualConfig.CCM_SoftwareMeteringClientConfig meteringClientConfig2 = new actualConfig.CCM_SoftwareMeteringClientConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        meteringClientConfig2.remoteRunspace = this.remoteRunspace;
        meteringClientConfig2.pSCode = this.pSCode;
        meteringClientConfig1.Add(meteringClientConfig2);
      }
      return meteringClientConfig1;
    }
  }

  /// <summary>
  /// Gets a list of hardware inventory client configurations.
  /// </summary>
  /// <value>A list of hardware inventory client configurations.</value>
  public List<actualConfig.CCM_HardwareInventoryClientConfig> HardwareInventoryClientConfig
  {
    get
    {
      List<actualConfig.CCM_HardwareInventoryClientConfig> inventoryClientConfig1 = new List<actualConfig.CCM_HardwareInventoryClientConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\ActualConfig", "SELECT * CCM_HardwareInventoryClientConfig"))
      {
        actualConfig.CCM_HardwareInventoryClientConfig inventoryClientConfig2 = new actualConfig.CCM_HardwareInventoryClientConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        inventoryClientConfig2.remoteRunspace = this.remoteRunspace;
        inventoryClientConfig2.pSCode = this.pSCode;
        inventoryClientConfig1.Add(inventoryClientConfig2);
      }
      return inventoryClientConfig1;
    }
  }

  /// <summary>Gets a list of remote tools policies.</summary>
  /// <value>A list of remote tools policies.</value>
  public List<actualConfig.CCM_RemoteTools_Policy> RemoteTools_Policy
  {
    get
    {
      List<actualConfig.CCM_RemoteTools_Policy> remoteToolsPolicy1 = new List<actualConfig.CCM_RemoteTools_Policy>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\ActualConfig", "SELECT * CCM_RemoteTools_Policy"))
      {
        actualConfig.CCM_RemoteTools_Policy remoteToolsPolicy2 = new actualConfig.CCM_RemoteTools_Policy(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        remoteToolsPolicy2.remoteRunspace = this.remoteRunspace;
        remoteToolsPolicy2.pSCode = this.pSCode;
        remoteToolsPolicy1.Add(remoteToolsPolicy2);
      }
      return remoteToolsPolicy1;
    }
  }

  /// <summary>Gets a list of network access accounts.</summary>
  /// <value>A list of network access accounts.</value>
  public List<actualConfig.CCM_NetworkAccessAccount> NetworkAccessAccount
  {
    get
    {
      List<actualConfig.CCM_NetworkAccessAccount> networkAccessAccount1 = new List<actualConfig.CCM_NetworkAccessAccount>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\ActualConfig", "SELECT * CCM_NetworkAccessAccount"))
      {
        actualConfig.CCM_NetworkAccessAccount networkAccessAccount2 = new actualConfig.CCM_NetworkAccessAccount(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        networkAccessAccount2.remoteRunspace = this.remoteRunspace;
        networkAccessAccount2.pSCode = this.pSCode;
        networkAccessAccount1.Add(networkAccessAccount2);
      }
      return networkAccessAccount1;
    }
  }

  /// <summary>
  /// Gets a list of application management client configurations.
  /// </summary>
  /// <value>A list of application management client configurations.</value>
  public List<actualConfig.CCM_ApplicationManagementClientConfig> ApplicationManagementClientConfig
  {
    get
    {
      List<actualConfig.CCM_ApplicationManagementClientConfig> managementClientConfig1 = new List<actualConfig.CCM_ApplicationManagementClientConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\ActualConfig", "SELECT * CCM_ApplicationManagementClientConfig"))
      {
        actualConfig.CCM_ApplicationManagementClientConfig managementClientConfig2 = new actualConfig.CCM_ApplicationManagementClientConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        managementClientConfig2.remoteRunspace = this.remoteRunspace;
        managementClientConfig2.pSCode = this.pSCode;
        managementClientConfig1.Add(managementClientConfig2);
      }
      return managementClientConfig1;
    }
  }

  /// <summary>
  /// Gets a list of out of band management client configurations.
  /// </summary>
  /// <value>A list of out of band management client configurations.</value>
  public List<actualConfig.CCM_OutOfBandManagementClientConfig> OutOfBandManagementClientConfig
  {
    get
    {
      List<actualConfig.CCM_OutOfBandManagementClientConfig> managementClientConfig1 = new List<actualConfig.CCM_OutOfBandManagementClientConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\ActualConfig", "SELECT * CCM_OutOfBandManagementClientConfig"))
      {
        actualConfig.CCM_OutOfBandManagementClientConfig managementClientConfig2 = new actualConfig.CCM_OutOfBandManagementClientConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        managementClientConfig2.remoteRunspace = this.remoteRunspace;
        managementClientConfig2.pSCode = this.pSCode;
        managementClientConfig1.Add(managementClientConfig2);
      }
      return managementClientConfig1;
    }
  }

  /// <summary>Gets a list of service windows.</summary>
  /// <value>A list of service windows.</value>
  public List<actualConfig.CCM_ServiceWindow> ServiceWindow
  {
    get
    {
      List<actualConfig.CCM_ServiceWindow> serviceWindow = new List<actualConfig.CCM_ServiceWindow>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\ActualConfig", "SELECT * FROM CCM_ServiceWindow", true))
      {
        actualConfig.CCM_ServiceWindow ccmServiceWindow = new actualConfig.CCM_ServiceWindow(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        ccmServiceWindow.remoteRunspace = this.remoteRunspace;
        ccmServiceWindow.pSCode = this.pSCode;
        serviceWindow.Add(ccmServiceWindow);
      }
      return serviceWindow;
    }
  }

  /// <summary>Collection Variables</summary>
  public List<actualConfig.CCM_CollectionVariable> CollectionVariables
  {
    get
    {
      List<actualConfig.CCM_CollectionVariable> collectionVariables = new List<actualConfig.CCM_CollectionVariable>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\ActualConfig", "SELECT * FROM CCM_CollectionVariable", true))
      {
        actualConfig.CCM_CollectionVariable collectionVariable = new actualConfig.CCM_CollectionVariable(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        collectionVariable.remoteRunspace = this.remoteRunspace;
        collectionVariable.pSCode = this.pSCode;
        collectionVariables.Add(collectionVariable);
      }
      return collectionVariables;
    }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_Policy
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.actualConfig.CCM_Policy" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    /// <param name="oClient">A CCM Client object.</param>
    public CCM_Policy(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode,
      ccm oClient)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_ComponentClientConfig : actualConfig.CCM_Policy
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.actualConfig.CCM_Policy" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    /// <param name="oClient">A CCM Client object.</param>
    public CCM_ComponentClientConfig(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode,
      ccm oClient)
      : base(WMIObject, RemoteRunspace, PSCode, oClient)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.ComponentName = WMIObject.Properties[nameof (ComponentName)].Value as string;
      this.Enabled = WMIObject.Properties[nameof (Enabled)].Value as bool?;
    }

    public string ComponentName { get; set; }

    public bool? Enabled { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_SoftwareUpdatesClientConfig : actualConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.actualConfig.CCM_Policy" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    /// <param name="oClient">A CCM Client object.</param>
    public CCM_SoftwareUpdatesClientConfig(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode,
      ccm oClient)
      : base(WMIObject, RemoteRunspace, PSCode, oClient)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.AssignmentBatchingTimeout = WMIObject.Properties[nameof (AssignmentBatchingTimeout)].Value as uint?;
      this.BrandingSubTitle = WMIObject.Properties[nameof (BrandingSubTitle)].Value as string;
      this.BrandingTitle = WMIObject.Properties[nameof (BrandingTitle)].Value as string;
      this.ContentDownloadTimeout = WMIObject.Properties[nameof (ContentDownloadTimeout)].Value as uint?;
      this.ContentLocationTimeout = WMIObject.Properties[nameof (ContentLocationTimeout)].Value as uint?;
      this.DayReminderInterval = WMIObject.Properties[nameof (DayReminderInterval)].Value as uint?;
      this.HourReminderInterval = WMIObject.Properties[nameof (HourReminderInterval)].Value as uint?;
      this.MaxScanRetryCount = WMIObject.Properties[nameof (MaxScanRetryCount)].Value as uint?;
      this.PerDPInactivityTimeout = WMIObject.Properties[nameof (PerDPInactivityTimeout)].Value as uint?;
      this.ReminderInterval = WMIObject.Properties[nameof (ReminderInterval)].Value as uint?;
      this.Reserved1 = WMIObject.Properties[nameof (Reserved1)].Value as string;
      this.Reserved2 = WMIObject.Properties[nameof (Reserved2)].Value as string;
      this.Reserved3 = WMIObject.Properties[nameof (Reserved3)].Value as string;
      this.ScanRetryDelay = WMIObject.Properties[nameof (ScanRetryDelay)].Value as uint?;
      this.SiteSettingsKey = WMIObject.Properties[nameof (SiteSettingsKey)].Value as uint?;
      this.TotalInactivityTimeout = WMIObject.Properties[nameof (TotalInactivityTimeout)].Value as uint?;
      this.UserJobPerDPInactivityTimeout = WMIObject.Properties[nameof (UserJobPerDPInactivityTimeout)].Value as uint?;
      this.UserJobTotalInactivityTimeout = WMIObject.Properties[nameof (UserJobTotalInactivityTimeout)].Value as uint?;
      this.WSUSLocationTimeout = WMIObject.Properties[nameof (WSUSLocationTimeout)].Value as uint?;
    }

    public uint? AssignmentBatchingTimeout { get; set; }

    public string BrandingSubTitle { get; set; }

    public string BrandingTitle { get; set; }

    public uint? ContentDownloadTimeout { get; set; }

    public uint? ContentLocationTimeout { get; set; }

    public uint? DayReminderInterval { get; set; }

    public uint? HourReminderInterval { get; set; }

    public uint? MaxScanRetryCount { get; set; }

    public uint? PerDPInactivityTimeout { get; set; }

    public uint? ReminderInterval { get; set; }

    public string Reserved1 { get; set; }

    public string Reserved2 { get; set; }

    public string Reserved3 { get; set; }

    public uint? ScanRetryDelay { get; set; }

    public uint? SiteSettingsKey { get; set; }

    public uint? TotalInactivityTimeout { get; set; }

    public uint? UserJobPerDPInactivityTimeout { get; set; }

    public uint? UserJobTotalInactivityTimeout { get; set; }

    public uint? WSUSLocationTimeout { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_RootCACertificates : actualConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.actualConfig.CCM_Policy" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    /// <param name="oClient">A CCM Client object.</param>
    public CCM_RootCACertificates(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode,
      ccm oClient)
      : base(WMIObject, RemoteRunspace, PSCode, oClient)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.Reserved1 = WMIObject.Properties[nameof (Reserved1)].Value as string;
      this.RootCACerts = WMIObject.Properties[nameof (RootCACerts)].Value as string;
      this.SiteSettingsKey = WMIObject.Properties[nameof (SiteSettingsKey)].Value as uint?;
    }

    public string Reserved1 { get; set; }

    public string RootCACerts { get; set; }

    public uint? SiteSettingsKey { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_SourceUpdateClientConfig : actualConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.actualConfig.CCM_Policy" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    /// <param name="oClient">A CCM Client object.</param>
    public CCM_SourceUpdateClientConfig(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode,
      ccm oClient)
      : base(WMIObject, RemoteRunspace, PSCode, oClient)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.LocationTimeOut = WMIObject.Properties[nameof (LocationTimeOut)].Value as uint?;
      this.MaxRetryCount = WMIObject.Properties[nameof (MaxRetryCount)].Value as uint?;
      this.NetworkChangeDelay = WMIObject.Properties[nameof (NetworkChangeDelay)].Value as uint?;
      this.RemoteDPs = WMIObject.Properties[nameof (RemoteDPs)].Value as bool?;
      this.Reserved1 = WMIObject.Properties[nameof (Reserved1)].Value as string;
      this.Reserved2 = WMIObject.Properties[nameof (Reserved2)].Value as string;
      this.Reserved3 = WMIObject.Properties[nameof (Reserved3)].Value as string;
      this.RetryTimeOut = WMIObject.Properties[nameof (RetryTimeOut)].Value as uint?;
      this.SiteSettingsKey = WMIObject.Properties[nameof (SiteSettingsKey)].Value as uint?;
    }

    public uint? LocationTimeOut { get; set; }

    public uint? MaxRetryCount { get; set; }

    public uint? NetworkChangeDelay { get; set; }

    public bool? RemoteDPs { get; set; }

    public string Reserved1 { get; set; }

    public string Reserved2 { get; set; }

    public string Reserved3 { get; set; }

    public uint? RetryTimeOut { get; set; }

    public uint? SiteSettingsKey { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_SoftwareCenterSettings : actualConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.actualConfig.CCM_Policy" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    /// <param name="oClient">A CCM Client object.</param>
    public CCM_SoftwareCenterSettings(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode,
      ccm oClient)
      : base(WMIObject, RemoteRunspace, PSCode, oClient)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.AutoInstallRequiredSoftware = WMIObject.Properties[nameof (AutoInstallRequiredSoftware)].Value as bool?;
      this.SiteSettingsKey = WMIObject.Properties[nameof (SiteSettingsKey)].Value as uint?;
      this.SuppressComputerActivityInPresentationMode = WMIObject.Properties[nameof (SuppressComputerActivityInPresentationMode)].Value as bool?;
    }

    public bool? AutoInstallRequiredSoftware { get; set; }

    public uint? SiteSettingsKey { get; set; }

    public bool? SuppressComputerActivityInPresentationMode { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_SoftwareInventoryClientConfig : actualConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.actualConfig.CCM_Policy" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    /// <param name="oClient">A CCM Client object.</param>
    public CCM_SoftwareInventoryClientConfig(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode,
      ccm oClient)
      : base(WMIObject, RemoteRunspace, PSCode, oClient)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.Reserved1 = WMIObject.Properties[nameof (Reserved1)].Value as string;
      this.Reserved2 = WMIObject.Properties[nameof (Reserved2)].Value as string;
      this.Reserved3 = WMIObject.Properties[nameof (Reserved3)].Value as string;
      this.SiteSettingsKey = WMIObject.Properties[nameof (SiteSettingsKey)].Value as uint?;
    }

    public string Reserved1 { get; set; }

    public string Reserved2 { get; set; }

    public string Reserved3 { get; set; }

    public uint? SiteSettingsKey { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_TargetingSettings : actualConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.actualConfig.CCM_Policy" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    /// <param name="oClient">A CCM Client object.</param>
    public CCM_TargetingSettings(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode,
      ccm oClient)
      : base(WMIObject, RemoteRunspace, PSCode, oClient)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.AllowUserAffinity = WMIObject.Properties[nameof (AllowUserAffinity)].Value as uint?;
      this.AllowUserAffinityAfterMinutes = WMIObject.Properties[nameof (AllowUserAffinityAfterMinutes)].Value as uint?;
      this.AutoApproveAffinity = WMIObject.Properties[nameof (AutoApproveAffinity)].Value as uint?;
      this.ConsoleMinutes = WMIObject.Properties[nameof (ConsoleMinutes)].Value as uint?;
      this.IntervalDays = WMIObject.Properties[nameof (IntervalDays)].Value as uint?;
      this.Reserved1 = WMIObject.Properties[nameof (Reserved1)].Value as string;
      this.Reserved2 = WMIObject.Properties[nameof (Reserved2)].Value as string;
      this.Reserved3 = WMIObject.Properties[nameof (Reserved3)].Value as string;
      this.SiteSettingsKey = WMIObject.Properties[nameof (SiteSettingsKey)].Value as uint?;
    }

    public uint? AllowUserAffinity { get; set; }

    public uint? AllowUserAffinityAfterMinutes { get; set; }

    public uint? AutoApproveAffinity { get; set; }

    public uint? ConsoleMinutes { get; set; }

    public uint? IntervalDays { get; set; }

    public string Reserved1 { get; set; }

    public string Reserved2 { get; set; }

    public string Reserved3 { get; set; }

    public uint? SiteSettingsKey { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_MulticastConfig : actualConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.actualConfig.CCM_Policy" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    /// <param name="oClient">A CCM Client object.</param>
    public CCM_MulticastConfig(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode,
      ccm oClient)
      : base(WMIObject, RemoteRunspace, PSCode, oClient)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.RetryCount = WMIObject.Properties[nameof (RetryCount)].Value as uint?;
      this.RetryDelay = WMIObject.Properties[nameof (RetryDelay)].Value as uint?;
      this.SiteSettingsKey = WMIObject.Properties[nameof (SiteSettingsKey)].Value as uint?;
    }

    public uint? RetryCount { get; set; }

    public uint? RetryDelay { get; set; }

    public uint? SiteSettingsKey { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_SoftwareDistributionClientConfig : actualConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.actualConfig.CCM_Policy" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    /// <param name="oClient">A CCM Client object.</param>
    public CCM_SoftwareDistributionClientConfig(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode,
      ccm oClient)
      : base(WMIObject, RemoteRunspace, PSCode, oClient)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.ADV_RebootLogoffNotification = WMIObject.Properties[nameof (ADV_RebootLogoffNotification)].Value as bool?;
      this.ADV_RebootLogoffNotificationCountdownDuration = WMIObject.Properties[nameof (ADV_RebootLogoffNotificationCountdownDuration)].Value as uint?;
      this.ADV_RebootLogoffNotificationFinalWindow = WMIObject.Properties[nameof (ADV_RebootLogoffNotificationFinalWindow)].Value as uint?;
      this.ADV_RunNotificationCountdownDuration = WMIObject.Properties[nameof (ADV_RunNotificationCountdownDuration)].Value as uint?;
      this.ADV_WhatsNewDuration = WMIObject.Properties[nameof (ADV_WhatsNewDuration)].Value as uint?;
      this.CacheContentTimeout = WMIObject.Properties[nameof (CacheContentTimeout)].Value as uint?;
      this.CacheSpaceFailureRetryCount = WMIObject.Properties[nameof (CacheSpaceFailureRetryCount)].Value as uint?;
      this.CacheSpaceFailureRetryInterval = WMIObject.Properties[nameof (CacheSpaceFailureRetryInterval)].Value as uint?;
      this.CacheTombstoneContentMinDuration = WMIObject.Properties[nameof (CacheTombstoneContentMinDuration)].Value as uint?;
      this.ContentLocationTimeoutInterval = WMIObject.Properties[nameof (ContentLocationTimeoutInterval)].Value as uint?;
      this.ContentLocationTimeoutRetryCount = WMIObject.Properties[nameof (ContentLocationTimeoutRetryCount)].Value as uint?;
      this.DefaultMaxDuration = WMIObject.Properties[nameof (DefaultMaxDuration)].Value as uint?;
      this.DisplayNewProgramNotification = WMIObject.Properties[nameof (DisplayNewProgramNotification)].Value as bool?;
      this.ExecutionFailureRetryCount = WMIObject.Properties[nameof (ExecutionFailureRetryCount)].Value as uint?;
      this.ExecutionFailureRetryErrorCodes = WMIObject.Properties[nameof (ExecutionFailureRetryErrorCodes)].Value as uint?[];
      this.ExecutionFailureRetryInterval = WMIObject.Properties[nameof (ExecutionFailureRetryInterval)].Value as uint?;
      this.LockSettings = WMIObject.Properties[nameof (LockSettings)].Value as bool?;
      this.LogoffReturnCodes = WMIObject.Properties[nameof (LogoffReturnCodes)].Value as uint?[];
      this.NetworkAccessPassword = WMIObject.Properties[nameof (NetworkAccessPassword)].Value as string;
      this.NetworkAccessUsername = WMIObject.Properties[nameof (NetworkAccessUsername)].Value as string;
      this.NetworkFailureRetryCount = WMIObject.Properties[nameof (NetworkFailureRetryCount)].Value as uint?;
      this.NetworkFailureRetryInterval = WMIObject.Properties[nameof (NetworkFailureRetryInterval)].Value as uint?;
      this.NewProgramNotificationUI = WMIObject.Properties[nameof (NewProgramNotificationUI)].Value as string;
      this.PRG_PRF_RunNotification = WMIObject.Properties[nameof (PRG_PRF_RunNotification)].Value as bool?;
      this.RebootReturnCodes = WMIObject.Properties[nameof (RebootReturnCodes)].Value as uint?[];
      this.Reserved = WMIObject.Properties[nameof (Reserved)].Value as string;
      this.Reserved1 = WMIObject.Properties[nameof (Reserved1)].Value as string;
      this.Reserved2 = WMIObject.Properties[nameof (Reserved2)].Value as string;
      this.Reserved3 = WMIObject.Properties[nameof (Reserved3)].Value as string;
      this.SiteSettingsKey = WMIObject.Properties[nameof (SiteSettingsKey)].Value as uint?;
      this.SuccessReturnCodes = WMIObject.Properties[nameof (SuccessReturnCodes)].Value as uint?[];
      this.UIContentLocationTimeoutInterval = WMIObject.Properties[nameof (UIContentLocationTimeoutInterval)].Value as uint?;
      this.UserPreemptionCountdown = WMIObject.Properties[nameof (UserPreemptionCountdown)].Value as uint?;
      this.UserPreemptionTimeout = WMIObject.Properties[nameof (UserPreemptionTimeout)].Value as uint?;
    }

    public bool? ADV_RebootLogoffNotification { get; set; }

    public uint? ADV_RebootLogoffNotificationCountdownDuration { get; set; }

    public uint? ADV_RebootLogoffNotificationFinalWindow { get; set; }

    public uint? ADV_RunNotificationCountdownDuration { get; set; }

    public uint? ADV_WhatsNewDuration { get; set; }

    public uint? CacheContentTimeout { get; set; }

    public uint? CacheSpaceFailureRetryCount { get; set; }

    public uint? CacheSpaceFailureRetryInterval { get; set; }

    public uint? CacheTombstoneContentMinDuration { get; set; }

    public uint? ContentLocationTimeoutInterval { get; set; }

    public uint? ContentLocationTimeoutRetryCount { get; set; }

    public uint? DefaultMaxDuration { get; set; }

    public bool? DisplayNewProgramNotification { get; set; }

    public uint? ExecutionFailureRetryCount { get; set; }

    public uint?[] ExecutionFailureRetryErrorCodes { get; set; }

    public uint? ExecutionFailureRetryInterval { get; set; }

    public bool? LockSettings { get; set; }

    public uint?[] LogoffReturnCodes { get; set; }

    public string NetworkAccessPassword { get; set; }

    public string NetworkAccessUsername { get; set; }

    public uint? NetworkFailureRetryCount { get; set; }

    public uint? NetworkFailureRetryInterval { get; set; }

    public string NewProgramNotificationUI { get; set; }

    public bool? PRG_PRF_RunNotification { get; set; }

    public uint?[] RebootReturnCodes { get; set; }

    public string Reserved { get; set; }

    public string Reserved1 { get; set; }

    public string Reserved2 { get; set; }

    public string Reserved3 { get; set; }

    public uint? SiteSettingsKey { get; set; }

    public uint?[] SuccessReturnCodes { get; set; }

    public uint? UIContentLocationTimeoutInterval { get; set; }

    public uint? UserPreemptionCountdown { get; set; }

    public uint? UserPreemptionTimeout { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_ConfigurationManagementClientConfig : actualConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.actualConfig.CCM_Policy" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    /// <param name="oClient">A CCM Client object.</param>
    public CCM_ConfigurationManagementClientConfig(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode,
      ccm oClient)
      : base(WMIObject, RemoteRunspace, PSCode, oClient)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.PerProviderTimeOut = WMIObject.Properties[nameof (PerProviderTimeOut)].Value as uint?;
      this.PerScanTimeout = WMIObject.Properties[nameof (PerScanTimeout)].Value as uint?;
      this.PerScanTTL = WMIObject.Properties[nameof (PerScanTTL)].Value as uint?;
      this.Reserved1 = WMIObject.Properties[nameof (Reserved1)].Value as string;
      this.Reserved2 = WMIObject.Properties[nameof (Reserved2)].Value as string;
      this.Reserved3 = WMIObject.Properties[nameof (Reserved3)].Value as string;
      this.SiteSettingsKey = WMIObject.Properties[nameof (SiteSettingsKey)].Value as uint?;
    }

    public uint? PerProviderTimeOut { get; set; }

    public uint? PerScanTimeout { get; set; }

    public uint? PerScanTTL { get; set; }

    public string Reserved1 { get; set; }

    public string Reserved2 { get; set; }

    public string Reserved3 { get; set; }

    public uint? SiteSettingsKey { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_ClientAgentConfig : actualConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.actualConfig.CCM_Policy" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    /// <param name="oClient">A CCM Client object.</param>
    public CCM_ClientAgentConfig(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode,
      ccm oClient)
      : base(WMIObject, RemoteRunspace, PSCode, oClient)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.BrandingTitle = WMIObject.Properties[nameof (BrandingTitle)].Value as string;
      this.DayReminderInterval = WMIObject.Properties[nameof (DayReminderInterval)].Value as uint?;
      this.DisplayNewProgramNotification = WMIObject.Properties[nameof (DisplayNewProgramNotification)].Value as bool?;
      this.EnableThirdPartyOrchestration = WMIObject.Properties[nameof (EnableThirdPartyOrchestration)].Value as uint?;
      this.HourReminderInterval = WMIObject.Properties[nameof (HourReminderInterval)].Value as uint?;
      this.InstallRestriction = WMIObject.Properties[nameof (InstallRestriction)].Value as uint?;
      this.OSDBrandingSubTitle = WMIObject.Properties[nameof (OSDBrandingSubTitle)].Value as string;
      this.PowerShellExecutionPolicy = WMIObject.Properties[nameof (PowerShellExecutionPolicy)].Value as uint?;
      this.ReminderInterval = WMIObject.Properties[nameof (ReminderInterval)].Value as uint?;
      this.Reserved1 = WMIObject.Properties[nameof (Reserved1)].Value as string;
      this.Reserved2 = WMIObject.Properties[nameof (Reserved2)].Value as string;
      this.Reserved3 = WMIObject.Properties[nameof (Reserved3)].Value as string;
      this.SiteSettingsKey = WMIObject.Properties[nameof (SiteSettingsKey)].Value as uint?;
      this.SUMBrandingSubTitle = WMIObject.Properties[nameof (SUMBrandingSubTitle)].Value as string;
      this.SuspendBitLocker = WMIObject.Properties[nameof (SuspendBitLocker)].Value as uint?;
      this.SWDBrandingSubTitle = WMIObject.Properties[nameof (SWDBrandingSubTitle)].Value as string;
      this.SystemRestartTurnaroundTime = WMIObject.Properties[nameof (SystemRestartTurnaroundTime)].Value as uint?;
    }

    public string BrandingTitle { get; set; }

    public uint? DayReminderInterval { get; set; }

    public bool? DisplayNewProgramNotification { get; set; }

    public uint? EnableThirdPartyOrchestration { get; set; }

    public uint? HourReminderInterval { get; set; }

    public uint? InstallRestriction { get; set; }

    public string OSDBrandingSubTitle { get; set; }

    public uint? PowerShellExecutionPolicy { get; set; }

    public uint? ReminderInterval { get; set; }

    public string Reserved1 { get; set; }

    public string Reserved2 { get; set; }

    public string Reserved3 { get; set; }

    public uint? SiteSettingsKey { get; set; }

    public string SUMBrandingSubTitle { get; set; }

    public uint? SuspendBitLocker { get; set; }

    public string SWDBrandingSubTitle { get; set; }

    public uint? SystemRestartTurnaroundTime { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_SystemHealthClientConfig : actualConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.actualConfig.CCM_Policy" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    /// <param name="oClient">A CCM Client object.</param>
    public CCM_SystemHealthClientConfig(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode,
      ccm oClient)
      : base(WMIObject, RemoteRunspace, PSCode, oClient)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.CumulativeDownloadTimeout = WMIObject.Properties[nameof (CumulativeDownloadTimeout)].Value as uint?;
      this.CumulativeInactivityTimeout = WMIObject.Properties[nameof (CumulativeInactivityTimeout)].Value as uint?;
      this.DPLocality = WMIObject.Properties[nameof (DPLocality)].Value as uint?;
      this.EffectiveTimeinUTC = WMIObject.Properties[nameof (EffectiveTimeinUTC)].Value as uint?;
      this.ForceScan = WMIObject.Properties[nameof (ForceScan)].Value as bool?;
      this.LocationsTimeout = WMIObject.Properties[nameof (LocationsTimeout)].Value as uint?;
      this.PerDPInactivityTimeout = WMIObject.Properties[nameof (PerDPInactivityTimeout)].Value as uint?;
      this.Reserved1 = WMIObject.Properties[nameof (Reserved1)].Value as string;
      this.Reserved2 = WMIObject.Properties[nameof (Reserved2)].Value as string;
      this.Reserved3 = WMIObject.Properties[nameof (Reserved3)].Value as string;
      this.Reserved4 = WMIObject.Properties[nameof (Reserved4)].Value as uint?;
      this.SiteCode = WMIObject.Properties[nameof (SiteCode)].Value as string;
      this.SiteID = WMIObject.Properties[nameof (SiteID)].Value as string;
      this.SiteSettingsKey = WMIObject.Properties[nameof (SiteSettingsKey)].Value as uint?;
    }

    public uint? CumulativeDownloadTimeout { get; set; }

    public uint? CumulativeInactivityTimeout { get; set; }

    public uint? DPLocality { get; set; }

    public uint? EffectiveTimeinUTC { get; set; }

    public bool? ForceScan { get; set; }

    public uint? LocationsTimeout { get; set; }

    public uint? PerDPInactivityTimeout { get; set; }

    public string Reserved1 { get; set; }

    public string Reserved2 { get; set; }

    public string Reserved3 { get; set; }

    public uint? Reserved4 { get; set; }

    public string SiteCode { get; set; }

    public string SiteID { get; set; }

    public uint? SiteSettingsKey { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_PowerManagementClientConfig : actualConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.actualConfig.CCM_Policy" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    /// <param name="oClient">A CCM Client object.</param>
    public CCM_PowerManagementClientConfig(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode,
      ccm oClient)
      : base(WMIObject, RemoteRunspace, PSCode, oClient)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.AllowUserToOptOutFromPowerPlan = WMIObject.Properties[nameof (AllowUserToOptOutFromPowerPlan)].Value as bool?;
      this.EnableUserIdleMonitoring = WMIObject.Properties[nameof (EnableUserIdleMonitoring)].Value as bool?;
      this.NumberOfDaysToKeep = WMIObject.Properties[nameof (NumberOfDaysToKeep)].Value as uint?;
      this.NumberOfMonthsToKeep = WMIObject.Properties[nameof (NumberOfMonthsToKeep)].Value as uint?;
      this.SiteSettingsKey = WMIObject.Properties[nameof (SiteSettingsKey)].Value as uint?;
    }

    public bool? AllowUserToOptOutFromPowerPlan { get; set; }

    public bool? EnableUserIdleMonitoring { get; set; }

    public uint? NumberOfDaysToKeep { get; set; }

    public uint? NumberOfMonthsToKeep { get; set; }

    public uint? SiteSettingsKey { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_SoftwareMeteringClientConfig : actualConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.actualConfig.CCM_Policy" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    /// <param name="oClient">A CCM Client object.</param>
    public CCM_SoftwareMeteringClientConfig(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode,
      ccm oClient)
      : base(WMIObject, RemoteRunspace, PSCode, oClient)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.MaximumUsageInstancesPerReport = WMIObject.Properties[nameof (MaximumUsageInstancesPerReport)].Value as uint?;
      this.ReportTimeout = WMIObject.Properties[nameof (ReportTimeout)].Value as uint?;
      this.Reserved1 = WMIObject.Properties[nameof (Reserved1)].Value as string;
      this.Reserved2 = WMIObject.Properties[nameof (Reserved2)].Value as string;
      this.Reserved3 = WMIObject.Properties[nameof (Reserved3)].Value as string;
      this.SiteSettingsKey = WMIObject.Properties[nameof (SiteSettingsKey)].Value as uint?;
    }

    public uint? MaximumUsageInstancesPerReport { get; set; }

    public uint? ReportTimeout { get; set; }

    public string Reserved1 { get; set; }

    public string Reserved2 { get; set; }

    public string Reserved3 { get; set; }

    public uint? SiteSettingsKey { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_HardwareInventoryClientConfig : actualConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.actualConfig.CCM_Policy" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    /// <param name="oClient">A CCM Client object.</param>
    public CCM_HardwareInventoryClientConfig(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode,
      ccm oClient)
      : base(WMIObject, RemoteRunspace, PSCode, oClient)
    {
      this.remoteRunspace = RemoteRunspace;
      this.Reserved1 = WMIObject.Properties[nameof (Reserved1)].Value as string;
      this.Reserved2 = WMIObject.Properties[nameof (Reserved2)].Value as string;
      this.Reserved3 = WMIObject.Properties[nameof (Reserved3)].Value as string;
      this.SiteSettingsKey = WMIObject.Properties[nameof (SiteSettingsKey)].Value as uint?;
    }

    public string Reserved1 { get; set; }

    public string Reserved2 { get; set; }

    public string Reserved3 { get; set; }

    public uint? SiteSettingsKey { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_RemoteTools_Policy : actualConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.actualConfig.CCM_Policy" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    /// <param name="oClient">A CCM Client object.</param>
    public CCM_RemoteTools_Policy(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode,
      ccm oClient)
      : base(WMIObject, RemoteRunspace, PSCode, oClient)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
    }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_NetworkAccessAccount : actualConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.actualConfig.CCM_Policy" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    /// <param name="oClient">A CCM Client object.</param>
    public CCM_NetworkAccessAccount(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode,
      ccm oClient)
      : base(WMIObject, RemoteRunspace, PSCode, oClient)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.NetworkAccessPassword = WMIObject.Properties[nameof (NetworkAccessPassword)].Value as string;
      this.NetworkAccessUsername = WMIObject.Properties[nameof (NetworkAccessUsername)].Value as string;
      this.Reserved1 = WMIObject.Properties[nameof (Reserved1)].Value as string;
      this.Reserved2 = WMIObject.Properties[nameof (Reserved2)].Value as string;
      this.Reserved3 = WMIObject.Properties[nameof (Reserved3)].Value as string;
      this.SiteSettingsKey = WMIObject.Properties[nameof (SiteSettingsKey)].Value as uint?;
    }

    public string NetworkAccessPassword { get; set; }

    public string NetworkAccessUsername { get; set; }

    public string Reserved1 { get; set; }

    public string Reserved2 { get; set; }

    public string Reserved3 { get; set; }

    public uint? SiteSettingsKey { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_ApplicationManagementClientConfig : actualConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.actualConfig.CCM_Policy" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    /// <param name="oClient">A CCM Client object.</param>
    public CCM_ApplicationManagementClientConfig(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode,
      ccm oClient)
      : base(WMIObject, RemoteRunspace, PSCode, oClient)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.ContentDownloadTimeOut = WMIObject.Properties[nameof (ContentDownloadTimeOut)].Value as uint?;
      this.Reserved1 = WMIObject.Properties[nameof (Reserved1)].Value as string;
      this.Reserved2 = WMIObject.Properties[nameof (Reserved2)].Value as string;
      this.Reserved3 = WMIObject.Properties[nameof (Reserved3)].Value as string;
      this.SiteSettingsKey = WMIObject.Properties[nameof (SiteSettingsKey)].Value as uint?;
    }

    public uint? ContentDownloadTimeOut { get; set; }

    public string Reserved1 { get; set; }

    public string Reserved2 { get; set; }

    public string Reserved3 { get; set; }

    public uint? SiteSettingsKey { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_OutOfBandManagementClientConfig : actualConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.actualConfig.CCM_Policy" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    /// <param name="oClient">A CCM Client object.</param>
    public CCM_OutOfBandManagementClientConfig(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode,
      ccm oClient)
      : base(WMIObject, RemoteRunspace, PSCode, oClient)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.SiteSettingsKey = WMIObject.Properties[nameof (SiteSettingsKey)].Value as uint?;
    }

    public uint? SiteSettingsKey { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_ServiceWindow : actualConfig.CCM_Policy
  {
    internal ccm baseClient;

    internal CCM_ServiceWindow(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode,
      ccm oClient)
      : base(WMIObject, RemoteRunspace, PSCode, oClient)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.baseClient = oClient;
      this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
      this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
      this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.Schedules = WMIObject.Properties[nameof (Schedules)].Value as string;
      this.ServiceWindowID = WMIObject.Properties[nameof (ServiceWindowID)].Value as string;
      this.ServiceWindowType = WMIObject.Properties[nameof (ServiceWindowType)].Value as uint?;
    }

    public string Schedules { get; set; }

    public string ServiceWindowID { get; set; }

    public uint? ServiceWindowType { get; set; }

    /// <summary>
    /// Decode ScheduleID to Object of type: SMS_ST_NonRecurring, SMS_ST_RecurInterval, SMS_ST_RecurWeekly, SMS_ST_RecurMonthlyByWeekday or SMS_ST_RecurMonthlyByDate
    /// </summary>
    public object DecodedSchedule
    {
      get
      {
        string schedules = this.Schedules;
        string[] scheduleIds = ScheduleDecoding.GetScheduleIDs(this.Schedules);
        if (scheduleIds.Length <= 1)
          return scheduleIds.Length == 1 ? ScheduleDecoding.DecodeScheduleID(scheduleIds[0]) : (object) null;
        List<object> decodedSchedule = new List<object>();
        foreach (string ScheduleID in scheduleIds)
          decodedSchedule.Add(ScheduleDecoding.DecodeScheduleID(ScheduleID));
        return (object) decodedSchedule;
      }
    }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_CollectionVariable : actualConfig.CCM_Policy
  {
    internal ccm baseClient;

    internal CCM_CollectionVariable(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode,
      ccm oClient)
      : base(WMIObject, RemoteRunspace, PSCode, oClient)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
      this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
      this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.Name = WMIObject.Properties[nameof (Name)].Value as string;
      this.Value = WMIObject.Properties[nameof (Value)].Value as string;
      this.baseClient = oClient;
    }

    public string Name { get; set; }

    public string Value { get; set; }

    /// <summary>Decode the PolicySecret</summary>
    /// <returns>Decoded secret</returns>
    public string DecodeValue()
    {
      foreach (TraceListener listener in this.pSCode.Listeners)
        listener.Filter = (TraceFilter) new SourceFilter("No match");
      string str = this.baseClient.GetStringFromPS($"{Resources.SecretDecode}\r$PolicySecret = ([XML]'{this.Value}').PolicySecret.FirstChild.Value\rInvoke-Command $script").Replace("\0", "");
      foreach (TraceListener listener in this.pSCode.Listeners)
        listener.Filter = (TraceFilter) null;
      return str;
    }
  }
}
