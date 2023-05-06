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
				case ModWindowViewModel context:
					return new ModWindow(context);
				default:
					throw new ArgumentOutOfRangeException(nameof(viewModel));
			}
		}
	}
}