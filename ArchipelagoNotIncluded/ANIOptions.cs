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
        [Option("URL", "Archipelago server location. Default: Archipelago.gg\nIf you are connecting locally: localhost.")]
        [JsonProperty]
		public string URL { get; set; }
        [Option("Port", "Port number to connect to.")]
        [JsonProperty]
        public int Port { get; set; }
        [Option("Player Name", "Also called Slot Name.")]
        [JsonProperty]
        public string SlotName { get; set; }
        [Option("Password", "Password for Multiworld. Leave blank if there isn't one.")]
        [JsonProperty]
        public string Password { get; set; }

        public ANIOptions()
        {
            URL = "Archipelago.gg";
            Port = 38281;
            SlotName = "PlayerName";
            SlotName = "";
        }
    }
}
