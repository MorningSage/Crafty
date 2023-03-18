using Crafty.Core;
using ReactiveUI;

namespace Crafty.ViewModels
{
	public class SettingsViewModel : ViewModelBase
	{
		public int PhysicalMemory
		{
			get => Launcher.PhysicalMemory;
			set => this.RaiseAndSetIfChanged(ref Launcher.PhysicalMemory, value);
		}
	}
}