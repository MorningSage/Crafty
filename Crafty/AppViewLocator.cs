using Crafty.ViewModels;
using Crafty.Views;
using ReactiveUI;
using System;

namespace Crafty
{
	public class AppViewLocator : IViewLocator
	{
		IViewFor IViewLocator.ResolveView<T>(T viewModel, string contract)
		{
			switch (viewModel)
			{
				case AboutPageViewModel context:
					return new AboutPage(context);
				case AccountPageViewModel context:
					return new AccountPage(context);
				case ModBrowserPageViewModel context:
					return new ModBrowserPage(context);
				case ModOverviewPageViewModel context:
					return new ModOverviewPage(context);
				case SettingsPageViewModel context:
					return new SettingsPage(context);
				default:
					throw new ArgumentOutOfRangeException(nameof(viewModel));
			}
		}
	}
}