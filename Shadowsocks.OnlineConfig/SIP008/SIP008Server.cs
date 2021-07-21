using Shadowsocks.Models;
using System;
using System.Text.Json.Serialization;

namespace Shadowsocks.OnlineConfig.SIP008
{
    public class SIP008Server : IServer
    {
        /// <inheritdoc/>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <inheritdoc/>
        [JsonPropertyName("remarks")]
        public string Name { get; set; } = "";

        /// <inheritdoc/>
        [JsonPropertyName("server")]
        public string Host { get; set; } = "";

        /// <inheritdoc/>
        [JsonPropertyName("server_port")]
        public int Port { get; set; }

        /// <inheritdoc/>
        public string Method { get; set; } = "chacha20-ietf-poly1305";

        /// <inheritdoc/>
        public string Password { get; set; } = "";

        /// <inheritdoc/>
        [JsonPropertyName("plugin")]
        public string? PluginName { get; set; }

        /// <inheritdoc/>
        public string? PluginVersion { get; set; }

        /// <inheritdoc/>
        [JsonPropertyName("plugin_opts")]
        public string? PluginOptions { get; set; }

        /// <inheritdoc/>
        [JsonPropertyName("plugin_args")]
        public string? PluginArguments { get; set; }

        public SIP008Server()
        {
        }

        public SIP008Server(IServer server)
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
