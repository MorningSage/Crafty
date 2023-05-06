using System.Reactive;
using ModBrowser.Managers;
using ModBrowser.Models;
using Modrinth.Models;
using ReactiveUI;

namespace ModBrowser.ViewModels
{
	public class ModWindowViewModel : ReactiveObject
	{
		public ModWindowViewModel(Mod mod)
		{
			Mod = mod;
			DownloadModCommand = ReactiveCommand.Create<Version>(DownloadMod);
		}

		public Mod Mod { get; }

		public ReactiveCommand<Version, Unit> DownloadModCommand { get; }

		private void DownloadMod(object selectedItem)
		{
			Version version = (Version)selectedItem;
			foreach (var x in version.Files) DownloadManager.Download(x.Url);
		}
	}
}