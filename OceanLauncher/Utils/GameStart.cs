using Newtonsoft.Json;
using OceanLauncher.Pages;
using System;
using System.Diagnostics;
using System.Windows;
using WpfWidgetDesktop.Utils;

namespace OceanLauncher.Utils
{
    public class GameHelper
    {
        SettingPage.Config _config;
        public GameHelper()
        {
            try
            {
                _config = JsonConvert.DeserializeObject<SettingPage.Config>(SettingProvider.Get(SettingPage.Id));

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
            Process progress = new Process();
            progress.StartInfo = new ProcessStartInfo
            {
                FileName = _config.Path,
                Arguments = GetArguments(),

            };
            try
            {
                progress.Start();

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
