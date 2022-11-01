using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft;
using CmlLib.Core.Auth.Microsoft.UI.Wpf;

namespace Crafty;

public partial class MainWindow : Window
{
    public static MainWindow Current;

    private List<Version> VersionList { get { return CraftyLauncher.VersionList; } }
    private List<Version> FabricVersionList { get { return CraftyLauncher.FabricVersionList; } }

    public MainWindow()
    {
        Current = this;
        InitializeComponent();
        CraftyEssentials.GetVersions();
        VersionBox.ItemsSource = VersionList;
        VersionBox.SelectedItem = VersionList.First();
    }

    private async void PlayEvent(object sender, RoutedEventArgs e)
    {
        if (!CraftyEssentials.CheckUsername(Username.Text))
        {
            MessageBox.Show($"Wrong username!");
            return;
        }
        
        if (!CraftyLauncher.LoggedIn) { CraftyLauncher.Session = MSession.GetOfflineSession(Username.Text); }

        Version Version = (Version)VersionBox.SelectedItem;

        Username.IsEnabled = false;
        LoginLogout.IsEnabled = false;
        VersionBox.IsEnabled = false;
        Play.IsEnabled = false;

        if (Version.isOriginal)
        {
            // DownloadText.Text = "Downloading Java";
            // await CraftyEssentials.DownloadJava();
            // Old versions crash on Java 19 - using Minecraft's default Java runtimes for now

            DownloadText.Text = $"Downloading {Version.id}.jar";
            await CraftyEssentials.DownloadVersion(Version.id);

            DownloadText.Text = $"Downloading {Version.id}.json";
            await CraftyEssentials.DownloadJson(Version.id);

            DownloadText.Text = "Fetching assets";
            await CraftyEssentials.DownloadAssets(Version.id);

            DownloadText.Text = "Fetching libraries";
            await CraftyEssentials.DownloadLibraries(Version.id);
        }

        DownloadText.Text = $"Downloading missing files (this might take a while)";
        var process = await CraftyLauncher.Launcher.CreateProcessAsync(Version.id, CraftyLauncher.LauncherOptions, true);
        process.Start();
        UpdateVersionBox();
        DownloadText.Text = $"Launched Minecraft {Version.id}";

        await Task.Delay(3000);
        DownloadText.Text = "Crafty by heapy & Badder1337";
        if (!CraftyLauncher.LoggedIn)
        {
            Username.IsEnabled = true;
        }
        LoginLogout.IsEnabled = true;
        VersionBox.IsEnabled = true;
        Play.IsEnabled = true;
    }

    private async void LoginLogoutEvent(object sender, RoutedEventArgs e)
    {
        if (!CraftyLauncher.LoggedIn)
        {
            MicrosoftLoginWindow LoginWindow = new MicrosoftLoginWindow();
            LoginWindow.Width = 500;
            LoginWindow.Height = 500;
            LoginWindow.Title = Title;

            try
            {
                MSession LoginSession = await LoginWindow.ShowLoginDialog();

                CraftyLauncher.Session = LoginSession;
                CraftyLauncher.LoggedIn = true;
                Username.IsEnabled = false;
                Username.Text = LoginSession.Username;
                LoginLogout.Content = "Logout";
            }

            catch (LoginCancelledException)
            {
                return;
            }
        }

        else
        {
            // LoginHandler CraftyLogin = new LoginHandler();
            // Saved for later ;)

            MicrosoftLoginWindow LogoutWindow = new MicrosoftLoginWindow();
            LogoutWindow.Width = 500;
            LogoutWindow.Height = 500;
            LogoutWindow.Title = Title;
            LogoutWindow.ShowLogoutDialog();

            CraftyLauncher.LoggedIn = false;
            Username.IsEnabled = true;
            LoginLogout.Content = "Login";
        }
    }

    private void OnExit(object sender, CancelEventArgs e) { Environment.Exit(0); }

    private void UpdateVersionBox()
    {
        int SelectedIndex = VersionBox.SelectedIndex;
        VersionBox.SelectedIndex = -1;
        VersionBox.SelectedIndex = SelectedIndex;
        VersionBox.Items.Refresh();
    }

    public async Task ChangeDownloadText(string s) { Dispatcher.Invoke(new Action(() => DownloadText.Text = s)); }
}
