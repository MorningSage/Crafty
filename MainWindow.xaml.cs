using System;
using System.Threading.Tasks;
using System.Windows;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Version;

namespace Crafty;

public partial class MainWindow : Window
{
    public static MainWindow Current;

    public MainWindow()
    {
        Current = this;
        InitializeComponent();
        CraftyEssentials.GetVersions();
        VersionList.SelectedItem = CraftyEssentials.LatestVersion;
    }

    public async void PlayEvent(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(Username.Text) || !CraftyEssentials.CheckUsername(Username.Text))
        {
            MessageBox.Show($"Wrong username!");
            return;
        }

        var path = new MinecraftPath(CraftyEssentials.CraftyPath);
        var launcher = new CMLauncher(path);
        var launchOption = new MLaunchOption
        {
            MaximumRamMb = 2048,
            Session = MSession.GetOfflineSession(Username.Text),
            JavaPath = $"{CraftyEssentials.JavaPath}/bin/javaw.exe",
            JavaVersion = "19"
        };

        Username.IsEnabled = false;
        VersionList.IsEnabled = false;
        Play.IsEnabled = false;

        DownloadText.Text = "Downloading Java 19";
        await CraftyEssentials.DownloadJava();

        DownloadText.Text = $"Downloading {(string)VersionList.SelectedItem}.jar";
        await CraftyEssentials.DownloadVersion((string)VersionList.SelectedItem);

        DownloadText.Text = $"Downloading {(string)VersionList.SelectedItem}.jar";
        await CraftyEssentials.DownloadJson((string)VersionList.SelectedItem);

        DownloadText.Text = "Downloading assets...";
        await CraftyEssentials.DownloadAssets((string)VersionList.SelectedItem);

        DownloadText.Text = "Downloading libraries...";
        await CraftyEssentials.DownloadLibraries((string)VersionList.SelectedItem);

        DownloadText.Text = $"Downloading missing files...";
        var process = await launcher.CreateProcessAsync((string)VersionList.SelectedItem, launchOption, false);
        process.Start();
        DownloadText.Text = $"Launched Minecraft {VersionList.SelectedItem}";

        Username.IsEnabled = true;
        VersionList.IsEnabled = true;
        Play.IsEnabled = true;
    }

    public async void TestEvent(object sender, RoutedEventArgs e)
    {
        await CraftyEssentials.DownloadLibraries((string)VersionList.SelectedItem);
    }

    public async void ChangeTitle(string title)
    {
        Title = title;
    }

    public async void AddVersion(string version)
    {
        VersionList.Items.Add(version);
    }

    public async Task ChangeDownloadText(string s)
    {
        Dispatcher.Invoke(new Action(() => DownloadText.Text = s));
    }
}
