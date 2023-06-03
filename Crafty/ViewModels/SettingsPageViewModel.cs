using ReactiveUI;
using System;

namespace Crafty.ViewModels
{
	public class SettingsPageViewModel : ViewModelBase, IRoutableViewModel
	{
		public SettingsPageViewModel(IScreen screen)
		{
			HostScreen = screen;
		}

		public IScreen HostScreen { get; }

		public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
	}
}