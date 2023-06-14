using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Crafty.Models;
using Crafty.ViewModels;

namespace Crafty.Views
{
	public partial class ModBrowserPage : ReactiveUserControl<ModBrowserPageViewModel>
	{
		public ModBrowserPage(ModBrowserPageViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
		}

		private void ModList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
		{
			Mod mod = ModList.SelectedItem as Mod;

			if (mod != null)
			{
				((ModBrowserPageViewModel)DataContext).NavigateToMod(mod);
			}
		}

		private void UseCurseForge_OnChecked(object? sender, RoutedEventArgs e)
		{
			((ModBrowserPageViewModel)DataContext).UseCurseForge = true;
		}

		private void UseCurseForge_OnUnchecked(object? sender, RoutedEventArgs e)
		{
			((ModBrowserPageViewModel)DataContext).UseCurseForge = false;
		}
	}
}
