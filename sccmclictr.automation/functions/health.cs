// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.functions.health
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using sccmclictr.automation.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

#nullable disable
namespace sccmclictr.automation.functions;

/// <summary>Agent health and repair functions</summary>
public class health : baseInit
{
  internal Runspace remoteRunspace;
  internal TraceSource pSCode;
  internal ccm baseClient;

  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.health" /> class.
  /// </summary>
  /// <param name="RemoteRunspace">The remote runspace.</param>
  /// <param name="PSCode">The PowerShell code.</param>
  /// <param name="oClient">a CCM Client object.</param>
  public health(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
    : base(RemoteRunspace, PSCode)
  {
    this.remoteRunspace = RemoteRunspace;
    this.pSCode = PSCode;
    this.baseClient = oClient;
  }

  /// <summary>Verify WMI Repository (winmgmt /verifyrepository).</summary>
  /// <returns>Command results as string</returns>
  public string WMIVerifyRepository()
  {
    TimeSpan cacheTime = this.cacheTime;
    this.cacheTime = new TimeSpan(0, 0, 30);
    string stringFromPs = this.GetStringFromPS("winmgmt /verifyrepository");
    this.cacheTime = cacheTime;
    return stringFromPs;
  }

  /// <summary>
  /// Performs a consistency check on the WMI repository (winmgmt /salvagerepository).
  /// </summary>
  /// <returns>Command results as string</returns>
  public string WMISalvageRepository() => this.GetStringFromPS("winmgmt /salvagerepository", true);

  /// <summary>
  /// The repository is reset to the initial state when the operating system is first installed (winmgmt /resetrepository).
  /// </summary>
  /// <returns>Command results as string</returns>
  public string WMIResetRepository()
  {
    return this.GetStringFromPS("Stop-Service winmgmt -Force; winmgmt /resetrepository", true);
  }

  /// <summary>
  /// Registers the system performance libraries with WMI (winmgmt /resyncperf).
  /// </summary>
  /// <returns>Command results as string</returns>
  public string WMIRegPerfLibraries() => this.GetStringFromPS("winmgmt /resyncperf", true);

  /// <summary>Rgister DLL using Regsvr32</summary>
  /// <param name="File"></param>
  /// <returns></returns>
  public string RegsiertDLL(string File)
  {
    return this.GetStringFromPS($"regsvr32.exe /s \"{Environment.ExpandEnvironmentVariables(File)}\"", true);
  }

  /// <summary>
  /// State if DCOM is enabled (HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Ole:EnableDCOM)
  /// </summary>
  public bool EnableDCOM
  {
    get
    {
      TimeSpan cacheTime = this.cacheTime;
      this.cacheTime = new TimeSpan(0, 0, 30);
      string stringFromPs = this.GetStringFromPS("(get-ItemProperty \"HKLM:\\SOFTWARE\\Microsoft\\Ole\").\"EnableDCOM\"");
      this.cacheTime = cacheTime;
      return string.Compare(stringFromPs, "Y", true) == 0;
    }
    set
    {
      this.GetStringFromPS($"set-ItemProperty -Path \"HKLM:\\SOFTWARE\\Microsoft\\Ole\" -Name \"EnableDCOM\" -Value \"{(value ? (object) "Y" : (object) "N")}\"");
    }
  }

  /// <summary>
  /// Get the DCOM Permission ACL from a Binary Registry Key.
  /// </summary>
  /// <param name="HKLMKey">HKLM Key like "software\microsoft\ole"</param>
  /// <param name="RegValue">Registry Value that contains the DCOM Permission like "MachineLaunchRestriction"</param>
  /// <returns></returns>
  public string GetDCOMPerm(string HKLMKey, string RegValue)
  {
    TimeSpan cacheTime = this.cacheTime;
    this.cacheTime = new TimeSpan(0, 0, 30);
    string stringFromPs = this.GetStringFromPS(string.Format(Settings.Default.PSGetDCOMPerm, (object) HKLMKey, (object) RegValue));
    this.cacheTime = cacheTime;
    return stringFromPs;
  }

  /// <summary>
  /// Set the DCOM Permission ACL to a Binary Registry Key.
  /// O:BAG:BAD:(A;;CCDCSW;;;WD)(A;;CCDCLCSWRP;;;BA)(A;;CCDCLCSWRP;;;LU)(A;;CCDCLCSWRP;;;S-1-5-32-562)
  /// </summary>
  /// <param name="HKLMKey">HKLM Key like "software\microsoft\ole"</param>
  /// <param name="RegValue">Registry Value that contains the DCOM Permission like "MachineLaunchRestriction"</param>
  /// <param name="ACL">ACL like "O:BAG:BAD:(A;;CCDCSW;;;WD)(A;;CCDCLCSWRP;;;BA)(A;;CCDCLCSWRP;;;LU)(A;;CCDCLCSWRP;;;S-1-5-32-562)"</param>
  /// <returns></returns>
  public string SetDCOMPerm(string HKLMKey, string RegValue, string ACL)
  {
    this.GetStringFromPS(string.Format(Settings.Default.PSSetDCOMPerm, (object) HKLMKey, (object) RegValue, (object) ACL), true);
    return "";
  }

  /// <summary>Run a "basic" Powershell Health check</summary>
  /// <returns></returns>
  public string RunHealthCheck()
  {
    try
    {
      return this.GetObjectsFromPS(Resources.HealthCheck)[0].ToString();
    }
    catch
    {
    }
    return "";
  }

  /// <summary>Delete root\ccm namespace in WMI</summary>
  public void DeleteCCMNamespace()
  {
    this.GetStringFromPS("gwmi -query \"SELECT * FROM __Namespace WHERE Name='CCM'\" -Namespace \"root\" | Remove-WmiObject");
  }

  /// <summary>Delete all machine certificates from the SMS Folder</summary>
  public void DeleteSMSCertificates()
  {
    try
    {
      this.GetStringFromPS("Remove-Item -path HKLM:\\SOFTWARE\\Microsoft\\SystemCertificates\\SMS\\* -Recurse");
    }
    catch
    {
    }
  }

  /// <summary>Run CCMEval.exe</summary>
  public void RunCCMEval()
  {
    this.baseClient.Process.CreateProcess(this.baseClient.AgentProperties.LocalSCCMAgentPath + "ccmeval.exe");
  }

  /// <summary>DateTime of last CCMEval cycle</summary>
  public DateTime LastCCMEval
  {
    get
    {
      try
      {
        return DateTime.Parse(this.GetStringFromPS($"[xml]$ccmeval = Get-Content \"{this.baseClient.AgentProperties.LocalSCCMAgentPath}CcmEvalReport.xml\"; $ccmeval.ClientHealthReport.Summary.EvaluationTime"));
      }
      catch
      {
      }
      return new DateTime();
    }
  }

  /// <summary>Show results of the CCMEval taks</summary>
  /// <returns>List of CCMEval results</returns>
  public List<health.ccmeval> GetCCMEvalStatus()
  {
    List<health.ccmeval> ccmEvalStatus = new List<health.ccmeval>();
    foreach (PSObject psObject in this.GetObjectsFromPS($"[xml]$ccmeval = Get-Content \"{this.baseClient.AgentProperties.LocalSCCMAgentPath}CcmEvalReport.xml\"; $ccmeval.ClientHealthReport.HealthChecks.HealthCheck", true))
    {
      try
      {
        ccmEvalStatus.Add(new health.ccmeval()
        {
          ID = psObject.Properties["ID"].Value.ToString(),
          Description = psObject.Properties["Description"].Value.ToString(),
          ResultCode = psObject.Properties["ResultCode"].Value.ToString(),
          ResultType = psObject.Properties["ResultType"].Value.ToString(),
          ResultDetail = psObject.Properties["ResultDetail"].Value.ToString(),
          StepDetail = psObject.Properties["StepDetail"].Value.ToString(),
          text = psObject.Properties["#text"].Value.ToString()
        });
      }
      catch
      {
      }
    }
    return ccmEvalStatus;
  }

  /// <summary>
  /// https://msdn.microsoft.com/en-us/library/cc146437.aspx; This causes the client to resend a full compliance report to the Configuration Manager server
  /// </summary>
  public void RefreshServerComplianceState()
  {
    try
    {
      this.GetStringFromPS("(New-Object -ComObject Microsoft.CCM.UpdatesStore).RefreshServerComplianceState()", true);
    }
    catch
    {
    }
  }

  /// <summary>ccmeval result entry</summary>
  public class ccmeval
  {
    public string ID { get; set; }

    public string Description { get; set; }

    public string ResultCode { get; set; }

    public string ResultType { get; set; }

    public string ResultDetail { get; set; }

    public string StepDetail { get; set; }

    public string text { get; set; }
  }
}
