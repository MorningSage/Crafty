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

namespace Crafty;

public static class CraftyEssentials
{
    public static string CraftyPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/.crafty";
    public static string JavaPath = $"{CraftyPath}/java";
    public static string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_123456789";
    public static string VersionManifest = "https://piston-meta.mojang.com/mc/game/version_manifest.json";
    public static string LatestVersion = null;
    public static DownloadConfiguration DownloadConfig = new DownloadConfiguration()
    {
        ChunkCount = 8,
        MaxTryAgainOnFailover = 5,
        ParallelDownload = true,
        ParallelCount = 4,
        Timeout = 1000,
    };

    private static Random random = new Random();

    public static string RandomString(int length)
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
                MainWindow.Current.AddVersion(id);
                Debug.WriteLine($"Added {id}");

                if (LatestVersion == null)
                {
                    LatestVersion = id;
                }
            }
        }
    }

    public static async Task DownloadVersion(string version)
    {
        Directory.CreateDirectory($"{CraftyPath}/versions/{version}");
        string Path = $"{CraftyPath}/versions/{version}/{version}.jar";
        if (File.Exists(Path)) { return; }

        WebClient Client = new WebClient();
        string Website = Client.DownloadString($"https://mcversions.net/download/{version}");

        foreach (LinkItem Item in LinkFinder.Find(Website))
        {
            if (Item.Text == "Download Client Jar")
            {
                var Downloader = new DownloadService(DownloadConfig);
                await Downloader.DownloadFileTaskAsync(Item.Href, Path);

                return;
            }
        }
    }

    public static async Task DownloadJson(string version)
    {
        Directory.CreateDirectory($"{CraftyPath}/versions/{version}");
        string Path = $"{CraftyPath}/versions/{version}/{version}.json";
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
        Directory.CreateDirectory($"{CraftyPath}/temp");
        string TempPath = $"{CraftyPath}/temp/{RandomString(10)}.zip";
        if (File.Exists($"{JavaPath}/bin/javaw.exe")) { return; }

        var Downloader = new DownloadService(DownloadConfig);
        await Downloader.DownloadFileTaskAsync("https://download.oracle.com/java/19/latest/jdk-19_windows-x64_bin.zip", TempPath);

        await Task.Run(async () =>
        {
            string JavaVersion;
            using (ZipArchive Zip = ZipFile.Open(TempPath, ZipArchiveMode.Update))
            {
                Zip.ExtractToDirectory($"{CraftyPath}/temp/");
                JavaVersion = Zip.Entries.First().ToString();
            }
            if (Directory.Exists(JavaPath)) { Directory.Delete(JavaPath); }
            Directory.Move($"{CraftyPath}/temp/{JavaVersion}", JavaPath);

            await ClearTemp();
        });
        // Unzipping is glitched and can cause app to crash - fix will be added later
    }

    public static async Task ClearTemp()
    {
        DirectoryInfo TempPath = new DirectoryInfo($"{CraftyPath}/temp");

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
        Directory.CreateDirectory($"{CraftyPath}/assets/indexes");
        Directory.CreateDirectory($"{CraftyPath}/assets/objects");
        string JsonUrl = GetPackageUrl(version);
        string JsonId = GetPackageId(version);
        string JsonPath = $"{CraftyPath}/assets/indexes/{JsonId}.json";

        if (!File.Exists(JsonPath)) {
            var IndexDownloader = new DownloadService(DownloadConfig);
            await IndexDownloader.DownloadFileTaskAsync(JsonUrl, JsonPath);
        }

        StreamReader Read = new StreamReader(JsonPath);
        var Json = JObject.Parse(Read.ReadToEnd()).Values();
        var Assets = Json.Children().ToArray();
        int AssetsLength = Assets.Count();
        int Done = 0;

        foreach (var Object in Assets)
        {
            foreach (var ObjectInfo in Object)
            {
                string Hash = (string)ObjectInfo["hash"];
                int Size = (int)ObjectInfo["size"];
                string ShortHash = Hash.Substring(0, 2);
                string ObjectPath = $"{CraftyPath}/assets/objects/{ShortHash}";
                Directory.CreateDirectory(ObjectPath);

                await MainWindow.Current.ChangeDownloadText($"Downloading assets... ({Done}/{AssetsLength})");
                Done++;

                FileInfo HashFile = new FileInfo($"{ObjectPath}/{Hash}");
                if (HashFile.Exists && HashFile.Length != Size) { HashFile.Delete(); }
                else if (HashFile.Exists && HashFile.Length == Size) { continue; }

                var Downloader = new DownloadService(DownloadConfig);
                await Downloader.DownloadFileTaskAsync($"http://resources.download.minecraft.net/{ShortHash}/{Hash}", $"{ObjectPath}/{Hash}");
            }
        }
    }

    public static async Task DownloadLibraries(string version)
    {
        Directory.CreateDirectory($"{CraftyPath}/libraries");

        string JsonPath = $"{CraftyPath}/versions/{version}/{version}.json";
        StreamReader Read = new StreamReader(JsonPath);
        JsonTextReader Reader = new JsonTextReader(Read);
        JObject Json = (JObject)JToken.ReadFrom(Reader);
        int LibrariesLength = Json["libraries"].Count();
        int Done = 0;

        foreach (var Object in Json["libraries"])
        {
            string LibraryPath = (string)Object["downloads"].SelectTokens("$..path").Last();
            string LibraryFolderPath = Path.GetDirectoryName(LibraryPath);
            int Size = (int)Object["downloads"].SelectTokens("$..size").Last();
            string Url = $"https://libraries.minecraft.net/{LibraryPath}";

            Directory.CreateDirectory(LibraryFolderPath);

            await MainWindow.Current.ChangeDownloadText($"Downloading libraries... ({Done}/{LibrariesLength})");
            Done++;

            FileInfo LibraryFile = new FileInfo(LibraryPath);
            if (LibraryFile.Exists && LibraryFile.Length != Size) { LibraryFile.Delete(); }
            else if (LibraryFile.Exists && LibraryFile.Length == Size) { continue; }

            var Downloader = new DownloadService(DownloadConfig);
            await Downloader.DownloadFileTaskAsync(Url, LibraryPath);
        }
    }

public static string GetPackageUrl(string version)
    {
        string JsonPath = $"{CraftyPath}/versions/{version}/{version}.json";
        StreamReader Read = new StreamReader(JsonPath);
        JsonTextReader Reader = new JsonTextReader(Read);
        JObject Json = (JObject)JToken.ReadFrom(Reader);

        string Url = (string)Json["assetIndex"]["url"];
        return Url;
    }

    public static string GetPackageId(string version)
    {
        string JsonPath = $"{CraftyPath}/versions/{version}/{version}.json";
        StreamReader Read = new StreamReader(JsonPath);
        JsonTextReader Reader = new JsonTextReader(Read);
        JObject Json = (JObject)JToken.ReadFrom(Reader);

        string Id = (string)Json["assetIndex"]["id"];
        return Id;
    }
}
