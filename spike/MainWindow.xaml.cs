using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using Wpf.Ui.Controls;

namespace SccmCliCtr.V2Spike;

public partial class MainWindow : FluentWindow
{
    public MainWindow()
    {
        InitializeComponent();

        RuntimeText.Text = $".NET {Environment.Version} ({System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription})";
        ProcessText.Text = $"PID {Process.GetCurrentProcess().Id} — {Process.GetCurrentProcess().ProcessName}";
        WpfUiText.Text = typeof(FluentWindow).Assembly.GetName().Version?.ToString() ?? "unknown";
        OsText.Text = $"{Environment.OSVersion.VersionString} ({(Environment.Is64BitOperatingSystem ? "x64" : "x86")})";

        StatusText.Text = "Stack OK. PS runspace integration deferred to U1.";
    }

    private void OnProbeClick(object sender, RoutedEventArgs e)
    {
        StatusText.Text = $"Click handled at {DateTime.Now:HH:mm:ss}. Event routing works.";
    }
}
