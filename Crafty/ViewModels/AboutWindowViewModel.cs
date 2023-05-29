using Crafty.Core;
using ReactiveUI;
using System;

namespace Crafty.ViewModels
{
	public class AboutWindowViewModel : ViewModelBase, IRoutableViewModel
	{
		public AboutWindowViewModel(IScreen screen)
		{
			HostScreen = screen;
		}

		public IScreen HostScreen { get; }

		public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);

		public string Version => Launcher.Version;
	}
}
