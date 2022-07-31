using Microsoft.Win32;
using System;
using System.IO;
using System.Text;

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
            string startPath = "";
            string launcherPath = GetLauncherPath();
            #region 获取游戏启动路径，和官方配置一致
            string cfgPath = Path.Combine(launcherPath, "config.ini");
            if (File.Exists(launcherPath) || File.Exists(cfgPath))
            {
                //获取游戏本体路径
                using (StreamReader reader = new StreamReader(cfgPath))
                {
                    string[] abc = reader.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.None);
                    foreach (var item in abc)
                    {
                        //从官方获取更多配置
                        if (item.IndexOf("game_install_path", StringComparison.Ordinal) != -1)
                        {
                            startPath += item.Substring(item.IndexOf("=", StringComparison.Ordinal) + 1);
                        }
                    }
                }
            }
            byte[] byteArr = Encoding.UTF8.GetBytes(startPath);
            string path = Encoding.UTF8.GetString(byteArr);
            return path;
            #endregion
        }


        /// <summary>
        /// 启动器地址
        /// </summary>
        /// <returns></returns>
        public static string GetLauncherPath()
        {
            RegistryKey key = Registry.LocalMachine;            //打开指定注册表根
                                                                //获取官方启动器路径
            string launcherPath = "";
            try
            {
                var regKeyOcean = key.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Genshin Impact");

                launcherPath = regKeyOcean.GetValue("InstallPath").ToString();

            }
            catch (Exception)
            {
                try
                {
                    var regKeyChinese = key.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\原神");
                
                    launcherPath = regKeyChinese.GetValue("InstallPath").ToString();
                }
                catch (Exception e)
                {
                    return "";
                }

                
            }

            byte[] bytePath = Encoding.UTF8.GetBytes(launcherPath);     //编码转换
            string path = Encoding.UTF8.GetString(bytePath);
            return path;

        }
    }
}
