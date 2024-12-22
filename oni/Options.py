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


@dataclass
class ONIOptions(PerGameCommonOptions):
    goal: Goal
