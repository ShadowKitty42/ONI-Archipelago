# Archipelago Not Included

## What is this?

This is a continuation of a project started by digiholic [ArchipelagoNotIncluded](https://github.com/digiholic/ArchipelagoNotIncluded)

In its current state, it's basically a Research Tree Randomizer for Oxygen Not Included. The goal is to add it to the large list of games for [Archipelago Multiworld Randomizer](https://archipelago.gg)
Right now, it is currently up-to-date with all DLC except for Bionic (which came out recently) and any combination of DLC can be enabled/disabled.

## Warning

This mod affects the entire Research Tree regardless of what save file you load. Loading a pre-existing save will not cause permanent damage, but saving afterward could. Therefore it is not recommended to load any existing saves while the mod is enabled.

## Usage

The randomization logic is handled by the Archipelago randomizer, so you have to have that installed to create the seed information. Archipelago will generate a mod json file that you can use with this mod to swap the technology around on startup. If this json is not present, or cannot be parsed for whatever reason, the vanilla tech tree is loaded instead. The mod does not change any of the standard planetoid or dupe generation and can work with any setting (see below note about logical requirements if you find an unwinnable scenario).

This mod should be compatible with any other mod that does not modify the tech tree in any way. Any mods that add technologies will not be included in the randomization and will not appear in your tech tree. **The ability to use mods which add technologies is a planned feature.**

## Logic

In addition to requiring their research points and research buildings, each tier of research also comes with another logical requirement:
+ Advanced Research logically requires the ability to pipe fluids and gasses
+ Materials Research logically requires the ability to Refine metal, and produce Radbolts
+ Space Research logically requires the ability to build a functioning rocket and produce Data Banks

The logic might still not be completable, due to starting planetoid conditions. If this happens, please share your mod json and world seed in the Oxygen Not Included thread in the Archipelago Discord, under Future-Game-Design.

Ranching Dreckos for plastic or Hatches for Refined Metal is considered in-logic, and if necessary, buildings will be available before they are logically required.

## Seed Generation

Install Archipelago if you haven't already. It can be found [here](https://github.com/ArchipelagoMW/Archipelago/releases).
Required files can be found on the [Releases](https://github.com/ShadowKitty42/ONI-Archipelago/releases) page.

1. Place the `oni.apworld` file in your `libs/worlds/` directory in your Archipelago installation
2. Download the attached YAML and modify any options you deem necessary and place it in your Players directory in your Archipelago Installation. You can choose any combination of DLC listed in the YAML, just make sure when you start a new game that you choose the same combination. Currently Goals are not implemented.
3. Run "Generate" either through the executable or the AP launcher.
4. In your `Output` directory, extract the ZIP and find the `.json` file generated. This will be needed in "Installing the mod" below.

## Installing the Mod

1. Download the `ArchipelagoNotIncluded.zip`
2. Navigate to your ONI Mods Folder. On Windows, this is either `C:\Users\[YOUR NAME]\Documents\Klei\OxygenNotIncluded\mods` OR `C:\Users\[YOUR NAME]\OneDrive\Documents\Klei\OxygenNotIncluded\mods`
3. If there is a folder called `Local`, open it. If not, make a new folder called `Local` and then open.
4. Place the `ArchipelagoNotIncluded.zip` file in the `Local` folder and extract it. You should now have a folder called `ArchipelagoNotIncluded`.
5. Open the new `ArchipelagoNotIncluded` folder. You should see `.dll` files and most importantly, `mod.yaml` and `mod_info.yaml`. If you do, everything has been done correctly.
6. Copy the `.json` file from the end of "Seed Generation" and paste it in this folder.
7. You're ready to play! Open Oxygen Not Included.

Note: The mod will always use the *most recent* json in the directory. If you generate another seed, copying it over will apply that mod to all colonies from that point forward. Previous jsons are not currently deleted, but can be manually deleted if you find that you are loading the wrong one.