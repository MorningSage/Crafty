using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft.UI.Wpf;
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

    private async void PlayEvent(object sender, RoutedEventArgs e)
    {
        if (!CraftyEssentials.CheckUsername(Username.Text))
        {
            MessageBox.Show($"Wrong username!");
            return;
        }

        if (!CraftyEssentials.LoggedIn) { CraftyEssentials.Session = MSession.GetOfflineSession(Username.Text); }

        string Version = (string)VersionList.SelectedItem;
        var Path = new MinecraftPath(CraftyEssentials.CraftyPath);
        var Launcher = new CMLauncher(Path);
        var LauncherOptions = new MLaunchOption
        {
            MaximumRamMb = 2048,
            Session = CraftyEssentials.Session,
        };

        Username.IsEnabled = false;
        Login.IsEnabled = false;
        Logout.IsEnabled = false;
        VersionList.IsEnabled = false;
        Play.IsEnabled = false;

        // DownloadText.Text = "Downloading Java";
        // await CraftyEssentials.DownloadJava();
        // Java 19 crashes when launching old versions - using CmlLib's "Java downloader" for now

        DownloadText.Text = $"Downloading {Version}.jar";
        await CraftyEssentials.DownloadVersion(Version);

        DownloadText.Text = $"Downloading {Version}.json";
        await CraftyEssentials.DownloadJson(Version);

        DownloadText.Text = "Fetching assets";
        await CraftyEssentials.DownloadAssets(Version);

        DownloadText.Text = "Fetching libraries";
        await CraftyEssentials.DownloadLibraries(Version);

        DownloadText.Text = $"Downloading missing files (this might take a while)";
        var process = await Launcher.CreateProcessAsync(Version, LauncherOptions, true);
        process.Start();
        DownloadText.Text = $"Launched Minecraft {Version}";

        await Task.Delay(3000);
        DownloadText.Text = "Crafty by heapy & Badder1337";
        if (!CraftyEssentials.LoggedIn)
        {
            Username.IsEnabled = true;
            Login.IsEnabled = true;
            Logout.IsEnabled = false;
        }
        else { Logout.IsEnabled = true; }
        VersionList.IsEnabled = true;
        Play.IsEnabled = true;
    }

    private async void AddAccountEvent(object sender, RoutedEventArgs e)
    {
        MicrosoftLoginWindow LoginWindow = new MicrosoftLoginWindow();
        LoginWindow.Width = 500;
        LoginWindow.Height = 500;
        LoginWindow.Title = Title;
        try
        {
            MSession LoginSession = await LoginWindow.ShowLoginDialog();

            CraftyEssentials.Session = LoginSession;
            CraftyEssentials.LoggedIn = true;
            Username.Text = LoginSession.Username;
            Username.IsEnabled = false;
            Login.IsEnabled = false;
            Logout.IsEnabled = true;
        }

        catch (CmlLib.Core.Auth.Microsoft.LoginCancelledException)
        {
            return;
        }
    }

    private void DeleteAccountEvent(object sender, RoutedEventArgs e)
    {
        MicrosoftLoginWindow LogoutWindow = new MicrosoftLoginWindow();
        LogoutWindow.Width = 500;
        LogoutWindow.Height = 500;
        LogoutWindow.Title = Title;
        LogoutWindow.ShowLogoutDialog();

        CraftyEssentials.LoggedIn = false;
        Username.IsEnabled = true;
        Login.IsEnabled = true;
        Logout.IsEnabled = false;
    }

    private void OnExit(object sender, CancelEventArgs e)
    {
        Environment.Exit(0);
    }

    public void AddVersion(string version)
    {
        VersionList.Items.Add(version);
    }

    public async Task ChangeDownloadText(string s)
    {
        Dispatcher.Invoke(new Action(() => DownloadText.Text = s));
    }
}
