// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.functions.appv4
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

#nullable disable
namespace sccmclictr.automation.functions;

/// <summary>
/// App-V 4.x Classes
/// Thanks to Mattias Benninge to provide the details
/// </summary>
public class appv4 : baseInit
{
  internal Runspace remoteRunspace;
  internal TraceSource pSCode;
  internal ccm baseClient;

  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.appv4" /> class.
  /// </summary>
  /// <param name="RemoteRunspace">The remote runspace.</param>
  /// <param name="PSCode">The PowerShell code.</param>
  /// <param name="oClient">A CCM client object</param>
  public appv4(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
    : base(RemoteRunspace, PSCode)
  {
    this.remoteRunspace = RemoteRunspace;
    this.pSCode = PSCode;
    this.baseClient = oClient;
  }

  /// <summary>Gets the appv4 applications.</summary>
  /// <value>The appv4 applications.</value>
  public List<appv4.Application> Appv4Applications
  {
    get => this.Appv4ApplicationsList(false, new TimeSpan(0, 0, 30));
  }

  /// <summary>Gets the appv4 applications.</summary>
  /// <param name="Reload">true = do not load from cache</param>
  /// <param name="TTL">Time to keep in cache</param>
  /// <returns>List of App-V 4 Applications</returns>
  public List<appv4.Application> Appv4ApplicationsList(bool Reload, TimeSpan TTL)
  {
    List<appv4.Application> applicationList = new List<appv4.Application>();
    foreach (PSObject WMIObject in this.GetObjects("ROOT\\microsoft\\appvirt\\client", "SELECT * FROM Application", Reload, TTL))
      applicationList.Add(new appv4.Application(WMIObject, this.remoteRunspace, this.pSCode)
      {
        remoteRunspace = this.remoteRunspace,
        pSCode = this.pSCode
      });
    return applicationList;
  }

  /// <summary>Gets the appv4 packages.</summary>
  /// <value>The appv4 packages.</value>
  public List<appv4.Package> Appv4Packages => this.Appv4PackagesList(false, new TimeSpan(0, 0, 30));

  /// <summary>Gets the appv4 packages.</summary>
  /// <param name="Reload">True = do not use cached results</param>
  /// <param name="TTL">Time how log the objects are cached</param>
  /// <returns></returns>
  public List<appv4.Package> Appv4PackagesList(bool Reload, TimeSpan TTL)
  {
    List<appv4.Package> packageList = new List<appv4.Package>();
    foreach (PSObject WMIObject in this.GetObjects("ROOT\\microsoft\\appvirt\\client", "SELECT * FROM Package", Reload, TTL))
      packageList.Add(new appv4.Package(WMIObject, this.remoteRunspace, this.pSCode)
      {
        remoteRunspace = this.remoteRunspace,
        pSCode = this.pSCode
      });
    return packageList;
  }

  /// <summary>Source:ROOT\microsoft\appvirt\client</summary>
  public class Application
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.appv4.Application" /> class.
    /// </summary>
    /// <param name="WMIObject">A WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public Application(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.CachedOsdPath = WMIObject.Properties[nameof (CachedOsdPath)].Value as string;
      this.GlobalRunningCount = WMIObject.Properties[nameof (GlobalRunningCount)].Value as uint?;
      string dmtfDate = WMIObject.Properties[nameof (LastLaunchOnSystem)].Value as string;
      this.LastLaunchOnSystem = !string.IsNullOrEmpty(dmtfDate) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(dmtfDate)) : new DateTime?();
      this.Loading = WMIObject.Properties[nameof (Loading)].Value as bool?;
      this.Name = WMIObject.Properties[nameof (Name)].Value as string;
      this.OriginalOsdPath = WMIObject.Properties[nameof (OriginalOsdPath)].Value as string;
      this.PackageGUID = WMIObject.Properties[nameof (PackageGUID)].Value as string;
      this.Version = WMIObject.Properties[nameof (Version)].Value as string;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    public string CachedOsdPath { get; set; }

    public uint? GlobalRunningCount { get; set; }

    public DateTime? LastLaunchOnSystem { get; set; }

    public bool? Loading { get; set; }

    public string Name { get; set; }

    public string OriginalOsdPath { get; set; }

    public string PackageGUID { get; set; }

    public string Version { get; set; }
  }

  /// <summary>Source:ROOT\microsoft\appvirt\client</summary>
  public class Package
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.appv4.Package" /> class.
    /// </summary>
    /// <param name="WMIObject">A WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public Package(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.CachedLaunchSize = WMIObject.Properties[nameof (CachedLaunchSize)].Value as ulong?;
      this.CachedPercentage = WMIObject.Properties[nameof (CachedPercentage)].Value as ushort?;
      this.CachedSize = WMIObject.Properties[nameof (CachedSize)].Value as ulong?;
      this.InUse = WMIObject.Properties[nameof (InUse)].Value as bool?;
      this.LaunchSize = WMIObject.Properties[nameof (LaunchSize)].Value as ulong?;
      this.Locked = WMIObject.Properties[nameof (Locked)].Value as bool?;
      this.Name = WMIObject.Properties[nameof (Name)].Value as string;
      this.PackageGUID = WMIObject.Properties[nameof (PackageGUID)].Value as string;
      this.SftPath = WMIObject.Properties[nameof (SftPath)].Value as string;
      this.TotalSize = WMIObject.Properties[nameof (TotalSize)].Value as ulong?;
      this.Version = WMIObject.Properties[nameof (Version)].Value as string;
      this.VersionGUID = WMIObject.Properties[nameof (VersionGUID)].Value as string;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    public ulong? CachedLaunchSize { get; set; }

    public ushort? CachedPercentage { get; set; }

    public ulong? CachedSize { get; set; }

    public bool? InUse { get; set; }

    public ulong? LaunchSize { get; set; }

    public bool? Locked { get; set; }

    public string Name { get; set; }

    public string PackageGUID { get; set; }

    public string SftPath { get; set; }

    public ulong? TotalSize { get; set; }

    public string Version { get; set; }

    public string VersionGUID { get; set; }
  }
}
