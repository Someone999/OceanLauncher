using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OceanLauncher.Utils
{
    [Obsolete("这个类已经废弃，现已改用Configs.LauncherConfig")]
    public static class SettingProvider
    {

        private static string cfgPath = @"./Config/cfg.json";

        public static JObject GlobalSetting { get; set; }

        private static void Save(string s)
        {
            File.WriteAllText(cfgPath, s);
        }
        public static void Save()
        {
            Save(JsonConvert.SerializeObject(GlobalSetting));
        }
        public static void Init()
        {
            if (!File.Exists(cfgPath))
            {
                Directory.CreateDirectory(cfgPath.Replace("cfg.json", ""));
                Save("");
            }
            try
            {
                GlobalSetting = JObject.Parse(File.ReadAllText(cfgPath));

            }
            catch (Exception ex)
            {
                GlobalSetting = new JObject();
            }
        }
        public static void Set(string id, object obj)
        {
            GlobalSetting[id] = JsonConvert.SerializeObject(obj);
            Save(JsonConvert.SerializeObject(GlobalSetting));
        }
        public static void SetNoSave(string id, object obj)
        {
            GlobalSetting[id] = JsonConvert.SerializeObject(obj);
            //Save(JsonConvert.SerializeObject(GlobalSetting));
        }
        public static string Get(string id)
        {
            try
            {
                return GlobalSetting == null
                    ? ""
                    : GlobalSetting[id]?.ToString() ?? "";

            }
            catch
            {
                return "";
            }
        }

    }
}