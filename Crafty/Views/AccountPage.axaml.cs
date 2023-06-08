using Avalonia.ReactiveUI;
using Crafty.Core;
using Crafty.ViewModels;

namespace Crafty.Views
{
	public partial class AccountPage : ReactiveUserControl<AccountPageViewModel>
	{
		public AccountPage(AccountPageViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
			Login();
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
