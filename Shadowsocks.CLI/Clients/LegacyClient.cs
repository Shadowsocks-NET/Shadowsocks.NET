using Shadowsocks.Models;
using Shadowsocks.Net;
using System.Collections.Generic;
using System.Net;

namespace Shadowsocks.CLI.Clients
{
    public class LegacyClient
    {
        private TCPListener? _tcpListener;
        private UDPListener? _udpListener;

        public void Start(IPEndPoint listenSocks, string serverAddress, int serverPort, string method, string password, string? pluginPath, string? pluginOpts, string? pluginArgs)
        {
            var server = new Server()
            {
                Host = serverAddress,
                Port = serverPort,
                Method = method,
                Password = password,
                PluginPath = pluginPath,
                PluginOptions = pluginOpts,
                PluginArguments = pluginArgs,
            };

            var tcpRelay = new TCPRelay(server);
            _tcpListener = new TCPListener(listenSocks, new List<IStreamService>()
                        {
                            tcpRelay,
                        });
            _tcpListener.Start();

            var udpRelay = new UDPRelay(server);
            _udpListener = new UDPListener(listenSocks, new List<IDatagramService>()
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
