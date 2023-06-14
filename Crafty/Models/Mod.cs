using Avalonia.Media.Imaging;
using CurseForge.APIClient.Models.Mods;
using Downloader;
using Modrinth.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Crafty.Models
{
	public class Mod
	{
		public Mod(SearchResult mod, Modrinth.Models.Version[] projectVersionList, Project project)
		{
			Title = mod.Title;
			Body = project.Body;
			Author = mod.Author;
			IconUrl = mod.IconUrl;
			ProjectVersionList = ConvertToModVersion(projectVersionList);
			LatestProjectVersion = ProjectVersionList.First();
		}

		public Mod(CurseForge.APIClient.Models.Mods.Mod mod, string description, List<CurseForge.APIClient.Models.Files.File> modFiles)
		{
			Title = mod.Name;
			Body = description;
			Author = ReturnAuthors(mod.Authors);
			IconUrl = mod.Logo.Url;
			ProjectVersionList = ConvertToModVersion(modFiles);
			LatestProjectVersion = ProjectVersionList.First();
		}

		public string Title { get; }

		public string Body { get; }

		public string Author { get; }

		public string IconUrl { get; }

		public ModVersion[] ProjectVersionList { get; }

		public ModVersion LatestProjectVersion { get; }

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

		private string ReturnAuthors(List<ModAuthor> authorList)
		{
			List<string> authors = new();

			foreach (var author in authorList)
			{
				authors.Add(author.Name);
			}

			return String.Join(", ", authors);
		}

		private ModVersion[] ConvertToModVersion(Modrinth.Models.Version[] projectVersionList)
		{
			List<ModVersion> modVersionList = new();

			foreach (var modVersion in projectVersionList)
			{
				modVersionList.Add(new ModVersion(modVersion.Name, modVersion.VersionNumber, modVersion.Files.First().Url));
			}

			return modVersionList.ToArray();
		}

		private ModVersion[] ConvertToModVersion(List<CurseForge.APIClient.Models.Files.File> projectVersionList)
		{
			List<ModVersion> modVersionList = new();

			foreach (var modVersion in projectVersionList)
			{
				modVersionList.Add(new ModVersion(modVersion.DisplayName, modVersion.FileName, modVersion.DownloadUrl));
			}

			return modVersionList.ToArray();
		}
	}
}