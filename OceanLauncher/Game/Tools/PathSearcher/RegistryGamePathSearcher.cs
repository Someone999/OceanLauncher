using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using OceanLauncher.Launcher;

namespace OceanLauncher.Game.Tools.PathSearcher
{
    public class RegistryGamePathSearcher : IGamePathSearcher
    {
        /// <summary>
        /// 在注册表中搜索启动器地址
        /// </summary>
        /// <returns>启动器的地址</returns>
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
                    return "";
                }


            }

            launcherPath = launcherPath ?? "";
            byte[] bytePath = Encoding.UTF8.GetBytes(launcherPath);//编码转换
            string path = Encoding.UTF8.GetString(bytePath);
            return path;
        }
        
        
        public LauncherInfo[] Search()
        {
            string path = GetLauncherPath();
            return Directory.Exists(path)
                ? new[] {new LauncherInfo(path)}
                : Array.Empty<LauncherInfo>();
        }
    }
}