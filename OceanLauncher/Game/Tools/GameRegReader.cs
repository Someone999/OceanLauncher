using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using OceanLauncher.Config;
using OceanLauncher.Game;
using OceanLauncher.Launcher;

namespace OceanLauncher.Utils
{
    public static class GameRegReader
    {
        /// <summary>
        /// 获取游戏目录，是静态方法
        /// </summary>
        /// <returns></returns>
        public static string GetGamePath()
        {
            string launcherPath = GetLauncherPath();

            #region 获取游戏启动路径，和官方配置一致

            string cfgPath = Path.Combine(launcherPath, "config.ini");
            if (File.Exists(launcherPath) || File.Exists(cfgPath))
            {
                //获取游戏本体路径
                return new GameInfo(launcherPath).GamePath;
            }
            
            return null;

            #endregion

        }


        /// <summary>
        /// 启动器地址
        /// </summary>
        /// <returns></returns>
        public static string GetLauncherPath()
        {
            RegistryKey key = Registry.LocalMachine;//打开指定注册表根
            //获取官方启动器路径
            string launcherPath = "";
            try
            {
                var regKeyOcean = key.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Genshin Impact");

                launcherPath = regKeyOcean?.GetValue("InstallPath").ToString();

            }
            catch (Exception)
            {
                try
                {
                    var regKeyChinese = key.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\原神");

                    launcherPath = regKeyChinese?.GetValue("InstallPath").ToString();
                }
                catch (Exception e)
                {
                    return null;
                }


            }
            
            return launcherPath;

        }
    }
}