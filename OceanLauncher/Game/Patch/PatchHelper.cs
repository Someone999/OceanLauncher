using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Windows;
using Newtonsoft.Json;
using OceanLauncher.Config;
using OceanLauncher.Pages;

namespace OceanLauncher.Game.Patch
{

    public class PatchHelper
    {
        const string MetadataPath = "patch-metadata";
        const string FileName = "global-metadata.dat";
        private GameInfo _currentGameInfo;
        SettingPage.Config _config;

        public bool IsAdministrator()

        {

            WindowsIdentity current = WindowsIdentity.GetCurrent();
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);

        }
        

        public PatchHelper()
        {
            try
            {
                _config = JsonConvert.DeserializeObject<SettingPage.Config>(Configs.LauncherConfig[SettingPage.Id].GetValue<string>());
                //_config = JsonConvert.DeserializeObject<SettingPage.Config>(SettingProvider.Get(SettingPage.Id));
            }
            catch
            {
                _config = new SettingPage.Config();
            }
        }

        
        

        public void Patch()
        {
            if (!IsAdministrator())
            {
                string launchPath = Assembly.GetExecutingAssembly().Location;
                ProcessStartInfo startInfo = new ProcessStartInfo(launchPath)
                {
                    WorkingDirectory = Path.GetDirectoryName(launchPath) ?? "",
                    Verb = "runas"
                };

                Process.Start(startInfo);
                Environment.Exit(0); 
                //MessageBox.Show("未获取原神文件夹的读写权限，请以管理员身份运行启动器！");
                return;
            }

            _currentGameInfo = new GameInfo(_config.Path);
            string filePath = _currentGameInfo.PatchFolder ?? throw new ArgumentNullException("GetPatchDir()");

            //备份
            if (!File.Exists(Path.Combine(filePath, FileName + ".bak")))
            {
                File.Copy(
                    Path.Combine(filePath, FileName),
                    Path.Combine(filePath, FileName + ".bak"));
            }
            string patchedFile = "";
            switch (_currentGameInfo.ClientType)
            {
                //修补
                case ClientType.Chinese:
                    {
                        patchedFile = Path.Combine(MetadataPath, "cnrel-" + FileName);
                        if (File.Exists(patchedFile))
                        {
                            File.Copy(patchedFile, Path.Combine(filePath, FileName), true
                            );
                        }
                        else
                        {
                            MessageBox.Show($"找不到补丁文件");
                            return;
                        }
                        break;
                    }
                case ClientType.Ocean:
                    {
                        patchedFile = Path.Combine(MetadataPath, "osrel-" + FileName);
                        if (File.Exists(patchedFile))
                        {
                            File.Copy(patchedFile, Path.Combine(filePath, FileName), true
                            );
                        }
                        else
                        {
                            MessageBox.Show($"找不到补丁文件");
                            return;
                        }
                        break;
                    }
                case ClientType.None:
                case ClientType.NotSupported:
                default:
                    MessageBox.Show($"不支持的客户端类型！");
                    return;
            }
            MessageBox.Show($"成功Patch了客户端！");


        }

        public void UnPatch()
        {
            if (!IsAdministrator())
            {
                MessageBox.Show("未获取原神文件夹的读写权限，请以管理员身份运行启动器！");
                return;
            }

            try
            {
               var _ = Path.GetDirectoryName(_config.Path);

            }
            catch (Exception ex)
            {
                MessageBox.Show("游戏路径不正确！" + ex.Message);
                return
                ;
            }
            string filePath = _currentGameInfo.PatchFolder;

            switch (_currentGameInfo.ClientType)
            {
                case ClientType.Chinese when File.Exists(Path.Combine(filePath, FileName + ".bak")):
                    File.Copy(
                        Path.Combine(filePath, FileName + ".bak"),
                        Path.Combine(filePath, FileName), true);
                    break;
                case ClientType.Chinese:
                    MessageBox.Show("未找到备份文件！");
                    break;
                case ClientType.Ocean when File.Exists(Path.Combine(filePath, FileName + ".bak")):
                    File.Copy(
                        Path.Combine(filePath, FileName + ".bak"),
                        Path.Combine(filePath, FileName), true);
                    break;
                case ClientType.Ocean:
                    MessageBox.Show("未找到备份文件！");
                    break;
                case ClientType.None:
                case ClientType.NotSupported:
                default:
                    MessageBox.Show($"不支持的客户端类型！");
                    return;
            }

            MessageBox.Show($"成功UnPatch了客户端！");

        }

        public void OpenFolder()
        {
            if (!Directory.Exists(MetadataPath))
            {
                Directory.CreateDirectory(MetadataPath);
            }

            Process.Start(Path.Combine(Environment.CurrentDirectory, MetadataPath));

        }

        public void OpenPatchFolder()
        {
            if (!Directory.Exists(MetadataPath))
            {
                Directory.CreateDirectory(MetadataPath);
            }

            Process.Start(_currentGameInfo.PatchFolder);
        }

        public bool IsPatched()
        {
            var dir = _currentGameInfo.PatchFolder;
            var patched = false;
            try
            {
                //不相同即为已 Patch
                patched = !IsSameFile(Path.Combine(dir, FileName), Path.Combine(dir, FileName + ".bak"));

            }
            catch (Exception)
            {
                //Nothing to do
            }
            return patched;
        }

        public static bool IsSameFile(string filePath1, string filePath2)
        {
            //创建一个哈希算法对象
            using (HashAlgorithm hash = HashAlgorithm.Create())
            {
                using (FileStream file1 = new FileStream(filePath1, FileMode.Open), file2 = new FileStream(filePath2, FileMode.Open))
                {
                    byte[] hashByte1 = hash.ComputeHash(file1);//哈希算法根据文本得到哈希码的字节数组
                    byte[] hashByte2 = hash.ComputeHash(file2);
                    string str1 = BitConverter.ToString(hashByte1);//将字节数组装换为字符串
                    string str2 = BitConverter.ToString(hashByte2);
                    return str1 == str2;//比较哈希码
                }
            }
        }

    }
}
