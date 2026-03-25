// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.functions.ExtProcess
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

#nullable disable
namespace sccmclictr.automation.functions;

/// <summary>Extended Win32_Process with Owner attribute</summary>
public class ExtProcess : Win32_Process
{
  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.ExtProcess" /> class.
  /// </summary>
  /// <param name="WMIObject">The WMI object.</param>
  /// <param name="RemoteRunspace">The remote runspace.</param>
  /// <param name="PSCode">The PowerShell code.</param>
  public ExtProcess(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    : base(WMIObject, RemoteRunspace, PSCode)
  {
    this.remoteRunspace = RemoteRunspace;
    this.pSCode = PSCode;
    this.Owner = WMIObject.Properties[nameof (Owner)].Value as string;
  }

  /// <summary>Gets or sets the process owner.</summary>
  /// <value>The process owner.</value>
  public string Owner { get; set; }
}
