from dataclasses import dataclass

from Options import Choice, Toggle, Range, PerGameCommonOptions


class Goal(Choice):
    """
    research_all: Complete all Research
    monument: Build the Monument
    space: Launch your first rocket
    """
    display_name = "Goal"
    option_research_all = 0
    option_monument = 1
    option_space = 2
    default = 0

class SpacedOut(Toggle):
    """
    Enable Spaced Out DLC
    """
    display_name = "Enable Spaced Out DLC"

class Frosty(Toggle):
    """
    Enable Frosty DLC
    """
    display_name = "Enable Frosty DLC"

class Bionic(Toggle):
    """
    Enable Bionic DLC
    """
    display_name = "Enable Bionic DLC"


@dataclass
class ONIOptions(PerGameCommonOptions):
    goal: Goal
    spaced_out: SpacedOut
    frosty: Frosty
    bionic: Bionic
