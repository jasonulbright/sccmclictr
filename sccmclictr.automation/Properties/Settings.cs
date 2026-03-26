// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.Properties.Settings
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable disable
namespace sccmclictr.automation.Properties;

[CompilerGenerated]
[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.5.0.0")]
internal sealed class Settings : ApplicationSettingsBase
{
  private static Settings defaultInstance = (Settings) SettingsBase.Synchronized((SettingsBase) new Settings());

  public static Settings Default => Settings.defaultInstance;

  [ApplicationScopedSetting]
  [DebuggerNonUserCode]
  [DefaultSettingValue("$DCOM = (Invoke-CimMethod -Namespace 'root\\default' -ClassName StdRegProv -MethodName GetBinaryValue -Arguments @{hDefKey=[uint32]2147483650;sSubKeyName='{0}';sValueName='{1}'}).uValue\r\n$security = Get-CimInstance -Namespace root/cimv2 -ClassName __SystemSecurity\r\n$converter = Get-CimClass -Namespace root/cimv2 -ClassName Win32_SecurityDescriptorHelper\r\n(Invoke-CimMethod -InputObject $converter -MethodName BinarySDToSDDL -Arguments @{BinarySD=[byte[]]$DCOM}).SDDL\r\n")]
  public string PSGetDCOMPerm => (string) this[nameof (PSGetDCOMPerm)];

  [ApplicationScopedSetting]
  [DebuggerNonUserCode]
  [DefaultSettingValue("$newDCOMSDDL = \"{2}\"\r\n$DCOMbinarySD = (Invoke-CimMethod -Namespace 'root/cimv2' -ClassName Win32_SecurityDescriptorHelper -MethodName SDDLToBinarySD -Arguments @{SDDL=$newDCOMSDDL}).BinarySD\r\nInvoke-CimMethod -Namespace 'root\\default' -ClassName StdRegProv -MethodName SetBinaryValue -Arguments @{hDefKey=[uint32]2147483650;sSubKeyName='{0}';sValueName='{1}';uValue=[byte[]]$DCOMbinarySD}\r\n")]
  public string PSSetDCOMPerm => (string) this[nameof (PSSetDCOMPerm)];
}
