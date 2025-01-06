# Archipelago Not Included

## What is this?

This is a continuation of a project started by digiholic [ArchipelagoNotIncluded](https://github.com/digiholic/ArchipelagoNotIncluded)

As of right now, the bare minimum has been completed to enable Multiworld functionality. The goal is to add it to the large list of games for [Archipelago Multiworld Randomizer](https://archipelago.gg)
It is currently up-to-date with all DLC except for Bionic (which came out recently) and any combination of DLC can be enabled/disabled.
There are some known visual bugs that haven't been fixed yet, but nothing should affect gameplay. (See [Known Bugs](#known-bugs) below)

## Warning

This mod adds new information to its savegame files to keep track of Multiworld information. For this reason, the game is guaranteed to crash if you try to load a save that was not created using it. The opposite is also true, savegames made during a Multiworld can't be played outside of it.

## Usage

The randomization logic is handled by the Archipelago randomizer, so you have to have that installed to create the seed information. Archipelago will generate a mod json file that you can use with this mod to swap the technology around on startup. If this json is not present, or cannot be parsed for whatever reason, the vanilla tech tree is loaded instead. The mod does not change any of the standard planetoid or dupe generation and can work with any setting (see below note about logical requirements if you find an unwinnable scenario).

This mod should be compatible with any other mod that does not modify the tech tree in any way. Any mods that add technologies will not be included in the randomization and will not appear in your tech tree. **The ability to use mods which add technologies is a planned feature.**

### Important Note

The Research Tree currently ignores items from other games and will not display them. This does NOT affect how many items a Research topic will give you. This means if a Research topic gives 4 items in vanilla, it still gives 4 items, even if it doesn't display 4. If you don't remember how many items a Research Topic will give, I recommend using the wiki page for reference. Use either [Base Game](https://oxygennotincluded.fandom.com/wiki/Category:Research) or [Spaced Out (and later)](https://oxygennotincluded.fandom.com/wiki/Category:Research).

## Logic

In addition to requiring their research points and research buildings, each tier of research also comes with another logical requirement:
+ Advanced Research logically requires the ability to pipe fluids and gasses
+ Materials Research logically requires the ability to Refine metal, and produce Radbolts
+ Space Research logically requires the ability to build a functioning rocket and produce Data Banks

The logic might still not be completable, due to starting planetoid conditions. If this happens, please share your mod json and world seed in the Oxygen Not Included thread in the Archipelago Discord, under Future-Game-Design.

Ranching Dreckos for plastic or Hatches for Refined Metal is considered in-logic, and if necessary, buildings will be available before they are logically required.

## Seed Generation

Install Archipelago if you haven't already. It can be found [here](https://github.com/ArchipelagoMW/Archipelago/releases).  
Required files can be found on the [Releases](https://github.com/ShadowKitty42/ONI-Archipelago/releases/latest) page.

1. Copy the `oni_world.zip` file into your `libs/worlds/` directory in your Archipelago installation
2. Extract the `oni_world.zip` file and you should have a folder called `oni` (It is recommended to delete `oni_world.zip` after extracting it.)
3. Download the attached YAML and modify any options you deem necessary and place it in your Players directory in your Archipelago Installation. You can choose any combination of DLC listed in the YAML, just make sure when you start a new game that you choose the same combination. The only Goal currrently implemented is to complete the Research Tree.
4. Run "Generate" either through the executable or the AP launcher.
5. The result will be in your `Output` directory. This can be uploaded to [Archipelago Host Game](https://archipelago.gg/uploads) to use Archipelago servers as the host. Or it can be ran locally by extracting the ZIP and running the `.archipelago` file.

## Installing the Mod

1. Download the `ArchipelagoNotIncluded.zip`
2. Navigate to your ONI Mods Folder. On Windows, this is either<br/>`C:\Users\[YOUR NAME]\Documents\Klei\OxygenNotIncluded\mods` OR<br/>`C:\Users\[YOUR NAME]\OneDrive\Documents\Klei\OxygenNotIncluded\mods`
3. If there is a folder called `Local`, open it. If not, make a new folder called `Local` and then open.
4. Place the `ArchipelagoNotIncluded.zip` file in the `Local` folder and extract it. You should now have a folder called `ArchipelagoNotIncluded`.
5. Open the new `ArchipelagoNotIncluded` folder. You should see `.dll` files and most importantly, `mod.yaml` and `mod_info.yaml`. If you do, everything has been done correctly.
6. Open Oxygen Not Included.
7. Navigate to the `Mods` menu.
8. Click the empty checkbox next to `ArchipelagoNotIncluded`.
9. Close the `Mods` menu and accept the changes. The game will need to restart.
10. After the game restarts, open the `Mods` menu again.
11. Click the `Settings` button next to `ArchipelagoNotIncluded`
12. Change the settings listed to match your setup. NOTE: Player name MUST match what you put in the YAML file in step #3 of Seed Generation. (in URL, change it to `localhost` if running locally)
13. The game will need to restart to apply changes.
14. You're ready to play! Start a new game and enjoy!

Note: The mod will always use the *most recent* json in the directory. If you generate another seed, copying it over will apply that mod to all colonies from that point forward. Previous jsons are not currently deleted, but can be manually deleted if you find that you are loading the wrong one.

## Things to do

### Planned Features

- [x] ~~Add in-game options menu for changing Multiworld settings (URL and Port)~~
- [ ] Add additional locations to the game other than Research Tree
- [ ] Add Death Link Options. Current ideas for this are Random (Kill a random dupe when someone dies), Oldest (Kill oldest dupe), and Character Linked (Each dupe is linked to someone participating in Death Link)
- [ ] Maybe add support for mods that add new technologies

### Known Bugs

- [ ] When research is completed, the original tech category gets a <!> icon even if nothing was added to it.
- [ ] When research is completed, a research complete notification is generated and has incorrect information about what was unlocked.
- [ ] If your game includes items from other games, they will not appear in the Research Tree. Despite this, every Research topic still unlocks the same number of items as normal.
- [ ] Building Categories appear at the bottom even if there are no buildings listed for it.
- [ ] Trying to "Copy" the Ration Box you start the game causes a crash.