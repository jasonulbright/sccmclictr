// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.functions._PublicClass
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

#nullable disable
namespace sccmclictr.automation.functions;

/// <summary>Class _PublicClass.</summary>
public class _PublicClass
{
  internal Runspace remoteRunspace;
  internal TraceSource pSCode;

  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions._PublicClass" /> class.
  /// </summary>
  /// <param name="WMIObject">A WMI object.</param>
  /// <param name="RemoteRunspace">The remote runspace.</param>
  /// <param name="PSCode">The PowerShell code.</param>
  public _PublicClass(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
