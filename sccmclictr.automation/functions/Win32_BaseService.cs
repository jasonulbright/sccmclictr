// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.functions.Win32_BaseService
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
public class Win32_BaseService : CIM_Service
{
  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.Win32_BaseService" /> class.
  /// </summary>
  /// <param name="WMIObject">The WMI object.</param>
  /// <param name="RemoteRunspace">The remote runspace.</param>
  /// <param name="PSCode">The PowerShell code.</param>
  public Win32_BaseService(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    : base(WMIObject, RemoteRunspace, PSCode)
  {
    this.remoteRunspace = RemoteRunspace;
    this.pSCode = PSCode;
    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
    this.__INSTANCE = true;
    this.WMIObject = WMIObject;
    this.AcceptPause = WMIObject.Properties[nameof (AcceptPause)].Value as bool?;
    this.AcceptStop = WMIObject.Properties[nameof (AcceptStop)].Value as bool?;
    this.DesktopInteract = WMIObject.Properties[nameof (DesktopInteract)].Value as bool?;
    this.DisplayName = WMIObject.Properties[nameof (DisplayName)].Value as string;
    this.ErrorControl = WMIObject.Properties[nameof (ErrorControl)].Value as string;
    this.ExitCode = WMIObject.Properties[nameof (ExitCode)].Value as uint?;
    this.PathName = WMIObject.Properties[nameof (PathName)].Value as string;
    this.ServiceSpecificExitCode = WMIObject.Properties[nameof (ServiceSpecificExitCode)].Value as uint?;
    this.ServiceType = WMIObject.Properties[nameof (ServiceType)].Value as string;
    this.StartName = WMIObject.Properties[nameof (StartName)].Value as string;
    this.State = WMIObject.Properties[nameof (State)].Value as string;
    this.TagId = WMIObject.Properties[nameof (TagId)].Value as uint?;
  }

  public bool? AcceptPause { get; set; }

  public bool? AcceptStop { get; set; }

  public bool? DesktopInteract { get; set; }

  public string DisplayName { get; set; }

  public string ErrorControl { get; set; }

  public uint? ExitCode { get; set; }

  public string PathName { get; set; }

  public uint? ServiceSpecificExitCode { get; set; }

  public string ServiceType { get; set; }

  public string StartName { get; set; }

  public string State { get; set; }

  public uint? TagId { get; set; }
}
