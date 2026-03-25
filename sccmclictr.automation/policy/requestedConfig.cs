// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.policy.requestedConfig
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using sccmclictr.automation.schedule;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

#nullable disable
namespace sccmclictr.automation.policy;

/// <summary>SCCM Requested Policy.</summary>
public class requestedConfig : baseInit
{
  internal Runspace remoteRunspace;
  internal TraceSource pSCode;
  internal ccm baseClient;

  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.requestedConfig" /> class.
  /// </summary>
  /// <param name="RemoteRunspace">The remote runspace.</param>
  /// <param name="PSCode">The PowerShell code.</param>
  /// <param name="oClient">A CCM Client object.</param>
  public requestedConfig(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
    : base(RemoteRunspace, PSCode)
  {
    this.remoteRunspace = RemoteRunspace;
    this.pSCode = PSCode;
    this.baseClient = oClient;
  }

  /// <summary>Create a new ServiceWindows</summary>
  /// <param name="Schedules">ScheduleID</param>
  /// <param name="ServiceWindowType">1 = All Programs Service Window, 2 = Program Service Window, 3 = Reboot Required Service Window, 4 = Software Update Service Window, 5 = OSD Service Window, 6 = Corresponds to non-working hours.</param>
  /// <returns>Service Window ID (GUID)</returns>
  public string CreateServiceWindow(string Schedules, uint ServiceWindowType)
  {
    string str = Guid.NewGuid().ToString().Replace("{", "").Replace("}", "");
    string[] strArray = new string[11]
    {
      "$a = Set-WmiInstance -Class CCM_ServiceWindow -Namespace 'ROOT\\ccm\\Policy\\Machine\\RequestedConfig' -PutType 'CreateOnly' -argument @{PolicySource = 'LOCAL'; PolicyRuleID = 'NONE'; PolicyVersion = '1.0'; Schedules = '",
      Schedules,
      "'; ServiceWindowType = ",
      ServiceWindowType.ToString(),
      "; ServiceWindowID = '",
      str,
      "'; PolicyID = '",
      str,
      "'; PolicyInstanceID = '",
      str,
      "'};$a.ServiceWindowID"
    };
    foreach (PSObject psObject in this.baseClient.GetObjectsFromPS(string.Concat(strArray)))
    {
      if (psObject != null && psObject.ToString().Length == 36)
        return psObject.ToString();
    }
    return (string) null;
  }

  /// <summary>Deletes a service window.</summary>
  /// <param name="ServiceWindowID">A service window identifier.</param>
  public void DeleteServiceWindow(string ServiceWindowID)
  {
    this.baseClient.GetStringFromPS($"Get-WMIObject -Namespace 'ROOT\\ccm\\Policy\\Machine\\RequestedConfig' -Query 'SELECT * FROM CCM_ServiceWindow WHERE ServiceWindowID = \"{ServiceWindowID}\"' | Remove-WmiObject");
  }

  /// <summary>Gets a list of component client configuration.</summary>
  /// <value>A list of component client configuration.</value>
  public List<requestedConfig.CCM_ComponentClientConfig> ComponentClientConfig
  {
    get
    {
      List<requestedConfig.CCM_ComponentClientConfig> componentClientConfig1 = new List<requestedConfig.CCM_ComponentClientConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\RequestedConfig", "SELECT * FROM CCM_ComponentClientConfig"))
      {
        requestedConfig.CCM_ComponentClientConfig componentClientConfig2 = new requestedConfig.CCM_ComponentClientConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        componentClientConfig2.remoteRunspace = this.remoteRunspace;
        componentClientConfig2.pSCode = this.pSCode;
        componentClientConfig1.Add(componentClientConfig2);
      }
      return componentClientConfig1;
    }
  }

  /// <summary>Gets a list of software updates client configuration.</summary>
  /// <value>A list of software updates client configuration.</value>
  public List<requestedConfig.CCM_SoftwareUpdatesClientConfig> SoftwareUpdatesClientConfig
  {
    get
    {
      List<requestedConfig.CCM_SoftwareUpdatesClientConfig> updatesClientConfig1 = new List<requestedConfig.CCM_SoftwareUpdatesClientConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\RequestedConfig", "SELECT * FROM CCM_SoftwareUpdatesClientConfig"))
      {
        requestedConfig.CCM_SoftwareUpdatesClientConfig updatesClientConfig2 = new requestedConfig.CCM_SoftwareUpdatesClientConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        updatesClientConfig2.remoteRunspace = this.remoteRunspace;
        updatesClientConfig2.pSCode = this.pSCode;
        updatesClientConfig1.Add(updatesClientConfig2);
      }
      return updatesClientConfig1;
    }
  }

  /// <summary>Gets a list of root ca certificates.</summary>
  /// <value>A list of root ca certificates.</value>
  public List<requestedConfig.CCM_RootCACertificates> RootCACertificates
  {
    get
    {
      List<requestedConfig.CCM_RootCACertificates> rootCaCertificates1 = new List<requestedConfig.CCM_RootCACertificates>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\RequestedConfig", "SELECT * FROM CCM_RootCACertificates"))
      {
        requestedConfig.CCM_RootCACertificates rootCaCertificates2 = new requestedConfig.CCM_RootCACertificates(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        rootCaCertificates2.remoteRunspace = this.remoteRunspace;
        rootCaCertificates2.pSCode = this.pSCode;
        rootCaCertificates1.Add(rootCaCertificates2);
      }
      return rootCaCertificates1;
    }
  }

  /// <summary>Gets a list of source update client configuration.</summary>
  /// <value>A list of source update client configuration.</value>
  public List<requestedConfig.CCM_SourceUpdateClientConfig> SourceUpdateClientConfig
  {
    get
    {
      List<requestedConfig.CCM_SourceUpdateClientConfig> updateClientConfig1 = new List<requestedConfig.CCM_SourceUpdateClientConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\RequestedConfig", "SELECT * FROM CCM_SourceUpdateClientConfig"))
      {
        requestedConfig.CCM_SourceUpdateClientConfig updateClientConfig2 = new requestedConfig.CCM_SourceUpdateClientConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        updateClientConfig2.remoteRunspace = this.remoteRunspace;
        updateClientConfig2.pSCode = this.pSCode;
        updateClientConfig1.Add(updateClientConfig2);
      }
      return updateClientConfig1;
    }
  }

  /// <summary>Gets a list of software center settings.</summary>
  /// <value>A list of software center settings.</value>
  public List<requestedConfig.CCM_SoftwareCenterSettings> SoftwareCenterSettings
  {
    get
    {
      List<requestedConfig.CCM_SoftwareCenterSettings> softwareCenterSettings1 = new List<requestedConfig.CCM_SoftwareCenterSettings>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\RequestedConfig", "SELECT * CCM_SoftwareCenterSettings"))
      {
        requestedConfig.CCM_SoftwareCenterSettings softwareCenterSettings2 = new requestedConfig.CCM_SoftwareCenterSettings(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        softwareCenterSettings2.remoteRunspace = this.remoteRunspace;
        softwareCenterSettings2.pSCode = this.pSCode;
        softwareCenterSettings1.Add(softwareCenterSettings2);
      }
      return softwareCenterSettings1;
    }
  }

  /// <summary>
  /// Gets a list of software inventory client configuration.
  /// </summary>
  /// <value>A list of software inventory client configuration.</value>
  public List<requestedConfig.CCM_SoftwareInventoryClientConfig> SoftwareInventoryClientConfig
  {
    get
    {
      List<requestedConfig.CCM_SoftwareInventoryClientConfig> inventoryClientConfig1 = new List<requestedConfig.CCM_SoftwareInventoryClientConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\RequestedConfig", "SELECT * CCM_SoftwareInventoryClientConfig"))
      {
        requestedConfig.CCM_SoftwareInventoryClientConfig inventoryClientConfig2 = new requestedConfig.CCM_SoftwareInventoryClientConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        inventoryClientConfig2.remoteRunspace = this.remoteRunspace;
        inventoryClientConfig2.pSCode = this.pSCode;
        inventoryClientConfig1.Add(inventoryClientConfig2);
      }
      return inventoryClientConfig1;
    }
  }

  /// <summary>Gets a list of targeting settings.</summary>
  /// <value>A list of targeting settings.</value>
  public List<requestedConfig.CCM_TargetingSettings> TargetingSettings
  {
    get
    {
      List<requestedConfig.CCM_TargetingSettings> targetingSettings1 = new List<requestedConfig.CCM_TargetingSettings>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\RequestedConfig", "SELECT * CCM_TargetingSettings"))
      {
        requestedConfig.CCM_TargetingSettings targetingSettings2 = new requestedConfig.CCM_TargetingSettings(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        targetingSettings2.remoteRunspace = this.remoteRunspace;
        targetingSettings2.pSCode = this.pSCode;
        targetingSettings1.Add(targetingSettings2);
      }
      return targetingSettings1;
    }
  }

  /// <summary>Gets a list of multicast configuration.</summary>
  /// <value>A list of multicast configuration.</value>
  public List<requestedConfig.CCM_MulticastConfig> MulticastConfig
  {
    get
    {
      List<requestedConfig.CCM_MulticastConfig> multicastConfig = new List<requestedConfig.CCM_MulticastConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\RequestedConfig", "SELECT * CCM_MulticastConfig"))
      {
        requestedConfig.CCM_MulticastConfig ccmMulticastConfig = new requestedConfig.CCM_MulticastConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
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
  public List<requestedConfig.CCM_SoftwareDistributionClientConfig> SoftwareDistributionClientConfig
  {
    get
    {
      List<requestedConfig.CCM_SoftwareDistributionClientConfig> distributionClientConfig1 = new List<requestedConfig.CCM_SoftwareDistributionClientConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\RequestedConfig", "SELECT * CCM_SoftwareDistributionClientConfig"))
      {
        requestedConfig.CCM_SoftwareDistributionClientConfig distributionClientConfig2 = new requestedConfig.CCM_SoftwareDistributionClientConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        distributionClientConfig2.remoteRunspace = this.remoteRunspace;
        distributionClientConfig2.pSCode = this.pSCode;
        distributionClientConfig1.Add(distributionClientConfig2);
      }
      return distributionClientConfig1;
    }
  }

  /// <summary>
  /// Gets a list of configuration management client configuration.
  /// </summary>
  /// <value>A list of configuration management client configuration.</value>
  public List<requestedConfig.CCM_ConfigurationManagementClientConfig> ConfigurationManagementClientConfig
  {
    get
    {
      List<requestedConfig.CCM_ConfigurationManagementClientConfig> managementClientConfig1 = new List<requestedConfig.CCM_ConfigurationManagementClientConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\RequestedConfig", "SELECT * CCM_ConfigurationManagementClientConfig"))
      {
        requestedConfig.CCM_ConfigurationManagementClientConfig managementClientConfig2 = new requestedConfig.CCM_ConfigurationManagementClientConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        managementClientConfig2.remoteRunspace = this.remoteRunspace;
        managementClientConfig2.pSCode = this.pSCode;
        managementClientConfig1.Add(managementClientConfig2);
      }
      return managementClientConfig1;
    }
  }

  /// <summary>Gets a list of client agent configuration.</summary>
  /// <value>A list of client agent configuration.</value>
  public List<requestedConfig.CCM_ClientAgentConfig> ClientAgentConfig
  {
    get
    {
      List<requestedConfig.CCM_ClientAgentConfig> clientAgentConfig1 = new List<requestedConfig.CCM_ClientAgentConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\RequestedConfig", "SELECT * CCM_ClientAgentConfig"))
      {
        requestedConfig.CCM_ClientAgentConfig clientAgentConfig2 = new requestedConfig.CCM_ClientAgentConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        clientAgentConfig2.remoteRunspace = this.remoteRunspace;
        clientAgentConfig2.pSCode = this.pSCode;
        clientAgentConfig1.Add(clientAgentConfig2);
      }
      return clientAgentConfig1;
    }
  }

  /// <summary>Gets a list of system health client configuration.</summary>
  /// <value>A list of system health client configuration.</value>
  public List<requestedConfig.CCM_SystemHealthClientConfig> SystemHealthClientConfig
  {
    get
    {
      List<requestedConfig.CCM_SystemHealthClientConfig> healthClientConfig1 = new List<requestedConfig.CCM_SystemHealthClientConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\RequestedConfig", "SELECT * CCM_SystemHealthClientConfig"))
      {
        requestedConfig.CCM_SystemHealthClientConfig healthClientConfig2 = new requestedConfig.CCM_SystemHealthClientConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        healthClientConfig2.remoteRunspace = this.remoteRunspace;
        healthClientConfig2.pSCode = this.pSCode;
        healthClientConfig1.Add(healthClientConfig2);
      }
      return healthClientConfig1;
    }
  }

  /// <summary>Gets a list of power management client configuration.</summary>
  /// <value>A list of power management client configuration.</value>
  public List<requestedConfig.CCM_PowerManagementClientConfig> PowerManagementClientConfig
  {
    get
    {
      List<requestedConfig.CCM_PowerManagementClientConfig> managementClientConfig1 = new List<requestedConfig.CCM_PowerManagementClientConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\RequestedConfig", "SELECT * CCM_PowerManagementClientConfig"))
      {
        requestedConfig.CCM_PowerManagementClientConfig managementClientConfig2 = new requestedConfig.CCM_PowerManagementClientConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        managementClientConfig2.remoteRunspace = this.remoteRunspace;
        managementClientConfig2.pSCode = this.pSCode;
        managementClientConfig1.Add(managementClientConfig2);
      }
      return managementClientConfig1;
    }
  }

  /// <summary>
  /// Gets a list of software metering client configuration.
  /// </summary>
  /// <value>A list of software metering client configuration.</value>
  public List<requestedConfig.CCM_SoftwareMeteringClientConfig> SoftwareMeteringClientConfig
  {
    get
    {
      List<requestedConfig.CCM_SoftwareMeteringClientConfig> meteringClientConfig1 = new List<requestedConfig.CCM_SoftwareMeteringClientConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\RequestedConfig", "SELECT * CCM_SoftwareMeteringClientConfig"))
      {
        requestedConfig.CCM_SoftwareMeteringClientConfig meteringClientConfig2 = new requestedConfig.CCM_SoftwareMeteringClientConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        meteringClientConfig2.remoteRunspace = this.remoteRunspace;
        meteringClientConfig2.pSCode = this.pSCode;
        meteringClientConfig1.Add(meteringClientConfig2);
      }
      return meteringClientConfig1;
    }
  }

  /// <summary>
  /// Gets a list of hardware inventory client configuration.
  /// </summary>
  /// <value>A list of hardware inventory client configuration.</value>
  public List<requestedConfig.CCM_HardwareInventoryClientConfig> HardwareInventoryClientConfig
  {
    get
    {
      List<requestedConfig.CCM_HardwareInventoryClientConfig> inventoryClientConfig1 = new List<requestedConfig.CCM_HardwareInventoryClientConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\RequestedConfig", "SELECT * CCM_HardwareInventoryClientConfig"))
      {
        requestedConfig.CCM_HardwareInventoryClientConfig inventoryClientConfig2 = new requestedConfig.CCM_HardwareInventoryClientConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        inventoryClientConfig2.remoteRunspace = this.remoteRunspace;
        inventoryClientConfig2.pSCode = this.pSCode;
        inventoryClientConfig1.Add(inventoryClientConfig2);
      }
      return inventoryClientConfig1;
    }
  }

  /// <summary>Gets A list of remote tools policies.</summary>
  /// <value>A list of remote tools policies.</value>
  public List<requestedConfig.CCM_RemoteTools_Policy> RemoteTools_Policy
  {
    get
    {
      List<requestedConfig.CCM_RemoteTools_Policy> remoteToolsPolicy1 = new List<requestedConfig.CCM_RemoteTools_Policy>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\RequestedConfig", "SELECT * CCM_RemoteTools_Policy"))
      {
        requestedConfig.CCM_RemoteTools_Policy remoteToolsPolicy2 = new requestedConfig.CCM_RemoteTools_Policy(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        remoteToolsPolicy2.remoteRunspace = this.remoteRunspace;
        remoteToolsPolicy2.pSCode = this.pSCode;
        remoteToolsPolicy1.Add(remoteToolsPolicy2);
      }
      return remoteToolsPolicy1;
    }
  }

  /// <summary>Gets a list of network access accounts.</summary>
  /// <value>A list of network access accounts.</value>
  public List<requestedConfig.CCM_NetworkAccessAccount> NetworkAccessAccount
  {
    get
    {
      List<requestedConfig.CCM_NetworkAccessAccount> networkAccessAccount1 = new List<requestedConfig.CCM_NetworkAccessAccount>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\RequestedConfig", "SELECT * CCM_NetworkAccessAccount"))
      {
        requestedConfig.CCM_NetworkAccessAccount networkAccessAccount2 = new requestedConfig.CCM_NetworkAccessAccount(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
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
  public List<requestedConfig.CCM_ApplicationManagementClientConfig> ApplicationManagementClientConfig
  {
    get
    {
      List<requestedConfig.CCM_ApplicationManagementClientConfig> managementClientConfig1 = new List<requestedConfig.CCM_ApplicationManagementClientConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\RequestedConfig", "SELECT * CCM_ApplicationManagementClientConfig"))
      {
        requestedConfig.CCM_ApplicationManagementClientConfig managementClientConfig2 = new requestedConfig.CCM_ApplicationManagementClientConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        managementClientConfig2.remoteRunspace = this.remoteRunspace;
        managementClientConfig2.pSCode = this.pSCode;
        managementClientConfig1.Add(managementClientConfig2);
      }
      return managementClientConfig1;
    }
  }

  /// <summary>List of CCM_OutOfBandManagementClientConfig objects</summary>
  public List<requestedConfig.CCM_OutOfBandManagementClientConfig> OutOfBandManagementClientConfig
  {
    get
    {
      List<requestedConfig.CCM_OutOfBandManagementClientConfig> managementClientConfig1 = new List<requestedConfig.CCM_OutOfBandManagementClientConfig>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\RequestedConfig", "SELECT * CCM_OutOfBandManagementClientConfig"))
      {
        requestedConfig.CCM_OutOfBandManagementClientConfig managementClientConfig2 = new requestedConfig.CCM_OutOfBandManagementClientConfig(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        managementClientConfig2.remoteRunspace = this.remoteRunspace;
        managementClientConfig2.pSCode = this.pSCode;
        managementClientConfig1.Add(managementClientConfig2);
      }
      return managementClientConfig1;
    }
  }

  /// <summary>List of CCM_ServiceWindow Objects</summary>
  public List<requestedConfig.CCM_ServiceWindow> ServiceWindow
  {
    get
    {
      List<requestedConfig.CCM_ServiceWindow> serviceWindow = new List<requestedConfig.CCM_ServiceWindow>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\RequestedConfig", "SELECT * FROM CCM_ServiceWindow", true))
      {
        requestedConfig.CCM_ServiceWindow ccmServiceWindow = new requestedConfig.CCM_ServiceWindow(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient);
        ccmServiceWindow.remoteRunspace = this.remoteRunspace;
        ccmServiceWindow.pSCode = this.pSCode;
        serviceWindow.Add(ccmServiceWindow);
      }
      return serviceWindow;
    }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\RequestedConfig</summary>
  public class CCM_Policy
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.requestedConfig.CCM_Policy" /> class.
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
      this.PolicyID = WMIObject.Properties[nameof (PolicyID)].Value as string;
      this.PolicyInstanceID = WMIObject.Properties[nameof (PolicyInstanceID)].Value as string;
      this.PolicyPrecedence = WMIObject.Properties[nameof (PolicyPrecedence)].Value as uint?;
      this.PolicyRuleID = WMIObject.Properties[nameof (PolicyRuleID)].Value as string;
      this.PolicySource = WMIObject.Properties[nameof (PolicySource)].Value as string;
      this.PolicyVersion = WMIObject.Properties[nameof (PolicyVersion)].Value as string;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    /// <summary>Gets or sets the policy identifier.</summary>
    /// <value>Unique ID of the policy.</value>
    public string PolicyID { get; set; }

    /// <summary>Gets or sets the policy instance identifier.</summary>
    /// <value>The policy instance identifier.</value>
    public string PolicyInstanceID { get; set; }

    /// <summary>Gets or sets the policy precedence.</summary>
    /// <value>The policy precedence.</value>
    public uint? PolicyPrecedence { get; set; }

    /// <summary>Gets or sets the policy rule identifier.</summary>
    /// <value>Unique ID of the rule used to create the policy.</value>
    public string PolicyRuleID { get; set; }

    /// <summary>Gets or sets the policy source.</summary>
    /// <value>The policy source.</value>
    public string PolicySource { get; set; }

    /// <summary>Gets or sets the policy version.</summary>
    /// <value>The policy version.</value>
    public string PolicyVersion { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\RequestedConfig</summary>
  public class CCM_ComponentClientConfig : requestedConfig.CCM_Policy
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.requestedConfig.CCM_Policy" /> class.
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

    /// <summary>Gets or sets the name of the component.</summary>
    /// <value>The name of the client component.</value>
    public string ComponentName { get; set; }

    /// <summary>Gets or sets the enabled.</summary>
    /// <value>Shows weather the client component is enabled or not</value>
    public bool? Enabled { get; set; }
  }

  /// <summary>Source:ROOT\ccm\policy\Machine\RequestedConfig</summary>
  public class CCM_SoftwareUpdatesClientConfig : requestedConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.requestedConfig.CCM_Policy" /> class.
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

  /// <summary>Source:ROOT\ccm\Policy\Machine\RequestedConfig</summary>
  public class CCM_RootCACertificates : requestedConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.requestedConfig.CCM_Policy" /> class.
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

  /// <summary>Source:ROOT\ccm\Policy\Machine\RequestedConfig</summary>
  public class CCM_SourceUpdateClientConfig : requestedConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.requestedConfig.CCM_Policy" /> class.
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

  /// <summary>Source:ROOT\ccm\Policy\Machine\RequestedConfig</summary>
  public class CCM_SoftwareCenterSettings : requestedConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.requestedConfig.CCM_Policy" /> class.
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

  /// <summary>Source:ROOT\ccm\Policy\Machine\RequestedConfig</summary>
  public class CCM_SoftwareInventoryClientConfig : requestedConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.requestedConfig.CCM_Policy" /> class.
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

  /// <summary>Source:ROOT\ccm\Policy\Machine\RequestedConfig</summary>
  public class CCM_TargetingSettings : requestedConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.requestedConfig.CCM_Policy" /> class.
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

  /// <summary>Source:ROOT\ccm\Policy\Machine\RequestedConfig</summary>
  public class CCM_MulticastConfig : requestedConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.requestedConfig.CCM_Policy" /> class.
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

  /// <summary>Source:ROOT\ccm\policy\Machine\RequestedConfig</summary>
  public class CCM_SoftwareDistributionClientConfig : requestedConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.requestedConfig.CCM_Policy" /> class.
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

    /// <summary>
    /// Gets or sets the advert reboot/logoff notification flag.
    /// </summary>
    /// <value>If TRUE a notification countdown timer will be shown telling user they will be logged off or the computer will be rebooted.</value>
    public bool? ADV_RebootLogoffNotification { get; set; }

    /// <summary>
    /// Gets or sets the duration of the advert reboot/logoff notification countdown.
    /// </summary>
    /// <value>The duration in seconds of the reboot/logoff notification countdown.</value>
    public uint? ADV_RebootLogoffNotificationCountdownDuration { get; set; }

    /// <summary>
    /// Gets or sets the advert reboot/logoff notification final window.
    /// </summary>
    /// <value>The duration in seconds of the final reboot/logoff notification countdown.</value>
    public uint? ADV_RebootLogoffNotificationFinalWindow { get; set; }

    /// <summary>
    /// Gets or sets the duration of the advert run notification countdown.
    /// </summary>
    /// <value>The duration in seconds of the advert run notification countdown.</value>
    public uint? ADV_RunNotificationCountdownDuration { get; set; }

    /// <summary>
    /// Gets or sets the duration that an advert appears in the whats new section.
    /// </summary>
    /// <value>The duration in days that a advert appears in the Whats new section.</value>
    public uint? ADV_WhatsNewDuration { get; set; }

    /// <summary>Gets or sets the cache content timeout.</summary>
    /// <value>The cache content timeout in seconds. After this period content can be deleted.</value>
    public uint? CacheContentTimeout { get; set; }

    /// <summary>Gets or sets the cache space failure retry count.</summary>
    /// <value>The number of times to retry caching content if it fails due to space restrictions.</value>
    public uint? CacheSpaceFailureRetryCount { get; set; }

    /// <summary>Gets or sets the cache space failure retry interval.</summary>
    /// <value>The time in seconds to wait before retrying to cache content that previous failed.</value>
    public uint? CacheSpaceFailureRetryInterval { get; set; }

    /// <summary>
    /// Gets or sets the minimum duration the content should be available in the cache.
    /// </summary>
    /// <value>The minimum duration in seconds that the content should be available in the cache.</value>
    public uint? CacheTombstoneContentMinDuration { get; set; }

    /// <summary>Gets or sets the content location timeout interval.</summary>
    /// <value>The maximum time in seconds that the client should search for content locations.</value>
    public uint? ContentLocationTimeoutInterval { get; set; }

    /// <summary>
    /// Gets or sets the content location timeout retry count.
    /// </summary>
    /// <value>The maximum number of times that the client should search for content locations.</value>
    public uint? ContentLocationTimeoutRetryCount { get; set; }

    /// <summary>Gets or sets the default duration of the program.</summary>
    /// <value>The default duration that the program can run for.</value>
    public uint? DefaultMaxDuration { get; set; }

    /// <summary>Gets or sets the display new program notification.</summary>
    /// <value>If TRUE notify the user that a new program is available.</value>
    public bool? DisplayNewProgramNotification { get; set; }

    /// <summary>Gets or sets the execution failure retry count.</summary>
    /// <value>The number of times to retry running the program if it previously failed.</value>
    public uint? ExecutionFailureRetryCount { get; set; }

    /// <summary>Gets or sets the execution failure retry error codes.</summary>
    /// <value>The error codes that will trigger a retry. refer to: http://msdn.microsoft.com/en-us/library/cc143632.aspx for the list of error codes</value>
    public uint?[] ExecutionFailureRetryErrorCodes { get; set; }

    /// <summary>Gets or sets the execution failure retry interval.</summary>
    /// <value>The execution failure retry interval.</value>
    public uint? ExecutionFailureRetryInterval { get; set; }

    /// <summary>Gets or sets the lock settings.</summary>
    /// <value>If TRUE site settings cannot be overridden.</value>
    public bool? LockSettings { get; set; }

    /// <summary>Gets or sets the logoff return codes.</summary>
    /// <value>Array of return codes that if returned by a program signals that a logoff is required.</value>
    public uint?[] LogoffReturnCodes { get; set; }

    /// <summary>Gets or sets the network access account password.</summary>
    /// <value>The network access account password.</value>
    public string NetworkAccessPassword { get; set; }

    /// <summary>Gets or sets the network access account username.</summary>
    /// <value>The network access account username.</value>
    public string NetworkAccessUsername { get; set; }

    /// <summary>Gets or sets the network failure retry count.</summary>
    /// <value>The number of times to try connecting to the network.</value>
    public uint? NetworkFailureRetryCount { get; set; }

    /// <summary>Gets or sets the network failure retry interval.</summary>
    /// <value>The time in seconds between trying to connect to the network.</value>
    public uint? NetworkFailureRetryInterval { get; set; }

    /// <summary>
    /// Gets or sets the new program notification UI interface.
    /// </summary>
    /// <value>The new program notification UI interface. Either ARP or RAP. This is the interface that is displayed when the notification ballon is clicked.</value>
    public string NewProgramNotificationUI { get; set; }

    /// <summary>Gets or sets the program run notification.</summary>
    /// <value>If TRUE signifys that the client will display a countdown notification.</value>
    public bool? PRG_PRF_RunNotification { get; set; }

    /// <summary>Gets or sets the reboot return codes.</summary>
    /// <value>An array of return codes that if a program returns signifies that a reboot is required.</value>
    public uint?[] RebootReturnCodes { get; set; }

    /// <summary>Gets or sets reserved.</summary>
    /// <value>Reserved.</value>
    public string Reserved { get; set; }

    /// <summary>Gets or sets reserved1.</summary>
    /// <value>Reserved1.</value>
    public string Reserved1 { get; set; }

    /// <summary>Gets or sets reserved2.</summary>
    /// <value>Reserved2.</value>
    public string Reserved2 { get; set; }

    /// <summary>Gets or sets reserved3.</summary>
    /// <value>Reserved3.</value>
    public string Reserved3 { get; set; }

    /// <summary>Gets or sets the site settings key.</summary>
    /// <value>The site settings key.</value>
    public uint? SiteSettingsKey { get; set; }

    /// <summary>Gets or sets the success return codes.</summary>
    /// <value>An array of return codes that if a program returns signifies that a the program installed successfully.</value>
    public uint?[] SuccessReturnCodes { get; set; }

    /// <summary>
    /// Gets or sets the UI content location timeout interval.
    /// </summary>
    /// <value>The UI content location timeout interval.</value>
    public uint? UIContentLocationTimeoutInterval { get; set; }

    /// <summary>Gets or sets the user preemption countdown.</summary>
    /// <value>The user preemption countdown.</value>
    public uint? UserPreemptionCountdown { get; set; }

    /// <summary>Gets or sets the user preemption timeout.</summary>
    /// <value>The user preemption timeout.</value>
    public uint? UserPreemptionTimeout { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\RequestedConfig</summary>
  public class CCM_ConfigurationManagementClientConfig : requestedConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.requestedConfig.CCM_Policy" /> class.
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

  /// <summary>Source:ROOT\ccm\Policy\Machine\RequestedConfig</summary>
  public class CCM_ClientAgentConfig : requestedConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.requestedConfig.CCM_Policy" /> class.
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

  /// <summary>Source:ROOT\ccm\Policy\Machine\RequestedConfig</summary>
  public class CCM_SystemHealthClientConfig : requestedConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.requestedConfig.CCM_Policy" /> class.
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

  /// <summary>Source:ROOT\ccm\Policy\Machine\RequestedConfig</summary>
  public class CCM_PowerManagementClientConfig : requestedConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.requestedConfig.CCM_Policy" /> class.
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

  /// <summary>Source:ROOT\ccm\Policy\Machine\RequestedConfig</summary>
  public class CCM_SoftwareMeteringClientConfig : requestedConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.requestedConfig.CCM_Policy" /> class.
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

  /// <summary>Source:ROOT\ccm\Policy\Machine\RequestedConfig</summary>
  public class CCM_HardwareInventoryClientConfig : requestedConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.requestedConfig.CCM_Policy" /> class.
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

  /// <summary>Source:ROOT\ccm\Policy\Machine\RequestedConfig</summary>
  public class CCM_RemoteTools_Policy : requestedConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.requestedConfig.CCM_Policy" /> class.
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

  /// <summary>Source:ROOT\ccm\Policy\Machine\RequestedConfig</summary>
  public class CCM_NetworkAccessAccount : requestedConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.requestedConfig.CCM_Policy" /> class.
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

    /// <summary>Gets or sets the network access account password.</summary>
    /// <value>The network access account password.</value>
    public string NetworkAccessPassword { get; set; }

    /// <summary>Gets or sets the network access account username.</summary>
    /// <value>The network access account username.</value>
    public string NetworkAccessUsername { get; set; }

    /// <summary>Gets or sets reserved1.</summary>
    /// <value>Reserved1.</value>
    public string Reserved1 { get; set; }

    /// <summary>Gets or sets reserved2.</summary>
    /// <value>Reserved2.</value>
    public string Reserved2 { get; set; }

    /// <summary>Gets or sets reserved3.</summary>
    /// <value>Reserved3.</value>
    public string Reserved3 { get; set; }

    /// <summary>Gets or sets the site settings key.</summary>
    /// <value>The site settings key.</value>
    public uint? SiteSettingsKey { get; set; }
  }

  /// <summary>Source:ROOT\ccm\policy\Machine\RequestedConfig</summary>
  public class CCM_ApplicationManagementClientConfig : requestedConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.requestedConfig.CCM_Policy" /> class.
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

  /// <summary>Source:ROOT\ccm\policy\Machine\RequestedConfig</summary>
  public class CCM_OutOfBandManagementClientConfig : requestedConfig.CCM_ComponentClientConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.requestedConfig.CCM_Policy" /> class.
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

  /// <summary>Source:ROOT\ccm\Policy\Machine\RequestedConfig</summary>
  public class CCM_ServiceWindow : requestedConfig.CCM_Policy
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.policy.requestedConfig.CCM_Policy" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    /// <param name="oClient">A CCM Client object.</param>
    public CCM_ServiceWindow(
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
}
