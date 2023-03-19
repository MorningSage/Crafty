using System.Linq;
using Avalonia.Interactivity;
using CmlLib.Core;
using Crafty.Core;
using Crafty.Managers;
using System.Threading.Tasks;
using CmlLib.Core.Auth;
using Version = Crafty.Models.Version;
using Avalonia.ReactiveUI;
using Crafty.ViewModels;
using ReactiveUI;

namespace Crafty.Views
{
	public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
	{
		public MainWindow()
		{
			System.Net.ServicePointManager.DefaultConnectionLimit = 512;

			InitializeComponent();

			this.WhenActivated(d => d(ViewModel!.ShowSettings.RegisterHandler(ShowSettingsAsync)));
			this.WhenActivated(d => d(ViewModel!.ShowAccount.RegisterHandler(ShowAccountAsync)));

			Cover.Source = RandomManager.RandomCover();
			VersionList.SelectedItem = Launcher.VersionList.Where(x => x.Id == ConfigManager.Config.LastVersionUsed).First();
		}

		private async void PlayClicked(object? sender, RoutedEventArgs e)
		{
			if (VersionList.SelectedItem == null) return;

			if (!Launcher.IsLoggedIn) Username.IsEnabled = false;
			VersionList.IsEnabled = false;
			PlayButton.IsEnabled = false;

			Version selectedVersion = (Version)VersionList.SelectedItem;
			await Launcher.CmLauncher.CheckAndDownloadAsync(await Launcher.CmLauncher.GetVersionAsync(selectedVersion.Id));
			VersionManager.UpdateVersion(selectedVersion);

			ProgressBar.ProgressTextFormat = "Done!";

			if (!Launcher.CheckUsername(Username.Text)) return;

			MLaunchOption launcherOptions = new() { MaximumRamMb = ConfigManager.Config.Ram };

			if (Launcher.IsLoggedIn) launcherOptions.Session = Launcher.Session;
			else launcherOptions.Session = MSession.GetOfflineSession(Username.Text);

			ConfigManager.Config.Username = Username.Text;
			ConfigManager.Config.LastVersionUsed = selectedVersion.Id;
			ConfigManager.SaveConfig();

			var minecraft = await Launcher.CmLauncher.CreateProcessAsync(selectedVersion.Id, launcherOptions, false);
			minecraft.Start();

			await Task.Delay(3000);

			if (!Launcher.IsLoggedIn) Username.IsEnabled = true;
			VersionList.IsEnabled = true;
			PlayButton.IsEnabled = true;
			ProgressBar.ProgressTextFormat = "";
			ProgressBar.Value = 0;
		}

		private async Task ShowSettingsAsync(InteractionContext<SettingsViewModel, MainWindowViewModel?> interaction)
		{
			var dialog = new SettingsWindow { DataContext = interaction.Input };
			var result = await dialog.ShowDialog<MainWindowViewModel?>(this);
			interaction.SetOutput(result);
		}

		private async Task ShowAccountAsync(InteractionContext<AccountViewModel, MainWindowViewModel?> interaction)
		{
			var dialog = new AccountWindow { DataContext = interaction.Input };
			var result = await dialog.ShowDialog<MainWindowViewModel?>(this);
			interaction.SetOutput(result);
		}
	}
}