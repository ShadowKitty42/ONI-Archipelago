import typing

from .Locations import LocationNames, basic_locations, advanced_locations, radbolt_locations, orbital_locations
from .Names import RegionNames


class RegionInfo(typing.NamedTuple):
    name: str
    locations: typing.List[str]


all_regions = [
    RegionInfo("Menu", []),
    RegionInfo(RegionNames.Basic, basic_locations),
    RegionInfo(RegionNames.Advanced, advanced_locations),
    RegionInfo(RegionNames.Nuclear, radbolt_locations),
    RegionInfo(RegionNames.Space_DLC, orbital_locations)
]

regions_by_name: typing.Dict[str, RegionInfo] = {region.name: region for region in all_regions}
