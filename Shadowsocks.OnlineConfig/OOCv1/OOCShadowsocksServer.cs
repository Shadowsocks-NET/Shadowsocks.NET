using Shadowsocks.Models;
using System;
using System.Collections.Generic;
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
        public string? PluginPath { get; set; }

        /// <inheritdoc/>
        public string? PluginOpts { get; set; }

        /// <inheritdoc/>
        public string? PluginArgs { get; set; }

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
            PluginPath = server.PluginPath;
            PluginOpts = server.PluginOpts;
            PluginArgs = server.PluginArgs;
        }

        public IServer ToIServer() => this;
    }
}
