using System.Linq;
using Avalonia.Interactivity;
using CmlLib.Core;
using Crafty.Core;
using Crafty.Managers;
using System.Threading.Tasks;
using Avalonia.Media;
using CmlLib.Core.Auth;
using Version = Crafty.Models.Version;
using Avalonia.ReactiveUI;
using CmlLib.Core.Version;
using Crafty.ViewModels;
using ReactiveUI;

namespace Crafty.Views
{
	public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
	{
		public MainWindow()
		{
			InitializeComponent();

			this.WhenActivated(d => d(ViewModel!.ShowSettings.RegisterHandler(ShowSettingsAsync)));
			this.WhenActivated(d => d(ViewModel!.ShowAccount.RegisterHandler(ShowAccountAsync)));
			this.WhenActivated(d => d(ViewModel!.ShowAbout.RegisterHandler(ShowAboutAsync)));

			Cover.Source = RandomManager.RandomCover();
			try { VersionList.SelectedItem = Launcher.VersionList.Where(x => x.Id == ConfigManager.Config.LastVersionUsed).First(); }
			catch { VersionList.SelectedItem = null; }
		}

		private void CloseClicked(object? sender, RoutedEventArgs e) => ConfigManager.SaveConfig();

		private async void PlayClicked(object? sender, RoutedEventArgs e)
		{
			if (VersionList.SelectedItem == null) return;

			if (!Launcher.IsLoggedIn) Username.IsEnabled = false;
			VersionList.IsEnabled = false;
			PlayButton.IsEnabled = false;

			Version selectedVersion = (Version)VersionList.SelectedItem;
			try
			{
				try
				{
					await Launcher.CmLauncher.CheckAndDownloadAsync(await Launcher.CmLauncher.GetVersionAsync(selectedVersion.Id));
					VersionManager.UpdateVersion(selectedVersion);
				} catch { }

				ProgressBar.Maximum = 1;
				ProgressBar.Value = 1;
				ProgressBar.ProgressTextFormat = "Launching...";

				if (!Launcher.CheckUsername(Username.Text)) return;

				MVersion startVersion = VersionManager.MVersionList.GetVersion(selectedVersion.Id);
				MLaunchOption launchOptions = new() { Path = Launcher.CmLauncher.MinecraftPath, StartVersion = startVersion, ServerPort = 25565, ScreenWidth = 856, ScreenHeight = 482, MaximumRamMb = ConfigManager.Config.Ram };
				if (Launcher.IsLoggedIn) launchOptions.Session = Launcher.Session;
				else launchOptions.Session = MSession.GetOfflineSession(Username.Text);

				ConfigManager.Config.Username = Username.Text;
				ConfigManager.Config.LastVersionUsed = selectedVersion.Id;
				ConfigManager.SaveConfig();

				var minecraft = await Launcher.CmLauncher.CreateProcessAsync(launchOptions);
				minecraft.Start();

				ProgressBar.ProgressTextFormat = "Done!";
			}

			catch
			{
				ProgressBar.Background = Brush.Parse("#8a1a1a");
				ProgressBar.ProgressTextFormat = $"Couldn't launch {selectedVersion.Id}!";
			}

			await Task.Delay(3000);

			if (!Launcher.IsLoggedIn) Username.IsEnabled = true;
			VersionList.IsEnabled = true;
			PlayButton.IsEnabled = true;
			ProgressBar.Background = Brush.Parse("#141414");
			ProgressBar.ProgressTextFormat = "";
			ProgressBar.Value = 0;
		}

		private async Task ShowSettingsAsync(InteractionContext<SettingsWindowViewModel, MainWindowViewModel?> interaction)
		{
			var dialog = new SettingsWindow { DataContext = interaction.Input };
			var result = await dialog.ShowDialog<MainWindowViewModel?>(this);
			interaction.SetOutput(result);
		}

		private async Task ShowAccountAsync(InteractionContext<AccountWindowViewModel, MainWindowViewModel?> interaction)
		{
			var dialog = new AccountWindow { DataContext = interaction.Input };
			var result = await dialog.ShowDialog<MainWindowViewModel?>(this);
			interaction.SetOutput(result);

			if (Launcher.IsLoggedIn) { Username.Text = Launcher.Session.Username; }
		}

		private async Task ShowAboutAsync(InteractionContext<AboutWindowViewModel, MainWindowViewModel?> interaction)
		{
			var dialog = new AboutWindow { DataContext = interaction.Input };
			var result = await dialog.ShowDialog<MainWindowViewModel?>(this);
			interaction.SetOutput(result);
		}
	}
}