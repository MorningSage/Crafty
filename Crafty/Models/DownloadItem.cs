using Downloader;
using System;

namespace Crafty.Models
{
	public class DownloadItem
	{
		public string Filename { get; set; }
		public string Url { get; set; }
		public DownloadService Downloader { get; set; }
		public int Progress;

		public DownloadItem(string filename, string url, DownloadService downloader)
		{
			Filename = filename;
			Url = url;
			Downloader = downloader;
			Downloader.DownloadProgressChanged += OnDownloadProgressChanged;
		}

		private void OnDownloadProgressChanged(object? sender, DownloadProgressChangedEventArgs e) => Progress = Convert.ToInt32(e.ProgressPercentage);
	}
}
