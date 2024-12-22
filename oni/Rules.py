from ast import Dict
from BaseClasses import CollectionState
from .Names import ItemNames


def can_advanced_research(player, item_list: Dict, state: CollectionState) -> bool:
    # Need to be able to actually do the research, and handle liquids and gas
    return state.has(item_list["AdvancedResearchCenter"], player) and \
           can_manage_liquid(player, item_list, state) and can_manage_gas(player, item_list, state) and \
           can_survive_basic(player, item_list, state)


def can_nuclear_research(player, item_list: Dict, state: CollectionState) -> bool:
    # Need the material science terminal, and also be able to make refined metal
    return state.has(item_list["NuclearResearchCenter"], player) and \
           state.has_any([item_list["ManualHighEnergyParticleSpawner"], item_list["HighEnergyParticleSpawner"]],
                         player) and can_refine_metal(player, item_list, state)


def can_space_research(player, item_list: Dict, state: CollectionState) -> bool:
    return state.has_all([item_list["DLC1CosmicResearchCenter"], item_list["OrbitalResearchCenter"]], player) and \
        can_reach_space(player, item_list, state) and can_make_plastic(player, item_list, state)

def can_survive_basic(player, item_list: Dict, state: CollectionState) -> bool:
    return state.has_any([item_list["PlanterBox"], item_list["FarmTile"]], player)

def can_reach_space(player, item_list: Dict, state: CollectionState) -> bool:
    # Launchpad is non-negotiable
    running_state = state.has_any([item_list["LaunchPad"]], player)
    # Has any engine and fuel tank
    running_state = running_state and state.has_any([], player)
    # Has any crew module
    running_state = running_state and state.has_any(
        [item_list["HabitatModuleSmall"], item_list["HabitatModuleMedium"]], player)
    # Has any nosecone
    running_state = running_state and state.has_any(
        [item_list["NoseconeBasic"], item_list["NoseconeHarvest"], item_list["HabitatModuleSmall"]], player)
    return running_state


def can_ranch(player, item_list: Dict, state: CollectionState) -> bool:
    return state.has_all(
        [item_list["CritterDropOff"], item_list["CreatureFeeder"], item_list["RanchStation"]], player)


def can_make_plastic(player, item_list: Dict, state: CollectionState) -> bool:
    # Either polymer press chain, or ranching dreckos
    return state.has_all([item_list["OilWellCap"], item_list["OilRefinery"], item_list["Polymerizer"]], player) or \
           (can_ranch(player, item_list, state) and state.has(item_list["ShearingStation"], player))


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
    running_state = running_state and state.has(item_list["GasVent"], player) or \
                    (state.has(item_list["GasVentHighPressure"], player) and
                     can_make_plastic(player, item_list, state) and can_refine_metal(player, item_list, state))
    return running_state
