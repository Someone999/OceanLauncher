using System.IO;
using OceanLauncher.Config;
using OceanLauncher.Game;

namespace OceanLauncher.Launcher
{
    /// <summary>
    /// 存储启动器信息的类
    /// </summary>
    public class LauncherInfo
    {
        public LauncherInfo(string launcherDirectory)
        {
            bool folderNotExists = !Directory.Exists(launcherDirectory);
            bool noLauncher = !File.Exists(Path.Combine(launcherDirectory, "launcher.exe"));
            bool noLauncherConfig = !File.Exists(Path.Combine(launcherDirectory, "config.ini"));

            if (folderNotExists || noLauncher || noLauncherConfig)
            {
                ExecutableFile = null;
                ConfigFile = null;
            }

            ExecutableFile = Path.Combine(launcherDirectory, "launcher.exe");
            ConfigFile = Path.Combine(launcherDirectory, "config.ini");
            Config = new IniConfig(ConfigFile);
        }
        
        public string ExecutableFile { get; }
        public string ConfigFile { get; }
        public IConfigElement Config { get; }
        public GameInfo GetGameInfo() => ExecutableFile == null
            ? null
            : new GameInfo(Path.GetDirectoryName(ExecutableFile));

    }
}