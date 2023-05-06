using Avalonia.Controls;
using ModBrowser.ViewModels;

namespace ModBrowser.Views
{
	public partial class ModWindow : Window
	{
		public ModWindow(ModWindowViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
		}
	}
}