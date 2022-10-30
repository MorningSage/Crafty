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

namespace Crafty;

public static class CraftyEssentials
{
    public static string CraftyPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/.crafty";
    public static string JavaPath = $"{CraftyPath}/java";
    public static string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_1234567890";
    private static string VersionManifest = "https://piston-meta.mojang.com/mc/game/version_manifest.json";
    public static string LatestVersion = null;
    private static int MaxTasks = 128;
    public static bool LoggedIn = false;
    public static MSession Session = null;
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
        await Downloader.DownloadFileTaskAsync("https://cdn.azul.com/zulu/bin/zulu19.30.11-ca-jre19.0.1-win_x64.zip", TempPath);

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
    }

    private static async Task ClearTemp()
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
        int Remaining = Assets.Count();
        int Done = 0;
        int Tasks = 0;
        Task[] TaskList = { };

        foreach (var Object in Assets)
        {
            foreach (var ObjectInfo in Object)
            {
                string Hash = (string)ObjectInfo["hash"];
                int Size = (int)ObjectInfo["size"];
                string ShortHash = Hash.Substring(0, 2);
                string ObjectPath = $"{CraftyPath}/assets/objects/{ShortHash}";

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
                string ObjectPath = $"{CraftyPath}/assets/objects/{ShortHash}";
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
                    await MainWindow.Current.ChangeDownloadText($"Downloading assets ({Done}/{Remaining})");
                    Directory.CreateDirectory(ObjectPath);
                    var Downloader = new DownloadService(DownloadConfig);
                    await Downloader.DownloadFileTaskAsync(Url, HashPath);
                    Done++;
                    Tasks--;
                };

                Task DownloadThread = new Task(DownloadAction);
                await Task.Run(() => DownloadThread.Start());
                TaskList.Append(DownloadThread);
            }
        }

        Task.WaitAll(TaskList);
    }

    public static async Task DownloadLibraries(string version)
    {
        Directory.CreateDirectory($"{CraftyPath}/libraries");

        string JsonPath = $"{CraftyPath}/versions/{version}/{version}.json";
        StreamReader Read = new StreamReader(JsonPath);
        JsonTextReader Reader = new JsonTextReader(Read);
        JObject Json = (JObject)JToken.ReadFrom(Reader);
        int Remaining = Json["libraries"].Count();
        int Done = 0;
        int Tasks = 0;
        Task[] TaskList = { };

        foreach (var Object in Json["libraries"])
        {
            string LibraryPath = (string)Object["downloads"].SelectTokens("$..path").Last();
            string LibraryFolderPath = Path.GetDirectoryName(LibraryPath);
            int Size = (int)Object["downloads"].SelectTokens("$..size").Last();
            string Url = $"https://libraries.minecraft.net/{LibraryPath}";

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
                await MainWindow.Current.ChangeDownloadText($"Downloading libraries ({Done}/{Remaining})");
                Directory.CreateDirectory(LibraryFolderPath);
                var Downloader = new DownloadService(DownloadConfig);
                await Downloader.DownloadFileTaskAsync(Url, LibraryPath);
                Done++;
                Tasks--;
            };

            Task DownloadThread = new Task(DownloadAction);
            await Task.Run(() => DownloadThread.Start());
            TaskList.Append(DownloadThread);
        }

        Task.WaitAll(TaskList);
    }

    private static string GetPackageUrl(string version)
    {
        string JsonPath = $"{CraftyPath}/versions/{version}/{version}.json";
        StreamReader Read = new StreamReader(JsonPath);
        JsonTextReader Reader = new JsonTextReader(Read);
        JObject Json = (JObject)JToken.ReadFrom(Reader);

        string Url = (string)Json["assetIndex"]["url"];
        return Url;
    }

    private static string GetPackageId(string version)
    {
        string JsonPath = $"{CraftyPath}/versions/{version}/{version}.json";
        StreamReader Read = new StreamReader(JsonPath);
        JsonTextReader Reader = new JsonTextReader(Read);
        JObject Json = (JObject)JToken.ReadFrom(Reader);

        string Id = (string)Json["assetIndex"]["id"];
        return Id;
    }
}
