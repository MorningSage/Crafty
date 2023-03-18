using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Collections;
using CmlLib.Core.Downloader;
using Crafty.Core;
using ReactiveUI;
using Version = Crafty.Models.Version;
using System.Windows.Input;
using Crafty.Managers;


namespace Crafty.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		public MainWindowViewModel()
		{
			if (File.Exists($"{Launcher.MinecraftPath}/crafty_session.json"))
			{
				var loginTask = Task.Run(Launcher.Login);
				loginTask.Wait();
				Username = Launcher.Session.Username;
			}

			else Username = ConfigManager.Config.Username;

			ShowSettings = new Interaction<SettingsViewModel, MainWindowViewModel?>();

			OpenSettingsCommand = ReactiveCommand.CreateFromTask(async () =>
			{
				IsDialogVisible = true;
				var settings = new SettingsViewModel();
				var result = await ShowSettings.Handle(settings);
				VersionList = Launcher.VersionList;
				IsDialogVisible = false;
			});

			ShowAccount = new Interaction<AccountViewModel, MainWindowViewModel?>();

			OpenAccountCommand = ReactiveCommand.CreateFromTask(async () =>
			{
				IsDialogVisible = true;
				var settings = new AccountViewModel();
				var result = await ShowAccount.Handle(settings);
				IsDialogVisible = false;
				IsLoggedIn = Launcher.IsLoggedIn;
			});

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

		private bool _isDialogVisible;

		public bool IsDialogVisible
		{
			get => _isDialogVisible;
			set => this.RaiseAndSetIfChanged(ref _isDialogVisible, value);
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

		public Interaction<SettingsViewModel, MainWindowViewModel?> ShowSettings { get; }
		public ICommand OpenSettingsCommand { get; }

		public Interaction<AccountViewModel, MainWindowViewModel?> ShowAccount { get; }
		public ICommand OpenAccountCommand { get; }
	}
}