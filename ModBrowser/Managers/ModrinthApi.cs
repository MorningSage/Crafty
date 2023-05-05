using Modrinth;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

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

		public static async Task<string> GetProjectDownloadUrl(string projectVersion)
		{
			HttpResponseMessage response = await new HttpClient().GetAsync($"https://api.modrinth.com/v2/version/{projectVersion}");
			string data = await response.Content.ReadAsStringAsync();

			JObject json = JObject.Parse(data);
			return json.SelectToken("files[0].url").ToString();
		}
	}
}