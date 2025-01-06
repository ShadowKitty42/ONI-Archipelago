import typing

from BaseClasses import Location, Region

class APItem():
    def __init__(self, itemId, typeClass):
        self.code = itemId
        self.classification = typeClass