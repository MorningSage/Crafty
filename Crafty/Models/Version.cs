namespace Crafty.Models
{
	public class Version
	{
		public string Name { get; set; }
		public string Id { get; set; }
		public string Type { get; set; }
		public bool IsInstalled { get; set; }

		public Version(string name, string id, string type, bool isInstalled = false)
		{
			Name = name;
			Id = id;
			Type = type;
			IsInstalled = isInstalled;
		}
	}
}
