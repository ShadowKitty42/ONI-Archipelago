from collections import namedtuple
import os
import json
from typing import *
import typing

from BaseClasses import Item, Tutorial, Region, ItemClassification
from worlds.AutoWorld import WebWorld, World
from worlds.generic.Rules import set_rule
from .Items import ONIItem, ItemData
from .Locations import ONILocation
from .ArchipelagoItem import APItem
from .ModJson import ModJson, APJson, APLocationJson
from .Names import LocationNames, ItemNames, RegionNames
from .Options import ONIOptions
from .Regions import RegionInfo
from .Rules import *
from .DefaultItem import DefaultItem

def object_decoder(obj):
    return DefaultItem(obj['name'], obj['internal_name'], obj['research_level'],
                        obj['tech'], obj['internal_tech'], obj['ap_classification'], research_level_base="advanced",
                        version="Base", tech_base="unknown", internal_tech_base="unknown")

def item_decoder(objdict):
    return namedtuple('DefaultItem', objdict.keys())(*objdict.values())
 
class ONIWeb(WebWorld):
    theme = "ice"

    setup_en = Tutorial(
        "Multiworld Setup Guide",
        "A guide to setting up the Oxygen Not Included Randomizer connected to an Archipelago Multiworld",
        "English",
        "docs/setup_en.md",
        "setup/en",
        ["digiholic"]
    )
    tutorials = [setup_en]


class ONIWorld(World):
    game = "Oxygen Not Included"
    options_dataclass = ONIOptions
    options: ONIOptions
    topology_present = False
    web = ONIWeb()
    base_id = 0x257514000  # 0xYGEN___, clever! Thanks, Medic
    data_version = 0

    file = open(os.path.join(__file__, f"..\data\DefaultItemList.json"))
    default_item_list = json.load(file, object_hook=item_decoder)
    #print(default_item_list[0].name)
    file.close()

    '''spaced_out = [ "RadiationLight", "GeneticAnalysisStation", "SugarEngine", "SmallOxidizerTank", "KeroseneEngineClusterSmall", "MissionControlCluster",
                  "KeroseneEngineCluster", "BatteryModule", "SolarPanelModule", "RocketInteriorPowerPlug", "SteamEngineCluster", "CargoBayCluster",
                  "SolidCargoBaySmall", "RocketInteriorSolidInput", "RocketInteriorSolidOutput", "ModularLaunchpadPortSolid", "ModularLaunchpadPortSolidUnloader",
                  "RailGun", "LandingBeacon", "NoseconeHarvest", "LadderBed", "ModularLaunchpadPortBridge", "DiamondPress", "ScoutModule", "RailGunPayloadOpener",
                  "HydrogenEngineCluster", "OxidizerTankLiquidCluster", "ClusterTelescope", "ExobaseHeadquarters", "LaunchPad", "HabitatModuleSmall",
                  "OrbitalCargoModule", "RocketControlStation", "PioneerModule", "OrbitalResearchCenter", "DLC1CosmicResearchCenter", "NoseconeBasic",
                  "HabitatModuleMedium", "ArtifactAnalysisStation", "ArtifactCargoBay", "SpecialCargoBayCluster", "Telephone", "ClusterTelescopeEnclosed",
                  "NuclearResearchCenter", "ManualHighEnergyParticleSpawner", "HighEnergyParticleSpawner", "HighEnergyParticleRedirector", "HEPBattery",
                  "NuclearReactor", "UraniumCentrifuge", "HEPBridgeTile", "HEPEngine", "LogicRadiationSensor", "LeadSuit", "LeadSuitMarker", "LeadSuitLocker",
                  "LogicHEPSensor", "ModularLaunchpadPortLiquid", "ModularLaunchpadPortLiquidUnloader", "LiquidCargoBaySmall", "RocketInteriorLiquidInput",
                  "RocketInteriorLiquidOutput", "WallToilet", "DecontaminationShower", "SludgePress", "LiquidFuelTankCluster", "LiquidCargoBayCluster",
                  "GasCargoBayCluster", "CO2Engine", "ModularLaunchpadPortGas", "ModularLaunchpadPortGasUnloader", "GasCargoBaySmall", "RocketInteriorGasInput",
                  "RocketInteriorGasOutput", "OxidizerTankCluster", "LogicClusterLocationSensor", "ScannerModule", "LogicInterasteroidSender",
                  "LogicInterasteroidReceiver"
        ]
    frosty = [ "Deepfryer", "Campfire", "MercuryCeilingLight", "IceKettle", "Oxysconce", "WoodTile", "WoodSculpture"
        ]

    for item in default_item_list:
        if item.research_level == "basic":
            item.research_level_base = "basic"
        else:
            item.research_level_base = "advanced"
        item.tech_base = item.tech
        item.internal_tech_base = item.internal_tech
        if item.internal_name in spaced_out:
            item.version = "SpacedOut"
        if item.internal_name in frosty:
            item.version = "Frosty"
        if item.internal_name == "Gantry":
            item.tech_base = "Introductory Rocketry"
            item.internal_tech_base = "BasicRocketry"
    default_item_list.append(DefaultItem("Virtual Planetarium", "CosmicResearchCenter", "advanced", "None", "None", "Progression", version = "BaseOnly", tech_base="Computing", internal_tech_base="DupeTrafficControl"))
    default_item_list.append(DefaultItem("Telescope", "Telescope", "advanced", "None", "None", "Progression", version = "BaseOnly", tech_base="Celestial Detection", internal_tech_base="SkyDetectors"))
    default_item_list.append(DefaultItem("Command Capsule", "CommandModule", "advanced", "None", "None", "Progression", version = "BaseOnly", tech_base="Introductory Rocketry", internal_tech_base="BasicRocketry"))
    default_item_list.append(DefaultItem("Steam Engine", "SteamEngine", "advanced", "None", "None", "Progression", version = "BaseOnly", tech_base="Introductory Rocketry", internal_tech_base="BasicRocketry"))
    default_item_list.append(DefaultItem("Research Module", "ResearchModule", "advanced", "None", "None", "Progression", version = "BaseOnly", tech_base="Introductory Rocketry", internal_tech_base="BasicRocketry"))
    default_item_list.append(DefaultItem("Solid Fuel Thruster", "SolidBooster", "orbital", "None", "None", "Useful", version = "BaseOnly", tech_base="Solid Fuel Combustion", internal_tech_base="EnginesI"))
    default_item_list.append(DefaultItem("Mission Control Station", "MissionControl", "orbital", "None", "None", "Useful", version = "BaseOnly", tech_base="Solid Fuel Combustion", internal_tech_base="EnginesI"))
    default_item_list.append(DefaultItem("Petroleum Engine", "KeroseneEngine", "orbital", "None", "None", "Useful", version = "BaseOnly", tech_base="Hydrocarbon Combustion", internal_tech_base="EnginesII"))
    default_item_list.append(DefaultItem("Liquid Fuel Tank", "LiquidFuelTank", "orbital", "None", "None", "Useful", version = "BaseOnly", tech_base="Hydrocarbon Combustion", internal_tech_base="EnginesII"))
    default_item_list.append(DefaultItem("Solid Oxidizer Tank", "OxidizerTank", "orbital", "None", "None", "Useful", version = "BaseOnly", tech_base="Hydrocarbon Combustion", internal_tech_base="EnginesII"))
    default_item_list.append(DefaultItem("Liquid Oxidizer Tank", "OxidizerTankLiquid", "orbital", "None", "None", "Useful", version = "BaseOnly", tech_base="Cryofuel Combustion", internal_tech_base="EnginesIII"))
    default_item_list.append(DefaultItem("Hydrogen Engine", "HydrogenEngine", "orbital", "None", "None", "Useful", version = "BaseOnly", tech_base="Cryofuel Combustion", internal_tech_base="EnginesIII"))
    default_item_list.append(DefaultItem("Cargo Bay", "CargoBay", "orbital", "None", "None", "Useful", version = "BaseOnly", tech_base="Solid Cargo", internal_tech_base="CargoI"))
    default_item_list.append(DefaultItem("Liquid Cargo Bay", "LiquidCargoBay", "orbital", "None", "None", "Useful", version = "BaseOnly", tech_base="Liquid and Gas Cargo", internal_tech_base="CargoII"))
    default_item_list.append(DefaultItem("Gas Cargo Canister", "GasCargoBay", "orbital", "None", "None", "Useful", version = "BaseOnly", tech_base="Liquid and Gas Cargo", internal_tech_base="CargoII"))
    default_item_list.append(DefaultItem("Sight-Seeing Module", "TouristModule", "orbital", "None", "None", "Useful", version = "BaseOnly", tech_base="Unique Cargo", internal_tech_base="CargoIII"))
    default_item_list.append(DefaultItem("Biological Cargo Bay", "SpecialCargoBay", "orbital", "None", "None", "Useful", version = "BaseOnly", tech_base="Unique Cargo", internal_tech_base="CargoIII"))

    json_string = json.dumps(default_item_list, default=lambda o: o.__dict__, indent=4)
    output_file_path = os.path.join(__file__, f"..\item_list.json")
    with open(output_file_path, "w") as file:
        file.write(json_string)'''

    science_dicts = {}
    location_name_to_internal = {}
    internal_item_to_name = {}
    all_items = []
    basic_locations = []
    advanced_locations = []
    radbolt_locations = []
    orbital_locations = []
    all_regions = []
    all_locations = []

    item_name_to_id = {}
    location_name_to_id = {}
    
    regions_by_name = {}
    items_by_name = {}

    #ap_items = {}
    #ap_locations = {}
    
    base_only = True
    spaced_out = False
    frosty = False

    def generate_early(self) -> None:
        """
        Run before any general steps of the MultiWorld other than options. Useful for getting and adjusting option
        results and determining layouts for entrance rando etc. start inventory gets pushed after this step.
        """

        if self.options.spaced_out:
            self.base_only = False
            self.spaced_out = True
        if self.options.frosty:
            self.frosty = True

        for item in self.default_item_list:
            if self.base_only == False and item.version == "BaseOnly":
                continue;
            if self.spaced_out == False and item.version == "SpacedOut":
                continue;
            if self.frosty == False and item.version == "Frosty":
                continue;

            self.internal_item_to_name[item.internal_name] = item.name

            # Create list of Items
            ap_class = ItemClassification.useful
            #print(item)
            match item.ap_classification:
                case "Filler":
                    ap_class = ItemClassification.filler
                case "Progression":
                    ap_class = ItemClassification.progression
                case "Useful":
                    ap_class = ItemClassification.useful
                case "Trap":
                    ap_class = ItemClassification.trap
                case "SkipBalancing":
                    ap_class = ItemClassification.skip_balancing
                case "ProgressionSkipBalancing":
                    ap_class = ItemClassification.progression_skip_balancing
            self.all_items.append(ItemData(item.name, ap_class))

            # Add to correct list of locations
            location_name = ""
            research_level = ""
            tech = ""
            internal_tech = ""
            if self.base_only == True:
                research_level = item.research_level_base
                tech = item.tech_base
                internal_tech = item.internal_tech_base
            else:
                research_level = item.research_level
                tech = item.tech
                internal_tech = item.internal_tech
            match research_level:
                case "basic":
                    count = len(list(filter(lambda location: tech in location, self.basic_locations))) + 1
                    location_name = f"{tech} - {count}"
                    self.basic_locations.append(location_name)
                case "advanced":
                    count = len(list(filter(lambda location: tech in location, self.advanced_locations))) + 1
                    location_name = f"{tech} - {count}"
                    self.advanced_locations.append(location_name)
                case "radbolt":
                    count = len(list(filter(lambda location: tech in location, self.radbolt_locations))) + 1
                    location_name = f"{tech} - {count}"
                    self.radbolt_locations.append(location_name)
                case "orbital":
                    count = len(list(filter(lambda location: tech in location, self.orbital_locations))) + 1
                    location_name = f"{tech} - {count}"
                    self.orbital_locations.append(location_name)

            #print(f"{research_level}, {tech}, {internal_tech}, {location_name}, {self.basic_locations.__len__() + self.advanced_locations.__len__() + self.radbolt_locations.__len__() + self.orbital_locations.__len__()}")
            # Create Location to Internal Mapping
            if location_name not in self.location_name_to_internal:
                self.location_name_to_internal[location_name] = internal_tech

            # Populate Science Dict (to be used in generate_output)
            if internal_tech not in self.science_dicts:
                self.science_dicts[internal_tech] = []
        
        if self.base_only == True:
            self.all_regions = [
                RegionInfo("Menu", []),
                RegionInfo(RegionNames.Basic, self.basic_locations),
                RegionInfo(RegionNames.Advanced, self.advanced_locations),
                RegionInfo(RegionNames.Space_Base, self.orbital_locations)
            ]
            self.all_locations = self.basic_locations + self.advanced_locations + self.orbital_locations
        else:
            self.all_regions = [
                RegionInfo("Menu", []),
                RegionInfo(RegionNames.Basic, self.basic_locations),
                RegionInfo(RegionNames.Advanced, self.advanced_locations),
                RegionInfo(RegionNames.Nuclear, self.radbolt_locations),
                RegionInfo(RegionNames.Space_DLC, self.orbital_locations)
            ]
            self.all_locations = self.basic_locations + self.advanced_locations + self.radbolt_locations + self.orbital_locations

        self.item_name_to_id = {data.itemName: 0x257514000 + index for index, data in enumerate(self.all_items)}
        self.location_name_to_id = {loc_name: 0x257514000 + index for index, loc_name in enumerate(self.all_locations)}
    
        self.regions_by_name = {region.name: region for region in self.all_regions}
        self.items_by_name = {item.itemName: item for item in self.all_items}

    def create_regions(self) -> None:
        """Method for creating and connecting regions for the World."""
        regions_by_name = {}

        for region_info in self.all_regions:
            region = Region(region_info.name, self.player, self.multiworld)
            regions_by_name[region_info.name] = region
            for location_name in region_info.locations:
                #self.ap_locations[location_name] = self.location_name_to_id.get(location_name, None)
                location = ONILocation(self.player, location_name, self.location_name_to_id.get(location_name, None), region)
                region.locations.append(location)
            self.multiworld.regions.append(region)

        if self.base_only == True:
            regions_by_name["Menu"].connect(
                regions_by_name[RegionNames.Basic], None, None)
            regions_by_name[RegionNames.Basic].connect(
                regions_by_name[RegionNames.Advanced], None, lambda state: can_advanced_research(self.player, self.internal_item_to_name, state))
            regions_by_name[RegionNames.Advanced].connect(
                regions_by_name[RegionNames.Space_Base], None, lambda state: can_space_research_base(self.player, self.internal_item_to_name, state))
        else:
            regions_by_name["Menu"].connect(
                regions_by_name[RegionNames.Basic], None, None)
            regions_by_name[RegionNames.Basic].connect(
                regions_by_name[RegionNames.Advanced], None, lambda state: can_advanced_research(self.player, self.internal_item_to_name, state))
            regions_by_name[RegionNames.Advanced].connect(
                regions_by_name[RegionNames.Nuclear], None, lambda state: can_nuclear_research(self.player, self.internal_item_to_name, state))
            regions_by_name[RegionNames.Nuclear].connect(
                regions_by_name[RegionNames.Space_DLC], None, lambda state: can_space_research(self.player, self.internal_item_to_name, state))

    def create_items(self) -> None:
        """
        Method for creating and submitting items to the itempool. Items and Regions must *not* be created and submitted
        to the MultiWorld after this step. If items need to be placed during pre_fill use `get_prefill_items`.
        """
        for item in self.all_items:
            progressionStr = "None"
            match item.progression:
                case ItemClassification.filler:
                    progressionStr = "Filler"
                case ItemClassification.progression:
                    progressionStr = "Progression"
                case ItemClassification.useful:
                    progressionStr = "Useful"
                case ItemClassification.trap:
                    progressionStr = "Trap"
                case ItemClassification.skip_balancing:
                    progressionStr = "SkipBalancing"
                case ItemClassification.progression_skip_balancing:
                    progressionStr = "ProgressionSkipBalancing"
                    
            #self.ap_items[item.itemName] = APItem(self.item_name_to_id.get(item.itemName, None), progressionStr)
            self.multiworld.itempool.append(self.create_item(item.itemName))

    def set_rules(self) -> None:
        """Method for setting the rules on the World's regions and locations."""
        pass

    def generate_basic(self) -> None:
        """
        Useful for randomizing things that don't affect logic but are better to be determined before the output stage.
        i.e. checking what the player has marked as priority or randomizing enemies
        """
        pass

    def pre_fill(self) -> None:
        """Optional method that is supposed to be used for special fill stages. This is run *after* plando."""
        pass

    def fill_hook(self,
                  progitempool: List["Item"],
                  usefulitempool: List["Item"],
                  filleritempool: List["Item"],
                  fill_locations: List["Location"]) -> None:
        """Special method that gets called as part of distribute_items_restrictive (main fill)."""
        pass

    def post_fill(self) -> None:
        """Optional Method that is called after regular fill. Can be used to do adjustments before output generation.
        This happens before progression balancing, so the items may not be in their final locations yet."""

    def generate_output(self, output_directory: str) -> None:
        """This method gets called from a threadpool, do not use multiworld.random here.
        If you need any last-second randomization, use self.random instead."""
        # TODO generate mod json
        item_names = [data.itemName for data in self.all_items]
        for location_name in self.all_locations:     # location_name = tech + location number
            tech_name = self.location_name_to_internal[location_name]
            location = self.multiworld.get_location(location_name, self.player)
            ap_item = location.item
            if ap_item is not None and ap_item.name in item_names:
                self.science_dicts[tech_name].append([x for x in self.default_item_list if x.name == ap_item.name][0].internal_name)

        mod_json = ModJson(str(self.multiworld.seed), self.multiworld.player_name[self.player], self.spaced_out, self.frosty, self.science_dicts)
        json_string = mod_json.to_json(indent=4)
        output_file_path = os.path.join(output_directory, f"{self.multiworld.get_out_file_name_base(self.player)}.json")
        with open(output_file_path, "w") as file:
            file.write(json_string)

        '''ap_json = APJson(self.ap_items)
        json_string = ap_json.to_json(indent=4)
        output_file_path = os.path.join(output_directory, f"oxygen not included_item_table.json")
        with open(output_file_path, "w") as file:
            file.write(json_string)

        ap_location_json = APLocationJson(self.ap_locations)
        json_string = ap_location_json.to_json(indent=4)
        output_file_path = os.path.join(output_directory, f"oxygen not included_location_table.json")
        with open(output_file_path, "w") as file:
            file.write(json_string)'''


    def fill_slot_data(self) -> typing.Dict[str, Any]:  # json of WebHostLib.models.Slot
        """Fill in the `slot_data` field in the `Connected` network package.
        This is a way the generator can give custom data to the client.
        The client will receive this as JSON in the `Connected` response.

        The generation does not wait for `generate_output` to complete before calling this.
        `threading.Event` can be used if you need to wait for something from `generate_output`."""
        return {}

    def extend_hint_information(self, hint_data: typing.Dict[int, typing.Dict[int, str]]):
        """Fill in additional entrance information text into locations, which is displayed when hinted.
        structure is {player_id: {location_id: text}} You will need to insert your own player_id."""
        pass

    def modify_multidata(self, multidata: typing.Dict[str, Any]) -> None:  # TODO: TypedDict for multidata?
        """For deeper modification of server multidata."""
        pass

    def create_item(self, name: str) -> "Item":
        """Create an item for this world type and player.
        Warning: this may be called with self.world = None, for example by MultiServer"""
        item = self.items_by_name[name]
        return ONIItem(item.itemName, item.progression, self.item_name_to_id[name], self.player)
