// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.functions.processes
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

#nullable disable
namespace sccmclictr.automation.functions;

/// <summary>Class processes.</summary>
public class processes : baseInit
{
  internal Runspace remoteRunspace;
  internal TraceSource pSCode;
  internal ccm baseClient;

  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.processes" /> class.
  /// </summary>
  /// <param name="RemoteRunspace">The remote runspace.</param>
  /// <param name="PSCode">The PowerShell code.</param>
  /// <param name="oClient">A CCM CLient object.</param>
  public processes(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
    : base(RemoteRunspace, PSCode)
  {
    this.remoteRunspace = RemoteRunspace;
    this.pSCode = PSCode;
    this.baseClient = oClient;
  }

  /// <summary>Gets or sets the win32_ processes.</summary>
  /// <value>The win32_ processes.</value>
  public List<Win32_Process> Win32_Processes
  {
    get
    {
      List<Win32_Process> win32Processes = new List<Win32_Process>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\cimv2", "SELECT * FROM Win32_Process", false))
      {
        Win32_Process win32Process = new Win32_Process(WMIObject, this.remoteRunspace, this.pSCode);
        win32Process.remoteRunspace = this.remoteRunspace;
        win32Process.pSCode = this.pSCode;
        win32Processes.Add(win32Process);
      }
      return win32Processes;
    }
    set
    {
    }
  }

  internal List<ExtProcess> LoadExtProcess(bool Reload)
  {
    List<ExtProcess> extProcessList = new List<ExtProcess>();
    TimeSpan cacheTime = this.cacheTime;
    this.cacheTime = new TimeSpan(0, 0, 10);
    foreach (PSObject WMIObject in this.GetObjectsFromPS("Get-WMIObject win32_Process | Foreach {  $owner = $_.GetOwner();  $_ | Add-Member -MemberType \"Noteproperty\" -name \"Owner\" -value $(\"{0}\\{1}\" -f $owner.Domain, $owner.User) -passthru }", Reload))
    {
      ExtProcess extProcess = new ExtProcess(WMIObject, this.remoteRunspace, this.pSCode);
      extProcess.remoteRunspace = this.remoteRunspace;
      extProcess.pSCode = this.pSCode;
      extProcessList.Add(extProcess);
    }
    this.cacheTime = cacheTime;
    return extProcessList;
  }

  internal List<ExtProcess> extProcess
  {
    get => this.LoadExtProcess(false);
    set
    {
    }
  }

  /// <summary>Extends the processes.</summary>
  /// <param name="Reload">if set to <c>true</c> [reload].</param>
  /// <returns>List{ExtProcess}.</returns>
  public List<ExtProcess> ExtProcesses(bool Reload) => this.LoadExtProcess(Reload);

  /// <summary>Get a single Process</summary>
  /// <param name="ProcessID">ProcessID of the process</param>
  /// <returns></returns>
  public ExtProcess GetExtProcess(string ProcessID)
  {
    TimeSpan cacheTime = this.cacheTime;
    using (List<PSObject>.Enumerator enumerator = this.GetObjectsFromPS($"Get-WMIObject win32_Process -filter \"ProcessId = {ProcessID}\" | Foreach {{  $owner = $_.GetOwner();  $_ | Add-Member -MemberType \"Noteproperty\" -name \"Owner\" -value $(\"{{0}}\\{{1}}\" -f $owner.Domain, $owner.User) -passthru }}", true).GetEnumerator())
    {
      if (enumerator.MoveNext())
      {
        ExtProcess extProcess = new ExtProcess(enumerator.Current, this.remoteRunspace, this.pSCode);
        extProcess.remoteRunspace = this.remoteRunspace;
        extProcess.pSCode = this.pSCode;
        return extProcess;
      }
    }
    return (ExtProcess) null;
  }

  /// <summary>Create a new Process</summary>
  /// <param name="Command">Command to start</param>
  /// <returns>ProcessId of the started process</returns>
  public uint? CreateProcess(string Command)
  {
    try
    {
      string stringFromPs = this.GetStringFromPS($"(start-process {Command} -PassThru).Id");
      if (!string.IsNullOrEmpty(stringFromPs))
        return new uint?(uint.Parse(stringFromPs));
    }
    catch
    {
    }
    return new uint?();
  }
}
