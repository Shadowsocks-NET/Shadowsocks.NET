using OpenOnlineConfig.v1;
using System.Collections.Generic;

namespace Shadowsocks.OnlineConfig.OOCv1
{
    public class OOCConfigShadowsocks : OOCv1ConfigBase
    {
        /// <summary>
        /// Gets or sets the list of Shadowsocks servers.
        /// </summary>
        public List<OOCShadowsocksServer> Shadowsocks { get; set; } = new();

        /// <summary>
        /// Initializes an OOCv1 Shadowsocks config.
        /// </summary>
        public OOCConfigShadowsocks() => Protocols.Add("shadowsocks");
    }
}
