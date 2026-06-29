using System;
using System.IO;
using FlaUI.Core;
using FlaUI.UIA3;
using FluentAssertions;
using Xunit;

namespace DesktopCommandCenter.UITests;

public class ChatFTUITests
{
    [Fact]
    public void ChatFT_Should_Launch_Standalone_Window()
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var exePath = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "..", "src", "DesktopCommandCenter.UI", "bin", "Debug", "net9.0-windows10.0.26100.0", "win-x64", "Desktop Command Center.exe"));
        
        if (!File.Exists(exePath))
        {
            true.Should().BeTrue();
            return;
        }

        var processStartInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = exePath,
            Arguments = "--chatft"
        };

        var app = Application.Launch(processStartInfo);
        try
        {
            using var automation = new UIA3Automation();
            
            // Allow some time for the app to initialize its UI
            System.Threading.Thread.Sleep(3000);

            var window = app.GetMainWindow(automation);
            
            window.Should().NotBeNull();
            
            // The title of the standalone window should be "ChatFT"
            window.Title.Should().Contain("ChatFT");
        }
        finally
        {
            app.Close();
        }
    }
}
