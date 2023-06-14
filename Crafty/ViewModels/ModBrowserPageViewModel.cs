using Avalonia.Collections;
using Crafty.Managers;
using Crafty.Models;
using Modrinth;
using Modrinth.Models;
using Modrinth.Models.Enums.Project;
using ReactiveUI;
using System;
using System.Reactive;

namespace Crafty.ViewModels
{
	public class ModBrowserPageViewModel : ViewModelBase, IRoutableViewModel
	{
		public ModBrowserPageViewModel(IScreen screen, RoutingState router)
		{
			HostScreen = screen;
			Router = router;
			SearchModsCommand = ReactiveCommand.Create<string>(SearchMods);
		}

		public IScreen HostScreen { get; }

		public RoutingState Router { get; }

		public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);

		private AvaloniaList<Mod> _results = new();

		public AvaloniaList<Mod> Results
		{
			get => _results;
			set => this.RaiseAndSetIfChanged(ref _results, value);
		}

		private bool _searching;

		public bool Searching
		{
			get => _searching;
			set => this.RaiseAndSetIfChanged(ref _searching, value);
		}

		private string _searchButtonText = "Search";

		public string SearchButtonText
		{
			get => _searchButtonText;
			set => this.RaiseAndSetIfChanged(ref _searchButtonText, value);
		}

		private bool _useCurseForge;

		public bool UseCurseForge
		{
			get => _useCurseForge;
			set => this.RaiseAndSetIfChanged(ref _useCurseForge, value);
		}

		public ReactiveCommand<string, Unit> SearchModsCommand { get; }

		private async void SearchMods(string query)
		{
			Searching = true;
			SearchButtonText = "Searching...";

			if (UseCurseForge)
			{
				try
				{
					Results.Clear();
					var search = await CurseForgeManager.Client.SearchModsAsync(gameId: 432, classId: 6, searchFilter: query);

					foreach (CurseForge.APIClient.Models.Mods.Mod mod in search.Data)
					{
						var description = (await CurseForgeManager.Client.GetModDescriptionAsync(mod.Id)).Data;
						var modFiles = await CurseForgeManager.Client.GetModFilesAsync(mod.Id);
						Results.Add(new Mod(mod, description, modFiles.Data));
					}
				}
				catch
				{
					Results.Clear();
				}
			}
			else
			{
				try
				{
					Results.Clear();
					var search = await ModrinthManager.Client.Project.SearchAsync(query, facets: new() { Facet.ProjectType(ProjectType.Mod) });

					foreach (SearchResult mod in search.Hits)
					{
						var project = await ModrinthManager.Client.Project.GetAsync(mod.ProjectId);
						var projectVersionList = await ModrinthManager.Client.Version.GetProjectVersionListAsync(mod.ProjectId);
						project.Body = project.Body.Replace("<br>", "\n").Replace("**", "");
						Results.Add(new Mod(mod, projectVersionList, project));
					}
				}
				catch
				{
					Results.Clear();
				}
			}

			Searching = false;
			SearchButtonText = "Search";
		}

		public void NavigateToMod(Mod mod)
		{
			Router.Navigate.Execute(new ModOverviewPageViewModel(HostScreen, mod));
		}
	}
}