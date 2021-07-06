using Shadowsocks.Models;
using Shadowsocks.Net;
using System.Collections.Generic;
using System.Net;

namespace Shadowsocks.CLI.Client
{
    public class Legacy
    {
        private TCPListener? _tcpListener;
        private UDPListener? _udpListener;

        public void Start(string listenSocks, string serverAddress, int serverPort, string method, string password, string? pluginPath, string? pluginOpts, string? pluginArgs)
        {
            var localEP = IPEndPoint.Parse(listenSocks);
            var server = new Server()
            {
                Host = serverAddress,
                Port = serverPort,
                Method = method,
                Password = password,
                PluginPath = pluginPath,
                PluginOpts = pluginOpts,
                PluginArgs = pluginArgs,
            };

            var tcpRelay = new TCPRelay(server);
            _tcpListener = new TCPListener(localEP, new List<IStreamService>()
                        {
                            tcpRelay,
                        });
            _tcpListener.Start();

            var udpRelay = new UDPRelay(server);
            _udpListener = new UDPListener(localEP, new List<IDatagramService>()
                        {
                            udpRelay,
                        });
            _udpListener.Start();
        }

        public void Stop()
        {
            _tcpListener?.Stop();
            _udpListener?.Stop();
        }
    }
}
