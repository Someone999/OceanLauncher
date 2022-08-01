using System.IO;
using OceanLauncher.Config.Json;

namespace OceanLauncher.Config
{
    public static class Configs
    {
        static Configs()
        {
            string[] configFiles = new[] {"./Config/cfg.json"};
            foreach (var configFile in configFiles)
            {
                if (File.Exists(configFile))
                {
                    continue;
                }
                
                if (!Directory.Exists(Path.GetDirectoryName(configFile)))
                {
                    string dirName = Path.GetDirectoryName(configFile);
                    if (dirName == null)
                    {
                        continue;
                    }
                    
                    Directory.CreateDirectory(dirName);
                }

                File.Create(configFile);

            }
        }
        
        public static ICanSaveConfigElement LauncherConfig { get; } = new JsonConfig("./Config/cfg.json");
    }
}