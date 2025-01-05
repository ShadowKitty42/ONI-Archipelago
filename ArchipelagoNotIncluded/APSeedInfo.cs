using System;
using System.Collections.Generic;

namespace ArchipelagoNotIncluded
{
    public class APSeedInfo
    {
        public string AP_seed { get; set; }
        public string AP_slotName { get; set; }
        public string URL { get; set; }
        public int port { get; set; }
        public bool spaced_out { get; set; }
        public bool frosty { get; set; }

        public Dictionary<string, List<string>> technologies { get; set; }
    }

}