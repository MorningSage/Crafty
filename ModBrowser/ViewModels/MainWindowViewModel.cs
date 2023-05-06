using ModBrowser.Managers;
using Modrinth.Models;
using ReactiveUI;
using System.Reactive;
using Modrinth;
using Avalonia.Collections;
using ModBrowser.Models;
using Modrinth.Models.Enums.Project;

namespace ModBrowser.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		public MainWindowViewModel()
		{
			SearchModsCommand = ReactiveCommand.Create<string>(SearchMods);
		}

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

		public ReactiveCommand<string, Unit> SearchModsCommand { get; }

		public async void SearchMods(string query)
		{
			Searching = true;
			SearchButtonText = "Searching...";

			Results.Clear();
			var search = await ModrinthApi.Client.Project.SearchAsync(query, facets: new() { Facet.ProjectType(ProjectType.Mod) });

			foreach (SearchResult searchResult in search.Hits)
			{
				var projectVersionList = await ModrinthApi.Client.Version.GetProjectVersionListAsync(searchResult.ProjectId);
				Results.Add(new Mod(searchResult, projectVersionList));
			}

			Searching = false;
			SearchButtonText = "Search";
		}
	}
}