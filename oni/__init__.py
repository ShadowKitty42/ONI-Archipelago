from collections import namedtuple
import os
import json
import threading
from typing import *
import typing

from BaseClasses import Item, Location, Tutorial, Region, ItemClassification
from worlds.AutoWorld import WebWorld, World
from worlds.generic.Rules import set_rule
from .Items import ONIItem, ItemData, all_items
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

def mod_item_decoder(objdict):
    return namedtuple('ModItem', objdict.keys())(*objdict.values())


def convert_ap_class(ap_string):
    # Create list of Items
    ap_class = ItemClassification.useful
    #print(item)
    match ap_string:
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
    return ap_class
 
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

    default_item_list = []
    mod_item_list = {}
    mod_items_exist = False
    data_path = os.path.join(__file__, f"..\\data\\")
    folder = os.scandir(data_path)

    for file in folder:
        if file.is_file():
            if file.name == "DefaultItemList.json":
                contents = open(file)
                default_item_list = json.load(contents, object_hook=item_decoder)
                contents.close()

            if file.name.endswith("ModItems.json"):
                mod_items_exist = True
                player_name = file.name.split("_")[0]
                contents = open(file)
                mod_item_list[player_name] = json.load(contents, object_hook=mod_item_decoder)
                contents.close()
    

    science_dicts = {}
    location_name_to_internal = {}
    internal_item_to_name = {}
    name_to_internal_name = {}
    all_items = []
    local_items = ["Atmo Suit", "Jet Suit Pattern", "Lead Suit", "Oxygen Mask Pattern"]
    basic_locations = []
    advanced_locations = []
    radbolt_locations = []
    orbital_locations = []
    all_regions = []
    all_locations = []
    mod_json = {}
    ap_mod_items = []
    slot_data_ready = threading.Event()
    
    base_only = True
    spaced_out = False
    frosty = False
    bionic = False

    
    for item in default_item_list:

        internal_item_to_name[item.internal_name] = item.name

        # Create list of Items
        all_items.append(ItemData(item.name, convert_ap_class(item.ap_classification)))

        location_name = ""
        tech = item.tech
        count = 1
        if (tech != "None"):
            for location in all_locations:
                if tech in location:
                    count += 1

            location_name = f"{tech} - {count}"
            all_locations.append(location_name)

        if (item.tech != item.tech_base):
            tech = item.tech_base
            count = 1
            for location in all_locations:
                if tech in location:
                    count += 1

            location_name = f"{tech} - {count}"
            all_locations.append(location_name)

    if (mod_items_exist):
        for player in mod_item_list:
            for item in mod_item_list[player]:

                internal_item_to_name[item.internal_name] = item.name

                # Create list of Items
                all_items.append(ItemData(item.name, convert_ap_class(item.ap_classification)))

                location_name = ""
                tech = item.tech
                count = 1
                for location in all_locations:
                    if tech in location:
                        count += 1

                location_name = f"{tech} - {count}"
                all_locations.append(location_name)

                ap_mod_items.append(item.internal_name)

    item_name_to_id = {data.itemName: 0x257514000 + index for index, data in enumerate(all_items)}
    #item_name_to_id = {data.name: index for index, data in enumerate(all_items, base_id)}
    location_name_to_id = {loc_name: 0x257514000 + index for index, loc_name in enumerate(all_locations)}
    #location_name_to_id = {loc_name: index for index, loc_name in enumerate(all_locations, base_id)}
    
    #regions_by_name = {region.name: region for region in all_regions}
    items_by_name = {item.itemName: item for item in all_items}


    #ap_items = {}
    #ap_locations = {}

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
        if self.options.bionic:
            self.bionic = True
            
        self.all_items = []
        for item in self.default_item_list:
            if self.base_only == False and item.version == "BaseOnly":
                continue;
            if self.spaced_out == False and item.version == "SpacedOut":
                continue;
            if self.frosty == False and item.version == "Frosty":
                continue;
            if self.bionic == False and item.version == "Bionic":
                continue;
            
            self.name_to_internal_name[item.name] = item.internal_name

            # Create list of Items
            self.all_items.append(ItemData(item.name, convert_ap_class(item.ap_classification)))

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


        if (self.mod_items_exist):
            for item in self.mod_item_list[self.player_name]:
                
                self.name_to_internal_name[item.name] = item.internal_name
                
                # Create list of Items
                self.all_items.append(ItemData(item.name, convert_ap_class(item.ap_classification)))

                # Add to correct list of locations
                location_name = ""
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
            '''progressionStr = "None"
            match item.classification:
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
                    progressionStr = "ProgressionSkipBalancing"'''
                    
            #self.ap_items[item.itemName] = APItem(self.item_name_to_id.get(item.itemName, None), progressionStr)
            if item.itemName not in self.local_items:
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
        from Fill import fill_restrictive

        world = self.multiworld
        player = self.player
        all_state = world.get_all_state(use_cache=True)
        suits = [self.create_item(name) for name in ['Atmo Suit', 'Jet Suit Pattern', 'Oxygen Mask Pattern']]
        if self.spaced_out == True:
            suits.append(self.create_item('Lead Suit'))

        loc_dict = world.get_locations(player)
        locs = list(loc_dict)
        world.random.shuffle(locs)

        fill_restrictive(world, all_state, locs, suits, True, True, name="ONI Add Suits")

        self.multiworld.local_items[self.player].value.add("Atmo Suit")
        self.multiworld.local_items[self.player].value.add("Jet Suit Pattern")
        if self.spaced_out == True:
            self.multiworld.local_items[self.player].value.add("Lead Suit")
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
        print(f"ModItems: {self.mod_items_exist}")
        for location_name in self.all_locations:     # location_name = tech + location number\
            tech_name = self.location_name_to_internal[location_name]
            location = self.multiworld.get_location(location_name, self.player)
            ap_item = location.item
            if ap_item is not None:
                output_item_name = ap_item.name
                if output_item_name in item_names:
                    output_item_name = self.name_to_internal_name[ap_item.name]
                    #output_item_name = [x for x in self.default_item_list if x.name == ap_item.name][0].internal_name
                self.science_dicts[tech_name].append(output_item_name)

        self.mod_json = ModJson(str(self.multiworld.seed), self.multiworld.player_name[self.player], self.spaced_out, self.frosty, self.bionic, self.science_dicts)
        json_string = self.mod_json.to_json(indent=4)
        output_file_path = os.path.join(output_directory, f"{self.multiworld.get_out_file_name_base(self.player)}.json")
        with open(output_file_path, "w") as file:
            file.write(json_string)
        
        self.slot_data_ready.set()

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
        self.slot_data_ready.wait()

        slot_data = {
            "AP_seed": self.mod_json.AP_seed,
            "AP_slotName": self.mod_json.AP_slotName,
            "URL": self.mod_json.URL,
            "port": self.mod_json.port,
            "spaced_out": self.spaced_out,
            "frosty": self.frosty,
            "bionic": self.bionic,
            "technologies": self.mod_json.technologies,
            "apModItems": self.ap_mod_items
        }
        return slot_data

    def extend_hint_information(self, hint_data: typing.Dict[int, typing.Dict[int, str]]):
        """Fill in additional entrance information text into locations, which is displayed when hinted.
        structure is {player_id: {location_id: text}} You will need to insert your own player_id."""
        pass

    def modify_multidata(self, multidata: typing.Dict[str, Any]) -> None:  # TODO: TypedDict for multidata?
        """For deeper modification of server multidata."""
        pass

    def create_item(self, name: str) -> "ONIItem":
        """Create an item for this world type and player.
        Warning: this may be called with self.world = None, for example by MultiServer"""
        #self.items_by_name[name].code = self.item_name_to_id[name]
        #self.items_by_name[name].player = self.player
        item = self.items_by_name[name]
        return ONIItem(item.itemName, item.progression, self.item_name_to_id[name], self.player)
        #return self.items_by_name[name]
