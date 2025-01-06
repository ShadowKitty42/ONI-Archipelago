using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchipelagoNotIncluded
{
    [ModInfo("https://github.com/peterhaneve/ONIMods", collapse: true)]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    [RestartRequired]
    public sealed class ANIOptions
    {
        /// <summary>
		/// Archipelago server location. Default: Archipelago.gg
        /// If you are connecting locally: localhost
		/// </summary>
        [Option("URL", "Archipelago server location. Default: Archipelago.gg\nIf you are connecting locally: localhost")]
        [JsonProperty]
		public string URL { get; set; }

        /// <summary>
        /// Port number to connect to.
        /// </summary>
        [Option("Port", "Port number to connect to")]
        [JsonProperty]
        public int Port { get; set; }

        /// <summary>
        /// Player name.
        /// </summary>
        [Option("Player Name", "Also called Slot Name")]
        [JsonProperty]
        public string SlotName { get; set; }

        public ANIOptions()
        {
            URL = "Archipelago.gg";
            Port = 38281;
            SlotName = "PlayerName";
        }
    }
}
