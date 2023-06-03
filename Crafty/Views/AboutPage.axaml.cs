using Avalonia.ReactiveUI;
using Crafty.ViewModels;

namespace Crafty.Views
{
	public partial class AboutPage : ReactiveUserControl<AboutPageViewModel>
	{
		public AboutPage(AboutPageViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
		}
	}
}