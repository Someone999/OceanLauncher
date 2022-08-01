using Newtonsoft.Json;
using OceanLauncher.Pages;
using System;
using System.Diagnostics;
using System.Windows;
using OceanLauncher.Config;

namespace OceanLauncher.Utils
{
    public class GameHelper
    {
        SettingPage.Config _config;
        public GameHelper()
        {
            try
            {
                _config = JsonConvert.DeserializeObject<SettingPage.Config>(Configs.LauncherConfig[SettingPage.Id].GetValue<string>());
                //_config = JsonConvert.DeserializeObject<SettingPage.Config>(SettingProvider.Get(SettingPage.Id));
            }
            catch
            {
                MessageBox.Show("无效的启动配置，请检查设置！");
            }
            finally
            {
                if (_config == null)
                {
                    _config = new SettingPage.Config();
                }
            }
        }


        public void Start()
        {
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = _config.Path,
                Arguments = GetArguments(),

            };
            try
            {
                process.Start();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "启动失败");

            }


        }


        private string GetArguments()
        {
            try
            {

                return $"-screen-height {_config.Height}" +
                       $" -screen-width {_config.Width} {_config.Args}";
            }
            catch
            {
                return "";
            }


        }
    }
}