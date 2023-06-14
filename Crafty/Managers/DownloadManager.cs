using Crafty.Models;
using Downloader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;

namespace Crafty.Managers
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
				await item.Downloader.DownloadFileTaskAsync(await GetRedirectedUrl(item.Url), $"{DownloadPath}/{item.Filename}");
			}

			_isDownloading = false;
		}

		public static async Task<string> GetRedirectedUrl(string url)
		{
			// Fix for Downloader not downloading mods from CurseForge
			// https://github.com/bezzad/Downloader/issues/31

			var handler = new HttpClientHandler()
			{
				AllowAutoRedirect = false
			};

			string redirectedUrl = null;

			using (HttpClient client = new HttpClient(handler))
			using (HttpResponseMessage response = await client.GetAsync(url))
			{
				if (response.StatusCode == System.Net.HttpStatusCode.Found)
				{
					HttpResponseHeaders headers = response.Headers;
					if (headers != null && headers.Location != null)
					{
						redirectedUrl = headers.Location.AbsoluteUri;
					}
				}
			}

			return redirectedUrl;
		}
	}
}