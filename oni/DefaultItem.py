import typing

from BaseClasses import Location, Region

class DefaultItem:
    def __init__(self, name, internal_name, research_level, tech, internal_tech, ap_classification):
        self.name = name
        self.internal_name = internal_name
        self.research_level = research_level
        self.tech = tech
        self.internal_tech = internal_tech
        self.ap_classification = ap_classification