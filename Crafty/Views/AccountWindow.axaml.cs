using Avalonia.Controls;
using Avalonia.Interactivity;
using Crafty.Core;

namespace Crafty.Views
{
	public partial class AccountWindow : Window
	{
		public AccountWindow()
		{
			InitializeComponent();
			Login();
		}

		private void CloseClicked(object? sender, RoutedEventArgs e) => Close();

		private async void LogoutClicked(object? sender, RoutedEventArgs e)
		{
			LoggedInPanel.IsVisible = false;
			Close.IsEnabled = false;

			await Launcher.Logout();
			Close();
		}

		private async void Login()
		{
			await Launcher.Login();

			Username.Text = Launcher.Session.Username;
			Skin.Source = Launcher.Skin;

			LoggingIn.IsVisible = false;
			LoggedInPanel.IsVisible = true;
			Close.IsEnabled = true;
		}
	}
}
