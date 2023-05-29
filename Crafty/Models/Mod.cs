using Avalonia.Media.Imaging;
using Downloader;
using Modrinth.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Crafty.Models
{
	public class Mod : SearchResult
	{
		public Mod(SearchResult searchResult, Modrinth.Models.Version[] projectVersionList, Project project)
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
			ProjectVersionList = projectVersionList;
			LatestProjectVersion = projectVersionList.First();
			Body = project.Body;
		}

		public Modrinth.Models.Version[] ProjectVersionList { get; }

		public Modrinth.Models.Version LatestProjectVersion { get; }

		public string Body { get; }

		public Task<Bitmap?> Icon => DownloadIcon(IconUrl);

		private async Task<Bitmap?> DownloadIcon(string url)
		{
			var downloader = new DownloadService(new DownloadConfiguration { ParallelDownload = true });
			Stream stream = await downloader.DownloadFileTaskAsync(url);

			try
			{
				Bitmap bitmap = new Bitmap(stream);
				return bitmap;
			}
			catch
			{
				return null;
			}
		}
	}
}