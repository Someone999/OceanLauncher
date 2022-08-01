using System;
using System.IO;
using System.Windows.Controls;
using OceanLauncher.Config;
using OceanLauncher.Game.Patch;
using OceanLauncher.Launcher;

namespace OceanLauncher.Game
{
    public class GameInfo
    {
        public GameInfo(string launcherDirectory)
        {
            string template = "Genshin Impact Game{0}{1}";
            string chineseServer = string.Format(template, Path.DirectorySeparatorChar, "YuanShen_Data");
            string oceanServer = string.Format(template, Path.DirectorySeparatorChar, "GenshinImpact_Data");
            bool noChineseGame = !Directory.Exists(Path.Combine(launcherDirectory, chineseServer));
            bool noOceanGame = !Directory.Exists(Path.Combine(launcherDirectory, oceanServer));
            if (noChineseGame && noOceanGame)
            {
                ExecutableFile = null;
                ConfigFile = null;
                PatchFolder = null;
            }

            ClientType = noChineseGame
                ? noOceanGame
                    ? ClientType.None
                    : ClientType.Ocean
                : ClientType.Chinese;

            switch (ClientType)
            {
                case ClientType.None:
                    ExecutableFile = null;
                    ConfigFile = null;
                    PatchFolder = null;
                    break;
                
                case ClientType.Chinese:
                    ExecutableFile = Path.Combine(launcherDirectory, "Genshin Impact Game", "YuanShen.exe");
                    PatchFolder = Path.Combine(launcherDirectory, chineseServer, "Managed", "Metadata", "global-metadata.dat");
                    ConfigFile = Path.Combine(launcherDirectory, "Genshin Impact Game", "config.ini");
                    break;
                
                case ClientType.Ocean:
                    ExecutableFile = Path.Combine(launcherDirectory, "Genshin Impact Game", "GenshinImpact.exe");
                    PatchFolder = Path.Combine(launcherDirectory, oceanServer, "Managed", "Metadata", "global-metadata.dat");
                    ConfigFile = null;
                    break;
                
                case ClientType.NotSupported:
                    ClientType = ClientType.NotSupported;
                    ExecutableFile = null;
                    ConfigFile = null;
                    PatchFolder = null;
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            //此处的赋值是正确的
            if (!(HasConfig = ConfigFile != null))
            {
                Config = null;
                return;
            }
            
            Config = new IniConfig(ConfigFile);
            _launcherDir = launcherDirectory;
            
            
            if (Directory.Exists(ExecutableFile) || ClientType == ClientType.NotSupported || ClientType == ClientType.None)
            {
                return;
            }
            
            //如果通过搜索文件夹找到的路径不存在，则使用启动器配置的游戏路径继续搜索
            LauncherInfo launcherInfo = GetLauncherInfo();
            var gamePath = launcherInfo.Config["General"]["game_install_path"].GetValue<string>();

            
            switch (ClientType)
            {
                case ClientType.None:
                    ExecutableFile = null;
                    ConfigFile = null;
                    PatchFolder = null;
                    break;
                
                case ClientType.Chinese:
                    ExecutableFile = Path.Combine(gamePath, "YuanShen.exe");
                    PatchFolder = Path.Combine(gamePath, "YuanShen_Data", "Managed", "Metadata", "global-metadata.dat");
                    ConfigFile = Path.Combine(launcherDirectory, "Genshin Impact Game", "config.ini");
                    break;
                
                case ClientType.Ocean:
                    ExecutableFile = Path.Combine(gamePath, "GenshinImpact.exe");
                    PatchFolder = Path.Combine(gamePath, "GenshinImpact_Data", "Managed", "Metadata", "global-metadata.dat");
                    ConfigFile = null;
                    break;
                
                case ClientType.NotSupported:
                    ClientType = ClientType.NotSupported;
                    ExecutableFile = null;
                    ConfigFile = null;
                    PatchFolder = null;
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
            HasConfig = ConfigFile != null;
            if (HasConfig)
            {
                return;
            }
            
            Config = null;
        }

        private string _launcherDir;
        public ClientType ClientType { get; }
        public string ExecutableFile { get; }
        public string ConfigFile { get; }
        public string PatchFolder { get; }
        public IConfigElement Config { get; }
        public bool HasConfig { get; }
        public LauncherInfo GetLauncherInfo() => new LauncherInfo(_launcherDir);

    }
}