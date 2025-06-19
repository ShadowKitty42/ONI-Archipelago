using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net.Models;
using KSerialization;

namespace ArchipelagoNotIncluded
{
    internal class APSaveData : KMonoBehaviour
    {
        public static APSaveData Instance;

        [Serialize] public string seed;
        [Serialize] public bool GoalComplete = false;
        [Serialize] public bool AllowResourceChecks = false;
        [Serialize] public bool HadBionicDupe = false;
        [Serialize] public int LastItemIndex = 0;
        [Serialize] public List<string> LocalItemList = new List<string>();
        [Serialize] public ConcurrentQueue<string> LocationQueue = new ConcurrentQueue<string>();
        [Serialize] public APSeedInfo APSeedInfo = null;
        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Instance = this;
            AllowResourceChecks = ArchipelagoNotIncluded.AllowResourceChecks;
            if (ArchipelagoNotIncluded.info == null && APSeedInfo != null)
                ArchipelagoNotIncluded.info = APSeedInfo;
        }
    }
}
