// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.functions.locationservices
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

public class locationservices : baseInit
{
  internal Runspace remoteRunspace;
  internal TraceSource pSCode;
  internal ccm baseClient;

  public locationservices(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
    : base(RemoteRunspace, PSCode)
  {
    this.remoteRunspace = RemoteRunspace;
    this.pSCode = PSCode;
    this.baseClient = oClient;
  }

  public List<locationservices.BoundaryGroupCache> BoundaryGroupCacheList
  {
    get
    {
      List<locationservices.BoundaryGroupCache> boundaryGroupCacheList = new List<locationservices.BoundaryGroupCache>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\LocationServices", "SELECT * FROM BoundaryGroupCache", true))
        boundaryGroupCacheList.Add(new locationservices.BoundaryGroupCache(WMIObject, this.remoteRunspace, this.pSCode)
        {
          remoteRunspace = this.remoteRunspace,
          pSCode = this.pSCode
        });
      return boundaryGroupCacheList;
    }
  }

  /// <summary>Source:ROOT\ccm\LocationServices</summary>
  public class BoundaryGroupCache
  {
    internal baseInit oNewBase;
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    public BoundaryGroupCache(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.oNewBase = new baseInit(this.remoteRunspace, this.pSCode);
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.BoundaryGroupIDs = WMIObject.Properties[nameof (BoundaryGroupIDs)].Value as string[];
      this.CacheToken = WMIObject.Properties[nameof (CacheToken)].Value as string;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    public string[] BoundaryGroupIDs { get; set; }

    public string CacheToken { get; set; }

    /// <summary>Delete BoundaryGroupCache</summary>
    /// <returns>true = success</returns>
    public bool Delete()
    {
      bool flag = false;
      try
      {
        this.oNewBase.GetStringFromPS($"Get-CimInstance -Namespace \"{this.__NAMESPACE}\" -Query \"SELECT * FROM {this.__RELPATH.Split('.')[0]} WHERE {this.__RELPATH.Substring(this.__RELPATH.IndexOf('.') + 1)}\" | Remove-CimInstance");
        flag = true;
      }
      catch
      {
      }
      return flag;
    }
  }
}
