using Newtonsoft.Json;
using OceanLauncher.Utils;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using OceanLauncher.Config;

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

}
