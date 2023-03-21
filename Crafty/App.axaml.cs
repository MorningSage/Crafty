using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Crafty.Managers;
using Crafty.ViewModels;
using Crafty.Views;

namespace Crafty
{
	public partial class App : Application
	{
		public override void Initialize()
		{
			System.Net.ServicePointManager.DefaultConnectionLimit = 512;
			AvaloniaXamlLoader.Load(this);
			ConfigManager.LoadConfig();
		}

		public override void OnFrameworkInitializationCompleted()
		{
			if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
			{
				desktop.MainWindow = new MainWindow
				{
					DataContext = new MainWindowViewModel(),
				};
			}

			base.OnFrameworkInitializationCompleted();
		}
	}
}