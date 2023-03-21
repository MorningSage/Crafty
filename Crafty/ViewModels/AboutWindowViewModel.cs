using Crafty.Core;

namespace Crafty.ViewModels
{
	public class AboutWindowViewModel : ViewModelBase
	{
		public string Version => Launcher.Version;
	}
}
