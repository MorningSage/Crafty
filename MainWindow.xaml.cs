using System.Windows;

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

        Username.IsEnabled = false;
        VersionList.IsEnabled = false;
        Play.IsEnabled = false;

        DownloadText.Text = "Downloading Java...";
        DownloadBar.Value = 33;
        await CraftyEssentials.DownloadJava();

        DownloadText.Text = "Downloading Jar...";
        DownloadBar.Value = 66;
        await CraftyEssentials.DownloadVersion((string)VersionList.SelectedItem);

        DownloadText.Text = "Downloading Json...";
        DownloadBar.Value = 100;
        await CraftyEssentials.DownloadJson((string)VersionList.SelectedItem);

        DownloadText.Text = $"Launching Minecraft {VersionList.SelectedItem}...";
        DownloadBar.Value = 0;
        Username.IsEnabled = true;
        VersionList.IsEnabled = true;
        Play.IsEnabled = true;
    }

    public async void ChangeTitle(string title)
    {
        Title = title;
    }

    public async void AddVersion(string version)
    {
        VersionList.Items.Add(version);
    }
}
