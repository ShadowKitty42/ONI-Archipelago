from collections import namedtuple
import os
import json
import threading
import logging
from typing import *
import typing
import pkgutil
import sys
import gc

import Utils
from BaseClasses import Item, Location, Tutorial, Region, ItemClassification
from worlds.AutoWorld import WebWorld, World
from worlds.generic.Rules import set_rule
from .Items import ONIItem, ItemData, care_packages_base, care_packages_frosty, care_packages_bionic
from .Locations import ONILocation, resource_locations
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
    ap_version = "0.9.5.0"

    default_item_list = {}
    mod_item_list = {}
    mod_items_exist = False
    data_path = Utils.user_path("data", "ONI")
    if not os.path.exists(data_path):
        os.makedirs(data_path)
    #print(data_path)
    folder = os.scandir(data_path)

    default_item_list = json.loads(pkgutil.get_data(__name__, "data/DefaultItemList.json"), object_hook=item_decoder)

    #modules = pkgutil.iter_modules([os.path.dirname(sys.modules[__name__].__file__)])
    #for module in modules:
    #    print(module)

    for file in folder:
        if file.is_file():
            if file.name.endswith("ModItems.json"):
                mod_items_exist = True
                player_name = file.name.split("_")[0]
                contents = open(file)
                mod_item_list[player_name] = json.load(contents, object_hook=mod_item_decoder)
                contents.close()

    science_dicts: typing.Dict[str, typing.List[str]]
    location_name_to_internal: typing.Dict[str, str]
    all_items: typing.List[ItemData]
    all_regions: typing.List[RegionInfo]
    all_locations: typing.List[str]
    resource_checks: typing.List[str]
    mod_json: ModJson
    ap_mod_items: typing.List[str]
    local_items: typing.List[str]
    filler_item_names: typing.List[str]

    internal_item_to_name = {}
    name_to_internal_name = {}
    complete_item_list = []
    complete_location_list = []
    #local_items = ["Atmo Suit", "Jet Suit Pattern", "Lead Suit", "Oxygen Mask Pattern"]

    slot_data_ready = threading.Event()
    
    base_only = True
    spaced_out = False
    frosty = False
    bionic = False
    prehistoric = False

    
    for item in default_item_list:

        internal_item_to_name[item.internal_name] = item.name

        # Create list of Items
        complete_item_list.append(ItemData(item.name, convert_ap_class(item.ap_classification)))

        location_name = ""
        tech = item.tech
        count = 1
        if (tech != "None"):
            for location in complete_location_list:
                tech_name = location.split(" - ")[0]
                if tech == tech_name:
                    count += 1

            location_name = f"{tech} - {count}"
            complete_location_list.append(location_name)

        if (item.tech != item.tech_base and item.tech_base != "None"):
            tech = item.tech_base
            count = 1
            for location in complete_location_list:
                tech_name = location.split(" - ")[0]
                if tech == tech_name:
                    count += 1

            location_name = f"{tech} - {count}"
            complete_location_list.append(location_name)

    for care_package in care_packages_base:
        complete_item_list.append(ItemData(f"Care Package: {care_package}", ItemClassification.filler))

    for care_package in care_packages_frosty:
        complete_item_list.append(ItemData(f"Care Package: {care_package}", ItemClassification.filler))

    for care_package in care_packages_bionic:
        complete_item_list.append(ItemData(f"Care Package: {care_package}", ItemClassification.filler))

    if (mod_items_exist):
        for player in mod_item_list:
            for item in mod_item_list[player]:

                internal_item_to_name[item.internal_name] = item.name

                # Create list of Items
                complete_item_list.append(ItemData(item.name, convert_ap_class(item.ap_classification)))

                location_name = ""
                tech = item.tech
                count = 1
                for location in complete_location_list:
                    tech_name = location.split(" - ")[0]
                    if tech == tech_name:
                        count += 1

                location_name = f"{tech} - {count}"
                complete_location_list.append(location_name)

    for planet in resource_locations:
        for level in resource_locations[planet]:
            for location in resource_locations[planet][level]:
                current = f"Discover Resource: {location}"
                if current not in complete_location_list:
                    complete_location_list.append(current)
                
    item_name_to_id = {data.itemName: 0x257514000 + index for index, data in enumerate(complete_item_list)}
    #item_name_to_id = {data.name: index for index, data in enumerate(all_items, base_id)}
    location_name_to_id = {loc_name: 0x257514000 + index for index, loc_name in enumerate(complete_location_list)}
    #location_name_to_id = {loc_name: index for index, loc_name in enumerate(all_locations, base_id)}
    
    #regions_by_name = {region.name: region for region in all_regions}
    items_by_name = {item.itemName: item for item in complete_item_list}


    #ap_items = {}
    #ap_locations = {}

    def generate_early(self) -> None:
        """
        Run before any general steps of the MultiWorld other than options. Useful for getting and adjusting option
        results and determining layouts for entrance rando etc. start inventory gets pushed after this step.
        """
            
        current_player_name = self.multiworld.get_player_name(self.player)
        basic_locations = []
        advanced_locations = []
        radbolt_locations = []
        orbital_locations = []

        self.science_dicts = {}
        self.location_name_to_internal = {}
        self.all_items = []
        self.all_regions = []
        self.all_locations = []
        self.ap_mod_items = []
        self.local_items = []

        if self.options.spaced_out.value:
            self.base_only = False
            self.spaced_out = True
        if self.options.frosty.value:
            self.frosty = True
        if self.options.bionic.value:
            self.bionic = True
            self.local_items.append("Crafting Station")
        if self.options.prehistoric.value:
            self.prehistoric = True
            
        self.filler_item_names = care_packages_base.copy()
        if self.options.frosty.value:
            self.filler_item_names += care_packages_frosty.copy()
        if self.options.bionic.value:
            self.filler_item_names += care_packages_bionic.copy()

        #if self.options.cluster.current_key != "custom" and self.base_only and self.options.cluster.has_basegame_equivalent == False:
        #    logging.warning(f"Base Game doesn't have starting planet called \"{self.options.cluster.current_key}\". Changing option to default planet.")
        #    self.options.cluster.value = 10         # If planet name not in base game, set to default

        
        if "ceres" in self.options.cluster.current_key and not self.frosty:
            logging.warning(f"Frosty DLC is not enabled and planet choice requires it. Changing option to default planet.")
            if self.spaced_out:
                self.options.cluster.value = 10         # set to default for spaced out
            else:
                self.options.cluster_base.value = 0         # set to default for base game
        
        for item in self.default_item_list:
            if self.base_only == False and item.version == "BaseOnly":
                continue;
            if self.spaced_out == False and item.version == "SpacedOut":
                continue;
            if self.frosty == False and item.version == "Frosty":
                continue;
            if self.bionic == False and item.version == "Bionic":
                continue;
            if self.prehistoric == False and item.version == "Prehistoric":
                continue;
            if self.bionic == True and self.base_only == True and item.tech_base == "None":
                continue;
            if item.internal_name == "RoboPilotCommandModule":
                if self.base_only == False or self.bionic == False:
                    continue;
            
            item_class = item.ap_classification
            if "ceres" in self.options.cluster.current_key:
                if item.internal_name == "WoodTile" or item.internal_name == "IceKettle":
                    self.items_by_name[item.name] = ItemData(item.name, ItemClassification.progression)
                    item_class = "Progression"
            
            if self.bionic:
                if item.internal_name == "Apothecary":
                    self.items_by_name[item.name] = ItemData(item.name, ItemClassification.progression)
                    item_class = "Progression"

            self.name_to_internal_name[item.name] = item.internal_name

            # Create list of Items
            self.all_items.append(ItemData(item.name, convert_ap_class(item_class)))

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
                    count = 1
                    for location in basic_locations:
                        tech_name = location.split(" - ")[0]
                        if tech == tech_name:
                            count += 1
                    location_name = f"{tech} - {count}"
                    basic_locations.append(location_name)
                case "advanced":
                    count = 1
                    for location in advanced_locations:
                        tech_name = location.split(" - ")[0]
                        if tech == tech_name:
                            count += 1
                    location_name = f"{tech} - {count}"
                    advanced_locations.append(location_name)
                case "radbolt":
                    count = 1
                    for location in radbolt_locations:
                        tech_name = location.split(" - ")[0]
                        if tech == tech_name:
                            count += 1
                    location_name = f"{tech} - {count}"
                    radbolt_locations.append(location_name)
                case "orbital":
                    count = 1
                    for location in orbital_locations:
                        tech_name = location.split(" - ")[0]
                        if tech == tech_name:
                            count += 1
                    location_name = f"{tech} - {count}"
                    orbital_locations.append(location_name)

            #print(f"{research_level}, {tech}, {internal_tech}, {location_name}, {self.basic_locations.__len__() + self.advanced_locations.__len__() + self.radbolt_locations.__len__() + self.orbital_locations.__len__()}")
            # Create Location to Internal Mapping
            if location_name not in self.location_name_to_internal:
                self.location_name_to_internal[location_name] = internal_tech

            # Populate Science Dict (to be used in generate_output)
            if internal_tech not in self.science_dicts:
                self.science_dicts[internal_tech] = []
                
        

        if (self.mod_item_list.__contains__(current_player_name)):
            base_orbital = []
            if self.base_only:
                base_orbital = ["EnginesI", "EnginesII", "EnginesIII", "CargoI", "CargoII", "CargoIII"]
            for item in self.mod_item_list[current_player_name]:
                #print(f"Adding mod items for: {self.player_name}")
                self.name_to_internal_name[item.name] = item.internal_name
                self.ap_mod_items.append(item.internal_name)
                
                # Create list of Items
                self.all_items.append(ItemData(item.name, convert_ap_class(item.ap_classification)))

                # Add to correct list of locations
                location_name = ""
                research_level = item.research_level
                if self.base_only:
                    if research_level == "radbolt":
                        research_level = "advanced"
                    elif research_level == "orbital" and item.internal_tech not in base_orbital:
                        research_level = "advanced"

                tech = item.tech
                internal_tech = item.internal_tech
                match research_level:
                    case "basic":
                        count = 1
                        for location in basic_locations:
                            tech_name = location.split(" - ")[0]
                            if tech == tech_name:
                                count += 1
                        location_name = f"{tech} - {count}"
                        basic_locations.append(location_name)
                    case "advanced":
                        count = 1
                        for location in advanced_locations:
                            tech_name = location.split(" - ")[0]
                            if tech == tech_name:
                                count += 1
                        location_name = f"{tech} - {count}"
                        advanced_locations.append(location_name)
                    case "radbolt":
                        count = 1
                        for location in radbolt_locations:
                            tech_name = location.split(" - ")[0]
                            if tech == tech_name:
                                count += 1
                        location_name = f"{tech} - {count}"
                        radbolt_locations.append(location_name)
                    case "orbital":
                        count = 1
                        for location in orbital_locations:
                            tech_name = location.split(" - ")[0]
                            if tech == tech_name:
                                count += 1
                        location_name = f"{tech} - {count}"
                        orbital_locations.append(location_name)

                #print(f"{research_level}, {tech}, {internal_tech}, {location_name}, {self.basic_locations.__len__() + self.advanced_locations.__len__() + self.radbolt_locations.__len__() + self.orbital_locations.__len__()}")
                # Create Location to Internal Mapping
                if location_name not in self.location_name_to_internal:
                    self.location_name_to_internal[location_name] = internal_tech

        self.resource_checks = []
        if self.options.resource_checks.value:
            planet = self.options.cluster.current_key
            if self.base_only:
                planet = f"{self.options.cluster_base.current_key}_base"
            if planet in resource_locations:
                for resource in resource_locations[planet]["basic"]:
                    basic_locations.append(f"Discover Resource: {resource}")
                    self.resource_checks.append(f"Discover Resource: {resource}")
                for resource in resource_locations[planet]["advanced"]:
                    advanced_locations.append(f"Discover Resource: {resource}")
                    self.resource_checks.append(f"Discover Resource: {resource}")

                if self.options.teleporter.value and self.spaced_out:
                    for resource in resource_locations[planet]["advanced2"]:
                        advanced_locations.append(f"Discover Resource: {resource}")
                        self.resource_checks.append(f"Discover Resource: {resource}")
                    for resource in resource_locations[planet]["radbolt"]:
                        radbolt_locations.append(f"Discover Resource: {resource}")
                        self.resource_checks.append(f"Discover Resource: {resource}")

        if self.base_only == True:
            self.all_regions = [
                RegionInfo("Menu", []),
                RegionInfo(RegionNames.Basic, basic_locations),
                RegionInfo(RegionNames.Advanced, advanced_locations),
                RegionInfo(RegionNames.Space_Base, orbital_locations)
            ]
            self.all_locations = basic_locations + advanced_locations + orbital_locations
        else:
            self.all_regions = [
                RegionInfo("Menu", []),
                RegionInfo(RegionNames.Basic, basic_locations),
                RegionInfo(RegionNames.Advanced, advanced_locations),
                RegionInfo(RegionNames.Nuclear, radbolt_locations),
                RegionInfo(RegionNames.Space_DLC, orbital_locations)
            ]
            self.all_locations = basic_locations + advanced_locations + radbolt_locations + orbital_locations

        '''print(f"{current_player_name} has {len(self.all_items)} items")
        print(f"{current_player_name} has {len(basic_locations)} basic")
        print(f"{current_player_name} has {len(advanced_locations)} advanced")
        print(f"{current_player_name} has {len(radbolt_locations)} radbolt")
        print(f"{current_player_name} has {len(orbital_locations)} orbital")
        self.all_locations.sort()
        json_string = json.dumps(self.all_locations, default=lambda o: o.__dict__, indent=4)
        output_file_path = os.path.join(__file__, f"..\\location_list.json")
        with open(output_file_path, "w") as file:
            file.write(json_string)'''

    def create_regions(self) -> None:
        """Method for creating and connecting regions for the World."""
        regions_by_name = {}
        print("\n+++++ pre-create +++++")
        print(gc.get_referrers(self.multiworld))
        print("+++++++++++++++++++++")
        for region_info in self.all_regions:
            region = Region(region_info.name, self.player, self.multiworld)
            regions_by_name[region_info.name] = region
            for location_name in region_info.locations:
                #self.ap_locations[location_name] = self.location_name_to_id.get(location_name, None)
                location = ONILocation(self.player, location_name, self.location_name_to_id.get(location_name, None), region)
                region.locations.append(location)
            self.multiworld.regions.append(region)
            
        logic_options = {
            "planet": self.options.cluster.current_key,
            "spaced_out": self.spaced_out,
            "frosty": self.frosty,
            "bionic": self.bionic,
            "prehistoric": self.prehistoric
        }
        if self.base_only == True:
            logic_options["planet"] = f"{self.options.cluster_base.current_key}_base"
            regions_by_name["Menu"].connect(
                regions_by_name[RegionNames.Basic], None, None)
            regions_by_name[RegionNames.Basic].connect(
                regions_by_name[RegionNames.Advanced], None, lambda state: can_advanced_research(self.player, self.internal_item_to_name, state, logic_options))
            regions_by_name[RegionNames.Advanced].connect(
                regions_by_name[RegionNames.Space_Base], None, lambda state: can_space_research_base(self.player, self.internal_item_to_name, state, logic_options))
        else:
            regions_by_name["Menu"].connect(
                regions_by_name[RegionNames.Basic], None, None)
            regions_by_name[RegionNames.Basic].connect(
                regions_by_name[RegionNames.Advanced], None, lambda state: can_advanced_research(self.player, self.internal_item_to_name, state, logic_options))
            regions_by_name[RegionNames.Advanced].connect(
                regions_by_name[RegionNames.Nuclear], None, lambda state: can_nuclear_research(self.player, self.internal_item_to_name, state, logic_options))
            regions_by_name[RegionNames.Nuclear].connect(
                regions_by_name[RegionNames.Space_DLC], None, lambda state: can_space_research(self.player, self.internal_item_to_name, state, logic_options))
        print("\n+++++ post-create +++++")
        print(gc.get_referrers(self.multiworld))
        print("+++++++++++++++++++++")

    def create_items(self) -> None:
        """
        Method for creating and submitting items to the itempool. Items and Regions must *not* be created and submitted
        to the MultiWorld after this step. If items need to be placed during pre_fill use `get_prefill_items`.
        """
        for item in self.all_items:
            if item.itemName not in self.local_items:
                self.multiworld.itempool.append(self.create_item(item.itemName))

        item_count = len(self.all_items)
        location_count = len(self.all_locations)
        #logging.warning(f"Player: {self.multiworld.get_player_name(self.player)} Items: {item_count} Locations: {location_count}")
        if item_count < location_count:
            junk = location_count - item_count
            junk = junk - len(self.local_items)
            if self.bionic:
                junk = junk + 1
            junk_list = self.multiworld.random.choices(self.filler_item_names, k = junk)
            for junk_item in junk_list:
                self.multiworld.itempool.append(self.create_item(f"Care Package: {junk_item}"))

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
        local_items = [self.create_item(name) for name in self.local_items]
        #suits = [self.create_item(name) for name in ['Atmo Suit', 'Jet Suit Pattern', 'Oxygen Mask Pattern']]
        #if self.spaced_out == True:
        #    local_items.append(self.create_item('Lead Suit'))

        loc_dict = world.get_locations(player)
        locs = list(loc_dict)
        world.random.shuffle(locs)

        '''loc_list = []
        for location in locs:
            loc_list.append(location.name)
        loc_list.sort()
        json_string = json.dumps(loc_list, indent=4)
        output_file_path = os.path.join(__file__, f"..\\world_location_list.json")
        with open(output_file_path, "w") as file:
            file.write(json_string)'''

        fill_restrictive(world, all_state, locs, local_items, True, True, name="ONI Add Local Item")
                         
        for item in local_items:
            self.options.local_items.value.add(item.name)

    def fill_hook(self,
                  progitempool: typing.List["Item"],
                  usefulitempool: typing.List["Item"],
                  filleritempool: typing.List["Item"],
                  fill_locations: typing.List["Location"]) -> None:
        """Special method that gets called as part of distribute_items_restrictive (main fill)."""
        pass

    def post_fill(self) -> None:
        """Optional Method that is called after regular fill. Can be used to do adjustments before output generation.
        This happens before progression balancing, so the items may not be in their final locations yet."""

    def generate_output(self, output_directory: str) -> None:
        """This method gets called from a threadpool, do not use multiworld.random here.
        If you need any last-second randomization, use self.random instead."""
        # TODO generate mod json
        current_player_name = self.multiworld.get_player_name(self.player)
        #print(f"ModItems: {self.mod_items_exist}")
        location_list = self.multiworld.get_locations(self.player)
        print(f"{current_player_name} has {len(self.all_items)} items and {len(location_list)} locations. {len(self.ap_mod_items)} items are added by mods")
        for location in location_list:     # location_name = tech + location number\
            #location = self.multiworld.get_location(location_name, self.player)
            #print(location.name)
            if location.name.startswith("Discover Resource"):
                continue
            tech_name = self.location_name_to_internal[location.name]
            ap_item = location.item
            if ap_item is not None:
                output_item_name = ap_item.name
                if output_item_name in self.name_to_internal_name:
                    output_item_name = f"{self.name_to_internal_name[ap_item.name]}"
                output_item_name = f"{output_item_name}>>{ap_item.player}"
                self.science_dicts[tech_name].append(output_item_name)

        self.mod_json = ModJson(str(self.multiworld.seed), self.multiworld.player_name[self.player], self.spaced_out, self.frosty, self.bionic, self.science_dicts)
        json_string = self.mod_json.to_json(indent=4)
        output_file_path = os.path.join(output_directory, f"{self.multiworld.get_out_file_name_base(self.player)}.json")
        with open(output_file_path, "w") as file:
            file.write(json_string)

        # json_string = json.dumps(self.get_data_package_data(), default=lambda o: o.__dict__, indent=4)
        # output_file_path = os.path.join(__file__, f"..\\data_package.json")
        # with open(output_file_path, "w") as file:
        #     file.write(json_string)

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
        planet = self.options.cluster.current_key
        if self.base_only:
            planet = self.options.cluster_base.current_key
        slot_data = {
            "APWorld_Version": self.ap_version,
            "AP_seed": self.mod_json.AP_seed,
            "AP_slotName": self.mod_json.AP_slotName,
            "AP_PlayerID": self.player,
            "URL": self.mod_json.URL,
            "port": self.mod_json.port,
            "spaced_out": self.spaced_out,
            "frosty": self.frosty,
            "bionic": self.bionic,
            "teleporter": self.options.teleporter.value,
            "technologies": self.mod_json.technologies,
            "apModItems": self.ap_mod_items,
            "goal": self.options.goal.current_key,
            "planet": planet,
            "resourceChecks": self.resource_checks
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
        item = self.items_by_name[name]
        return ONIItem(item.itemName, item.progression, self.item_name_to_id[name], self.player)

    def get_filler_item_name(self) -> str:
        return self.random.choice(self.filler_item_names)
