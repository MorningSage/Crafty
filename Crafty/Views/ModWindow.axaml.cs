using Avalonia.ReactiveUI;
using Crafty.ViewModels;

namespace Crafty.Views
{
    public partial class ModWindow : ReactiveUserControl<ModWindowViewModel>
	{
        public ModWindow(ModWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}