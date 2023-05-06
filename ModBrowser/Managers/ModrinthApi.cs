using Modrinth;

namespace ModBrowser.Managers
{
	public static class ModrinthApi
	{
		private static UserAgent _userAgent = new()
		{
			ProjectName = "Crafty",
			GitHubUsername = "Heapy1337"
		};

		public static ModrinthClient Client = new(userAgent: _userAgent);
	}
}