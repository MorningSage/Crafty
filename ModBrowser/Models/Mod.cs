using System.IO;
using System.Linq;
using Avalonia.Media.Imaging;
using Modrinth.Models;
using System.Threading.Tasks;
using Downloader;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;

namespace ModBrowser.Models
{
	public class Mod : SearchResult
	{
		public Mod(SearchResult searchResult)
		{
			Slug = searchResult.Slug;
			Title = searchResult.Title;
			Description = searchResult.Description;
			Categories = searchResult.Categories;
			ClientSide = searchResult.ClientSide;
			ServerSide = searchResult.ServerSide;
			ProjectType = searchResult.ProjectType;
			Downloads = searchResult.Downloads;
			IconUrl = searchResult.IconUrl;
			ProjectId = searchResult.ProjectId;
			Author = searchResult.Author;
			DisplayCategories = searchResult.DisplayCategories;
			DateCreated = searchResult.DateCreated;
			DateModified = searchResult.DateModified;
			Followers = searchResult.Followers;
			LatestVersion = searchResult.LatestVersion;
			License = searchResult.License;
			Versions = searchResult.Versions;
			Gallery = searchResult.Gallery;
			Color = searchResult.Color;

			SetProjectVersions();
		}

		public string[] ProjectVersions;

		private async void SetProjectVersions()
		{
			HttpResponseMessage response = await new HttpClient().GetAsync($"https://api.modrinth.com/v2/project/{ProjectId}");
			string data = await response.Content.ReadAsStringAsync();

			dynamic json = JsonConvert.DeserializeObject<dynamic>(data);
			ProjectVersions = ((JArray)json.versions).Select(x => (string)x).ToArray();
		}

		public Task<Bitmap> Icon => DownloadIcon(IconUrl);

		private async Task<Bitmap> DownloadIcon(string url)
		{
			var downloader = new DownloadService(new DownloadConfiguration { ParallelDownload = true});
			Stream stream = await downloader.DownloadFileTaskAsync(url);

			try
			{
				Bitmap bitmap = new Bitmap(stream);
				return bitmap;
			}

			catch { return null; }
		}
	}
}