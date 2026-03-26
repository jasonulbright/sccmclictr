// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.functions.swcache
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using sccmclictr.automation.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

#nullable disable
namespace sccmclictr.automation.functions;

/// <summary>
/// Class to manage SCCM Agent Cache (SW Packages, Updates etc.)
/// </summary>
public class swcache : baseInit
{
  internal Runspace remoteRunspace;
  internal TraceSource pSCode;
  internal ccm baseClient;

  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.swcache" /> class.
  /// </summary>
  /// <param name="RemoteRunspace">The remote runspace.</param>
  /// <param name="PSCode">The PowerShell code.</param>
  /// <param name="oClient">A CCM Client object.</param>
  public swcache(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
    : base(RemoteRunspace, PSCode)
  {
    this.remoteRunspace = RemoteRunspace;
    this.pSCode = PSCode;
    this.baseClient = oClient;
  }

  /// <summary>Gets the content of the cached.</summary>
  /// <value>The content of the cached.</value>
  public List<swcache.CacheInfoEx> CachedContent
  {
    get
    {
      TimeSpan cacheTime = this.cacheTime;
      this.cacheTime = new TimeSpan(0, 0, 30);
      List<swcache.CacheInfoEx> cachedContent = new List<swcache.CacheInfoEx>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\SoftMgmtAgent", "SELECT * FROM CacheInfoEx"))
        cachedContent.Add(new swcache.CacheInfoEx(WMIObject, this.remoteRunspace, this.pSCode)
        {
          remoteRunspace = this.remoteRunspace,
          pSCode = this.pSCode
        });
      this.cacheTime = cacheTime;
      return cachedContent;
    }
  }

  /// <summary>Gets all Directories in the SCCM Agent Cache Folder</summary>
  /// <returns>List{System.String}.</returns>
  public List<string> GetAllCacheDirs()
  {
    List<string> allCacheDirs = new List<string>();
    foreach (PSObject psObject in this.GetObjectsFromPS($"dir '{this.CachePath}' | WHERE {{$_.PsIsContainer}} | select Name"))
      allCacheDirs.Add(psObject.Members["Name"].Value.ToString());
    return allCacheDirs;
  }

  /// <summary>
  /// Get all Package Directories in the SCCM Agent Cache Folder
  /// </summary>
  /// <returns></returns>
  public List<string> GetPkgCacheDirs()
  {
    this.GetStringFromClassMethod("ROOT\\ccm:SMS_Client", "GetAssignedSite()", "sSiteCode");
    List<string> pkgCacheDirs = new List<string>();
    foreach (PSObject psObject in this.GetObjectsFromPS($"dir '{this.CachePath}' | WHERE {{$_.PsIsContainer -and $_.Name[8] -eq '.'}} | select Name"))
      pkgCacheDirs.Add(psObject.Members["Name"].Value.ToString());
    return pkgCacheDirs;
  }

  /// <summary>
  /// Gets or sets the path where SCCM Client stores software packages and updates.
  /// </summary>
  /// <value>The cache path.</value>
  public string CachePath
  {
    get => this.GetProperty("ROOT\\ccm\\SoftMgmtAgent:CacheConfig.ConfigKey='Cache'", "Location");
    set
    {
      this.SetProperty("ROOT\\ccm\\SoftMgmtAgent:CacheConfig.ConfigKey='Cache'", "Location", $"'{value}'");
    }
  }

  /// <summary>Gets or sets the size of the cache.</summary>
  /// <value>The size of the cache.</value>
  public uint? CacheSize
  {
    get
    {
      string property = this.GetProperty("ROOT\\ccm\\SoftMgmtAgent:CacheConfig.ConfigKey='Cache'", "Size");
      return string.IsNullOrEmpty(property) ? new uint?() : new uint?(uint.Parse(property));
    }
    set
    {
      this.SetProperty("ROOT\\ccm\\SoftMgmtAgent:CacheConfig.ConfigKey='Cache'", "Size", value.ToString());
    }
  }

  /// <summary>Gets the inUse property.</summary>
  /// <value>The in use.</value>
  public bool? InUse
  {
    get
    {
      string property = this.GetProperty("ROOT\\ccm\\SoftMgmtAgent:CacheConfig.ConfigKey='Cache'", nameof (InUse));
      return string.IsNullOrEmpty(property) ? new bool?() : new bool?(bool.Parse(property));
    }
  }

  /// <summary>
  /// Cleanups the orphaned cache items  (Clean from WMI and Disk).
  /// </summary>
  public string CleanupOrphanedCacheItems()
  {
    try
    {
      return this.GetStringFromPS(Resources.CacheCleanup, true);
    }
    catch (Exception ex)
    {
      return ex.Message;
    }
  }

  /// <summary>Class CacheInfoEx.</summary>
  public class CacheInfoEx
  {
    internal baseInit oNewBase;
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.swcache.CacheInfoEx" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CacheInfoEx(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.oNewBase = new baseInit(this.remoteRunspace, this.pSCode);
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      try
      {
        this.ContentFlags = WMIObject.Properties[nameof (PeerCaching)].Value as uint?;
      }
      catch
      {
        this.ContentFlags = new uint?();
      }
      this.CacheId = WMIObject.Properties[nameof (CacheId)].Value as string;
      this.ContentId = WMIObject.Properties[nameof (ContentId)].Value as string;
      try
      {
        this.ContentSize = WMIObject.Properties[nameof (ContentSize)].Value as uint?;
      }
      catch
      {
        this.ContentSize = new uint?();
      }
      this.ContentType = WMIObject.Properties[nameof (ContentType)].Value as string;
      this.ContentVer = WMIObject.Properties[nameof (ContentVer)].Value as string;
      try
      {
        this.ExcludeFileList = WMIObject.Properties[nameof (ExcludeFileList)].Value as string;
      }
      catch
      {
        this.ExcludeFileList = "";
      }
      string dmtfDate = WMIObject.Properties[nameof (LastReferenced)].Value as string;
      this.LastReferenced = !string.IsNullOrEmpty(dmtfDate) ? new DateTime?(common.DmtfToDateTime(dmtfDate)) : new DateTime?();
      this.Location = WMIObject.Properties[nameof (Location)].Value as string;
      try
      {
        this.PeerCaching = WMIObject.Properties[nameof (PeerCaching)].Value as bool?;
      }
      catch
      {
        this.PeerCaching = new bool?(false);
      }
      try
      {
        this.PersistInCache = WMIObject.Properties[nameof (PersistInCache)].Value as uint?;
      }
      catch
      {
        this.PersistInCache = new uint?();
      }
      try
      {
        this.ReferenceCount = WMIObject.Properties[nameof (ReferenceCount)].Value as uint?;
      }
      catch
      {
        this.ReferenceCount = new uint?();
      }
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    /// <summary>? TBD</summary>
    public uint? ContentFlags { get; set; }

    /// <summary>Gets or sets the cache identifier.</summary>
    /// <value>The cache identifier.</value>
    public string CacheId { get; set; }

    /// <summary>Gets or sets the content identifier.</summary>
    /// <value>The content identifier.</value>
    public string ContentId { get; set; }

    /// <summary>Gets or sets the size of the content.</summary>
    /// <value>The size of the content.</value>
    public uint? ContentSize { get; set; }

    /// <summary>Gets or sets the type of the content.</summary>
    /// <value>The type of the content.</value>
    public string ContentType { get; set; }

    /// <summary>Gets or sets the content ver.</summary>
    /// <value>The content ver.</value>
    public string ContentVer { get; set; }

    /// <summary>? TBD</summary>
    public string ExcludeFileList { get; set; }

    /// <summary>Gets or sets the last referenced.</summary>
    /// <value>The last referenced.</value>
    public DateTime? LastReferenced { get; set; }

    /// <summary>Gets or sets the location.</summary>
    /// <value>The location.</value>
    public string Location { get; set; }

    /// <summary>Gets or sets the persist in cache.</summary>
    /// <value>The persist in cache.</value>
    public uint? PersistInCache { get; set; }

    /// <summary>PeerCaching enabled</summary>
    public bool? PeerCaching { get; set; }

    /// <summary>Gets or sets the reference count.</summary>
    /// <value>The reference count.</value>
    public uint? ReferenceCount { get; set; }

    /// <summary>Check if the Folders exists.</summary>
    /// <returns>Boolean.</returns>
    public bool FolderExists()
    {
      string stringFromPs = this.oNewBase.GetStringFromPS($"Test-Path \"{this.Location}\"");
      return !string.IsNullOrEmpty(stringFromPs) && bool.Parse(stringFromPs);
    }

    /// <summary>Deletes the cached items from the disk.</summary>
    public void DeleteFolder()
    {
      if (this.Location.Length <= 3)
        return;
      this.oNewBase.GetStringFromPS($"Remove-Item \"{this.Location}\" -recurse");
    }

    /// <summary>Deletes the cached items from  the database (WMI).</summary>
    public void DeleteFromDatabase()
    {
      this.oNewBase.GetStringFromPS($"Get-CimInstance -Namespace \"{this.__NAMESPACE}\" -Query \"SELECT * FROM {this.__RELPATH.Split('.')[0]} WHERE {this.__RELPATH.Substring(this.__RELPATH.IndexOf('.') + 1)}\" | Remove-CimInstance");
    }

    /// <summary>
    /// Delete Cached Item from the Database (WMI) and from the Disk
    /// </summary>
    public void Delete()
    {
      this.DeleteFolder();
      this.DeleteFromDatabase();
    }
  }
}
