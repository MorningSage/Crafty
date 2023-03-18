namespace Crafty.Models
{
	public class Config
	{
		public string? Username { get; set; }
		public int Ram { get; set; }
		public string? LastVersionUsed { get; set; }
		public bool GetSnapshots { get; set; }
		public bool GetBetas { get; set; }
		public bool GetAlphas { get; set; }

		public Config(string? username, int ram, string? lastVersionUsed, bool getSnapshots, bool getBetas, bool getAlphas)
		{
			Username = username;
			Ram = ram;
			LastVersionUsed = lastVersionUsed;
			GetSnapshots = getSnapshots;
			GetBetas = getBetas;
			GetAlphas = getAlphas;
		}
	}
}
