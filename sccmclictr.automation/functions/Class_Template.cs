// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.functions.Class_Template
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
public class Class_Template : baseInit
{
  internal Runspace remoteRunspace;
  internal TraceSource pSCode;
  internal ccm baseClient;

  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.Class_Template" /> class.
  /// </summary>
  /// <param name="RemoteRunspace">The remote runspace.</param>
  /// <param name="PSCode">The PowerShell code.</param>
  /// <param name="oClient">A ccm client object.</param>
  public Class_Template(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
    : base(RemoteRunspace, PSCode)
  {
    this.remoteRunspace = RemoteRunspace;
    this.pSCode = PSCode;
    this.baseClient = oClient;
  }

  /// <summary>Gets the public list.</summary>
  /// <value>The public list.</value>
  public List<_PublicClass> PublicList
  {
    get
    {
      List<_PublicClass> publicList = new List<_PublicClass>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\SoftMgmtAgent", "SELECT * FROM CacheInfoEx"))
        publicList.Add(new _PublicClass(WMIObject, this.remoteRunspace, this.pSCode)
        {
          remoteRunspace = this.remoteRunspace,
          pSCode = this.pSCode
        });
      return publicList;
    }
  }
}
