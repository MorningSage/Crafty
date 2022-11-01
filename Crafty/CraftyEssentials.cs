using Downloader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CmlLib.Core.Auth;
using CmlLib.Core;
using System.Collections.Generic;

namespace Crafty;

public class Version
{
    public string name { get; set; }
    public string id { get; set; }
    public string installed { get; set; }

    public Version(string Name, string Id)
    {
        name = Name;
        id = Id;
    }
}

public static class CraftyLauncher
{
    public static string CraftyPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/.crafty";
    public static string JavaPath = $"{CraftyPath}/java";
    public static MinecraftPath Path = new MinecraftPath(CraftyPath);
    public static CMLauncher Launcher = new CMLauncher(Path);
    public static List<Version> VersionList = new List<Version>();
    public static List<Version> FabricVersionList = new List<Version>();
    public static bool LoggedIn = false;
    public static MSession Session = null;
    public static MLaunchOption LauncherOptions = new MLaunchOption
    {
        MaximumRamMb = 2048,
        Session = Session,
    };
}

public static class CraftyEssentials
{
    public static string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_1234567890";
    private static string VersionManifest = "https://piston-meta.mojang.com/mc/game/version_manifest.json";
    private static int MaxTasks = 128;
    private static DownloadConfiguration DownloadConfig = new DownloadConfiguration()
    {
        ChunkCount = 8,
        MaxTryAgainOnFailover = 5,
        ParallelDownload = true,
        ParallelCount = 4,
        Timeout = 1000,
    };

    private static Random random = new Random();

    private static string RandomString(int length)
    {
        return new string(Enumerable.Repeat(AllowedChars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static bool CheckUsername(string username)
    {
        foreach (char unvalid in username)
        {
            if (!AllowedChars.Contains(unvalid.ToString()))
                return false;
        }

        if (username.Length < 3 || username.Length > 16 || string.IsNullOrEmpty(username)) { return false; }

        return true;
    }

    public static void GetVersions()
    {
        var Client = new RestClient(VersionManifest);
        var Request = new RestRequest();
        var Response = Client.Execute(Request);

        var Json = JObject.Parse(Response.Content);
        Json.Remove("latest");
        var Versions = Json.Values().Children();

        foreach (var Version in Versions)
        {
            string id = (string)Version["id"];
            string type = (string)Version["type"];
            string url = (string)Version["url"];
            string time = (string)Version["time"];
            string releaseTime = (string)Version["releaseTime"];

            if (type == "release")
            {
                CraftyLauncher.VersionList.Add(new Version(id, id));
                Debug.WriteLine($"Added {id}");
            }
        }

        var OfficialVersions = CraftyLauncher.Launcher.GetAllVersions();
        // var FabricVersions = new FabricVersionLoader().GetFabricLoaders();
        // Saved for later ;)

        foreach (var item in OfficialVersions)
        {
            if (item.IsLocalVersion && !CraftyLauncher.VersionList.Any(x => x.name == item.Name))
            {
                CraftyLauncher.VersionList.Add(new Version($"{item.Name} (Installed)", item.Name));
                Debug.WriteLine($"Added already installed {item.Name}");
            }

            else if (item.IsLocalVersion && CraftyLauncher.VersionList.Any(x => x.name == item.Name))
            {
                var Index = CraftyLauncher.VersionList.FindIndex(x => x.name == item.Name);
                var VersionToEdit = CraftyLauncher.VersionList.Find(x => x.name == item.Name);
                VersionToEdit.name = $"{item.Name} (Installed)";
                CraftyLauncher.VersionList[Index] = VersionToEdit;
                Debug.WriteLine($"Changed status of {item.Name} to installed");
            }
        }
    }

    public static async Task DownloadVersion(string version)
    {
        Directory.CreateDirectory($"{CraftyLauncher.CraftyPath}/versions/{version}");
        string Path = $"{CraftyLauncher.CraftyPath}/versions/{version}/{version}.jar";
        if (File.Exists(Path)) { return; }

        WebClient Client = new WebClient();
        string Website = Client.DownloadString($"https://mcversions.net/download/{version}");

        foreach (LinkItem Item in LinkFinder.Find(Website))
        {
            if (Item.Text == "Download Client Jar")
            {
                var Downloader = new DownloadService(DownloadConfig);
                await Downloader.DownloadFileTaskAsync(Item.Href, Path);

                var Index = CraftyLauncher.VersionList.FindIndex(x => x.name == version);
                var VersionToEdit = CraftyLauncher.VersionList.Find(x => x.name == version);
                VersionToEdit.name = $"{version} (Installed)";
                CraftyLauncher.VersionList[Index] = VersionToEdit;
                Debug.WriteLine($"Changed status of {version} to installed");

                return;
            }
        }
    }

    public static async Task DownloadJson(string version)
    {
        Directory.CreateDirectory($"{CraftyLauncher.CraftyPath}/versions/{version}");
        string Path = $"{CraftyLauncher.CraftyPath}/versions/{version}/{version}.json";
        if (File.Exists(Path)) { return; }

        WebClient Client = new WebClient();
        string Website = Client.DownloadString($"https://minecraft.fandom.com/wiki/Java_Edition_{version}");

        foreach (LinkItem Item in LinkFinder.Find(Website))
        {
            if (Item.Text == ".json")
            {
                var Downloader = new DownloadService(DownloadConfig);
                await Downloader.DownloadFileTaskAsync(Item.Href, Path);

                return;
            }
        }
    }

    public static async Task DownloadJava()
    {
        Directory.CreateDirectory($"{CraftyLauncher.CraftyPath}/temp");
        string TempPath = $"{CraftyLauncher.CraftyPath}/temp/{RandomString(10)}.zip";
        if (File.Exists($"{CraftyLauncher.JavaPath}/bin/javaw.exe")) { return; }

        var Downloader = new DownloadService(DownloadConfig);
        await Downloader.DownloadFileTaskAsync("https://cdn.azul.com/zulu/bin/zulu19.30.11-ca-jre19.0.1-win_x64.zip", TempPath);

        await Task.Run(async () =>
        {
            string JavaVersion;

            using (ZipArchive Zip = ZipFile.Open(TempPath, ZipArchiveMode.Update))
            {
                Zip.ExtractToDirectory($"{CraftyLauncher.CraftyPath}/temp/");
                JavaVersion = Zip.Entries.First().ToString();
            }

            if (Directory.Exists(CraftyLauncher.JavaPath)) { Directory.Delete(CraftyLauncher.JavaPath); }
            Directory.Move($"{CraftyLauncher.CraftyPath}/temp/{JavaVersion}", CraftyLauncher.JavaPath);

            await ClearTemp();
        });
    }

    private static async Task ClearTemp()
    {
        DirectoryInfo TempPath = new DirectoryInfo($"{CraftyLauncher.CraftyPath}/temp");

        foreach (FileInfo File in TempPath.GetFiles())
        {
            File.Delete();
        }

        foreach (DirectoryInfo Directory in TempPath.GetDirectories())
        {
            Directory.Delete(true);
        }
    }

    public static async Task DownloadAssets(string version)
    {
        Directory.CreateDirectory($"{CraftyLauncher.CraftyPath}/assets/indexes");
        Directory.CreateDirectory($"{CraftyLauncher.CraftyPath}/assets/objects");

        string JsonUrl = GetPackageUrl(version);
        string JsonId = GetPackageId(version);
        string JsonPath = $"{CraftyLauncher.CraftyPath}/assets/indexes/{JsonId}.json";

        if (!File.Exists(JsonPath)) {
            var IndexDownloader = new DownloadService(DownloadConfig);
            await IndexDownloader.DownloadFileTaskAsync(JsonUrl, JsonPath);
        }

        StreamReader Read = new StreamReader(JsonPath);
        var Json = JObject.Parse(Read.ReadToEnd()).Values();
        Read.Close();
        var Assets = Json.Children().ToArray();
        int Remaining = Assets.Count();
        int Done = 0;
        int Tasks = 0;

        foreach (var Object in Assets)
        {
            foreach (var ObjectInfo in Object)
            {
                string Hash = (string)ObjectInfo["hash"];
                int Size = (int)ObjectInfo["size"];
                string ShortHash = Hash.Substring(0, 2);
                string ObjectPath = $"{CraftyLauncher.CraftyPath}/assets/objects/{ShortHash}";

                FileInfo HashFile = new FileInfo($"{ObjectPath}/{Hash}");
                if (HashFile.Exists && HashFile.Length == Size) { Remaining--; }
            }
        }

        foreach (var Object in Assets)
        {
            foreach (var ObjectInfo in Object)
            {
                while (Tasks > MaxTasks) { await Task.Delay(1000); }
                Tasks++;

                string Hash = (string)ObjectInfo["hash"];
                int Size = (int)ObjectInfo["size"];
                string ShortHash = Hash.Substring(0, 2);
                string ObjectPath = $"{CraftyLauncher.CraftyPath}/assets/objects/{ShortHash}";
                string HashPath = $"{ObjectPath}/{Hash}";
                string Url = $"http://resources.download.minecraft.net/{ShortHash}/{Hash}";

                FileInfo HashFile = new FileInfo($"{ObjectPath}/{Hash}");
                if (HashFile.Exists)
                { 
                    if (HashFile.Length != Size)
                    { 
                        try { HashFile.Delete(); }

                        catch (IOException)
                        {
                            Tasks--;
                            continue;
                        }
                    }

                    else
                    {
                        Tasks--;
                        continue; 
                    }
                }

                Action DownloadAction = async () =>
                {
                    Directory.CreateDirectory(ObjectPath);
                    var Downloader = new DownloadService(DownloadConfig);
                    await Downloader.DownloadFileTaskAsync(Url, HashPath);
                    Done++;
                    await MainWindow.Current.ChangeDownloadText($"Downloading assets ({Done}/{Remaining})");
                    Tasks--;
                };

                Task DownloadThread = new Task(DownloadAction);
                DownloadThread.Start();
            }
        }

        while (Tasks > 0) { await Task.Delay(1000); }
    }

    public static async Task DownloadLibraries(string version)
    {
        Directory.CreateDirectory($"{CraftyLauncher.CraftyPath}/libraries");

        string JsonPath = $"{CraftyLauncher.CraftyPath}/versions/{version}/{version}.json";
        StreamReader Read = new StreamReader(JsonPath);
        JsonTextReader Reader = new JsonTextReader(Read);
        JObject Json = (JObject)JToken.ReadFrom(Reader);
        Read.Close();
        int Remaining = Json["libraries"].Count();
        int Done = 0;
        int Tasks = 0;

        foreach (var Object in Json["libraries"])
        {
            string LibraryPath = (string)Object["downloads"].SelectTokens("$..path").Last();
            int Size = (int)Object["downloads"].SelectTokens("$..size").Last();

            FileInfo LibraryFile = new FileInfo(LibraryPath);
            if (LibraryFile.Exists && LibraryFile.Length == Size) { Remaining--; }
        }

        foreach (var Object in Json["libraries"])
        {
            while (Tasks > MaxTasks) { await Task.Delay(1000); }
            Tasks++;

            string LibraryPath = (string)Object["downloads"].SelectTokens("$..path").Last();
            string LibraryFolderPath = Path.GetDirectoryName(LibraryPath);
            int Size = (int)Object["downloads"].SelectTokens("$..size").Last();
            string Url = $"https://libraries.minecraft.net/{LibraryPath}";

            FileInfo LibraryFile = new FileInfo(LibraryPath);
            if (LibraryFile.Exists)
            {
                if (LibraryFile.Length != Size)
                { 
                    try { LibraryFile.Delete(); }

                    catch (IOException)
                    {
                        Tasks--;
                        continue;
                    }
                }

                else
                {
                    Tasks--;
                    continue;
                }
            }

            Action DownloadAction = async () =>
            {
                Directory.CreateDirectory(LibraryFolderPath);
                var Downloader = new DownloadService(DownloadConfig);
                await Downloader.DownloadFileTaskAsync(Url, LibraryPath);
                Done++;
                await MainWindow.Current.ChangeDownloadText($"Downloading libraries ({Done}/{Remaining})");
                Tasks--;
            };

            Task DownloadThread = new Task(DownloadAction);
            DownloadThread.Start();
        }

        while (Tasks > 0) { await Task.Delay(1000); }
    }

    private static string GetPackageUrl(string version)
    {
        string JsonPath = $"{CraftyLauncher.CraftyPath}/versions/{version}/{version}.json";
        StreamReader Read = new StreamReader(JsonPath);
        JsonTextReader Reader = new JsonTextReader(Read);
        JObject Json = (JObject)JToken.ReadFrom(Reader);

        string Url = (string)Json["assetIndex"]["url"];
        return Url;
    }

    private static string GetPackageId(string version)
    {
        string JsonPath = $"{CraftyLauncher.CraftyPath}/versions/{version}/{version}.json";
        StreamReader Read = new StreamReader(JsonPath);
        JsonTextReader Reader = new JsonTextReader(Read);
        JObject Json = (JObject)JToken.ReadFrom(Reader);

        string Id = (string)Json["assetIndex"]["id"];
        return Id;
    }
}
