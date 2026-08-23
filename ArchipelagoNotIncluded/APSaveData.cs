using System.Collections.Concurrent;
using System.Collections.Generic;
using ArchipelagoNotIncluded.Patches;
using KSerialization;

namespace ArchipelagoNotIncluded
{
    internal class APSaveData : KMonoBehaviour
    {
        public static APSaveData Instance;

        [Serialize] private bool _goalComplete = false;
        [Serialize]
        public bool GoalComplete
        {
            get => _goalComplete;
            set
            {
                _goalComplete = value;
                if (_goalComplete)
                    ArchipelagoNotIncluded.netmon.session?.SetGoalAchieved();
            }
        }
        [Serialize] public bool AllowResourceChecks = false;
        [Serialize] public bool HadBionicDupe = false;
        [Serialize] public int LastItemIndex = 0;
        [Serialize] public bool ResearchPortalUnlocked = false;
        [Serialize] public List<string> LocalItemList = new List<string>();
        [Serialize] public ConcurrentQueue<string> LocationQueue = new ConcurrentQueue<string>();
        [Serialize] public List<string> PendingCarePackages = new List<string>();
        [Serialize] public List<string> DeliveredCarePackages = new List<string>();
        [Serialize] public List<string> ForfeitedCarePackages = new List<string>();
        [Serialize] public List<string> ReceivedCarePackages = new List<string>();
        [Serialize] public APSeedInfo APSeedInfo = null;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Instance = this;
            AllowResourceChecks = ArchipelagoNotIncluded.AllowResourceChecks;
            if (ArchipelagoNotIncluded.info == null && APSeedInfo != null)
                ArchipelagoNotIncluded.info = APSeedInfo;

            SaveLoader_Load_Patch.TryInitializeFromSave();
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            CarePackageQueue.ReconcileCarePackages();
            CarePackageQueue.RebuildFromSave();
            if (PendingCarePackages.Count > 0 && APSeedInfo != null)
            {
                APSeedInfo.replace_print_cycle = true;
                if (ArchipelagoNotIncluded.info != null)
                    ArchipelagoNotIncluded.info.replace_print_cycle = true;
            }
            TelepadApCarePackagePatches.RefreshUi();
        }
    }
}
