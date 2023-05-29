using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Crafty.Core;
using Crafty.ViewModels;

namespace Crafty.Views
{
	public partial class AccountWindow : ReactiveUserControl<AccountWindowViewModel>
	{
		public AccountWindow(AccountWindowViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
			Login();
		}

		private async void LogoutClicked(object? sender, RoutedEventArgs e)
		{
			LoggedInPanel.IsVisible = false;

			await Launcher.Logout();
		}

		private async void Login()
		{
			bool results = await Launcher.Login();

			if (!results) return;

			Username.Text = Launcher.Session.Username;
			LoggingIn.IsVisible = false;
			LoggedInPanel.IsVisible = true;
		}
	}
}
