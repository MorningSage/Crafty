using Modrinth;

namespace Crafty.Managers
{
	public static class ModrinthManager
	{
		private static UserAgent _userAgent = new()
		{
			ProjectName = "Crafty",
			GitHubUsername = "Heapy1337"
		};

		public static ModrinthClient Client = new(userAgent: _userAgent);
	}
}