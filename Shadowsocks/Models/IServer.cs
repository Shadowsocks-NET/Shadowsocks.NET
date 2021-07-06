using System;
using System.Text.Json.Serialization;

namespace Shadowsocks.Models
{
    public interface IServer
    {
        /// <summary>
        /// Gets or sets the server ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the server name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the server address.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the server port.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the method used for the server.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the password for the server.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the plugin executable path.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? PluginPath { get; set; }

        /// <summary>
        /// Gets or sets the plugin options passed as environment variable SS_PLUGIN_OPTIONS.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? PluginOpts { get; set; }

        /// <summary>
        /// Gets or sets the plugin startup arguments.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? PluginArgs { get; set; }

        public IServer ToIServer();

        /// <summary>
        /// Converts this server object into an ss:// URL.
        /// </summary>
        /// <returns></returns>
        public Uri ToUrl()
        {
            UriBuilder uriBuilder = new("ss", Host, Port)
            {
                UserName = Utils.Base64Url.Encode($"{Method}:{Password}"),
                Fragment = Name,
            };

            if (!string.IsNullOrEmpty(PluginPath))
            {
                if (!string.IsNullOrEmpty(PluginOpts))
                {
                    uriBuilder.Query = $"plugin={Uri.EscapeDataString($"{PluginPath};{PluginOpts}")}"; // manually escape as a workaround
                }
                else
                {
                    uriBuilder.Query = $"plugin={PluginPath}";
                }
            }

            return uriBuilder.Uri;
        }
    }
}
