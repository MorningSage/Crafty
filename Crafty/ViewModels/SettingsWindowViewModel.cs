using Crafty.Core;
using ReactiveUI;

namespace Crafty.ViewModels
{
	public class SettingsWindowViewModel : ViewModelBase
	{
		public int PhysicalMemory
		{
			get => Launcher.PhysicalMemory;
			set => this.RaiseAndSetIfChanged(ref Launcher.PhysicalMemory, value);
		}
	}
}