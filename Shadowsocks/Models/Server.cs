using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Shadowsocks.Models
{
    public class Server : IEquatable<Server>, IServer
    {
        /// <inheritdoc/>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <inheritdoc/>
        public string Name { get; set; } = "";

        /// <inheritdoc/>
        public string Host { get; set; } = "";

        /// <inheritdoc/>
        public int Port { get; set; }

        /// <inheritdoc/>
        public string Method { get; set; } = "chacha20-ietf-poly1305";

        /// <inheritdoc/>
        public string Password { get; set; } = "";

        /// <inheritdoc/>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? PluginName { get; set; }

        /// <inheritdoc/>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? PluginVersion { get; set; }

        /// <summary>
        /// Gets or sets the plugin executable path.
        /// Null when not using a plugin.
        /// </summary>
        [Obsolete("Use PluginName and PluginVersion to resolve plugin instead.")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? PluginPath { get; set; }

        /// <inheritdoc/>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? PluginOptions { get; set; }

        /// <inheritdoc/>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? PluginArguments { get; set; }

        public Server()
        {
        }

        public Server(IServer server)
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

        public bool Equals(Server? other) => Id == other?.Id;
        public override bool Equals(object? obj) => Equals(obj as Server);
        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString() => Name;

        public IServer ToIServer() => this;

        /// <summary>
        /// Tries to parse an ss:// URL into a Server object.
        /// </summary>
        /// <param name="url">The ss:// URL to parse.</param>
        /// <param name="server">
        /// A Server object represented by the URL.
        /// A new empty Server object if the URL is invalid.
        /// </param>
        /// <returns>True for success. False for failure.</returns>
        public static bool TryParse(string url, [NotNullWhen(true)] out Server? server)
        {
            server = null;
            return Uri.TryCreate(url, UriKind.Absolute, out var uri) && TryParse(uri, out server);
        }

        /// <summary>
        /// Tries to parse an ss:// URL into a Server object.
        /// </summary>
        /// <param name="uri">The ss:// URL to parse.</param>
        /// <param name="server">
        /// A Server object represented by the URL.
        /// A new empty Server object if the URL is invalid.
        /// </param>
        /// <returns>True for success. False for failure.</returns>
        public static bool TryParse(Uri uri, [NotNullWhen(true)] out Server? server)
        {
            server = null;
            try
            {
                if (uri.Scheme != "ss")
                    return false;

                var userinfo_base64url = uri.UserInfo;
                var userinfo = Utils.Base64Url.DecodeToString(userinfo_base64url);
                var userinfoSplitArray = userinfo.Split(':', 2);
                var method = userinfoSplitArray[0];
                var password = userinfoSplitArray[1];
                var host = uri.HostNameType == UriHostNameType.IPv6 ? uri.Host[1..^1] : uri.Host;
                var escapedFragment = string.IsNullOrEmpty(uri.Fragment) ? uri.Fragment : uri.Fragment[1..];
                var name = Uri.UnescapeDataString(escapedFragment);

                server = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = name,
                    Host = host,
                    Port = uri.Port,
                    Method = method,
                    Password = password,
                };

                // find the plugin query
                var parsedQueriesArray = uri.Query.Split('?', '&');

                string? pluginQueryContent = null;
                string? pluginVersion = null;
                string? pluginArguments = null;

                foreach (var query in parsedQueriesArray)
                {
                    if (query.StartsWith("plugin=") && query.Length > 7)
                    {
                        pluginQueryContent = Uri.UnescapeDataString(query[7..]); // remove "plugin=" and unescape
                    }

                    if (query.StartsWith("pluginVersion=") && query.Length > 14)
                    {
                        pluginVersion = Uri.UnescapeDataString(query[14..]);
                    }

                    if (query.StartsWith("pluginArguments=") && query.Length > 16)
                    {
                        pluginArguments = Uri.UnescapeDataString(query[16..]);
                    }
                }

                if (string.IsNullOrEmpty(pluginQueryContent)) // no plugin
                    return true;

                var parsedPluginQueryArray = pluginQueryContent.Split(';', 2);

                switch (parsedPluginQueryArray.Length)
                {
                    case 1:
                        server.PluginName = parsedPluginQueryArray[0];
                        break;
                    case 2:
                        server.PluginName = parsedPluginQueryArray[0];
                        server.PluginOptions = parsedPluginQueryArray[1];
                        break;
                }

                server.PluginVersion = pluginVersion;
                server.PluginArguments = pluginArguments;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
