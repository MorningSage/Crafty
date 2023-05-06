using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Crafty.ViewModels;
using ReactiveUI;

namespace Crafty.Views
{
	public partial class ModBrowserWindow : ReactiveWindow<ModBrowserWindowViewModel>
	{
		public ModBrowserWindow()
		{
			this.WhenActivated(disposables => { });
			InitializeComponent();
		}

		private void CloseClicked(object? sender, RoutedEventArgs e) => Close();
	}
}
