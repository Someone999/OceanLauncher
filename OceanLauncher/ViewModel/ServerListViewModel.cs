using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OceanLauncher.Config;

namespace OceanLauncher.Pages
{
    public class ServerListViewModel : ObservableObject
    {
        public ICommand GoHome { get; set; }
        public ICommand Delete { get; set; }
        public ServerListViewModel()
        {

            try
            {
                bool containsKey = Configs.LauncherConfig.ContainsKey(GlobalProps.ServerListCfgID);
                var obj = containsKey
                    ? Configs.LauncherConfig[GlobalProps.ServerListCfgID].GetValue<ObservableCollection<ServerInfo>>()
                    : new ObservableCollection<ServerInfo>();
                ServerList = obj;
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
            get => _serverList;
            set => SetProperty(ref _serverList, value);
        }

    }
}