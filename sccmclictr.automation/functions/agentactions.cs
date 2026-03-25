// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.functions.agentactions
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using sccmclictr.automation.policy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management.Automation.Runspaces;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;

#nullable disable
namespace sccmclictr.automation.functions;

/// <summary>Class agentactions.</summary>
public class agentactions : baseInit
{
  internal Runspace remoteRunspace;
  internal TraceSource pSCode;
  internal ccm baseClient;

  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.agentactions" /> class.
  /// </summary>
  /// <param name="RemoteRunspace">The remote runspace.</param>
  /// <param name="PSCode">The ps code.</param>
  /// <param name="oClient">The CCM client object.</param>
  public agentactions(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
    : base(RemoteRunspace, PSCode)
  {
    this.remoteRunspace = RemoteRunspace;
    this.pSCode = PSCode;
    this.baseClient = oClient;
  }

  /// <summary>
  /// Delete InventoryActionStatus from Root\ccm\invagt:InventoryActionStatus
  /// </summary>
  /// <param name="ScheduleID">SCCM Schedule ID</param>
  private void SMSDelInvHist(string ScheduleID)
  {
    try
    {
      this.GetStringFromPS($"[wmi]\"ROOT\\ccm\\invagt:InventoryActionStatus.InventoryActionID='{ScheduleID}'\" | remove-wmiobject");
    }
    catch
    {
    }
  }

  /// <summary>Trigger Hardware-Inventory</summary>
  /// <param name="Full">Enforce a full inventory (Default=Delta)</param>
  /// <returns>false=Error</returns>
  public bool HardwareInventory(bool Full)
  {
    try
    {
      if (Full)
        this.SMSDelInvHist("{00000000-0000-0000-0000-000000000001}");
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000001}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Trigger Software-Inventory</summary>
  /// <param name="Full">Enforce a full inventory (Default=Delta)</param>
  /// <returns>false=Error</returns>
  public bool SoftwareInventory(bool Full)
  {
    try
    {
      if (Full)
        this.SMSDelInvHist("{00000000-0000-0000-0000-000000000002}");
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000002}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Trigger Discovery Inventory</summary>
  /// <param name="Full">Enforce a full Discovery (Delete History-Timestamp)</param>
  /// <returns>false=Error</returns>
  public bool DataDiscovery(bool Full)
  {
    try
    {
      if (Full)
        this.SMSDelInvHist("{00000000-0000-0000-0000-000000000003}");
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000003}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Trigger File Collection</summary>
  /// <param name="Full">Enforce a full File Colelction (Delete History-Timestamp)</param>
  /// <returns>false=Error</returns>
  public bool FileCollection(bool Full)
  {
    try
    {
      if (Full)
        this.SMSDelInvHist("{00000000-0000-0000-0000-000000000010}");
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000010}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>IDMIF Collection</summary>
  /// <param name="Full"></param>
  /// <returns></returns>
  public bool IDMIFCollection(bool Full)
  {
    try
    {
      if (Full)
        this.SMSDelInvHist("{00000000-0000-0000-0000-000000000011}");
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000011}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Client Machine Authentication</summary>
  /// <param name="Full"></param>
  /// <returns></returns>
  public bool ClientMachineAuthentication(bool Full)
  {
    try
    {
      if (Full)
        this.SMSDelInvHist("{00000000-0000-0000-0000-000000000012}");
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000012}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Request Machine Policy Assignments</summary>
  /// <returns>false=Error</returns>
  public bool RequestMachinePolicyAssignments()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000021}'", true);
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Evaluate Machine Policy Assignments</summary>
  /// <returns>false=Error</returns>
  public bool EvaluateMachinePolicyAssignments()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000022}'", true);
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Refresh Default MP Task</summary>
  /// <returns>false=Error</returns>
  public bool RefreshDefaultMPTask()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000023}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Refresh Location Services Task</summary>
  /// <returns>false=Error</returns>
  public bool RefreshLocationServicesTask()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000024}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>LS (Location Service) Timeout Refresh Task</summary>
  /// <returns>false=Error</returns>
  public bool TimeoutLocationServicesTask()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000025}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Policy Agent Request Assignment (User)</summary>
  /// <returns>false=Error</returns>
  public bool RequestUserAssignments()
  {
    try
    {
      foreach (string loggedOnUserSiD in this.baseClient.AgentProperties.GetLoggedOnUserSIDs())
      {
        try
        {
          this.baseClient.SetProperty($"root\\ccm\\Policy\\{loggedOnUserSiD.Replace('-', '_')}\\ActualConfig:CCM_Scheduler_ScheduledMessage.ScheduledMessageID='{{00000000-0000-0000-0000-000000000026}}'", "Triggers", "@('SimpleInterval;Minutes=1;MaxRandomDelayMinutes=0')");
        }
        catch
        {
        }
      }
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Policy Agent Evaluate Assignment (User)</summary>
  /// <returns>false=Error</returns>
  public bool EvaluateUserPolicies()
  {
    try
    {
      foreach (string loggedOnUserSiD in this.baseClient.AgentProperties.GetLoggedOnUserSIDs())
      {
        try
        {
          this.baseClient.SetProperty($"root\\ccm\\Policy\\{loggedOnUserSiD.Replace('-', '_')}\\ActualConfig:CCM_Scheduler_ScheduledMessage.ScheduledMessageID='{{00000000-0000-0000-0000-000000000027}}'", "Triggers", "@('SimpleInterval;Minutes=1')");
        }
        catch
        {
        }
      }
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Software Metering Report Cycle</summary>
  /// <returns>false=Error</returns>
  public bool SoftwareMeteringReportCycle()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000031}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Source Update Message</summary>
  /// <returns>false=Error</returns>
  public bool SourceUpdateMessage()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000032}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Clearing proxy settings cache</summary>
  /// <returns></returns>
  public bool ClearingProxySettingsCache()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000037}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Machine Policy Agent Cleanup</summary>
  /// <returns>false=Error</returns>
  public bool MachinePolicyAgentCleanupCycle()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000040}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>User Policy Agent Cleanup</summary>
  /// <returns></returns>
  public bool UserPolicyAgentCleanupCycle()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000041}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Policy Agent Validate Machine Policy / Assignment</summary>
  /// <returns>false=Error</returns>
  public bool ValidateMachineAssignments()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000042}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Policy Agent Validate User Policy / Assignment</summary>
  /// <returns></returns>
  public bool ValidateUserAssignments()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000043}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Retrying/Refreshing certificates in AD on MP</summary>
  /// <returns>false=Error</returns>
  public bool CertificateMaintenanceCycle()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000051}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Peer DP Status reporting</summary>
  /// <returns>false=Error</returns>
  public bool PeerDistributionPointStatusTask()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000061}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Peer DP Pending package check schedule</summary>
  /// <returns>false=Error</returns>
  public bool PeerDPPackageCheck()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000062}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>SUM Updates install schedule</summary>
  /// <returns></returns>
  public bool SUMUpdatesInstall()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000063}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Hardware Inventory Collection Cycle</summary>
  /// <returns></returns>
  public bool HWInvCollection()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000101}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Software Inventory Collection Cycle</summary>
  /// <returns></returns>
  public bool SWInvCollection()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000102}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Discovery Data Collection Cycle</summary>
  /// <returns></returns>
  public bool DDRCollection()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000103}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>File Collection Cycle</summary>
  /// <returns></returns>
  public bool FileCollection()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000104}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>IDMIF Collection Cycle</summary>
  /// <returns></returns>
  public bool IDMIFCollection()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000105}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Software Metering Usage Report Cycle</summary>
  /// <returns></returns>
  public bool SWMeteringUsageReport()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000106}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Windows Installer Source List Update Cycle</summary>
  /// <returns></returns>
  public bool MSISourceListUpdate()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000107}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Software Updates Agent Assignment Evaluation Cycle</summary>
  /// <returns>false=Error</returns>
  public bool SoftwareUpdatesAgentAssignmentEvaluationCycle()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000108}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Branch Distribution Point Maintenance Task</summary>
  /// <returns></returns>
  public bool BranchDPMaintenanceTask()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000109}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Send Unsent State Messages</summary>
  /// <returns>false=Error</returns>
  public bool SendUnsentStatusMessages()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000111}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>State System policy cache cleanout</summary>
  /// <returns>false=Error</returns>
  public bool StateMessageManagerTask()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000112}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Scan by Update Source</summary>
  /// <returns>false=Error</returns>
  public bool ForceUpdateScan()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000113}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Update Store Policy</summary>
  /// <returns>false=Error</returns>
  public bool UpdateStatusRefresh()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000114}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>State system policy bulk send high</summary>
  /// <returns>false=Error</returns>
  public bool StateSystemPolicyBulksendHigh()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000115}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>State system policy bulk send low</summary>
  /// <returns>false=Error</returns>
  public bool StateSystemPolicyBulksendLow()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000116}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>AMT Status Check Policy</summary>
  /// <returns>false=Error</returns>
  public bool AMTProvisionCycle()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000120}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Application manager policy action</summary>
  /// <returns>false=Error</returns>
  public bool AppManPolicyAction()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000121}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Application manager user policy action</summary>
  /// <returns></returns>
  public bool AppManUserPolicyAction()
  {
    try
    {
      foreach (string loggedOnUserSiD in this.baseClient.AgentProperties.GetLoggedOnUserSIDs())
      {
        try
        {
          this.baseClient.SetProperty($"root\\ccm\\Policy\\{loggedOnUserSiD.Replace('-', '_')}\\ActualConfig:CCM_Scheduler_ScheduledMessage.ScheduledMessageID='{{00000000-0000-0000-0000-000000000122}}'", "Triggers", "@('SimpleInterval;Minutes=1;MaxRandomDelayMinutes=0')");
        }
        catch
        {
        }
      }
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Application manager global evaluation action</summary>
  /// <returns></returns>
  public bool AppManGlobalEvaluation()
  {
    try
    {
      this.GetObjectsFromPS("((New-Object -comobject \"CPApplet.CPAppletMgr\").GetClientActions() | Where-Object { $_.ActionID -eq '{00000000-0000-0000-0000-000000000123}' }).PerformAction()", true);
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>PowerMgmt Start Summarization Task</summary>
  /// <returns>false=Error</returns>
  public bool PowerMgmtStartSummarizationTask()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000131}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Endpoint deployment reevaluate</summary>
  /// <returns>false=Error</returns>
  public bool EndpointDeploymentMessage()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000221}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Endpoint AM policy reevaluate</summary>
  /// <returns></returns>
  public bool EndpointAMPolicyreevaluate()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000222}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>External event detection</summary>
  /// <returns>false=Error</returns>
  public bool ExternalEventDetectionMessage()
  {
    try
    {
      this.CallClassMethod("ROOT\\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000223}'");
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Cleanup the Message Queue from the SCCM Agent</summary>
  /// <returns></returns>
  public bool CleanupMessageQueue()
  {
    try
    {
      int stopResult = (int) this.baseClient.Services.GetService("CcmExec").StopService();
      this.GetStringFromPS($"get-childitem '{Path.Combine(this.baseClient.AgentProperties.LocalSCCMAgentPath, "ServiceData\\Messaging\\EndpointQueues")}' -include *.msg,*.que -recurse | foreach ($_) {{remove-item $_.fullname -force}}");
      int startResult = (int) this.baseClient.Services.GetService("CcmExec").StartService();
      this.StateMessageManagerTask();
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Reset Policy</summary>
  /// <param name="Hardreset"></param>
  /// <returns>false=Error</returns>
  public bool ResetPolicy(bool Hardreset)
  {
    try
    {
      if (Hardreset)
        this.CallClassMethod("ROOT\\ccm:SMS_Client", nameof (ResetPolicy), "1", true);
      else
        this.CallClassMethod("ROOT\\ccm:SMS_Client", nameof (ResetPolicy), "0", true);
      this.MachinePolicyAgentCleanupCycle();
      this.RequestMachinePolicyAssignments();
    }
    catch
    {
      return false;
    }
    return true;
  }

  public bool SetClientProvisioningMode(bool bEnable)
  {
    try
    {
      if (bEnable)
        this.CallClassMethod("ROOT\\ccm:SMS_Client", nameof (SetClientProvisioningMode), "1", true);
      else
        this.CallClassMethod("ROOT\\ccm:SMS_Client", nameof (SetClientProvisioningMode), "0", true);
    }
    catch
    {
      return false;
    }
    return true;
  }

  /// <summary>Repairs the SCCM/CM12 agent.</summary>
  /// <returns><c>true</c> if method attempted to repair the agent, <c>false</c> otherwise.</returns>
  public bool RepairAgent()
  {
    string productCode = this.baseClient.AgentProperties.ProductCode;
    if (!productCode.StartsWith("{"))
      return false;
    this.baseClient.GetStringFromPS($"& msiexec.exe /fpecms '{productCode}'");
    return true;
  }

  /// <summary>Remove the SCCM/CM12 Agent</summary>
  /// <returns></returns>
  public bool UninstallAgent()
  {
    string productCode = this.baseClient.AgentProperties.ProductCode;
    if (!productCode.StartsWith("{"))
      return false;
    this.baseClient.GetStringFromPS($"& msiexec.exe /x '{productCode}' REBOOT=ReallySuppress /q");
    return true;
  }

  /// <summary>Reset the Pauses Software Distribution flag</summary>
  /// <returns>true = success; false = error</returns>
  public bool ResetPausedSWDist()
  {
    try
    {
      this.GetStringFromPS("New-ItemProperty -path \"HKLM:\\SOFTWARE\\Microsoft\\SMS\\Mobile Client\\Software Distribution\\State\" -Name \"Paused\" -Type DWORD -force -value 0", true);
      this.GetStringFromPS("New-ItemProperty -path \"HKLM:\\SOFTWARE\\Microsoft\\SMS\\Mobile Client\\Software Distribution\\State\" -Name \"PausedCookie\" -Type DWORD -force -value 0", true);
      return true;
    }
    catch
    {
    }
    return false;
  }

  /// <summary>Reset the ProvisioningMode Flag</summary>
  /// <returns>true = success; false = error</returns>
  public bool ResetProvisioningMode()
  {
    try
    {
      this.GetStringFromPS("New-ItemProperty -path \"HKLM:\\SOFTWARE\\Microsoft\\CCM\\CcmExec\" -Name \"ProvisioningMode\" -Type string -force -value \"false\"", true);
      this.baseClient.AgentActions.SetClientProvisioningMode(false);
      return true;
    }
    catch
    {
    }
    return false;
  }

  /// <summary>Remove SystemTaskExclude entries from Registry</summary>
  /// <returns>true = success; false = error</returns>
  public bool SystemTaskExclude()
  {
    try
    {
      this.GetStringFromPS("New-ItemProperty -path \"HKLM:\\SOFTWARE\\Microsoft\\CCM\\CcmExec\" -Name \"SystemTaskExcludes\" -Type string -force -value \"\"", true);
      return true;
    }
    catch
    {
    }
    return false;
  }

  /// <summary>Remove IsCacheCopyNeededCallBack entry from Registry</summary>
  /// <returns>true = success; false = error</returns>
  public bool IsCacheCopyNeededCallBack()
  {
    try
    {
      this.GetStringFromPS("Remove-ItemProperty 'hklm:\\Software\\Microsoft\\SMS\\Mobile Client\\Software Distribution\\' 'IsCacheCopyNeededCallBack' -ea SilentlyContinue", true);
      return true;
    }
    catch
    {
    }
    return false;
  }

  /// <summary>Import a local Application Policy</summary>
  /// <param name="Body">Policy Body as XML</param>
  /// <param name="BodySignature">Body Signature</param>
  /// <param name="BodySource">'LOCAL' if not specified</param>
  /// <returns></returns>
  public bool ApplyPolicyEx(string Body, string BodySignature, string BodySource = "LOCAL")
  {
    try
    {
      return !string.IsNullOrEmpty(this.baseClient.GetStringFromPS($"([wmiclass]'ROOT\\ccm\\ClientSdk:CCM_SoftwareCatalogUtilities').ApplyPolicyEx('{Body}','{BodySignature}','{BodySource}').Id"));
    }
    catch
    {
    }
    return false;
  }

  /// <summary>Import an Application into the users policy store</summary>
  /// <param name="ApplicationID"></param>
  /// <returns></returns>
  public bool ImportApplicationPolicy(string ApplicationID)
  {
    try
    {
      agentproperties.DeviceId getDeviceId = this.baseClient.AgentProperties.GetDeviceId;
      string portalUrl = this.baseClient.AgentProperties.PortalURL;
      string xml = "";
      if (string.IsNullOrEmpty(portalUrl))
        return false;
      HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(portalUrl + "/applicationviewservice.asmx");
      httpWebRequest.Headers.Add("SOAPAction", "http://schemas.microsoft.com/5.0.0.0/ConfigurationManager/SoftwareCatalog/Website/InstallApplication");
      httpWebRequest.ContentType = "text/xml;charset=\"utf-8\"";
      httpWebRequest.Accept = "text/xml";
      httpWebRequest.Method = "POST";
      httpWebRequest.UseDefaultCredentials = true;
      XmlDocument xmlDocument1 = new XmlDocument();
      xmlDocument1.LoadXml($"<?xml version=\"1.0\" encoding=\"utf-8\"?><s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body><InstallApplication xmlns=\"http://schemas.microsoft.com/5.0.0.0/ConfigurationManager/SoftwareCatalog/Website\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><applicationID>{ApplicationID}</applicationID><deviceID>{getDeviceId.ClientId},{getDeviceId.SignedClientId}</deviceID><reserved/></InstallApplication></s:Body></s:Envelope>");
      using (Stream requestStream = httpWebRequest.GetRequestStream())
        xmlDocument1.Save(requestStream);
      using (WebResponse response = httpWebRequest.GetResponse())
      {
        using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
          xml = streamReader.ReadToEnd();
      }
      if (!string.IsNullOrEmpty(xml))
      {
        XmlDocument xmlDocument2 = new XmlDocument();
        xmlDocument2.LoadXml(xml);
        this.baseClient.GetStringFromPS(agentactions.getPSWMIScript(Encoding.Unicode.GetString(Convert.FromBase64String(xmlDocument2.SelectSingleNode("//*[local-name()='InstallApplicationResult']")["PolicyAssignmentsDocument"].InnerText))), true);
        Thread.Sleep(1000);
        this.baseClient.AgentActions.AppManGlobalEvaluation();
        return true;
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
    }
    return false;
  }

  /// <summary>Generate PS to import App Policy</summary>
  /// <param name="sXMLBody"></param>
  /// <returns></returns>
  public static string getPSWMIScript(string sXMLBody)
  {
    System.IO.File.WriteAllText(Environment.ExpandEnvironmentVariables("%temp%\\body.xml"), sXMLBody);
    XmlDocument xmlDocument1 = new XmlDocument();
    xmlDocument1.LoadXml(sXMLBody);
    List<string> stringList = new List<string>();
    foreach (XmlNode selectNode1 in xmlDocument1.SelectNodes("/ReplyAssignments/EPolicy/PolicyXML"))
    {
      try
      {
        string xml = localpolicy.DecompressPolicy(selectNode1.InnerXml.ToString());
        xml.ToString();
        XmlDocument xmlDocument2 = new XmlDocument();
        xmlDocument2.LoadXml(xml);
        foreach (XmlNode selectNode2 in xmlDocument2.SelectNodes("/Policy/PolicyRule/PolicyAction/instance"))
        {
          try
          {
            string wmiClassName = selectNode2.Attributes["class"].Value.ToString();
            stringList.Add($"$ruleClass = ([WMICLASS]'ROOT\\ccm\\Policy\\Machine\\ActualConfig:{wmiClassName}').CreateInstance();");
            foreach (XmlNode selectNode3 in selectNode2.SelectNodes("property"))
            {
              try
              {
                string propertyType = selectNode3.Attributes["type"].Value;
                if (!string.IsNullOrEmpty(selectNode3.InnerText))
                {
                  switch (propertyType)
                  {
                    case "8200":
                      stringList.Add($"$ruleClass[\"{selectNode3.Attributes["name"].Value.ToString()}\"] = @(\"{selectNode3.InnerText.Trim().Replace("\r\n", "`r`n").Replace("'", "`'").Replace("\"", "`\"")}\");");
                      continue;
                    case "19":
                      stringList.Add($"$ruleClass[\"{selectNode3.Attributes["name"].Value.ToString()}\"] = {selectNode3.InnerText.Trim().Replace("\r\n", "`r`n").Replace("'", "`'").Replace("\"", "`\"")};");
                      continue;
                    default:
                      stringList.Add($"$ruleClass[\"{selectNode3.Attributes["name"].Value.ToString()}\"] = \"{selectNode3.InnerText.Trim().Replace("\r\n", "`r`n").Replace("'", "`'").Replace("\"", "`\"")}\";");
                      continue;
                  }
                }
              }
              catch
              {
              }
            }
            stringList.Add("$ruleClass.Put();");
          }
          catch
          {
          }
        }
      }
      catch
      {
      }
    }
    string contents = string.Join("", stringList.ToArray());
    System.IO.File.WriteAllText(Environment.ExpandEnvironmentVariables("%temp%\\sccmclictr.ps1"), contents);
    return contents;
  }
}
