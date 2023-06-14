using Crafty.Managers;
using Crafty.Models;
using ReactiveUI;
using System;
using System.Reactive;

namespace Crafty.ViewModels
{
	public class ModOverviewPageViewModel : ViewModelBase, IRoutableViewModel
	{
		public ModOverviewPageViewModel(IScreen screen, Mod mod)
		{
			HostScreen = screen;
			Mod = mod;
			DownloadModCommand = ReactiveCommand.Create<ModVersion>(DownloadMod);
		}

		public IScreen HostScreen { get; }

		public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);

		public Mod Mod { get; }

		public ReactiveCommand<ModVersion, Unit> DownloadModCommand { get; }

		private void DownloadMod(object selectedItem)
		{
			try
			{
				ModVersion? version = (ModVersion?)selectedItem;
				if (version == null) return;
				DownloadManager.Download(version.DownloadUrl);
			}
			catch { }
		}
	}
}