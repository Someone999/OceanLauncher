using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Newtonsoft.Json;
using OceanLauncher.Utils;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OceanLauncher.Config;
using OceanLauncher.Game.Patch;
using WpfWidgetDesktop.Utils;

namespace OceanLauncher.Pages
{
    /// <summary>
    /// SettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingPage : Page
    {

        public static string Id = "core.cfg";
        public class Config
        {
            public string Height = "1080";
            public string Width = "1920";
            public string Port = "1145";
            //public string Path = @"C:\Program Files\Genshin Impact\Genshin Impact Game\YuanShen.exe";
            public string Path = "";
            public string Args = "";
        }


        SettingViewModel _viewModel = new SettingViewModel
        {

        };
        public SettingPage()
        {
            InitializeComponent();
            DataContext = _viewModel;

            Config config = new Config();
            try
            {
                config = JsonConvert.DeserializeObject<Config>(Configs.LauncherConfig["Id"].GetValue<string>());
                //config = JsonConvert.DeserializeObject<Config>(SettingProvider.Get("id"));
            }
            catch { }
            finally
            {
                if (config == null)
                {
                    config = new Config();
                }
                _viewModel.Args = config.Args;
                _viewModel.Width = config.Width;
                _viewModel.Height = config.Height;
                _viewModel.Path = config.Path;
                _viewModel.Port = config.Port;
            }
        }


        public class SettingViewModel : ObservableObject
        {
            public ICommand GoHome { get; set; }

            public SettingViewModel()
            {
                GoHome = new RelayCommand(() =>
                {
                    GlobalProps.NavigateTo(new Home());
                });
            }

            private string _path;

            public string Path
            {
                get => _path;
                set => SetProperty(ref _path, value);
            }
            private string _width;

            public string Width
            {
                get => _width;
                set => SetProperty(ref _width, value);
            }

            private string _height;

            public string Height
            {
                get => _height;
                set => SetProperty(ref _height, value);
            }

            private string _args;

            public string Args
            {
                get => _args;
                set => SetProperty(ref _args, value);
            }

            private string _port;

            public string Port
            {
                get => _port;
                set => SetProperty(ref _port, value);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Config cfg = new Config();
            //cfg.Args = vm.Args;
            //cfg.Width = vm.Width;
            //cfg.Height = vm.Height;
            //cfg.Path = vm.Path;
            //cfg.Port = vm.Port;

            //SettingProvider.Set(Id, _viewModel);
            Configs.LauncherConfig[Id] = new DefaultConfigElement(_viewModel);
            Configs.LauncherConfig.Save();
            GlobalProps.Frame.Navigate(new Home());
        }

        private void SearchPath(object sender, RoutedEventArgs e)
        {
            string gamePath = "";
            try
            {
                gamePath = GameRegReader.GetGamePath();

            }
            catch (Exception)
            {
                MessageBox.Show("自动搜索失败，请手动指定 YuanShen.exe 所在位置！");

                return;
            }
            string cn = Path.Combine(gamePath, "YuanShen.exe");
            string os = Path.Combine(gamePath, "GenshinImpact.exe");
            if (File.Exists(cn))
            {
                _viewModel.Path = cn;
            }
            else if (File.Exists(os))
            {
                _viewModel.Path = os;
            }
            else
            {
                MessageBox.Show("自动搜索失败，请手动指定 YuanShen.exe 所在位置！");
                return;
            }


            Configs.LauncherConfig[Id] = new DefaultConfigElement(_viewModel);
            //SettingProvider.Set(Id, _viewModel);

        }



        private void Patch(object sender, RoutedEventArgs e)
        {

            new PatchHelper().Patch();


        }

        private void UnPatch(object sender, RoutedEventArgs e)
        {
            new PatchHelper().UnPatch();
        }

        private void OpenFolder(object sender, RoutedEventArgs e)
        {
            new PatchHelper().OpenFolder();


        }
    }
}
