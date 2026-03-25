// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.WSMan
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Text;

#nullable disable
namespace sccmclictr.automation;

internal static class WSMan
{
  /// <summary>Connect a remote Runspace</summary>
  /// <param name="connectionInfo">WSManConnectionInfo</param>
  /// <param name="remoteRunspace">Reference to a Runspace</param>
  internal static void openRunspace(WSManConnectionInfo connectionInfo, ref Runspace remoteRunspace)
  {
    remoteRunspace = RunspaceFactory.CreateRunspace((RunspaceConnectionInfo) connectionInfo);
    remoteRunspace.Open();
  }

  /// <summary>Run a PSScript</summary>
  /// <param name="scriptText"></param>
  /// <param name="remoteRunspace"></param>
  /// <returns></returns>
  internal static Collection<PSObject> RunPSScript(string scriptText, Runspace remoteRunspace)
  {
    try
    {
      using (PowerShell powerShell = PowerShell.Create())
      {
        powerShell.Runspace = remoteRunspace;
        powerShell.AddScript(scriptText);
        List<PSObject> list = powerShell.Invoke().Where<PSObject>((Func<PSObject, bool>) (t => t != null)).ToList<PSObject>();
        Collection<PSObject> collection = new Collection<PSObject>();
        foreach (PSObject psObject in list)
        {
          if (psObject != null)
            collection.Add(psObject);
        }
        if (list.Count == 0)
        {
          foreach (object obj in powerShell.Streams.Error.ReadAll())
          {
            PSObject psObject = new PSObject(obj);
            collection.Add(psObject);
          }
        }
        return collection;
      }
    }
    catch
    {
    }
    return (Collection<PSObject>) null;
  }

  /// <summary>Run a PSScript and return the result as string</summary>
  /// <param name="scriptText"></param>
  /// <param name="remoteRunspace"></param>
  /// <returns></returns>
  internal static string RunPSScriptAsString(string scriptText, Runspace remoteRunspace)
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (PSObject psObject in WSMan.RunPSScript(scriptText, remoteRunspace))
    {
      try
      {
        if (psObject != null)
          stringBuilder.AppendLine(psObject.ToString());
      }
      catch
      {
      }
    }
    return stringBuilder.ToString();
  }

  private static void openRunspace(
    string uri,
    string schema,
    string username,
    string livePass,
    ref Runspace remoteRunspace)
  {
    SecureString password = new SecureString();
    foreach (char c in livePass.ToCharArray())
      password.AppendChar(c);
    PSCredential credential = new PSCredential(username, password);
    WSManConnectionInfo connectionInfo = new WSManConnectionInfo(new Uri(uri), schema, credential);
    connectionInfo.AuthenticationMechanism = AuthenticationMechanism.Kerberos;
    connectionInfo.ProxyAuthentication = AuthenticationMechanism.Negotiate;
    remoteRunspace = RunspaceFactory.CreateRunspace((RunspaceConnectionInfo) connectionInfo);
    remoteRunspace.Open();
  }

  private static void openRunspace(string uri, ref Runspace remoteRunspace)
  {
    WSManConnectionInfo connectionInfo = new WSManConnectionInfo(new Uri(uri));
    connectionInfo.AuthenticationMechanism = AuthenticationMechanism.Kerberos;
    connectionInfo.ProxyAuthentication = AuthenticationMechanism.Negotiate;
    remoteRunspace = RunspaceFactory.CreateRunspace((RunspaceConnectionInfo) connectionInfo);
    remoteRunspace.Open();
  }

  private static string RunScriptAsString(
    string scriptText,
    string servername,
    string username,
    string password)
  {
    return WSMan.RunScriptAsString(scriptText, servername, username, password, 5985);
  }

  private static string RunScriptAsString(
    string scriptText,
    string servername,
    string username,
    string password,
    int port)
  {
    Runspace remoteRunspace = (Runspace) null;
    if (!string.IsNullOrEmpty(username))
      WSMan.openRunspace($"http://{servername}:{port}/wsman", "http://schemas.microsoft.com/powershell/Microsoft.PowerShell", username, password, ref remoteRunspace);
    else
      WSMan.openRunspace($"http://{servername}:{port}/wsman", ref remoteRunspace);
    StringBuilder stringBuilder = new StringBuilder();
    using (PowerShell powerShell = PowerShell.Create())
    {
      powerShell.Runspace = remoteRunspace;
      powerShell.AddScript(scriptText);
      powerShell.Invoke();
      Collection<PSObject> collection = powerShell.Invoke();
      remoteRunspace.Close();
      foreach (PSObject psObject in collection)
      {
        try
        {
          stringBuilder.AppendLine(psObject.ToString());
        }
        catch
        {
        }
      }
    }
    return stringBuilder.ToString();
  }
}
