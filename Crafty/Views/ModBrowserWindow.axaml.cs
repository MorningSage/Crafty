using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Crafty.ViewModels;

namespace Crafty.Views
{
	public partial class ModBrowserWindow : ReactiveWindow<ModBrowserWindowViewModel>
	{
		public ModBrowserWindow()
		{
			InitializeComponent();
		}

		private void CloseClicked(object? sender, RoutedEventArgs e) => Close();
	}
}
