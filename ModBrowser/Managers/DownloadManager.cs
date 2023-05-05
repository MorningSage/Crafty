using System;
using System.Collections.Generic;
using System.Linq;
using Downloader;
using ModBrowser.Models;

namespace ModBrowser.Managers
{
	public static class DownloadManager
	{
		private static readonly string DownloadPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/.crafty/mods";
		private static Queue<DownloadItem> _queue = new();
		private static bool _isDownloading;

		public static void Download(string url)
		{
			_queue.Enqueue(new DownloadItem(System.Net.WebUtility.UrlDecode(url.Split("/").Last()), url, new DownloadService(new DownloadConfiguration { ParallelDownload = true })));

			if (!_isDownloading) Start();
		}

		public static async void Start()
		{
			_isDownloading = true;

			while (_queue.Count > 0)
			{
				DownloadItem item = _queue.Dequeue();
				await item.Downloader.DownloadFileTaskAsync(item.Url, $"{DownloadPath}/{item.Filename}");
			}

			_isDownloading = false;
		}
	}
}