namespace Crafty.Models
{
	public class ModVersion
	{
		public string Name { get; set; }
		public string VersionNumber { get; set; }
		public string DownloadUrl { get; set; }

		public ModVersion(string name, string versionNumber, string downloadUrl)
		{
			Name = name;
			VersionNumber = versionNumber;
			DownloadUrl = downloadUrl;
		}
	}
}
