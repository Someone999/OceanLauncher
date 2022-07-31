using Newtonsoft.Json;
using OceanLauncher.Pages;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Windows;
using WpfWidgetDesktop.Utils;

namespace OceanLauncher.Utils
{
    public class PatchHelper
    {
        const string METADATA_PATH = "patch-metadata";
        const string FILE_NAME = "global-metadata.dat";
        SettingPage.Config _config = new SettingPage.Config();

        public bool IsAdministrator()

        {

            WindowsIdentity current = WindowsIdentity.GetCurrent();

            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);

            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);

        }

        public string GetPatchDir()
        {
            string filePath = Path.Combine(Path.GetDirectoryName(_config.Path), "YuanShen_Data", "Managed", "Metadata");
            string filePathOsrel = Path.Combine(Path.GetDirectoryName(_config.Path), "GenshinImpact_Data", "Managed", "Metadata");

            if (GetCilentType() == ClientType.Ocean)
            {
                filePath = filePathOsrel;
            }
            return filePath;
        }

        public PatchHelper()
        {
            try
            {
                _config = JsonConvert.DeserializeObject<SettingPage.Config>(SettingProvider.Get(SettingPage.Id));
            }
            catch
            {
                _config = new SettingPage.Config();
            }
        }

        public enum ClientType
        {
            China,
            Ocean,
            NotSupported
        }

        public ClientType GetCilentType()
        {
            string filePath = Path.Combine(Path.GetDirectoryName(_config.Path), "YuanShen_Data", "Managed", "Metadata");

            string filePathOcean = Path.Combine(Path.GetDirectoryName(_config.Path), "GenshinImpact_Data", "Managed", "Metadata");

            if (Directory.Exists(filePathOcean))
            {
                return ClientType.Ocean;
            }
            return Directory.Exists(filePath)
                ? ClientType.China
                : ClientType.NotSupported;

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

            try
            {
                Path.GetDirectoryName(_config.Path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("游戏路径不正确！" + ex.Message);
                return
                ;
            }
            string file_path = GetPatchDir();

            //备份
            if (!File.Exists(Path.Combine(file_path, FILE_NAME + ".bak")))
            {
                File.Copy(
                    Path.Combine(file_path, FILE_NAME),
                    Path.Combine(file_path, FILE_NAME + ".bak"));
            }

            //修补
            if (GetCilentType() == ClientType.China)
            {


                string patched_file = Path.Combine(METADATA_PATH, "cnrel-" + FILE_NAME);
                if (File.Exists(patched_file))
                {
                    File.Copy(patched_file, Path.Combine(file_path, FILE_NAME), true
                        );
                }
                else
                {
                    MessageBox.Show($"文件不存在！{patched_file}");
                    return;
                }
            }
            else if (GetCilentType() == ClientType.Ocean)
            {

                string patched_file = Path.Combine(METADATA_PATH, "osrel-" + FILE_NAME);
                if (File.Exists(patched_file))
                {
                    File.Copy(patched_file, Path.Combine(file_path, FILE_NAME), true
                        );
                }
                else
                {
                    MessageBox.Show($"文件不存在！{patched_file}");
                    return;
                }
            }
            else
            {
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
                Path.GetDirectoryName(_config.Path);

            }
            catch (Exception ex)
            {
                MessageBox.Show("游戏路径不正确！" + ex.Message);
                return
                ;
            }
            string file_path = GetPatchDir();

            if (GetCilentType() == ClientType.China)
            {

                if (File.Exists(Path.Combine(file_path, FILE_NAME + ".bak")))
                {
                    File.Copy(
                        Path.Combine(file_path, FILE_NAME + ".bak"),
                        Path.Combine(file_path, FILE_NAME), true);
                }
                else
                {
                    MessageBox.Show("未找到备份文件！");
                }
            }
            else if (GetCilentType() == ClientType.Ocean)
            {

                if (File.Exists(Path.Combine(file_path, FILE_NAME + ".bak")))
                {
                    File.Copy(
                        Path.Combine(file_path, FILE_NAME + ".bak"),
                        Path.Combine(file_path, FILE_NAME), true);
                }
                else
                {
                    MessageBox.Show("未找到备份文件！");
                }
            }
            else
            {
                MessageBox.Show($"不支持的客户端类型！");
                return;
            }

            MessageBox.Show($"成功UnPatch了客户端！");

        }

        public void OpenFolder()
        {
            if (!Directory.Exists(METADATA_PATH))
            {
                Directory.CreateDirectory(METADATA_PATH);
            }

            System.Diagnostics.Process.Start(Path.Combine(Environment.CurrentDirectory, METADATA_PATH));

        }

        public void OpenPatchFolder()
        {
            if (!Directory.Exists(METADATA_PATH))
            {
                Directory.CreateDirectory(METADATA_PATH);
            }

            System.Diagnostics.Process.Start(GetPatchDir());
        }

        public bool IsPatched()
        {
            var dir = GetPatchDir();
            var r = false;
            try
            {
                //不相同即为已 Patch
                r = !IsSameFile(Path.Combine(dir, FILE_NAME), Path.Combine(dir, FILE_NAME + ".bak"));

            }
            catch (Exception)
            {
                //Nothing to do
            }
            return r;
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
