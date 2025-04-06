from ast import Dict
from BaseClasses import CollectionState
from .Options import ONIOptions
from .Names import ItemNames


def can_advanced_research(player, item_list: Dict, state: CollectionState, player_options: ONIOptions) -> bool:
    # Need to be able to actually do the research, and handle liquids and gas
    running_state = state.has(item_list["AdvancedResearchCenter"], player) 
    running_state = running_state and can_manage_liquid(player, item_list, state) and can_manage_gas(player, item_list, state)
    # Player has basic survival buildings
    running_state = running_state and can_survive_basic(player, item_list, state, player_options)
    return running_state


def can_nuclear_research(player, item_list: Dict, state: CollectionState, player_options: ONIOptions) -> bool:
    # Need the material science terminal, and also be able to make refined metal
    return state.has(item_list["NuclearResearchCenter"], player) and \
           state.has_any([item_list["ManualHighEnergyParticleSpawner"], item_list["HighEnergyParticleSpawner"]],
                         player) and can_refine_metal(player, item_list, state)


def can_space_research(player, item_list: Dict, state: CollectionState, player_options: ONIOptions) -> bool:
    return state.has(item_list["DLC1CosmicResearchCenter"], player) and can_make_databanks(player, item_list, state, player_options)

def can_space_research_base(player, item_list: Dict, state: CollectionState, player_options: ONIOptions) -> bool:
    return state.has_all([item_list["CosmicResearchCenter"], item_list["ResearchModule"],
                          item_list["Telescope"]], player) and can_reach_space_base(player, item_list, state)

def can_survive_basic(player, item_list: Dict, state: CollectionState, player_options: ONIOptions) -> bool:
    running_state = state.has_any([item_list["PlanterBox"], item_list["FarmTile"]], player)
    if on_frosty_planet(player_options):
        running_state = running_state and state.has_all([item_list["IceKettle"], item_list["WoodTile"]], player)
    if player_options.bionic:
        running_state = running_state and state.has_all([item_list["Apothecary"], item_list["LubricationStick"]], player)
    return running_state

def can_reach_space_base(player, item_list: Dict, state: CollectionState) -> bool:
    # Command Capsule is non-negotiable
    running_state = state.has_any([item_list["CommandModule"]], player)
    # Has any engine and fuel tank
    running_state = running_state and (state.has(item_list["SteamEngine"], player) or \
        (state.has_any([item_list["KeroseneEngine"], item_list["HydrogenEngine"]], player) and \
        state.has(item_list["LiquidFuelTank"], player) and \
        state.has_any([item_list["OxidizerTank"], item_list["OxidizerTankLiquid"]], player)))
    return running_state

def can_reach_space(player, item_list: Dict, state: CollectionState) -> bool:
    # Launchpad is non-negotiable
    running_state = state.has_any([item_list["LaunchPad"]], player)
    # Has any engine and fuel tank
    running_state = running_state and (state.has_any([item_list["CO2Engine"], item_list["HEPEngine"], 
                                                      item_list["SteamEngineCluster"]], player) or \
       (state.has_any([item_list["SugarEngine"], item_list["KeroseneEngineClusterSmall"]], player) and \
       has_spacedout_oxidizer(player, item_list, state)) or \
       (state.has_any([item_list["KeroseneEngineCluster"], item_list["HydrogenEngineCluster"]], player) and \
       state.has(item_list["LiquidFuelTankCluster"], player) and has_spacedout_oxidizer(player, item_list, state)))

    # Has any crew module
    running_state = running_state and (state.has(item_list["HabitatModuleSmall"], player) or \
                    (state.has_any([item_list["NoseconeBasic"], item_list["NoseconeHarvest"]], player) and \
                    state.has(item_list["HabitatModuleMedium"], player)))
    return running_state

def has_spacedout_oxidizer(player, item_list: Dict, state: CollectionState) -> bool:
    return state.has_any([item_list["SmallOxidizerTank"], item_list["OxidizerTankLiquidCluster"], item_list["OxidizerTankCluster"]], player)

def can_ranch(player, item_list: Dict, state: CollectionState) -> bool:
    return state.has_all([item_list["CritterDropOff"], item_list["CreatureFeeder"], item_list["RanchStation"]], player)


def can_make_plastic(player, item_list: Dict, state: CollectionState) -> bool:
    # Oil -> Petroleum -> Plastic
    running_state = state.has_all([item_list["OilWellCap"], item_list["OilRefinery"], item_list["Polymerizer"]], player)
    # Drecko Ranching
    running_state = running_state or (can_ranch(player, item_list, state) and state.has(item_list["ShearingStation"], player))
    # Nectar from BonBon Trees
    if on_frosty_planet:
        running_state = running_state or state.has(item_list["Polymerizer"], player)
    return running_state

def can_make_databanks(player, item_list: Dict, state: CollectionState, player_options: ONIOptions) -> bool:
    running_state = can_reach_space(player, item_list, state) and state.has(item_list["OrbitalResearchCenter"], player)
    if player_options.bionic:
        running_state = running_state or state.has(item_list["DataMiner"], player)
    running_state = running_state and can_make_plastic(player, item_list, state)
    return running_state

def can_refine_metal(player, item_list: Dict, state: CollectionState) -> bool:
    # Crusher, Refinery, or Smooth Hatches
    return state.has_any([item_list["RockCrusher"], item_list["MetalRefinery"]], player) or can_ranch(player, item_list, state)


def can_manage_liquid(player, item_list: Dict, state: CollectionState) -> bool:
    # Some form of liquid pump
    running_state = state.has(item_list["LiquidPump"], player) or \
                    (state.has(item_list["LiquidMiniPump"], player) and can_make_plastic(player, item_list, state))
    # Some form of liquid pipe
    running_state = running_state and (
            state.has_any([item_list["LiquidConduit"], item_list["InsulatedLiquidConduit"]], player) or
            state.has(item_list["LiquidConduitRadiant"], player) and can_refine_metal(player, item_list, state))
    # Liquid Vent
    running_state = running_state and state.has(item_list["LiquidVent"], player)
    return running_state


def can_manage_gas(player, item_list: Dict, state: CollectionState) -> bool:
    # Some form of gas pump
    running_state = state.has(item_list["GasPump"], player) or \
                    (state.has(item_list["GasMiniPump"], player) and can_make_plastic(player, item_list, state))
    # Some form of gas pipe
    running_state = running_state and (state.has_any(
        [item_list["GasConduit"], item_list["InsulatedGasConduit"], item_list["GasConduitRadiant"]], player))
    # Some form of gas vent
    running_state = running_state and (state.has(item_list["GasVent"], player) or \
                    (state.has(item_list["GasVentHighPressure"], player) and
                     can_make_plastic(player, item_list, state) and can_refine_metal(player, item_list, state)))
    return running_state

def on_frosty_planet(options: ONIOptions) -> bool:
    if options.cluster == "ceres" or options.cluster == "ceres_minor" or options.cluster == "ceres_mantle":
        return True
    return False