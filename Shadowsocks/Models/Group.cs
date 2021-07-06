using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Shadowsocks.Models
{
    public class Group
    {
        /// <summary>
        /// Gets or sets the group name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the UUID of the group.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the list of servers in the group.
        /// </summary>
        public List<Server> Servers { get; set; }

        /// <summary>
        /// Gets or sets the data usage in bytes.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ulong BytesUsed { get; set; }

        /// <summary>
        /// Gets or sets the data remaining to be used in bytes.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ulong BytesRemaining { get; set; }

        public Group(string name = "")
        {
            Name = name;
            Id = Guid.NewGuid();
            BytesUsed = 0UL;
            BytesRemaining = 0UL;
            Servers = new List<Server>();
        }
    }
}
