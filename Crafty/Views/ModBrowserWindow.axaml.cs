using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Crafty.Models;
using Crafty.ViewModels;

namespace Crafty.Views
{
	public partial class ModBrowserWindow : ReactiveUserControl<ModBrowserWindowViewModel>
	{
		public ModBrowserWindow(ModBrowserWindowViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
		}

		private void ModList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
		{
			Mod mod = ModList.SelectedItem as Mod;

			if (mod != null)
			{
				((ModBrowserWindowViewModel)DataContext).NavigateToMod(mod);
			}
		}
	}
}
