// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.functions.softwareupdates
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

#nullable disable
namespace sccmclictr.automation.functions;

/// <summary>Software Update Class</summary>
public class softwareupdates : baseInit
{
  internal Runspace remoteRunspace;
  internal TraceSource pSCode;
  internal ccm baseClient;

  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwareupdates" /> class.
  /// </summary>
  /// <param name="RemoteRunspace">The remote runspace.</param>
  /// <param name="PSCode">The PowerShell code.</param>
  /// <param name="oClient">A CCM Client object.</param>
  public softwareupdates(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
    : base(RemoteRunspace, PSCode)
  {
    this.remoteRunspace = RemoteRunspace;
    this.pSCode = PSCode;
    this.baseClient = oClient;
  }

  /// <summary>Get all known Software Updates with status</summary>
  public List<softwareupdates.CCM_UpdateStatus> UpdateStatus => this.GetUpdateStatus(false);

  /// <summary>Get all known Software Updates with status</summary>
  /// <param name="reLoad">true = force reload; false = use cached results</param>
  /// <returns></returns>
  public List<softwareupdates.CCM_UpdateStatus> GetUpdateStatus(bool reLoad)
  {
    List<softwareupdates.CCM_UpdateStatus> updateStatus = new List<softwareupdates.CCM_UpdateStatus>();
    foreach (PSObject WMIObject in this.GetObjects("root\\ccm\\SoftwareUpdates\\UpdatesStore", "SELECT * FROM CCM_UpdateStatus", reLoad, new TimeSpan(0, 2, 0)))
      updateStatus.Add(new softwareupdates.CCM_UpdateStatus(WMIObject, this.remoteRunspace, this.pSCode)
      {
        remoteRunspace = this.remoteRunspace,
        pSCode = this.pSCode
      });
    return updateStatus;
  }

  /// <summary>Get all mandatory Updates</summary>
  public List<softwareupdates.CCM_TargetedUpdateEx1> TargetUpdates
  {
    get
    {
      List<softwareupdates.CCM_TargetedUpdateEx1> targetUpdates = new List<softwareupdates.CCM_TargetedUpdateEx1>();
      foreach (PSObject WMIObject in this.GetObjects("root\\ccm\\SoftwareUpdates\\DeploymentAgent", "SELECT * FROM CCM_TargetedUpdateEx1"))
        targetUpdates.Add(new softwareupdates.CCM_TargetedUpdateEx1(WMIObject, this.remoteRunspace, this.pSCode)
        {
          remoteRunspace = this.remoteRunspace,
          pSCode = this.pSCode
        });
      return targetUpdates;
    }
  }

  /// <summary>Get the Content Locations for Software Updates</summary>
  public List<softwareupdates.CCM_UpdateSource> UpdateSource
  {
    get
    {
      List<softwareupdates.CCM_UpdateSource> updateSource = new List<softwareupdates.CCM_UpdateSource>();
      foreach (PSObject WMIObject in this.GetObjects("root\\ccm\\SoftwareUpdates\\WUAHandler", "SELECT * FROM CCM_UpdateSource"))
        updateSource.Add(new softwareupdates.CCM_UpdateSource(WMIObject, this.remoteRunspace, this.pSCode)
        {
          remoteRunspace = this.remoteRunspace,
          pSCode = this.pSCode
        });
      return updateSource;
    }
  }

  /// <summary>
  /// Show required Software Updates and the current state (from cache)
  /// </summary>
  public List<softwareupdates.CCM_SoftwareUpdate> SoftwareUpdate => this.GetSoftwareUpdate(false);

  /// <summary>Show required Software Updates and the current state</summary>
  /// <param name="reLoad">Enforce reloading the cache</param>
  /// <returns></returns>
  public List<softwareupdates.CCM_SoftwareUpdate> GetSoftwareUpdate(bool reLoad)
  {
    return this.GetSoftwareUpdate(reLoad, new TimeSpan(0, 2, 0));
  }

  /// <summary>Show required Software Updates and the current state</summary>
  /// <param name="reLoad">Enforce reloading the cache</param>
  /// <param name="tCache">TTL to keep Items in Cache</param>
  /// <returns></returns>
  public List<softwareupdates.CCM_SoftwareUpdate> GetSoftwareUpdate(bool reLoad, TimeSpan tCache)
  {
    List<softwareupdates.CCM_SoftwareUpdate> softwareUpdate = new List<softwareupdates.CCM_SoftwareUpdate>();
    foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\ClientSDK", "SELECT * FROM CCM_SoftwareUpdate", reLoad, tCache))
    {
      softwareupdates.CCM_SoftwareUpdate ccmSoftwareUpdate = new softwareupdates.CCM_SoftwareUpdate(WMIObject, this.remoteRunspace, this.pSCode);
      ccmSoftwareUpdate.remoteRunspace = this.remoteRunspace;
      ccmSoftwareUpdate.pSCode = this.pSCode;
      softwareUpdate.Add(ccmSoftwareUpdate);
    }
    return softwareUpdate;
  }

  /// <summary>
  /// Install all required updates with a deadline (mandatory).
  /// </summary>
  public void InstallAllRequiredUpdates()
  {
    this.baseClient.GetObjectsFromPS(string.Format("([wmiclass]'ROOT\\ccm\\ClientSDK:CCM_SoftwareUpdatesManager').InstallUpdates()"), true);
  }

  /// <summary>
  /// Install all approved updates even if they do not have a deadline.
  /// </summary>
  public void InstallAllApprovedUpdates()
  {
    this.baseClient.GetObjectsFromPS(string.Format("([wmiclass]'ROOT\\ccm\\ClientSDK:CCM_SoftwareUpdatesManager').InstallUpdates([System.Management.ManagementObject[]] (get-wmiobject -query 'SELECT * FROM CCM_SoftwareUpdate' -namespace 'ROOT\\ccm\\ClientSDK'))"), true);
  }

  /// <summary>Check if there are updates with pending reboot</summary>
  /// <returns></returns>
  public bool UpdateWithPendingReboot()
  {
    return this.GetSoftwareUpdate(true).Where<softwareupdates.CCM_SoftwareUpdate>((Func<softwareupdates.CCM_SoftwareUpdate, bool>) (t =>
    {
      uint? evaluationState = t.EvaluationState;
      uint pendingSoftReboot = 8;
      int isPendingSoftReboot = (int) evaluationState.GetValueOrDefault() == (int) pendingSoftReboot & evaluationState.HasValue ? 1 : 0;
      evaluationState = t.EvaluationState;
      uint pendingHardReboot = 9;
      int isPendingHardReboot = (int) evaluationState.GetValueOrDefault() == (int) pendingHardReboot & evaluationState.HasValue ? 1 : 0;
      int hasPendingReboot = isPendingSoftReboot | isPendingHardReboot;
      evaluationState = t.EvaluationState;
      uint waitReboot = 10;
      int isWaitReboot = (int) evaluationState.GetValueOrDefault() == (int) waitReboot & evaluationState.HasValue ? 1 : 0;
      return (hasPendingReboot | isWaitReboot) != 0;
    })).ToList<softwareupdates.CCM_SoftwareUpdate>().Count > 0;
  }

  /// <summary>
  /// Check if there are pending Updates (Installation not started)
  /// </summary>
  /// <returns></returns>
  public bool UpdatesInstallationNotStarted()
  {
    return this.GetSoftwareUpdate(true).Where<softwareupdates.CCM_SoftwareUpdate>((Func<softwareupdates.CCM_SoftwareUpdate, bool>) (t =>
    {
      uint? evaluationState = t.EvaluationState;
      uint stateAvailable = 1;
      int isAvailable = (int) evaluationState.GetValueOrDefault() == (int) stateAvailable & evaluationState.HasValue ? 1 : 0;
      evaluationState = t.EvaluationState;
      uint stateNone = 0;
      int isNone = (int) evaluationState.GetValueOrDefault() == (int) stateNone & evaluationState.HasValue ? 1 : 0;
      int isNotStarted = isAvailable | isNone;
      evaluationState = t.EvaluationState;
      uint stateWaitServiceWindow = 14;
      int isWaitServiceWindow = (int) evaluationState.GetValueOrDefault() == (int) stateWaitServiceWindow & evaluationState.HasValue ? 1 : 0;
      int isNotStartedOrWaiting = isNotStarted | isWaitServiceWindow;
      evaluationState = t.EvaluationState;
      uint stateWaitUserLogon = 21;
      int isWaitUserLogon = (int) evaluationState.GetValueOrDefault() == (int) stateWaitUserLogon & evaluationState.HasValue ? 1 : 0;
      return (isNotStartedOrWaiting | isWaitUserLogon) != 0;
    })).ToList<softwareupdates.CCM_SoftwareUpdate>().Count > 0;
  }

  /// <summary>Chek if there an update installation is running</summary>
  /// <returns></returns>
  public bool UpdateInstallationRunning()
  {
    return this.GetSoftwareUpdate(true).Where<softwareupdates.CCM_SoftwareUpdate>((Func<softwareupdates.CCM_SoftwareUpdate, bool>) (t =>
    {
      uint? evaluationState = t.EvaluationState;
      uint stateSubmitted = 2;
      int isSubmitted = (int) evaluationState.GetValueOrDefault() == (int) stateSubmitted & evaluationState.HasValue ? 1 : 0;
      evaluationState = t.EvaluationState;
      uint stateDetecting = 3;
      int isDetecting = (int) evaluationState.GetValueOrDefault() == (int) stateDetecting & evaluationState.HasValue ? 1 : 0;
      int isSubmittedOrDetecting = isSubmitted | isDetecting;
      evaluationState = t.EvaluationState;
      uint statePreDownload = 4;
      int isPreDownload = (int) evaluationState.GetValueOrDefault() == (int) statePreDownload & evaluationState.HasValue ? 1 : 0;
      int isInProgress = isSubmittedOrDetecting | isPreDownload;
      evaluationState = t.EvaluationState;
      uint stateDownloading = 5;
      int isDownloading = (int) evaluationState.GetValueOrDefault() == (int) stateDownloading & evaluationState.HasValue ? 1 : 0;
      int isInProgressOrDownloading = isInProgress | isDownloading;
      evaluationState = t.EvaluationState;
      uint stateWaitInstall = 6;
      int isWaitInstall = (int) evaluationState.GetValueOrDefault() == (int) stateWaitInstall & evaluationState.HasValue ? 1 : 0;
      int isActiveOrWaiting = isInProgressOrDownloading | isWaitInstall;
      evaluationState = t.EvaluationState;
      uint stateInstalling = 7;
      int isInstalling = (int) evaluationState.GetValueOrDefault() == (int) stateInstalling & evaluationState.HasValue ? 1 : 0;
      int isRunning = isActiveOrWaiting | isInstalling;
      evaluationState = t.EvaluationState;
      uint stateVerifying = 11;
      int isVerifying = (int) evaluationState.GetValueOrDefault() == (int) stateVerifying & evaluationState.HasValue ? 1 : 0;
      return (isRunning | isVerifying) != 0;
    })).ToList<softwareupdates.CCM_SoftwareUpdate>().Count > 0;
  }

  /// <summary>Check if there are failed updates</summary>
  /// <returns></returns>
  public bool UpdateInstallationErrors()
  {
    return this.GetSoftwareUpdate(true).Where<softwareupdates.CCM_SoftwareUpdate>((Func<softwareupdates.CCM_SoftwareUpdate, bool>) (t =>
    {
      uint? evaluationState = t.EvaluationState;
      uint stateError = 13;
      return (int) evaluationState.GetValueOrDefault() == (int) stateError & evaluationState.HasValue;
    })).ToList<softwareupdates.CCM_SoftwareUpdate>().Count > 0;
  }

  /// <summary>Installs the updates.</summary>
  /// <param name="Updates">The updates.</param>
  public void InstallUpdates(List<softwareupdates.CCM_SoftwareUpdate> Updates)
  {
    List<string> updateIds = new List<string>();
    this.baseClient.GetObjectsFromPS($"[System.Management.ManagementObject[]] $a = get-wmiobject -query \"SELECT * FROM CCM_SoftwareUpdate WHERE UpdateID like '{string.Join("' OR UpdateID='", (IEnumerable<string>) Updates.Select<softwareupdates.CCM_SoftwareUpdate, string>((Func<softwareupdates.CCM_SoftwareUpdate, string>) (t => t.UpdateID)).ToList<string>())}'\" -namespace \"ROOT\\ccm\\ClientSDK\";([wmiclass]'ROOT\\ccm\\ClientSDK:CCM_SoftwareUpdatesManager').InstallUpdates($a)", true);
  }

  /// <summary>Source:root\ccm\SoftwareUpdates\UpdatesStore</summary>
  public class CCM_UpdateStatus
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwareupdates.CCM_UpdateStatus" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CCM_UpdateStatus(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.Article = WMIObject.Properties[nameof (Article)].Value as string;
      this.Bulletin = WMIObject.Properties[nameof (Bulletin)].Value as string;
      this.Language = WMIObject.Properties[nameof (Language)].Value as string;
      this.ProductID = WMIObject.Properties[nameof (ProductID)].Value as string;
      this.RevisionNumber = WMIObject.Properties[nameof (RevisionNumber)].Value as uint?;
      string scanTimeDmtf = WMIObject.Properties[nameof (ScanTime)].Value as string;
      this.ScanTime = !string.IsNullOrEmpty(scanTimeDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(scanTimeDmtf)) : new DateTime?();
      this.Sources = (softwareupdates.CCM_SourceStatus[]) null;
      this.SourceType = WMIObject.Properties[nameof (SourceType)].Value as uint?;
      this.SourceUniqueId = WMIObject.Properties[nameof (SourceUniqueId)].Value as string;
      this.SourceVersion = WMIObject.Properties[nameof (SourceVersion)].Value as uint?;
      this.Status = WMIObject.Properties[nameof (Status)].Value as string;
      this.Title = WMIObject.Properties[nameof (Title)].Value as string;
      this.UniqueId = WMIObject.Properties[nameof (UniqueId)].Value as string;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    /// <summary>Gets or sets the article.</summary>
    /// <value>The article.</value>
    public string Article { get; set; }

    /// <summary>Gets or sets the bulletin.</summary>
    /// <value>The bulletin.</value>
    public string Bulletin { get; set; }

    /// <summary>Gets or sets the language.</summary>
    /// <value>The language.</value>
    public string Language { get; set; }

    /// <summary>Gets or sets the product identifier.</summary>
    /// <value>The product identifier.</value>
    public string ProductID { get; set; }

    /// <summary>Gets or sets the revision number.</summary>
    /// <value>The revision number.</value>
    public uint? RevisionNumber { get; set; }

    /// <summary>Gets or sets the scan time.</summary>
    /// <value>The scan time.</value>
    public DateTime? ScanTime { get; set; }

    /// <summary>Gets or sets the sources.</summary>
    /// <value>The sources.</value>
    public softwareupdates.CCM_SourceStatus[] Sources { get; set; }

    /// <summary>Gets or sets the type of the source.</summary>
    /// <value>The type of the source.</value>
    public uint? SourceType { get; set; }

    /// <summary>Gets or sets the source unique identifier.</summary>
    /// <value>The source unique identifier.</value>
    public string SourceUniqueId { get; set; }

    /// <summary>Gets or sets the source version.</summary>
    /// <value>The source version.</value>
    public uint? SourceVersion { get; set; }

    /// <summary>Gets or sets the status.</summary>
    /// <value>The status.</value>
    public string Status { get; set; }

    /// <summary>Gets or sets the title.</summary>
    /// <value>The title.</value>
    public string Title { get; set; }

    /// <summary>Gets or sets the unique identifier.</summary>
    /// <value>The unique identifier.</value>
    public string UniqueId { get; set; }
  }

  /// <summary>Source:root\ccm\SoftwareUpdates\UpdatesStore</summary>
  public class CCM_SourceStatus
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwareupdates.CCM_SourceStatus" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CCM_SourceStatus(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.RevisionNumber = WMIObject.Properties[nameof (RevisionNumber)].Value as uint?;
      this.ScanTime = WMIObject.Properties[nameof (ScanTime)].Value as DateTime?;
      this.SourceType = WMIObject.Properties[nameof (SourceType)].Value as uint?;
      this.SourceUniqueId = WMIObject.Properties[nameof (SourceUniqueId)].Value as string;
      this.SourceVersion = WMIObject.Properties[nameof (SourceVersion)].Value as uint?;
      this.Status = WMIObject.Properties[nameof (Status)].Value as string;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    /// <summary>Gets or sets the revision number.</summary>
    /// <value>The revision number.</value>
    public uint? RevisionNumber { get; set; }

    /// <summary>Gets or sets the scan time.</summary>
    /// <value>The scan time.</value>
    public DateTime? ScanTime { get; set; }

    /// <summary>Gets or sets the type of the source.</summary>
    /// <value>The type of the source.</value>
    public uint? SourceType { get; set; }

    /// <summary>Gets or sets the source unique identifier.</summary>
    /// <value>The source unique identifier.</value>
    public string SourceUniqueId { get; set; }

    /// <summary>Gets or sets the source version.</summary>
    /// <value>The source version.</value>
    public uint? SourceVersion { get; set; }

    /// <summary>Gets or sets the status.</summary>
    /// <value>The status.</value>
    public string Status { get; set; }
  }

  /// <summary>Source:ROOT\ccm\SoftwareUpdates\DeploymentAgent</summary>
  public class CCM_AssignmentCompliance
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwareupdates.CCM_AssignmentCompliance" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CCM_AssignmentCompliance(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.AssignmentId = WMIObject.Properties[nameof (AssignmentId)].Value as string;
      this.IsCompliant = WMIObject.Properties[nameof (IsCompliant)].Value as bool?;
      this.Reserved = WMIObject.Properties[nameof (Reserved)].Value as string;
      this.Signature = WMIObject.Properties[nameof (Signature)].Value as string;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    /// <summary>Gets or sets the assignment identifier.</summary>
    /// <value>The assignment identifier.</value>
    public string AssignmentId { get; set; }

    /// <summary>Gets or sets the is compliant.</summary>
    /// <value>The is compliant.</value>
    public bool? IsCompliant { get; set; }

    /// <summary>Gets or sets the reserved.</summary>
    /// <value>The reserved.</value>
    public string Reserved { get; set; }

    /// <summary>Gets or sets the signature.</summary>
    /// <value>The signature.</value>
    public string Signature { get; set; }
  }

  /// <summary>Source:ROOT\ccm\SoftwareUpdates\DeploymentAgent</summary>
  public class CCM_AssignmentJobEx1
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwareupdates.CCM_AssignmentJobEx1" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CCM_AssignmentJobEx1(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.AssignmentId = WMIObject.Properties[nameof (AssignmentId)].Value as string;
      this.AssignmentState = WMIObject.Properties[nameof (AssignmentState)].Value as uint?;
      this.AssignmentSubState = WMIObject.Properties[nameof (AssignmentSubState)].Value as uint?;
      this.AssignmentTrigger = WMIObject.Properties[nameof (AssignmentTrigger)].Value as uint?;
      this.JobId = WMIObject.Properties[nameof (JobId)].Value as string;
      this.ReEvaluateTrigger = WMIObject.Properties[nameof (ReEvaluateTrigger)].Value as uint?;
      this.Reserved = WMIObject.Properties[nameof (Reserved)].Value as string;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    /// <summary>Gets or sets the assignment identifier.</summary>
    /// <value>The assignment identifier.</value>
    public string AssignmentId { get; set; }

    /// <summary>Gets or sets the state of the assignment.</summary>
    /// <value>The state of the assignment.</value>
    public uint? AssignmentState { get; set; }

    /// <summary>Gets or sets the state of the assignment sub.</summary>
    /// <value>The state of the assignment sub.</value>
    public uint? AssignmentSubState { get; set; }

    /// <summary>Gets or sets the assignment trigger.</summary>
    /// <value>The assignment trigger.</value>
    public uint? AssignmentTrigger { get; set; }

    /// <summary>Gets or sets the job identifier.</summary>
    /// <value>The job identifier.</value>
    public string JobId { get; set; }

    /// <summary>Gets or sets the re evaluate trigger.</summary>
    /// <value>The re evaluate trigger.</value>
    public uint? ReEvaluateTrigger { get; set; }

    /// <summary>Gets or sets the reserved.</summary>
    /// <value>The reserved.</value>
    public string Reserved { get; set; }
  }

  /// <summary>Source:ROOT\ccm\SoftwareUpdates\DeploymentAgent</summary>
  public class CCM_ComponentState
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwareupdates.CCM_ComponentState" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CCM_ComponentState(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.ComponentName = WMIObject.Properties[nameof (ComponentName)].Value as string;
      this.MaxPauseDuration = WMIObject.Properties[nameof (MaxPauseDuration)].Value as uint?;
      this.PauseCookie = WMIObject.Properties[nameof (PauseCookie)].Value as uint?;
      string pauseStartTimeDmtf = WMIObject.Properties[nameof (PauseStartTime)].Value as string;
      this.PauseStartTime = !string.IsNullOrEmpty(pauseStartTimeDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(pauseStartTimeDmtf)) : new DateTime?();
      this.Reserved = WMIObject.Properties[nameof (Reserved)].Value as string;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    /// <summary>Gets or sets the name of the component.</summary>
    /// <value>The name of the component.</value>
    public string ComponentName { get; set; }

    /// <summary>Gets or sets the maximum duration of the pause.</summary>
    /// <value>The maximum duration of the pause.</value>
    public uint? MaxPauseDuration { get; set; }

    /// <summary>Gets or sets the pause cookie.</summary>
    /// <value>The pause cookie.</value>
    public uint? PauseCookie { get; set; }

    /// <summary>Gets or sets the pause start time.</summary>
    /// <value>The pause start time.</value>
    public DateTime? PauseStartTime { get; set; }

    /// <summary>Gets or sets the reserved.</summary>
    /// <value>The reserved.</value>
    public string Reserved { get; set; }
  }

  /// <summary>Source:ROOT\ccm\SoftwareUpdates\DeploymentAgent</summary>
  public class CCM_DeploymentTaskEx1
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwareupdates.CCM_DeploymentTaskEx1" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CCM_DeploymentTaskEx1(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.AssignmentId = WMIObject.Properties[nameof (AssignmentId)].Value as string;
      this.DetectJobId = WMIObject.Properties[nameof (DetectJobId)].Value as string;
      this.IsEnforcedInstall = WMIObject.Properties[nameof (IsEnforcedInstall)].Value as bool?;
      this.JobId = WMIObject.Properties[nameof (JobId)].Value as string;
      this.nKey = WMIObject.Properties[nameof (nKey)].Value as uint?;
      this.Reserved = WMIObject.Properties[nameof (Reserved)].Value as string;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    /// <summary>Gets or sets the assignment identifier.</summary>
    /// <value>The assignment identifier.</value>
    public string AssignmentId { get; set; }

    /// <summary>Gets or sets the detect job identifier.</summary>
    /// <value>The detect job identifier.</value>
    public string DetectJobId { get; set; }

    /// <summary>Gets or sets the is enforced install.</summary>
    /// <value>The is enforced install.</value>
    public bool? IsEnforcedInstall { get; set; }

    /// <summary>Gets or sets the job identifier.</summary>
    /// <value>The job identifier.</value>
    public string JobId { get; set; }

    /// <summary>Gets or sets the n key.</summary>
    /// <value>The n key.</value>
    public uint? nKey { get; set; }

    /// <summary>Gets or sets the reserved.</summary>
    /// <value>The reserved.</value>
    public string Reserved { get; set; }
  }

  /// <summary>Source:ROOT\ccm\SoftwareUpdates\DeploymentAgent</summary>
  public class CCM_SUMLocalSettings
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwareupdates.CCM_SUMLocalSettings" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CCM_SUMLocalSettings(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.nKey = WMIObject.Properties[nameof (nKey)].Value as uint?;
      this.Reserved = WMIObject.Properties[nameof (Reserved)].Value as string;
      this.UserExperience = WMIObject.Properties[nameof (UserExperience)].Value as uint?;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    /// <summary>Gets or sets the n key.</summary>
    /// <value>The n key.</value>
    public uint? nKey { get; set; }

    /// <summary>Gets or sets the reserved.</summary>
    /// <value>The reserved.</value>
    public string Reserved { get; set; }

    /// <summary>Gets or sets the user experience.</summary>
    /// <value>The user experience.</value>
    public uint? UserExperience { get; set; }
  }

  /// <summary>Source:ROOT\ccm\SoftwareUpdates\DeploymentAgent</summary>
  public class CCM_TargetedUpdateEx1
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwareupdates.CCM_TargetedUpdateEx1" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CCM_TargetedUpdateEx1(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      string deadlineDmtf = WMIObject.Properties[nameof (Deadline)].Value as string;
      this.Deadline = !string.IsNullOrEmpty(deadlineDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(deadlineDmtf)) : new DateTime?();
      this.DisableMomAlerts = WMIObject.Properties[nameof (DisableMomAlerts)].Value as bool?;
      this.DownloadSize = WMIObject.Properties[nameof (DownloadSize)].Value as uint?;
      this.DPLocality = WMIObject.Properties[nameof (DPLocality)].Value as uint?;
      this.IsDeleted = WMIObject.Properties[nameof (IsDeleted)].Value as bool?;
      this.KeepHidden = WMIObject.Properties[nameof (KeepHidden)].Value as bool?;
      this.LimitStateMessageVerbosity = WMIObject.Properties[nameof (LimitStateMessageVerbosity)].Value as bool?;
      this.OverrideServiceWindows = WMIObject.Properties[nameof (OverrideServiceWindows)].Value as bool?;
      this.PercentComplete = WMIObject.Properties[nameof (PercentComplete)].Value as uint?;
      this.RaiseMomAlertsOnFailure = WMIObject.Properties[nameof (RaiseMomAlertsOnFailure)].Value as bool?;
      string rebootDeadlineDmtf = WMIObject.Properties[nameof (RebootDeadline)].Value as string;
      this.RebootDeadline = !string.IsNullOrEmpty(rebootDeadlineDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(rebootDeadlineDmtf)) : new DateTime?();
      this.RebootOutsideOfServiceWindows = WMIObject.Properties[nameof (RebootOutsideOfServiceWindows)].Value as bool?;
      this.RefAssignments = WMIObject.Properties[nameof (RefAssignments)].Value as string;
      this.Reserved = WMIObject.Properties[nameof (Reserved)].Value as string;
      this.ScheduledUpdateOptions = WMIObject.Properties[nameof (ScheduledUpdateOptions)].Value as uint?;
      string startTimeDmtf = WMIObject.Properties[nameof (StartTime)].Value as string;
      this.StartTime = !string.IsNullOrEmpty(startTimeDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(startTimeDmtf)) : new DateTime?();
      this.UpdateAction = WMIObject.Properties[nameof (UpdateAction)].Value as uint?;
      this.UpdateApplicability = WMIObject.Properties[nameof (UpdateApplicability)].Value as uint?;
      this.UpdateId = WMIObject.Properties[nameof (UpdateId)].Value as string;
      this.UpdateState = WMIObject.Properties[nameof (UpdateState)].Value as uint?;
      this.UpdateStatus = WMIObject.Properties[nameof (UpdateStatus)].Value as uint?;
      this.UpdateVersion = WMIObject.Properties[nameof (UpdateVersion)].Value as string;
      this.UserUIExperience = WMIObject.Properties[nameof (UserUIExperience)].Value as bool?;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    /// <summary>Gets or sets the deadline.</summary>
    /// <value>The deadline.</value>
    public DateTime? Deadline { get; set; }

    /// <summary>Gets or sets the disable mom alerts.</summary>
    /// <value>The disable mom alerts.</value>
    public bool? DisableMomAlerts { get; set; }

    /// <summary>Gets or sets the size of the download.</summary>
    /// <value>The size of the download.</value>
    public uint? DownloadSize { get; set; }

    /// <summary>Gets or sets the dp locality.</summary>
    /// <value>The dp locality.</value>
    public uint? DPLocality { get; set; }

    /// <summary>Gets or sets the is deleted.</summary>
    /// <value>The is deleted.</value>
    public bool? IsDeleted { get; set; }

    /// <summary>Gets or sets the keep hidden.</summary>
    /// <value>The keep hidden.</value>
    public bool? KeepHidden { get; set; }

    /// <summary>Gets or sets the limit state message verbosity.</summary>
    /// <value>The limit state message verbosity.</value>
    public bool? LimitStateMessageVerbosity { get; set; }

    /// <summary>Gets or sets the override service windows.</summary>
    /// <value>The override service windows.</value>
    public bool? OverrideServiceWindows { get; set; }

    /// <summary>Gets or sets the percent complete.</summary>
    /// <value>The percent complete.</value>
    public uint? PercentComplete { get; set; }

    /// <summary>Gets or sets the raise mom alerts on failure.</summary>
    /// <value>The raise mom alerts on failure.</value>
    public bool? RaiseMomAlertsOnFailure { get; set; }

    /// <summary>Gets or sets the reboot deadline.</summary>
    /// <value>The reboot deadline.</value>
    public DateTime? RebootDeadline { get; set; }

    /// <summary>Gets or sets the reboot outside of service windows.</summary>
    /// <value>The reboot outside of service windows.</value>
    public bool? RebootOutsideOfServiceWindows { get; set; }

    /// <summary>Gets or sets the reference assignments.</summary>
    /// <value>The reference assignments.</value>
    public string RefAssignments { get; set; }

    /// <summary>Gets or sets the reserved.</summary>
    /// <value>The reserved.</value>
    public string Reserved { get; set; }

    /// <summary>Gets or sets the scheduled update options.</summary>
    /// <value>The scheduled update options.</value>
    public uint? ScheduledUpdateOptions { get; set; }

    /// <summary>Gets or sets the start time.</summary>
    /// <value>The start time.</value>
    public DateTime? StartTime { get; set; }

    /// <summary>Gets or sets the update action.</summary>
    /// <value>The update action.</value>
    public uint? UpdateAction { get; set; }

    /// <summary>Gets or sets the update applicability.</summary>
    /// <value>The update applicability.</value>
    public uint? UpdateApplicability { get; set; }

    /// <summary>Gets or sets the update identifier.</summary>
    /// <value>The update identifier.</value>
    public string UpdateId { get; set; }

    /// <summary>Gets or sets the state of the update.</summary>
    /// <value>The state of the update.</value>
    public uint? UpdateState { get; set; }

    /// <summary>Gets or sets the update status.</summary>
    /// <value>The update status.</value>
    public uint? UpdateStatus { get; set; }

    /// <summary>Gets or sets the update version.</summary>
    /// <value>The update version.</value>
    public string UpdateVersion { get; set; }

    /// <summary>Gets or sets the user UI experience.</summary>
    /// <value>The user UI experience.</value>
    public bool? UserUIExperience { get; set; }
  }

  /// <summary>Source:ROOT\ccm\SoftwareUpdates\Handler</summary>
  public class CCM_AtomicUpdateEx1
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwareupdates.CCM_AtomicUpdateEx1" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CCM_AtomicUpdateEx1(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.CachedContentInUse = WMIObject.Properties[nameof (CachedContentInUse)].Value as uint?;
      this.ContentPriority = WMIObject.Properties[nameof (ContentPriority)].Value as uint?;
      this.ContentRequestId = WMIObject.Properties[nameof (ContentRequestId)].Value as string;
      this.ExecutionRequestId = WMIObject.Properties[nameof (ExecutionRequestId)].Value as string;
      this.FailureDetail = WMIObject.Properties[nameof (FailureDetail)].Value as string;
      this.Reserved = WMIObject.Properties[nameof (Reserved)].Value as string;
      this.UpdateID = WMIObject.Properties[nameof (UpdateID)].Value as string;
      this.UpdateResult = WMIObject.Properties[nameof (UpdateResult)].Value as uint?;
      this.UpdateState = WMIObject.Properties[nameof (UpdateState)].Value as uint?;
      this.UpdateVersion = WMIObject.Properties[nameof (UpdateVersion)].Value as string;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    /// <summary>Gets or sets the cached content in use.</summary>
    /// <value>The cached content in use.</value>
    public uint? CachedContentInUse { get; set; }

    /// <summary>Gets or sets the content priority.</summary>
    /// <value>The content priority.</value>
    public uint? ContentPriority { get; set; }

    /// <summary>Gets or sets the content request identifier.</summary>
    /// <value>The content request identifier.</value>
    public string ContentRequestId { get; set; }

    /// <summary>Gets or sets the execution request identifier.</summary>
    /// <value>The execution request identifier.</value>
    public string ExecutionRequestId { get; set; }

    /// <summary>Gets or sets the failure detail.</summary>
    /// <value>The failure detail.</value>
    public string FailureDetail { get; set; }

    /// <summary>Gets or sets the reserved.</summary>
    /// <value>The reserved.</value>
    public string Reserved { get; set; }

    /// <summary>Gets or sets the update identifier.</summary>
    /// <value>The update identifier.</value>
    public string UpdateID { get; set; }

    /// <summary>Gets or sets the update result.</summary>
    /// <value>The update result.</value>
    public uint? UpdateResult { get; set; }

    /// <summary>Gets or sets the state of the update.</summary>
    /// <value>The state of the update.</value>
    public uint? UpdateState { get; set; }

    /// <summary>Gets or sets the update version.</summary>
    /// <value>The update version.</value>
    public string UpdateVersion { get; set; }
  }

  /// <summary>Source:ROOT\ccm\SoftwareUpdates\Handler</summary>
  public class CCM_BundledUpdateEx1
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwareupdates.CCM_BundledUpdateEx1" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The ps code.</param>
    public CCM_BundledUpdateEx1(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.CachedContentInUse = WMIObject.Properties[nameof (CachedContentInUse)].Value as uint?;
      this.ContentPriority = WMIObject.Properties[nameof (ContentPriority)].Value as uint?;
      this.ContentRequestId = WMIObject.Properties[nameof (ContentRequestId)].Value as string;
      this.ExecutionRequestId = WMIObject.Properties[nameof (ExecutionRequestId)].Value as string;
      this.FailureDetail = WMIObject.Properties[nameof (FailureDetail)].Value as string;
      this.IsSelfDownloadComplete = WMIObject.Properties[nameof (IsSelfDownloadComplete)].Value as bool?;
      this.IsSelfInstalling = WMIObject.Properties[nameof (IsSelfInstalling)].Value as bool?;
      this.RelatedUpdates = WMIObject.Properties[nameof (RelatedUpdates)].Value as string;
      this.Reserved = WMIObject.Properties[nameof (Reserved)].Value as string;
      this.UpdateID = WMIObject.Properties[nameof (UpdateID)].Value as string;
      this.UpdateResult = WMIObject.Properties[nameof (UpdateResult)].Value as uint?;
      this.UpdateState = WMIObject.Properties[nameof (UpdateState)].Value as uint?;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    /// <summary>Gets or sets the cached content in use.</summary>
    /// <value>The cached content in use.</value>
    public uint? CachedContentInUse { get; set; }

    /// <summary>Gets or sets the content priority.</summary>
    /// <value>The content priority.</value>
    public uint? ContentPriority { get; set; }

    /// <summary>Gets or sets the content request identifier.</summary>
    /// <value>The content request identifier.</value>
    public string ContentRequestId { get; set; }

    /// <summary>Gets or sets the execution request identifier.</summary>
    /// <value>The execution request identifier.</value>
    public string ExecutionRequestId { get; set; }

    /// <summary>Gets or sets the failure detail.</summary>
    /// <value>The failure detail.</value>
    public string FailureDetail { get; set; }

    /// <summary>Gets or sets the is self download complete.</summary>
    /// <value>The is self download complete.</value>
    public bool? IsSelfDownloadComplete { get; set; }

    /// <summary>Gets or sets the is self installing.</summary>
    /// <value>The is self installing.</value>
    public bool? IsSelfInstalling { get; set; }

    /// <summary>Gets or sets the related updates.</summary>
    /// <value>The related updates.</value>
    public string RelatedUpdates { get; set; }

    /// <summary>Gets or sets the reserved.</summary>
    /// <value>The reserved.</value>
    public string Reserved { get; set; }

    /// <summary>Gets or sets the update identifier.</summary>
    /// <value>The update identifier.</value>
    public string UpdateID { get; set; }

    /// <summary>Gets or sets the update result.</summary>
    /// <value>The update result.</value>
    public uint? UpdateResult { get; set; }

    /// <summary>Gets or sets the state of the update.</summary>
    /// <value>The state of the update.</value>
    public uint? UpdateState { get; set; }
  }

  /// <summary>Source:ROOT\ccm\SoftwareUpdates\Handler</summary>
  public class CCM_UpdatesDeploymentJobEx1
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwareupdates.CCM_UpdatesDeploymentJobEx1" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CCM_UpdatesDeploymentJobEx1(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.HandleForMTC = WMIObject.Properties[nameof (HandleForMTC)].Value as string;
      this.IsCompleted = WMIObject.Properties[nameof (IsCompleted)].Value as bool?;
      this.IsOwnerOfMTCTask = WMIObject.Properties[nameof (IsOwnerOfMTCTask)].Value as bool?;
      this.IsRequestSumbittedToMTC = WMIObject.Properties[nameof (IsRequestSumbittedToMTC)].Value as bool?;
      this.JobID = WMIObject.Properties[nameof (JobID)].Value as string;
      this.JobState = WMIObject.Properties[nameof (JobState)].Value as uint?;
      this.JobType = WMIObject.Properties[nameof (JobType)].Value as uint?;
      this.JobUpdates = WMIObject.Properties[nameof (JobUpdates)].Value as string;
      this.NotifyDownloadComplete = WMIObject.Properties[nameof (NotifyDownloadComplete)].Value as string;
      this.Reserved = WMIObject.Properties[nameof (Reserved)].Value as string;
      this.TaskPriorityForMTC = WMIObject.Properties[nameof (TaskPriorityForMTC)].Value as uint?;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    /// <summary>Gets or sets the handle for MTC.</summary>
    /// <value>The handle for MTC.</value>
    public string HandleForMTC { get; set; }

    /// <summary>Gets or sets the is completed.</summary>
    /// <value>The is completed.</value>
    public bool? IsCompleted { get; set; }

    /// <summary>Gets or sets the is owner of MTC task.</summary>
    /// <value>The is owner of MTC task.</value>
    public bool? IsOwnerOfMTCTask { get; set; }

    /// <summary>Gets or sets the is request sumbitted to MTC.</summary>
    /// <value>The is request sumbitted to MTC.</value>
    public bool? IsRequestSumbittedToMTC { get; set; }

    /// <summary>Gets or sets the job identifier.</summary>
    /// <value>The job identifier.</value>
    public string JobID { get; set; }

    /// <summary>Gets or sets the state of the job.</summary>
    /// <value>The state of the job.</value>
    public uint? JobState { get; set; }

    /// <summary>Gets or sets the type of the job.</summary>
    /// <value>The type of the job.</value>
    public uint? JobType { get; set; }

    /// <summary>Gets or sets the job updates.</summary>
    /// <value>The job updates.</value>
    public string JobUpdates { get; set; }

    /// <summary>Gets or sets the notify download complete.</summary>
    /// <value>The notify download complete.</value>
    public string NotifyDownloadComplete { get; set; }

    /// <summary>Gets or sets the reserved.</summary>
    /// <value>The reserved.</value>
    public string Reserved { get; set; }

    /// <summary>Gets or sets the task priority for MTC.</summary>
    /// <value>The task priority for MTC.</value>
    public uint? TaskPriorityForMTC { get; set; }
  }

  /// <summary>Source:ROOT\ccm\SoftwareUpdates\WUAHandler</summary>
  public class CCM_UpdateSource
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwareupdates.CCM_UpdateSource" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CCM_UpdateSource(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.ContentLocation = WMIObject.Properties[nameof (ContentLocation)].Value as string;
      this.ContentType = WMIObject.Properties[nameof (ContentType)].Value as uint?;
      this.ContentVersion = WMIObject.Properties[nameof (ContentVersion)].Value as uint?;
      this.ServiceId = WMIObject.Properties[nameof (ServiceId)].Value as string;
      this.UniqueId = WMIObject.Properties[nameof (UniqueId)].Value as string;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    /// <summary>Gets or sets the content location.</summary>
    /// <value>The content location.</value>
    public string ContentLocation { get; set; }

    /// <summary>Gets or sets the type of the content.</summary>
    /// <value>The type of the content.</value>
    public uint? ContentType { get; set; }

    /// <summary>Gets or sets the content version.</summary>
    /// <value>The content version.</value>
    public uint? ContentVersion { get; set; }

    /// <summary>Gets or sets the service identifier.</summary>
    /// <value>The service identifier.</value>
    public string ServiceId { get; set; }

    /// <summary>Gets or sets the unique identifier.</summary>
    /// <value>The unique identifier.</value>
    public string UniqueId { get; set; }
  }

  /// <summary>Source:ROOT\ccm\ClientSDK</summary>
  public class CCM_SoftwareUpdate : softwaredistribution.CCM_SoftwareUpdate
  {
    internal baseInit oNewBase;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwareupdates.CCM_SoftwareUpdate" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CCM_SoftwareUpdate(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
      this.ArticleID = WMIObject.Properties[nameof (ArticleID)].Value as string;
      this.BulletinID = WMIObject.Properties[nameof (BulletinID)].Value as string;
      this.ComplianceState = WMIObject.Properties[nameof (ComplianceState)].Value as uint?;
      this.ExclusiveUpdate = WMIObject.Properties[nameof (ExclusiveUpdate)].Value as bool?;
      this.OverrideServiceWindows = WMIObject.Properties[nameof (OverrideServiceWindows)].Value as bool?;
      this.RebootOutsideServiceWindows = WMIObject.Properties[nameof (RebootOutsideServiceWindows)].Value as bool?;
      string restartDeadlineDmtf = WMIObject.Properties[nameof (RestartDeadline)].Value as string;
      if (string.IsNullOrEmpty(restartDeadlineDmtf))
      {
        this.RestartDeadline = new DateTime?();
      }
      else
      {
        this.RestartDeadline = new DateTime?(ManagementDateTimeConverter.ToDateTime(restartDeadlineDmtf));
        this.RestartDeadline = new DateTime?(this.RestartDeadline.Value.ToUniversalTime());
      }
      this.UpdateID = WMIObject.Properties[nameof (UpdateID)].Value as string;
      this.URL = WMIObject.Properties[nameof (URL)].Value as string;
      this.UserUIExperience = WMIObject.Properties[nameof (UserUIExperience)].Value as bool?;
    }

    /// <summary>Gets or sets the article identifier.</summary>
    /// <value>The article identifier.</value>
    public new string ArticleID { get; set; }

    /// <summary>Gets or sets the bulletin identifier.</summary>
    /// <value>The bulletin identifier.</value>
    public new string BulletinID { get; set; }

    /// <summary>Gets or sets the state of the compliance.</summary>
    /// <value>The state of the compliance.</value>
    public new uint? ComplianceState { get; set; }

    /// <summary>Gets or sets the exclusive update.</summary>
    /// <value>The exclusive update.</value>
    public new bool? ExclusiveUpdate { get; set; }

    /// <summary>Gets or sets the override service windows.</summary>
    /// <value>The override service windows.</value>
    public new bool? OverrideServiceWindows { get; set; }

    /// <summary>Gets or sets the reboot outside service windows.</summary>
    /// <value>The reboot outside service windows.</value>
    public new bool? RebootOutsideServiceWindows { get; set; }

    /// <summary>Gets or sets the restart deadline.</summary>
    /// <value>The restart deadline.</value>
    public new DateTime? RestartDeadline { get; set; }

    /// <summary>Gets or sets the update identifier.</summary>
    /// <value>The update identifier.</value>
    public new string UpdateID { get; set; }

    /// <summary>Gets or sets the URL.</summary>
    /// <value>The URL.</value>
    public new string URL { get; set; }

    /// <summary>Gets or sets the user UI experience.</summary>
    /// <value>The user UI experience.</value>
    public new bool? UserUIExperience { get; set; }

    /// <summary>Installs this instance.</summary>
    public void Install()
    {
      this.oNewBase.GetObjectsFromPS($"$a = get-wmiobject -query \"SELECT * FROM CCM_SoftwareUpdate WHERE UpdateID='{this.UpdateID}'\" -namespace \"ROOT\\ccm\\ClientSDK\";([wmiclass]'ROOT\\ccm\\ClientSDK:CCM_SoftwareUpdatesManager').InstallUpdates($a)", true);
    }
  }
}
