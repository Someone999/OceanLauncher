using System.Windows.Controls;
using OceanLauncher.Utils;

namespace OceanLauncher
{
    public static class GlobalProps
    {
        public delegate void navigateTo(Page pg);
        public static navigateTo NavigateTo;


        public delegate void addServer(ServerInfo si);
        public static addServer AddServer;

        public delegate void setServer(ServerInfo si);
        public static setServer SetServer;


        public static Frame Frame;

        public readonly static string ServerListCfgID = "core.serverlist";

        public static ProxyController Controller;


    }
}
