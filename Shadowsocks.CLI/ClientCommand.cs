using Shadowsocks.CLI.Utils;
using Splat;
using System.CommandLine.Parsing;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Shadowsocks.CLI
{
    public static class ClientCommand
    {
        public static void ValidateClientCommand(CommandResult commandResult)
        {
            var hasListen = commandResult.ContainsSymbolWithName("listen");
            var hasListenSocks = commandResult.ContainsSymbolWithName("listen-socks");
            var hasListenHttp = commandResult.ContainsSymbolWithName("listen-http");

            if (!hasListen && !hasListenSocks && !hasListenHttp)
            {
                commandResult.ErrorMessage = "Please enable at least one local listener. Choose between `--listen`, `--listen-socks`, and `--listen-http`.";
                return;
            }

            var hasPassword = commandResult.ContainsSymbolWithName("password");
            var hasKey = commandResult.ContainsSymbolWithName("key");

            if ((!hasPassword && !hasKey) || (hasPassword && hasKey))
            {
                commandResult.ErrorMessage = "Please specify either a password or an encryption key.";
                return;
            }

            var hasPlugin = commandResult.ContainsSymbolWithName("plugin-path");
            var hasPluginOpts = commandResult.ContainsSymbolWithName("plugin-opts");
            var hasPluginArgs = commandResult.ContainsSymbolWithName("plugin-args");

            if (!hasPlugin && (hasPluginOpts || hasPluginArgs))
            {
                commandResult.ErrorMessage = "Missing `--plugin-path`.";
            }
        }

        public static async Task<int> RunClientAsync(Backend backend, IPEndPoint? listen, IPEndPoint? listenSocks, IPEndPoint? listenHttp, string serverAddress, int serverPort, string method, string password, string key, string? pluginPath, string? pluginOpts, string? pluginArgs, CancellationToken cancellationToken)
        {
            if (listenSocks is null)
            {
                LogHost.Default.Error("For now, you must specify SOCKS5 listen address and port.");
                return -1;
            }

            Clients.LegacyClient? legacyClient = null;
            Clients.PipelinesClient? pipelinesClient = null;

            switch (backend)
            {
                case Backend.SsRust:
                    LogHost.Default.Error("Not implemented.");
                    return -1;
                case Backend.V2Ray:
                    LogHost.Default.Error("Not implemented.");
                    return -1;
                case Backend.Legacy:
                    if (!string.IsNullOrEmpty(password))
                    {
                        legacyClient = new();
                        legacyClient.Start(listenSocks, serverAddress, serverPort, method, password, pluginPath, pluginOpts, pluginArgs);
                    }
                    else
                    {
                        LogHost.Default.Error("The legacy backend requires password.");
                        return -1;
                    }
                    break;
                case Backend.Pipelines:
                    pipelinesClient = new();
                    await pipelinesClient.Start(listenSocks, serverAddress, serverPort, method, password, key, pluginPath, pluginOpts, pluginArgs);
                    break;
                default:
                    LogHost.Default.Error("Not implemented.");
                    return -1;
            }

            cancellationToken.WaitHandle.WaitOne();

            switch (backend)
            {
                case Backend.SsRust:
                    LogHost.Default.Error("Not implemented.");
                    break;
                case Backend.V2Ray:
                    LogHost.Default.Error("Not implemented.");
                    break;
                case Backend.Legacy:
                    legacyClient?.Stop();
                    break;
                case Backend.Pipelines:
                    pipelinesClient?.Stop();
                    break;
                default:
                    LogHost.Default.Error("Not implemented.");
                    break;
            }

            return 0;
        }
    }
}
