using Avalonia.ReactiveUI;
using Crafty.ViewModels;

namespace Crafty.Views
{
	public partial class AboutWindow : ReactiveUserControl<AboutWindowViewModel>
	{
		public AboutWindow(AboutWindowViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
		}
	}
}