// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.DDRFile
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

#nullable disable
namespace sccmclictr.automation;

/// <summary>Create a DataDiscoveryRecord (DDR) File</summary>
public class DDRFile
{
  internal StringBuilder sDDR = new StringBuilder();
  internal static string sArchitecture;
  internal static string sAgentName;
  internal static string sSiteCode;

  /// <summary>Create a new DDR File Structure</summary>
  /// <param name="Architecture">Name of the Archtecture (like "System")</param>
  /// <param name="AgentName">Name of the Discovery Agent</param>
  /// <param name="SiteCode">3 Digit SMS Site Code</param>
  public DDRFile(string Architecture, string AgentName, string SiteCode)
  {
    DDRFile.sArchitecture = Architecture;
    DDRFile.sAgentName = AgentName;
    DDRFile.sSiteCode = SiteCode;
    this.sDDR.AppendLine($"<{DDRFile.sArchitecture}>");
  }

  /// <summary>Create the DDR File</summary>
  /// <param name="FileName">Full Path and Filname</param>
  public void DDRWrite(string FileName)
  {
    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
    this.sDDR.AppendLine($"{$"AGENTINFO<{DDRFile.sAgentName}><{DDRFile.sSiteCode}>"}<{DateTime.Now.ToString("M/d/yyyy H:m:s")}>");
    this.sDDR.Append("FEOF");
    int num = this.sDDR.Length + 17;
    byte[] buffer1 = new byte[16 /*0x10*/]
    {
      (byte) 1,
      (byte) 0,
      (byte) 216,
      (byte) 4,
      (byte) 0,
      (byte) 0,
      (byte) 70,
      (byte) 86,
      (byte) 160 /*0xA0*/,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      BitConverter.GetBytes(num)[0],
      BitConverter.GetBytes(num)[1],
      (byte) 0,
      (byte) 0
    };
    byte[] buffer2 = new byte[5]
    {
      (byte) 13,
      (byte) 0,
      (byte) 0,
      (byte) 70,
      (byte) 86
    };
    FileStream fileStream = new FileStream(FileName, FileMode.Create);
    StreamWriter streamWriter = new StreamWriter((Stream) fileStream, Encoding.Default);
    fileStream.Write(buffer1, 0, buffer1.Length);
    streamWriter.Write((object) this.sDDR);
    streamWriter.Flush();
    fileStream.Write(buffer2, 0, buffer2.Length);
    fileStream.Close();
    fileStream.Dispose();
  }

  internal void AddBegin() => this.sDDR.AppendLine("BEGIN_PROPERTY");

  internal void AddEnd() => this.sDDR.AppendLine("END_PROPERTY");

  internal void AddBeginArray() => this.sDDR.AppendLine("BEGIN_ARRAY_VALUES");

  internal void AddEndArray() => this.sDDR.AppendLine("END_ARRAY_VALUES");

  /// <summary>Add a string property</summary>
  /// <param name="Name"></param>
  /// <param name="Value"></param>
  /// <param name="SQLWidth"></param>
  /// <param name="DDRPropertyFlag"></param>
  public void DDRAddString(
    string Name,
    string Value,
    int SQLWidth,
    DDRFile.DDRPropertyFlagsEnum DDRPropertyFlag)
  {
    if (Value.Length > SQLWidth)
      Value = Value.Substring(0, SQLWidth);
    this.AddBegin();
    this.sDDR.AppendLine(string.Format("<{3}><{0}><{4}><{2}><{1}>", (object) Name, (object) Value, (object) SQLWidth.ToString(), (object) ((int) DDRPropertyFlag).ToString(), (object) "11"));
    this.AddEnd();
  }

  /// <summary>Add a string array property</summary>
  /// <param name="Name"></param>
  /// <param name="Value"></param>
  /// <param name="SQLWidth"></param>
  /// <param name="DDRPropertyFlag"></param>
  public void DDRAddStringArray(
    string Name,
    object Value,
    int SQLWidth,
    DDRFile.DDRPropertyFlagsEnum DDRPropertyFlag)
  {
    this.AddBegin();
    object[] objArray1 = new object[5]
    {
      (object) Name,
      Value,
      (object) SQLWidth.ToString(),
      (object) ((int) (DDRPropertyFlag | DDRFile.DDRPropertyFlagsEnum.ADDPROP_ARRAY)).ToString(),
      (object) "11"
    };
    object[] objArray2 = Value as object[];
    this.sDDR.AppendLine(string.Format("<{3}><{0}><{4}><{2}>", objArray1));
    this.AddBeginArray();
    foreach (object obj in objArray2)
      this.sDDR.AppendLine($"<{obj.ToString()}>");
    this.AddEndArray();
    this.AddEnd();
  }

  /// <summary>Add an Integer Property</summary>
  /// <param name="Name"></param>
  /// <param name="Value"></param>
  /// <param name="DDRPropertyFlag"></param>
  public void DDRAddInteger(string Name, int Value, DDRFile.DDRPropertyFlagsEnum DDRPropertyFlag)
  {
    this.AddBegin();
    this.sDDR.AppendLine(string.Format("<{3}><{0}><{4}><{2}><{1}>", (object) Name, (object) Value, (object) 4, (object) ((int) DDRPropertyFlag).ToString(), (object) "8"));
    this.AddEnd();
  }

  /// <summary>Add an Integer Array Property</summary>
  /// <param name="Name"></param>
  /// <param name="Value"></param>
  /// <param name="DDRPropertyFlag"></param>
  public void DDRAddIntegerArray(
    string Name,
    object Value,
    DDRFile.DDRPropertyFlagsEnum DDRPropertyFlag)
  {
    this.AddBegin();
    object[] objArray1 = new object[5]
    {
      (object) Name,
      Value,
      (object) 4,
      (object) ((int) (DDRPropertyFlag | DDRFile.DDRPropertyFlagsEnum.ADDPROP_ARRAY)).ToString(),
      (object) "8"
    };
    object[] objArray2 = Value as object[];
    this.sDDR.AppendLine(string.Format("<{3}><{0}><{4}><{2}>", objArray1));
    this.AddBeginArray();
    foreach (object obj in objArray2)
      this.sDDR.AppendLine($"<{obj.ToString()}>");
    this.AddEndArray();
    this.AddEnd();
  }

  /// <summary>Add DateTime value (MM/DD/YY HH:MM:SS)</summary>
  /// <param name="Name"></param>
  /// <param name="Value"></param>
  /// <param name="DDRPropertyFlag"></param>
  public void DDRAddDateTime(
    string Name,
    DateTime Value,
    DDRFile.DDRPropertyFlagsEnum DDRPropertyFlag)
  {
    this.AddBegin();
    this.sDDR.AppendLine(string.Format("<{3}><{0}><{4}><{2}><{1}>", (object) Name, (object) Value.ToString("MM/dd/yy HH:mm:ss", (IFormatProvider) CultureInfo.InvariantCulture), (object) 4, (object) ((int) DDRPropertyFlag).ToString(), (object) "12"));
    this.AddEnd();
  }

  /// <summary>DDR Property Flags</summary>
  [Flags]
  public enum DDRPropertyFlagsEnum
  {
    /// <summary>Reserved.</summary>
    ADDPROP_AGENT = 32, // 0x00000020
    /// <summary>Value is an Array value</summary>
    ADDPROP_ARRAY = 16, // 0x00000010
    /// <summary>Reserved.</summary>
    ADDPROP_GROUPING = 4,
    /// <summary>Defines this property as being a GUID.</summary>
    ADDPROP_GUID = 2,
    /// <summary>
    /// Defines this property as being a Key value that must be unique.
    /// </summary>
    ADDPROP_KEY = 8,
    /// <summary>
    /// Specifies this property as the actual Name property in the resource.
    /// </summary>
    ADDPROP_NAME = 68, // 0x00000044
    /// <summary>
    /// Specifies this property as the actual Comment property in the resource.
    /// </summary>
    ADDPROP_NAME2 = 132, // 0x00000084
    /// <summary>replace existing values</summary>
    ADDPROP_REPLACE = 1,
    /// <summary>No special properties.</summary>
    ADDPROP_NONE = 0,
  }
}
