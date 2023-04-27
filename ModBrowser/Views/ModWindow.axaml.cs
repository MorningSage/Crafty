using Avalonia.Controls;
using ModBrowser.Models;

namespace ModBrowser.Views
{
	public partial class ModWindow : Window
	{
		public Mod Mod { get; set; }

		public ModWindow(Mod mod)
		{
			InitializeComponent();
			Mod = mod;
			DataContext = Mod;
		}
	}
}