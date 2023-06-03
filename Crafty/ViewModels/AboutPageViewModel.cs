using Crafty.Core;
using ReactiveUI;
using System;

namespace Crafty.ViewModels
{
	public class AboutPageViewModel : ViewModelBase, IRoutableViewModel
	{
		public AboutPageViewModel(IScreen screen)
		{
			HostScreen = screen;
		}

		public IScreen HostScreen { get; }

		public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);

		public string Version => Launcher.Version;
	}
}
