﻿using System;
using System.Diagnostics;
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
		private static string _clientID = "ENTER_YOUR_CLIENT_ID_HERE";
		private static IPublicClientApplication _msalApp;

		public static async Task<MSession> Login()
		{
			_msalApp = await MsalMinecraftLoginHelper.BuildApplicationWithCache(_clientID);
			Launcher.LoginHandler = new LoginHandlerBuilder().WithCachePath($"{Launcher.MinecraftPath}/crafty_session.json").ForJavaEdition().WithMsalOAuth(_msalApp, factory => factory.CreateInteractiveApi()).Build();

			try
			{
				var session = await Launcher.LoginHandler.LoginFromCache();
				return session.GameSession;
			}

			catch (Exception e)
			{
				var session = await Launcher.LoginHandler.LoginFromOAuth();
				return session.GameSession;
			}
		}

		public static async void Logout()
		{
			var accounts = await _msalApp.GetAccountsAsync();
			foreach (var account in accounts) await _msalApp.RemoveAsync(account);
			try { await Launcher.LoginHandler.ClearCache(); } catch { Debug.WriteLine("Couldn't clear cache!"); }
			try { File.Delete($"{Launcher.MinecraftPath}/crafty_session.json"); } catch { Debug.WriteLine("Couldn't delete cache file!"); }
		}
	}
}