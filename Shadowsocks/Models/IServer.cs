using System;
using System.Text;
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
        /// Gets or sets the plugin name.
        /// Null when not using a plugin.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? PluginName { get; set; }

        /// <summary>
        /// Gets or sets the required plugin version string.
        /// Null when not using a plugin.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? PluginVersion { get; set; }

        /// <summary>
        /// Gets or sets the plugin options passed as environment variable SS_PLUGIN_OPTIONS.
        /// Null when not using a plugin.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? PluginOptions { get; set; }

        /// <summary>
        /// Gets or sets the plugin startup arguments.
        /// Null when not using a plugin.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? PluginArguments { get; set; }

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

            if (!string.IsNullOrEmpty(PluginName))
            {
                var querySB = new StringBuilder("plugin=");

                querySB.Append(Uri.EscapeDataString(PluginName));

                if (!string.IsNullOrEmpty(PluginOptions))
                {
                    querySB.Append("%3B"); // URI-escaped ';'
                    querySB.Append(Uri.EscapeDataString(PluginOptions));
                }

                if (!string.IsNullOrEmpty(PluginVersion))
                {
                    querySB.Append("&pluginVersion=");
                    querySB.Append(Uri.EscapeDataString(PluginVersion));
                }

                if (!string.IsNullOrEmpty(PluginArguments))
                {
                    querySB.Append("&pluginArguments=");
                    querySB.Append(Uri.EscapeDataString(PluginArguments));
                }

                uriBuilder.Query = querySB.ToString();
            }

            return uriBuilder.Uri;
        }
    }
}
