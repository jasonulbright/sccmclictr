// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.functions.Win32_Process
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

#nullable disable
namespace sccmclictr.automation.functions;

/// <summary>Source:ROOT\cimv2</summary>
public class Win32_Process : CIM_Process
{
  internal baseInit oNewBase;

  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.CIM_Process" /> class.
  /// </summary>
  /// <param name="WMIObject">The WMI object.</param>
  /// <param name="RemoteRunspace">The remote runspace.</param>
  /// <param name="PSCode">The PowerShell code.</param>
  public Win32_Process(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    : base(WMIObject, RemoteRunspace, PSCode)
  {
    this.remoteRunspace = RemoteRunspace;
    this.pSCode = PSCode;
    this.oNewBase = new baseInit(this.remoteRunspace, this.pSCode);
    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
    this.__INSTANCE = true;
    this.WMIObject = WMIObject;
    this.CommandLine = WMIObject.Properties[nameof (CommandLine)].Value as string;
    this.ExecutablePath = WMIObject.Properties[nameof (ExecutablePath)].Value as string;
    this.HandleCount = WMIObject.Properties[nameof (HandleCount)].Value as uint?;
    this.MaximumWorkingSetSize = WMIObject.Properties[nameof (MaximumWorkingSetSize)].Value as uint?;
    this.MinimumWorkingSetSize = WMIObject.Properties[nameof (MinimumWorkingSetSize)].Value as uint?;
    this.OtherOperationCount = WMIObject.Properties[nameof (OtherOperationCount)].Value as ulong?;
    this.OtherTransferCount = WMIObject.Properties[nameof (OtherTransferCount)].Value as ulong?;
    this.PageFaults = WMIObject.Properties[nameof (PageFaults)].Value as uint?;
    this.PageFileUsage = WMIObject.Properties[nameof (PageFileUsage)].Value as uint?;
    this.ParentProcessId = WMIObject.Properties[nameof (ParentProcessId)].Value as uint?;
    this.PeakPageFileUsage = WMIObject.Properties[nameof (PeakPageFileUsage)].Value as uint?;
    this.PeakVirtualSize = WMIObject.Properties[nameof (PeakVirtualSize)].Value as ulong?;
    this.PeakWorkingSetSize = WMIObject.Properties[nameof (PeakWorkingSetSize)].Value as uint?;
    this.PrivatePageCount = WMIObject.Properties[nameof (PrivatePageCount)].Value as ulong?;
    this.ProcessId = WMIObject.Properties[nameof (ProcessId)].Value as uint?;
    this.QuotaNonPagedPoolUsage = WMIObject.Properties[nameof (QuotaNonPagedPoolUsage)].Value as uint?;
    this.QuotaPagedPoolUsage = WMIObject.Properties[nameof (QuotaPagedPoolUsage)].Value as uint?;
    this.QuotaPeakNonPagedPoolUsage = WMIObject.Properties[nameof (QuotaPeakNonPagedPoolUsage)].Value as uint?;
    this.QuotaPeakPagedPoolUsage = WMIObject.Properties[nameof (QuotaPeakPagedPoolUsage)].Value as uint?;
    this.ReadOperationCount = WMIObject.Properties[nameof (ReadOperationCount)].Value as ulong?;
    this.ReadTransferCount = WMIObject.Properties[nameof (ReadTransferCount)].Value as ulong?;
    this.SessionId = WMIObject.Properties[nameof (SessionId)].Value as uint?;
    this.ThreadCount = WMIObject.Properties[nameof (ThreadCount)].Value as uint?;
    this.VirtualSize = WMIObject.Properties[nameof (VirtualSize)].Value as ulong?;
    this.WindowsVersion = WMIObject.Properties[nameof (WindowsVersion)].Value as string;
    this.WriteOperationCount = WMIObject.Properties[nameof (WriteOperationCount)].Value as ulong?;
    this.WriteTransferCount = WMIObject.Properties[nameof (WriteTransferCount)].Value as ulong?;
  }

  /// <summary>Gets or sets the command line.</summary>
  /// <value>Command line used to start a specific process, if applicable.</value>
  public string CommandLine { get; set; }

  /// <summary>Gets or sets the executable path.</summary>
  /// <value>Path to the executable file of the process.</value>
  public string ExecutablePath { get; set; }

  /// <summary>Gets or sets the handle count.</summary>
  /// <value>Total number of open handles owned by the process. HandleCount is the sum of the handles currently open by each thread in this process.</value>
  public uint? HandleCount { get; set; }

  /// <summary>Gets or sets the maximum size of the working set.</summary>
  /// <value>Maximum working set size of the process. The working set of a process is the set of memory pages visible to the process in physical RAM. These pages are resident, and available for an application to use without triggering a page fault.</value>
  public uint? MaximumWorkingSetSize { get; set; }

  /// <summary>Gets or sets the minimum size of the working set.</summary>
  /// <value>Minimum working set size of the process. The working set of a process is the set of memory pages visible to the process in physical RAM. These pages are resident and available for an application to use without triggering a page fault.</value>
  public uint? MinimumWorkingSetSize { get; set; }

  /// <summary>Gets or sets the other operation count.</summary>
  /// <value>Number of I/O operations performed that are not read or write operations.</value>
  public ulong? OtherOperationCount { get; set; }

  /// <summary>Gets or sets the other transfer count.</summary>
  /// <value>Amount of data transferred during operations that are not read or write operations.</value>
  public ulong? OtherTransferCount { get; set; }

  /// <summary>Gets or sets the page faults.</summary>
  /// <value>Number of page faults that a process generates.</value>
  public uint? PageFaults { get; set; }

  /// <summary>Gets or sets the page file usage.</summary>
  /// <value>Maximum amount of page file space used during the life of a process.</value>
  public uint? PageFileUsage { get; set; }

  /// <summary>Gets or sets the parent process identifier.</summary>
  /// <value>Unique identifier of the process that creates a process.</value>
  public uint? ParentProcessId { get; set; }

  /// <summary>Gets or sets the peak page file usage.</summary>
  /// <value>Maximum amount of page file space used during the life of a process.</value>
  public uint? PeakPageFileUsage { get; set; }

  /// <summary>Gets or sets the size of the peak virtual.</summary>
  /// <value>Maximum virtual address space a process uses at any one time.</value>
  public ulong? PeakVirtualSize { get; set; }

  /// <summary>Gets or sets the size of the peak working set.</summary>
  /// <value>Peak working set size of a process in Kilobytes</value>
  public uint? PeakWorkingSetSize { get; set; }

  /// <summary>Gets or sets the private page count.</summary>
  /// <value>Current number of pages allocated that are only accessible to the process represented by this Win32_Process instance</value>
  public ulong? PrivatePageCount { get; set; }

  /// <summary>Gets or sets the process identifier.</summary>
  /// <value>Global process identifier that is used to identify a process. The value is valid from the time a process is created until it is terminated.</value>
  public uint? ProcessId { get; set; }

  /// <summary>Gets or sets the quota non paged pool usage.</summary>
  /// <value>Quota amount of nonpaged pool usage for a process.</value>
  public uint? QuotaNonPagedPoolUsage { get; set; }

  /// <summary>Gets or sets the quota paged pool usage.</summary>
  /// <value>Quota amount of paged pool usage for a process.</value>
  public uint? QuotaPagedPoolUsage { get; set; }

  /// <summary>Gets or sets the quota peak non paged pool usage.</summary>
  /// <value>Peak quota amount of nonpaged pool usage for a process.</value>
  public uint? QuotaPeakNonPagedPoolUsage { get; set; }

  /// <summary>Gets or sets the quota peak paged pool usage.</summary>
  /// <value>Peak quota amount of paged pool usage for a process.</value>
  public uint? QuotaPeakPagedPoolUsage { get; set; }

  /// <summary>Gets or sets the read operation count.</summary>
  /// <value>Number of read operations performed.</value>
  public ulong? ReadOperationCount { get; set; }

  /// <summary>Gets or sets the read transfer count.</summary>
  /// <value>Amount of data read.</value>
  public ulong? ReadTransferCount { get; set; }

  /// <summary>Gets or sets the session identifier.</summary>
  /// <value>Unique identifier that an operating system generates when a session is created. A session spans a period of time from logon until logoff from a specific system.</value>
  public uint? SessionId { get; set; }

  /// <summary>Gets or sets the thread count.</summary>
  /// <value>Number of active threads in a process. An instruction is the basic unit of execution in a processor, and a thread is the object that executes an instruction. Each running process has at least one thread. </value>
  public uint? ThreadCount { get; set; }

  /// <summary>Gets or sets the size of the virtual.</summary>
  /// <value>Current size of the virtual address space that a process is using, not the physical or virtual memory actually used by the process.</value>
  public ulong? VirtualSize { get; set; }

  /// <summary>Gets or sets the windows version.</summary>
  /// <value>Version of Windows in which the process is running.</value>
  public string WindowsVersion { get; set; }

  /// <summary>Gets or sets the write operation count.</summary>
  /// <value>Number of write operations performed.</value>
  public ulong? WriteOperationCount { get; set; }

  /// <summary>Gets or sets the write transfer count.</summary>
  /// <value>Amount of data written.</value>
  public ulong? WriteTransferCount { get; set; }

  /// <summary>Terminates a process and all of its threads.</summary>
  /// <returns>UInt32.</returns>
  public uint Terminate()
  {
    string hash = this.oNewBase.CreateHash($"Get-Process | Where {{ $_.Id -Eq {this.ProcessId.ToString()} }} | Kill -Force");
    this.oNewBase.Cache.Remove(hash, (string) null);
    this.oNewBase.GetStringFromPS($"Get-Process | Where {{ $_.Id -Eq '{this.ProcessId.ToString()}' }} | Kill -Force");
    this.oNewBase.Cache.Remove(hash, (string) null);
    return 0;
  }
}
