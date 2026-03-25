// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.functions.softwaredistribution
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Xml;

#nullable disable
namespace sccmclictr.automation.functions;

/// <summary>Template for an empty Class</summary>
public class softwaredistribution : baseInit
{
  internal Runspace remoteRunspace;
  internal TraceSource pSCode;
  internal ccm baseClient;

  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwaredistribution" /> class.
  /// </summary>
  /// <param name="RemoteRunspace">The remote runspace.</param>
  /// <param name="PSCode">The PowerShell code.</param>
  /// <param name="oClient">A CCM Client object.</param>
  public softwaredistribution(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
    : base(RemoteRunspace, PSCode)
  {
    this.remoteRunspace = RemoteRunspace;
    this.pSCode = PSCode;
    this.baseClient = oClient;
  }

  /// <summary>
  /// Get a list of Applications (SELECT * FROM CCM_Application)
  /// </summary>
  public List<softwaredistribution.CCM_Application> Applications => this.Applications_(false);

  /// <summary>
  /// Get a list of Applications (SELECT * FROM CCM_Application)
  /// </summary>
  /// <param name="bReload">enforce reload</param>
  /// <returns>List of CCM_Application</returns>
  public List<softwaredistribution.CCM_Application> Applications_(bool bReload)
  {
    return this.Applications_(bReload, new TimeSpan(0, 3, 0));
  }

  /// <summary>
  /// Get a list of Applications (SELECT * FROM CCM_Application)
  /// </summary>
  /// <param name="bReload">enforce reload</param>
  /// <param name="CacheTime">TTL for Cached items</param>
  /// <returns></returns>
  public List<softwaredistribution.CCM_Application> Applications_(bool bReload, TimeSpan CacheTime)
  {
    List<softwaredistribution.CCM_Application> ccmApplicationList = new List<softwaredistribution.CCM_Application>();
    List<PSObject> psObjectList = new List<PSObject>();
    foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\ClientSDK", "SELECT * FROM CCM_Application", bReload, CacheTime))
    {
      try
      {
        softwaredistribution.CCM_Application ccmApplication = new softwaredistribution.CCM_Application(WMIObject, this.remoteRunspace, this.pSCode);
        ccmApplication.remoteRunspace = this.remoteRunspace;
        ccmApplication.pSCode = this.pSCode;
        ccmApplicationList.Add(ccmApplication);
      }
      catch
      {
      }
    }
    return ccmApplicationList;
  }

  public softwaredistribution.CCM_ApplicationActions ApplicationActions(
    bool bReload,
    TimeSpan CacheTime)
  {
    List<PSObject> psObjectList = new List<PSObject>();
    foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\ClientSDK", "SELECT * FROM CCM_ClientAgentSettings", bReload, CacheTime))
    {
      try
      {
        return new softwaredistribution.CCM_ApplicationActions(WMIObject, this.remoteRunspace, this.pSCode)
        {
          remoteRunspace = this.remoteRunspace,
          pSCode = this.pSCode
        };
      }
      catch
      {
      }
    }
    return (softwaredistribution.CCM_ApplicationActions) null;
  }

  /// <summary>Get a list of Programs</summary>
  public List<softwaredistribution.CCM_Program> Programs
  {
    get
    {
      List<softwaredistribution.CCM_Program> programs = new List<softwaredistribution.CCM_Program>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\ClientSDK", "SELECT * FROM CCM_Program"))
        programs.Add(new softwaredistribution.CCM_Program(WMIObject, this.remoteRunspace, this.pSCode)
        {
          remoteRunspace = this.remoteRunspace,
          pSCode = this.pSCode
        });
      return programs;
    }
  }

  /// <summary>
  /// List of the System Execution History (only Machine based !)
  /// </summary>
  public List<softwaredistribution.REG_ExecutionHistory> ExecutionHistory
  {
    get => this.ExecutionHistory_(false);
  }

  /// <summary>
  /// List of the System Execution History (only Machine based !)
  /// </summary>
  public List<softwaredistribution.REG_ExecutionHistory> ExecutionHistory_(bool bReload)
  {
    List<softwaredistribution.REG_ExecutionHistory> executionHistoryList = new List<softwaredistribution.REG_ExecutionHistory>();
    List<PSObject> psObjectList = new List<PSObject>();
    int sccm2012Flag = this.baseClient.AgentProperties.isSCCM2012 ? 1 : 0;
    bool is64Bit = true;
    if (sccm2012Flag == 0)
      is64Bit = this.baseClient.Inventory.isx64OS;
    if (sccm2012Flag != 0)
      psObjectList = this.GetObjectsFromPS("Get-ChildItem -path \"HKLM:\\SOFTWARE\\Microsoft\\SMS\\Mobile Client\\Software Distribution\\Execution History\" -Recurse | % { get-itemproperty -path  $_.PsPath }", bReload, new TimeSpan(0, 0, 10));
    if (sccm2012Flag == 0 & is64Bit)
      psObjectList = this.GetObjectsFromPS("Get-ChildItem -path \"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\SMS\\Mobile Client\\Software Distribution\\Execution History\" -Recurse | % { get-itemproperty -path  $_.PsPath }", bReload, new TimeSpan(0, 0, 10));
    if (sccm2012Flag == 0 & !is64Bit)
      psObjectList = this.GetObjectsFromPS("Get-ChildItem -path \"HKLM:\\SOFTWARE\\Microsoft\\SMS\\Mobile Client\\Software Distribution\\Execution History\" -Recurse | % { get-itemproperty -path  $_.PsPath }", bReload, new TimeSpan(0, 0, 10));
    foreach (PSObject RegObject in psObjectList)
      executionHistoryList.Add(new softwaredistribution.REG_ExecutionHistory(RegObject, this.remoteRunspace, this.pSCode)
      {
        remoteRunspace = this.remoteRunspace,
        pSCode = this.pSCode
      });
    return executionHistoryList;
  }

  /// <summary>List of Package-Deployments (old Package-Model)</summary>
  public List<softwaredistribution.CCM_SoftwareDistribution> Advertisements
  {
    get => this.Advertisements_(false);
  }

  /// <summary>List of Package-Deployments (old Package-Model)</summary>
  public List<softwaredistribution.CCM_SoftwareDistribution> Advertisements_(bool bReload)
  {
    List<softwaredistribution.CCM_SoftwareDistribution> softwareDistributionList = new List<softwaredistribution.CCM_SoftwareDistribution>();
    foreach (PSObject WMIObject in this.GetObjects("root\\ccm\\policy\\machine\\actualconfig", "SELECT * FROM CCM_SoftwareDistribution", bReload))
    {
      softwaredistribution.CCM_SoftwareDistribution softwareDistribution = new softwaredistribution.CCM_SoftwareDistribution(WMIObject, this.remoteRunspace, this.pSCode);
      softwareDistribution.remoteRunspace = this.remoteRunspace;
      softwareDistribution.pSCode = this.pSCode;
      softwareDistributionList.Add(softwareDistribution);
    }
    return softwareDistributionList;
  }

  /// <summary>List of Applications, Updates and Acvertisements</summary>
  public List<softwaredistribution.SoftwareStatus> SoftwareSummary => this.SoftwareSummary_(false);

  /// <summary>List of Applications, Updates and Acvertisements</summary>
  /// <param name="bReload">enforce a reload, otherwise it will use the data from cache</param>
  /// <returns></returns>
  public List<softwaredistribution.SoftwareStatus> SoftwareSummary_(bool bReload)
  {
    List<softwaredistribution.SoftwareStatus> softwareStatusList = new List<softwaredistribution.SoftwareStatus>();
    List<PSObject> psObjectList = new List<PSObject>();
    foreach (PSObject SWObject in this.GetObjects("ROOT\\ccm\\ClientSDK", "SELECT * FROM CCM_SoftwareBase", bReload))
    {
      try
      {
        softwaredistribution.SoftwareStatus softwareStatus = new softwaredistribution.SoftwareStatus(SWObject, this.remoteRunspace, this.pSCode);
        if (!string.IsNullOrEmpty(softwareStatus.Type))
        {
          softwareStatus.remoteRunspace = this.remoteRunspace;
          softwareStatus.pSCode = this.pSCode;
          softwareStatusList.Add(softwareStatus);
        }
      }
      catch
      {
      }
    }
    return softwareStatusList;
  }

  /// <summary>Get Application from the Catalog</summary>
  /// <param name="AppCatalogURL">e.g. http://CatalogServer/CMApplicationCatalog</param>
  /// <param name="searchFilter"></param>
  /// <returns></returns>
  public List<softwaredistribution.AppDetailView> ApplicationCatalog(
    string AppCatalogURL,
    string searchFilter = "")
  {
    List<softwaredistribution.AppDetailView> appDetailViewList = new List<softwaredistribution.AppDetailView>();
    try
    {
      string responseXml = "";
      if (string.IsNullOrEmpty(AppCatalogURL))
        AppCatalogURL = this.baseClient.AgentProperties.PortalURL;
      string osArchitecture = this.baseClient.Inventory.OSArchitecture;
      string osVersion = this.baseClient.Inventory.OSVersion;
      string osMajorMinorVersion = osVersion.Substring(0, osVersion.LastIndexOf('.'));
      if (string.IsNullOrEmpty(AppCatalogURL))
        return appDetailViewList;
      HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(AppCatalogURL + "/applicationviewservice.asmx");
      httpWebRequest.Headers.Add("SOAPAction", "http://schemas.microsoft.com/5.0.0.0/ConfigurationManager/SoftwareCatalog/Website/GetFilteredApplications");
      httpWebRequest.Headers.Add("Accept-Language", "de-DE");
      httpWebRequest.Headers.Add("request-source", "softwarecenter");
      httpWebRequest.Headers.Add("api-version", "4.0");
      httpWebRequest.ContentType = "text/xml;charset=\"utf-8\"";
      httpWebRequest.Accept = "text/xml";
      httpWebRequest.Method = "POST";
      httpWebRequest.UseDefaultCredentials = true;
      string queryStringElement = "<queryString/>";
      if (!string.IsNullOrEmpty(searchFilter))
        queryStringElement = $"<queryString>{searchFilter}</queryString>";
      string requestXml = $"<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body><GetFilteredApplications xmlns=\"http://schemas.microsoft.com/5.0.0.0/ConfigurationManager/SoftwareCatalog/Website\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><sortBy>Name</sortBy><filterByProperty>IsFeatured</filterByProperty >{queryStringElement}<maximumRows>1000</maximumRows><startRowIndex>0</startRowIndex><sortAscending>true</sortAscending><classicNameFields>PackageProgramName</classicNameFields><useSecondarySort>true</useSecondarySort><fillInIcon>false</fillInIcon><platform><OSVersion>{osMajorMinorVersion}</OSVersion><OSArchitecture>{osArchitecture}</OSArchitecture><OSProductType>1</OSProductType><SMSID/><SspVersion>SWCenter:4.0.0.0</SspVersion><IsClassicAppSupported>true</IsClassicAppSupported></platform></GetFilteredApplications></s:Body></s:Envelope> ";
      XmlDocument requestDoc = new XmlDocument();
      requestDoc.LoadXml(requestXml);
      using (Stream requestStream = httpWebRequest.GetRequestStream())
        requestDoc.Save(requestStream);
      using (WebResponse response = httpWebRequest.GetResponse())
      {
        using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
          responseXml = streamReader.ReadToEnd();
      }
      if (!string.IsNullOrEmpty(responseXml))
      {
        XmlDocument responseDoc = new XmlDocument();
        responseDoc.LoadXml(responseXml);
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(responseDoc.NameTable);
        nsmgr.AddNamespace("ns", "http://schemas.microsoft.com/5.0.0.0/ConfigurationManager/SoftwareCatalog/Website");
        foreach (XmlNode selectNode in responseDoc.SelectNodes("//ns:GetFilteredApplicationsResponse/ns:GetFilteredApplicationsResult/ns:AppDetailView", nsmgr))
        {
          softwaredistribution.AppDetailView appDetailView = new softwaredistribution.AppDetailView();
          foreach (FieldInfo field in appDetailView.GetType().GetFields())
          {
            try
            {
              field.SetValue((object) appDetailView, (object) selectNode[field.Name].InnerText);
            }
            catch
            {
            }
          }
          appDetailViewList.Add(appDetailView);
        }
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
    }
    return appDetailViewList;
  }

  /// <summary>
  /// Get ApplicationCIAssignment's from ROOT\ccm\Policy\Machine\ActualConfig
  /// </summary>
  /// <returns></returns>
  public List<softwaredistribution.CCM_ApplicationCIAssignment> ApplicationCIAssignment()
  {
    return this.ApplicationCIAssignment(false, new TimeSpan(0, 0, 30));
  }

  /// <summary>
  /// Get ApplicationCIAssignment's from ROOT\ccm\Policy\Machine\ActualConfig
  /// </summary>
  /// <param name="bReload">true = do not use cached Version</param>
  /// <returns></returns>
  public List<softwaredistribution.CCM_ApplicationCIAssignment> ApplicationCIAssignment(bool bReload)
  {
    return this.ApplicationCIAssignment(bReload, new TimeSpan(0, 0, 30));
  }

  /// <summary>
  /// Get ApplicationCIAssignment's from ROOT\ccm\Policy\Machine\ActualConfig
  /// </summary>
  /// <param name="bReload">true = do not use cached Version</param>
  /// <param name="CacheTime">TTL for caching</param>
  /// <returns></returns>
  public List<softwaredistribution.CCM_ApplicationCIAssignment> ApplicationCIAssignment(
    bool bReload,
    TimeSpan CacheTime)
  {
    List<softwaredistribution.CCM_ApplicationCIAssignment> applicationCiAssignmentList = new List<softwaredistribution.CCM_ApplicationCIAssignment>();
    List<PSObject> psObjectList = new List<PSObject>();
    foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\Policy\\Machine\\ActualConfig", "SELECT * FROM CCM_ApplicationCIAssignment", bReload, CacheTime))
    {
      try
      {
        applicationCiAssignmentList.Add(new softwaredistribution.CCM_ApplicationCIAssignment(WMIObject, this.remoteRunspace, this.pSCode)
        {
          remoteRunspace = this.remoteRunspace,
          pSCode = this.pSCode
        });
      }
      catch
      {
      }
    }
    return applicationCiAssignmentList;
  }

  /// <summary>ROOT\ccm\ClientSDK:CCM_SoftwareBase</summary>
  public class CCM_SoftwareBase
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    /// <summary>
    /// Look up the localized error message for an error code using SrsResources.dll. Currently always returns en-US result.
    /// </summary>
    /// <param name="errorID"></param>
    /// <returns></returns>
    private static string GetErrorMessage(string errorID)
    {
      string errorMessage = (string) null;
      string environmentVariable = Environment.GetEnvironmentVariable("SMS_ADMIN_UI_PATH");
      if (environmentVariable == null)
        return (string) null;
      string path = Path.Combine(Directory.GetParent(environmentVariable).ToString(), "SrsResources.dll");
      if (!System.IO.File.Exists(path))
        return (string) null;
      try
      {
        errorMessage = Assembly.LoadFile(path).GetType("SrsResources.Localization").GetMethod(nameof (GetErrorMessage)).Invoke((object) null, new object[2]
        {
          (object) errorID,
          (object) "en-US"
        }).ToString();
      }
      catch (Exception ex)
      {
      }
      return errorMessage;
    }

    public uint? ContentSize { get; set; }

    public DateTime? Deadline { get; set; }

    public string Description { get; set; }

    public uint? ErrorCode { get; set; }

    public string ErrorCodeText { get; set; }

    public uint? EstimatedInstallTime { get; set; }

    public uint? EvaluationState { get; set; }

    public string FullName { get; set; }

    public string Name { get; set; }

    public DateTime? NextUserScheduledTime { get; set; }

    public uint? PercentComplete { get; set; }

    public string Publisher { get; set; }

    public uint? Type { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwaredistribution.CCM_SoftwareBase" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    public CCM_SoftwareBase(PSObject WMIObject)
    {
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.ContentSize = WMIObject.Properties[nameof (ContentSize)].Value as uint?;
      string deadlineDmtf = WMIObject.Properties[nameof (Deadline)].Value as string;
      DateTime? utcConverted;
      if (string.IsNullOrEmpty(deadlineDmtf))
      {
        this.Deadline = new DateTime?();
      }
      else
      {
        this.Deadline = new DateTime?(ManagementDateTimeConverter.ToDateTime(deadlineDmtf));
        utcConverted = this.Deadline;
        this.Deadline = new DateTime?(utcConverted.Value.ToUniversalTime());
      }
      this.Description = WMIObject.Properties[nameof (Description)].Value as string;
      this.ErrorCode = WMIObject.Properties[nameof (ErrorCode)].Value as uint?;
      this.ErrorCodeText = softwaredistribution.CCM_SoftwareBase.GetErrorMessage(this.ErrorCode.ToString());
      this.EstimatedInstallTime = WMIObject.Properties[nameof (EstimatedInstallTime)].Value as uint?;
      this.EvaluationState = WMIObject.Properties[nameof (EvaluationState)].Value as uint?;
      this.FullName = WMIObject.Properties[nameof (FullName)].Value as string;
      this.Name = WMIObject.Properties[nameof (Name)].Value as string;
      string nextUserTimeDmtf = WMIObject.Properties[nameof (NextUserScheduledTime)].Value as string;
      if (string.IsNullOrEmpty(nextUserTimeDmtf))
      {
        utcConverted = new DateTime?();
        this.NextUserScheduledTime = utcConverted;
      }
      else
        this.NextUserScheduledTime = new DateTime?(ManagementDateTimeConverter.ToDateTime(nextUserTimeDmtf));
      this.PercentComplete = WMIObject.Properties[nameof (PercentComplete)].Value as uint?;
      this.Publisher = WMIObject.Properties[nameof (Publisher)].Value as string;
      this.Type = WMIObject.Properties[nameof (Type)].Value as uint?;
    }
  }

  /// <summary>Class CCM_AppDeploymentType.</summary>
  public class CCM_AppDeploymentType : softwaredistribution.CCM_SoftwareBase
  {
    /// <summary>Gets or sets the allowed actions.</summary>
    /// <value>The allowed actions.</value>
    public string[] AllowedActions { get; set; }

    /// <summary>Gets or sets the state of the applicability.</summary>
    /// <value>The state of the applicability.</value>
    public string ApplicabilityState { get; set; }

    /// <summary>Gets or sets the dependencies.</summary>
    /// <value>The dependencies.</value>
    public softwaredistribution.CCM_AppDeploymentType[] Dependencies { get; set; }

    /// <summary>Gets or sets the deployment report.</summary>
    /// <value>The deployment report.</value>
    public string DeploymentReport { get; set; }

    /// <summary>Gets or sets the identifier.</summary>
    /// <value>The identifier.</value>
    public string Id { get; set; }

    /// <summary>Gets or sets the state of the install.</summary>
    /// <value>The state of the install.</value>
    public string InstallState { get; set; }

    /// <summary>Gets or sets the last eval time.</summary>
    /// <value>The last eval time.</value>
    public DateTime? LastEvalTime { get; set; }

    /// <summary>Gets or sets the post install action.</summary>
    /// <value>The post install action.</value>
    public string PostInstallAction { get; set; }

    /// <summary>Gets or sets the resolved state.</summary>
    /// <value>The resolved state.</value>
    public string ResolvedState { get; set; }

    /// <summary>Gets or sets the number of retries remaining.</summary>
    /// <value>The number of retries remaining.</value>
    public uint? RetriesRemaining { get; set; }

    /// <summary>Gets or sets the revision.</summary>
    /// <value>The revision.</value>
    public string Revision { get; set; }

    /// <summary>Gets or sets the supersession state.</summary>
    /// <value>The supersession state.</value>
    public string SupersessionState { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwaredistribution.CCM_SoftwareBase" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    public CCM_AppDeploymentType(PSObject WMIObject)
      : base(WMIObject)
    {
      this.WMIObject = WMIObject;
      this.AllowedActions = WMIObject.Properties[nameof (AllowedActions)].Value as string[];
      this.ApplicabilityState = WMIObject.Properties[nameof (ApplicabilityState)].Value as string;
      try
      {
        if (WMIObject.Properties[nameof (Dependencies)].Value == null)
        {
          this.Dependencies = (softwaredistribution.CCM_AppDeploymentType[]) null;
        }
        else
        {
          List<softwaredistribution.CCM_AppDeploymentType> appDeploymentTypeList = new List<softwaredistribution.CCM_AppDeploymentType>();
          foreach (PSObject WMIObject1 in WMIObject.Properties[nameof (Dependencies)].Value as PSObject[])
            appDeploymentTypeList.Add(new softwaredistribution.CCM_AppDeploymentType(WMIObject1));
          this.Dependencies = appDeploymentTypeList.ToArray();
        }
      }
      catch (Exception ex)
      {
        ex.Message.ToString();
      }
      this.DeploymentReport = WMIObject.Properties[nameof (DeploymentReport)].Value as string;
      this.Id = WMIObject.Properties[nameof (Id)].Value as string;
      this.InstallState = WMIObject.Properties[nameof (InstallState)].Value as string;
      string lastEvalDmtf = WMIObject.Properties[nameof (LastEvalTime)].Value as string;
      this.LastEvalTime = !string.IsNullOrEmpty(lastEvalDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(lastEvalDmtf)) : new DateTime?();
      this.PostInstallAction = WMIObject.Properties[nameof (PostInstallAction)].Value as string;
      this.ResolvedState = WMIObject.Properties[nameof (ResolvedState)].Value as string;
      this.RetriesRemaining = WMIObject.Properties[nameof (RetriesRemaining)].Value as uint?;
      this.Revision = WMIObject.Properties[nameof (Revision)].Value as string;
      this.SupersessionState = WMIObject.Properties[nameof (SupersessionState)].Value as string;
    }
  }

  /// <summary>CCM_Application from ROOT\ccm\ClientSDK</summary>
  public class CCM_Application : softwaredistribution.CCM_SoftwareBase
  {
    internal baseInit oNewBase;

    public string[] AllowedActions { get; set; }

    public softwaredistribution.CCM_AppDeploymentType[] AppDTs { get; set; }

    public string ApplicabilityState { get; set; }

    public string ConfigureState { get; set; }

    public string DeploymentReport { get; set; }

    public uint? EnforcePreference { get; set; }

    public string FileTypes { get; set; }

    public string Icon { get; set; }

    public string Id { get; set; }

    public string InformativeUrl { get; set; }

    public string[] InProgressActions { get; set; }

    public string InstallState { get; set; }

    public bool? IsMachineTarget { get; set; }

    public bool? IsPreflightOnly { get; set; }

    public DateTime? LastEvalTime { get; set; }

    public DateTime? LastInstallTime { get; set; }

    public bool? NotifyUser { get; set; }

    public bool? OverrideServiceWindow { get; set; }

    public bool? RebootOutsideServiceWindow { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public string ResolvedState { get; set; }

    public string Revision { get; set; }

    public string SoftwareVersion { get; set; }

    public DateTime? StartTime { get; set; }

    public string SupersessionState { get; set; }

    public bool? UserUIExperience { get; set; }

    /// <summary>
    /// Transalated EvaluationState into text from MSDN (http://msdn.microsoft.com/en-us/library/jj874280.aspx)
    /// </summary>
    public string EvaluationStateText
    {
      get
      {
        uint? evaluationState = this.EvaluationState;
        if (evaluationState.HasValue)
        {
          switch (evaluationState.GetValueOrDefault())
          {
            case 0:
              return "No state information is available.";
            case 1:
              return "Application is enforced to desired/resolved state.";
            case 2:
              return "Application is not required on the client.";
            case 3:
              return "Application is available for enforcement (install or uninstall based on resolved state). Content may/may not have been downloaded.";
            case 4:
              return "Application last failed to enforce (install/uninstall).";
            case 5:
              return "Application is currently waiting for content download to complete.";
            case 6:
              return "Application is currently waiting for content download to complete.";
            case 7:
              return "Application is currently waiting for its dependencies to download.";
            case 8:
              return "Application is currently waiting for a service (maintenance) window.";
            case 9:
              return "Application is currently waiting for a previously pending reboot.";
            case 10:
              return "Application is currently waiting for serialized enforcement.";
            case 11:
              return "Application is currently enforcing dependencies.";
            case 12:
              return "Application is currently enforcing.";
            case 13:
              return "Application install/uninstall enforced and soft reboot is pending.";
            case 14:
              return "Application installed/uninstalled and hard reboot is pending.";
            case 15:
              return "Update is available but pending installation.";
            case 16 /*0x10*/:
              return "Application failed to evaluate.";
            case 17:
              return "Application is currently waiting for an active user session to enforce.";
            case 18:
              return "Application is currently waiting for all users to logoff.";
            case 19:
              return "Application is currently waiting for a user logon.";
            case 20:
              return "Application in progress, waiting for retry.";
            case 21:
              return "Application is waiting for presentation mode to be switched off.";
            case 22:
              return "Application is pre-downloading content (downloading outside of install job).";
            case 23:
              return "Application is pre-downloading dependent content (downloading outside of install job).";
            case 24:
              return "Application download failed (downloading during install job).";
            case 25:
              return "Application pre-downloading failed (downloading outside of install job).";
            case 26:
              return "Download success (downloading during install job).";
            case 27:
              return "Post-enforce evaluation.";
            case 28:
              return "Waiting for network connectivity.";
          }
        }
        return "Unknown state information.";
      }
    }

    /// <summary>Install an Application</summary>
    /// <returns></returns>
    public string Install() => this.Install(softwaredistribution.AppPriority.Normal, false);

    /// <summary>Install an Application</summary>
    /// <param name="AppPriority">Foreground, High, Normal , Low</param>
    /// <param name="isRebootIfNeeded"></param>
    /// <returns></returns>
    public string Install(string AppPriority, bool isRebootIfNeeded)
    {
      if (string.IsNullOrEmpty(AppPriority))
        AppPriority = "Normal";
      string installResult = "";
      this.oNewBase.CallClassMethod("ROOT\\ccm\\ClientSdk:CCM_Application", nameof (Install), $"'{this.Id}', {this.Revision}, ${this.IsMachineTarget.ToString()}, {softwaredistribution.AppEnforcePreference.Immediate.ToString()}, '{AppPriority}', ${isRebootIfNeeded.ToString()}");
      return installResult;
    }

    public string Repair() => this.Repair(softwaredistribution.AppPriority.Normal, false);

    public string Repair(string AppPriority, bool isRebootIfNeeded)
    {
      if (string.IsNullOrEmpty(AppPriority))
        AppPriority = "Normal";
      string repairResult = "";
      this.oNewBase.CallClassMethod("ROOT\\ccm\\ClientSdk:CCM_Application", nameof (Repair), $"'{this.Id}', {this.Revision}, ${this.IsMachineTarget.ToString()}, {softwaredistribution.AppEnforcePreference.Immediate.ToString()}, '{AppPriority}', ${isRebootIfNeeded.ToString()}");
      return repairResult;
    }

    /// <summary>Uninstall an Application</summary>
    /// <returns></returns>
    public string Uninstall() => this.Uninstall(softwaredistribution.AppPriority.Normal, false);

    /// <summary>Uninstall an Application</summary>
    /// <param name="AppPriority">Foreground, High, Normal , Low</param>
    /// <param name="isRebootIfNeeded"></param>
    /// <returns></returns>
    public string Uninstall(string AppPriority, bool isRebootIfNeeded)
    {
      bool hasDeadlineOverride = false;
      List<PSObject> psObjectList = new List<PSObject>();
      List<softwaredistribution.CCM_ApplicationCIAssignment> source = new List<softwaredistribution.CCM_ApplicationCIAssignment>();
      List<PSObject> objects = this.oNewBase.GetObjects("ROOT\\ccm\\Policy\\Machine\\ActualConfig", "SELECT * FROM CCM_ApplicationCIAssignment", true);
      foreach (PSObject WMIObject in objects)
      {
        try
        {
          softwaredistribution.CCM_ApplicationCIAssignment applicationCiAssignment = new softwaredistribution.CCM_ApplicationCIAssignment(WMIObject, this.remoteRunspace, this.pSCode);
          applicationCiAssignment.remoteRunspace = this.remoteRunspace;
          applicationCiAssignment.pSCode = this.pSCode;
          if (((IEnumerable<string>) Array.FindAll<string>(applicationCiAssignment.AssignedCIs, (Predicate<string>) (s => s.IndexOf(this.Id.Split('_')[2]) >= 0))).Count<string>() > 0)
          {
            source.Add(applicationCiAssignment);
            this.oNewBase.SetProperty($"{applicationCiAssignment.__NAMESPACE}:{applicationCiAssignment.__RELPATH.Replace("\"", "`\"")}", "EnforcementDeadline", "$null");
            hasDeadlineOverride = true;
          }
        }
        catch
        {
        }
      }
      if (hasDeadlineOverride)
        Thread.Sleep(2000);
      this.oNewBase.CallClassMethod("ROOT\\ccm\\ClientSdk:CCM_Application", nameof (Uninstall), $"'{this.Id}', {this.Revision}, ${this.IsMachineTarget.ToString()}, {softwaredistribution.AppEnforcePreference.Immediate.ToString()}, '{AppPriority}', ${isRebootIfNeeded.ToString()}");
      if (hasDeadlineOverride)
      {
        Thread.Sleep(1000);
        foreach (PSObject WMIObject in objects)
        {
          try
          {
            softwaredistribution.CCM_ApplicationCIAssignment oApp = new softwaredistribution.CCM_ApplicationCIAssignment(WMIObject, this.remoteRunspace, this.pSCode);
            oApp.remoteRunspace = this.remoteRunspace;
            oApp.pSCode = this.pSCode;
            if (((IEnumerable<string>) Array.FindAll<string>(oApp.AssignedCIs, (Predicate<string>) (s => s.IndexOf(this.Id.Split('_')[2]) >= 0))).Count<string>() > 0)
            {
              foreach (softwaredistribution.CCM_ApplicationCIAssignment applicationCiAssignment in source.Where<softwaredistribution.CCM_ApplicationCIAssignment>((Func<softwaredistribution.CCM_ApplicationCIAssignment, bool>) (t => t.AssignedCIs == oApp.AssignedCIs)))
                this.oNewBase.SetProperty($"{oApp.__NAMESPACE}:{oApp.__RELPATH.Replace("\"", "`\"")}", "EnforcementDeadline", $"\"{applicationCiAssignment.WMIObject.Properties["EnforcementDeadline"].Value.ToString()}\"");
            }
          }
          catch
          {
          }
        }
      }
      return "";
    }

    /// <summary>Cancel a Job -&gt; Does not work !</summary>
    /// <returns></returns>
    public string Cancel()
    {
      return this.oNewBase.CallClassMethod("ROOT\\ccm\\ClientSdk:CCM_Application", nameof (Cancel), $"'{this.Id}', {this.Revision}, ${this.IsMachineTarget.ToString()}").Properties["ReturnValue"].ToString();
    }

    /// <summary>Download Content</summary>
    /// <returns></returns>
    public string DownloadContents()
    {
      return this.oNewBase.CallClassMethod("ROOT\\ccm\\ClientSdk:CCM_Application", nameof (DownloadContents), $"'{this.Id}', {this.Revision}, ${this.IsMachineTarget.ToString()}, 'Low'").Properties["ReturnValue"].ToString();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwaredistribution.CCM_Application" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CCM_Application(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
      : base(WMIObject)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.oNewBase = new baseInit(this.remoteRunspace, this.pSCode);
      this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
      this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
      this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      List<string> stringList = new List<string>();
      foreach (string str in (WMIObject.Properties[nameof (AllowedActions)].Value as PSObject).BaseObject as ArrayList)
        stringList.Add(str);
      this.AllowedActions = stringList.ToArray();
      try
      {
        List<softwaredistribution.CCM_AppDeploymentType> appDeploymentTypeList = new List<softwaredistribution.CCM_AppDeploymentType>();
        if (this.remoteRunspace != null)
        {
          foreach (PSObject property in this.oNewBase.GetProperties("ROOT\\ccm\\clientsdk:" + this.__RELPATH, nameof (AppDTs)))
            appDeploymentTypeList.Add(new softwaredistribution.CCM_AppDeploymentType(property));
        }
        this.AppDTs = appDeploymentTypeList.ToArray();
      }
      catch
      {
      }
      this.ApplicabilityState = WMIObject.Properties[nameof (ApplicabilityState)].Value as string;
      this.ConfigureState = WMIObject.Properties[nameof (ConfigureState)].Value as string;
      this.DeploymentReport = WMIObject.Properties[nameof (DeploymentReport)].Value as string;
      this.EnforcePreference = WMIObject.Properties[nameof (EnforcePreference)].Value as uint?;
      this.FileTypes = WMIObject.Properties[nameof (FileTypes)].Value as string;
      this.Icon = WMIObject.Properties[nameof (Icon)].Value as string;
      this.Id = WMIObject.Properties[nameof (Id)].Value as string;
      this.InformativeUrl = WMIObject.Properties[nameof (InformativeUrl)].Value as string;
      this.InProgressActions = WMIObject.Properties[nameof (InProgressActions)].Value as string[];
      this.InstallState = WMIObject.Properties[nameof (InstallState)].Value as string;
      this.IsMachineTarget = WMIObject.Properties[nameof (IsMachineTarget)].Value as bool?;
      this.IsPreflightOnly = WMIObject.Properties[nameof (IsPreflightOnly)].Value as bool?;
      string lastEvalDmtf = WMIObject.Properties[nameof (LastEvalTime)].Value as string;
      if (string.IsNullOrEmpty(lastEvalDmtf))
      {
        this.LastEvalTime = new DateTime?();
      }
      else
      {
        try
        {
          this.LastEvalTime = new DateTime?(ManagementDateTimeConverter.ToDateTime(lastEvalDmtf));
          this.LastEvalTime = new DateTime?(this.LastEvalTime.Value.ToUniversalTime());
        }
        catch
        {
        }
      }
      string lastInstallDmtf = WMIObject.Properties[nameof (LastInstallTime)].Value as string;
      if (string.IsNullOrEmpty(lastInstallDmtf))
      {
        this.LastInstallTime = new DateTime?();
      }
      else
      {
        try
        {
          this.LastInstallTime = new DateTime?(ManagementDateTimeConverter.ToDateTime(lastInstallDmtf));
          this.LastInstallTime = new DateTime?(this.LastInstallTime.Value.ToUniversalTime());
        }
        catch
        {
        }
      }
      this.NotifyUser = WMIObject.Properties[nameof (NotifyUser)].Value as bool?;
      this.OverrideServiceWindow = WMIObject.Properties[nameof (OverrideServiceWindow)].Value as bool?;
      this.RebootOutsideServiceWindow = WMIObject.Properties[nameof (RebootOutsideServiceWindow)].Value as bool?;
      string releaseDateDmtf = WMIObject.Properties[nameof (ReleaseDate)].Value as string;
      if (string.IsNullOrEmpty(releaseDateDmtf))
      {
        this.ReleaseDate = new DateTime?();
      }
      else
      {
        try
        {
          this.ReleaseDate = new DateTime?(ManagementDateTimeConverter.ToDateTime(releaseDateDmtf));
          this.ReleaseDate = new DateTime?(this.ReleaseDate.Value.ToUniversalTime());
        }
        catch
        {
        }
      }
      this.ResolvedState = WMIObject.Properties[nameof (ResolvedState)].Value as string;
      this.Revision = WMIObject.Properties[nameof (Revision)].Value as string;
      this.SoftwareVersion = WMIObject.Properties[nameof (SoftwareVersion)].Value as string;
      string startTimeDmtf = WMIObject.Properties[nameof (StartTime)].Value as string;
      if (string.IsNullOrEmpty(startTimeDmtf))
      {
        this.StartTime = new DateTime?();
      }
      else
      {
        try
        {
          this.StartTime = new DateTime?(ManagementDateTimeConverter.ToDateTime(startTimeDmtf));
          this.StartTime = new DateTime?(this.StartTime.Value.ToUniversalTime());
        }
        catch
        {
        }
      }
      this.SupersessionState = WMIObject.Properties[nameof (SupersessionState)].Value as string;
      this.UserUIExperience = WMIObject.Properties[nameof (UserUIExperience)].Value as bool?;
    }
  }

  /// <summary>Source:ROOT\ccm\ClientSDK</summary>
  public class CCM_ApplicationActions
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    public CCM_ApplicationActions(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      string nextRevalDmtf = WMIObject.Properties[nameof (NextGlobalRevalTime)].Value as string;
      this.NextGlobalRevalTime = !string.IsNullOrEmpty(nextRevalDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(nextRevalDmtf)) : new DateTime?();
      string nextRetryDmtf = WMIObject.Properties[nameof (NextRetryTime)].Value as string;
      this.NextRetryTime = !string.IsNullOrEmpty(nextRetryDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(nextRetryDmtf)) : new DateTime?();
      string nextServiceWindowDmtf = WMIObject.Properties[nameof (NextServiceWindowTime)].Value as string;
      if (string.IsNullOrEmpty(nextServiceWindowDmtf))
        this.NextServiceWindowTime = new DateTime?();
      else
        this.NextServiceWindowTime = new DateTime?(ManagementDateTimeConverter.ToDateTime(nextServiceWindowDmtf));
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    public DateTime? NextGlobalRevalTime { get; set; }

    public DateTime? NextRetryTime { get; set; }

    public DateTime? NextServiceWindowTime { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_Policy
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwaredistribution.CCM_Policy" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    public CCM_Policy(PSObject WMIObject)
    {
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_SoftwareDistribution : softwaredistribution.CCM_Policy
  {
    internal baseInit oNewBase;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwaredistribution.CCM_SoftwareDistribution" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CCM_SoftwareDistribution(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode)
      : base(WMIObject)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.oNewBase = new baseInit(this.remoteRunspace, this.pSCode);
      this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
      this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
      this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      string activeTimeDmtf = WMIObject.Properties[nameof (ADV_ActiveTime)].Value as string;
      this.ADV_ActiveTime = !string.IsNullOrEmpty(activeTimeDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(activeTimeDmtf)) : new DateTime?();
      this.ADV_ActiveTimeIsGMT = WMIObject.Properties[nameof (ADV_ActiveTimeIsGMT)].Value as bool?;
      this.ADV_ADF_Published = WMIObject.Properties[nameof (ADV_ADF_Published)].Value as bool?;
      this.ADV_ADF_RunNotification = WMIObject.Properties[nameof (ADV_ADF_RunNotification)].Value as bool?;
      this.ADV_AdvertisementID = WMIObject.Properties[nameof (ADV_AdvertisementID)].Value as string;
      string expirationTimeDmtf = WMIObject.Properties[nameof (ADV_ExpirationTime)].Value as string;
      this.ADV_ExpirationTime = !string.IsNullOrEmpty(expirationTimeDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(expirationTimeDmtf)) : new DateTime?();
      this.ADV_ExpirationTimeIsGMT = WMIObject.Properties[nameof (ADV_ExpirationTimeIsGMT)].Value as bool?;
      this.ADV_FirstRunBehavior = WMIObject.Properties[nameof (ADV_FirstRunBehavior)].Value as string;
      this.ADV_MandatoryAssignments = WMIObject.Properties[nameof (ADV_MandatoryAssignments)].Value as bool?;
      this.ADV_ProgramWindowIsGMT = WMIObject.Properties[nameof (ADV_ProgramWindowIsGMT)].Value as bool?;
      string windowStartDmtf = WMIObject.Properties[nameof (ADV_ProgramWindowStartTime)].Value as string;
      this.ADV_ProgramWindowStartTime = !string.IsNullOrEmpty(windowStartDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(windowStartDmtf)) : new DateTime?();
      string windowStopDmtf = WMIObject.Properties[nameof (ADV_ProgramWindowStopTime)].Value as string;
      this.ADV_ProgramWindowStopTime = !string.IsNullOrEmpty(windowStopDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(windowStopDmtf)) : new DateTime?();
      this.ADV_RCF_InstallFromCDOptions = WMIObject.Properties[nameof (ADV_RCF_InstallFromCDOptions)].Value as string;
      this.ADV_RCF_InstallFromLocalDPOptions = WMIObject.Properties[nameof (ADV_RCF_InstallFromLocalDPOptions)].Value as string;
      this.ADV_RCF_InstallFromRemoteDPOptions = WMIObject.Properties[nameof (ADV_RCF_InstallFromRemoteDPOptions)].Value as string;
      this.ADV_RCF_PostponeToAC = WMIObject.Properties[nameof (ADV_RCF_PostponeToAC)].Value as bool?;
      this.ADV_RebootLogoffNotification = WMIObject.Properties[nameof (ADV_RebootLogoffNotification)].Value as bool?;
      this.ADV_RebootLogoffNotificationCountdownDuration = WMIObject.Properties[nameof (ADV_RebootLogoffNotificationCountdownDuration)].Value as uint?;
      this.ADV_RebootLogoffNotificationFinalWindow = WMIObject.Properties[nameof (ADV_RebootLogoffNotificationFinalWindow)].Value as uint?;
      this.ADV_RepeatRunBehavior = WMIObject.Properties[nameof (ADV_RepeatRunBehavior)].Value as string;
      this.ADV_RetryCount = WMIObject.Properties[nameof (ADV_RetryCount)].Value as uint?;
      this.ADV_RetryInterval = WMIObject.Properties[nameof (ADV_RetryInterval)].Value as uint?;
      this.ADV_RunNotificationCountdownDuration = WMIObject.Properties[nameof (ADV_RunNotificationCountdownDuration)].Value as uint?;
      this.PKG_ContentSize = WMIObject.Properties[nameof (PKG_ContentSize)].Value as uint?;
      this.PKG_Language = WMIObject.Properties[nameof (PKG_Language)].Value as string;
      this.PKG_Manufacturer = WMIObject.Properties[nameof (PKG_Manufacturer)].Value as string;
      this.PKG_MIFChecking = WMIObject.Properties[nameof (PKG_MIFChecking)].Value as bool?;
      this.PKG_MifFileName = WMIObject.Properties[nameof (PKG_MifFileName)].Value as string;
      this.PKG_MIFName = WMIObject.Properties[nameof (PKG_MIFName)].Value as string;
      this.PKG_MIFPublisher = WMIObject.Properties[nameof (PKG_MIFPublisher)].Value as string;
      this.PKG_MIFVersion = WMIObject.Properties[nameof (PKG_MIFVersion)].Value as string;
      this.PKG_Name = WMIObject.Properties[nameof (PKG_Name)].Value as string;
      this.PKG_PackageID = WMIObject.Properties[nameof (PKG_PackageID)].Value as string;
      this.PKG_PSF_ContainsSourceFiles = WMIObject.Properties[nameof (PKG_PSF_ContainsSourceFiles)].Value as bool?;
      this.PKG_SourceHash = WMIObject.Properties[nameof (PKG_SourceHash)].Value as string;
      this.PKG_SourceVersion = WMIObject.Properties[nameof (PKG_SourceVersion)].Value as string;
      this.PKG_version = WMIObject.Properties[nameof (PKG_version)].Value as string;
      this.PRG_Category = WMIObject.Properties[nameof (PRG_Category)].Value as string[];
      this.PRG_CommandLine = WMIObject.Properties[nameof (PRG_CommandLine)].Value as string;
      this.PRG_Comment = WMIObject.Properties[nameof (PRG_Comment)].Value as string;
      this.PRG_CustomLogoffReturnCodes = WMIObject.Properties[nameof (PRG_CustomLogoffReturnCodes)].Value as uint?[];
      this.PRG_CustomRebootReturnCodes = WMIObject.Properties[nameof (PRG_CustomRebootReturnCodes)].Value as uint?[];
      this.PRG_CustomSuccessReturnCodes = WMIObject.Properties[nameof (PRG_CustomSuccessReturnCodes)].Value as uint?[];
      this.PRG_DependentPolicy = WMIObject.Properties[nameof (PRG_DependentPolicy)].Value as bool?;
      this.PRG_DependentProgramPackageID = WMIObject.Properties[nameof (PRG_DependentProgramPackageID)].Value as string;
      this.PRG_DependentProgramProgramID = WMIObject.Properties[nameof (PRG_DependentProgramProgramID)].Value as string;
      this.PRG_DiskSpaceReq = WMIObject.Properties[nameof (PRG_DiskSpaceReq)].Value as string;
      this.PRG_DriveLetter = WMIObject.Properties[nameof (PRG_DriveLetter)].Value as string;
      this.PRG_ForceDependencyRun = WMIObject.Properties[nameof (PRG_ForceDependencyRun)].Value as bool?;
      this.PRG_HistoryLocation = WMIObject.Properties[nameof (PRG_HistoryLocation)].Value as string;
      this.PRG_MaxDuration = WMIObject.Properties[nameof (PRG_MaxDuration)].Value as uint?;
      this.PRG_PRF_AfterRunning = WMIObject.Properties[nameof (PRG_PRF_AfterRunning)].Value as string;
      this.PRG_PRF_Disabled = WMIObject.Properties[nameof (PRG_PRF_Disabled)].Value as bool?;
      this.PRG_PRF_InstallsApplication = WMIObject.Properties[nameof (PRG_PRF_InstallsApplication)].Value as bool?;
      this.PRG_PRF_MappedDriveRequired = WMIObject.Properties[nameof (PRG_PRF_MappedDriveRequired)].Value as bool?;
      this.PRG_PRF_PersistMappedDrive = WMIObject.Properties[nameof (PRG_PRF_PersistMappedDrive)].Value as bool?;
      this.PRG_PRF_RunNotification = WMIObject.Properties[nameof (PRG_PRF_RunNotification)].Value as bool?;
      this.PRG_PRF_RunWithAdminRights = WMIObject.Properties[nameof (PRG_PRF_RunWithAdminRights)].Value as bool?;
      this.PRG_PRF_ShowWindow = WMIObject.Properties[nameof (PRG_PRF_ShowWindow)].Value as string;
      this.PRG_PRF_UserInputRequired = WMIObject.Properties[nameof (PRG_PRF_UserInputRequired)].Value as bool?;
      this.PRG_PRF_UserLogonRequirement = WMIObject.Properties[nameof (PRG_PRF_UserLogonRequirement)].Value as string;
      this.PRG_ProgramID = WMIObject.Properties[nameof (PRG_ProgramID)].Value as string;
      this.PRG_ProgramName = WMIObject.Properties[nameof (PRG_ProgramName)].Value as string;
      this.PRG_Requirements = WMIObject.Properties[nameof (PRG_Requirements)].Value as string;
      this.PRG_ReturnCodesSource = WMIObject.Properties[nameof (PRG_ReturnCodesSource)].Value as string;
      this.PRG_WorkingDirectory = WMIObject.Properties[nameof (PRG_WorkingDirectory)].Value as string;
      this._RawObject = WMIObject;
    }

    public DateTime? ADV_ActiveTime { get; set; }

    public bool? ADV_ActiveTimeIsGMT { get; set; }

    public bool? ADV_ADF_Published { get; set; }

    public bool? ADV_ADF_RunNotification { get; set; }

    public string ADV_AdvertisementID { get; set; }

    public DateTime? ADV_ExpirationTime { get; set; }

    public bool? ADV_ExpirationTimeIsGMT { get; set; }

    public string ADV_FirstRunBehavior { get; set; }

    public bool? ADV_MandatoryAssignments { get; set; }

    public bool? ADV_ProgramWindowIsGMT { get; set; }

    public DateTime? ADV_ProgramWindowStartTime { get; set; }

    public DateTime? ADV_ProgramWindowStopTime { get; set; }

    public string ADV_RCF_InstallFromCDOptions { get; set; }

    public string ADV_RCF_InstallFromLocalDPOptions { get; set; }

    public string ADV_RCF_InstallFromRemoteDPOptions { get; set; }

    public bool? ADV_RCF_PostponeToAC { get; set; }

    public bool? ADV_RebootLogoffNotification { get; set; }

    public uint? ADV_RebootLogoffNotificationCountdownDuration { get; set; }

    public uint? ADV_RebootLogoffNotificationFinalWindow { get; set; }

    public string ADV_RepeatRunBehavior { get; set; }

    public uint? ADV_RetryCount { get; set; }

    public uint? ADV_RetryInterval { get; set; }

    public uint? ADV_RunNotificationCountdownDuration { get; set; }

    public uint? PKG_ContentSize { get; set; }

    public string PKG_Language { get; set; }

    public string PKG_Manufacturer { get; set; }

    public bool? PKG_MIFChecking { get; set; }

    public string PKG_MifFileName { get; set; }

    public string PKG_MIFName { get; set; }

    public string PKG_MIFPublisher { get; set; }

    public string PKG_MIFVersion { get; set; }

    public string PKG_Name { get; set; }

    public string PKG_PackageID { get; set; }

    public bool? PKG_PSF_ContainsSourceFiles { get; set; }

    public string PKG_SourceHash { get; set; }

    public string PKG_SourceVersion { get; set; }

    public string PKG_version { get; set; }

    public string[] PRG_Category { get; set; }

    public string PRG_CommandLine { get; set; }

    public string PRG_Comment { get; set; }

    public uint?[] PRG_CustomLogoffReturnCodes { get; set; }

    public uint?[] PRG_CustomRebootReturnCodes { get; set; }

    public uint?[] PRG_CustomSuccessReturnCodes { get; set; }

    public bool? PRG_DependentPolicy { get; set; }

    public string PRG_DependentProgramPackageID { get; set; }

    public string PRG_DependentProgramProgramID { get; set; }

    public string PRG_DiskSpaceReq { get; set; }

    public string PRG_DriveLetter { get; set; }

    public bool? PRG_ForceDependencyRun { get; set; }

    public string PRG_HistoryLocation { get; set; }

    public uint? PRG_MaxDuration { get; set; }

    public string PRG_PRF_AfterRunning { get; set; }

    public bool? PRG_PRF_Disabled { get; set; }

    public bool? PRG_PRF_InstallsApplication { get; set; }

    public bool? PRG_PRF_MappedDriveRequired { get; set; }

    public bool? PRG_PRF_PersistMappedDrive { get; set; }

    public bool? PRG_PRF_RunNotification { get; set; }

    public bool? PRG_PRF_RunWithAdminRights { get; set; }

    public string PRG_PRF_ShowWindow { get; set; }

    public bool? PRG_PRF_UserInputRequired { get; set; }

    public string PRG_PRF_UserLogonRequirement { get; set; }

    public string PRG_ProgramID { get; set; }

    public string PRG_ProgramName { get; set; }

    public string PRG_Requirements { get; set; }

    public string PRG_ReturnCodesSource { get; set; }

    public string PRG_WorkingDirectory { get; set; }

    public PSObject _RawObject { get; set; }

    /// <summary>CCM_Scheduler_ScheduledMessage object</summary>
    /// <returns></returns>
    public softwaredistribution.CCM_Scheduler_ScheduledMessage _ScheduledMessage()
    {
      try
      {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(this.PRG_Requirements);
        string str = xmlDocument.SelectSingleNode("/SWDReserved/ScheduledMessageID").InnerText.ToString();
        foreach (PSObject WMIObject in this.oNewBase.GetObjects(this.WMIObject.Properties["__NAMESPACE"].Value.ToString(), $"SELECT * FROM CCM_Scheduler_ScheduledMessage WHERE ScheduledMessageID='{str}'"))
        {
          try
          {
            softwaredistribution.CCM_Scheduler_ScheduledMessage scheduledMessage = new softwaredistribution.CCM_Scheduler_ScheduledMessage(WMIObject, this.remoteRunspace, this.pSCode);
            scheduledMessage.remoteRunspace = this.remoteRunspace;
            scheduledMessage.pSCode = this.pSCode;
            return scheduledMessage;
          }
          catch
          {
          }
        }
      }
      catch (Exception ex)
      {
        Trace.TraceError(ex.Message);
      }
      return (softwaredistribution.CCM_Scheduler_ScheduledMessage) null;
    }

    /// <summary>Run (trigger) an advertisement</summary>
    public void TriggerSchedule(bool enforce)
    {
      try
      {
        string scheduledMessageId = this._ScheduledMessage().ScheduledMessageID;
        if (enforce)
        {
          string enforcedRequirements = this.PRG_Requirements.Replace("<OverrideServiceWindows>FALSE</OverrideServiceWindows>", "<OverrideServiceWindows>TRUE</OverrideServiceWindows>");
          string requirementsForRestore = enforcedRequirements;
          string sanitizedRequirements = enforcedRequirements.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("'", "''");
          try
          {
            this.oNewBase.SetProperty($"{this.__NAMESPACE}:{this.__RELPATH.Replace("\"", "'")}", "ADV_RepeatRunBehavior", "'RerunAlways'");
          }
          catch
          {
          }
          try
          {
            this.oNewBase.SetProperty($"{this.__NAMESPACE}:{this.__RELPATH.Replace("\"", "'")}", "ADV_MandatoryAssignments", "$True");
          }
          catch
          {
          }
          try
          {
            this.oNewBase.SetProperty($"{this.__NAMESPACE}:{this.__RELPATH.Replace("\"", "'")}", "PRG_Requirements", $"'{sanitizedRequirements}'");
          }
          catch (Exception ex)
          {
            Trace.TraceError(ex.Message);
          }
          this.ADV_RepeatRunBehavior = "RerunAlways";
          this.ADV_MandatoryAssignments = new bool?(true);
          this.PRG_Requirements = requirementsForRestore;
          Thread.Sleep(500);
        }
        this.oNewBase.CallClassMethod("ROOT\\ccm:SMS_Client", nameof (TriggerSchedule), $"'{scheduledMessageId}'");
      }
      catch (Exception ex)
      {
        Trace.TraceError(ex.Message);
      }
    }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_TaskSequence : softwaredistribution.CCM_SoftwareDistribution
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwaredistribution.CCM_SoftwareDistribution" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CCM_TaskSequence(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
      : base(WMIObject, RemoteRunspace, PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
      this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
      this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.Reserved = WMIObject.Properties[nameof (Reserved)].Value as string;
      this.TS_BootImageID = WMIObject.Properties[nameof (TS_BootImageID)].Value as string;
      string tsDeadlineDmtf = WMIObject.Properties[nameof (TS_Deadline)].Value as string;
      this.TS_Deadline = !string.IsNullOrEmpty(tsDeadlineDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(tsDeadlineDmtf)) : new DateTime?();
      this.TS_MandatoryCountdown = WMIObject.Properties[nameof (TS_MandatoryCountdown)].Value as uint?;
      this.TS_PopupReminderInterval = WMIObject.Properties[nameof (TS_PopupReminderInterval)].Value as uint?;
      this.TS_References = WMIObject.Properties[nameof (TS_References)].Value as string[];
      this.TS_Sequence = WMIObject.Properties[nameof (TS_Sequence)].Value as string;
      this.TS_Type = WMIObject.Properties[nameof (TS_Type)].Value as uint?;
      this.TS_UserNotificationFlags = WMIObject.Properties[nameof (TS_UserNotificationFlags)].Value as uint?;
    }

    public string Reserved { get; set; }

    public string TS_BootImageID { get; set; }

    public DateTime? TS_Deadline { get; set; }

    public uint? TS_MandatoryCountdown { get; set; }

    public uint? TS_PopupReminderInterval { get; set; }

    public string[] TS_References { get; set; }

    public string TS_Sequence { get; set; }

    public uint? TS_Type { get; set; }

    public uint? TS_UserNotificationFlags { get; set; }
  }

  /// <summary>Source:ROOT\ccm\ClientSDK</summary>
  public class CCM_Program : softwaredistribution.CCM_SoftwareBase
  {
    internal new Runspace remoteRunspace;
    internal new TraceSource pSCode;

    /// <summary>Constructor</summary>
    /// <param name="WMIObject"></param>
    /// <param name="RemoteRunspace"></param>
    /// <param name="PSCode"></param>
    public CCM_Program(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
      : base(WMIObject)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      string activationDmtf = WMIObject.Properties[nameof (ActivationTime)].Value as string;
      this.ActivationTime = !string.IsNullOrEmpty(activationDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(activationDmtf)) : new DateTime?();
      this.AdvertisedDirectly = WMIObject.Properties[nameof (AdvertisedDirectly)].Value as bool?;
      this.Categories = WMIObject.Properties[nameof (Categories)].Value as string[];
      this.CompletionAction = WMIObject.Properties[nameof (CompletionAction)].Value as uint?;
      this.Dependencies = WMIObject.Properties[nameof (Dependencies)].Value as softwaredistribution.CCM_Program[];
      this.DependentPackageID = WMIObject.Properties[nameof (DependentPackageID)].Value as string;
      this.DependentProgramID = WMIObject.Properties[nameof (DependentProgramID)].Value as string;
      this.DiskSpaceRequired = WMIObject.Properties[nameof (DiskSpaceRequired)].Value as string;
      this.Duration = WMIObject.Properties[nameof (Duration)].Value as uint?;
      string expirationDmtf = WMIObject.Properties[nameof (ExpirationTime)].Value as string;
      this.ExpirationTime = !string.IsNullOrEmpty(expirationDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(expirationDmtf)) : new DateTime?();
      this.ForceDependencyToRun = WMIObject.Properties[nameof (ForceDependencyToRun)].Value as bool?;
      this.HighImpact = WMIObject.Properties[nameof (HighImpact)].Value as bool?;
      this.LastExitCode = WMIObject.Properties[nameof (LastExitCode)].Value as uint?;
      this.LastRunStatus = WMIObject.Properties[nameof (LastRunStatus)].Value as string;
      string lastRunDmtf = WMIObject.Properties[nameof (LastRunTime)].Value as string;
      this.LastRunTime = !string.IsNullOrEmpty(lastRunDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(lastRunDmtf)) : new DateTime?();
      this.Level = WMIObject.Properties[nameof (Level)].Value as uint?;
      this.NotifyUser = WMIObject.Properties[nameof (NotifyUser)].Value as bool?;
      this.PackageID = WMIObject.Properties[nameof (PackageID)].Value as string;
      this.PackageLanguage = WMIObject.Properties[nameof (PackageLanguage)].Value as string;
      this.PackageName = WMIObject.Properties[nameof (PackageName)].Value as string;
      this.ProgramID = WMIObject.Properties[nameof (ProgramID)].Value as string;
      this.Published = WMIObject.Properties[nameof (Published)].Value as bool?;
      this.RepeatRunBehavior = WMIObject.Properties[nameof (RepeatRunBehavior)].Value as string;
      this.RequiresUserInput = WMIObject.Properties[nameof (RequiresUserInput)].Value as bool?;
      this.RunAtLogoff = WMIObject.Properties[nameof (RunAtLogoff)].Value as bool?;
      this.RunAtLogon = WMIObject.Properties[nameof (RunAtLogon)].Value as bool?;
      this.RunDependent = WMIObject.Properties[nameof (RunDependent)].Value as bool?;
      this.TaskSequence = WMIObject.Properties[nameof (TaskSequence)].Value as bool?;
      this.Version = WMIObject.Properties[nameof (Version)].Value as string;
    }

    internal new string __CLASS { get; set; }

    internal new string __NAMESPACE { get; set; }

    internal new bool __INSTANCE { get; set; }

    internal new string __RELPATH { get; set; }

    internal new PSObject WMIObject { get; set; }

    public DateTime? ActivationTime { get; set; }

    public bool? AdvertisedDirectly { get; set; }

    public string[] Categories { get; set; }

    public uint? CompletionAction { get; set; }

    public softwaredistribution.CCM_Program[] Dependencies { get; set; }

    public string DependentPackageID { get; set; }

    public string DependentProgramID { get; set; }

    public string DiskSpaceRequired { get; set; }

    public uint? Duration { get; set; }

    public DateTime? ExpirationTime { get; set; }

    public bool? ForceDependencyToRun { get; set; }

    public bool? HighImpact { get; set; }

    public uint? LastExitCode { get; set; }

    public string LastRunStatus { get; set; }

    public DateTime? LastRunTime { get; set; }

    public uint? Level { get; set; }

    public bool? NotifyUser { get; set; }

    public string PackageID { get; set; }

    public string PackageLanguage { get; set; }

    public string PackageName { get; set; }

    public string ProgramID { get; set; }

    public bool? Published { get; set; }

    public string RepeatRunBehavior { get; set; }

    public bool? RequiresUserInput { get; set; }

    public bool? RunAtLogoff { get; set; }

    public bool? RunAtLogon { get; set; }

    public bool? RunDependent { get; set; }

    public bool? TaskSequence { get; set; }

    public string Version { get; set; }

    /// <summary>
    /// Transalated EvaluationState into text from MSDN (http://msdn.microsoft.com/en-us/library/jj874280.aspx)
    /// </summary>
    public string EvaluationStateText => "Unknown state information.";
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_Scheduler_ScheduledMessage : softwaredistribution.CCM_Policy
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwaredistribution.CCM_Scheduler_ScheduledMessage" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CCM_Scheduler_ScheduledMessage(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode)
      : base(WMIObject)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
      this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
      this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.ActiveMessage = WMIObject.Properties[nameof (ActiveMessage)].Value as string;
      string activeTimeDmtf = WMIObject.Properties[nameof (ActiveTime)].Value as string;
      this.ActiveTime = !string.IsNullOrEmpty(activeTimeDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(activeTimeDmtf)) : new DateTime?();
      this.ActiveTimeIsGMT = WMIObject.Properties[nameof (ActiveTimeIsGMT)].Value as bool?;
      this.DeliverMode = WMIObject.Properties[nameof (DeliverMode)].Value as string;
      this.ExpireMessage = WMIObject.Properties[nameof (ExpireMessage)].Value as string;
      string expireTimeDmtf = WMIObject.Properties[nameof (ExpireTime)].Value as string;
      this.ExpireTime = !string.IsNullOrEmpty(expireTimeDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(expireTimeDmtf)) : new DateTime?();
      this.ExpireTimeIsGMT = WMIObject.Properties[nameof (ExpireTimeIsGMT)].Value as bool?;
      this.MessageName = WMIObject.Properties[nameof (MessageName)].Value as string;
      this.MessageTimeout = WMIObject.Properties[nameof (MessageTimeout)].Value as string;
      this.ReplyToEndpoint = WMIObject.Properties[nameof (ReplyToEndpoint)].Value as string;
      this.ScheduledMessageID = WMIObject.Properties[nameof (ScheduledMessageID)].Value as string;
      this.TargetEndpoint = WMIObject.Properties[nameof (TargetEndpoint)].Value as string;
      this.TriggerMessage = WMIObject.Properties[nameof (TriggerMessage)].Value as string;
      this.Triggers = WMIObject.Properties[nameof (Triggers)].Value as string[];
      this._RawObject = WMIObject;
    }

    public string ActiveMessage { get; set; }

    public DateTime? ActiveTime { get; set; }

    public bool? ActiveTimeIsGMT { get; set; }

    public string DeliverMode { get; set; }

    public string ExpireMessage { get; set; }

    public DateTime? ExpireTime { get; set; }

    public bool? ExpireTimeIsGMT { get; set; }

    public string MessageName { get; set; }

    public string MessageTimeout { get; set; }

    public string ReplyToEndpoint { get; set; }

    public string ScheduledMessageID { get; set; }

    public string TargetEndpoint { get; set; }

    public string TriggerMessage { get; set; }

    public string[] Triggers { get; set; }

    public PSObject _RawObject { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Scheduler</summary>
  public class CCM_Scheduler_History
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwaredistribution.CCM_Scheduler_History" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CCM_Scheduler_History(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      string activationSentDmtf = WMIObject.Properties[nameof (ActivationMessageSent)].Value as string;
      this.ActivationMessageSent = !string.IsNullOrEmpty(activationSentDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(activationSentDmtf)) : new DateTime?();
      this.ActivationMessageSentIsGMT = WMIObject.Properties[nameof (ActivationMessageSentIsGMT)].Value as bool?;
      string expirationSentDmtf = WMIObject.Properties[nameof (ExpirationMessageSent)].Value as string;
      this.ExpirationMessageSent = !string.IsNullOrEmpty(expirationSentDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(expirationSentDmtf)) : new DateTime?();
      this.ExpirationMessageSentIsGMT = WMIObject.Properties[nameof (ExpirationMessageSentIsGMT)].Value as bool?;
      string firstEvalDmtf = WMIObject.Properties[nameof (FirstEvalTime)].Value as string;
      this.FirstEvalTime = !string.IsNullOrEmpty(firstEvalDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(firstEvalDmtf)) : new DateTime?();
      string lastTriggerDmtf = WMIObject.Properties[nameof (LastTriggerTime)].Value as string;
      this.LastTriggerTime = !string.IsNullOrEmpty(lastTriggerDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(lastTriggerDmtf)) : new DateTime?();
      this.ScheduleID = WMIObject.Properties[nameof (ScheduleID)].Value as string;
      this.TriggerState = WMIObject.Properties[nameof (TriggerState)].Value as string;
      this.UserSID = WMIObject.Properties[nameof (UserSID)].Value as string;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    public DateTime? ActivationMessageSent { get; set; }

    public bool? ActivationMessageSentIsGMT { get; set; }

    public DateTime? ExpirationMessageSent { get; set; }

    public bool? ExpirationMessageSentIsGMT { get; set; }

    public DateTime? FirstEvalTime { get; set; }

    public DateTime? LastTriggerTime { get; set; }

    public string ScheduleID { get; set; }

    public string TriggerState { get; set; }

    public string UserSID { get; set; }
  }

  /// <summary>Source:ROOT\ccm\ClientSDK</summary>
  public class CCM_SoftwareUpdate : softwaredistribution.CCM_SoftwareBase
  {
    internal new Runspace remoteRunspace;
    internal new TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwaredistribution.CCM_SoftwareUpdate" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CCM_SoftwareUpdate(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
      : base(WMIObject)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.ArticleID = WMIObject.Properties[nameof (ArticleID)].Value as string;
      this.BulletinID = WMIObject.Properties[nameof (BulletinID)].Value as string;
      this.ComplianceState = WMIObject.Properties[nameof (ComplianceState)].Value as uint?;
      this.ExclusiveUpdate = WMIObject.Properties[nameof (ExclusiveUpdate)].Value as bool?;
      this.MaxExecutionTime = WMIObject.Properties[nameof (MaxExecutionTime)].Value as uint?;
      this.NotifyUser = WMIObject.Properties[nameof (NotifyUser)].Value as bool?;
      this.OverrideServiceWindows = WMIObject.Properties[nameof (OverrideServiceWindows)].Value as bool?;
      this.RebootOutsideServiceWindows = WMIObject.Properties[nameof (RebootOutsideServiceWindows)].Value as bool?;
      string restartDeadlineDmtf = WMIObject.Properties[nameof (RestartDeadline)].Value as string;
      this.RestartDeadline = !string.IsNullOrEmpty(restartDeadlineDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(restartDeadlineDmtf)) : new DateTime?();
      string startTimeDmtf = WMIObject.Properties[nameof (StartTime)].Value as string;
      this.StartTime = !string.IsNullOrEmpty(startTimeDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(startTimeDmtf)) : new DateTime?();
      this.UpdateID = WMIObject.Properties[nameof (UpdateID)].Value as string;
      this.URL = WMIObject.Properties[nameof (URL)].Value as string;
      this.UserUIExperience = WMIObject.Properties[nameof (UserUIExperience)].Value as bool?;
    }

    internal new string __CLASS { get; set; }

    internal new string __NAMESPACE { get; set; }

    internal new bool __INSTANCE { get; set; }

    internal new string __RELPATH { get; set; }

    internal new PSObject WMIObject { get; set; }

    public string ArticleID { get; set; }

    public string BulletinID { get; set; }

    public uint? ComplianceState { get; set; }

    public bool? ExclusiveUpdate { get; set; }

    public uint? MaxExecutionTime { get; set; }

    public bool? NotifyUser { get; set; }

    public bool? OverrideServiceWindows { get; set; }

    public bool? RebootOutsideServiceWindows { get; set; }

    public DateTime? RestartDeadline { get; set; }

    public DateTime? StartTime { get; set; }

    public string UpdateID { get; set; }

    public string URL { get; set; }

    public bool? UserUIExperience { get; set; }

    /// <summary>
    /// Transalated EvaluationState into text from MSDN (http://msdn.microsoft.com/en-us/library/jj874280.aspx)
    /// </summary>
    public string EvaluationStateText
    {
      get
      {
        uint? evaluationState = this.EvaluationState;
        if (evaluationState.HasValue)
        {
          switch (evaluationState.GetValueOrDefault())
          {
            case 0:
              return "ciJobStateNone";
            case 1:
              return "ciJobStateAvailable";
            case 2:
              return "ciJobStateSubmitted";
            case 3:
              return "ciJobStateDetecting";
            case 4:
              return "ciJobStatePreDownload";
            case 5:
              return "ciJobStateDownloading";
            case 6:
              return "ciJobStateWaitInstall";
            case 7:
              return "ciJobStateInstalling";
            case 8:
              return "ciJobStatePendingSoftReboot";
            case 9:
              return "ciJobStatePendingHardReboot";
            case 10:
              return "ciJobStateWaitReboot";
            case 11:
              return "ciJobStateVerifying";
            case 12:
              return "ciJobStateInstallComplete";
            case 13:
              return "ciJobStateError";
            case 14:
              return "ciJobStateWaitServiceWindow";
            case 15:
              return "ciJobStateWaitUserLogon";
            case 16 /*0x10*/:
              return "ciJobStateWaitUserLogoff";
            case 17:
              return "ciJobStateWaitJobUserLogon";
            case 18:
              return "ciJobStateWaitUserReconnect";
            case 19:
              return "ciJobStatePendingUserLogoff";
            case 20:
              return "ciJobStatePendingUpdate";
            case 21:
              return "ciJobStateWaitingRetry";
            case 22:
              return "ciJobStateWaitPresModeOff";
            case 23:
              return "ciJobStateWaitForOrchestration";
          }
        }
        return "Unknown state information.";
      }
    }
  }

  /// <summary>Application Priorities</summary>
  public static class AppPriority
  {
    /// <summary>Gets the low AppPriority.</summary>
    /// <value>The low AppPriority.</value>
    public static string Low => nameof (Low);

    /// <summary>Gets the normal AppPriority.</summary>
    /// <value>The normal AppPriority.</value>
    public static string Normal => nameof (Normal);

    /// <summary>Gets the high AppPriority.</summary>
    /// <value>The high AppPriority.</value>
    public static string High => nameof (High);

    /// <summary>Gets the foreground AppPriority.</summary>
    /// <value>The foreground AppPriority.</value>
    public static string Foreground => nameof (Foreground);
  }

  /// <summary>AppEnforcePreference</summary>
  public static class AppEnforcePreference
  {
    /// <summary>Gets the immediate AppEnforcePreference.</summary>
    /// <value>The immediate AppEnforcePreference.</value>
    public static uint Immediate => 0;

    /// <summary>Gets the non business hours AppEnforcePreference.</summary>
    /// <value>The non business hours AppEnforcePreference.</value>
    public static uint NonBusinessHours => 1;

    /// <summary>Gets the admin schedule AppEnforcePreference.</summary>
    /// <value>The admin schedule AppEnforcePreference.</value>
    public static uint AdminSchedule => 2;
  }

  /// <summary>Execution History</summary>
  public class REG_ExecutionHistory
  {
    internal baseInit oNewBase;
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    internal string __RegPATH { get; set; }

    internal PSObject RegObject { get; set; }

    public string _ProgramID { get; set; }

    public string _State { get; set; }

    public DateTime? _RunStartTime { get; set; }

    public int? SuccessOrFailureCode { get; set; }

    public string SuccessOrFailureReason { get; set; }

    public string UserID { get; set; }

    public string PackageID { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwaredistribution.REG_ExecutionHistory" /> class.
    /// </summary>
    /// <param name="RegObject">The reg object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public REG_ExecutionHistory(PSObject RegObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.oNewBase = new baseInit(this.remoteRunspace, this.pSCode);
      this.__RegPATH = (RegObject.Properties["PSPath"].Value as string).Replace("Microsoft.PowerShell.Core\\Registry::", "");
      this._ProgramID = RegObject.Properties[nameof (_ProgramID)].Value as string;
      this._State = RegObject.Properties[nameof (_State)].Value as string;
      this._RunStartTime = new DateTime?(DateTime.Parse(RegObject.Properties[nameof (_RunStartTime)].Value as string));
      if (!string.IsNullOrEmpty(RegObject.Properties[nameof (SuccessOrFailureCode)].Value as string))
        this.SuccessOrFailureCode = new int?(int.Parse(RegObject.Properties[nameof (SuccessOrFailureCode)].Value as string));
      if (!string.IsNullOrEmpty(RegObject.Properties[nameof (SuccessOrFailureReason)].Value as string))
        this.SuccessOrFailureReason = RegObject.Properties[nameof (SuccessOrFailureReason)].Value as string;
      this.UserID = this.__RegPATH.Substring(this.__RegPATH.IndexOf("Execution History", StringComparison.CurrentCultureIgnoreCase)).Split('\\')[1];
      this.PackageID = this.__RegPATH.Substring(this.__RegPATH.IndexOf(this.UserID, StringComparison.CurrentCultureIgnoreCase)).Split('\\')[1];
    }

    /// <summary>Delete the Execution-History Item</summary>
    public void Delete()
    {
      try
      {
        this.oNewBase.GetObjectsFromPS($"Remove-Item \"{this.__RegPATH.Replace("HKEY_LOCAL_MACHINE\\", "HKLM:\\")}\" -Recurse", true, new TimeSpan(0, 0, 1));
      }
      catch
      {
      }
    }

    /// <summary>Translate the SID to a readable Username</summary>
    public void GetUserFromSID()
    {
      if (!this.UserID.StartsWith("S-1-5-21-"))
        return;
      this.UserID = this.oNewBase.GetStringFromPS($"((New-Object System.Security.Principal.SecurityIdentifier(\"{this.UserID}\")).Translate([System.Security.Principal.NTAccount])).value", false);
    }
  }

  /// <summary>Class SoftwareStatus.</summary>
  public class SoftwareStatus
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;
    internal baseInit oNewBase;

    /// <summary>Gets or sets the SoftwareStatus icon.</summary>
    /// <value>The SoftwareStatus icon.</value>
    public string Icon { get; set; }

    /// <summary>Gets or sets the SoftwareStatus name.</summary>
    /// <value>The SoftwareStatus name.</value>
    public string Name { get; set; }

    /// <summary>Gets or sets the SoftwareStatus type.</summary>
    /// <value>The SoftwareStatus type.</value>
    public string Type { get; set; }

    /// <summary>Gets or sets the SoftwareStatus publisher.</summary>
    /// <value>The SoftwareStatus publisher.</value>
    public string Publisher { get; set; }

    /// <summary>Gets or sets the SoftwareStatus availability.</summary>
    /// <value>The SoftwareStatus availability.</value>
    public DateTime? AvailableAfter { get; set; }

    /// <summary>Gets or sets the SoftwareStatus status.</summary>
    /// <value>The SoftwareStatus status.</value>
    public string Status { get; set; }

    /// <summary>Gets or sets the SoftwareStatus percent complete.</summary>
    /// <value>The SoftwareStatus percent complete.</value>
    public uint PercentComplete { get; set; }

    /// <summary>Gets or sets the SoftwareStatus error code.</summary>
    /// <value>The SoftwareStatus error code.</value>
    public uint ErrorCode { get; set; }

    private softwaredistribution.CCM_SoftwareBase _rawObject { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwaredistribution.SoftwareStatus" /> class.
    /// </summary>
    /// <param name="SWObject">The sw object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public SoftwareStatus(PSObject SWObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      try
      {
        this.remoteRunspace = RemoteRunspace;
        this.pSCode = PSCode;
        this.oNewBase = new baseInit(this.remoteRunspace, this.pSCode);
        try
        {
          if (SWObject.Properties[nameof (Type)].Value == null)
            this.Type = "";
          string typeValue = SWObject.Properties[nameof (Type)].Value.ToString();
          if (string.IsNullOrEmpty(typeValue))
            typeValue = "99";
          switch (int.Parse(typeValue))
          {
            case 0:
              this.Type = "Program";
              this._rawObject = (softwaredistribution.CCM_SoftwareBase) new softwaredistribution.CCM_Program(SWObject, RemoteRunspace, PSCode);
              this.Icon = "";
              this.AvailableAfter = ((softwaredistribution.CCM_Program) this._rawObject).ActivationTime;
              this.Status = ((softwaredistribution.CCM_Program) this._rawObject).LastRunStatus;
              this.Name = $"{((softwaredistribution.CCM_Program) this._rawObject).PackageName};{((softwaredistribution.CCM_Program) this._rawObject).ProgramID}";
              if (this.Status == "Succeeded")
                this.Status = "Installed";
              if (((softwaredistribution.CCM_Program) this._rawObject).RepeatRunBehavior == "RerunAlways")
              {
                if (this._rawObject.Deadline.HasValue)
                {
                  DateTime? deadline = this._rawObject.Deadline;
                  DateTime now = DateTime.Now;
                  if ((deadline.HasValue ? (deadline.GetValueOrDefault() > now ? 1 : 0) : 0) != 0)
                  {
                    string status = this.Status;
                    deadline = this._rawObject.Deadline;
                    string str = deadline.ToString();
                    this.Status = $"{status}; waiting to install again at {str}";
                    break;
                  }
                  break;
                }
                break;
              }
              break;
            case 1:
              this.Type = "Application";
              this._rawObject = (softwaredistribution.CCM_SoftwareBase) new softwaredistribution.CCM_Application(SWObject, RemoteRunspace, PSCode);
              this.Icon = SWObject.Properties[nameof (Icon)].Value as string;
              this.AvailableAfter = ((softwaredistribution.CCM_Application) this._rawObject).StartTime;
              this.Name = SWObject.Properties[nameof (Name)].Value as string;
              switch (int.Parse(SWObject.Properties["EvaluationState"].Value.ToString()))
              {
                case 0:
                  this.Status = "Not Installed";
                  break;
                case 1:
                  this.Status = "Installed";
                  break;
                case 2:
                  this.Status = "Not required";
                  break;
                case 3:
                  this.Status = "Ready";
                  break;
                case 4:
                  this.Status = "Failed";
                  break;
                case 5:
                  this.Status = "Downloading content";
                  break;
                case 6:
                  this.Status = "Downloading content";
                  break;
                case 7:
                  this.Status = "Waiting";
                  break;
                case 8:
                  this.Status = "Waiting";
                  break;
                case 9:
                  this.Status = "Waiting";
                  break;
                case 10:
                  this.Status = "Waiting";
                  break;
                case 11:
                  this.Status = "Waiting";
                  break;
                case 12:
                  this.Status = "Installing";
                  break;
                case 13:
                  this.Status = "Reboot pending";
                  break;
                case 14:
                  this.Status = "Reboot pending";
                  break;
                case 15:
                  this.Status = "Waiting";
                  break;
                case 16 /*0x10*/:
                  this.Status = "Failed";
                  break;
                case 17:
                  this.Status = "Waiting";
                  break;
                case 18:
                  this.Status = "Waiting";
                  break;
                case 19:
                  this.Status = "Waiting";
                  break;
                case 20:
                  this.Status = "Waiting";
                  break;
                case 21:
                  this.Status = "Waiting";
                  break;
                case 22:
                  this.Status = "Downloading content";
                  break;
                case 23:
                  this.Status = "Downloading content";
                  break;
                case 24:
                  this.Status = "Failed";
                  break;
                case 25:
                  this.Status = "Failed";
                  break;
                case 26:
                  this.Status = "Waiting";
                  break;
                case 27:
                  this.Status = "Waiting";
                  break;
                case 28:
                  this.Status = "Waiting";
                  break;
                default:
                  this.Status = "Unknown state information.";
                  break;
              }
              break;
            case 2:
              this.Type = "Software Update";
              this._rawObject = (softwaredistribution.CCM_SoftwareBase) new softwaredistribution.CCM_SoftwareUpdate(SWObject, RemoteRunspace, PSCode);
              this.Icon = "Updates";
              this.AvailableAfter = ((softwaredistribution.CCM_SoftwareUpdate) this._rawObject).StartTime;
              this.Name = SWObject.Properties[nameof (Name)].Value as string;
              switch (int.Parse(SWObject.Properties["EvaluationState"].Value.ToString()))
              {
                case 0:
                  this.Status = "No State";
                  try
                  {
                    if (SWObject.Properties["ComplianceState"].Value.ToString() == "0")
                    {
                      this.Status = "Missing";
                      break;
                    }
                    break;
                  }
                  catch
                  {
                    break;
                  }
                case 1:
                  this.Status = "Missing";
                  break;
                case 2:
                  this.Status = "Ready";
                  break;
                case 3:
                  this.Status = "Detecting";
                  break;
                case 4:
                  this.Status = "Downloading content";
                  break;
                case 5:
                  this.Status = "Downloading content";
                  break;
                case 6:
                  this.Status = "Waiting";
                  break;
                case 7:
                  this.Status = "Installing";
                  break;
                case 8:
                  this.Status = "Reboot pending";
                  break;
                case 9:
                  this.Status = "Reboot pending";
                  break;
                case 10:
                  this.Status = "Reboot pending";
                  break;
                case 11:
                  this.Status = "Verifying";
                  break;
                case 12:
                  this.Status = "Installed";
                  break;
                case 13:
                  this.Status = "Failed";
                  break;
                case 14:
                  this.Status = "Waiting";
                  break;
                case 15:
                  this.Status = "Waiting";
                  break;
                case 16 /*0x10*/:
                  this.Status = "Waiting";
                  break;
                case 17:
                  this.Status = "Waiting";
                  break;
                case 18:
                  this.Status = "Waiting";
                  break;
                case 19:
                  this.Status = "Waiting";
                  break;
                case 20:
                  this.Status = "Pending";
                  break;
                case 21:
                  this.Status = "Waiting";
                  break;
                case 22:
                  this.Status = "Waiting";
                  break;
                case 23:
                  this.Status = "Waiting";
                  break;
                default:
                  this.Status = "Unknown state information.";
                  break;
              }
              break;
            case 3:
              this.Type = "Program";
              this._rawObject = (softwaredistribution.CCM_SoftwareBase) new softwaredistribution.CCM_Program(SWObject, RemoteRunspace, PSCode);
              this.Icon = "";
              bool? taskSequence = ((softwaredistribution.CCM_Program) this._rawObject).TaskSequence;
              bool isTaskSequence = true;
              if (taskSequence.GetValueOrDefault() == isTaskSequence & taskSequence.HasValue)
                this.Type = "Operating System";
              this.Status = ((softwaredistribution.CCM_Program) this._rawObject).LastRunStatus;
              this.AvailableAfter = ((softwaredistribution.CCM_Program) this._rawObject).ActivationTime;
              this.Name = SWObject.Properties[nameof (Name)].Value as string;
              if (this.Status == "Succeeded")
                this.Status = "Installed";
              if (((softwaredistribution.CCM_Program) this._rawObject).RepeatRunBehavior == "RerunAlways")
              {
                if (this._rawObject.Deadline.HasValue)
                {
                  DateTime? deadline = this._rawObject.Deadline;
                  DateTime now = DateTime.Now;
                  if ((deadline.HasValue ? (deadline.GetValueOrDefault() > now ? 1 : 0) : 0) != 0)
                  {
                    string status = this.Status;
                    deadline = this._rawObject.Deadline;
                    string str = deadline.ToString();
                    this.Status = $"{status}; waiting to install again at {str}";
                    break;
                  }
                  break;
                }
                break;
              }
              break;
            default:
              this.Type = "Unknown";
              this.Icon = "";
              this.Status = "";
              break;
          }
        }
        catch
        {
        }
        this.Publisher = SWObject.Properties[nameof (Publisher)].Value as string;
        try
        {
          this.ErrorCode = uint.Parse(SWObject.Properties[nameof (ErrorCode)].Value.ToString());
        }
        catch
        {
        }
        try
        {
          this.PercentComplete = uint.Parse(SWObject.Properties[nameof (PercentComplete)].Value.ToString());
        }
        catch
        {
        }
      }
      catch
      {
      }
    }
  }

  /// <summary>Source:ROOT\ccm\cimodels</summary>
  public class Synclet
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwaredistribution.Synclet" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.ClassName = WMIObject.Properties[nameof (ClassName)].Value as string;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    /// <summary>Gets or sets the name of the class.</summary>
    /// <value>The name of the class.</value>
    public string ClassName { get; set; }
  }

  /// <summary>Source:ROOT\ccm\cimodels</summary>
  public class CCM_HandlerSynclet : softwaredistribution.Synclet
  {
    internal new Runspace remoteRunspace;
    internal new TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwaredistribution.Synclet" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CCM_HandlerSynclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
      : base(WMIObject, RemoteRunspace, PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.ActionType = WMIObject.Properties[nameof (ActionType)].Value as string;
      this.AppDeliveryTypeId = WMIObject.Properties[nameof (AppDeliveryTypeId)].Value as string;
      this.ExecutionContext = WMIObject.Properties[nameof (ExecutionContext)].Value as string;
      this.RequiresLogOn = WMIObject.Properties[nameof (RequiresLogOn)].Value as string;
      this.Reserved = WMIObject.Properties[nameof (Reserved)].Value as string;
      this.Revision = WMIObject.Properties[nameof (Revision)].Value as uint?;
    }

    internal new string __CLASS { get; set; }

    internal new string __NAMESPACE { get; set; }

    internal new bool __INSTANCE { get; set; }

    internal new string __RELPATH { get; set; }

    internal new PSObject WMIObject { get; set; }

    public string ActionType { get; set; }

    public string AppDeliveryTypeId { get; set; }

    public string ExecutionContext { get; set; }

    public string RequiresLogOn { get; set; }

    public string Reserved { get; set; }

    public uint? Revision { get; set; }
  }

  /// <summary>Source:ROOT\ccm\cimodels</summary>
  public class CCM_LocalInstallationSynclet : softwaredistribution.CCM_HandlerSynclet
  {
    internal new Runspace remoteRunspace;
    internal new TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwaredistribution.Synclet" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CCM_LocalInstallationSynclet(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode)
      : base(WMIObject, RemoteRunspace, PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.AllowedTarget = WMIObject.Properties[nameof (AllowedTarget)].Value as string;
      this.ExecuteTime = WMIObject.Properties[nameof (ExecuteTime)].Value as uint?;
      this.FastRetryExitCodes = WMIObject.Properties[nameof (FastRetryExitCodes)].Value as uint?[];
      this.HardRebootExitCodes = WMIObject.Properties[nameof (HardRebootExitCodes)].Value as uint?[];
      this.InstallCommandLine = WMIObject.Properties[nameof (InstallCommandLine)].Value as string;
      this.MaxExecuteTime = WMIObject.Properties[nameof (MaxExecuteTime)].Value as uint?;
      this.PostInstallbehavior = WMIObject.Properties[nameof (PostInstallbehavior)].Value as string;
      this.RebootExitCodes = WMIObject.Properties[nameof (RebootExitCodes)].Value as uint?[];
      this.RequiresElevatedRights = WMIObject.Properties[nameof (RequiresElevatedRights)].Value as bool?;
      this.RequiresReboot = WMIObject.Properties[nameof (RequiresReboot)].Value as bool?;
      this.RequiresUserInteraction = WMIObject.Properties[nameof (RequiresUserInteraction)].Value as bool?;
      this.RunAs32Bit = WMIObject.Properties[nameof (RunAs32Bit)].Value as bool?;
      this.SuccessExitCodes = WMIObject.Properties[nameof (SuccessExitCodes)].Value as uint?[];
      this.UserInteractionMode = WMIObject.Properties[nameof (UserInteractionMode)].Value as string;
      this.WorkingDirectory = WMIObject.Properties[nameof (WorkingDirectory)].Value as string;
    }

    internal new string __CLASS { get; set; }

    internal new string __NAMESPACE { get; set; }

    internal new bool __INSTANCE { get; set; }

    internal new string __RELPATH { get; set; }

    internal new PSObject WMIObject { get; set; }

    public string AllowedTarget { get; set; }

    public uint? ExecuteTime { get; set; }

    public uint?[] FastRetryExitCodes { get; set; }

    public uint?[] HardRebootExitCodes { get; set; }

    public string InstallCommandLine { get; set; }

    public uint? MaxExecuteTime { get; set; }

    public string PostInstallbehavior { get; set; }

    public uint?[] RebootExitCodes { get; set; }

    public bool? RequiresElevatedRights { get; set; }

    public bool? RequiresReboot { get; set; }

    public bool? RequiresUserInteraction { get; set; }

    public bool? RunAs32Bit { get; set; }

    public uint?[] SuccessExitCodes { get; set; }

    public string UserInteractionMode { get; set; }

    public string WorkingDirectory { get; set; }
  }

  /// <summary>Source:ROOT\ccm\cimodels</summary>
  public class CCM_AppEnforceStatus
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwaredistribution.CCM_AppEnforceStatus" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CCM_AppEnforceStatus(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.AppDeliveryTypeId = WMIObject.Properties[nameof (AppDeliveryTypeId)].Value as string;
      this.ExecutionStatus = WMIObject.Properties[nameof (ExecutionStatus)].Value as string;
      this.ExitCode = WMIObject.Properties[nameof (ExitCode)].Value as uint?;
      this.ReconnectData = WMIObject.Properties[nameof (ReconnectData)].Value as softwaredistribution.CCM_AppReconnectData_Base;
      this.Revision = WMIObject.Properties[nameof (Revision)].Value as uint?;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    public string AppDeliveryTypeId { get; set; }

    public string ExecutionStatus { get; set; }

    public uint? ExitCode { get; set; }

    public softwaredistribution.CCM_AppReconnectData_Base ReconnectData { get; set; }

    public uint? Revision { get; set; }
  }

  /// <summary>Source:ROOT\ccm\cimodels</summary>
  public class CCM_AppReconnectData_Base
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwaredistribution.CCM_AppReconnectData_Base" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CCM_AppReconnectData_Base(
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
      this.AppDeliveryTypeId = WMIObject.Properties[nameof (AppDeliveryTypeId)].Value as string;
      this.Revision = WMIObject.Properties[nameof (Revision)].Value as uint?;
      this.UserSid = WMIObject.Properties[nameof (UserSid)].Value as string;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    /// <summary>
    /// Gets or sets the application delivery type identifier.
    /// </summary>
    /// <value>The application delivery type identifier.</value>
    public string AppDeliveryTypeId { get; set; }

    /// <summary>Gets or sets the revision.</summary>
    /// <value>The revision.</value>
    public uint? Revision { get; set; }

    /// <summary>Gets or sets the user sid.</summary>
    /// <value>The user sid.</value>
    public string UserSid { get; set; }
  }

  /// <summary>Source:ROOT\ccm\cimodels</summary>
  public class CCM_AppDeliveryType
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwaredistribution.CCM_AppDeliveryType" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CCM_AppDeliveryType(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.AppDeliveryTypeId = WMIObject.Properties[nameof (AppDeliveryTypeId)].Value as string;
      this.AppId = WMIObject.Properties[nameof (AppId)].Value as string;
      this.HostType = WMIObject.Properties[nameof (HostType)].Value as string;
      this.Revision = WMIObject.Properties[nameof (Revision)].Value as uint?;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    /// <summary>
    /// Gets or sets the application delivery type identifier.
    /// </summary>
    /// <value>The application delivery type identifier.</value>
    public string AppDeliveryTypeId { get; set; }

    /// <summary>Gets or sets the application identifier.</summary>
    /// <value>The application identifier.</value>
    public string AppId { get; set; }

    /// <summary>Gets or sets the type of the host.</summary>
    /// <value>The type of the host.</value>
    public string HostType { get; set; }

    /// <summary>Gets or sets the revision.</summary>
    /// <value>The revision.</value>
    public uint? Revision { get; set; }
  }

  /// <summary>Source:ROOT\ccm\cimodels</summary>
  public class CCM_AppDeliveryTypeSynclet : softwaredistribution.Synclet
  {
    internal new Runspace remoteRunspace;
    internal new TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwaredistribution.Synclet" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CCM_AppDeliveryTypeSynclet(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode)
      : base(WMIObject, RemoteRunspace, PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.AppDeliveryTypeId = WMIObject.Properties[nameof (AppDeliveryTypeId)].Value as string;
      this.AppDeliveryTypeName = WMIObject.Properties[nameof (AppDeliveryTypeName)].Value as string;
      this.AppId = WMIObject.Properties[nameof (AppId)].Value as string;
      this.DiscAction = WMIObject.Properties[nameof (DiscAction)].Value as softwaredistribution.CCM_AppAction;
      this.HostType = WMIObject.Properties[nameof (HostType)].Value as string;
      this.InstallAction = WMIObject.Properties[nameof (InstallAction)].Value as softwaredistribution.CCM_AppAction;
      this.Revision = WMIObject.Properties[nameof (Revision)].Value as uint?;
      this.UninstallAction = WMIObject.Properties[nameof (UninstallAction)].Value as softwaredistribution.CCM_AppAction;
    }

    internal new string __CLASS { get; set; }

    internal new string __NAMESPACE { get; set; }

    internal new bool __INSTANCE { get; set; }

    internal new string __RELPATH { get; set; }

    internal new PSObject WMIObject { get; set; }

    /// <summary>
    /// Gets or sets the application delivery type identifier.
    /// </summary>
    /// <value>The application delivery type identifier.</value>
    public string AppDeliveryTypeId { get; set; }

    /// <summary>
    /// Gets or sets the name of the application delivery type.
    /// </summary>
    /// <value>The name of the application delivery type.</value>
    public string AppDeliveryTypeName { get; set; }

    /// <summary>Gets or sets the application identifier.</summary>
    /// <value>The application identifier.</value>
    public string AppId { get; set; }

    /// <summary>Gets or sets the disc action.</summary>
    /// <value>The disc action.</value>
    public softwaredistribution.CCM_AppAction DiscAction { get; set; }

    /// <summary>Gets or sets the type of the host.</summary>
    /// <value>The type of the host.</value>
    public string HostType { get; set; }

    /// <summary>Gets or sets the install action.</summary>
    /// <value>The install action.</value>
    public softwaredistribution.CCM_AppAction InstallAction { get; set; }

    /// <summary>Gets or sets the revision.</summary>
    /// <value>The revision.</value>
    public uint? Revision { get; set; }

    /// <summary>Gets or sets the uninstall action.</summary>
    /// <value>The uninstall action.</value>
    public softwaredistribution.CCM_AppAction UninstallAction { get; set; }
  }

  /// <summary>Source:ROOT\ccm\cimodels</summary>
  public class CCM_AppAction
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwaredistribution.CCM_AppAction" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public CCM_AppAction(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.ActionType = WMIObject.Properties[nameof (ActionType)].Value as string;
      this.Content = WMIObject.Properties[nameof (Content)].Value as softwaredistribution.ContentInfo;
      this.HandlerName = WMIObject.Properties[nameof (HandlerName)].Value as string;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    /// <summary>Gets or sets the type of the action.</summary>
    /// <value>The type of the action.</value>
    public string ActionType { get; set; }

    /// <summary>Gets or sets the content.</summary>
    /// <value>The content.</value>
    public softwaredistribution.ContentInfo Content { get; set; }

    /// <summary>Gets or sets the name of the handler.</summary>
    /// <value>The name of the handler.</value>
    public string HandlerName { get; set; }
  }

  /// <summary>Source:ROOT\ccm\cimodels</summary>
  public class ContentInfo
  {
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.softwaredistribution.ContentInfo" /> class.
    /// </summary>
    /// <param name="WMIObject">The WMI object.</param>
    /// <param name="RemoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">The PowerShell code.</param>
    public ContentInfo(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.ContentId = WMIObject.Properties[nameof (ContentId)].Value as string;
      this.ContentVersion = WMIObject.Properties[nameof (ContentVersion)].Value as string;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    /// <summary>Gets or sets the content identifier.</summary>
    /// <value>The content identifier.</value>
    public string ContentId { get; set; }

    /// <summary>Gets or sets the content version.</summary>
    /// <value>The content version.</value>
    public string ContentVersion { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_CIAssignment : softwaredistribution.CCM_Policy
  {
    internal new Runspace remoteRunspace;
    internal new TraceSource pSCode;

    /// <summary>CCM_CIAssignment</summary>
    /// <param name="WMIObject"></param>
    /// <param name="RemoteRunspace"></param>
    /// <param name="PSCode"></param>
    public CCM_CIAssignment(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
      : base(WMIObject)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      try
      {
        object assignedCIsValue = WMIObject.Properties[nameof (AssignedCIs)].Value;
        object[] array;
        if (assignedCIsValue.GetType() == typeof (PSObject))
          array = new object[1]{ assignedCIsValue };
        else
          array = (object[]) assignedCIsValue;
        this.AssignedCIs = Array.ConvertAll<object, string>(array, (Converter<object, string>) (x => x.ToString()));
      }
      catch
      {
      }
      this.AssignmentAction = WMIObject.Properties[nameof (AssignmentAction)].Value as uint?;
      this.AssignmentFlags = WMIObject.Properties[nameof (AssignmentFlags)].Value as ulong?;
      this.AssignmentID = WMIObject.Properties[nameof (AssignmentID)].Value as string;
      this.AssignmentName = WMIObject.Properties[nameof (AssignmentName)].Value as string;
      this.ConfigurationFlags = WMIObject.Properties[nameof (ConfigurationFlags)].Value as ulong?;
      this.DesiredConfigType = WMIObject.Properties[nameof (DesiredConfigType)].Value as uint?;
      this.DisableMomAlerts = WMIObject.Properties[nameof (DisableMomAlerts)].Value as bool?;
      this.DPLocality = WMIObject.Properties[nameof (DPLocality)].Value as uint?;
      string enforcementDmtf = WMIObject.Properties[nameof (EnforcementDeadline)].Value as string;
      this.EnforcementDeadline = !string.IsNullOrEmpty(enforcementDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(enforcementDmtf)) : new DateTime?();
      string expirationDmtf = WMIObject.Properties[nameof (ExpirationTime)].Value as string;
      this.ExpirationTime = !string.IsNullOrEmpty(expirationDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(expirationDmtf)) : new DateTime?();
      this.LogComplianceToWinEvent = WMIObject.Properties[nameof (LogComplianceToWinEvent)].Value as bool?;
      this.NonComplianceCriticality = WMIObject.Properties[nameof (NonComplianceCriticality)].Value as uint?;
      this.NotifyUser = WMIObject.Properties[nameof (NotifyUser)].Value as bool?;
      this.OverrideServiceWindows = WMIObject.Properties[nameof (OverrideServiceWindows)].Value as bool?;
      this.PersistOnWriteFilterDevices = WMIObject.Properties[nameof (PersistOnWriteFilterDevices)].Value as bool?;
      this.RaiseMomAlertsOnFailure = WMIObject.Properties[nameof (RaiseMomAlertsOnFailure)].Value as bool?;
      this.RebootOutsideOfServiceWindows = WMIObject.Properties[nameof (RebootOutsideOfServiceWindows)].Value as bool?;
      this.Reserved1 = WMIObject.Properties[nameof (Reserved1)].Value as string;
      this.Reserved2 = WMIObject.Properties[nameof (Reserved2)].Value as string;
      this.Reserved3 = WMIObject.Properties[nameof (Reserved3)].Value as string;
      this.SendDetailedNonComplianceStatus = WMIObject.Properties[nameof (SendDetailedNonComplianceStatus)].Value as bool?;
      this.SettingTypes = WMIObject.Properties[nameof (SettingTypes)].Value as string;
      this.SoftDeadlineEnabled = WMIObject.Properties[nameof (SoftDeadlineEnabled)].Value as bool?;
      string startTimeDmtf = WMIObject.Properties[nameof (StartTime)].Value as string;
      this.StartTime = !string.IsNullOrEmpty(startTimeDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(startTimeDmtf)) : new DateTime?();
      this.StateMessagePriority = WMIObject.Properties[nameof (StateMessagePriority)].Value as uint?;
      this.SuppressReboot = WMIObject.Properties[nameof (SuppressReboot)].Value as uint?;
      string updateDeadlineDmtf = WMIObject.Properties[nameof (UpdateDeadline)].Value as string;
      this.UpdateDeadline = !string.IsNullOrEmpty(updateDeadlineDmtf) ? new DateTime?(ManagementDateTimeConverter.ToDateTime(updateDeadlineDmtf)) : new DateTime?();
      this.UseGMTTimes = WMIObject.Properties[nameof (UseGMTTimes)].Value as bool?;
      this.UseSiteEvaluation = WMIObject.Properties[nameof (UseSiteEvaluation)].Value as bool?;
      this.WoLEnabled = WMIObject.Properties[nameof (WoLEnabled)].Value as bool?;
    }

    internal new string __CLASS { get; set; }

    internal new string __NAMESPACE { get; set; }

    internal new bool __INSTANCE { get; set; }

    internal new string __RELPATH { get; set; }

    internal new PSObject WMIObject { get; set; }

    public string[] AssignedCIs { get; set; }

    public uint? AssignmentAction { get; set; }

    public ulong? AssignmentFlags { get; set; }

    public string AssignmentID { get; set; }

    public string AssignmentName { get; set; }

    public ulong? ConfigurationFlags { get; set; }

    public uint? DesiredConfigType { get; set; }

    public bool? DisableMomAlerts { get; set; }

    public uint? DPLocality { get; set; }

    public DateTime? EnforcementDeadline { get; set; }

    public DateTime? ExpirationTime { get; set; }

    public bool? LogComplianceToWinEvent { get; set; }

    public uint? NonComplianceCriticality { get; set; }

    public bool? NotifyUser { get; set; }

    public bool? OverrideServiceWindows { get; set; }

    public bool? PersistOnWriteFilterDevices { get; set; }

    public bool? RaiseMomAlertsOnFailure { get; set; }

    public bool? RebootOutsideOfServiceWindows { get; set; }

    public string Reserved1 { get; set; }

    public string Reserved2 { get; set; }

    public string Reserved3 { get; set; }

    public bool? SendDetailedNonComplianceStatus { get; set; }

    public string SettingTypes { get; set; }

    public bool? SoftDeadlineEnabled { get; set; }

    public DateTime? StartTime { get; set; }

    public uint? StateMessagePriority { get; set; }

    public uint? SuppressReboot { get; set; }

    public DateTime? UpdateDeadline { get; set; }

    public bool? UseGMTTimes { get; set; }

    public bool? UseSiteEvaluation { get; set; }

    public bool? WoLEnabled { get; set; }
  }

  /// <summary>Source:ROOT\ccm\Policy\Machine\ActualConfig</summary>
  public class CCM_ApplicationCIAssignment : softwaredistribution.CCM_CIAssignment
  {
    internal new Runspace remoteRunspace;
    internal new TraceSource pSCode;

    /// <summary>CCM_ApplicationCIAssignment</summary>
    /// <param name="WMIObject"></param>
    /// <param name="RemoteRunspace"></param>
    /// <param name="PSCode"></param>
    public CCM_ApplicationCIAssignment(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode)
      : base(WMIObject, RemoteRunspace, PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.Priority = WMIObject.Properties[nameof (Priority)].Value as uint?;
      this.Reserved4 = WMIObject.Properties[nameof (Reserved4)].Value as string;
      this.UserUIExperience = WMIObject.Properties[nameof (UserUIExperience)].Value as bool?;
    }

    internal new string __CLASS { get; set; }

    internal new string __NAMESPACE { get; set; }

    internal new bool __INSTANCE { get; set; }

    internal new string __RELPATH { get; set; }

    internal new PSObject WMIObject { get; set; }

    public uint? Priority { get; set; }

    public string Reserved4 { get; set; }

    public bool? UserUIExperience { get; set; }
  }

  /// <summary>AppDetailView</summary>
  public class AppDetailView
  {
    public string Category;
    public string Publisher;
    public string ApplicationId;
    public string Description;
    public string Name;
    public string AppVersion;
    public string AvailableDate;
    public string ApplicationDefinitionVersion;
    public string IsAppModelApplication;
    public string PackageName;
    public string ImagePath;
    public string AutoApproval;
    public string SupersededApplications;
    public string Icon;
    public string DeploymentTypeExperience;
    public string InstallationState;
    public string InstallationErrorCode;
  }
}
