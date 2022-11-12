using Newtonsoft.Json;
using System.IO;

namespace Crafty;

public static class CraftyConfig
{
    public static void writeFile(string username, double ram)
    {
        data.username = username;
        data.ram = (int)ram;

        var json = JsonConvert.SerializeObject(data);
        File.WriteAllTextAsync(CraftyLauncher.CraftyPath + "/config.json", json);
    }

    public static void loadFile()
    {
        if (File.Exists(CraftyLauncher.CraftyPath + "/config.json"))
        {
            string jsonString = File.ReadAllText(CraftyLauncher.CraftyPath + "/config.json");
            JsonConvert.PopulateObject(jsonString, data);

            isJsonExist = true;
            return;
        }
        isJsonExist = false;
    }

    public static string loadUsernameFromJson()
    {
        if (isJsonExist)
        {
            return data.username;
        }
        return "";
    }

    public static double loadRamFromJson()
    {
        if (isJsonExist)
        {
            return (double)data.ram;
        }
        return 2048;
    }


    private class Data_t
    {
        public string? username { get; set; }
        public int? ram { get; set; }
    }

    private static Data_t data = new Data_t { };
    private static bool isJsonExist;
}
