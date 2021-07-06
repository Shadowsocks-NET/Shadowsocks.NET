using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Shadowsocks.Models
{
    public class Server : IEquatable<Server>, IServer
    {
        /// <summary>
        /// Gets or sets the server ID.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the server name.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Gets or sets the server address.
        /// </summary>
        public string Host { get; set; } = "";

        /// <summary>
        /// Gets or sets the server port.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the method used for the server.
        /// </summary>
        public string Method { get; set; } = "chacha20-ietf-poly1305";

        /// <summary>
        /// Gets or sets the password for the server.
        /// </summary>
        public string Password { get; set; } = "";

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
            PluginPath = server.PluginPath;
            PluginOpts = server.PluginOpts;
            PluginArgs = server.PluginArgs;
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
                var pluginQueryContent = "";

                foreach (var query in parsedQueriesArray)
                {
                    if (query.StartsWith("plugin=") && query.Length > 7)
                    {
                        pluginQueryContent = query[7..]; // remove "plugin="
                    }
                }

                if (string.IsNullOrEmpty(pluginQueryContent)) // no plugin
                    return true;

                var unescapedpluginQuery = Uri.UnescapeDataString(pluginQueryContent);
                var parsedPluginQueryArray = unescapedpluginQuery.Split(';', 2);

                if (parsedPluginQueryArray.Length == 1)
                {
                    server.PluginPath = parsedPluginQueryArray[0];
                }
                else if (parsedPluginQueryArray.Length == 2) // is valid plugin query
                {
                    server.PluginPath = parsedPluginQueryArray[0];
                    server.PluginOpts = parsedPluginQueryArray[1];
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
