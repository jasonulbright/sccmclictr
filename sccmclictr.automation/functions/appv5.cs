// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.functions.appv5
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

#nullable disable
namespace sccmclictr.automation.functions;

/// <summary>
/// App-V 5.x Classes
/// Thanks to Mattias Benninge to provide the details
/// </summary>
public class appv5 : baseInit
{
  internal Runspace remoteRunspace;
  internal TraceSource pSCode;
  internal ccm baseClient;

  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.appv5" /> class.
  /// </summary>
  /// <param name="RemoteRunspace">The remote runspace.</param>
  /// <param name="PSCode">The PowerShell code.</param>
  /// <param name="oClient">A CCM client object.</param>
  public appv5(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
    : base(RemoteRunspace, PSCode)
  {
    this.remoteRunspace = RemoteRunspace;
    this.pSCode = PSCode;
    this.baseClient = oClient;
  }

  /// <summary>Gets a list of AppV5 client applications.</summary>
  /// <value>A list of AppV5 client applications.</value>
  public List<appv5.AppvClientApplication> Appv5ClientApplications
  {
    get
    {
      List<appv5.AppvClientApplication> clientApplications = new List<appv5.AppvClientApplication>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\Appv", "SELECT * FROM AppvClientApplication"))
        clientApplications.Add(new appv5.AppvClientApplication(WMIObject, this.remoteRunspace, this.pSCode)
        {
          remoteRunspace = this.remoteRunspace,
          pSCode = this.pSCode
        });
      return clientApplications;
    }
  }

  /// <summary>Gets a list of AppV5 client assets.</summary>
  /// <value>A list of AppV5 client assets.</value>
  public List<appv5.AppvClientAsset> Appv5AppvClientAssets
  {
    get
    {
      List<appv5.AppvClientAsset> appvClientAssets = new List<appv5.AppvClientAsset>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\Appv", "SELECT * FROM AppvClientAsset"))
        appvClientAssets.Add(new appv5.AppvClientAsset(WMIObject, this.remoteRunspace, this.pSCode)
        {
          remoteRunspace = this.remoteRunspace,
          pSCode = this.pSCode
        });
      return appvClientAssets;
    }
  }

  /// <summary>Gets a list of AppV5 client connection groups.</summary>
  /// <value>A list of AppV5 client connection groups.</value>
  public List<appv5.AppvClientConnectionGroup> Appv5AppvClientConnectionGroups
  {
    get
    {
      List<appv5.AppvClientConnectionGroup> connectionGroups = new List<appv5.AppvClientConnectionGroup>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\Appv", "SELECT * FROM AppvClientConnectionGroup"))
        connectionGroups.Add(new appv5.AppvClientConnectionGroup(WMIObject, this.remoteRunspace, this.pSCode)
        {
          remoteRunspace = this.remoteRunspace,
          pSCode = this.pSCode
        });
      return connectionGroups;
    }
  }

  /// <summary>Gets a list of AppV5 client packages.</summary>
  /// <value>A list of AppV5 client packages.</value>
  public List<appv5.AppvClientPackage> Appv5AppvClientPackages
  {
    get
    {
      List<appv5.AppvClientPackage> appvClientPackages = new List<appv5.AppvClientPackage>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\Appv", "SELECT * FROM AppvClientPackage"))
        appvClientPackages.Add(new appv5.AppvClientPackage(WMIObject, this.remoteRunspace, this.pSCode)
        {
          remoteRunspace = this.remoteRunspace,
          pSCode = this.pSCode
        });
      return appvClientPackages;
    }
  }

  /// <summary>Gets a list of AppV5 publishing servers.</summary>
  /// <value>A list of AppV5 publishing servers.</value>
  public List<appv5.AppvPublishingServer> Appv5AppvPublishingServers
  {
    get
    {
      List<appv5.AppvPublishingServer> publishingServers = new List<appv5.AppvPublishingServer>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\Appv", "SELECT * FROM AppvPublishingServer"))
        publishingServers.Add(new appv5.AppvPublishingServer(WMIObject, this.remoteRunspace, this.pSCode)
        {
          remoteRunspace = this.remoteRunspace,
          pSCode = this.pSCode
        });
      return publishingServers;
    }
  }

  /// <summary>Source:ROOT\Appv</summary>
  public class AppvClientApplication
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.appv5.AppvClientApplication" /> class.
    /// </summary>
    /// <param name="WMIObject">A WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public AppvClientApplication(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.ApplicationId = WMIObject.Properties[nameof (ApplicationId)].Value as string;
      this.EnabledForUser = WMIObject.Properties[nameof (EnabledForUser)].Value as bool?;
      this.EnabledGlobally = WMIObject.Properties[nameof (EnabledGlobally)].Value as bool?;
      this.Name = WMIObject.Properties[nameof (Name)].Value as string;
      this.PackageId = WMIObject.Properties[nameof (PackageId)].Value as string;
      this.PackageVersionId = WMIObject.Properties[nameof (PackageVersionId)].Value as string;
      this.TargetPath = WMIObject.Properties[nameof (TargetPath)].Value as string;
      this.Version = WMIObject.Properties[nameof (Version)].Value as string;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    public string ApplicationId { get; set; }

    public bool? EnabledForUser { get; set; }

    public bool? EnabledGlobally { get; set; }

    public string Name { get; set; }

    public string PackageId { get; set; }

    public string PackageVersionId { get; set; }

    public string TargetPath { get; set; }

    public string Version { get; set; }
  }

  /// <summary>Source:ROOT\Appv</summary>
  public class AppvClientAsset
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.appv5.AppvClientAsset" /> class.
    /// </summary>
    /// <param name="WMIObject">A WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public AppvClientAsset(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.ChannelCode = WMIObject.Properties[nameof (ChannelCode)].Value as string;
      this.CM_DSLID = WMIObject.Properties[nameof (CM_DSLID)].Value as string;
      this.InstallDate = WMIObject.Properties[nameof (InstallDate)].Value as string;
      this.InstalledLocation = WMIObject.Properties[nameof (InstalledLocation)].Value as string;
      this.Language = WMIObject.Properties[nameof (Language)].Value as string;
      this.OsComponent = WMIObject.Properties[nameof (OsComponent)].Value as string;
      this.PackageId = WMIObject.Properties[nameof (PackageId)].Value as string;
      this.PackageVersionId = WMIObject.Properties[nameof (PackageVersionId)].Value as string;
      this.ProductID = WMIObject.Properties[nameof (ProductID)].Value as string;
      this.ProductName = WMIObject.Properties[nameof (ProductName)].Value as string;
      this.ProductVersion = WMIObject.Properties[nameof (ProductVersion)].Value as string;
      this.Publisher = WMIObject.Properties[nameof (Publisher)].Value as string;
      this.RegisteredUser = WMIObject.Properties[nameof (RegisteredUser)].Value as string;
      this.ServicePack = WMIObject.Properties[nameof (ServicePack)].Value as string;
      this.SoftwareCode = WMIObject.Properties[nameof (SoftwareCode)].Value as string;
      this.UpgradeCode = WMIObject.Properties[nameof (UpgradeCode)].Value as string;
      this.VersionMajor = WMIObject.Properties[nameof (VersionMajor)].Value as string;
      this.VersionMinor = WMIObject.Properties[nameof (VersionMinor)].Value as string;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    public string ChannelCode { get; set; }

    public string CM_DSLID { get; set; }

    public string InstallDate { get; set; }

    public string InstalledLocation { get; set; }

    public string Language { get; set; }

    public string OsComponent { get; set; }

    public string PackageId { get; set; }

    public string PackageVersionId { get; set; }

    public string ProductID { get; set; }

    public string ProductName { get; set; }

    public string ProductVersion { get; set; }

    public string Publisher { get; set; }

    public string RegisteredUser { get; set; }

    public string ServicePack { get; set; }

    public string SoftwareCode { get; set; }

    public string UpgradeCode { get; set; }

    public string VersionMajor { get; set; }

    public string VersionMinor { get; set; }
  }

  /// <summary>Source:ROOT\Appv</summary>
  public class AppvClientConnectionGroup
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.appv5.AppvClientConnectionGroup" /> class.
    /// </summary>
    /// <param name="WMIObject">A WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public AppvClientConnectionGroup(
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
      this.CustomData = WMIObject.Properties[nameof (CustomData)].Value as string;
      this.GlobalPending = WMIObject.Properties[nameof (GlobalPending)].Value as bool?;
      this.GroupId = WMIObject.Properties[nameof (GroupId)].Value as string;
      this.InUse = WMIObject.Properties[nameof (InUse)].Value as bool?;
      this.IsEnabledGlobally = WMIObject.Properties[nameof (IsEnabledGlobally)].Value as bool?;
      this.IsEnabledToUser = WMIObject.Properties[nameof (IsEnabledToUser)].Value as bool?;
      this.Name = WMIObject.Properties[nameof (Name)].Value as string;
      this.Packages = WMIObject.Properties[nameof (Packages)].Value as string[];
      this.PercentLoaded = WMIObject.Properties[nameof (PercentLoaded)].Value as ushort?;
      this.Priority = WMIObject.Properties[nameof (Priority)].Value as uint?;
      this.UserPending = WMIObject.Properties[nameof (UserPending)].Value as bool?;
      this.VersionId = WMIObject.Properties[nameof (VersionId)].Value as string;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    public string CustomData { get; set; }

    public bool? GlobalPending { get; set; }

    public string GroupId { get; set; }

    public bool? InUse { get; set; }

    public bool? IsEnabledGlobally { get; set; }

    public bool? IsEnabledToUser { get; set; }

    public string Name { get; set; }

    public string[] Packages { get; set; }

    public ushort? PercentLoaded { get; set; }

    public uint? Priority { get; set; }

    public bool? UserPending { get; set; }

    public string VersionId { get; set; }
  }

  /// <summary>Source:ROOT\Appv</summary>
  public class AppvClientPackage
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.appv5.AppvClientPackage" /> class.
    /// </summary>
    /// <param name="WMIObject">A WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public AppvClientPackage(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.Assets = WMIObject.Properties[nameof (Assets)].Value as string[];
      this.DeploymentMachineData = WMIObject.Properties[nameof (DeploymentMachineData)].Value as string;
      this.DeploymentUserData = WMIObject.Properties[nameof (DeploymentUserData)].Value as string;
      this.GlobalPending = WMIObject.Properties[nameof (GlobalPending)].Value as bool?;
      this.HasAssetIntelligence = WMIObject.Properties[nameof (HasAssetIntelligence)].Value as bool?;
      this.InUse = WMIObject.Properties[nameof (InUse)].Value as bool?;
      this.IsPublishedGlobally = WMIObject.Properties[nameof (IsPublishedGlobally)].Value as bool?;
      this.IsPublishedToUser = WMIObject.Properties[nameof (IsPublishedToUser)].Value as bool?;
      this.Name = WMIObject.Properties[nameof (Name)].Value as string;
      this.PackageId = WMIObject.Properties[nameof (PackageId)].Value as string;
      this.PackageSize = WMIObject.Properties[nameof (PackageSize)].Value as ulong?;
      this.Path = WMIObject.Properties[nameof (Path)].Value as string;
      this.PercentLoaded = WMIObject.Properties[nameof (PercentLoaded)].Value as ushort?;
      this.UserConfigurationData = WMIObject.Properties[nameof (UserConfigurationData)].Value as string;
      this.UserPending = WMIObject.Properties[nameof (UserPending)].Value as bool?;
      this.Version = WMIObject.Properties[nameof (Version)].Value as string;
      this.VersionId = WMIObject.Properties[nameof (VersionId)].Value as string;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    public string[] Assets { get; set; }

    public string DeploymentMachineData { get; set; }

    public string DeploymentUserData { get; set; }

    public bool? GlobalPending { get; set; }

    public bool? HasAssetIntelligence { get; set; }

    public bool? InUse { get; set; }

    public bool? IsPublishedGlobally { get; set; }

    public bool? IsPublishedToUser { get; set; }

    public string Name { get; set; }

    public string PackageId { get; set; }

    public ulong? PackageSize { get; set; }

    public string Path { get; set; }

    public ushort? PercentLoaded { get; set; }

    public string UserConfigurationData { get; set; }

    public bool? UserPending { get; set; }

    public string Version { get; set; }

    public string VersionId { get; set; }
  }

  /// <summary>Source:ROOT\Appv</summary>
  public class AppvPublishingServer
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.appv5.AppvPublishingServer" /> class.
    /// </summary>
    /// <param name="WMIObject">A WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public AppvPublishingServer(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.GlobalRefreshEnabled = WMIObject.Properties[nameof (GlobalRefreshEnabled)].Value as bool?;
      this.GlobalRefreshInterval = WMIObject.Properties[nameof (GlobalRefreshInterval)].Value as uint?;
      this.GlobalRefreshIntervalUnit = WMIObject.Properties[nameof (GlobalRefreshIntervalUnit)].Value as string;
      this.GlobalRefreshOnLogon = WMIObject.Properties[nameof (GlobalRefreshOnLogon)].Value as bool?;
      this.ID = WMIObject.Properties[nameof (ID)].Value as uint?;
      this.SetByGroupPolicy = WMIObject.Properties[nameof (SetByGroupPolicy)].Value as bool?;
      this.Url = WMIObject.Properties[nameof (Url)].Value as string;
      this.UserRefreshEnabled = WMIObject.Properties[nameof (UserRefreshEnabled)].Value as bool?;
      this.UserRefreshInterval = WMIObject.Properties[nameof (UserRefreshInterval)].Value as uint?;
      this.UserRefreshIntervalUnit = WMIObject.Properties[nameof (UserRefreshIntervalUnit)].Value as string;
      this.UserRefreshOnLogon = WMIObject.Properties[nameof (UserRefreshOnLogon)].Value as bool?;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    public bool? GlobalRefreshEnabled { get; set; }

    public uint? GlobalRefreshInterval { get; set; }

    public string GlobalRefreshIntervalUnit { get; set; }

    public bool? GlobalRefreshOnLogon { get; set; }

    public uint? ID { get; set; }

    public bool? SetByGroupPolicy { get; set; }

    public string Url { get; set; }

    public bool? UserRefreshEnabled { get; set; }

    public uint? UserRefreshInterval { get; set; }

    public string UserRefreshIntervalUnit { get; set; }

    public bool? UserRefreshOnLogon { get; set; }
  }
}
