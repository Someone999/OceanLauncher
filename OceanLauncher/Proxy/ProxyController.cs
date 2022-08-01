using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace OceanLauncher.Utils
{
    //public static class Global
    //{
    //    public static ProxyController controller { get; set; }
    //}

    public class ProxyController
    {

        public static List<string> Domains { get; } = new List<string>()
        {
            ".yuanshen.com",
            ".hoyoverse.com",
            ".mihoyo.com"
        };
            
        ProxyServer _proxyServer;
        ExplicitProxyEndPoint _explicitEndPoint;
        private string _port;
        private string _fakeHost;

        public ProxyController(string port, string host)
        {
            this._port = port;
            this._fakeHost = host;


        }

        private bool _isRun;

        public bool IsRun
        {
            get => _proxyServer.ProxyRunning;
            set => _isRun = value;
        }


        public void Start()
        {
            _proxyServer = new ProxyServer();
            _proxyServer.CertificateManager.EnsureRootCertificate();


            _proxyServer.BeforeRequest += OnRequest;
            _proxyServer.ServerCertificateValidationCallback += OnCertificateValidation;
            if (String.IsNullOrEmpty(_port))
            {
                _port = 11451.ToString(); ;
            }

            _explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, int.Parse(_port), true)
            {
            };

            // Fired when a CONNECT request is received
            _explicitEndPoint.BeforeTunnelConnectRequest += OnBeforeTunnelConnectRequest;

            // An explicit endpoint is where the client knows about the existence of a proxy
            // So client sends request in a proxy friendly manner
            _proxyServer.AddEndPoint(_explicitEndPoint);
            _proxyServer.Start();


            foreach (var endPoint in _proxyServer.ProxyEndPoints)
                Console.WriteLine("Listening on '{0}' endpoint at Ip {1} and port: {2} ",
                    endPoint.GetType().Name, endPoint.IpAddress, endPoint.Port);

            // Only explicit proxies can be set as system proxy!
            _proxyServer.SetAsSystemHttpProxy(_explicitEndPoint);
            _proxyServer.SetAsSystemHttpsProxy(_explicitEndPoint);

        }



        public void Stop()
        {
            try
            {
                _explicitEndPoint.BeforeTunnelConnectRequest -= OnBeforeTunnelConnectRequest;
                _proxyServer.BeforeRequest -= OnRequest;
                //proxyServer.BeforeResponse -= OnResponse;
                _proxyServer.ServerCertificateValidationCallback -= OnCertificateValidation;

                // proxyServer.ClientCertificateSelectionCallback -= OnCertificateSelection;

            }
            catch (Exception)
            {
                //Nothing to do
            }
            finally
            {
                if (_proxyServer.ProxyRunning)
                {
                    _proxyServer.Stop();

                }
                else
                {
                }

            }

        }

        public void UninstallCertificate()
        {

            _proxyServer.CertificateManager.RemoveTrustedRootCertificate();
            _proxyServer.CertificateManager.RemoveTrustedRootCertificateAsAdmin();


        }



        private async Task OnBeforeTunnelConnectRequest(object sender, TunnelConnectSessionEventArgs e)
        {
            string hostname = e.WebSession.Request.RequestUri.Host;
            if (Domains.Any(d => hostname.EndsWith(d)) || hostname.EndsWith(_fakeHost))
            {
                e.DecryptSsl = true;
            }
            else
            {
                e.DecryptSsl = false;
            }
        }


        private async Task OnRequest(object sender, SessionEventArgs e)
        {
            //Uri uri1 = new Uri(GameNitify.Url); Uri uri2 = new Uri(GameNitify.MoreUrl);
            //Uri uri3 = new Uri("https://uploadstatic.mihoyo.com");
            //if (hostname == uri1.Host || hostname == uri2.Host || hostname == uri3.Host)
            //    return;

            string hostname = e.WebSession.Request.RequestUri.Host;
            if (Domains.Any(d => hostname.EndsWith(d)))
            {
                string oHost = e.WebSession.Request.RequestUri.Host;
                e.HttpClient.Request.Url = e.HttpClient.Request.Url.Replace(oHost, _fakeHost);
            }
        }

        // Allows overriding default certificate validation logic
        private Task OnCertificateValidation(object sender, CertificateValidationEventArgs e)
        {
            // set IsValid to true/false based on Certificate Errors
            //if (e.SslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            //    e.IsValid = true;
            e.IsValid = true;
            return Task.CompletedTask;
        }

    }
}