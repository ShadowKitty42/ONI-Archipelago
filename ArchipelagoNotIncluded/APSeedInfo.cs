using System;
using System.Collections.Generic;

namespace ArchipelagoNotIncluded
{
    public class APSeedInfo
    {
        public string AP_seed { get; set; }
        public string AP_slotName { get; set; }

        public Dictionary<string, List<string>> technologies { get; set; }
    }

}