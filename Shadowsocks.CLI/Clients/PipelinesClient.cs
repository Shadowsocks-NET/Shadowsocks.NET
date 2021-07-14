using Shadowsocks.Protocol;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Shadowsocks.CLI.Clients
{
    public class PipelinesClient
    {
        private TcpPipeListener? _tcpPipeListener;

        public Task Start(IPEndPoint listenSocks, string serverAddress, int serverPort, string method, string? password, string? key, string? plugin, string? pluginOpts, string? pluginArgs)
        {
            // TODO
            var remoteEp = new DnsEndPoint(serverAddress, serverPort);
            byte[]? mainKey = null;
            if (!string.IsNullOrEmpty(key))
                mainKey = Encoding.UTF8.GetBytes(key);
            _tcpPipeListener = new(listenSocks);
            return _tcpPipeListener.Start(listenSocks, remoteEp, method, password, mainKey);
        }

        public void Stop() => _tcpPipeListener?.Stop();
    }
}
