using System;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Crafty.Managers
{
	public static class RandomManager
	{
		public static Bitmap RandomCover()
		{
			IAssetLoader assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
			Uri uri = new Uri($"avares://Crafty/Assets/background-{new Random().Next(1, 4)}.png");
			return new Bitmap(assets.Open(uri));
		}
	}
}