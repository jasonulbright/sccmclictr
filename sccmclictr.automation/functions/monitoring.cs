// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.functions.monitoring
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;

#nullable disable
namespace sccmclictr.automation.functions;

/// <summary>Class monitoring.</summary>
public class monitoring : baseInit
{
  internal Runspace remoteRunspace;
  internal TraceSource pSCode;
  internal ccm baseClient;
  /// <summary>The asynchronous script</summary>
  public monitoring.runScriptAsync AsynchronousScript;

  /// <summary>
  /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.monitoring" /> class.
  /// </summary>
  /// <param name="RemoteRunspace">The remote runspace.</param>
  /// <param name="PSCode">The PowerShell code.</param>
  /// <param name="oClient">A CCM Client object.</param>
  public monitoring(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
    : base(RemoteRunspace, PSCode)
  {
    this.remoteRunspace = RemoteRunspace;
    this.pSCode = PSCode;
    this.baseClient = oClient;
    this.AsynchronousScript = new monitoring.runScriptAsync(RemoteRunspace, this.pSCode);
  }

  /// <summary>Class runScriptAsync.</summary>
  public class runScriptAsync : IDisposable
  {
    private AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
    internal Runspace _remoteRunspace;
    internal Pipeline pipeline;
    internal RunspaceConnectionInfo _connectionInfo;
    internal TraceSource pSCode;

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      if (this._remoteRunspace != null)
        this._remoteRunspace.Dispose();
      if (this.pipeline != null)
        this.pipeline.Dispose();
      this._connectionInfo = (RunspaceConnectionInfo) null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:sccmclictr.automation.functions.monitoring.runScriptAsync" /> class.
    /// </summary>
    /// <param name="remoteRunspace">The remote runspace.</param>
    /// <param name="PSCode">TraceSource</param>
    public runScriptAsync(Runspace remoteRunspace, TraceSource PSCode)
    {
      if (this._remoteRunspace == null)
      {
        this._connectionInfo = remoteRunspace.ConnectionInfo;
        this._remoteRunspace = RunspaceFactory.CreateRunspace(this._connectionInfo);
      }
      else
        this._remoteRunspace = RunspaceFactory.CreateRunspace(remoteRunspace.ConnectionInfo);
      this.pSCode = PSCode;
    }

    /// <summary>Connects this instance.</summary>
    public void Connect()
    {
      if (this._remoteRunspace.RunspaceStateInfo.State != RunspaceState.Opened)
      {
        this._remoteRunspace = RunspaceFactory.CreateRunspace(this._connectionInfo);
        this._remoteRunspace.Open();
      }
      this.pipeline = this._remoteRunspace.CreatePipeline();
      this.pipeline.Output.DataReady += new EventHandler(this.Output_DataReady);
      this.pipeline.StateChanged += new EventHandler<PipelineStateEventArgs>(this.pipeline_StateChanged);
    }

    private void pipeline_StateChanged(object sender, PipelineStateEventArgs e)
    {
      if (e.PipelineStateInfo.State != PipelineState.Running)
      {
        this.pipeline.Output.Close();
        this.pipeline.Input.Close();
      }
      if (e.PipelineStateInfo.State == PipelineState.Completed && this.Finished != null)
        this.Finished((object) this.pipeline.Output, new EventArgs());
      this._autoResetEvent.Set();
    }

    internal void Output_DataReady(object sender, EventArgs e)
    {
      PipelineReader<PSObject> pipelineReader1 = sender as PipelineReader<PSObject>;
      List<string> sender1 = new List<string>();
      List<object> sender2 = new List<object>();
      if (pipelineReader1 != null)
      {
        Collection<PSObject> sender3 = pipelineReader1.NonBlockingRead();
        if (sender3.Count > 0)
        {
          if (this.RawOutput != null)
            this.RawOutput((object) sender3, e);
          foreach (PSObject sourceValue in sender3)
          {
            if (sourceValue != null)
            {
              sender1.Add(sourceValue.ToString());
              foreach (string typeName in sourceValue.TypeNames)
              {
                ConvertThroughString convertThroughString = new ConvertThroughString();
                Type type = Type.GetType(typeName.Replace("Deserialized.", ""));
                if (convertThroughString.CanConvertFrom(sourceValue, type))
                {
                  try
                  {
                    sender2.Add(convertThroughString.ConvertFrom(sourceValue, type, (IFormatProvider) null, true));
                    break;
                  }
                  catch
                  {
                    try
                    {
                      Hashtable hashtable = new Hashtable();
                      foreach (PSPropertyInfo property in sourceValue.Properties)
                      {
                        try
                        {
                          hashtable.Add((object) property.Name, (object) property.Value.ToString());
                        }
                        catch
                        {
                        }
                      }
                      sender2.Add((object) hashtable);
                      break;
                    }
                    catch
                    {
                    }
                  }
                }
              }
            }
          }
        }
        if (pipelineReader1.EndOfPipeline & pipelineReader1.IsOpen)
          pipelineReader1.Close();
      }
      else if (sender is PipelineReader<object> pipelineReader2)
      {
        while (pipelineReader2.Count > 0)
        {
          sender1.Add(pipelineReader2.Read().ToString());
          sender2.Add(pipelineReader2.Read());
        }
        if (pipelineReader2.EndOfPipeline)
          pipelineReader2.Close();
        if (this.ErrorOccured != null)
          this.ErrorOccured(sender, e);
        this._autoResetEvent.Set();
      }
      if (this.StringOutput != null)
        this.StringOutput((object) sender1, e);
      if (this.TypedOutput != null)
        this.TypedOutput((object) sender2, e);
      this._autoResetEvent.Set();
    }

    /// <summary>Powershell Script to execute</summary>
    public string Command
    {
      get
      {
        if (this.pipeline == null)
          this.Connect();
        return this.pipeline.Commands.ToString();
      }
      set
      {
        if (this.pipeline == null)
          this.Connect();
        this.pipeline.Commands.Clear();
        this.pipeline.Commands.AddScript(value);
        this.pSCode.TraceInformation(value);
      }
    }

    /// <summary>Runs this instance.</summary>
    public void Run()
    {
      if (this.pipeline == null)
        this.Connect();
      this.pipeline.InvokeAsync();
      this.pipeline.Input.Close();
    }

    /// <summary>Runs and waits.</summary>
    public void RunWait()
    {
      if (this.pipeline == null)
        this.Connect();
      this.pipeline.InvokeAsync();
      this.pipeline.Input.Close();
      do
      {
        this._autoResetEvent.WaitOne(500);
      }
      while (this.pipeline.Output.IsOpen);
    }

    /// <summary>Stop the pieline reader</summary>
    public void Stop()
    {
      if (this.pipeline == null || !this.pipeline.Output.IsOpen)
        return;
      this.pipeline.Output.Close();
    }

    /// <summary>Check if PS output is open..</summary>
    public bool isRunning
    {
      get => this.pipeline != null && this.pipeline.Output != null && this.pipeline.Output.IsOpen;
    }

    /// <summary>Close the pipeline</summary>
    public void Close()
    {
      try
      {
        if (this.pipeline == null)
          return;
        if (this.pipeline.Output.IsOpen)
          this.pipeline.Output.Close();
        this.pipeline.Output.DataReady -= new EventHandler(this.Output_DataReady);
        this.pipeline.StateChanged -= new EventHandler<PipelineStateEventArgs>(this.pipeline_StateChanged);
        this._remoteRunspace.Close();
      }
      catch
      {
      }
    }

    public event EventHandler StringOutput;

    public event EventHandler RawOutput;

    public event EventHandler TypedOutput;

    public event EventHandler Finished;

    public event EventHandler ErrorOccured;
  }
}
