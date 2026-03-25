// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.functions.services
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

/// <summary>Template for an empty Class</summary>
public class services : baseInit
{
  internal Runspace remoteRunspace;
  internal TraceSource pSCode;
  internal ccm baseClient;

  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.services" /> class.
  /// </summary>
  /// <param name="RemoteRunspace">The remote runspace.</param>
  /// <param name="PSCode">The PowerShell code.</param>
  /// <param name="oClient">A CCM client object.</param>
  public services(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
    : base(RemoteRunspace, PSCode)
  {
    this.remoteRunspace = RemoteRunspace;
    this.pSCode = PSCode;
    this.baseClient = oClient;
  }

  internal List<Win32_Service> win32_Services
  {
    get
    {
      List<Win32_Service> win32Services = new List<Win32_Service>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\cimv2", "SELECT * FROM Win32_Service", false))
      {
        Win32_Service win32Service = new Win32_Service(WMIObject, this.remoteRunspace, this.pSCode);
        win32Service.remoteRunspace = this.remoteRunspace;
        win32Service.pSCode = this.pSCode;
        win32Services.Add(win32Service);
      }
      return win32Services;
    }
    set
    {
    }
  }

  /// <summary>Get a List of all Services</summary>
  public List<Win32_Service> Win32_Services => this.win32_Services;

  /// <summary>Reloads this instance.</summary>
  public void Reload()
  {
    List<Win32_Service> win32ServiceList = new List<Win32_Service>();
    foreach (PSObject WMIObject in this.GetObjects("ROOT\\cimv2", "SELECT * FROM Win32_Service", true))
    {
      Win32_Service win32Service = new Win32_Service(WMIObject, this.remoteRunspace, this.pSCode);
      win32Service.remoteRunspace = this.remoteRunspace;
      win32Service.pSCode = this.pSCode;
      win32ServiceList.Add(win32Service);
    }
    this.win32_Services = win32ServiceList;
  }

  /// <summary>Get a single Service Instance</summary>
  /// <param name="ServiceName">Name of the Service</param>
  /// <param name="Reload">true = do not get results from cache</param>
  /// <returns></returns>
  public Win32_Service GetService(string ServiceName, bool Reload = true)
  {
    this.Cache.Remove(this.CreateHash("ROOT\\cimv2" + $"SELECT * FROM Win32_Service WHERE Name ='{ServiceName}'"), (string) null);
    using (List<PSObject>.Enumerator enumerator = this.GetObjects("ROOT\\cimv2", $"SELECT * FROM Win32_Service WHERE Name ='{ServiceName}'", Reload).GetEnumerator())
    {
      if (enumerator.MoveNext())
      {
        Win32_Service service = new Win32_Service(enumerator.Current, this.remoteRunspace, this.pSCode);
        service.remoteRunspace = this.remoteRunspace;
        service.pSCode = this.pSCode;
        return service;
      }
    }
    return (Win32_Service) null;
  }
}
