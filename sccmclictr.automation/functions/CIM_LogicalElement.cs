// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.functions.CIM_LogicalElement
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

#nullable disable
namespace sccmclictr.automation.functions;

/// <summary>Source:ROOT\CIMV2</summary>
public class CIM_LogicalElement : CIM_ManagedSystemElement
{
  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.CIM_LogicalElement" /> class.
  /// </summary>
  /// <param name="WMIObject">The WMI object.</param>
  /// <param name="RemoteRunspace">The remote runspace.</param>
  /// <param name="PSCode">The PowerShell code.</param>
  public CIM_LogicalElement(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    : base(WMIObject, RemoteRunspace, PSCode)
  {
    this.remoteRunspace = RemoteRunspace;
    this.pSCode = PSCode;
    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
    this.__INSTANCE = true;
    this.WMIObject = WMIObject;
  }
}
