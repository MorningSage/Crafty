using Avalonia.Media.Imaging;
using Crafty.Core;
using Downloader;
using ReactiveUI;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Crafty.ViewModels
{
	public class AccountWindowViewModel : ViewModelBase, IRoutableViewModel
	{
		public AccountWindowViewModel(IScreen screen)
		{
			HostScreen = screen;
		}

		public IScreen HostScreen { get; }

		public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);

		public Task<Bitmap?> Skin => DownloadSkin();

		private async Task<Bitmap?> DownloadSkin()
		{
			while (!Launcher.IsLoggedIn) await Task.Delay(1000);

			if (Launcher.Skin != null) return Launcher.Skin;

			var downloader = new DownloadService(new DownloadConfiguration { ParallelDownload = true });
			Stream stream = await downloader.DownloadFileTaskAsync($"https://crafatar.com/renders/body/{Launcher.Session.UUID}");

			try
			{
				Bitmap bitmap = new Bitmap(stream);
				Launcher.Skin = bitmap;
				return bitmap;
			}
			catch
			{
				return null;
			}
		}
	}
}
