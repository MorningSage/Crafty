using Crafty.Core;
using ReactiveUI;
using System;

namespace Crafty.ViewModels
{
	public class SettingsWindowViewModel : ViewModelBase, IRoutableViewModel
	{
		public SettingsWindowViewModel(IScreen screen)
		{
			HostScreen = screen;
		}

		public IScreen HostScreen { get; }

		public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);

		public int PhysicalMemory
		{
			get => Launcher.PhysicalMemory;
			set => this.RaiseAndSetIfChanged(ref Launcher.PhysicalMemory, value);
		}
	}
}