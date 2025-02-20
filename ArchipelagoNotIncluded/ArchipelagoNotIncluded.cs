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
using static ArchipelagoNotIncluded.Patches;
using PeterHan.PLib.Options;
using PeterHan.PLib.PatchManager;
using UtilLibs;
using static MathUtil;
using Epic.OnlineServices;

namespace ArchipelagoNotIncluded
{
    public class ArchipelagoNotIncluded : UserMod2
    {
        //public KaitoKid.ArchipelagoUtilities.Net.Interfaces.ILogger logger;
        private static bool patched = false;
        public static bool cheatmode = false;

        private static int Counter = 0;
        public static APNetworkMonitor netmon = null;
        //public static ArchipelagoSession session = null;
        public static int lastItem;
        //public static ArchipelagoConnectionInfo apConnection = null;
        //public ArchipelagoState State { get; set; }

        public static DirectoryInfo modDirectory = null;
        internal static ANIOptions Options {  get; private set; }
        public static APSeedInfo info = null;
        public static List<KeyValuePair<string, string>> apItems = new List<KeyValuePair<string, string>>();
        public static List<DefaultItem> AllDefaultItems = new List<DefaultItem>();
        public static List<ModItem> AllModItems = new List<ModItem>();
        public static List<string> TechList = new List<string>();
        public static string AtmoSuitTech = "";
        public static string JetSuitTech = "";
        public static string LeadSuitTech = "";

        public static Dictionary<string, List<string>> Sciences = new Dictionary<string, List<string>>()
        {
            {"FarmingTech", new List<string>()
            {
                "AlgaeHabitat",
                "PlanterBox",
                "RationBox",
                "Compost"
            }},
            {"FineDining", new List<string>()
            {
                "CookingStation",
                "EggCracker",
                "DiningTable",
                "FarmTile"
            }},
            {"FoodRepurposing", new List<string>()
            {
                "Juicer",
                "SpiceGrinder",
                "MilkPress"
            }},
            {"FinerDining", new List<string>()
            {
                "GourmetCookingStation",
                "FoodDehydrator",
                "FoodRehydrator"
            }},
            {"Agriculture", new List<string>()
            {
                "FarmStation",
                "FertilizerMaker",
                "Refrigerator",
                "HydroponicFarm",
                "ParkSign",
                "RadiationLight"
            }},
            {"Ranching", new List<string>()
            {
                "RanchStation",
                "CreatureDeliveryPoint",
                "ShearingStation",
                "CreatureFeeder",
                "FishDeliveryPoint",
                "FishFeeder"
            }},
            {"AnimalControl", new List<string>()
            {
                "CreatureAirTrap",
                "CreatureGroundTrap",
                "WaterTrap",
                "EggIncubator",
                LogicCritterCountSensorConfig.ID
            }},
            {"AnimalComfort", new List<string>()
            {
                "CritterCondo",
                "UnderwaterCritterCondo"
            }},
            {"DairyOperation", new List<string>()
            {
                "MilkFeeder",
                "MilkFatSeparator",
                "MilkingStation"
            }},
            {"ImprovedOxygen", new List<string>()
            {
                "Electrolyzer",
                "RustDeoxidizer"
            }},
            {"GasPiping", new List<string>()
            {
                "GasConduit",
                "GasConduitBridge",
                "GasPump",
                "GasVent"
            }},
            {"ImprovedGasPiping", new List<string>()
            {
                "InsulatedGasConduit",
                LogicPressureSensorGasConfig.ID,
                "GasLogicValve",
                "GasVentHighPressure"
            }},
            {"SpaceGas", new List<string>()
            {
                "CO2Engine",
                "ModularLaunchpadPortGas",
                "ModularLaunchpadPortGasUnloader",
                "GasCargoBaySmall"
            }},
            {"PressureManagement", new List<string>()
            {
                "LiquidValve",
                "GasValve",
                "GasPermeableMembrane",
                "ManualPressureDoor"
            }},
            {"DirectedAirStreams", new List<string>()
            {
                "AirFilter",
                "CO2Scrubber",
                "PressureDoor"
            }},
            {"LiquidFiltering", new List<string>()
            {
                "OreScrubber",
                "Desalinator"
            }},
            {"MedicineI", new List<string>()
            {
                "Apothecary"
            }},
            {"MedicineII", new List<string>()
            {
                "DoctorStation",
                "HandSanitizer"
            }},
            {"MedicineIII", new List<string>()
            {
                GasConduitDiseaseSensorConfig.ID,
                LiquidConduitDiseaseSensorConfig.ID,
                LogicDiseaseSensorConfig.ID
            }},
            {"MedicineIV", new List<string>()
            {
                "AdvancedDoctorStation",
                "AdvancedApothecary", // Not present
				"HotTub",
                LogicRadiationSensorConfig.ID
            }},
            {"LiquidPiping", new List<string>()
            {
                "LiquidConduit",
                "LiquidConduitBridge",
                "LiquidPump",
                "LiquidVent"
            }},
            {"ImprovedLiquidPiping", new List<string>()
            {
                "InsulatedLiquidConduit",
                LogicPressureSensorLiquidConfig.ID,
                "LiquidLogicValve",
                "LiquidConduitPreferentialFlow",// Not Present
				"LiquidConduitOverflow",// Not Present
				"LiquidReservoir"
            }},
            {"PrecisionPlumbing", new List<string>()
            {
                "EspressoMachine",
                "LiquidFuelTankCluster"
            }},
            {"SanitationSciences", new List<string>()
            {
                "FlushToilet",
                "WashSink",
                ShowerConfig.ID,
                "MeshTile"
            }},
            {"FlowRedirection", new List<string>()
            {
                "MechanicalSurfboard",
                "ModularLaunchpadPortLiquid",
                "ModularLaunchpadPortLiquidUnloader",
                "LiquidCargoBaySmall"
            }},
            {"LiquidDistribution", new List<string>()
            {
                "RocketInteriorLiquidInput",
                "RocketInteriorLiquidOutput",
                "WallToilet"
            }},
            {"AdvancedSanitation", new List<string>()
            {
                "DecontaminationShower"
            }},
            {"AdvancedFiltration", new List<string>()
            {
                "GasFilter",
                "LiquidFilter",
                "SludgePress"
            }},
            {"Distillation", new List<string>()
            {
                "AlgaeDistillery",
                "EthanolDistillery",
                "WaterPurifier"
            }},
            {"Catalytics", new List<string>()
            {
                "OxyliteRefinery",
                "Chlorinator",
                "SupermaterialRefinery",
                "SodaFountain",
                "GasCargoBayCluster"
            }},
            {"AdvancedResourceExtraction", new List<string>()  //Base Game
			{
                "NoseconeHarvest"
            }},
            {"PowerRegulation", new List<string>()
            {
                "BatteryMedium",
                SwitchConfig.ID,
                "WireBridge"
            }},
            {"AdvancedPowerRegulation", new List<string>()
            {
                "HighWattageWire",
                "WireBridgeHighWattage",
                "HydrogenGenerator",
                LogicPowerRelayConfig.ID,
                "PowerTransformerSmall",
                LogicWattageSensorConfig.ID
            }},
            {"PrettyGoodConductors", new List<string>()
            {
                "WireRefined",
                "WireRefinedBridge",
                "WireRefinedHighWattage",
                "WireRefinedBridgeHighWattage",
                "PowerTransformer"
            }},
            {"RenewableEnergy", new List<string>()
            {
                "SteamTurbine2",
                "SolarPanel",
                "Sauna",
                "SteamEngineCluster"
            }},
            {"Combustion", new List<string>()
            {
                "Generator",
                "WoodGasGenerator"
            }},
            {"ImprovedCombustion", new List<string>()
            {
                "MethaneGenerator",
                "OilRefinery",
                "PetroleumGenerator"
            }},
            {"InteriorDecor", new List<string>()
            {
                "FlowerVase",
                "FloorLamp",
                "CeilingLight"
            }},
            {"Artistry", new List<string>()
            {
                "FlowerVaseWall",
                "FlowerVaseHanging",
                "CornerMoulding",
                "CrownMoulding",
                "ItemPedestal",
                "SmallSculpture",
                "IceSculpture"
            }},
            {"Clothing", new List<string>()
            {
                "ClothingFabricator",
                "CarpetTile",
                "ExteriorWall"
            }},
            {"Acoustics", new List<string>()
            {
                "BatterySmart",
                "Phonobox",
                "PowerControlStation"
            }},
            {"SpacePower", new List<string>()
            {
                "BatteryModule",
                "SolarPanelModule",
                "RocketInteriorPowerPlug"
            }},
            {"NuclearRefinement", new List<string>()
            {
                "NuclearReactor",
                "UraniumCentrifuge",
                "HEPBridgeTile"
            }},
            {"FineArt", new List<string>()
            {
                "Canvas",
                "Sculpture"
            }},
            {"EnvironmentalAppreciation", new List<string>()
            {
                "BeachChair"
            }},
            {"Luxury", new List<string>()
            {
                "LuxuryBed",
                "LadderFast",
                "PlasticTile",
                "ClothingAlterationStation"
            }},
            {"RefractiveDecor", new List<string>()
            {
                "CanvasWide",
                "MetalSculpture"
            }},
            {"GlassFurnishings", new List<string>()
            {
                "GlassTile",
                "FlowerVaseHangingFancy",
                "SunLamp"
            }},
            {"Screens", new List<string>()
            {
                PixelPackConfig.ID
            }},
            {"RenaissanceArt", new List<string>()
            {
                "CanvasTall",
                "MarbleSculpture"
            }},
            {"Plastics", new List<string>()
            {
                "Polymerizer",
                "OilWellCap"
            }},
            {"ValveMiniaturization", new List<string>()
            {
                "LiquidMiniPump",
                "GasMiniPump"
            }},
            {"HydrocarbonPropulsion", new List<string>()
            {
                "KeroseneEngineClusterSmall",
                "MissionControlCluster"
            }},
            {"BetterHydroCarbonPropulsion", new List<string>()
            {
                "KeroseneEngineCluster"
            }},
            {"CryoFuelPropulsion", new List<string>()
            {
                "HydrogenEngineCluster",
                "OxidizerTankLiquidCluster"
            }},
            {"Suits", new List<string>()
            {
                "SuitsOverlay",
                "AtmoSuit",
                "SuitFabricator",
                "SuitMarker",
                "SuitLocker"
            }},
            {"Jobs", new List<string>()
            {
                "WaterCooler",
                "CraftingTable"
            }},
            {"AdvancedResearch", new List<string>()
            {
                "BetaResearchPoint",
                "AdvancedResearchCenter",
                "ResetSkillsStation",
                "ClusterTelescope",
                "ExobaseHeadquarters"
            }},
            {"SpaceProgram", new List<string>()
            {
                "LaunchPad",
                "HabitatModuleSmall",
                "OrbitalCargoModule",
                RocketControlStationConfig.ID
            }},
            {"CrashPlan", new List<string>()
            {
                "OrbitalResearchPoint",
                "PioneerModule",
                "OrbitalResearchCenter",
                "DLC1CosmicResearchCenter"
            }},
            {"DurableLifeSupport", new List<string>()
            {
                "NoseconeBasic",
                "HabitatModuleMedium",
                "ArtifactAnalysisStation",
                "ArtifactCargoBay",
                "SpecialCargoBayCluster"
            }},
            {"NuclearResearch", new List<string>()
            {
                "DeltaResearchPoint",
                "NuclearResearchCenter",
                "ManualHighEnergyParticleSpawner"
            }},
            {"AdvancedNuclearResearch", new List<string>()
            {
                "HighEnergyParticleSpawner",
                "HighEnergyParticleRedirector"
            }},
            {"NuclearStorage", new List<string>()
            {
                "HEPBattery"
            }},
            {"NuclearPropulsion", new List<string>()
            {
                "HEPEngine"
            }},
            {"NotificationSystems", new List<string>()
            {
                LogicHammerConfig.ID,
                LogicAlarmConfig.ID,
                "Telephone"
            }},
            {"ArtificialFriends", new List<string>()
            {
                "SweepBotStation",
                "ScoutModule"
            }},
            {"BasicRefinement", new List<string>()
            {
                "RockCrusher",
                "Kiln"
            }},
            {"RefinedObjects", new List<string>()
            {
                "FirePole",
                "ThermalBlock",
                LadderBedConfig.ID,
                "ModularLaunchpadPortBridge"
            }},
            {"Smelting", new List<string>()
            {
                "MetalRefinery",
                "MetalTile"
            }},
            {"HighTempForging", new List<string>()
            {
                "GlassForge",
                "BunkerTile",
                "BunkerDoor",
                "GeoTuner",
                "Gantry" //manually added, normally done in code during boot
			}},
            {"HighPressureForging", new List<string>()
            {
                "DiamondPress"
            }},
            {"RadiationProtection", new List<string>()
            {
                "LeadSuit",
                "LeadSuitMarker",
                "LeadSuitLocker",
                LogicHEPSensorConfig.ID
            }},
            {"TemperatureModulation", new List<string>()
            {
                "LiquidCooledFan", // Not present
				"IceCooledFan",
                "IceMachine",
                "InsulationTile",
                "SpaceHeater"
            }},
            {"HVAC", new List<string>()
            {
                "AirConditioner",
                LogicTemperatureSensorConfig.ID,
                GasConduitTemperatureSensorConfig.ID,
                GasConduitElementSensorConfig.ID,
                "GasConduitRadiant",
                "GasReservoir",
                "GasLimitValve"
            }},
            {"LiquidTemperature", new List<string>()
            {
                "LiquidConduitRadiant",
                "LiquidConditioner",
                LiquidConduitTemperatureSensorConfig.ID,
                LiquidConduitElementSensorConfig.ID,
                "LiquidHeater",
                "LiquidLimitValve",
                "ContactConductivePipeBridge"
            }},
            {"LogicControl", new List<string>()
            {
                "AutomationOverlay",
                LogicSwitchConfig.ID,
                "LogicWire",
                "LogicWireBridge",
                "LogicDuplicantSensor"
            }},
            {"GenericSensors", new List<string>()
            {
                "FloorSwitch",
                LogicElementSensorGasConfig.ID,
                LogicElementSensorLiquidConfig.ID,
                "LogicGateNOT",
                LogicTimeOfDaySensorConfig.ID,
                LogicTimerSensorConfig.ID,
                LogicLightSensorConfig.ID,
                LogicClusterLocationSensorConfig.ID
            }},
            {"LogicCircuits", new List<string>()
            {
                "LogicGateAND",
                "LogicGateOR",
                "LogicGateBUFFER",
                "LogicGateFILTER"
            }},
            {"ParallelAutomation", new List<string>()
            {
                "LogicRibbon",
                "LogicRibbonBridge",
                LogicRibbonWriterConfig.ID,
                LogicRibbonReaderConfig.ID
            }},
            {"DupeTrafficControl", new List<string>()
            {
                LogicCounterConfig.ID,
                LogicMemoryConfig.ID,
                "LogicGateXOR",
                "ArcadeMachine",
                "Checkpoint",
                "CosmicResearchCenter" // Not Present
			}},
            {"Multiplexing", new List<string>()
            {
                "LogicGateMultiplexer",
                "LogicGateDemultiplexer"
            }},
            {"SkyDetectors", new List<string>()
            {
                CometDetectorConfig.ID,
                "Telescope",
                "ClusterTelescopeEnclosed",
                "AstronautTrainingCenter"
            }},
            {"TravelTubes", new List<string>()
            {
                "TravelTubeEntrance",
                "TravelTube",
                "TravelTubeWallBridge",
                "VerticalWindTunnel"
            }},
            {"SmartStorage", new List<string>()
            {
                "ConveyorOverlay",
                "SolidTransferArm",
                "StorageLockerSmart",
                "ObjectDispenser"
            }},
            {"SolidManagement", new List<string>()
            {
                "SolidFilter",
                SolidConduitTemperatureSensorConfig.ID,
                SolidConduitElementSensorConfig.ID,
                SolidConduitDiseaseSensorConfig.ID,
                "StorageTile",
                "CargoBayCluster"
            }},
            {"HighVelocityTransport", new List<string>()
            {
                "RailGun",
                "LandingBeacon"
            }},
            {"BasicRocketry", new List<string>() //Base Game
			{
                "CommandModule",
                "SteamEngine",
                "ResearchModule",
                "Gantry"
            }},
            {"CargoI", new List<string>() //Base Game
			{
                "CargoBay"
            }},
            {"CargoII", new List<string>() //Base Game
			{
                "LiquidCargoBay",
                "GasCargoBay"
            }},
            {"CargoIII", new List<string>() //Base Game
			{
                "TouristModule",
                "SpecialCargoBay"
            }},
            {"EnginesI", new List<string>() //Base Game
			{
                "SolidBooster",
                "MissionControl"
            }},
            {"EnginesII", new List<string>() //Base Game
			{
                "KeroseneEngine",
                "LiquidFuelTank",
                "OxidizerTank"
            }},
            {"EnginesIII", new List<string>() //Base Game
			{
                "OxidizerTankLiquid",
                "OxidizerTankCluster",
                "HydrogenEngine"
            }},
            {"Jetpacks", new List<string>() //Base Game
			{
                "JetSuit",
                "JetSuitMarker",
                "JetSuitLocker",
                "LiquidCargoBayCluster",
                "MissileFabricator",
                "MissileLauncher"
            }},
            {"SolidTransport", new List<string>()
            {
                "SolidConduitInbox",
                "SolidConduit",
                "SolidConduitBridge",
                "SolidVent"
            }},
            {"Monuments", new List<string>()
            {
                "MonumentBottom",
                "MonumentMiddle",
                "MonumentTop"
            }},
            {"SolidSpace", new List<string>()
            {
                "SolidLogicValve",
                "SolidConduitOutbox",
                "SolidLimitValve",
                "SolidCargoBaySmall",
                "RocketInteriorSolidInput",
                "RocketInteriorSolidOutput",
                "ModularLaunchpadPortSolid",
                "ModularLaunchpadPortSolidUnloader"
            }},
            {"RoboticTools", new List<string>()
            {
                "AutoMiner",
                "RailGunPayloadOpener"
            }},
            {"PortableGasses", new List<string>()
            {
                "GasBottler",
                "BottleEmptierGas",
                "OxygenMask",
                "OxygenMaskLocker",
                "OxygenMaskMarker"
            }},
            {"Bioengineering", new List<string>()
            {
                "GeneticAnalysisStation"
            }},

            {"SpaceCombustion", new List<string>()
            {
                "SugarEngine",
                "SmallOxidizerTank"
            }},

            {"HighVelocityDestruction", new List<string>()
            {
                "NoseconeHarvest"
            }},

            {"GasDistribution", new List<string>()
            {
                "RocketInteriorGasInput",
                "RocketInteriorGasOutput",
                "OxidizerTankCluster"
            }},

            {"AdvancedScanners", new List<string>()
              {
                "ScannerModule",
                "LogicInterasteroidSender",
                "LogicInterasteroidReceiver"
            }},
        };

        public static Dictionary<string, List<string>> BaseGameSciences = new Dictionary<string, List<string>>()
        {
            {"BasicRocketry", new List<string>()
            {
                "CommandModule",
                "SteamEngine",
                "ResearchModule",
                "Gantry"
            }},
            {"CargoI", new List<string>()
            {
                "CargoBay"
            }},
            {"CargoII", new List<string>()
            {
                "LiquidCargoBay",
                "GasCargoBay"
            }},
            {"CargoIII", new List<string>()
            {
                "TouristModule",
                "SpecialCargoBay"
            }},
            {"EnginesI", new List<string>()
            {
                "SolidBooster",
                "MissionControl"
            }},
            {"EnginesII", new List<string>()
            {
                "KeroseneEngine",
                "LiquidFuelTank",
                "OxidizerTank"
            }},
            {"EnginesIII", new List<string>()
            {
                "OxidizerTankLiquid",
                "OxidizerTankCluster",
                "HydrogenEngine"
            }},
            {"Jetpacks", new List<string>()
            {
                "JetSuit",
                "JetSuitMarker",
                "JetSuitLocker",
                "LiquidCargoBayCluster",
                "MissileFabricator",
                "MissileLauncher"
            }},
            {"SkyDetectors", new List<string>()
            {
                CometDetectorConfig.ID,
                "Telescope",
                "ClusterTelescopeEnclosed",
                "AstronautTrainingCenter"
            }},
            {"AdvancedResourceExtraction", new List<string>()
            {
                "NoseconeHarvest"
            }}
        };

        public static List<string> PreUnlockedTech = new List<string>()
        {
            "BetaResearchPoint",
            "DeltaResearchPoint",
            "OrbitalResearchPoint",
            "ConveyorOverlay",
            "AutomationOverlay",
            "SuitsOverlay"
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

        public static int getLastIndex()
        {
            lastItem = netmon.session.Items.AllItemsReceived.Count;
            return lastItem;
        }
        public static int setLastIndex(int index)
        {
            return lastItem = index;
        }

        public override void OnLoad(Harmony harmony)
        {
            PUtil.InitLibrary();
            cheatmode = true;
            
            Options = POptions.ReadSettings<ANIOptions>() ?? new ANIOptions();
            new POptions().RegisterOptions(this, typeof(ANIOptions));
            //Options = ReloadOptions();
            if (Options.Password == "")
                netmon = new APNetworkMonitor(Options.URL, Options.Port, Options.SlotName);
            else
                netmon = new APNetworkMonitor(Options.URL, Options.Port, Options.SlotName, Options.Password);
            LoginResult result = netmon.TryConnectArchipelago(ItemsHandlingFlags.AllItems);
            if (result.Successful)
            {
                LoginSuccessful success = (LoginSuccessful)result;
                info = JsonConvert.DeserializeObject<APSeedInfo>(JsonConvert.SerializeObject(success.SlotData));
                //netmon.session.Socket.DisconnectAsync();
            }

            lastItem = 0;

            modDirectory = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            Sciences = new Dictionary<string, List<string>>();
            foreach (FileInfo jsonFile in modDirectory.EnumerateFiles("*.json").OrderByDescending(f => f.LastWriteTime))
            {
                try
                {
                    string json = File.ReadAllText(jsonFile.FullName);
                    if (jsonFile.Name == "DefaultItemList.json")
                    {
                        AllDefaultItems = JsonConvert.DeserializeObject<List<DefaultItem>>(json);
                        continue;
                    }
                    if (jsonFile.Name == $"{Options.SlotName}_ModItems.json")
                    {
                        AllModItems = JsonConvert.DeserializeObject<List<ModItem>>(json);
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
            if (info != null)
            {
                foreach (DefaultItem item in AllDefaultItems)
                {
                    //Debug.Log(info.spaced_out);
                    //Debug.Log(item.internal_tech + " " + item.internal_tech_base);
                    string InternalTech = info.spaced_out ? item.internal_tech : item.internal_tech_base;
                    if (Sciences.Count > 0 && Sciences?.TryGetValue(InternalTech, out List<string> techList) == true)
                    {
                        if (techList == null)
                            techList = new List<string>();
                        techList.Add(item.internal_name);
                    }
                    else
                    {
                        if (InternalTech == "None")
                            continue;
                        Sciences[InternalTech] = new List<string>
                        {
                            item.internal_name
                        };
                    }
                }

                if (AllModItems != null)
                {
                    foreach (ModItem item in AllModItems)
                    {
                        if (info?.apModItems.Contains(item.internal_name) == true)
                            item.randomized = true;
                    }
                }
            }

            var original = AccessTools.Method(typeof(Database.Techs), nameof(Database.Techs.Init));
            var prefix = AccessTools.Method(typeof(Techs_Init_Patch), nameof(Techs_Init_Patch.Prefix));
            harmony.Patch(original, new HarmonyMethod(prefix));
            original = AccessTools.Method(typeof(Database.Techs), nameof(Database.Techs.Load));
            prefix = AccessTools.Method(typeof(Techs_Load_Patch), nameof(Techs_Load_Patch.Prefix));
            harmony.Patch(original, new HarmonyMethod(prefix));
            original = AccessTools.Method(typeof(Db), nameof(Db.Initialize));
            var postfix = AccessTools.Method(typeof(Db_Initialize_Patch), nameof(Db_Initialize_Patch.Postfix));
            harmony.Patch(original, postfix: new HarmonyMethod(postfix));
            //ModUtil.AddBuildingToPlanScreen((HashedString)"test", "id");

            SceneManager.sceneLoaded += (scene, loadScene) => {
                Debug.Log($"Scene: {scene.name}");
                if (!patched)
                {
                    base.OnLoad(harmony);
                    patched = true;
                }
                if (scene.name == "backend")
                {
                    if (Options.CreateModList)
                    {
                        List<ModItem> modItems = new List<ModItem>();
                        foreach (Tech tech in Db.Get().Techs.resources)
                        {
                            foreach (TechItem techitem in tech.unlockedItems)
                            {
                                DefaultItem defItem = AllDefaultItems.Find(i => i.internal_name == techitem.Id);
                                if (defItem == null && !PreUnlockedTech.Contains(techitem.Id))
                                    modItems.Add(new ModItem(techitem));
                            }
                        }
                        //Debug.Log("Directory: " + modDirectory.ToString());
                        //File.WriteAllText(modDirectory.ToString() + "\\ModItems.json", JsonConvert.SerializeObject(modItems, Formatting.Indented));
                        if (modItems.Count > 0)
                        {
                            using (FileStream fs = File.Open(modDirectory.ToString() + $"\\{Options.SlotName}_ModItems.json", FileMode.Create))
                            {
                                using (StreamWriter sw = new StreamWriter(fs))
                                {
                                    using (JsonTextWriter jw = new JsonTextWriter(sw))
                                    {
                                        jw.Formatting = Formatting.Indented;
                                        jw.IndentChar = ' ';
                                        jw.Indentation = 4;

                                        JsonSerializer serializer = new JsonSerializer();
                                        serializer.Serialize(jw, modItems);
                                    }
                                }
                            }
                        }
                        Options.CreateModList = false;
                        POptions.WriteSettings(Options);
                    }
                    else if (netmon.session == null)
                    {
                        netmon.TryConnectArchipelago();
                    }
                    //JsonConvert.SerializeObject()
                    //netmon.UpdateAllItems();
                }
                /*else
                {
                    netmon.TryConnectArchipelago();
                }*/
            };
        }

        [PLibMethod(RunAt.OnStartGame)]
        public static void Connect()
        {
            netmon.UpdateAllItems();
            
        }
        
        [PLibMethod(RunAt.OnStartGame)]
        public static void ReloadOptions()
        {
            Options = POptions.ReadSettings<ANIOptions>() ?? new ANIOptions();
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

        /*private void SceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
        {
            this.OnLoad(harmony);
        }*/

        /*public void Render1000ms(float _)
        {
            Debug.Log($"Render1000ms");
            if (session == null)
            {
                Counter++;
                if (Counter == 20)
                {
                    Debug.Log($"Attempting to reconnect");
                    TryConnectArchipelago();
                    Counter = 0;
                }
            }
        }*/
    }
}
