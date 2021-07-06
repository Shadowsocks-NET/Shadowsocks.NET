using Shadowsocks.Models;
using System.Collections.Generic;

namespace Shadowsocks.OnlineConfig.SIP008
{
    public class SIP008Config
    {
        /// <summary>
        /// Gets or sets the SIP008 document version.
        /// </summary>
        public int Version { get; set; } = 1;

        /// <inheritdoc cref="Group.BytesUsed"/>
        public ulong BytesUsed { get; set; }

        /// <inheritdoc cref="Group.BytesRemaining"/>
        public ulong BytesRemaining { get; set; }

        /// <summary>
        /// Gets or sets the list of servers.
        /// </summary>
        public List<SIP008Server> Servers { get; set; } = new();
    }
}
