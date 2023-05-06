using System;
using System.Reactive;
using Crafty.Managers;
using Crafty.Models;
using ReactiveUI;
using Version = Modrinth.Models.Version;

namespace Crafty.ViewModels
{
    public class ModWindowViewModel : ReactiveObject, IRoutableViewModel
	{
        public ModWindowViewModel(Mod mod, IScreen screen)
        {
            Mod = mod;
            HostScreen = screen;
			DownloadModCommand = ReactiveCommand.Create<Version>(DownloadMod);
        }

        public IScreen HostScreen { get; }

        public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);

        public Mod Mod { get; }

        public ReactiveCommand<Version, Unit> DownloadModCommand { get; }

        private void DownloadMod(object selectedItem)
        {
	        try
	        {
	            Version? version = (Version?)selectedItem;
	            if (version == null) return;
	            foreach (var file in version.Files) DownloadManager.Download(file.Url);
	        }

            catch { }
        }
    }
}