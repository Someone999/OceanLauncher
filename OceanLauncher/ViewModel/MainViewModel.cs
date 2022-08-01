using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OceanLauncher.Pages;

namespace OceanLauncher
{
    public class MainViewModel : ObservableObject
    {
        public ICommand OpenServerList { get; set; }

        public MainViewModel()
        {
            OpenServerList = new RelayCommand(() =>
            {
                GlobalProps.NavigateTo(new ServerList());
            });
        }


        private ServerInfo _info;

        public ServerInfo ServerInfo
        {
            get => _info;
            set => SetProperty(ref _info, value);
        }

    }
}