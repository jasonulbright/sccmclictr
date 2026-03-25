// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.functions.CIM_Service
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
public class CIM_Service : CIM_LogicalElement
{
  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.CIM_Service" /> class.
  /// </summary>
  /// <param name="WMIObject">The WMI object.</param>
  /// <param name="RemoteRunspace">The remote runspace.</param>
  /// <param name="PSCode">The ps code.</param>
  public CIM_Service(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    : base(WMIObject, RemoteRunspace, PSCode)
  {
    this.remoteRunspace = RemoteRunspace;
    this.pSCode = PSCode;
    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
    this.__INSTANCE = true;
    this.WMIObject = WMIObject;
    this.CreationClassName = WMIObject.Properties[nameof (CreationClassName)].Value as string;
    this.Started = WMIObject.Properties[nameof (Started)].Value as bool?;
    this.StartMode = WMIObject.Properties[nameof (StartMode)].Value as string;
    this.SystemCreationClassName = WMIObject.Properties[nameof (SystemCreationClassName)].Value as string;
    this.SystemName = WMIObject.Properties[nameof (SystemName)].Value as string;
  }

  public string CreationClassName { get; set; }

  public bool? Started { get; set; }

  public string StartMode { get; set; }

  public string SystemCreationClassName { get; set; }

  public string SystemName { get; set; }
}
