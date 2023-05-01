using System.IO;
using System.Threading.Tasks;
using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft;
using CmlLib.Core.Auth.Microsoft.MsalClient;
using Crafty.Core;
using Microsoft.Identity.Client;

namespace Crafty.Managers
{
	public static class MsalClientManager
	{
		private static string _clientID = "0c494150-132b-46f6-a53f-7c75afce7f61";
		private static IPublicClientApplication _msalApp;

		public static async Task<MSession?> Login()
		{
			_msalApp = await MsalMinecraftLoginHelper.BuildApplicationWithCache(_clientID);
			Launcher.LoginHandler = new LoginHandlerBuilder().WithCachePath($"{Launcher.MinecraftPath}/crafty_session.json").ForJavaEdition().WithMsalOAuth(_msalApp, factory => factory.CreateInteractiveApi()).Build();

			try
			{
				var session = await Launcher.LoginHandler.LoginFromCache();
				return session.GameSession;
			}

			catch
			{
				try
				{
					var session = await Launcher.LoginHandler.LoginFromOAuth();
					return session.GameSession;
				}

				catch { return null; }
			}
		}

		public static async void Logout()
		{
			foreach (var account in await _msalApp.GetAccountsAsync()) await _msalApp.RemoveAsync(account);
			try { await Launcher.LoginHandler.ClearCache(); } catch { }
			try { File.Delete($"{Launcher.MinecraftPath}/crafty_session.json"); } catch { }
		}
	}
}