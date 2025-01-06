import typing

from BaseClasses import Location, Region

class DefaultItem:
    def __init__(self, name, internal_name, research_level, research_level_base, tech, tech_base, internal_tech, internal_tech_base, ap_classification, version):
        self.name = name
        self.internal_name = internal_name
        self.research_level = research_level
        self.research_level_base = research_level_base
        self.tech = tech
        self.tech_base = tech_base
        self.internal_tech = internal_tech
        self.internal_tech_base = internal_tech_base
        self.ap_classification = ap_classification
        self.version = version