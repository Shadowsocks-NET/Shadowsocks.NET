using Shadowsocks.Models;
using System;
using System.Text.Json.Serialization;

namespace Shadowsocks.OnlineConfig.OOCv1
{
    public class OOCShadowsocksServer : IServer
    {
        /// <inheritdoc/>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <inheritdoc/>
        public string Name { get; set; } = "";

        /// <inheritdoc/>
        [JsonPropertyName("address")]
        public string Host { get; set; } = "";

        /// <inheritdoc/>
        public int Port { get; set; }

        /// <inheritdoc/>
        public string Method { get; set; } = "chacha20-ietf-poly1305";

        /// <inheritdoc/>
        public string Password { get; set; } = "";

        /// <inheritdoc/>
        public string? PluginName { get; set; }

        /// <inheritdoc/>
        public string? PluginVersion { get; set; }

        /// <inheritdoc/>
        public string? PluginOptions { get; set; }

        /// <inheritdoc/>
        public string? PluginArguments { get; set; }

        public OOCShadowsocksServer()
        {
        }

        public OOCShadowsocksServer(IServer server)
        {
            Id = server.Id;
            Name = server.Name;
            Host = server.Host;
            Port = server.Port;
            Method = server.Method;
            Password = server.Password;
            PluginName = server.PluginName;
            PluginVersion = server.PluginVersion;
            PluginOptions = server.PluginOptions;
            PluginArguments = server.PluginArguments;
        }

        public IServer ToIServer() => this;
    }
}
