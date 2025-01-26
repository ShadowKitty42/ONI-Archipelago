import json
import typing
from dataclasses import dataclass

from .ArchipelagoItem import APItem

class ModJson:
    """
    Class representing the JSON file the Archipelago Not Included Mod expects to use.
    Will be serialized to json and put in output directory
    """
    AP_seed: str
    AP_slotName: str
    URL: str
    port: int
    spaced_out: bool
    frosty: bool
    bionic: bool
    technologies: typing.Dict[str, typing.List[str]]

    def __init__(self, seed, slot, space, frost, bionic, tech):
        self.AP_seed = seed
        self.AP_slotName = slot
        self.spaced_out = space
        self.frosty = frost
        self.bionic = bionic
        self.technologies = tech
        self.URL = "archipelago.gg"
        self.port = 38281

    def to_json(self, indent):
        return json.dumps(self, default=lambda o: o.__dict__,
                          sort_keys=True, indent=indent)




class APJson:
    """
    Class representing the JSON file the Archipelago Not Included Mod expects to use.
    Will be serialized to json and put in output directory
    """
    items: typing.List[APItem]

    def __init__(self, itemList):
        self.items = itemList

    def to_json(self, indent):
        return json.dumps(self, default=lambda o: o.__dict__,
                          sort_keys=True, indent=indent)
    

class APLocationJson:
    """
    Class representing the JSON file the Archipelago Not Included Mod expects to use.
    Will be serialized to json and put in output directory
    """
    locations: typing.Dict[str, int]

    def __init__(self, locationList):
        self.locations = locationList

    def to_json(self, indent):
        return json.dumps(self, default=lambda o: o.__dict__,
                          sort_keys=True, indent=indent)
