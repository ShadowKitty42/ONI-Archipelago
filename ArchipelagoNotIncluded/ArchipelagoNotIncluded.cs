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
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace ArchipelagoNotIncluded
{
    public class ArchipelagoNotIncluded : UserMod2
    {
        //public KaitoKid.ArchipelagoUtilities.Net.Interfaces.ILogger logger;

        public static ArchipelagoSession session = null;
        //public static ArchipelagoConnectionInfo apConnection = null;
        //public ArchipelagoState State { get; set; }

        public static APSeedInfo info = null;

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

        public override void OnLoad(Harmony harmony)
        {
            PUtil.InitLibrary();

            DirectoryInfo modDirectory = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            foreach (FileInfo jsonFile in modDirectory.EnumerateFiles("*.json").OrderByDescending(f => f.LastWriteTime))
            {
                try
                {
                    if (jsonFile.Name == "DefaultItemList.json")
                        continue;
                    string json = File.ReadAllText(jsonFile.FullName);
                    info = JsonConvert.DeserializeObject<APSeedInfo>(json);
                    break;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Debug.LogWarning($"Failed to parse JSON file {jsonFile.FullName}");
                    continue;
                }
            }
            // Load Default Research
            List<DefaultItem> ItemInfo = new List<DefaultItem>();
            Sciences = new Dictionary<string, List<string>>();
            foreach (FileInfo jsonFile in modDirectory.EnumerateFiles("*.json"))
            {
                Debug.Log(jsonFile.FullName);
                try
                {
                    if (jsonFile.Name != "DefaultItemList.json")
                        continue;
                    string json = File.ReadAllText(jsonFile.FullName);
                    ItemInfo = JsonConvert.DeserializeObject<List<DefaultItem>>(json);
                    break;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Debug.LogWarning($"Failed to parse JSON file {jsonFile.FullName}");
                    continue;
                }
            }
            foreach (DefaultItem item in ItemInfo)
            {
                Debug.Log(info.spaced_out);
                Debug.Log(item.internal_tech + " " + item.internal_tech_base);
                string InternalTech = info.spaced_out ? item.internal_tech : item.internal_tech_base;
                if (Sciences.Count > 0 && Sciences?.TryGetValue(InternalTech, out List<string> techList) == true)
                {
                    if (techList == null)
                        techList = new List<string>();
                    techList.Add(item.internal_name);
                }
                else
                {
                    Sciences[InternalTech] = new List<string>
                    {
                        item.internal_name
                    };
                }
            }

            base.OnLoad(harmony);
            //State = new ArchipelagoState();
            session = ArchipelagoSessionFactory.CreateSession("localhost",  38281);
            /*apConnection = new ArchipelagoConnectionInfo("localhost", 38281, "Shadow", false);
            Debug.Log(apConnection.ToString());
            if (TryConnectArchipelago())*/
            LoginResult result = session.TryConnectAndLogin("Oxygen Not Included", "Shadow", Archipelago.MultiClient.Net.Enums.ItemsHandlingFlags.AllItems);
            if (result.Successful)
                Debug.Log("Connection successful");
            else
                Debug.Log("Connection failed");
        }

        /*public bool TryConnectArchipelago()
        {
            if (apConnection != null)
                State.APConnectionInfo = apConnection;
            
            if (session.IsConnected)
                return true;

            var error = "";
            if (session.ConnectToMultiworld(apConnection, out error))
                return true;

            return false;
        }*/

        private void OnItemReceived()
        { }
    }
}
