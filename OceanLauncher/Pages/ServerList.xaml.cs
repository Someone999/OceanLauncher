using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Newtonsoft.Json;
using OceanLauncher.Utils;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OceanLauncher.Config;
using WpfWidgetDesktop.Utils;

namespace OceanLauncher.Pages
{
    /// <summary>
    /// ServerList.xaml 的交互逻辑
    /// </summary>
    public partial class ServerList : Page
    {
        ServerListViewModel _viewModel = new ServerListViewModel();
        public ServerList()
        {
            InitializeComponent();
            DataContext = _viewModel;



            GlobalProps.AddServer = AddServer;

        }

        public async Task CheckNetAsync()
        {
            for (int i = 0; i < _viewModel.ServerList.Count; i++)
            {
                try
                {
                    _viewModel.ServerList[i] = await ServerInfoGetter.GetAsync(_viewModel.ServerList[i]);

                }
                catch
                {
                }




            }
            DataContext = null;
            DataContext = _viewModel;
        }




        public void AddServer(ServerInfo si)
        {
            _viewModel.ServerList.Add(si);
            //SettingProvider.SetNoSave(GlobalProps.ServerListCfgID, _viewModel.ServerList);
            Configs.LauncherConfig[GlobalProps.ServerListCfgID] = new DefaultConfigElement(_viewModel.ServerList);
            Configs.LauncherConfig.Save();

        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            GlobalProps.NavigateTo(new ServerAddDialog());
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var selected = lv.SelectedItem as ServerInfo;
            _viewModel.ServerList.Remove(selected);

            //SettingProvider.Set(GlobalProps.ServerListCfgID, _viewModel.ServerList);
            Configs.LauncherConfig[GlobalProps.ServerListCfgID] = new DefaultConfigElement(_viewModel.ServerList);
            Configs.LauncherConfig.Save();
        }

        private void root_Loaded(object sender, RoutedEventArgs e)
        {
            CheckNetAsync();
        }
    }

    public class ServerListViewModel : ObservableObject
    {
        public ICommand GoHome { get; set; }
        public ICommand Delete { get; set; }
        public ServerListViewModel()
        {

            try
            {
                bool containsKey = Configs.LauncherConfig.ContainsKey(GlobalProps.ServerListCfgID);
                string cfg = "";
                if (containsKey)
                {
                    cfg = Configs.LauncherConfig[GlobalProps.ServerListCfgID].GetValue().ToString();
                }
                
                ServerList = JsonConvert.DeserializeObject<ObservableCollection<ServerInfo>>(cfg);
            }
            finally
            {
                if (ServerList == null)
                {
                    ServerList = new ObservableCollection<ServerInfo>();
                }
            }

            GoHome = new RelayCommand(() =>
            {
                GlobalProps.NavigateTo(new Home());
            });

            Delete = new RelayCommand<object>((o) =>
            {
                GlobalProps.SetServer(o as ServerInfo);
                GlobalProps.NavigateTo(new Home());
            });

        }



        private ObservableCollection<ServerInfo> _serverList;

        public ObservableCollection<ServerInfo> ServerList
        {
            get { return _serverList; }
            set
            {
                SetProperty(ref _serverList, value);
            }
        }

    }
}
