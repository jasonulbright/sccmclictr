// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.ccm
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using sccmclictr.automation.functions;
using sccmclictr.automation.policy;
using System.Diagnostics;
using System.Management.Automation.Runspaces;

#nullable disable
namespace sccmclictr.automation;

/// <summary>
/// 
/// </summary>
public class ccm : baseInit
{
  public agentproperties AgentProperties;
  public agentactions AgentActions;
  public softwaredistribution SoftwareDistribution;
  public swcache SWCache;
  public softwareupdates SoftwareUpdates;
  public inventory Inventory;
  public components Components;
  public services Services;
  public processes Process;
  public dcm DCM;
  public locationservices LocationServices;
  public requestedConfig RequestedConfig;
  public actualConfig ActualConfig;
  public monitoring Monitoring;
  public health Health;
  public appv5 AppV5;
  public appv4 AppV4;

  /// <summary>
  /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
  /// </summary>
  public new void Dispose()
  {
    this.AgentProperties.Dispose();
    this.AgentActions.Dispose();
    this.Health.Dispose();
    this.Monitoring.Dispose();
    this.ActualConfig.Dispose();
    this.RequestedConfig.Dispose();
    this.Process.Dispose();
    this.Services.Dispose();
    this.Components.Dispose();
    this.Inventory.Dispose();
    this.SoftwareUpdates.Dispose();
    this.SWCache.Dispose();
    this.SoftwareDistribution.Dispose();
    this.DCM.Dispose();
    this.SWCache.Dispose();
    this.AppV4.Dispose();
    this.AppV5.Dispose();
    this.LocationServices.Dispose();
  }

  /// <summary>Constructor</summary>
  /// <param name="RemoteRunspace"></param>
  /// <param name="PSCode"></param>
  internal ccm(Runspace RemoteRunspace, TraceSource PSCode)
    : base(RemoteRunspace, PSCode)
  {
    this.AgentProperties = new agentproperties(RemoteRunspace, PSCode, this);
    this.AgentActions = new agentactions(RemoteRunspace, PSCode, this);
    this.SoftwareDistribution = new softwaredistribution(RemoteRunspace, PSCode, this);
    this.SWCache = new swcache(RemoteRunspace, PSCode, this);
    this.SoftwareUpdates = new softwareupdates(RemoteRunspace, PSCode, this);
    this.Inventory = new inventory(RemoteRunspace, PSCode, this);
    this.Components = new components(RemoteRunspace, PSCode, this);
    this.RequestedConfig = new requestedConfig(RemoteRunspace, PSCode, this);
    this.ActualConfig = new actualConfig(RemoteRunspace, PSCode, this);
    this.Services = new services(RemoteRunspace, PSCode, this);
    this.Process = new processes(RemoteRunspace, PSCode, this);
    this.Monitoring = new monitoring(RemoteRunspace, PSCode, this);
    this.Health = new health(RemoteRunspace, PSCode, this);
    this.DCM = new dcm(RemoteRunspace, PSCode, this);
    this.AppV5 = new appv5(RemoteRunspace, PSCode, this);
    this.AppV4 = new appv4(RemoteRunspace, PSCode, this);
    this.LocationServices = new locationservices(RemoteRunspace, PSCode, this);
  }
}
