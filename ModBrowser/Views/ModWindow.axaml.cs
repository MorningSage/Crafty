using System.Diagnostics;
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

		private void DownloadClicked(object? sender, RoutedEventArgs e)
		{
			foreach (var x in Mod.ProjectVersionList.First().Files)
			{
				Debug.WriteLine(x.Url);
				DownloadManager.Download(x.Url);
			}
		}
	}
}