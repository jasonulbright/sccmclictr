// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.Properties.Resources
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace sccmclictr.automation.Properties;

/// <summary>
///   A strongly-typed resource class, for looking up localized strings, etc.
/// </summary>
[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Resources
{
  private static ResourceManager resourceMan;
  private static CultureInfo resourceCulture;

  internal Resources()
  {
  }

  /// <summary>
  ///   Returns the cached ResourceManager instance used by this class.
  /// </summary>
  [EditorBrowsable(EditorBrowsableState.Advanced)]
  internal static ResourceManager ResourceManager
  {
    get
    {
      if (sccmclictr.automation.Properties.Resources.resourceMan == null)
        sccmclictr.automation.Properties.Resources.resourceMan = new ResourceManager("sccmclictr.automation.Properties.Resources", typeof (sccmclictr.automation.Properties.Resources).Assembly);
      return sccmclictr.automation.Properties.Resources.resourceMan;
    }
  }

  /// <summary>
  ///   Overrides the current thread's CurrentUICulture property for all
  ///   resource lookups using this strongly typed resource class.
  /// </summary>
  [EditorBrowsable(EditorBrowsableState.Advanced)]
  internal static CultureInfo Culture
  {
    get => sccmclictr.automation.Properties.Resources.resourceCulture;
    set => sccmclictr.automation.Properties.Resources.resourceCulture = value;
  }

  /// <summary>
  ///    Looks up a localized string similar to $CacheElements =  get-wmiobject -query "SELECT * FROM CacheInfoEx" -namespace "ROOT\ccm\SoftMgmtAgent"
  /// $ElementGroup = $CacheElements | Group-Object ContentID
  /// [int]$Cleaned = 0;
  /// 
  /// #Cleanup CacheItems where ContentFolder does not exist
  /// $CacheElements | where {!(Test-Path $_.Location)} | % { $_.Delete(); $Cleaned++ }
  /// $CacheElements = get-wmiobject -query "SELECT * FROM CacheInfoEx" -namespace "ROOT\ccm\SoftMgmtAgent"
  /// 
  /// foreach ($ElementID in $ElementGroup)
  /// {
  ///     if ($ElementID.Count -gt 1)
  ///     {
  ///   [rest of string was truncated]";.
  ///  </summary>
  internal static string CacheCleanup
  {
    get => sccmclictr.automation.Properties.Resources.ResourceManager.GetString(nameof (CacheCleanup), sccmclictr.automation.Properties.Resources.resourceCulture);
  }

  /// <summary>
  ///    Looks up a localized string similar to $MissingUpdates = $false;
  /// $FileRenamePending = $false;
  /// $CM12RebootPending = $false;
  /// $CM12HardRebootPending = $false;
  /// $PatchRebootPending = $false;
  /// $RebootRequired = $false;
  /// $RunningUpdates = $false;
  /// $UserLoggedOn = $false;
  /// $CCMUpdate = get-wmiobject -query "SELECT * FROM CCM_SoftwareUpdate" -namespace "ROOT\ccm\ClientSDK";
  /// $CCMAppl = get-wmiobject -query "SELECT * FROM CCM_Application" -namespace "ROOT\ccm\ClientSDK";
  /// $CCMProg = get-wmiobject -query "SELECT * FROM CCM_Program" -namespace "ROOT\ccm [rest of string was truncated]";.
  ///  </summary>
  internal static string HealthCheck
  {
    get => sccmclictr.automation.Properties.Resources.ResourceManager.GetString(nameof (HealthCheck), sccmclictr.automation.Properties.Resources.resourceCulture);
  }

  /// <summary>
  ///    Looks up a localized string similar to $script =
  /// {
  /// function Enable-TSDuplicateToken {
  /// 
  /// [CmdletBinding()]
  /// param()
  /// 
  /// $signature = @"
  ///     [StructLayout(LayoutKind.Sequential, Pack = 1)]
  ///      public struct TokPriv1Luid
  ///      {
  ///          public int Count;
  ///          public long Luid;
  ///          public int Attr;
  ///      }
  /// 
  ///     public const int SE_PRIVILEGE_ENABLED = 0x00000002;
  ///     public const int TOKEN_QUERY = 0x00000008;
  ///     public const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
  ///     public const UInt32 STANDARD_RIGHTS_REQUIRED = 0x000F000 [rest of string was truncated]";.
  ///  </summary>
  internal static string SecretDecode
  {
    get => sccmclictr.automation.Properties.Resources.ResourceManager.GetString(nameof (SecretDecode), sccmclictr.automation.Properties.Resources.resourceCulture);
  }
}
