using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Crafty.Core;
using Crafty.Managers;

namespace Crafty.Views
{
	public partial class SettingsWindow : Window
	{
		public SettingsWindow()
		{
			InitializeComponent();
		}

		private void CloseClicked(object? sender, RoutedEventArgs e) => Close();

		private void SaveClicked(object? sender, RoutedEventArgs e)
		{
			ConfigManager.Config.Ram = Convert.ToInt32(RamSlider.Value);
			ConfigManager.Config.GetSnapshots = (bool)ShowSnapshotsCheck.IsChecked;
			ConfigManager.Config.GetBetas = (bool)ShowBetasCheck.IsChecked;
			ConfigManager.Config.GetAlphas = (bool)ShowAlphasCheck.IsChecked;
			ConfigManager.SaveConfig();
			Launcher.VersionList = VersionManager.GetVersions();
			Close();
		}

		private void RamSlider_OnPointerMoved(object? sender, PointerEventArgs e) { RamText.Text = $"{RamSlider.Value}MB"; }
		private void RamSlider_OnPointerExited(object? sender, PointerEventArgs e) { RamText.Text = "RAM Usage"; }
	}
}
