using Crafty.Core;
using Crafty.Models;
using Newtonsoft.Json;
using System.IO;

namespace Crafty.Managers
{
	public static class ConfigManager
	{
		public static Config Config;

		public static async void SaveConfig()
		{
			var json = JsonConvert.SerializeObject(Config);
			await File.WriteAllTextAsync(Launcher.MinecraftPath + "/config.json", json);
		}

		public static void LoadConfig()
		{
			try
			{
				string json = File.ReadAllText(Launcher.MinecraftPath + "/config.json");
				Config = JsonConvert.DeserializeObject<Config>(json);
			}
			catch
			{
				Config = new Config(null, 2048, null, false, false, false);
				SaveConfig();
			}
		}

		public static Config ReturnConfig()
		{
			try
			{
				string json = File.ReadAllText(Launcher.MinecraftPath + "/config.json");
				return JsonConvert.DeserializeObject<Config>(json);
			}
			catch
			{
				return new Config(null, 2048, null, false, false, false);
			}
		}
	}
}