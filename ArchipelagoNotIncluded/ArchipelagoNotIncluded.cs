using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using KMod;
//using KaitoKid.ArchipelagoUtilities.Net.Client;
//using KaitoKid.ArchipelagoUtilities.Net.Interfaces;
//using ArchipelagoNotIncluded.Serialization;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using ArchipelagoNotIncluded.Patches;
using PeterHan.PLib.Options;
using PeterHan.PLib.PatchManager;
using UtilLibs;
using static MathUtil;
using Epic.OnlineServices;
using Newtonsoft.Json.Converters;

namespace ArchipelagoNotIncluded
{
    public class ArchipelagoNotIncluded : UserMod2
    {
        /*
         * Use Game Scheduler in 2 places, item queue and reconnection
         * offline list of items in case network is down
         * fix techitems in LoadGeneratedBuildings_Patch        - Done
         * Create list of missing techitems in techs.init. create them in loadgeneratedbuildings    - Done
         */

        public static ArchipelagoNotIncluded Instance { get; private set; } = null;

        //public KaitoKid.ArchipelagoUtilities.Net.Interfaces.ILogger logger;
        public static bool cheatmode = false;
        public static bool devmode = false;
        public static bool skipUnlockedItems = false;
        public static bool AllowResourceChecks = false;

        public static APNetworkMonitor netmon = null;
        //public static ArchipelagoSession session = null;
        public static int lastItem;
        //public static ArchipelagoConnectionInfo apConnection = null;
        //public ArchipelagoState State { get; set; }

        public static DirectoryInfo modDirectory = null;
        internal static ANIOptions Options {  get; private set; }
        /*public static ANIOptions Options
        {
            get
            {
                if (Options == null)
                    Options = POptions.ReadSettings<ANIOptions>() ?? new ANIOptions();
                return Options;
            }
            protected set
            {
                if (Options != null)
                    Options = value;
            }
        }*/
        public static APSeedInfo info = null;
        public static List<KeyValuePair<string, string>> apItems = new List<KeyValuePair<string, string>>();
        public static List<DefaultItem> AllDefaultItems = new List<DefaultItem>();
        public static List<ModItem> AllModItems = new List<ModItem>();
        public static Dictionary<string, string> allTechList = new Dictionary<string, string>();
        public static string AtmoSuitTech = "";
        public static string JetSuitTech = "";
        public static string LeadSuitTech = "";

        public static Dictionary<string, List<string>> Sciences = new Dictionary<string, List<string>>();

        public static List<string> PreUnlockedTech = new List<string>()
        {
            "BetaResearchPoint",
            "DeltaResearchPoint",
            "OrbitalResearchPoint",
            "ConveyorOverlay",
            "AutomationOverlay",
            "SuitsOverlay"
        };

        public static Dictionary<string, string> BasePlanets = new Dictionary<string, string>()
        {
            {"terra", "clusters/SandstoneDefault" },
            {"relica", "dlc2::clusters/PrehistoricBaseGameCluster" },
            {"ceres", "dlc2::clusters/CeresBaseGameCluster" },
            {"oceania", "clusters/Oceania" },
            {"rime", "clusters/SandstoneFrozen" },
            {"verdante", "clusters/ForestLush" },
            {"arboria", "clusters/ForestDefault" },
            {"volcanea", "clusters/Volcanic" },
            {"badlands", "clusters/Badlands" },
            {"aridio", "clusters/ForestHot" },
            {"oasisse", "clusters/Oasis" }
        };

        public static Dictionary<string, string> BaseLabPlanets = new Dictionary<string, string>()
        {
            {"skewed", "clusters/KleiFest2023" },
            {"relicargh", "dlc4::clusters/PrehistoricShatteredBaseGameCluster" },
            {"blasted", "dlc2::clusters/CeresBaseGameShatteredCluster" }
        };

        public static Dictionary<string, string> ClassicPlanets = new Dictionary<string, string>()
        {
            {"terra", "expansion1::clusters/VanillaSandstoneCluster" },
            {"relica", "dlc4::clusters/PrehistoricClassicCluster" },
            {"ceres", "dlc2::clusters/CeresClassicCluster" },
            {"oceania", "expansion1::clusters/VanillaOceaniaCluster" },
            {"squelchy", "expansion1::clusters/VanillaSwampCluster" },
            {"rime", "expansion1::clusters/VanillaSandstoneFrozenCluster" },
            {"verdante", "expansion1::clusters/VanillaForestCluster" },
            {"arboria", "expansion1::clusters/VanillaArboriaCluster" },
            {"volcanea", "expansion1::clusters/VanillaVolcanicCluster" },
            {"badlands", "expansion1::clusters/VanillaBadlandsCluster" },
            {"aridio", "expansion1::clusters/VanillaAridioCluster" },
            {"oasisse", "expansion1::clusters/VanillaOasisCluster" }
        };

        public static Dictionary<string, string> SpacedOutPlanets = new Dictionary<string, string>()
        {
            {"terrania", "expansion1::clusters/SandstoneStartCluster" },
            {"relica_minor", "dlc4::clusters/PrehistoricSpacedOutCluster" },
            {"ceres_minor", "dlc2::clusters/CeresSpacedOutCluster" },
            {"folia", "expansion1::clusters/ForestStartCluster" },
            {"quagmiris", "expansion1::clusters/SwampStartCluster" },
            {"metallic_swampy", "expansion1::clusters/MiniClusterMetallicSwampyStart" },
            {"desolands", "expansion1::clusters/MiniClusterBadlandsStart" },
            {"frozen_forest", "expansion1::clusters/MiniClusterForestFrozenStart" },
            {"flipped", "expansion1::clusters/MiniClusterFlippedStart" },
            {"radioactive_ocean", "expansion1::clusters/MiniClusterRadioactiveOceanStart" },
            {"ceres_mantle", "dlc2::clusters/CeresSpacedOutShatteredCluster" }
        };

        public static Dictionary<string, string> ClassicLabPlanets = new Dictionary<string, string>()
        {
            {"skewed", "expansion1::clusters/KleiFest2023Cluster" },
            {"relicargh", "dlc4::clusters/PrehistoricShatteredClassicCluster" },
            {"blasted", "dlc2::clusters/CeresClassicShatteredCluster" }
        };

        public static List<string> StarterTech = new List<string>()
        {
            "Ladder",
            "Tile",
            "SnowTile",
            "Door",
            "StorageLocker",
            "MineralDeoxidizer",
            "SublimationStation",
            "ManualGenerator",
            "Wire",
            "Battery",
            "MicrobeMusher",
            "Outhouse",
            "LiquidPumpingStation",
            "BottleEmptier",
            "WashBasin",
            "MedicalCot",
            "MassageTable",
            "Grave",
            "Bed",
            "ResearchCenter"
        };

        public static List<string> ItemList = new List<string>();
        public static List<string> ItemListDetailed = new List<string>();
        public static bool DebugWasUsed = false;
        public static int lastIndexSaved = 0;
        public static int runCount = 0;
        public static string planetText = string.Empty;

        public static StatusItemCategory ArchipelagoConnection = null;
        public static StatusItem ArchipelagoConnected = null;
        public static StatusItem ArchipelagoDisconnected = null;

        public static int getLastIndex()
        {
            //lastItem = netmon.session.Items.AllItemsReceived.Count;
            Debug.Log($"getLastIndex: {lastItem}");
            return lastItem;
        }
        public static int setLastIndex(int index)
        {
            Debug.Log($"setLastIndex: {index}");
            return lastItem = index;
        }

        public override void OnLoad(Harmony harmony)
        {
            Instance = this;

            PUtil.InitLibrary();
            string configPath = Path.Combine(Manager.GetDirectory(), "config");
            if (!System.IO.Directory.Exists(configPath))
                System.IO.Directory.CreateDirectory(configPath);

            Options = POptions.ReadSettings<ANIOptions>() ?? new ANIOptions();
            new POptions().RegisterOptions(this, typeof(ANIOptions));
            //Options = ReloadOptions();

            lastItem = 0;

            modDirectory = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            foreach (FileInfo jsonFile in modDirectory.EnumerateFiles("*.json"))
            {
                try
                {
                    string json = File.ReadAllText(jsonFile.FullName);
                    if (jsonFile.Name == "DefaultItemList.json")
                    {
                        AllDefaultItems = JsonConvert.DeserializeObject<List<DefaultItem>>(json);
                        continue;
                    }
                    //if (info == null)
                    //info = JsonConvert.DeserializeObject<APSeedInfo>(json);
                    continue;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Debug.LogWarning($"Failed to parse JSON file {jsonFile.FullName}");
                    continue;
                }
            }
            /*modDirectory = new DirectoryInfo(modItemsPath);
            foreach (FileInfo jsonFile in modDirectory.EnumerateFiles("*.json"))
            {
                try
                {
                    string json = File.ReadAllText(jsonFile.FullName);
                    if (jsonFile.Name == $"{Options.SlotName}_ModItems.json")
                    {
                        AllModItems = JsonConvert.DeserializeObject<List<ModItem>>(json);
                        continue;
                    }
                    continue;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Debug.LogWarning($"Failed to parse JSON file {jsonFile.FullName}");
                    continue;
                }
            }*/

            base.OnLoad(harmony);

            netmon = new APNetworkMonitor(Options.URL, Options.Port, Options.SlotName, Options.Password);
            netmon.TryConnectArchipelago();
            SceneManager.sceneLoaded += (scene, loadScene) => {
                Debug.Log($"Scene: {scene.name}");
                if (scene.name == "frontend")
                {
                    List<TechItem> newTechs = new List<TechItem>();
                    foreach (KeyValuePair<IBuildingConfig, BuildingDef> kvp in BuildingConfigManager.Instance.configTable)
                    {
                        TechItem techItem = Db.Get().TechItems.TryGet(kvp.Value.PrefabID);
                        string assembly = kvp.Key.GetType().Assembly.GetName().Name;
                        if (techItem != null)
                        {
                            if (assembly == "Assembly-CSharp")
                            {
                                DefaultItem defItem = AllDefaultItems.Find(i => i.internal_name == techItem.Id);
                                if (defItem == null && !PreUnlockedTech.Contains(techItem.Id))
                                {
                                    Debug.LogWarning($"Klei Building Not in List: {CleanName(techItem.Name)} ({techItem.Id}) Assembly: {assembly}");
                                    if (devmode)
                                    {
                                        newTechs.Add(techItem);
                                        //Debug.Log($"Added Default Item: {techItem.Name} ({techItem.Id}) Assembly: {assembly}");
                                    }
                                }
                            }
                            else
                            {
                                if (!AllModItems.Exists(i => i.internal_name == techItem.Id))
                                    AllModItems.Add(new ModItem(techItem));
                                //Debug.Log($"Added Mod Item: {techItem.Name} ({techItem.Id}) Assembly: {assembly}");
                            }
                        }
                    }
                    if (devmode && newTechs.Count > 0)
                    {
                        Debug.LogWarning($"New Building(s) Added. Make sure all fields are correct.");
                        foreach (TechItem techItem in newTechs)
                            AllDefaultItems.Add(new DefaultItem(techItem));
                        using (FileStream fs = File.Open(Path.Combine(modDirectory.FullName, "DefaultItemList.json"), FileMode.Create))
                        {
                            using (StreamWriter sw = new StreamWriter(fs))
                            {
                                using (JsonTextWriter jw = new JsonTextWriter(sw))
                                {
                                    jw.Formatting = Formatting.Indented;
                                    jw.IndentChar = ' ';
                                    jw.Indentation = 4;

                                    JsonSerializer serializer = new JsonSerializer();
                                    serializer.Serialize(jw, AllDefaultItems);
                                }
                            }
                        }
                    }
                }
            };
        }
        
        [PLibMethod(RunAt.OnStartGame)]
        public static void ReloadOptions()
        {
            Options = POptions.ReadSettings<ANIOptions>() ?? new ANIOptions();
        }

        public static void HandleSlotData(Dictionary<string, object> data)
        {
            info = JsonConvert.DeserializeObject<APSeedInfo>(JsonConvert.SerializeObject(data), [new VersionConverter()]);
            Debug.Log($"SlotData Received - AP World Version: {info.APWorld_Version}");
        }

        public static string CleanName(string name)
        {
            char[] delimiters = { '<', '>' };
            string cleaned = "";
            if (name.Contains('<'))
            {
                cleaned = name.Split(delimiters)[2];
            }
            else
            {
                cleaned = name;
            }
            return cleaned;
        }

        public static void AddLocationChecks(params string[] LocationNames)
        {
            if (!netmon.session.Socket.Connected)
            {
                Debug.Log("Network not connected, caching location checks.");
                foreach (string name in LocationNames)
                    APSaveData.Instance.LocationQueue.Enqueue(name);
            }
            else
            {
                netmon.SendLocationChecks(LocationNames);
            }
        }
    }
}
