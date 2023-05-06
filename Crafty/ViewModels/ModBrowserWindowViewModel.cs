using System.Diagnostics;
using Modrinth.Models;
using ReactiveUI;
using System.Reactive;
using Modrinth;
using Avalonia.Collections;
using Crafty.Managers;
using Crafty.Models;
using Modrinth.Models.Enums.Project;

namespace Crafty.ViewModels
{
    public class ModBrowserWindowViewModel : ReactiveObject, IScreen
	{
		public ModBrowserWindowViewModel()
        {
	        SearchModsCommand = ReactiveCommand.Create<string>(SearchMods);
            NavigateBackCommand = ReactiveCommand.Create(NavigateBack);
		}

		public RoutingState Router { get; } = new();

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

        private async void SearchMods(string query)
        {
            Searching = true;
            SearchButtonText = "Searching...";

            Results.Clear();
            var search = await ModrinthManager.Client.Project.SearchAsync(query, facets: new() { Facet.ProjectType(ProjectType.Mod) });

            foreach (SearchResult searchResult in search.Hits)
            {
	            var projectVersionList = await ModrinthManager.Client.Version.GetProjectVersionListAsync(searchResult.ProjectId);
                Results.Add(new Mod(searchResult, projectVersionList));
            }

            Searching = false;
            SearchButtonText = "Search";
        }

        private Mod _selectedMod;

        public Mod SelectedMod
        {
	        get => _selectedMod;
	        set
	        {
		        if (Searching) return;

		        if (value == null) value = SelectedMod;

	            this.RaiseAndSetIfChanged(ref _selectedMod, value);
                Router.Navigate.Execute(new ModWindowViewModel(value, this));
	        }
        }

        public ReactiveCommand<Unit, Unit> NavigateBackCommand { get; }

        private void NavigateBack()
        {
	        try
	        {
		        Router.NavigateBack.Execute();
	        }
	        catch { }
        } 
	}
}