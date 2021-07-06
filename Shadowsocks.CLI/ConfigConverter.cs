using OpenOnlineConfig.v1;
using Shadowsocks.Interop.Utils;
using Shadowsocks.Models;
using Shadowsocks.OnlineConfig.OOCv1;
using Shadowsocks.OnlineConfig.SIP008;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Shadowsocks.CLI
{
    public class ConfigConverter
    {
        /// <summary>
        /// Gets or sets whether to prefix group name to server names.
        /// </summary>
        public bool PrefixGroupName { get; set; }

        /// <summary>
        /// Gets or sets the list of servers that are not in any groups.
        /// </summary>
        public List<IServer> Servers { get; set; } = new();

        public ConfigConverter(bool prefixGroupName = false) => PrefixGroupName = prefixGroupName;

        /// <summary>
        /// Collects servers from ss:// links or SIP008 delivery links.
        /// </summary>
        /// <param name="uris">URLs to collect servers from.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task FromUrls(IEnumerable<Uri> uris, CancellationToken cancellationToken = default)
        {
            var sip008Links = new List<Uri>();

            foreach (var uri in uris)
            {
                switch (uri.Scheme)
                {
                    case "ss":
                        {
                            if (Server.TryParse(uri, out var server))
                                Servers.Add(server);
                            else
                                Console.WriteLine($"Warning: Skipping invalid ss:// link {uri.AbsoluteUri}");

                            break;
                        }

                    case "https":
                        sip008Links.Add(uri);
                        break;
                }
            }

            if (sip008Links.Count > 0)
            {
                var httpClient = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(30.0)
                };
                var tasks = sip008Links.Select(x => httpClient.GetFromJsonAsync<SIP008Config>(x, JsonHelper.snakeCaseJsonDeserializerOptions, cancellationToken))
                                       .ToList();

                while (tasks.Count > 0)
                {
                    var finishedTask = await Task.WhenAny(tasks);
                    var group = await finishedTask;
                    if (group != null)
                        Servers.AddRange(group.Servers);
                    tasks.Remove(finishedTask);
                }
            }
        }

        public async Task FromOOCv1ApiTokens(IEnumerable<string> tokens, CancellationToken cancellationToken = default)
        {
            var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30.0)
            };

            var tasks = new List<Task<OOCConfigShadowsocks?>>();

            foreach (var tokenString in tokens)
            {
                var token = JsonSerializer.Deserialize<OOCv1ApiToken>(tokenString, JsonHelper.camelCaseJsonDeserializerOptions);
                if (token is null)
                {
                    Console.WriteLine($"Warning: Skipping invalid API token {tokenString}");
                    continue;
                }

                tasks.Add(httpClient.GetFromJsonAsync<OOCConfigShadowsocks>($"{token.BaseUrl}/{token.Secret}/ooc/v1/{token.UserId}", cancellationToken));
            }

            while (tasks.Count > 0)
            {
                var finishedTask = await Task.WhenAny(tasks);
                var config = await finishedTask;
                if (config is not null)
                    Servers.AddRange(config.Shadowsocks);
                tasks.Remove(finishedTask);
            }
        }

        /// <summary>
        /// Collects servers from OOCv1 JSON files.
        /// </summary>
        /// <param name="paths">JSON file paths.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        public async Task FromOOCv1Json(IEnumerable<string> paths, CancellationToken cancellationToken = default)
        {
            foreach (var path in paths)
            {
                using var jsonFile = new FileStream(path, FileMode.Open);
                var config = await JsonSerializer.DeserializeAsync<OOCConfigShadowsocks>(jsonFile, JsonHelper.camelCaseJsonDeserializerOptions, cancellationToken);
                if (config != null)
                {
                    Servers.AddRange(config.Shadowsocks);
                }
            }
        }

        /// <summary>
        /// Collects servers from SIP008 JSON files.
        /// </summary>
        /// <param name="paths">JSON file paths.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        public async Task FromSip008Json(IEnumerable<string> paths, CancellationToken cancellationToken = default)
        {
            foreach (var path in paths)
            {
                using var jsonFile = new FileStream(path, FileMode.Open);
                var config = await JsonSerializer.DeserializeAsync<SIP008Config>(jsonFile, JsonHelper.snakeCaseJsonDeserializerOptions, cancellationToken);
                if (config != null)
                {
                    Servers.AddRange(config.Servers);
                }
            }
        }

        /// <summary>
        /// Collects servers from outbounds in V2Ray JSON files.
        /// </summary>
        /// <param name="paths">JSON file paths.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        public async Task FromV2rayJson(IEnumerable<string> paths, CancellationToken cancellationToken = default)
        {
            foreach (var path in paths)
            {
                using var jsonFile = new FileStream(path, FileMode.Open);
                var v2rayConfig = await JsonSerializer.DeserializeAsync<Interop.V2Ray.Config>(jsonFile, JsonHelper.camelCaseJsonDeserializerOptions, cancellationToken);
                if (v2rayConfig?.Outbounds != null)
                {
                    foreach (var outbound in v2rayConfig.Outbounds)
                    {
                        if (outbound.Protocol == "shadowsocks" && outbound.Settings is JsonElement jsonElement)
                        {
                            var jsonText = jsonElement.GetRawText();
                            var ssConfig = JsonSerializer.Deserialize<Interop.V2Ray.Protocols.Shadowsocks.OutboundConfigurationObject>(jsonText, JsonHelper.camelCaseJsonDeserializerOptions);
                            if (ssConfig != null)
                            {
                                foreach (var ssServer in ssConfig.Servers)
                                {
                                    var server = new Server
                                    {
                                        Name = outbound.Tag,
                                        Host = ssServer.Address,
                                        Port = ssServer.Port,
                                        Method = ssServer.Method,
                                        Password = ssServer.Password
                                    };

                                    Servers.Add(server);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Converts saved servers to ss:// URLs.
        /// </summary>
        /// <returns>A list of ss:// URLs.</returns>
        public List<Uri> ToUrls()
        {
            var urls = new List<Uri>();

            foreach (var server in Servers)
                urls.Add(server.ToUrl());

            return urls;
        }

        /// <summary>
        /// Converts saved servers to OOCv1 JSON.
        /// </summary>
        /// <param name="path">JSON file path.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the write operation.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public Task ToOOCv1Json(string path, CancellationToken cancellationToken = default)
        {
            var config = new OOCConfigShadowsocks();
            foreach (var server in Servers)
                config.Shadowsocks.Add(new(server));

            var fullPath = Path.GetFullPath(path);
            var directoryPath = Path.GetDirectoryName(fullPath) ?? throw new ArgumentException("Invalid path", nameof(path));
            Directory.CreateDirectory(directoryPath);

            using var jsonFile = new FileStream(fullPath, FileMode.Create);
            return JsonSerializer.SerializeAsync(jsonFile, config, JsonHelper.camelCaseJsonSerializerOptions, cancellationToken);
        }

        /// <summary>
        /// Converts saved servers to SIP008 JSON.
        /// </summary>
        /// <param name="path">JSON file path.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the write operation.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public Task ToSip008Json(string path, CancellationToken cancellationToken = default)
        {
            var config = new SIP008Config();
            foreach (var server in Servers)
                config.Servers.Add(new(server));

            var fullPath = Path.GetFullPath(path);
            var directoryPath = Path.GetDirectoryName(fullPath) ?? throw new ArgumentException("Invalid path", nameof(path));
            Directory.CreateDirectory(directoryPath);

            using var jsonFile = new FileStream(fullPath, FileMode.Create);
            return JsonSerializer.SerializeAsync(jsonFile, config, JsonHelper.snakeCaseJsonSerializerOptions, cancellationToken);
        }

        /// <summary>
        /// Converts saved servers to V2Ray outbounds.
        /// </summary>
        /// <param name="path">JSON file path.</param>
        /// <param name="prefixGroupName">Whether to prefix group name to server names.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the write operation.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public Task ToV2rayJson(string path, CancellationToken cancellationToken = default)
        {
            var v2rayConfig = new Interop.V2Ray.Config
            {
                Outbounds = new()
            };

            foreach (var server in Servers)
            {
                var ssOutbound = Interop.V2Ray.OutboundObject.GetShadowsocks(server);
                v2rayConfig.Outbounds.Add(ssOutbound);
            }

            // enforce outbound tag uniqueness
            var serversWithDuplicateTags = v2rayConfig.Outbounds.GroupBy(x => x.Tag)
                                                                .Where(x => x.Count() > 1);
            foreach (var serversWithSameTag in serversWithDuplicateTags)
            {
                var duplicates = serversWithSameTag.ToList();
                for (var i = 0; i < duplicates.Count; i++)
                {
                    duplicates[i].Tag = $"{duplicates[i].Tag} {i}";
                }
            }

            var fullPath = Path.GetFullPath(path);
            var directoryPath = Path.GetDirectoryName(fullPath) ?? throw new ArgumentException("Invalid path", nameof(path));
            Directory.CreateDirectory(directoryPath);
            using var jsonFile = new FileStream(fullPath, FileMode.Create);
            return JsonSerializer.SerializeAsync(jsonFile, v2rayConfig, JsonHelper.camelCaseJsonSerializerOptions, cancellationToken);
        }
    }
}
