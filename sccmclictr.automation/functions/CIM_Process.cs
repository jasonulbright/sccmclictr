// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.functions.CIM_Process
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using System;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

#nullable disable
namespace sccmclictr.automation.functions;

/// <summary>Source:ROOT\cimv2</summary>
public class CIM_Process : CIM_LogicalElement
{
  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.CIM_Process" /> class.
  /// </summary>
  /// <param name="WMIObject">The WMI object.</param>
  /// <param name="RemoteRunspace">The remote runspace.</param>
  /// <param name="PSCode">The PowerShell code.</param>
  public CIM_Process(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
    string dmtfDate1 = WMIObject.Properties[nameof (CreationDate)].Value as string;
    this.CreationDate = !string.IsNullOrEmpty(dmtfDate1) ? new DateTime?(common.DmtfToDateTime(dmtfDate1)) : new DateTime?();
    this.CSCreationClassName = WMIObject.Properties[nameof (CSCreationClassName)].Value as string;
    this.CSName = WMIObject.Properties[nameof (CSName)].Value as string;
    this.ExecutionState = WMIObject.Properties[nameof (ExecutionState)].Value as ushort?;
    this.Handle = WMIObject.Properties[nameof (Handle)].Value as string;
    this.KernelModeTime = WMIObject.Properties[nameof (KernelModeTime)].Value as ulong?;
    this.OSCreationClassName = WMIObject.Properties[nameof (OSCreationClassName)].Value as string;
    this.OSName = WMIObject.Properties[nameof (OSName)].Value as string;
    this.Priority = WMIObject.Properties[nameof (Priority)].Value as uint?;
    string dmtfDate2 = WMIObject.Properties[nameof (TerminationDate)].Value as string;
    this.TerminationDate = !string.IsNullOrEmpty(dmtfDate2) ? new DateTime?(common.DmtfToDateTime(dmtfDate2)) : new DateTime?();
    this.UserModeTime = WMIObject.Properties[nameof (UserModeTime)].Value as ulong?;
    this.WorkingSetSize = WMIObject.Properties[nameof (WorkingSetSize)].Value as ulong?;
  }

  /// <summary>Gets or sets the name of the creation class.</summary>
  /// <value>Name of the first concrete class in the inheritance chain that is used to create an instance.</value>
  public string CreationClassName { get; set; }

  /// <summary>Gets or sets the creation date.</summary>
  /// <value>Date the process begins executing.</value>
  public DateTime? CreationDate { get; set; }

  /// <summary>Gets or sets the name of the cs creation class.</summary>
  /// <value>Creation class name of the scoping computer system.</value>
  public string CSCreationClassName { get; set; }

  /// <summary>Gets or sets the name of the cs.</summary>
  /// <value>Name of the scoping computer system.</value>
  public string CSName { get; set; }

  /// <summary>Gets or sets the state of the execution.</summary>
  /// <value>This property is not implemented and does not get populated for any instance of this class. This property is always NULL.</value>
  public ushort? ExecutionState { get; set; }

  /// <summary>Gets or sets the handle.</summary>
  /// <value>Process identifier.</value>
  public string Handle { get; set; }

  /// <summary>Gets or sets the kernel mode time.</summary>
  /// <value>Time in kernel mode, in 100 nanosecond units. If this information is not available, use a value of 0 (zero).</value>
  public ulong? KernelModeTime { get; set; }

  /// <summary>Gets or sets the name of the os creation class.</summary>
  /// <value>Creation class name of the scoping operating system.</value>
  public string OSCreationClassName { get; set; }

  /// <summary>Gets or sets the name of the os.</summary>
  /// <value>Name of the scoping operating system.</value>
  public string OSName { get; set; }

  /// <summary>Gets or sets the priority.</summary>
  /// <value>Scheduling priority of a process within an operating system. The higher the value, the higher priority a process receives. Priority values can range from 0 (zero), which is the lowest priority to 31, which is highest priority.</value>
  public uint? Priority { get; set; }

  /// <summary>Gets or sets the termination date.</summary>
  /// <value>Process was stopped or terminated. To get the termination time, a handle to the process must be held open. Otherwise, this property returns NULL.</value>
  public DateTime? TerminationDate { get; set; }

  /// <summary>Gets or sets the user mode time.</summary>
  /// <value>Time in user mode, in 100 nanosecond units. If this information is not available, use a value of 0 (zero).</value>
  public ulong? UserModeTime { get; set; }

  /// <summary>Gets or sets the size of the working set.</summary>
  /// <value>Amount of memory in bytes that a process needs to execute efficiently—for an operating system that uses page-based memory management.</value>
  public ulong? WorkingSetSize { get; set; }
}
