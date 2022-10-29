using System;
using System.Threading.Tasks;
using System.Windows;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft.UI.Wpf;

namespace Crafty;

public partial class MainWindow : Window
{
    public static MainWindow Current;
    MicrosoftLoginWindow Window;

    public MainWindow()
    {
        Current = this;
        InitializeComponent();
        CraftyEssentials.GetVersions();
        VersionList.SelectedItem = CraftyEssentials.LatestVersion;

        Window = new MicrosoftLoginWindow();
    }

    public async void PlayEvent(object sender, RoutedEventArgs e)
    {
        if (!CraftyEssentials.CheckUsername(Username.Text))
        {
            MessageBox.Show($"Wrong username!");
            return;
        }

        var path = new MinecraftPath(CraftyEssentials.CraftyPath);
        var launcher = new CMLauncher(path);
        var launchOption = new MLaunchOption
        {
            MaximumRamMb = 2048,
            Session = CraftyEssentials.Session,
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

        DownloadText.Text = $"Downloading {(string)VersionList.SelectedItem}.json";
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

    public async void AddAccountEvent(object sender, RoutedEventArgs e)
    {
        MicrosoftLoginWindow LoginWindow = new MicrosoftLoginWindow();
        MSession LoginSession = await LoginWindow.ShowLoginDialog();
        CraftyEssentials.Session = LoginSession;
        Username.Text = LoginSession.Username;
    }

    public async void DeleteAccountEvent(object sender, RoutedEventArgs e)
    {
        MicrosoftLoginWindow LogoutWindow = new MicrosoftLoginWindow();
        LogoutWindow.ShowLogoutDialog();
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
