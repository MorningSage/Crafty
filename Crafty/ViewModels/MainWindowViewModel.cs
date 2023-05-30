using Avalonia.Collections;
using Avalonia.Media.Imaging;
using CmlLib.Core.Downloader;
using Crafty.Core;
using Crafty.Managers;
using ReactiveUI;
using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Version = Crafty.Models.Version;

namespace Crafty.ViewModels
{
	public class MainWindowViewModel : ReactiveObject, IScreen
	{
		public MainWindowViewModel()
		{
			try
			{
				if (File.Exists($"{Launcher.MinecraftPath}/crafty_session.json"))
				{
					var loginTask = Task.Run(Launcher.Login);
					loginTask.Wait();
					Username = Launcher.Session.Username;
				}
				else throw new Exception("Couldn't find session file");
			}
			catch
			{
				Username = ConfigManager.Config.Username;
			}

			try
			{
				Launcher.CmLauncher.FileChanged += e =>
				{
					switch (e.FileType)
					{
						case MFile.Library:
							ProgressBarText = $"Preparing libraries... {e.ProgressedFileCount}/{e.TotalFileCount} ({Math.Round((double)(100 * e.ProgressedFileCount) / e.TotalFileCount)}%)";
							break;
						case MFile.Minecraft:
							ProgressBarText = $"Preparing Minecraft... {e.ProgressedFileCount}/{e.TotalFileCount} ({Math.Round((double)(100 * e.ProgressedFileCount) / e.TotalFileCount)}%)";
							break;
						case MFile.Resource:
							ProgressBarText = $"Preparing resources... {e.ProgressedFileCount}/{e.TotalFileCount} ({Math.Round((double)(100 * e.ProgressedFileCount) / e.TotalFileCount)}%)";
							break;
						case MFile.Runtime:
							ProgressBarText = $"Preparing Java... {e.ProgressedFileCount}/{e.TotalFileCount} ({Math.Round((double)(100 * e.ProgressedFileCount) / e.TotalFileCount)}%)";
							break;
						case MFile.Others:
							ProgressBarText = $"Preparing other files... {e.ProgressedFileCount}/{e.TotalFileCount} ({Math.Round((double)(100 * e.ProgressedFileCount) / e.TotalFileCount)}%)";
							break;
					}

					ProgressBarMaximum = e.TotalFileCount;
					ProgressBarValue = e.ProgressedFileCount;
				};
			}
			catch { }

			try
			{
				SelectedItem = Launcher.VersionList.Where(x => x.Id == ConfigManager.Config.LastVersionUsed).First();
			}
			catch
			{
				SelectedItem = null;
			}

			NavigateAboutCommand = ReactiveCommand.Create(NavigateAbout);
			NavigateAccountCommand = ReactiveCommand.Create(NavigateAccount);
			NavigateModBrowserCommand = ReactiveCommand.Create(NavigateModBrowser);
			NavigateSettingsCommand = ReactiveCommand.Create(NavigateSettings);
			NavigateBackCommand = ReactiveCommand.Create(NavigateBack);
		}

		public string Title => $"Crafty ({Launcher.Version})";

		public Task<Bitmap> Cover => RandomManager.RandomCover();

		private string _progressBarText;

		public string ProgressBarText
		{
			get => _progressBarText;
			set => this.RaiseAndSetIfChanged(ref _progressBarText, value);
		}

		private double _progressBarMaximum = 100;

		public double ProgressBarMaximum
		{
			get => _progressBarMaximum;
			set => this.RaiseAndSetIfChanged(ref _progressBarMaximum, value);
		}

		private double _progressBarValue;

		public double ProgressBarValue
		{
			get => _progressBarValue;
			set => this.RaiseAndSetIfChanged(ref _progressBarValue, value);
		}

		private AvaloniaList<Version> _versionList;

		public AvaloniaList<Version> VersionList
		{
			get => Launcher.VersionList;
			set => this.RaiseAndSetIfChanged(ref _versionList, value);
		}

		private bool _isLoggedIn;

		public bool IsLoggedIn
		{
			get => Launcher.IsLoggedIn;
			set => this.RaiseAndSetIfChanged(ref _isLoggedIn, value);
		}

		private string _username;

		public string Username
		{
			get => _username;
			set => this.RaiseAndSetIfChanged(ref _username, value);
		}

		private Version? _selectedItem;

		public Version? SelectedItem
		{
			get => _selectedItem;
			set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
		}

		public RoutingState Router { get; } = new();

		public ReactiveCommand<Unit, Unit> NavigateAboutCommand { get; }
		public ReactiveCommand<Unit, Unit> NavigateAccountCommand { get; }
		public ReactiveCommand<Unit, Unit> NavigateModBrowserCommand { get; }
		public ReactiveCommand<Unit, Unit> NavigateSettingsCommand { get; }
		public ReactiveCommand<Unit, Unit> NavigateBackCommand { get; }

		private void NavigateAbout() => Router.Navigate.Execute(new AboutWindowViewModel(this));
		private void NavigateAccount() => Router.Navigate.Execute(new AccountWindowViewModel(this));
		private void NavigateModBrowser() => Router.Navigate.Execute(new ModBrowserWindowViewModel(this, Router));
		private void NavigateSettings() => Router.Navigate.Execute(new SettingsWindowViewModel(this));

		public void NavigateBack()
		{
			try
			{
				IRoutableViewModel? currentViewModel = Router.GetCurrentViewModel();

				if (currentViewModel.GetType() == typeof(AccountWindowViewModel) && Launcher.IsLoggedIn)
				{
					Username = Launcher.Session.Username;
				}
				else if (currentViewModel.GetType() == typeof(SettingsWindowViewModel))
				{
					ConfigManager.SaveConfig();
				}

				Router.NavigateBack.Execute();
			}
			catch { }
		}
	}
}