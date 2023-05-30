using Avalonia;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using Crafty.Core;
using Crafty.Managers;
using Crafty.ViewModels;
using System;

namespace Crafty.Views
{
	public partial class SettingsWindow : ReactiveUserControl<SettingsWindowViewModel>
	{
		public SettingsWindow(SettingsWindowViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;

			RamSlider.Minimum = 1024;
			RamSlider.Maximum = Launcher.PhysicalMemory;
			RamSlider.Value = ConfigManager.Config.Ram;
			RamSlider.PropertyChanged += RamSlider_OnPropertyChanged;
		}

		private void RamSlider_OnPointerMoved(object? sender, PointerEventArgs e) => RamText.Text = $"{RamSlider.Value}MB";
		private void RamSlider_OnPointerExited(object? sender, PointerEventArgs e) => RamText.Text = "RAM Usage";

		private void RamSlider_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
		{
			if (RamSlider != null) ConfigManager.Config.Ram = Convert.ToInt32(RamSlider.Value);
		}

		private void ShowSnapshotsCheck_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
		{
			if (ShowSnapshotsCheck.IsChecked != null) ConfigManager.Config.GetSnapshots = (bool)ShowSnapshotsCheck.IsChecked;
		}

		private void ShowBetasCheck_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
		{
			if (ShowBetasCheck.IsChecked != null) ConfigManager.Config.GetBetas = (bool)ShowBetasCheck.IsChecked;
		}

		private void ShowAlphasCheck_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
		{
			if (ShowAlphasCheck.IsChecked != null) ConfigManager.Config.GetAlphas = (bool)ShowAlphasCheck.IsChecked;
		}
	}
}
