using Avalonia.ReactiveUI;
using Crafty.ViewModels;

namespace Crafty.Views
{
	public partial class ModOverviewPage : ReactiveUserControl<ModOverviewPageViewModel>
	{
		public ModOverviewPage(ModOverviewPageViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
		}
	}
}