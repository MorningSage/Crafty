using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Threading.Tasks;

namespace Crafty.Managers
{
	public static class RandomManager
	{
		public static async Task<Bitmap> RandomCover()
		{
			Uri uri = new Uri($"avares://Crafty/Assets/background-{new Random().Next(1, 4)}.png");
			return new Bitmap(AssetLoader.Open(uri));
		}
	}
}