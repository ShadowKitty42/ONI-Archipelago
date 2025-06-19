from dataclasses import dataclass
import random

from Options import Choice, Toggle, DefaultOnToggle, Range, PerGameCommonOptions

class Goal(Choice):
    """
    research_all: Complete all Research
    monument: Build the Monument
    space: Launch your first rocket
    """
    display_name = "Goal"
    option_launch_rocket = 0
    option_monument = 1
    option_research_all = 2
    option_home_sweet_home = 3
    option_great_escape = 4
    option_cosmic_archaeology = 5
    default = 2

class SpacedOut(DefaultOnToggle):
    """
    Enable Spaced Out DLC
    """
    display_name = "Enable Spaced Out DLC"

class Frosty(DefaultOnToggle):
    """
    Enable Frosty DLC
    """
    display_name = "Enable Frosty DLC"

class Bionic(Toggle):
    """
    Enable Bionic DLC
    """
    display_name = "Enable Bionic DLC"

class ResourceChecks(DefaultOnToggle):
    """
    Add location checks to newly collected resources
    """
    display_name = "Enable Resource Checks"

class ClusterBase(Choice):
    """
    Choose your starting planet for Base Game
    Names are based on the in-game name in planet selection menu
    This option is ignored if you're using Spaced Out DLC
    """
    display_name = "Starting Planet (Base Game)"
    option_terra = 0
    option_ceres = 1
    option_oceania = 2
    option_rime = 3
    option_verdante = 4
    option_arboria = 5
    option_volcanea = 6
    option_badlands = 7
    option_aridio = 8
    option_oasisse = 9

    option_skewed = 10
    option_blasted = 11

    option_custom = 50
    default = 0

class Cluster(Choice):
    """
    Choose your starting planet for Spaced Out
    Names are based on the in-game name in planet selection menu
    This option is ignored if you're NOT using Spaced Out DLC
    """
    display_name = "Starting Planet"
    option_terrania = 0
    option_ceres_minor = 1
    option_folia = 2
    option_quagmiris = 3
    option_metallic_swampy = 4
    option_desolands = 5
    option_frozen_forest = 6
    option_flipped = 7
    option_radioactive_ocean = 8
    option_ceres_mantle = 9

    option_terra = 10
    option_ceres = 11
    option_oceania = 12
    option_rime = 13
    option_verdante = 14
    option_arboria = 15
    option_volcanea = 16
    option_badlands = 17
    option_aridio = 18
    option_oasisse = 19
    option_squelchy = 20

    option_skewed = 21
    option_blasted = 22

    option_custom = 50
    option_random_classic = 51
    option_random_spaced_out = 52
    default = 10

    @property
    def has_basegame_equivalent(self) -> bool:
        if self.value >= 10 and self.value < 20:
            return True
        return False

    @classmethod
    def planet_type(cls, value: int) -> str:
        if value >= 0 and value < 10:
            return "spaced_out"
        if value >= 10 and value < 21:
            return "classic"
        if value >= 21 and value < 23:
            return "the_lab"

    @classmethod
    def from_text(cls, text: str) -> Choice:
        text = text.lower()
        if text == "random_classic":
            choice_list = [option for option in cls.name_lookup if cls.planet_type(option) == "classic"]
            return cls(random.choice(choice_list))
        if text == "random_spaced_out":
            choice_list = [option for option in cls.name_lookup if cls.planet_type(option) == "spaced_out"]
            return cls(random.choice(choice_list))
        return super().from_text(text)

class Teleporter(Toggle):
    """
    Enable resource checks on second planet, accessible via teleporter
    This option doesn't do anything if you're NOT using Spaced Out DLC
    """
    display_name = "Enable Resource checks on Teleporter planet"

@dataclass
class ONIOptions(PerGameCommonOptions):
    goal: Goal
    spaced_out: SpacedOut
    frosty: Frosty
    bionic: Bionic
    resource_checks: ResourceChecks
    cluster_base: ClusterBase
    cluster: Cluster
    teleporter: Teleporter
    