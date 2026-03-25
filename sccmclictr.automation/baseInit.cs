// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.baseInit
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Text;

#nullable disable
namespace sccmclictr.automation;

/// <summary>
/// 
/// </summary>
public class baseInit : IDisposable
{
  internal MemoryCache Cache;
  internal bool bShowPSCodeOnly;
  internal TimeSpan cacheTime = new TimeSpan(0, 0, 30);
  /// <summary>Define the DebugLevel</summary>
  private TraceSwitch debugLevel = new TraceSwitch("DebugLevel", "DebugLevel from ConfigFile", "Verbose");

  /// <summary>
  /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
  /// </summary>
  public void Dispose()
  {
    try
    {
      if (this.remoteRunspace != null)
      {
        this.remoteRunspace.Close();
        this.remoteRunspace.Dispose();
      }
      if (this.tsPSCode != null)
      {
        try
        {
          this.tsPSCode.Close();
        }
        catch
        {
        }
        this.tsPSCode = (TraceSource) null;
      }
      try
      {
        if (this.Cache == null)
          return;
        foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) this.Cache)
          this.Cache.Remove(keyValuePair.Key, (string) null);
        this.Cache.Dispose();
      }
      catch
      {
      }
    }
    catch
    {
    }
  }

  private Runspace remoteRunspace { get; set; }

  internal string CreateHash(string str)
  {
    Encoder encoder = Encoding.Unicode.GetEncoder();
    byte[] buffer = new byte[str.Length * 2];
    char[] charArray = str.ToCharArray();
    int length = str.Length;
    byte[] bytes = buffer;
    encoder.GetBytes(charArray, 0, length, bytes, 0, true);
    byte[] hash = new SHA1CryptoServiceProvider().ComputeHash(buffer);
    StringBuilder stringBuilder = new StringBuilder();
    for (int index = 0; index < hash.Length; ++index)
      stringBuilder.Append(hash[index].ToString("X2"));
    return stringBuilder.ToString();
  }

  /// <summary>Trace Source for PowerShell Commands</summary>
  private TraceSource tsPSCode { get; set; }

  /// <summary>Constructor</summary>
  /// <param name="RemoteRunspace">PowerShell RunSpace</param>
  /// <param name="PSCode">TraceSource for PowerShell Commands</param>
  public baseInit(Runspace RemoteRunspace, TraceSource PSCode)
  {
    this.remoteRunspace = RemoteRunspace;
    this.tsPSCode = PSCode;
    this.Cache = new MemoryCache(RemoteRunspace.ConnectionInfo.ComputerName, new NameValueCollection());
  }

  /// <summary>Gets a string from cache or from a WMI class method.</summary>
  /// <param name="WMIPath">The WMI path.</param>
  /// <param name="WMIMethod">The WMI method.</param>
  /// <param name="ResultProperty">The name of the property you are trying to retrieve.</param>
  /// <returns>Command results as a string.</returns>
  /// <example><code>string siteCode = base.GetStringFromClassMethod(@"ROOT\ccm:SMS_Client", "GetAssignedSite()", "sSiteCode");</code></example>
  public string GetStringFromClassMethod(string WMIPath, string WMIMethod, string ResultProperty)
  {
    return this.GetStringFromClassMethod(WMIPath, WMIMethod, ResultProperty, false);
  }

  /// <summary>
  /// Gets a string from cache(if Reload==False) or from a WMI class method.
  /// </summary>
  /// <param name="WMIPath">The WMI path.</param>
  /// <param name="WMIMethod">The WMI method.</param>
  /// <param name="ResultProperty">The name of the property you are trying to retrieve.</param>
  /// <param name="Reload">Enforce reload. i.e. don't use cached results.</param>
  /// <returns>Command results as a string.</returns>
  /// <example><code>string siteCode = base.GetStringFromClassMethod(@"ROOT\ccm:SMS_Client", "GetAssignedSite()", "sSiteCode", True);</code></example>
  public string GetStringFromClassMethod(
    string WMIPath,
    string WMIMethod,
    string ResultProperty,
    bool Reload)
  {
    if (!ResultProperty.StartsWith("."))
      ResultProperty = "." + ResultProperty;
    string stringFromClassMethod = "";
    string str = $"([wmiclass]\"{WMIPath}\").{WMIMethod}{ResultProperty}";
    if (!this.bShowPSCodeOnly)
    {
      string hash = this.CreateHash(WMIPath + WMIMethod + ResultProperty);
      if (this.Cache.Get(hash, (string) null) != null & !Reload)
      {
        stringFromClassMethod = this.Cache.Get(hash, (string) null) as string;
      }
      else
      {
        foreach (PSObject psObject in WSMan.RunPSScript(str, this.remoteRunspace))
        {
          try
          {
            stringFromClassMethod = psObject.BaseObject.ToString();
            if (psObject.BaseObject.GetType() == typeof (ErrorRecord))
            {
              Trace.TraceError(psObject.ToString());
              Trace.WriteLineIf(this.debugLevel.TraceError, psObject.ToString());
              this.tsPSCode.TraceInformation("#ERROR:" + psObject.ToString());
              stringFromClassMethod = "";
              break;
            }
            this.Cache.Add(hash, (object) stringFromClassMethod, (DateTimeOffset) (DateTime.Now + this.cacheTime));
            break;
          }
          catch (Exception ex)
          {
            Trace.WriteLineIf(this.debugLevel.TraceError, ex.Message);
          }
        }
      }
    }
    this.tsPSCode.TraceInformation(str);
    return stringFromClassMethod;
  }

  /// <summary>Gets a string from cache or from a WMI method.</summary>
  /// <param name="WMIPath">The WMI path.</param>
  /// <param name="WMIMethod">The WMI method.</param>
  /// <param name="ResultProperty">The name of the property you are trying to retrieve.</param>
  /// <returns>Command results a as string.</returns>
  /// <example><code>bool multiUser = Boolean.Parse(GetStringFromMethod(@"ROOT\ccm\ClientSDK:CCM_ClientInternalUtilities=@", "AreMultiUsersLoggedOn", "MultiUsersLoggedOn"));</code></example>
  public string GetStringFromMethod(string WMIPath, string WMIMethod, string ResultProperty)
  {
    return this.GetStringFromMethod(WMIPath, WMIMethod, ResultProperty, false);
  }

  /// <summary>
  /// Gets a string from cache(if Reload==False) or from a WMI method.
  /// </summary>
  /// <param name="WMIPath">The WMI path.</param>
  /// <param name="WMIMethod">The WMI method.</param>
  /// <param name="ResultProperty">The name of the property you are trying to retrieve.</param>
  /// <param name="Reload">Enforce reload. i.e. don't use cached results.</param>
  /// <returns>Command results as a string.</returns>
  /// <example><code>bool multiUser = Boolean.Parse(GetStringFromMethod(@"ROOT\ccm\ClientSDK:CCM_ClientInternalUtilities=@", "AreMultiUsersLoggedOn", "MultiUsersLoggedOn", True));</code></example>
  public string GetStringFromMethod(
    string WMIPath,
    string WMIMethod,
    string ResultProperty,
    bool Reload)
  {
    if (!ResultProperty.StartsWith("("))
      ResultProperty = $"({ResultProperty})";
    string stringFromMethod = "";
    string str = $"([wmi]\"{WMIPath}\").{WMIMethod}{ResultProperty}";
    if (!this.bShowPSCodeOnly)
    {
      string hash = this.CreateHash(WMIPath + WMIMethod + ResultProperty);
      if (this.Cache.Get(hash, (string) null) != null & !Reload)
      {
        stringFromMethod = this.Cache.Get(hash, (string) null) as string;
      }
      else
      {
        foreach (PSObject psObject in WSMan.RunPSScript(str, this.remoteRunspace))
        {
          try
          {
            stringFromMethod = psObject.BaseObject.ToString();
            if (psObject.BaseObject.GetType() == typeof (ErrorRecord))
            {
              Trace.TraceError(psObject.ToString());
              Trace.WriteLineIf(this.debugLevel.TraceError, psObject.ToString());
              this.tsPSCode.TraceInformation("#ERROR:" + psObject.ToString());
              stringFromMethod = "";
              break;
            }
            this.Cache.Add(hash, (object) stringFromMethod, (DateTimeOffset) (DateTime.Now + this.cacheTime));
            break;
          }
          catch (Exception ex)
          {
            Trace.WriteLineIf(this.debugLevel.TraceError, ex.Message);
          }
        }
      }
    }
    this.tsPSCode.TraceInformation(str);
    return stringFromMethod;
  }

  /// <summary>Gets a PSObject from a WMI class method.</summary>
  /// <param name="WMIPath">The WMI path.</param>
  /// <param name="WMIMethod">The WMI method.</param>
  /// <param name="MethodParams">The method parameters.</param>
  /// <returns>Command results as a PSObject.</returns>
  /// <example><code>base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000001}'");</code></example>
  public PSObject CallClassMethod(string WMIPath, string WMIMethod, string MethodParams)
  {
    return this.CallClassMethod(WMIPath, WMIMethod, MethodParams, true);
  }

  /// <summary>
  /// Gets a PSObject from cache(if Reload==False) or from a WMI class method.
  /// </summary>
  /// <param name="WMIPath">The WMI path.</param>
  /// <param name="WMIMethod">The WMI method.</param>
  /// <param name="MethodParams">The method parameters.</param>
  /// <param name="Reload">Enforce reload. i.e. don't use cached results.</param>
  /// <returns>Command results as a PSObject.</returns>
  /// <example><code>base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000001}'", True);</code></example>
  public PSObject CallClassMethod(
    string WMIPath,
    string WMIMethod,
    string MethodParams,
    bool Reload)
  {
    if (!MethodParams.StartsWith("("))
      MethodParams = $"({MethodParams})";
    PSObject psObject1 = (PSObject) null;
    string str = $"([wmiclass]'{WMIPath}').{WMIMethod}{MethodParams}";
    if (!this.bShowPSCodeOnly)
    {
      string hash = this.CreateHash(WMIPath + WMIMethod + MethodParams);
      if (this.Cache.Get(hash, (string) null) != null & !Reload)
      {
        psObject1 = this.Cache.Get(hash, (string) null) as PSObject;
      }
      else
      {
        foreach (PSObject psObject2 in WSMan.RunPSScript(str, this.remoteRunspace))
        {
          try
          {
            psObject1 = psObject2;
            this.Cache.Add(hash, (object) psObject1, (DateTimeOffset) (DateTime.Now + this.cacheTime));
            break;
          }
          catch (Exception ex)
          {
            Trace.WriteLineIf(this.debugLevel.TraceError, ex.Message);
          }
        }
      }
    }
    this.tsPSCode.TraceInformation(str);
    return psObject1;
  }

  /// <summary>Gets a PSObject from a WMI instance method.</summary>
  /// <param name="WMIPath">The WMI path.</param>
  /// <param name="WMIMethod">The WMI method.</param>
  /// <param name="MethodParams">The method parameters.</param>
  /// <returns>Command results as a PSObject.</returns>
  public PSObject CallInstanceMethod(string WMIPath, string WMIMethod, string MethodParams)
  {
    return this.CallInstanceMethod(WMIPath, WMIMethod, MethodParams, true);
  }

  /// <summary>
  /// Gets a PSObject from cache(if Reload==False) of from a WMI instance method.
  /// </summary>
  /// <param name="WMIPath">The WMI path.</param>
  /// <param name="WMIMethod">The WMI method.</param>
  /// <param name="MethodParams">The method parameters.</param>
  /// <param name="Reload">Enforce reload. i.e. don't use cached results.</param>
  /// <returns>Command results as a PSObject.</returns>
  public PSObject CallInstanceMethod(
    string WMIPath,
    string WMIMethod,
    string MethodParams,
    bool Reload)
  {
    PSObject psObject1 = (PSObject) null;
    string str = $"([wmi]'{WMIPath}').{WMIMethod}({MethodParams})";
    if (!this.bShowPSCodeOnly)
    {
      string hash = this.CreateHash(WMIPath + WMIMethod + MethodParams);
      if (this.Cache.Get(hash, (string) null) != null & !Reload)
      {
        psObject1 = this.Cache.Get(hash, (string) null) as PSObject;
      }
      else
      {
        foreach (PSObject psObject2 in WSMan.RunPSScript(str, this.remoteRunspace))
        {
          try
          {
            psObject1 = psObject2;
            this.Cache.Add(hash, (object) psObject1, (DateTimeOffset) (DateTime.Now + this.cacheTime));
            break;
          }
          catch (Exception ex)
          {
            Trace.WriteLineIf(this.debugLevel.TraceError, ex.Message);
          }
        }
      }
    }
    this.tsPSCode.TraceInformation(str);
    return psObject1;
  }

  /// <summary>
  /// Gets a string from cache or from a PowerShell command.
  /// </summary>
  /// <param name="PSCode">The ps code.</param>
  /// <returns>Command results as a string.</returns>
  /// <example><code>string sPort = base.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Microsoft\\CCM\")).$(\"HttpPort\")");</code></example>
  public string GetStringFromPS(string PSCode) => this.GetStringFromPS(PSCode, false);

  /// <summary>
  /// Gets a string from cache(if Reload==False) or from a PowerShell command.
  /// </summary>
  /// <param name="PSCode">The ps code.</param>
  /// <param name="Reload">Enforce reload. i.e. don't use cached results.</param>
  /// <returns>Command results as a string.</returns>
  /// <example><code>string sPort = base.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Microsoft\\CCM\")).$(\"HttpPort\")", True);</code></example>
  public string GetStringFromPS(string PSCode, bool Reload = false)
  {
    string stringFromPs = "";
    if (!this.bShowPSCodeOnly)
    {
      string hash = this.CreateHash(PSCode);
      if (this.Cache.Get(hash, (string) null) != null & !Reload)
      {
        stringFromPs = this.Cache.Get(hash, (string) null) as string;
      }
      else
      {
        foreach (PSObject psObject in WSMan.RunPSScript(PSCode, this.remoteRunspace))
        {
          try
          {
            stringFromPs = psObject.ToString();
            if (psObject.BaseObject.GetType() == typeof (ErrorRecord))
            {
              Trace.TraceError(psObject.ToString());
              Trace.WriteLineIf(this.debugLevel.TraceError, psObject.ToString());
              this.tsPSCode.TraceInformation("#ERROR:" + psObject.ToString());
              stringFromPs = "";
              break;
            }
            this.Cache.Add(hash, (object) stringFromPs, (DateTimeOffset) (DateTime.Now + this.cacheTime));
            break;
          }
          catch (Exception ex)
          {
            Trace.WriteLineIf(this.debugLevel.TraceError, ex.Message);
          }
        }
      }
    }
    this.tsPSCode.TraceInformation(PSCode);
    return stringFromPs;
  }

  /// <summary>Gets a string from cache or from a WMI property.</summary>
  /// <param name="WMIPath">The WMI path.</param>
  /// <param name="ResultProperty">The name of the property you are trying to retrieve.</param>
  /// <returns>Command results as a string.</returns>
  /// <example><code>string siteCode = base.GetStringFromClassMethod(@"ROOT\ccm:CCM_Client=@", "ClientVersion");</code></example>
  public string GetProperty(string WMIPath, string ResultProperty)
  {
    return this.GetProperty(WMIPath, ResultProperty, false);
  }

  /// <summary>
  /// Gets a string from cache(if Reload==False) or from a WMI property.
  /// </summary>
  /// <param name="WMIPath">The WMI path.</param>
  /// <param name="ResultProperty">The name of the property you are trying to retrieve.</param>
  /// <param name="Reload">Enforce reload. i.e. don't use cached results.</param>
  /// <returns>Command results as a string.</returns>
  /// <example><code>string siteCode = base.GetStringFromClassMethod(@"ROOT\ccm:CCM_Client=@", "ClientVersion", True);</code></example>
  public string GetProperty(string WMIPath, string ResultProperty, bool Reload)
  {
    if (!ResultProperty.StartsWith("."))
      ResultProperty = "." + ResultProperty;
    string property = "";
    string str = $"([wmi]\"{WMIPath}\"){ResultProperty}";
    if (!this.bShowPSCodeOnly)
    {
      string hash = this.CreateHash(WMIPath + ResultProperty);
      if (this.Cache.Get(hash, (string) null) != null & !Reload)
      {
        property = this.Cache.Get(hash, (string) null) as string;
      }
      else
      {
        foreach (PSObject psObject in WSMan.RunPSScript(str, this.remoteRunspace))
        {
          try
          {
            property = psObject.BaseObject.ToString();
            if (psObject.BaseObject.GetType() == typeof (ErrorRecord))
            {
              Trace.TraceError(psObject.ToString());
              Trace.WriteLineIf(this.debugLevel.TraceError, psObject.ToString());
              this.tsPSCode.TraceInformation("#ERROR:" + psObject.ToString());
              property = "";
              break;
            }
            this.Cache.Add(hash, (object) property, (DateTimeOffset) (DateTime.Now + this.cacheTime));
            break;
          }
          catch (Exception ex)
          {
            Trace.WriteLineIf(this.debugLevel.TraceError, ex.Message);
          }
        }
      }
    }
    this.tsPSCode.TraceInformation(str);
    return property;
  }

  /// <summary>
  /// Gets a list of PSObjects from cache or from a WMI property.
  /// </summary>
  /// <param name="WMIPath">The WMI path.</param>
  /// <param name="ResultProperty">The name of the property you are trying to retrieve.</param>
  /// <returns>Command results as a list of PSObjects.</returns>
  /// <example><code>List&lt;PSObject&gt; lPSAppDts = base.GetProperties(@"ROOT\ccm\clientsdk:CCM_Application", "AppDTs");</code></example>
  public List<PSObject> GetProperties(string WMIPath, string ResultProperty)
  {
    return this.GetProperties(WMIPath, ResultProperty, false);
  }

  /// <summary>
  /// Gets a list of PSObjects from cache(if Reload==False) or from a WMI property.
  /// </summary>
  /// <param name="WMIPath">The WMI path.</param>
  /// <param name="ResultProperty">The name of the property you are trying to retrieve.</param>
  /// <param name="Reload">Enforce reload. i.e. don't use cached results.</param>
  /// <returns>Command results as a list of PSObjects.</returns>
  /// <example><code>List&lt;PSObject&gt; lPSAppDts = base.GetProperties(@"ROOT\ccm\clientsdk:CCM_Application", "AppDTs", True);</code></example>
  public List<PSObject> GetProperties(string WMIPath, string ResultProperty, bool Reload)
  {
    if (!ResultProperty.StartsWith("."))
      ResultProperty = "." + ResultProperty;
    List<PSObject> properties = new List<PSObject>();
    string str = $"([wmi]'{WMIPath}'){ResultProperty}";
    if (!this.bShowPSCodeOnly)
    {
      string hash = this.CreateHash(WMIPath + ResultProperty);
      if (this.Cache.Get(hash, (string) null) != null & !Reload)
      {
        properties = this.Cache.Get(hash, (string) null) as List<PSObject>;
      }
      else
      {
        foreach (PSObject psObject in WSMan.RunPSScript(str, this.remoteRunspace))
        {
          try
          {
            properties.Add(psObject);
          }
          catch (Exception ex)
          {
            Trace.WriteLineIf(this.debugLevel.TraceError, ex.Message);
          }
        }
        this.Cache.Add(hash, (object) properties, (DateTimeOffset) (DateTime.Now + this.cacheTime));
      }
    }
    this.tsPSCode.TraceInformation(str);
    return properties;
  }

  /// <summary>Sets a WMI property.</summary>
  /// <param name="WMIPath">The WMI path.</param>
  /// <param name="Property">The property.</param>
  /// <param name="Value">The value.</param>
  /// <example><code>base.SetProperty(@"ROOT\ccm:SMS_Client=@", "EnableAutoAssignment", "$True");</code></example>
  public void SetProperty(string WMIPath, string Property, string Value)
  {
    string str = $"$a=([wmi]\"{WMIPath}\");$a.{Property}={Value};$a.Put()";
    this.tsPSCode.TraceInformation(str);
    if (!this.bShowPSCodeOnly)
    {
      string hash = this.CreateHash($"{WMIPath}.{Property}");
      if (Value.StartsWith("$"))
        Value = Value.Remove(0, 1);
      this.Cache.Add(hash, (object) Value, (DateTimeOffset) (DateTime.Now + this.cacheTime));
      foreach (PSObject psObject in WSMan.RunPSScript(str, this.remoteRunspace))
      {
        try
        {
          psObject.BaseObject.ToString();
          break;
        }
        catch (Exception ex)
        {
          Trace.WriteLineIf(this.debugLevel.TraceError, ex.Message);
        }
      }
    }
    this.tsPSCode.TraceInformation(str);
  }

  /// <summary>
  /// Gets a list of PSObjects from cache or from a given WMI namespace using a given WQL query
  /// </summary>
  /// <param name="WMINamespace">The WMI namespace.</param>
  /// <param name="WQLQuery">The WQL query.</param>
  /// <returns>Command results as list of PSObjects.</returns>
  /// <example><code>List&lt;PSObject&gt; lResult = base.GetObjects(@"ROOT\CCM", "SELECT * FROM SMS_MPProxyInformation Where State = 'Active'");</code></example>
  public List<PSObject> GetObjects(string WMINamespace, string WQLQuery)
  {
    return this.GetObjects(WMINamespace, WQLQuery, false);
  }

  /// <summary>
  /// Gets a list of PSObjects from cache or from a given WMI namespace using the PowerShell CmdLet Get-CimInstance
  /// </summary>
  /// <param name="WMINamespace">The WMI namespace.</param>
  /// <param name="WQLQuery">The WQL query.</param>
  /// <returns>Command results as list of PSObjects.</returns>
  /// <example><code>List&lt;PSObject&gt; lResult = base.GetObjects(@"ROOT\CCM", "SELECT * FROM SMS_MPProxyInformation Where State = 'Active'");</code></example>
  public List<PSObject> GetCimObjects(string WMINamespace, string WQLQuery)
  {
    return this.GetCimObjects(WMINamespace, WQLQuery, false);
  }

  /// <summary>
  /// Gets a list of PSObjects from cache(if Reload==False) or from a given WMI namespace the PowerShell CmdLet Get-CimInstance
  /// </summary>
  /// <param name="WMINamespace">The WMI namespace.</param>
  /// <param name="WQLQuery">The WQL query.</param>
  /// <param name="Reload">Enforce reload. i.e. don't use cached results.</param>
  /// <returns>Command results as a list of PSObjects.</returns>
  /// <example><code>List&lt;PSObject&gt; lResult = base.GetObjects(@"ROOT\CCM", "SELECT * FROM SMS_MPProxyInformation Where State = 'Active'", True);</code></example>
  public List<PSObject> GetObjects(string WMINamespace, string WQLQuery, bool Reload)
  {
    return this.GetObjects(WMINamespace, WQLQuery, Reload, this.cacheTime);
  }

  /// <summary>
  /// Gets a list of PSObjects from cache(if Reload==False) or from a given WMI namespace using the PowerShell CmdLet Get-CimInstance
  /// </summary>
  /// <param name="WMINamespace">The WMI namespace.</param>
  /// <param name="WQLQuery">The WQL query.</param>
  /// <param name="Reload">Enforce reload. i.e. don't use cached results.</param>
  /// <returns>Command results as a list of PSObjects.</returns>
  /// <example><code>List&lt;PSObject&gt; lResult = base.GetObjects(@"ROOT\CCM", "SELECT * FROM SMS_MPProxyInformation Where State = 'Active'", True);</code></example>
  public List<PSObject> GetCimObjects(string WMINamespace, string WQLQuery, bool Reload)
  {
    return this.GetCimObjects(WMINamespace, WQLQuery, Reload, this.cacheTime);
  }

  /// <summary>
  /// Gets a list of PSObjects from cache(if Reload==False) or from a given WMI namespace using the PowerShell CmdLet Get-CimInstance
  /// </summary>
  /// <param name="WMINamespace">The WMI namespace.</param>
  /// <param name="WQLQuery">The WQL query.</param>
  /// <param name="Reload">Enforce reload. i.e. don't use cached results.</param>
  /// <param name="tCacheTime">Custom cache time.</param>
  /// <returns>Command results as a list of PSObjects.</returns>
  /// <example><code>List&lt;PSObject&gt; lResult = base.GetObjects(@"ROOT\CCM", "SELECT * FROM SMS_MPProxyInformation Where State = 'Active'", True, new TimeSpan(0,0,30));</code></example>
  public List<PSObject> GetObjects(
    string WMINamespace,
    string WQLQuery,
    bool Reload,
    TimeSpan tCacheTime)
  {
    List<PSObject> objects = new List<PSObject>();
    string str = $"Get-CimInstance -query \"{WQLQuery}\" -namespace \"{WMINamespace}\"";
    if (!this.bShowPSCodeOnly)
    {
      string hash = this.CreateHash(WMINamespace + WQLQuery);
      if (this.Cache.Get(hash, (string) null) != null & !Reload)
      {
        objects = this.Cache.Get(hash, (string) null) as List<PSObject>;
      }
      else
      {
        foreach (PSObject psObject in WSMan.RunPSScript(str, this.remoteRunspace))
        {
          try
          {
            objects.Add(psObject);
          }
          catch (Exception ex)
          {
            Trace.WriteLineIf(this.debugLevel.TraceError, ex.Message);
          }
        }
        this.Cache.Add(hash, (object) objects, (DateTimeOffset) (DateTime.Now + this.cacheTime));
      }
    }
    this.tsPSCode.TraceInformation(str);
    return objects;
  }

  /// <summary>
  /// Gets a list of PSObjects from cache(if Reload==False) or from a given WMI namespace using the PowerShell CmdLet Get-CimInstance
  /// </summary>
  /// <param name="WMINamespace">The WMI namespace.</param>
  /// <param name="WQLQuery">The WQL query.</param>
  /// <param name="Reload">Enforce reload. i.e. don't use cached results.</param>
  /// <param name="tCacheTime">Custom cache time.</param>
  /// <returns>Command results as a list of PSObjects.</returns>
  /// <example><code>List&lt;PSObject&gt; lResult = base.GetObjects(@"ROOT\CCM", "SELECT * FROM SMS_MPProxyInformation Where State = 'Active'", True, new TimeSpan(0,0,30));</code></example>
  public List<PSObject> GetCimObjects(
    string WMINamespace,
    string WQLQuery,
    bool Reload,
    TimeSpan tCacheTime)
  {
    List<PSObject> cimObjects = new List<PSObject>();
    string str = $"Get-CimInstance -query \"{WQLQuery}\" -namespace \"{WMINamespace}\"";
    if (!this.bShowPSCodeOnly)
    {
      string hash = this.CreateHash(WMINamespace + WQLQuery);
      if (this.Cache.Get(hash, (string) null) != null & !Reload)
      {
        cimObjects = this.Cache.Get(hash, (string) null) as List<PSObject>;
      }
      else
      {
        foreach (PSObject psObject in WSMan.RunPSScript(str, this.remoteRunspace))
        {
          try
          {
            cimObjects.Add(psObject);
          }
          catch (Exception ex)
          {
            Trace.WriteLineIf(this.debugLevel.TraceError, ex.Message);
          }
        }
        this.Cache.Add(hash, (object) cimObjects, (DateTimeOffset) (DateTime.Now + this.cacheTime));
      }
    }
    this.tsPSCode.TraceInformation(str);
    return cimObjects;
  }

  /// <summary>Get Object from PowerShell Command</summary>
  /// <param name="PSCode">PowerShell code</param>
  /// <returns>Command results as a list of PSObjects.</returns>
  /// <example><code>List&lt;PSObject&gt; lResult = base.GetObjectsFromPS("(Get-ItemProperty(\"HKLM:\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\")).$(\"PendingFileRenameOperations\")");</code></example>
  public List<PSObject> GetObjectsFromPS(string PSCode)
  {
    return this.GetObjectsFromPS(PSCode, false, this.cacheTime);
  }

  /// <summary>Get Object from PowerShell Command</summary>
  /// <param name="PSCode">PowerShell code</param>
  /// <param name="Reload">Ignore cached results, always reload Objects</param>
  /// <returns>Command results as a list of PSObjects.</returns>
  /// <example><code>List&lt;PSObject&gt; lResult = base.GetObjectsFromPS("(Get-ItemProperty(\"HKLM:\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\")).$(\"PendingFileRenameOperations\")", True);</code></example>
  public List<PSObject> GetObjectsFromPS(string PSCode, bool Reload)
  {
    return this.GetObjectsFromPS(PSCode, Reload, this.cacheTime);
  }

  /// <summary>Get Object from PowerShell Command</summary>
  /// <param name="PSCode">PowerShell code</param>
  /// <param name="Reload">enforce reload</param>
  /// <param name="tCacheTime">custom cache time</param>
  /// <returns>Command results as list of PSObjects.</returns>
  /// <example><code>List&lt;PSObject&gt; lResult = base.GetObjectsFromPS("(Get-ItemProperty(\"HKLM:\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\")).$(\"PendingFileRenameOperations\")", True, new TimeSpan(0,0,30));</code></example>
  public List<PSObject> GetObjectsFromPS(string PSCode, bool Reload, TimeSpan tCacheTime)
  {
    List<PSObject> objectsFromPs = new List<PSObject>();
    if (!this.bShowPSCodeOnly)
    {
      string hash = this.CreateHash(PSCode);
      if (this.Cache.Get(hash, (string) null) != null & !Reload)
      {
        objectsFromPs = this.Cache.Get(hash, (string) null) as List<PSObject>;
      }
      else
      {
        foreach (PSObject psObject in WSMan.RunPSScript(PSCode, this.remoteRunspace))
        {
          try
          {
            objectsFromPs.Add(psObject);
          }
          catch (Exception ex)
          {
            Trace.WriteLineIf(this.debugLevel.TraceError, ex.Message);
          }
        }
        this.Cache.Add(hash, (object) objectsFromPs, (DateTimeOffset) (DateTime.Now + tCacheTime));
      }
    }
    this.tsPSCode.TraceInformation(PSCode);
    return objectsFromPs;
  }
}
