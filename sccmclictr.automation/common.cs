// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.common
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;

#nullable disable
namespace sccmclictr.automation;

/// <summary>Class common.</summary>
public static class common
{
  /// <summary>Encrypt a string</summary>
  /// <param name="strPlainText"></param>
  /// <param name="strKey"></param>
  /// <returns></returns>
  public static string Encrypt(string strPlainText, string strKey)
  {
    try
    {
      TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider();
      byte[] hash = new SHA1CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(strKey));
      return Convert.ToBase64String(ProtectedData.Protect(Encoding.ASCII.GetBytes(strPlainText), hash, DataProtectionScope.CurrentUser));
    }
    catch (Exception ex)
    {
      ex.Message.ToString();
    }
    return "";
  }

  /// <summary>Decrypt a string</summary>
  /// <param name="strBase64Text"></param>
  /// <param name="strKey"></param>
  /// <returns></returns>
  public static string Decrypt(string strBase64Text, string strKey)
  {
    try
    {
      TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider();
      byte[] hash = new SHA1CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(strKey));
      return Encoding.ASCII.GetString(ProtectedData.Unprotect(Convert.FromBase64String(strBase64Text), hash, DataProtectionScope.CurrentUser));
    }
    catch (Exception ex)
    {
      ex.Message.ToString();
    }
    return "";
  }

  /// <summary>Gets the sha1 hash of the supplied value.</summary>
  /// <param name="value">The value.</param>
  /// <returns>System.String.</returns>
  public static string GetSha1(string value)
  {
    byte[] hash = new SHA1Managed().ComputeHash(Encoding.ASCII.GetBytes(value));
    string empty = string.Empty;
    foreach (byte num in hash)
      empty += num.ToString("X2");
    return empty;
  }

  /// <summary>Get Image from String</summary>
  /// <param name="base64String"></param>
  /// <returns></returns>
  public static Image Base64ToImage(string base64String)
  {
    byte[] buffer = Convert.FromBase64String(base64String);
    MemoryStream memoryStream = new MemoryStream(buffer, 0, buffer.Length);
    memoryStream.Write(buffer, 0, buffer.Length);
    return Image.FromStream((Stream) memoryStream, true);
  }

  /// <summary>Convert Image to string</summary>
  /// <param name="image"></param>
  /// <param name="format"></param>
  /// <returns></returns>
  public static string ImageToBase64(Image image, ImageFormat format)
  {
    using (MemoryStream memoryStream = new MemoryStream())
    {
      image.Save((Stream) memoryStream, format);
      return Convert.ToBase64String(memoryStream.ToArray());
    }
  }

  /// <summary>
  /// Converts a WMI DateTime string to a C# DateTime object
  /// </summary>
  /// <param name="ManagementDateTime">The WMI DateTime string.</param>
  /// <returns>System.Nullable{DateTime}.</returns>
  public static DateTime? WMIDateToDateTime(string ManagementDateTime)
  {
    try
    {
      return string.IsNullOrEmpty(ManagementDateTime) ? new DateTime?() : new DateTime?(ManagementDateTimeConverter.ToDateTime(ManagementDateTime));
    }
    catch
    {
    }
    return new DateTime?();
  }
}
