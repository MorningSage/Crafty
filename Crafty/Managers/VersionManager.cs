using System.IO;
using Crafty.Core;
using Avalonia.Collections;
using CmlLib.Core.Version;
using Crafty.Models;

namespace Crafty.Managers;

public static class VersionManager
{
	public static MVersionCollection MVersionList = GetMVersions();

	public static AvaloniaList<Version> GetVersions()
	{
		Config config = ConfigManager.ReturnConfig();

		MVersionCollection versions;
		try { versions = Launcher.CmLauncher.GetAllVersions(); }
		catch { versions = Launcher.LocalVersionLoader.GetVersionMetadatas(); }

		AvaloniaList<Version> versionList = new();

		foreach (var version in versions)
        {
	        if (version.IsLocalVersion && File.Exists($"{Launcher.MinecraftPath}/versions/{version.Name}/{version.Name}.jar")) versionList.Add(new Version($"✅ {version.Name}", version.Name, "local", true));
	        else if (version.Type == "release") versionList.Add(new Version(version.Name, version.Name, version.Type));
	        else if (version.Type == "snapshot" && config.GetSnapshots) versionList.Add(new Version(version.Name, version.Name, version.Type));
	        else if (version.Type == "old_beta" && config.GetBetas) versionList.Add(new Version(version.Name, version.Name, version.Type));
	        else if (version.Type == "old_alpha" && config.GetAlphas) versionList.Add(new Version(version.Name, version.Name, version.Type));
        }

        return versionList;
	}

	private static MVersionCollection GetMVersions()
	{
		MVersionCollection versions;
		try { versions = Launcher.CmLauncher.GetAllVersions(); }
		catch { versions = Launcher.LocalVersionLoader.GetVersionMetadatas(); }

		return versions;
	}

	public static void UpdateVersion(Version version)
	{
		if (version.IsInstalled) return;

		int index = Launcher.VersionList.IndexOf(version);
		version.Name = $"✅ {version.Name}";
		version.IsInstalled = true;
		Launcher.VersionList[index] = version;
	}
}