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
				case AboutWindowViewModel context:
					return new AboutWindow(context);
				case AccountWindowViewModel context:
					return new AccountWindow(context);
				case ModBrowserWindowViewModel context:
					return new ModBrowserWindow(context);
				case ModWindowViewModel context:
					return new ModWindow(context);
				case SettingsWindowViewModel context:
					return new SettingsWindow(context);
				default:
					throw new ArgumentOutOfRangeException(nameof(viewModel));
			}
		}
	}
}