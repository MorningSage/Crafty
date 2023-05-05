using Avalonia.Controls;
using ModBrowser.Models;

namespace ModBrowser.Views
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private async void ModSelected(object? sender, SelectionChangedEventArgs e)
		{
			try
			{
				var mod = ModList.SelectedItem as Mod;
				ModList.SelectedItem = null;

				if (mod == null) { return; }

				var dialog = new ModWindow(mod);
				var result = await dialog.ShowDialog<string>(this);
			}

			catch { }
		}
	}
}