// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.functions.Win32_Service
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
public class Win32_Service : Win32_BaseService
{
  internal baseInit oNewBase;

  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.Win32_Service" /> class.
  /// </summary>
  /// <param name="WMIObject">The WMI object.</param>
  /// <param name="RemoteRunspace">The remote runspace.</param>
  /// <param name="PSCode">The PowerShell code.</param>
  public Win32_Service(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
    this.CheckPoint = WMIObject.Properties[nameof (CheckPoint)].Value as uint?;
    this.ProcessId = WMIObject.Properties[nameof (ProcessId)].Value as uint?;
    this.WaitHint = WMIObject.Properties[nameof (WaitHint)].Value as uint?;
  }

  public uint? CheckPoint { get; set; }

  public uint? ProcessId { get; set; }

  public uint? WaitHint { get; set; }

  /// <summary>Start the Service and wait until it's started</summary>
  /// <returns>0</returns>
  public uint StartService()
  {
    string hash1 = this.oNewBase.CreateHash($"(Get-Service '{this.Name}').Start()");
    this.oNewBase.Cache.Remove(hash1, (string) null);
    string hash2 = this.oNewBase.CreateHash($"(Get-Service '{this.Name}').Status");
    this.oNewBase.Cache.Remove(hash2, (string) null);
    this.oNewBase.GetStringFromPS($"(Get-Service '{this.Name}').Start()");
    this.State = this.oNewBase.GetStringFromPS($"(Get-Service '{this.Name}').Status");
    this.oNewBase.Cache.Remove(hash1, (string) null);
    this.oNewBase.Cache.Remove(hash2, (string) null);
    return 0;
  }

  /// <summary>Stops the service.</summary>
  /// <returns>UInt32.</returns>
  public uint StopService()
  {
    string hash1 = this.oNewBase.CreateHash($"(Get-Service '{this.Name}').Stop()");
    this.oNewBase.Cache.Remove(hash1, (string) null);
    string hash2 = this.oNewBase.CreateHash($"(Get-Service '{this.Name}').Status");
    this.oNewBase.Cache.Remove(hash2, (string) null);
    this.oNewBase.GetStringFromPS($"(Get-Service '{this.Name}').Stop()");
    this.State = this.oNewBase.GetStringFromPS($"(Get-Service '{this.Name}').Status");
    this.oNewBase.Cache.Remove(hash1, (string) null);
    this.oNewBase.Cache.Remove(hash2, (string) null);
    return 0;
  }

  /// <summary>Restarts the service.</summary>
  /// <returns>UInt32.</returns>
  public uint RestartService()
  {
    string hash1 = this.oNewBase.CreateHash($"Restart-Service '{this.Name}'");
    this.oNewBase.Cache.Remove(hash1, (string) null);
    string hash2 = this.oNewBase.CreateHash($"(Get-Service '{this.Name}').Status");
    this.oNewBase.Cache.Remove(hash2, (string) null);
    this.oNewBase.GetStringFromPS($"Restart-Service '{this.Name}'");
    this.State = this.oNewBase.GetStringFromPS($"(Get-Service '{this.Name}').Status");
    this.oNewBase.Cache.Remove(hash1, (string) null);
    this.oNewBase.Cache.Remove(hash2, (string) null);
    return 0;
  }

  /// <summary>Pauses the service.</summary>
  /// <returns>UInt32.</returns>
  public uint PauseService()
  {
    string hash1 = this.oNewBase.CreateHash($"(Get-Service '{this.Name}').Pause()");
    this.oNewBase.Cache.Remove(hash1, (string) null);
    string hash2 = this.oNewBase.CreateHash($"(Get-Service '{this.Name}').Status");
    this.oNewBase.Cache.Remove(hash2, (string) null);
    this.oNewBase.GetStringFromPS($"(Get-Service '{this.Name}').Pause()");
    this.State = this.oNewBase.GetStringFromPS($"(Get-Service '{this.Name}').Status");
    this.oNewBase.Cache.Remove(hash1, (string) null);
    this.oNewBase.Cache.Remove(hash2, (string) null);
    return 0;
  }

  /// <summary>Resumes the service.</summary>
  /// <returns>UInt32.</returns>
  public uint ResumeService()
  {
    string hash1 = this.oNewBase.CreateHash($"(Get-Service '{this.Name}').Start()");
    this.oNewBase.Cache.Remove(hash1, (string) null);
    string hash2 = this.oNewBase.CreateHash($"(Get-Service '{this.Name}').Status");
    this.oNewBase.Cache.Remove(hash2, (string) null);
    this.oNewBase.GetStringFromPS($"(Get-Service '{this.Name}').Start()");
    this.State = this.oNewBase.GetStringFromPS($"(Get-Service '{this.Name}').Status");
    this.oNewBase.Cache.Remove(hash1, (string) null);
    this.oNewBase.Cache.Remove(hash2, (string) null);
    return 0;
  }

  /// <summary>Set a Service StartupType to Manual</summary>
  /// <returns>false = error</returns>
  public bool SetStartup_Manual()
  {
    bool flag = false;
    try
    {
      this.oNewBase.GetStringFromPS($"Set-Service {this.Name} -StartupType Manual", true);
    }
    catch
    {
      flag = false;
    }
    return flag;
  }

  /// <summary>Set a Service StartupType to Automatic</summary>
  /// <returns>false = error</returns>
  public bool SetStartup_Auto()
  {
    bool flag = false;
    try
    {
      this.oNewBase.GetStringFromPS($"Set-Service {this.Name} -StartupType Automatic", true);
    }
    catch
    {
      flag = false;
    }
    return flag;
  }

  /// <summary>Set a Service StartupType to Disabled</summary>
  /// <returns>false = error</returns>
  public bool SetStartup_Disabled()
  {
    bool flag = false;
    try
    {
      this.oNewBase.GetStringFromPS($"Set-Service {this.Name} -StartupType Disabled", true);
    }
    catch
    {
      flag = false;
    }
    return flag;
  }
}
