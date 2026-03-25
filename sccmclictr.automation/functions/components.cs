// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.functions.components
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

#nullable disable
namespace sccmclictr.automation.functions;

/// <summary>SCCM Agent Components</summary>
public class components : baseInit
{
  internal Runspace remoteRunspace;
  internal TraceSource pSCode;
  internal ccm baseClient;

  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.components" /> class.
  /// </summary>
  /// <param name="RemoteRunspace">The remote runspace.</param>
  /// <param name="PSCode">The PowerShell code.</param>
  /// <param name="oClient">A CCM Client object.</param>
  public components(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
    : base(RemoteRunspace, PSCode)
  {
    this.remoteRunspace = RemoteRunspace;
    this.pSCode = PSCode;
    this.baseClient = oClient;
  }

  /// <summary>List of Installed SCCM Agent Components</summary>
  public List<components.CCM_InstalledComponent> InstalledComponents
  {
    get
    {
      List<components.CCM_InstalledComponent> installedComponents = new List<components.CCM_InstalledComponent>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm", "SELECT * FROM CCM_InstalledComponent"))
        installedComponents.Add(new components.CCM_InstalledComponent(WMIObject, this.remoteRunspace, this.pSCode, this.baseClient)
        {
          remoteRunspace = this.remoteRunspace,
          pSCode = this.pSCode
        });
      return installedComponents;
    }
  }

  /// <summary>Gets a list of component client configuration.</summary>
  /// <value>A list of component client configuration.</value>
  public List<components.CCM_ComponentClientConfig> ComponentClientConfig
  {
    get
    {
      List<components.CCM_ComponentClientConfig> componentClientConfig = new List<components.CCM_ComponentClientConfig>();
      foreach (PSObject WMIObject in this.GetObjects("root\\ccm\\Policy\\Machine\\ActualConfig", "SELECT * FROM CCM_ComponentClientConfig"))
        componentClientConfig.Add(new components.CCM_ComponentClientConfig(WMIObject, this.remoteRunspace, this.pSCode)
        {
          remoteRunspace = this.remoteRunspace,
          pSCode = this.pSCode
        });
      return componentClientConfig;
    }
  }

  /// <summary>Source:ROOT\ccm</summary>
  public class CCM_InstalledComponent
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.components.CCM_InstalledComponent" /> class.
    /// </summary>
    /// <param name="WMIObject">A WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    /// <param name="oClient">A CCM Client object.</param>
    public CCM_InstalledComponent(
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
      this.DisplayName = WMIObject.Properties[nameof (DisplayName)].Value as string;
      this.DisplayNameResourceFile = WMIObject.Properties[nameof (DisplayNameResourceFile)].Value as string;
      this.DisplayNameResourceID = WMIObject.Properties[nameof (DisplayNameResourceID)].Value as uint?;
      this.Name = WMIObject.Properties[nameof (Name)].Value as string;
      this.Version = WMIObject.Properties[nameof (Version)].Value as string;
      try
      {
        PSCode.Switch.Level = SourceLevels.Off;
        this.Enabled = oClient.Components.ComponentClientConfig.First<components.CCM_ComponentClientConfig>((Func<components.CCM_ComponentClientConfig, bool>) (t => t.ComponentName == this.Name)).Enabled;
      }
      catch
      {
        this.Enabled = new bool?(false);
        PSCode.Switch.Level = SourceLevels.All;
      }
      PSCode.Switch.Level = SourceLevels.All;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    public string DisplayName { get; set; }

    public string DisplayNameResourceFile { get; set; }

    public uint? DisplayNameResourceID { get; set; }

    public string Name { get; set; }

    public string Version { get; set; }

    /// <summary>
    /// Get the Enabled Attribute from root\ccm\Policy\Machine\ActualConfig:CCM_InstalledComponent
    /// </summary>
    public bool? Enabled { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_ComponentClientConfig
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.components.CCM_ComponentClientConfig" /> class.
    /// </summary>
    /// <param name="WMIObject">A WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CCM_ComponentClientConfig(
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
      this.ComponentName = WMIObject.Properties[nameof (ComponentName)].Value as string;
      this.Enabled = WMIObject.Properties[nameof (Enabled)].Value as bool?;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    public string ComponentName { get; set; }

    public bool? Enabled { get; set; }
  }
}
