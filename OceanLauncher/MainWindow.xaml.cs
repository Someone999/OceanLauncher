using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Newtonsoft.Json;
using OceanLauncher.Pages;
using OceanLauncher.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using WpfWidgetDesktop.Utils;

namespace OceanLauncher
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        MainVM vm = new MainVM();
        SettingPage.Config _config;
        CustomCFG CustomCFG;
        readonly string id = "core.home";
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = vm;


            IntPtr hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle();
            //MyWindowStyle.EnableBlur(hWnd);
            MyWindowStyle.EnableRoundWindow(hWnd);


            GlobalProps.NavigateTo = this.NavigateTo;
            GlobalProps.Frame = frame;

            GlobalProps.SetServer = SetServer;


            LoadDataAsync();

            try
            {
                _config = JsonConvert.DeserializeObject<SettingPage.Config>(SettingProvider.Get(SettingPage.Id));
            }
            catch
            {
                // Nothing to do
            }
            finally
            {
                if (_config == null)
                {
                    NavigateTo(new SettingPage());

                }
                _config = new SettingPage.Config();
            }


        }


        public void SetServer(ServerInfo si)
        {
            vm.ServerInfo = si;
            SettingProvider.SetNoSave(id, si);

            LoadDataAsync();
        }


        public async Task LoadDataAsync()
        {
            try
            {
                CustomCFG = JsonConvert.DeserializeObject<CustomCFG>(File.ReadAllText("links.json"));

            }
            catch (Exception)
            {

            }



            try
            {
                vm.ServerInfo = JsonConvert.DeserializeObject<ServerInfo>(SettingProvider.Get(id));
            }
            finally
            {
                if (vm.ServerInfo == null)
                {
                    vm.ServerInfo = new ServerInfo { IP = "localhost:25565" };

                }
            }
            vm.ServerInfo = await ServerInfoGetter.GetAsync(vm.ServerInfo);
            DataContext = null;
            this.DataContext = vm;


        }




        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public void NavigateTo(Page pg)
        {
            frame.Navigate(pg);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(new SettingPage());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //SettingProvider.Save();
            //if (GlobalProps.controller!=null)
            //{
            //    GlobalProps.controller.Stop();

            //}
            CloseAsync();
            //Close();
        }
        private async Task CloseAsync()
        {
            Storyboard closeStoryboard = (Storyboard)this.FindResource("WindowClose");
            closeStoryboard.Begin();
            await Task.Delay(200);
            Application.Current.Shutdown();
        }



        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            LoadDataAsync();

        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            Border_MouseRightButtonUp(null, null);


        }

        private void Border_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var p = new Utils.PatchHelper().IsPatched();
            MessageBoxResult vr = System.Windows.MessageBox.Show($"当前Patch状态：{p}，\n确定继续启动？", "启动前提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (vr == MessageBoxResult.OK) // 如果是确定，就执行下面代码，记得换上自己的代码喔
            {

                GameHelper helper = new GameHelper();
                helper.Start();
            }
            else
            {
                frame.Navigate(new SettingPage());

            }



        }

        #region 链接按钮

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/SwetyCore/OceanLauncher");

        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (CustomCFG != null)
            {
                Process.Start(CustomCFG.logoUrl);
                return;
            }
            Process.Start("https://github.com/SwetyCore/OceanLauncher");

        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            if (CustomCFG != null)
            {
                Process.Start(CustomCFG.qqUrl);
                return;
            }
            Process.Start("https://github.com/SwetyCore/OceanLauncher");

        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            if (CustomCFG != null)
            {
                Process.Start(CustomCFG.githubUrl);
                return;
            }
            Process.Start("https://github.com/SwetyCore/OceanLauncher");

        }

        #endregion

        private void ProxyChecked(object sender, RoutedEventArgs e)
        {
            if (GlobalProps.Controller == null)
            {
                GlobalProps.Controller = new ProxyController(_config.Port, vm.ServerInfo.IP);


                GlobalProps.Controller.Start();


                //GameHelper helper = new GameHelper();
                //helper.Start();


            }
        }

        private void ProxyUnChecked(object sender, RoutedEventArgs e)
        {
            if (GlobalProps.Controller != null)
            {
                GlobalProps.Controller.Stop();
                GlobalProps.Controller = null;
            }

        }
    }

    public class MainVM : ObservableObject
    {
        public ICommand OpenServerList { get; set; }

        public MainVM()
        {
            OpenServerList = new RelayCommand(() =>
              {
                  GlobalProps.NavigateTo(new ServerList());
              });
        }


        private ServerInfo _info;

        public ServerInfo ServerInfo
        {
            get { return _info; }
            set { SetProperty(ref _info, value); }
        }

    }


    public class CustomCFG
    {
        public string logoUrl { get; set; }
        public string qqUrl { get; set; }
        public string githubUrl { get; set; }
    }


    public class LauncherCFG
    {


    }

}
