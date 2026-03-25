// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.SCCMAgent
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using System;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Runtime.InteropServices;
using System.Security;

#nullable disable
namespace sccmclictr.automation;

/// <summary>SCCMAgent Main Class</summary>
public class SCCMAgent : IDisposable
{
  private WSManConnectionInfo connectionInfo;
  internal const int RESOURCETYPE_ANY = 0;
  /// <summary>The client</summary>
  public ccm Client;

  private PSCredential Credential { get; set; }

  private string Hostname { get; set; }

  private int WSManPort { get; set; }

  private Runspace remoteRunspace { get; set; }

  private bool ipcconnected { get; set; }

  /// <summary>
  /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
  /// </summary>
  public void Dispose()
  {
    try
    {
      if (this.isConnected)
        this.disconnect();
    }
    catch
    {
    }
    this.connectionInfo = (WSManConnectionInfo) null;
    this.Credential = (PSCredential) null;
    this.Hostname = (string) null;
    if (this.Client != null)
      this.Client.Dispose();
    if (this.remoteRunspace == null)
      return;
    this.remoteRunspace.Dispose();
  }

  /// <summary>Gets the connection information.</summary>
  /// <value>The connection information.</value>
  public WSManConnectionInfo ConnectionInfo => this.connectionInfo;

  /// <summary>Get the connected computername.</summary>
  public string TargetHostname
  {
    get
    {
      try
      {
        return this.ConnectionInfo.ComputerName;
      }
      catch
      {
        return "";
      }
    }
  }

  /// <summary>TraceSource for all PowerShell Command</summary>
  public TraceSource PSCode { get; set; }

  /// <summary>
  /// True = Session to remote Host is Open
  /// False = Session to remote Host is not Open
  /// </summary>
  public bool isConnected
  {
    get
    {
      try
      {
        if (this.remoteRunspace.RunspaceStateInfo.State == RunspaceState.Opened)
          return true;
      }
      catch
      {
      }
      return false;
    }
  }

  /// <summary>Connect to a remote SCCM Agent by using WSMan</summary>
  /// <param name="hostname">target computername</param>
  public SCCMAgent(string hostname)
  {
    this.Initialize(hostname, (PSCredential) null, 5985, true, false);
  }

  /// <summary>Connect to a remote SCCM Agent by using WSMan</summary>
  /// <param name="hostname">target computername</param>
  /// <param name="username">username for the connection</param>
  /// <param name="password">password for the connection</param>
  public SCCMAgent(string hostname, string username, string password)
  {
    this.Initialize(hostname, username, password, 5985, true, false);
  }

  /// <summary>Connect to a remote SCCM Agent by using WSMan</summary>
  /// <param name="hostname">target computername</param>
  /// <param name="username">username for the connection</param>
  /// <param name="password">password for the connection</param>
  public SCCMAgent(string hostname, string username, SecureString password)
  {
    PSCredential credential = new PSCredential(username, password);
    this.Initialize(hostname, credential, 5985, true, false);
  }

  /// <summary>Connect to a remote SCCM Agent by using WSMan</summary>
  /// <param name="hostname">target computername</param>
  /// <param name="credential">PSCredential used to connect</param>
  public SCCMAgent(string hostname, PSCredential credential)
  {
    this.Initialize(hostname, credential, 5985, true, false);
  }

  /// <summary>Connect to a remote SCCM Agent by using WSMan</summary>
  /// <param name="hostname">target computername</param>
  /// <param name="username">username for the connection</param>
  /// <param name="password">password for the connection</param>
  /// <param name="wsManPort">WSManagement Port (Default = 5985)</param>
  /// <param name="Connect">automatically connect after initializing</param>
  public SCCMAgent(
    string hostname,
    string username,
    string password,
    int wsManPort,
    bool Connect)
  {
    this.Initialize(hostname, username, password, wsManPort, Connect, false);
  }

  /// <summary>Connect to a remote SCCM Agent by using WSMan</summary>
  /// <param name="hostname">target computername</param>
  /// <param name="username">username for the connection</param>
  /// <param name="password">password for the connection</param>
  /// <param name="wsManPort">WSManagement Port (Default = 5985)</param>
  /// <param name="Connect">automatically connect after initializing</param>
  public SCCMAgent(
    string hostname,
    string username,
    SecureString password,
    int wsManPort,
    bool Connect)
  {
    PSCredential credential = new PSCredential(username, password);
    this.Initialize(hostname, credential, wsManPort, Connect, false);
  }

  /// <summary>Connect to a remote SCCM Agent by using WSMan</summary>
  /// <param name="hostname">target computername</param>
  /// <param name="credential">PSCredential used to connect</param>
  /// <param name="wsManPort">WSManagement Port (Default = 5985)</param>
  /// <param name="Connect">automatically connect after initializing</param>
  public SCCMAgent(string hostname, PSCredential credential, int wsManPort, bool Connect)
  {
    this.Initialize(hostname, credential, wsManPort, Connect, false);
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.SCCMAgent" /> class.
  /// </summary>
  /// <param name="hostname">The hostname.</param>
  /// <param name="username">The username.</param>
  /// <param name="password">The password.</param>
  /// <param name="wsManPort">The ws man port.</param>
  /// <param name="Connect">if set to <c>true</c> [connect].</param>
  /// <param name="encryption">if set to <c>true</c> [encryption].</param>
  public SCCMAgent(
    string hostname,
    string username,
    string password,
    int wsManPort,
    bool Connect,
    bool encryption)
  {
    this.Initialize(hostname, username, password, wsManPort, Connect, encryption);
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.SCCMAgent" /> class.
  /// </summary>
  /// <param name="hostname">The hostname.</param>
  /// <param name="username">The username.</param>
  /// <param name="password">The password.</param>
  /// <param name="wsManPort">The ws man port.</param>
  /// <param name="Connect">if set to <c>true</c> [connect].</param>
  /// <param name="encryption">if set to <c>true</c> [encryption].</param>
  public SCCMAgent(
    string hostname,
    string username,
    SecureString password,
    int wsManPort,
    bool Connect,
    bool encryption)
  {
    PSCredential credential = new PSCredential(username, password);
    this.Initialize(hostname, credential, wsManPort, Connect, encryption);
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.SCCMAgent" /> class.
  /// </summary>
  /// <param name="hostname">The hostname.</param>
  /// <param name="credential">PSCredential used to connect</param>
  /// <param name="wsManPort">The ws man port.</param>
  /// <param name="Connect">if set to <c>true</c> [connect].</param>
  /// <param name="encryption">if set to <c>true</c> [encryption].</param>
  public SCCMAgent(
    string hostname,
    PSCredential credential,
    int wsManPort,
    bool Connect,
    bool encryption)
  {
    this.Initialize(hostname, credential, wsManPort, Connect, encryption);
  }

  /// <summary>Connect to a remote SCCM Agent by using WSMan</summary>
  /// <param name="hostname">target computername</param>
  /// <param name="username">username for the connection</param>
  /// <param name="password">password for the connection</param>
  /// <param name="wsManPort">WSManagement Port (Default = 5985)</param>
  public SCCMAgent(string hostname, string username, string password, int wsManPort)
  {
    this.Initialize(hostname, username, password, wsManPort, true, false);
  }

  /// <summary>Connect to a remote SCCM Agent by using WSMan</summary>
  /// <param name="hostname">target computername</param>
  /// <param name="username">username for the connection</param>
  /// <param name="password">password for the connection</param>
  /// <param name="wsManPort">WSManagement Port (Default = 5985)</param>
  public SCCMAgent(string hostname, string username, SecureString password, int wsManPort)
  {
    PSCredential credential = new PSCredential(username, password);
    this.Initialize(hostname, credential, wsManPort, true, false);
  }

  /// <summary>Connect to a remote SCCM Agent by using WSMan</summary>
  /// <param name="hostname">target computername</param>
  /// <param name="credential">PSCredential used to connect</param>
  /// <param name="wsManPort">WSManagement Port (Default = 5985)</param>
  public SCCMAgent(string hostname, PSCredential credential, int wsManPort)
  {
    this.Initialize(hostname, credential, wsManPort, true, false);
  }

  /// <summary>Connect to a remote computer by using WSMan</summary>
  /// <param name="hostname">target computername</param>
  /// <param name="username">username for the connectio</param>
  /// <param name="password">password for the connection</param>
  /// <param name="wsManPort">WSManagement Port (Default = 5985)</param>
  /// <param name="bConnect">Only prepare the connection, connection must be initialized with 'reconnect'</param>
  /// <param name="Encryption">Enable encryption'</param>
  protected void Initialize(
    string hostname,
    string username,
    string password,
    int wsManPort,
    bool bConnect,
    bool Encryption)
  {
    this.Hostname = hostname;
    SecureString securePassword = new SecureString();
    foreach (char c in password)
      securePassword.AppendChar(c);
    securePassword.MakeReadOnly();
    PSCredential credential = new PSCredential(username, securePassword);
    this.Initialize(hostname, credential, wsManPort, bConnect, Encryption);
  }

  /// <summary>Connect to a remote computer by using WSMan</summary>
  /// <param name="hostname">target computername</param>
  /// <param name="credential">PSCredential used to connect</param>
  /// <param name="wsManPort">WSManagement Port (Default = 5985)</param>
  /// <param name="bConnect">Only prepare the connection, connection must be initialized with 'reconnect'</param>
  /// <param name="Encryption">Enable encryption'</param>
  protected void Initialize(
    string hostname,
    PSCredential credential,
    int wsManPort,
    bool bConnect,
    bool Encryption)
  {
    this.Hostname = hostname;
    this.Credential = credential;
    this.ipcconnected = false;
    this.PSCode = new TraceSource("PSCode");
    this.PSCode.Switch.Level = SourceLevels.All;
    if (credential == null)
    {
      this.connectionInfo = Encryption ? new WSManConnectionInfo(new Uri($"https://{hostname}:{wsManPort}/wsman")) : new WSManConnectionInfo(new Uri($"http://{hostname}:{wsManPort}/wsman"));
      this.ipcconnected = true;
    }
    else
      this.connectionInfo = Encryption ? new WSManConnectionInfo(new Uri($"https://{hostname}:{wsManPort}/wsman"), "http://schemas.microsoft.com/powershell/Microsoft.PowerShell", credential) : new WSManConnectionInfo(new Uri($"http://{hostname}:{wsManPort}/wsman"), "http://schemas.microsoft.com/powershell/Microsoft.PowerShell", credential);
    this.connectionInfo.AuthenticationMechanism = AuthenticationMechanism.Default;
    this.connectionInfo.ProxyAuthentication = AuthenticationMechanism.Negotiate;
    this.connectionInfo.OpenTimeout = 60000;
    this.connectionInfo.OperationTimeout = 60000;
    this.connectionInfo.OpenTimeout = 60000;
    this.connectionInfo.CancelTimeout = 10000;
    this.connectionInfo.IdleTimeout = 60000;
    this.connectionInfo.NoMachineProfile = true;
    if (!bConnect)
      return;
    this.connect();
  }

  /// <summary>Connects this instance.</summary>
  /// <exception cref="T:System.Exception">Unable to connect</exception>
  public void connect()
  {
    Runspace remoteRunspace = (Runspace) null;
    if (this.Client != null)
      this.Client.Dispose();
    try
    {
      WSMan.openRunspace(this.connectionInfo, ref remoteRunspace);
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      this.PSCode.TraceInformation(ex.Message);
    }
    if (remoteRunspace.RunspaceStateInfo.State != RunspaceState.Opened)
      throw new Exception("Unable to connect");
    this.remoteRunspace = remoteRunspace;
    this.Client = new ccm(this.remoteRunspace, this.PSCode);
  }

  /// <summary>Disconnect an open connection</summary>
  public void disconnect()
  {
    this.Client.Cache.Dispose();
    this.remoteRunspace.Close();
    this.Client.Dispose();
  }

  /// <summary>
  /// Connect the IPC$ share on a remote computer to preauthenticate.
  /// Password is extracted from SecureString only for the P/Invoke call
  /// and immediately freed from unmanaged memory.
  /// </summary>
  internal int connectIPC(string hostname, PSCredential credential)
  {
    var netResource = new SCCMAgent.NETRESOURCE()
    {
      dwType = 0,
      RemoteName = "\\\\" + hostname
    };
    IntPtr passwordPtr = IntPtr.Zero;
    try
    {
      passwordPtr = Marshal.SecureStringToGlobalAllocUnicode(credential.Password);
      return SCCMAgent.WNetAddConnection3(IntPtr.Zero, ref netResource,
        Marshal.PtrToStringUni(passwordPtr), credential.UserName, 0);
    }
    finally
    {
      if (passwordPtr != IntPtr.Zero)
        Marshal.ZeroFreeGlobalAllocUnicode(passwordPtr);
    }
  }

  /// <summary>
  /// Connect the IPC$ Share if no integrated authentication is used
  /// </summary>
  public bool ConnectIPC_
  {
    get => this.ipcconnected;
    set
    {
      if (!value)
        return;
      try
      {
        if (this.Credential != null)
          this.connectIPC(this.Hostname, this.Credential);
        this.ipcconnected = true;
      }
      catch
      {
        this.ipcconnected = false;
      }
    }
  }

  public bool ConnectIPC(PSCredential credential)
  {
    try
    {
      this.Credential = credential;
      if (this.Credential != null)
        this.connectIPC(this.Hostname, this.Credential);
      this.ipcconnected = true;
    }
    catch
    {
      this.ipcconnected = false;
      return false;
    }
    return true;
  }

  public bool ConnectIPC(string username, SecureString password)
  {
    return this.ConnectIPC(new PSCredential(username, password));
  }

  [DllImport("mpr.dll")]
  private static extern int WNetAddConnection3(
    IntPtr hWndOwner,
    ref SCCMAgent.NETRESOURCE lpNetResource,
    string lpPassword,
    string lpUserName,
    int dwFlags);

  private struct NETRESOURCE
  {
    internal int dwScope;
    internal int dwType;
    internal int dwDisplayType;
    internal int dwUsage;
    internal string LocalName;
    internal string RemoteName;
    internal string Comment;
    internal string Provider;
  }
}
