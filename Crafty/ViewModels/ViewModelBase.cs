using Crafty.Managers;
using Crafty.Models;
using ReactiveUI;

namespace Crafty.ViewModels
{
	public class ViewModelBase : ReactiveObject
	{
		public Config Config => ConfigManager.Config;
	}
}