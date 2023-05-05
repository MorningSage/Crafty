using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ModBrowser.Managers;
using ModBrowser.Models;

namespace ModBrowser.Views
{
	public partial class ModWindow : Window
	{
		public Mod Mod;

		public ModWindow(Mod mod)
		{
			InitializeComponent();
			Mod = mod;
			DataContext = Mod;
		}

		private async void DownloadClicked(object? sender, RoutedEventArgs e)
		{
			string url = await ModrinthApi.GetProjectDownloadUrl(Mod.ProjectVersions.Last());
			DownloadManager.Download(url);
		}
	}
}