// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.policy.localpolicy
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;

#nullable disable
namespace sccmclictr.automation.policy;

/// <summary>Class localpolicy.</summary>
public class localpolicy
{
  /// <summary>Download policy from URL (or File)</summary>
  /// <param name="URL">e.g. http://win-29hctu7qses.corp.lab/SMS_MP/.sms_pol?{ce839d51-8469-42b2-ae09-c5a8faaa1ef7}.7_00</param>
  /// <returns>XML Body</returns>
  public static string DownloadPolicyFromURL(string URL)
  {
    XmlDocument xmlDocument = new XmlDocument();
    xmlDocument.Load(URL);
    try
    {
      XmlNode xmlNode = xmlDocument.SelectSingleNode("PolicyXML");
      if (string.Compare(xmlNode.Attributes["Compression"].Value, "zlib", true) == 0)
        return localpolicy.DecompressPolicy(xmlNode.InnerText);
    }
    catch
    {
    }
    return xmlDocument.InnerXml;
  }

  /// <summary>Decompress the hexstring of compressed policies...</summary>
  /// <param name="PolicyHexData">a string like: 789CDD96CB6EDA501086675DA9EF80B2A7...</param>
  /// <returns>the decompressed data (normaly XML) as string.</returns>
  public static string DecompressPolicy(string PolicyHexData)
  {
    string str = "";
    try
    {
      using (StreamReader streamReader = new StreamReader((Stream) new DeflateStream((Stream) new MemoryStream(localpolicy._stringToByteArray(PolicyHexData.Substring(4))), CompressionMode.Decompress)))
        str = streamReader.ReadToEnd();
    }
    catch (Exception ex)
    {
      Trace.WriteLine(ex.Message);
    }
    return str;
  }

  internal static byte[] _stringToByteArray(string hex)
  {
    byte[] byteArray = hex.Length % 2 != 1 ? new byte[hex.Length >> 1] : throw new Exception("The binary key cannot have an odd number of digits");
    for (int index = 0; index < hex.Length >> 1; ++index)
    {
      try
      {
        byteArray[index] = (byte) ((localpolicy._getHexVal(hex[index << 1]) << 4) + localpolicy._getHexVal(hex[(index << 1) + 1]));
      }
      catch (Exception ex)
      {
        ex.Message.ToString();
      }
    }
    return byteArray;
  }

  internal static int _getHexVal(char hex)
  {
    try
    {
      int num = (int) hex;
      return num - (num < 58 ? 48 /*0x30*/ : 55);
    }
    catch (Exception ex)
    {
      ex.Message.ToString();
    }
    return 0;
  }

  /// <summary>Format xml string into a formated xml structure</summary>
  /// <param name="xmlstring">xml string</param>
  /// <returns>formated xml string</returns>
  public static string FormatXML(string xmlstring)
  {
    XmlDocument xmlDocument = new XmlDocument();
    xmlDocument.LoadXml(xmlstring);
    StringBuilder sb = new StringBuilder();
    XmlTextWriter w = new XmlTextWriter((TextWriter) new StringWriter(sb));
    w.Formatting = Formatting.Indented;
    xmlDocument.Save((XmlWriter) w);
    w.Close();
    return sb.ToString();
  }
}
