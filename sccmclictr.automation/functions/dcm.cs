// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.functions.dcm
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Xml;

#nullable disable
namespace sccmclictr.automation.functions;

/// <summary>Class dcm.</summary>
public class dcm : baseInit
{
  internal Runspace remoteRunspace;
  internal TraceSource pSCode;
  internal ccm baseClient;

  /// <summary>Constructor</summary>
  /// <param name="RemoteRunspace"></param>
  /// <param name="PSCode"></param>
  /// <param name="oClient"></param>
  public dcm(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
    : base(RemoteRunspace, PSCode)
  {
    this.remoteRunspace = RemoteRunspace;
    this.pSCode = PSCode;
    this.baseClient = oClient;
  }

  /// <summary>Gets a list of  DCM baselines.</summary>
  /// <value>A list DCM baselines.</value>
  public List<dcm.SMS_DesiredConfiguration> DCMBaselines
  {
    get
    {
      List<dcm.SMS_DesiredConfiguration> dcmBaselines = new List<dcm.SMS_DesiredConfiguration>();
      foreach (PSObject WMIObject in this.GetObjects("ROOT\\ccm\\dcm", "SELECT * FROM SMS_DesiredConfiguration", true))
        dcmBaselines.Add(new dcm.SMS_DesiredConfiguration(WMIObject, this.remoteRunspace, this.pSCode)
        {
          remoteRunspace = this.remoteRunspace,
          pSCode = this.pSCode
        });
      return dcmBaselines;
    }
  }

  /// <summary>Source:ROOT\ccm\dcm</summary>
  public class SMS_DesiredConfiguration
  {
    internal baseInit oNewBase;
    internal Runspace remoteRunspace;
    internal TraceSource pSCode;

    /// <summary>Constructor</summary>
    /// <param name="WMIObject"></param>
    /// <param name="RemoteRunspace"></param>
    /// <param name="PSCode"></param>
    public SMS_DesiredConfiguration(
      PSObject WMIObject,
      Runspace RemoteRunspace,
      TraceSource PSCode)
    {
      this.remoteRunspace = RemoteRunspace;
      this.pSCode = PSCode;
      this.oNewBase = new baseInit(this.remoteRunspace, this.pSCode);
      this.__CLASS = WMIObject.Properties[nameof (__CLASS)].Value as string;
      this.__NAMESPACE = WMIObject.Properties[nameof (__NAMESPACE)].Value as string;
      this.__RELPATH = WMIObject.Properties[nameof (__RELPATH)].Value as string;
      this.__INSTANCE = true;
      this.WMIObject = WMIObject;
      this.ComplianceDetails = WMIObject.Properties[nameof (ComplianceDetails)].Value as string;
      this.DisplayName = WMIObject.Properties[nameof (DisplayName)].Value as string;
      this.IsMachineTarget = WMIObject.Properties[nameof (IsMachineTarget)].Value as bool?;
      this.LastComplianceStatus = WMIObject.Properties[nameof (LastComplianceStatus)].Value as uint?;
      string dmtfDate = WMIObject.Properties[nameof (LastEvalTime)].Value as string;
      if (string.IsNullOrEmpty(dmtfDate))
      {
        this.LastEvalTime = new DateTime?();
      }
      else
      {
        try
        {
          this.LastEvalTime = new DateTime?(ManagementDateTimeConverter.ToDateTime(dmtfDate));
        }
        catch
        {
        }
      }
      this.Name = WMIObject.Properties[nameof (Name)].Value as string;
      this.Status = WMIObject.Properties[nameof (Status)].Value as uint?;
      this.Version = WMIObject.Properties[nameof (Version)].Value as string;
      uint? complianceStatus = this.LastComplianceStatus;
      uint num = 1;
      this.isCompliant = (int) complianceStatus.GetValueOrDefault() == (int) num & complianceStatus.HasValue;
      this._RawObject = WMIObject;
    }

    internal string __CLASS { get; set; }

    internal string __NAMESPACE { get; set; }

    internal bool __INSTANCE { get; set; }

    internal string __RELPATH { get; set; }

    internal PSObject WMIObject { get; set; }

    public string ComplianceDetails { get; set; }

    public string DisplayName { get; set; }

    public bool? IsMachineTarget { get; set; }

    public uint? LastComplianceStatus { get; set; }

    public DateTime? LastEvalTime { get; set; }

    public string Name { get; set; }

    public uint? Status { get; set; }

    public string Version { get; set; }

    public bool isCompliant { get; set; }

    public PSObject _RawObject { get; set; }

    /// <summary>Trigger the Baseline evaluation</summary>
    /// <param name="IsEnforced"></param>
    /// <param name="JobId"></param>
    /// <returns></returns>
    public uint TriggerEvaluation(bool IsEnforced, out string JobId)
    {
      JobId = "";
      try
      {
        PSObject psObject = this.oNewBase.CallClassMethod("ROOT\\ccm\\dcm:SMS_DesiredConfiguration", nameof (TriggerEvaluation), $"'{this.Name}', '{this.Version}', ${this.IsMachineTarget.ToString()} , ${IsEnforced.ToString()}");
        JobId = psObject.Properties["JobID"].Value.ToString();
        return (uint) psObject.Properties["ReturnValue"].Value;
      }
      catch
      {
      }
      return 1;
    }

    /// <summary>List of Config Items (Class ConfigItem)</summary>
    /// <returns>List{ConfigItem}.</returns>
    public List<dcm.SMS_DesiredConfiguration.ConfigItem> ConfigItems()
    {
      List<dcm.SMS_DesiredConfiguration.ConfigItem> configItemList = new List<dcm.SMS_DesiredConfiguration.ConfigItem>();
      try
      {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(this.ComplianceDetails);
        foreach (XmlNode selectNode1 in xmlDocument.SelectNodes("//ConfigurationItemReport/ReferencedConfigurationItems/ConfigurationItemReport"))
        {
          dcm.SMS_DesiredConfiguration.ConfigItem configItem = new dcm.SMS_DesiredConfiguration.ConfigItem();
          configItem.LogicalName = selectNode1.Attributes["LogicalName"].Value.ToString();
          configItem.Applicable = selectNode1.Attributes["CIApplicablityState"].Value == "Applicable";
          configItem.Compliant = selectNode1.Attributes["CIComplianceState"].Value == "Compliant";
          try
          {
            configItem.Detected = bool.Parse(selectNode1.Attributes["IsDetected"].Value.ToString());
          }
          catch
          {
            configItem.Detected = true;
          }
          configItem.Type = selectNode1.Attributes["Type"].Value.ToString();
          configItem.Version = selectNode1.Attributes["Version"].Value.ToString();
          configItem.CIName = selectNode1.SelectSingleNode("./CIProperties/Name").InnerText;
          configItem.CIDescription = selectNode1.SelectSingleNode("./CIProperties/Description").InnerText;
          if (selectNode1.SelectSingleNode("./ConstraintViolations[@Count > 0]") != null)
          {
            string strA1 = "";
            foreach (XmlNode selectNode2 in selectNode1.SelectNodes("./ConstraintViolations/ConstraintViolation"))
            {
              string strA2 = selectNode2.Attributes["Severity"].Value.ToString();
              if (string.Compare(strA2, "Information", true) == 0 && string.Compare(strA1, "Error", true) != 0 & string.Compare(strA1, "Warning", true) != 0)
                strA1 = strA2;
              if (string.Compare(strA2, "Warning", true) == 0 && string.Compare(strA1, "Error", true) != 0)
                strA1 = strA2;
              if (string.Compare(strA2, "Error", true) == 0)
                strA1 = strA2;
            }
            configItem.ConstraintViolation = strA1;
          }
          else
            configItem.ConstraintViolation = "";
          configItemList.Add(configItem);
        }
      }
      catch
      {
      }
      return configItemList;
    }

    /// <summary>ConfigItem Object</summary>
    public class ConfigItem
    {
      /// <summary>Logical Name</summary>
      public string LogicalName { get; set; }

      /// <summary>Display Name</summary>
      public string CIName { get; set; }

      /// <summary>Description</summary>
      public string CIDescription { get; set; }

      /// <summary>CI Version</summary>
      public string Version { get; set; }

      /// <summary>CI Type</summary>
      public string Type { get; set; }

      /// <summary>Compliance state</summary>
      public bool Compliant { get; set; }

      /// <summary>Detection state</summary>
      public bool Detected { get; set; }

      /// <summary>Applicable state</summary>
      public bool Applicable { get; set; }

      /// <summary>Violation status</summary>
      public string ConstraintViolation { get; set; }
    }
  }
}
