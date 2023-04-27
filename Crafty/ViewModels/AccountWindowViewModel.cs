﻿using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Crafty.Core;
using Downloader;

namespace Crafty.ViewModels
{
	public class AccountWindowViewModel : ViewModelBase
	{
		public static Task<Bitmap?> Skin => DownloadSkin();

		private static async Task<Bitmap?> DownloadSkin()
		{
			if (Launcher.Skin != null)
			{
				return Launcher.Skin;
			}

			var downloader = new DownloadService(new DownloadConfiguration { ParallelDownload = true });
			Stream stream = await downloader.DownloadFileTaskAsync($"https://crafatar.com/renders/body/{Launcher.Session.UUID}");

			try
			{
				Bitmap bitmap = new Bitmap(stream);
				Launcher.Skin = bitmap;
				return bitmap;
			}

			catch { return null; }
		}
	}
}
