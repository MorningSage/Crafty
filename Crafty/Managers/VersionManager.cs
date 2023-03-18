using Crafty.Core;
using Avalonia.Collections;
using Crafty.Models;

namespace Crafty.Managers;

public static class VersionManager
{
	public static AvaloniaList<Version> GetVersions()
	{
		Config config = ConfigManager.ReturnConfig();

		AvaloniaList<Version> versionList = new();
	    var versions = Launcher.CmLauncher.GetAllVersions();
        // var FabricVersions = new FabricVersionLoader().GetFabricLoaders();

        foreach (var version in versions)
        {
	        if (version.IsLocalVersion) versionList.Add(new Version($"✅ {version.Name}", version.Name, "local", true));
	        else if (version.Type == "release") versionList.Add(new Version(version.Name, version.Name, version.Type));
	        else if (version.Type == "snapshot" && config.GetSnapshots) versionList.Add(new Version(version.Name, version.Name, version.Type));
	        else if (version.Type == "old_beta" && config.GetBetas) versionList.Add(new Version(version.Name, version.Name, version.Type));
	        else if (version.Type == "old_alpha" && config.GetAlphas) versionList.Add(new Version(version.Name, version.Name, version.Type));
        }

        return versionList;
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