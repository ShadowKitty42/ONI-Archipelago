using Database;
using HarmonyLib;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using static SeedProducer;
using System;
using System.Reflection.Emit;
using KMod;
using System.Runtime.CompilerServices;
using UnityEngine.LowLevel;
using UnityEngineInternal;
using UnityEngine.PlayerLoop;
using static MathUtil;
using System.Runtime.InteropServices;
using Archipelago.MultiClient.Net.DataPackage;
using Archipelago.MultiClient.Net.Models;
using System.Collections;
using System.Text;
using UtilLibs;
using Epic.OnlineServices.Platform;
using PeterHan.PLib.Options;
using ProcGen;
using TMPro;
using Klei.CustomSettings;
using UnityEngine.Device;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net;
using Newtonsoft.Json.Converters;
using Klei.AI;

namespace ArchipelagoNotIncluded
{

    public class Patches
    {
        [HarmonyPatch(typeof(DestinationSelectPanel))]
        [HarmonyPatch(nameof(DestinationSelectPanel.UpdateDisplayedClusters))]
        public static class UpdateDisplayedClusters_Patch
        {
            public static bool Prefix(DestinationSelectPanel __instance)
            {
                if (ArchipelagoNotIncluded.Options.CreateModList)
                    return true;

                string cluster = string.Empty;
                if (ArchipelagoNotIncluded.info != null)
                {
                    string planet = ArchipelagoNotIncluded.info.planet;
                    if (DlcManager.IsExpansion1Active())
                    {
                        if (ArchipelagoNotIncluded.ClassicPlanets.ContainsKey(planet))
                            cluster = ArchipelagoNotIncluded.ClassicPlanets[planet];
                        else if (ArchipelagoNotIncluded.SpacedOutPlanets.ContainsKey(planet))
                            cluster = ArchipelagoNotIncluded.SpacedOutPlanets[planet];
                        else if (ArchipelagoNotIncluded.ClassicLabPlanets.ContainsKey(planet))
                            cluster = ArchipelagoNotIncluded.ClassicLabPlanets[planet];
                    }
                    else
                    {
                        if (ArchipelagoNotIncluded.BasePlanets.ContainsKey(planet))
                            cluster = ArchipelagoNotIncluded.BasePlanets[planet];
                        else if (ArchipelagoNotIncluded.BaseLabPlanets.ContainsKey(planet))
                            cluster = ArchipelagoNotIncluded.BaseLabPlanets[planet];
                    }
                    if (!cluster.IsNullOrWhiteSpace())
                    {
                        __instance.clusterKeys.Clear();
                        __instance.clusterStartWorlds.Clear();
                        __instance.asteroidData.Clear();
                        __instance.clusterKeys.Add(cluster);
                        string layout = SettingsCache.clusterLayouts.clusterCache[cluster].GetStartWorld();
                        ColonyDestinationAsteroidBeltData value = new ColonyDestinationAsteroidBeltData(layout, 0, cluster);
                        __instance.asteroidData[cluster] = value;
                        __instance.clusterStartWorlds.Add(cluster, layout);

                        // Prevent UI crashes caused by having 1 planet listed
                        __instance.dragTarget.onBeginDrag -= new System.Action(__instance.BeginDrag);
                        __instance.dragTarget.onDrag -= new System.Action(__instance.Drag);
                        __instance.dragTarget.onEndDrag -= new System.Action(__instance.EndDrag);
                        __instance.leftArrowButton.gameObject.SetActive(false);
                        __instance.rightArrowButton.gameObject.SetActive(false);
                    }
                }
                foreach (string clusterKey in SettingsCache.clusterLayouts.clusterCache.Keys)
                    Debug.Log($"Cluster name: {clusterKey}, {Strings.Get(SettingsCache.clusterLayouts.clusterCache[clusterKey].name)}");
                /*foreach (string clusterName in SettingsCache.GetClusterNames())
                    Debug.Log($"Cluster name: {clusterName}");
                foreach (string world in SettingsCache.GetWorldNames())
                    Debug.Log($"World name: {world}");*/

                ArchipelagoNotIncluded.ItemList.Clear();
                ArchipelagoNotIncluded.ItemListDetailed.Clear();
                ArchipelagoNotIncluded.DebugWasUsed = false;
                ArchipelagoNotIncluded.lastIndexSaved = 0;
                ArchipelagoNotIncluded.runCount = 0;
                ArchipelagoNotIncluded.planetText = string.Empty;
                return cluster.IsNullOrWhiteSpace();
            }
        }

        [HarmonyPatch(typeof(ClusterCategorySelectionScreen))]
        [HarmonyPatch(nameof(ClusterCategorySelectionScreen.OnSpawn))]
        public static class Cluster_OnSpawn_Patch
        {
            public static void Postfix(ClusterCategorySelectionScreen __instance)
            {
                ArchipelagoNotIncluded.AllowResourceChecks = true;
                __instance.closeButton.onClick += new System.Action(DisallowResourceChecks);
                if (ArchipelagoNotIncluded.info == null)
                    return;
                if (DlcManager.IsExpansion1Active())
                {
                    if (ArchipelagoNotIncluded.ClassicPlanets.ContainsKey(ArchipelagoNotIncluded.info.planet))
                    {
                        __instance.eventStyle.button.gameObject.SetActive(false);
                        __instance.spacedOutStyle.button.gameObject.SetActive(false);
                    }
                    else if (ArchipelagoNotIncluded.SpacedOutPlanets.ContainsKey(ArchipelagoNotIncluded.info.planet))
                    {
                        __instance.classicStyle.button.gameObject.SetActive(false);
                        __instance.eventStyle.button.gameObject.SetActive(false);
                    }
                    else if (ArchipelagoNotIncluded.ClassicLabPlanets.ContainsKey(ArchipelagoNotIncluded.info.planet))
                    {
                        __instance.classicStyle.button.gameObject.SetActive(false);
                        __instance.spacedOutStyle.button.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (ArchipelagoNotIncluded.BasePlanets.ContainsKey(ArchipelagoNotIncluded.info.planet))
                    {
                        __instance.eventStyle.button.gameObject.SetActive(false);
                    }
                    else if (ArchipelagoNotIncluded.BaseLabPlanets.ContainsKey(ArchipelagoNotIncluded.info.planet))
                    {
                        __instance.vanillaStyle.button.gameObject.SetActive(false);
                    }
                }
            }
            public static void DisallowResourceChecks()
            {
                ArchipelagoNotIncluded.AllowResourceChecks = false;
            }
        }

        [HarmonyPatch(typeof(ColonyDestinationSelectScreen))]
        [HarmonyPatch(nameof(ColonyDestinationSelectScreen.LaunchClicked))]
        public static class LaunchClicked_Patch
        {
            public static void Postfix()
            {
                APSeedInfo info = ArchipelagoNotIncluded.info;
                if (ArchipelagoNotIncluded.Options.CreateModList || info == null)
                    return;
                if (info.teleporter)
                    CustomGameSettings.Instance.SetQualitySetting(CustomGameSettingConfigs.Teleporters, "Enabled");

                CustomGameSettings cgs = CustomGameSettings.Instance;
                foreach (KeyValuePair<string, SettingConfig> keyValuePair in cgs.MixingSettings)    // DLC2_ID  DLC3_ID
                {
                    DlcMixingSettingConfig dlcSetting = keyValuePair.Value as DlcMixingSettingConfig;
                    if (dlcSetting != null)
                    {
                        switch (dlcSetting.id)
                        {
                            case DlcManager.DLC2_ID:
                                if (!info.frosty)
                                    cgs.SetMixingSetting(dlcSetting, "Disabled");
                                else
                                    cgs.SetMixingSetting(dlcSetting, "Enabled");
                                break;
                            case DlcManager.DLC3_ID:
                                if (!info.bionic)
                                    cgs.SetMixingSetting(dlcSetting, "Disabled");
                                else
                                    cgs.SetMixingSetting(dlcSetting, "Enabled");
                                break;
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(MainMenu))]
        [HarmonyPatch(nameof(MainMenu.NewGame))]
        public static class NewGame_Patch
        {
            public static bool Prefix(MainMenu __instance)
            {
                var dialogue = ((ConfirmDialogScreen)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, Global.Instance.globalCanvas));
                string text = "No connection to Archipelago. New game will proceed without it.\n(If you're not trying to connect, just ignore this.)";
                string title = "Archipelago";
                System.Action confirm = null;
                System.Action cancel = new System.Action(() => __instance.FindOrAdd<NewGameFlow>().ClearCurrentScreen());
                if (ArchipelagoNotIncluded.netmon.session.Socket.Connected)
                {
                    text = "Successfully connected to Archipelago.";
                    cancel = null;
                }

                DlcManager.DlcInfo dlc2 = DlcManager.DLC_PACKS["DLC2_ID"];
                DlcManager.DlcInfo dlc3 = DlcManager.DLC_PACKS["DLC3_ID"];
                if (ArchipelagoNotIncluded.info?.frosty == true && !DlcManager.IsContentSubscribed(dlc2.id))
                {
                    text = "\nFrosty DLC is enabled in Archipelago but not detected.\nPlease confirm DLC installation before trying to start a new game.";
                    title = Strings.Get(dlc2.dlcTitle);
                    confirm = new System.Action(cancel);

                }
                if (ArchipelagoNotIncluded.info?.bionic == true && !DlcManager.IsContentSubscribed(dlc3.id))
                {
                    text = "\nBionic DLC is enabled in Archipelago but not detected.\nPlease confirm DLC installation before trying to start a new game.";
                    title = Strings.Get(dlc3.dlcTitle);
                    confirm = new System.Action(cancel);

                }
                dialogue.PopupConfirmDialog(text, confirm, cancel, title_text: title);
                return true;
            }
        }

        [HarmonyPatch(typeof(PauseScreen))]
        [HarmonyPatch(nameof(PauseScreen.RefreshDLCButton))]
        public static class RefreshDLCButton_Patch
        {
            public static void Postfix(MultiToggle button)
            {
                button.onClick = null;
            }
        }

        [HarmonyPatch(typeof(DiscoveredResources))]
        [HarmonyPatch(nameof(DiscoveredResources.Discover), new[] { typeof(Tag), typeof(Tag) })]
        public static class Discover_Patch
        {
            public static void Prefix(DiscoveredResources __instance, out int __state)
            {
                __state = __instance.newDiscoveries.Count;
            }

            public static void Postfix(DiscoveredResources __instance, Tag tag, int __state)
            {
                if (APSaveData.Instance.AllowResourceChecks && __instance.newDiscoveries.Count > __state && ArchipelagoNotIncluded.info != null)      // New Discovery was added
                {
                    string ResourceName = tag.ProperNameStripLink();
                    Debug.Log($"New Discovery: Name: {tag.Name}, StripLink: {ResourceName}");
                    //foreach (string resource in ArchipelagoNotIncluded.info.resourceChecks)
                    //Debug.Log(resource);
                    string location = $"Discover Resource: {ResourceName}";
                    if (ArchipelagoNotIncluded.info.resourceChecks.Contains(location))
                        ArchipelagoNotIncluded.AddLocationChecks(location);
                }
            }
        }

        [HarmonyPatch(typeof(Pickupable))]
        [HarmonyPatch(nameof(Pickupable.OnSpawn))]
        public static class Pickupable_OnSpawn_Patch
        {
            public static void Postfix(Pickupable __instance)
            {
                if (DebugHandler.InstantBuildMode)
                {
                    ArchipelagoNotIncluded.DebugWasUsed = true;
                    string element = __instance.primaryElement.ElementID.ToString();
                    string Names;
                    List<string> NamesList =
                    [
                        "Unobtanium", "OxyRock", "Snow", "SolidCarbonDioxide", "CrushedIce", "BrineIce", "Slickster", "SolidCrudeOil"
                    ];
                    if (NamesList.Contains(element) || NamesList.Contains(TagManager.StripLinkFormatting(__instance.GetProperName())))
                        return;
                    if (element == "Creature")
                        Names = $"{TagManager.StripLinkFormatting(__instance.GetProperName())} ({__instance.KPrefabID.name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0]}), ";
                    else
                        Names = $"{TagManager.StripLinkFormatting(__instance.GetProperName())} ({element}), ";
                    if (!ArchipelagoNotIncluded.ItemListDetailed.Contains(Names))
                    {
                        ArchipelagoNotIncluded.ItemList.Add(TagManager.StripLinkFormatting(__instance.GetProperName()));
                        ArchipelagoNotIncluded.ItemListDetailed.Add(Names);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(InterfaceTool))]
        [HarmonyPatch(nameof(InterfaceTool.ToggleConfig))]
        public static class ToggleConfig_Patch
        {
            public static void Postfix(Action configKey)
            {
                if (ArchipelagoNotIncluded.DebugWasUsed && configKey == Action.DebugInstantBuildMode)
                {
                    ArchipelagoNotIncluded.DebugWasUsed = false;
                    using (FileStream fs = File.Open(ArchipelagoNotIncluded.modDirectory.ToString() + $"\\ItemList.txt", FileMode.Append))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            ArchipelagoNotIncluded.runCount++;
                            switch (ArchipelagoNotIncluded.runCount)
                            {
                                case 1:
                                    ArchipelagoNotIncluded.planetText = "\t\"Planet\": {\n";
                                    ArchipelagoNotIncluded.planetText += "\t\t\"basic\": [\n\t\t\t\"Water\", \"Polluted Water\", ";
                                    break;
                                case 2:
                                    ArchipelagoNotIncluded.planetText += "\n\t\t],\n";
                                    ArchipelagoNotIncluded.planetText += "\t\t\"advanced\": [\n\t\t\t";
                                    break;
                                case 3:
                                    ArchipelagoNotIncluded.planetText += "\n\t\t],\n";
                                    ArchipelagoNotIncluded.planetText += "\t\t\"advanced2\": [\n\t\t\t";
                                    break;
                                case 4:
                                    ArchipelagoNotIncluded.planetText += "\n\t\t],\n";
                                    ArchipelagoNotIncluded.planetText += "\t\t\"radbolt\": [\n\t\t\t";
                                    break;
                            }
                            for (; ArchipelagoNotIncluded.lastIndexSaved < ArchipelagoNotIncluded.ItemList.Count; ArchipelagoNotIncluded.lastIndexSaved++)
                            {
                                ArchipelagoNotIncluded.planetText += $"\"{ArchipelagoNotIncluded.ItemList[ArchipelagoNotIncluded.lastIndexSaved]}\", ";
                                sw.Write(ArchipelagoNotIncluded.ItemListDetailed[ArchipelagoNotIncluded.lastIndexSaved]);
                            }
                            sw.Write("\n\n");
                            if (!DlcManager.IsExpansion1Active() && ArchipelagoNotIncluded.runCount == 2)
                            {
                                ArchipelagoNotIncluded.planetText += "\n\t\t]\n\t},";
                                sw.Write(ArchipelagoNotIncluded.planetText);
                            }
                            if (ArchipelagoNotIncluded.runCount == 4)
                            {
                                ArchipelagoNotIncluded.planetText += "\n\t\t]\n\t},";
                                sw.Write(ArchipelagoNotIncluded.planetText);
                            }
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(SandboxToolParameterMenu))]
        [HarmonyPatch(nameof(SandboxToolParameterMenu.OnSpawn))]
        public static class Sandbox_Patch
        {
            public static bool Prefix(SandboxToolParameterMenu __instance)
            {
                __instance.brushRadiusSlider = new SandboxToolParameterMenu.SliderValue(1f, 50f, "dash", "circle_hard", "", UI.SANDBOXTOOLS.SETTINGS.BRUSH_SIZE.TOOLTIP, UI.SANDBOXTOOLS.SETTINGS.BRUSH_SIZE.NAME, delegate (float value)
                {
                    SandboxToolParameterMenu.instance.settings.SetIntSetting("SandboxTools.BrushSize", Mathf.Clamp(Mathf.RoundToInt(value), 1, 50));
                }, 0);
                return true;
            }
        }

        [HarmonyPatch(typeof(ColonyAchievementTracker))]
        [HarmonyPatch(nameof(ColonyAchievementTracker.TriggerNewAchievementCompleted))]
        public static class TriggerNewAchievementCompleted_Patch
        {
            public static void Postfix(string achievement)
            {
                if (ArchipelagoNotIncluded.info == null)
                    return;

                string goal = ArchipelagoNotIncluded.info.goal;
                APSaveData inst = APSaveData.Instance;
                Debug.Log($"New Achievement: {achievement} Goal: {goal}");
                switch (achievement)
                {
                    case "CompleteResearchTree":
                        if (goal == "research_all")
                            inst.GoalComplete = true;
                        goto default;
                    case "space_race":
                        if (goal == "launch_rocket")
                            inst.GoalComplete = true;
                        goto default;
                    case "thriving":
                        if (goal == "home_sweet_home")
                            inst.GoalComplete = true;
                        goto default;
                    case "ReachedDistantPlanet":
                        if (goal == "great_escape")
                            inst.GoalComplete = true;
                        goto default;
                    case "CollectedArtifacts":
                        if (goal == "cosmic_archaeology")
                            inst.GoalComplete = true;
                        goto default;
                    default:
                        break;
                }
            }
        }

        [HarmonyPatch(typeof(MonumentPart))]
        [HarmonyPatch(nameof(MonumentPart.IsMonumentCompleted))]
        public static class Monument_Patch
        {
            public static void Postfix(bool __result)
            {
                if (ArchipelagoNotIncluded.info == null)
                    return;

                if (ArchipelagoNotIncluded.info.goal == "monument" && __result)
                    APSaveData.Instance.GoalComplete = true;

            }
        }

        [HarmonyPatch(typeof(ChemicalRefineryConfig))]
        [HarmonyPatch(nameof(ChemicalRefineryConfig.ConfigureBuildingTemplate))]
        public static class ChemicalRefineryConfig_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                CodeMatcher matcher = new CodeMatcher(instructions);
                MethodInfo dbGet = AccessTools.Method(typeof(Db), nameof(Db.Get));
                FieldInfo techItems = AccessTools.Field(typeof(TechItems), nameof(Db.TechItems));
                FieldInfo superLiquids = AccessTools.Field(typeof(TechItem), nameof(Db.TechItems.superLiquids));
                FieldInfo parentTechId = AccessTools.Field(typeof(TechItem), nameof(TechItem.parentTechId));

                matcher.MatchStartForward(
                        new CodeMatch(OpCodes.Call),
                        new CodeMatch(OpCodes.Ldfld, techItems),
                        new CodeMatch(OpCodes.Ldfld, superLiquids),
                        new CodeMatch(OpCodes.Ldfld, parentTechId)
                    )
                    .Repeat(matcher =>
                    {
                        matcher.RemoveInstructions(4);
                        matcher.Insert(new CodeInstruction(OpCodes.Ldstr, "Jobs"));
                    },
                    notFoundMessage =>
                    {
                        Debug.LogError($"No matches found: {notFoundMessage}");
                    });
                return matcher.InstructionEnumeration();
            }
        }

        /*[HarmonyPatch(typeof(GameplaySeasonInstance), nameof(GameplaySeasonInstance.StartEvent))]
        public static class GameplaySeasonInstance_StartEvent_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                CodeMatcher matcher = new CodeMatcher(instructions);
                MethodInfo mathMin = AccessTools.Method(typeof(Mathf), nameof(Mathf.Min), new[] { typeof(Int32), typeof(Int32) } );
                MethodInfo getMaxExclusive = AccessTools.Method(typeof(GameplaySeasonInstance_StartEvent_Patch), nameof(GameplaySeasonInstance_StartEvent_Patch.GetMaxExclusive));

                matcher.MatchStartForward(
                        new CodeMatch(OpCodes.Ldc_I4_5),
                        new CodeMatch(OpCodes.Call, mathMin))
                    .RemoveInstructions(2)
                    .Insert(
                        new CodeInstruction(OpCodes.Call, getMaxExclusive)
                    );
                return matcher.InstructionEnumeration();
            }

            static int GetMaxExclusive(int count)
            {
                bool isSingleCluster = CustomGameSettings.Instance.GetCurrentClusterLayout()?.clusterTags.Contains("Omnificia_SinglePlanetoid") ?? false;
                return isSingleCluster ? 10 : Mathf.Min(count, 5);
            }
        }*/

        [HarmonyPatch(typeof(Research))]
        [HarmonyPatch(nameof(Research.CheckBuyResearch))]
        public static class Research_CheckBuyResearch_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                CodeMatcher matcher = new CodeMatcher(instructions);
                MethodInfo getInstance = AccessTools.PropertyGetter(typeof(Game), nameof(Game.Instance));
                FieldInfo activeResearch = AccessTools.Field(typeof(Research), nameof(Research.activeResearch));
                MethodInfo sendCheck = AccessTools.Method(typeof(Research_CheckBuyResearch_Patch), nameof(Research_CheckBuyResearch_Patch.SendArchipelagoCheck));
                MethodInfo kmonoTrigger = AccessTools.Method(typeof(KMonoBehaviour), nameof(KMonoBehaviour.Trigger));

                matcher.MatchStartForward( new CodeMatch(OpCodes.Call, getInstance) )
                    .RemoveInstructions(6)
                    .Insert(
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, activeResearch),
                        new CodeInstruction(OpCodes.Call, sendCheck)
                    );

                return matcher.InstructionEnumeration();
            }

            static void SendArchipelagoCheck(TechInstance instance)
            {
                if (ArchipelagoNotIncluded.info == null)
                {
                    Game.Instance.Trigger(-107300940, instance.tech);
                    return;
                }

                char[] delimiters = { '<', '>' };
                string name = instance.tech.Name.Split(delimiters)[2];
                Debug.Log($"Name: {name}");
                if (name == "Cryofuel Propulsion")
                    name = "CryoFuel Propulsion";
                List<DefaultItem> defItems = ArchipelagoNotIncluded.info.spaced_out ? ArchipelagoNotIncluded.AllDefaultItems.FindAll(i => i.tech == name) : ArchipelagoNotIncluded.AllDefaultItems.FindAll(i => i.tech_base == name);
                int modItems = 0;
                if (ArchipelagoNotIncluded.info.apModItems.Count > 0)
                    modItems = ArchipelagoNotIncluded.AllModItems.FindAll(i => i.tech == name).Count;
                Debug.Log($"Count: {defItems.Count} {modItems}");
                int count = defItems.Count + modItems;
                string[] locationNames = new string[count];
                for (int i = 0; i < count; i++)
                {
                    string fullLocationName = $"{name} - {i + 1}";
                    Debug.Log($"Location: {fullLocationName} - {i + 1}");
                    locationNames[i] = fullLocationName;
                }
                ArchipelagoNotIncluded.AddLocationChecks(locationNames);
            }
        }

        [HarmonyPatch(typeof(TechItem))]
        [HarmonyPatch(nameof(TechItem.IsPOIUnlocked))]
        public static class IsPOIUnlocked_Patch
        {
            public static void Postfix(TechItem __instance, ref bool __result)
            {
                if (ArchipelagoNotIncluded.info == null)
                    return;

                if (isModItem(__instance.Id))
                    return;
                __result = CheckItemList(__instance);
                //__result = true;
            }
        }

        [HarmonyPatch(typeof(TechItem))]
        [HarmonyPatch(nameof(TechItem.IsComplete))]
        public static class IsComplete_Patch
        {
            public static bool Prefix(TechItem __instance, ref bool __result)
            {
                if (ArchipelagoNotIncluded.info == null)
                    return true;

                if (isModItem(__instance.Id))
                    return true;
                __result = CheckItemList(__instance);
                return false;
            }
        }

        [HarmonyPatch(typeof(POITechItemUnlocks.Instance))]
        [HarmonyPatch(nameof(POITechItemUnlocks.Instance.UnlockTechItems))]
        public static class POITechItemUnlocks_unlockTechItems_Patch
        {
            public static bool Prefix(POITechItemUnlocks.Instance __instance)
            {
                if (ArchipelagoNotIncluded.info == null)
                    return true;

                int count = 0;

                List<string> techIDs = __instance.def.POITechUnlockIDs;
                switch (techIDs)
                {
                    case List<string> x when x.Contains("Campfire"):
                        count = 3;
                        break;
                    case List<string> x when x.Contains("MissileFabricator"):
                        count = 2;
                        break;
                    default:
                        return true;
                }
                string[] locationNames = new string[count];
                for (int i = 0; i < count; i++)
                {
                    string fullLocationName = $"Research Portal - {i + 1}";
                    Debug.Log($"Location: {fullLocationName} - {i + 1}");
                    locationNames[i] = fullLocationName;
                }
                ArchipelagoNotIncluded.AddLocationChecks(locationNames);
                APSaveData.Instance.ResearchPortalUnlocked = true;

                MusicManager.instance.PlaySong("Stinger_ResearchComplete", false);
                __instance.UpdateUnlocked();

                return false;
            }
        }

        [HarmonyPatch(typeof(POITechItemUnlocks.Instance))]
        [HarmonyPatch(nameof(POITechItemUnlocks.Instance.UpdateUnlocked))]
        public static class POITechItemUnlocks_UpdateUnlocked_Patch
        {
            public static bool Prefix(POITechItemUnlocks.Instance __instance)
            {
                if (ArchipelagoNotIncluded.info == null)
                    return true;

                bool value = false;
                if (APSaveData.Instance.ResearchPortalUnlocked)
                    value = true;
                __instance.sm.isUnlocked.Set(value, __instance.smi, false);

                return false;
            }
        }

        [HarmonyPatch(typeof(PlanBuildingToggle))]
        [HarmonyPatch(nameof(PlanBuildingToggle.StandardDisplayFilter))]
        public static class DisplayFilter_Patch
        {
            public static bool Prefix(PlanBuildingToggle __instance, ref bool __result)
            {
                if (ArchipelagoNotIncluded.info == null)
                    return true;

                if (isModItem(__instance.def.PrefabID))
                    return true;
                __result = CheckItemList(__instance.def.PrefabID) && (__instance.planScreen.ActiveCategoryToggleInfo == null || __instance.buildingCategory == (HashedString)__instance.planScreen.ActiveCategoryToggleInfo.userData);
                return false;
            }
        }

        [HarmonyPatch(typeof(Tech))]
        [HarmonyPatch(nameof(Tech.ArePrerequisitesComplete))]
        public static class ArePrerequisitesComplete_Patch
        {
            public static bool Prefix(TechItem __instance, ref bool __result)
            {
                if (ArchipelagoNotIncluded.info == null)
                    return true;

                if (isModItem(__instance.Id))
                    return true;
                //__result = true;
                __result = CheckItemList(__instance);
                return false;
            }
        }

        [HarmonyPatch]
        public static class Event_Subscribe_Patch
        {
            public static IEnumerable<MethodBase> TargetMethods()
            {
                Dictionary<Type, string> MethodDict = new Dictionary<Type, string>()
                {
                    {typeof(PlanBuildingToggle), nameof(PlanBuildingToggle.Config)},
                    //{typeof(PlanScreen), nameof(PlanScreen.OnPrefabInit)},
                    {typeof(BuildMenuBuildingsScreen), nameof(BuildMenuBuildingsScreen.OnSpawn)},
                    //{typeof(BuildMenu), nameof(BuildMenu.OnCmpEnable)},
                    {typeof(ConsumerManager), nameof(ConsumerManager.OnSpawn)},
                    {typeof(MaterialSelectionPanel), nameof(MaterialSelectionPanel.OnPrefabInit)},
                    {typeof(SelectModuleSideScreen), nameof(SelectModuleSideScreen.OnCmpEnable)},
                };
                foreach (KeyValuePair<Type, string> pair in MethodDict)
                    yield return AccessTools.Method(pair.Key, pair.Value);
            }
            public static void Postfix(object __instance, MethodBase __originalMethod)
            {
                if (ArchipelagoNotIncluded.info == null)
                    return;

                int eventid = 11390976;
                switch (__instance)
                {
                    case PlanBuildingToggle toggle:
                        toggle.gameSubscriptions.Add(Game.Instance.Subscribe(eventid, toggle.CheckResearch));
                        break;
                    case PlanScreen screen:
                        if (!BuildMenu.UseHotkeyBuildMenu())
                            Game.Instance.Subscribe(eventid, screen.OnResearchComplete);
                        break;
                    case BuildMenuBuildingsScreen screen:
                        Game.Instance.Subscribe(eventid, screen.OnResearchComplete);
                        break;
                    case BuildMenu menu:
                        if (__originalMethod.Name == nameof(BuildMenu.OnCmpEnable))
                            Game.Instance.Subscribe(eventid, menu.OnResearchComplete);
                        else
                            Game.Instance.Unsubscribe(eventid, menu.OnResearchComplete);
                        break;
                    case ConsumerManager manager:
                        Game.Instance.Subscribe(eventid, manager.RefreshDiscovered);
                        break;
                    case MaterialSelectionPanel panel:
                        panel.gameSubscriptionHandles.Add(Game.Instance.Subscribe(eventid, delegate { panel.RefreshSelectors(); }));
                        break;
                    case SelectModuleSideScreen screen:
                        screen.gameSubscriptionHandles.Add(Game.Instance.Subscribe(eventid, screen.UpdateBuildableStates));
                        break;
                }
            }
        }

        static bool isModItem(string InternalName)
        {
            //Debug.Log($"Checking ModItem InternalName: {InternalName}");
            DefaultItem defItem = ArchipelagoNotIncluded.AllDefaultItems.Find(i => i.internal_name == InternalName);
            ModItem modItem = ArchipelagoNotIncluded.AllModItems.Find(i => i.internal_name == InternalName);
            if (defItem == null && (modItem != null && !modItem.randomized))
            {
                //Debug.Log($"Found ModItem: {InternalName}");
                return true;
            }
            //Debug.Log("Not ModItem");
            return false;
        }
        static bool CheckItemList(string InternalName)
        {
            if (ArchipelagoNotIncluded.StarterTech.Contains(InternalName))
                return true;

            if (ArchipelagoNotIncluded.Options.CreateModList)
                return false;

            //Debug.Log($"InternalName: {InternalName} {ArchipelagoNotIncluded.hadBionicDupe}");
            if (APSaveData.Instance.HadBionicDupe && InternalName == "CraftingTable")
                return true;

            DefaultItem defItem = ArchipelagoNotIncluded.AllDefaultItems.Find(i => i.internal_name == InternalName);
            ModItem modItem = ArchipelagoNotIncluded.AllModItems.Find(i => i.internal_name == InternalName);
            if (defItem != null)
            {
                bool ItemFound = APSaveData.Instance.LocalItemList.Contains(defItem.name);
                //Debug.Log($"CheckItemList: {ItemFound}");
                return ItemFound;
            }
            else if (modItem != null)
            {
                bool ItemFound = APSaveData.Instance.LocalItemList.Contains(modItem.name);
                //Debug.Log($"CheckItemList: {ItemFound}");
                return ItemFound;
            }
            else
                return false;
            /*    return false;
            if (ArchipelagoNotIncluded.netmon?.session?.Items?.AllItemsReceived?.Count() == 0)
                return false;
            foreach (ItemInfo item in ArchipelagoNotIncluded.netmon?.session?.Items?.AllItemsReceived)
                if (item.ItemDisplayName == defItem.name)
                    return true;
            return false;*/
        }

        static bool CheckItemList(TechItem TechItem)
        {
            //Debug.Log($"Name: {TechItem.Name}, Id: {TechItem.Id} {ArchipelagoNotIncluded.hadBionicDupe}");
            if (ArchipelagoNotIncluded.Options.CreateModList)
                return false;

            char[] delimiters = { '<', '>' };
            string name = ArchipelagoNotIncluded.CleanName(TechItem.Name);

            if (APSaveData.Instance.HadBionicDupe && TechItem.Id == "CraftingTable")
                return true;

            /*if (ArchipelagoNotIncluded.session.Items.AllItemsReceived.SingleOrDefault(i => i.ItemDisplayName == name) != null)
            {
                Debug.Log($"Found item in received list: {name}");
                return true;
            }
            else
            {
                Debug.Log($"Not in received list: {name}");
                return false;
            }*/
            //return true;
            return APSaveData.Instance.LocalItemList.Contains(name);
            /*if (ArchipelagoNotIncluded.netmon?.session?.Items?.AllItemsReceived == null)
                return false;
            if (ArchipelagoNotIncluded.netmon?.session?.Items?.AllItemsReceived.Count() == 0)
                return false;
            foreach (ItemInfo item in ArchipelagoNotIncluded.netmon?.session?.Items?.AllItemsReceived)
                if (item.ItemDisplayName == name)
                    return true;
            return false;*/
            //return false;
        }

        [HarmonyPatch(typeof(BionicMinionConfig))]
        [HarmonyPatch(nameof(BionicMinionConfig.BionicFreeDiscoveries))]
        public static class BionicFreeDiscoveries_Patch
        {
            public static bool Prefix()
            {
                APSaveData.Instance.HadBionicDupe = true;
                return false;
            }
        }

        /*[HarmonyPriority(Priority.First)]
        [HarmonyPatch(typeof(ArtifactAnalysisStationWorkable), nameof(ArtifactAnalysisStationWorkable.YieldPayload))]
        public static class ArtifactAnalysisStationWorkable_YieldPayload_Patch
        {
            public static bool Prefix(ArtifactAnalysisStationWorkable __instance, SpaceArtifact artifact)
            {
                GameUtil.KInstantiate(Assets.GetPrefab("GeneShufflerRecharge"), __instance.statesInstance.master.transform.position + __instance.finishedArtifactDropOffset, Grid.SceneLayer.Ore, null, 0).SetActive(true);
                int num = Mathf.FloorToInt(artifact.GetArtifactTier().payloadDropChance * 20f);
                for (int i = 0; i < num; i++)
                {
                    GameUtil.KInstantiate(Assets.GetPrefab("OrbitalResearchDatabank"), __instance.statesInstance.master.transform.position + __instance.finishedArtifactDropOffset, Grid.SceneLayer.Ore, null, 0).SetActive(true);
                }
                return false;
            }
        }*/
        //[HarmonyPatch(typeof(SaveLoader), nameof(SaveLoader.Load), new[] {typeof(IReader)})]
        [HarmonyPatch(typeof(Game), nameof(Game.OnSpawn))]
        public static class SaveLoader_Load_Patch
        {
            public static void Postfix()
            {
                if (ArchipelagoNotIncluded.Options.CreateModList)
                    return;
                if (APSaveData.Instance.APSeedInfo != null)
                    ArchipelagoNotIncluded.info = APSaveData.Instance.APSeedInfo;

                List<PlanScreen.PlanInfo> storage = new List<PlanScreen.PlanInfo>();
                /*foreach (PlanScreen.PlanInfo info in TUNING.BUILDINGS.PLANORDER)
                {
                    Debug.Log($"{info.category}: ");
                    foreach (KeyValuePair<string, string> kvp in info.buildingAndSubcategoryData)
                    {
                        Debug.Log($"{kvp.Key}: {kvp.Value}");
                    }
                }*/
                //foreach (PlanScreen.PlanInfo key in TUNING.BUILDINGS.PLANORDER)
                //storage.Add(new PlanScreen.PlanInfo(key.category, key.hideIfNotResearched, key.data, ));
                if (ArchipelagoNotIncluded.info == null)
                    return;

                foreach (DefaultItem item in ArchipelagoNotIncluded.AllDefaultItems)
                {
                    //Debug.Log(info.spaced_out);
                    //Debug.Log(item.internal_tech + " " + item.internal_tech_base);
                    string InternalTech = ArchipelagoNotIncluded.info.spaced_out ? item.internal_tech : item.internal_tech_base;
                    switch (item.version)
                    {
                        case "BaseOnly":
                            if (ArchipelagoNotIncluded.info.spaced_out)
                                break;
                            goto default;
                        case "SpacedOut":
                            if (!ArchipelagoNotIncluded.info.spaced_out)
                                break;
                            goto default;
                        case "Frosty":
                            if (!ArchipelagoNotIncluded.info.frosty)
                                break;
                            goto default;
                        case "Bionic":
                            if (!ArchipelagoNotIncluded.info.bionic)
                                break;
                            goto default;
                        default:
                            if (ArchipelagoNotIncluded.allTechList.ContainsKey(item.internal_name))
                                break;
                            ArchipelagoNotIncluded.allTechList.Add(item.internal_name, "uncategorized");
                            break;
                    }
                    if (ArchipelagoNotIncluded.Sciences.Count > 0 && ArchipelagoNotIncluded.Sciences?.TryGetValue(InternalTech, out List<string> techList) == true)
                    {
                        if (techList == null)
                            techList = new List<string>();
                        techList.Add(item.internal_name);
                    }
                    else
                    {
                        if (InternalTech == "None")
                            continue;
                        ArchipelagoNotIncluded.Sciences[InternalTech] = new List<string>
                        {
                            item.internal_name
                        };
                    }
                }

                if (ArchipelagoNotIncluded.AllModItems != null)
                {
                    foreach (ModItem item in ArchipelagoNotIncluded.AllModItems)
                    {
                        if (ArchipelagoNotIncluded.info?.apModItems.Contains(item.internal_name) == true)
                        {
                            item.randomized = true;
                            if (ArchipelagoNotIncluded.allTechList.ContainsKey(item.internal_name))
                                continue;
                            ArchipelagoNotIncluded.allTechList.Add(item.internal_name, "uncategorized");
                        }
                    }
                }

                Techs instance = Db.Get().Techs;

                Dictionary<string, int> idCounts = new Dictionary<string, int>();
                foreach (KeyValuePair<string, List<string>> pair in ArchipelagoNotIncluded.info.technologies)
                {
                    if (pair.Key.ToLower() == "none" || String.IsNullOrEmpty(pair.Key))
                    {
                        Debug.LogWarningFormat("Skipping technology with key '{0}'", pair.Key);
                        continue;
                    }
                    Debug.Log($"Generating research for {pair.Key}, ({pair.Value.Join(s => s, ",")})");
                    Tech tech = instance.TryGet(pair.Key);
                    Dictionary<string, float> researchCost = null;
                    if (ArchipelagoNotIncluded.cheatmode)
                        researchCost = new Dictionary<string, float>
                        {
                            {"basic", 1f },
                            {"advanced", 0f },
                            {"nuclear", 0f },
                            {"orbital", 0f }
                        };
                    if (tech == null)
                        tech = new Tech(pair.Key, new List<string>(), instance, researchCost);
                    else
                    {
                        if (researchCost != null)
                            tech.costsByResearchTypeID = researchCost;
                        tech.unlockedItemIDs = new List<string>();
                        tech.unlockedItems = new List<TechItem>();
                    }
                    foreach (string techitemidplayer in pair.Value)
                    {
                        string[] splits = techitemidplayer.Split(new string[] { ">>" }, StringSplitOptions.RemoveEmptyEntries);
                        string techitemid = splits[0];
                        int playerid = int.Parse(splits[1]);
                        if (idCounts.ContainsKey(techitemid))
                            idCounts[techitemid]++;
                        else
                            idCounts[techitemid] = 0;
                        //Debug.Log($"Player: {ArchipelagoNotIncluded.info.AP_PlayerID} ItemID: {techitemid} PlayerID: {playerid}");
                        if (ArchipelagoNotIncluded.info.AP_PlayerID == playerid && !techitemid.StartsWith("Care Package"))
                        {
                            //Debug.Log("Item was given default sprite");
                            TechItem item = Db.Get().TechItems.TryGet(techitemid);
                            if (item != null)
                            {
                                item.parentTechId = pair.Key;
                                tech.unlockedItems.Add(item);
                                //InjectionMethods.MoveItemToNewTech(techitemid, item.parentTechId, pair.Key);
                            }
                            else
                            {
                                Debug.Log($"TechItem for {techitemid} does not exist");
                                //tech.unlockedItems.Add(Db.Get().TechItems.AddTechItem(techitemid, techitemid, techitemid, Db.Get().TechItems.GetPrefabSpriteFnBuilder(techitemid.ToTag())));
                                //InjectionMethods.AddItemToTechnologyKanim(techitemid, pair.Key, techitemid, techitemid, techitemid + "_kanim");
                            }
                            tech.AddUnlockedItemIDs(new string[] { techitemid });
                            //InjectionMethods.AddBuildingToTechnology(pair.Key, techitemid);
                            ArchipelagoNotIncluded.allTechList.Remove(techitemid);
                        }
                        else
                        {
                            //Debug.Log("Item was given custom sprite");
                            techitemid += idCounts[techitemid];
                            TechItem item = Db.Get().TechItems.TryGet(techitemid);
                            if (item != null)
                            {
                                item.parentTechId = pair.Key;
                                tech.unlockedItems.Add(item);
                            }
                            else
                            {
                                if (ArchipelagoNotIncluded.cheatmode)
                                    InjectionMethods.AddItemToTechnologyKanim(techitemid, pair.Key, techitemid, "A mysterious item from another world", "apItemSprite_kanim");
                                else
                                    InjectionMethods.AddItemToTechnologyKanim(techitemid, pair.Key, "Unknown Artifact", "A mysterious item from another world", "apItemSprite_kanim");
                            }
                        }
                    }

                    //ArchipelagoNotIncluded.TechList.Add(tech.Id, new List<string>(tech.unlockedItemIDs));
                    //new Tech(pair.Key, pair.Value.ToList(), __instance);
                }
                /*foreach (string item in ArchipelagoNotIncluded.allTechList.Keys)
                {
                    ModUtil.AddBuildingToPlanScreen(TUNING.BUILDINGS.PLANSUBCATEGORYSORTING[item], item);
                }

                foreach (KeyValuePair<Tag, HashedString> kvp in PlanScreen.Instance.tagCategoryMap)
                {
                    Debug.Log($"{kvp.Key}: {kvp.Value}");
                }*/
                //PlanScreen.Instance.tagCategoryMap.Clear();
                //foreach (KeyValuePair<Tag, HashedString> keyValuePair in storage)
                //    PlanScreen.Instance.tagCategoryMap.Add(keyValuePair.Key, keyValuePair.Value);
                int apItems = APSaveData.Instance.LocalItemList.Count;
                Debug.Log($"apItems: {apItems}, lastItem: {ArchipelagoNotIncluded.lastItem}");
                /*if (apItems == 0)
                    return;

                if (apItems == ArchipelagoNotIncluded.lastItem)
                    return;*/

                //ArchipelagoNotIncluded.netmon.ProcessItemQueue();
            }
        }

        [HarmonyPatch(typeof(PlanScreen))]
        [HarmonyPatch(nameof(PlanScreen.OnSpawn))]
        public static class PlanScreen_OnSpawn_Patch
        {
            public static void Postfix()
            {
                if (ArchipelagoNotIncluded.info == null)
                    return;
                ArchipelagoNotIncluded.netmon.ProcessItemQueue();
            }
        }

        [HarmonyPatch(typeof(SaveGame))]
        [HarmonyPatch(nameof(SaveGame.OnPrefabInit))]
        public class SaveGame_Patch
        {
            public static void Postfix(SaveGame __instance)
            {
                __instance.gameObject.AddOrGet<APSaveData>();
            }
        }

        /*[HarmonyPatch(typeof(SaveLoader))]
        [HarmonyPatch(nameof(SaveLoader.Save), new[] { typeof(BinaryWriter) })]
        public class Save_Patch
        {
            public static void Postfix(BinaryWriter writer)
            {
                if (ArchipelagoNotIncluded.allowResourceChecks)
                    writer.Write(1);
                else
                    writer.Write(0);

                if (ArchipelagoNotIncluded.hadBionicDupe)
                    writer.Write(1);
                else
                    writer.Write(0);

                writer.Write(ArchipelagoNotIncluded.getLastIndex());
            }
        }

        [HarmonyPatch(typeof(SaveLoader))]
        [HarmonyPatch(nameof(SaveLoader.Load), new[] { typeof(IReader) })]
        public class Load_Patch
        {
            public static void Postfix(IReader reader)
            {
                try
                {
                    int allow = reader.ReadInt32();
                    ArchipelagoNotIncluded.allowResourceChecks = (allow == 1);

                    int bionic = reader.ReadInt32();
                    if (!ArchipelagoNotIncluded.hadBionicDupe)
                        ArchipelagoNotIncluded.hadBionicDupe = (bionic == 1);

                    int index = reader.ReadInt32();
                    ArchipelagoNotIncluded.setLastIndex(index);
                    APNetworkMonitor.HighestCount = index;
                }
                catch { }
            }
        }*/

        

        [HarmonyPatch(typeof(BuildingDef))]
        [HarmonyPatch(nameof(BuildingDef.IsAvailable))]
        public static class IsAvailable_Patch
        {
            public static void Postfix(BuildingDef __instance, ref bool __result)
            {
                if (ArchipelagoNotIncluded.info == null)
                    return;
                //Debug.Log("Found update method");
                //Debug.Log(__instance.PrefabID);
                if (isModItem(__instance.PrefabID))
                    return;
                __result = CheckItemList(__instance.PrefabID);
                //if (__result)
                //    Debug.Log($"{__instance.PrefabID} isAvailable");
            }
        }

        [HarmonyPatch(typeof(SpeedControlScreen))]
        [HarmonyPatch(nameof(SpeedControlScreen.Unpause))]
        public static class Unpause_Patch
        {
            public static void Postfix()
            {
            }
        }

        [HarmonyPatch(typeof(DlcManager))]
        [HarmonyPatch(nameof(DlcManager.IsContentSubscribed))]
        public static class IsContentSubscribed_Patch
        {
            public static void Postfix(string dlcId, ref bool __result)
            {
                //if (dlcId == "DLC3_ID")
                //    __result = false;
            }
        }

        [HarmonyPatch(typeof(PlanBuildingToggle))]
        [HarmonyPatch("RefreshFG")]
        public static class RefreshFG_Patch
        {
            /*public static bool Prefix(PlanBuildingToggle __instance, BuildingDef ___def, PlanScreen.RequirementsState requirementsState)
            {
                //Debug.Log("Found update method");
                //Debug.Log(___def.PrefabID);
                //if (!CheckItemList(___def.PrefabID))
                    //requirementsState = PlanScreen.RequirementsState.Invalid;
                return true;
            }*/
        }

        [HarmonyPatch(typeof(PlanScreen))]
        [HarmonyPatch(nameof(PlanScreen.GetBuildableState))]
        public static class GetBuildableState_Patch
        {
            public static bool Prefix(PlanScreen __instance, BuildingDef def, ref PlanScreen.RequirementsState __result)
            {
                if (ArchipelagoNotIncluded.info == null)
                {
                    //Debug.Log("ArchipelagoNotIncluded.info is null in GetBuildableState_Patch");
                    return true;
                }
                if ((UnityEngine.Object)def == (UnityEngine.Object)null)
                {
                    //Debug.Log("GetBuildableState called with null def");
                    return true;
                }
                if (__instance._buildableStatesByID.ContainsKey(def.PrefabID) && __instance._buildableStatesByID[def.PrefabID] == PlanScreen.RequirementsState.Complete)
                {
                    //Debug.Log("GetBuildableState found existing Complete state for " + def.PrefabID);
                    __result = PlanScreen.RequirementsState.Complete;
                    return false;
                }
                if (CheckItemList(def.PrefabID))
                {
                    //Debug.Log("GetBuildableState found Complete state for " + def.PrefabID);
                    if (__instance._buildableStatesByID.ContainsKey(def.PrefabID))
                        __instance._buildableStatesByID[def.PrefabID] = PlanScreen.RequirementsState.Complete;
                    else
                        __instance._buildableStatesByID.Add(def.PrefabID, PlanScreen.RequirementsState.Complete);
                    __result = PlanScreen.RequirementsState.Complete;
                    return false;
                }
                return true;
            }
        }

        /*[HarmonyPatch(typeof(BuildingConfigManager))]
        [HarmonyPatch(nameof(BuildingConfigManager.RegisterBuilding))]
        public static class RegisterBuilding_Patch
        {
            public static void Postfix(Dictionary<IBuildingConfig, BuildingDef> ___configTable, IBuildingConfig config)
            {
                if (ArchipelagoNotIncluded.info == null)
                    return;

                ___configTable[config].PrefabID;
                config.GetType().Assembly;
            }
        }*/

        [HarmonyPatch(typeof(TechItems))]
        [HarmonyPatch(nameof(TechItems.IsTechItemComplete))]
        public static class IsTechItemComplete_Patch
        {
            /*public static bool Prefix(TechItems __instance, string id, ref bool __result)
            {
                if (ArchipelagoNotIncluded.info == null)
                    return true;
                if (isModItem(id))
                    return true;
                //Debug.Log($"IsTechItemComplete {__result} for {id}");
                //__result = CheckItemList(techItemId);
                return true;
            }*/
        }

        [HarmonyPatch(typeof(PlanScreen))]
        [HarmonyPatch(nameof(PlanScreen.GetBuildableStateForDef))]
        public static class GetBuildableStateForDef_Patch
        {
            /*public static void Postfix(PlanScreen __instance, BuildingDef def, ref PlanScreen.RequirementsState __result)
            {
                if (ArchipelagoNotIncluded.info == null)
                {
                    Debug.Log("ArchipelagoNotIncluded.info is null in GetBuildableStateForDef_Patch");
                    return;
                }
                if (def == null)
                {
                    Debug.Log("GetBuildableStateForDef called with null def");
                    return;
                }
                if (__result.ToString() != "Invalid")
                    Debug.Log($"GetBuildableStateForDef {__result.ToString()} for {def.PrefabID} - {def.Name}");
                //__result = CheckItemList(def.PrefabID);
                //return;
            }*/
        }

        [HarmonyPatch(typeof(PlanScreen), nameof(PlanScreen.SetCategoryButtonState))]
        public class PlanScreen_SetCategoryButtonState_Patch
        {
            /*public static void Postfix(PlanScreen __instance)
            {
                foreach (var toggleEntry in __instance.toggleEntries)
                {
                    if (toggleEntry.toggleInfo.toggle.gameObject.activeInHierarchy)
                    {
                        toggleEntry.toggleInfo.toggle.fgImage.transform.Find("ResearchIcon").gameObject.SetActive(false);
                        ImageToggleState.State newState = __instance.activeCategoryInfo == null || toggleEntry.toggleInfo.userData != __instance.activeCategoryInfo.userData ? ImageToggleState.State.Inactive : ImageToggleState.State.Active;
                        foreach (ImageToggleState toggleImage in toggleEntry.toggleImages)
                        {
                            toggleImage.SetState(newState);
                        }
                        __instance.CategoryInteractive[toggleEntry.toggleInfo] = true;
                    }
                }
            }*/
        }

        [HarmonyPatch(typeof(PlanScreen.ToggleEntry))]
        [HarmonyPatch(nameof(PlanScreen.ToggleEntry.AreAnyRequiredTechItemsAvailable))]
        public static class AreAnyRequiredTechItemsAvailable_Patch
        {
            public static bool Prefix(PlanScreen.ToggleEntry __instance, ref bool __result)
            {
                if (ArchipelagoNotIncluded.info == null)
                    return true;
                
                __result = true;
                return false;
            }
        }
        
        [HarmonyPatch(typeof(ComplexRecipe))]
        [HarmonyPatch(nameof(ComplexRecipe.IsRequiredTechUnlocked))]
        public static class IsRequiredTechUnlocked_Patch
        {
            public static bool Prefix(ComplexRecipe __instance, ref bool __result)
            {
                if (ArchipelagoNotIncluded.info == null)
                    return true;

                string search = null;
                foreach (ComplexRecipe.RecipeElement element in __instance.results)
                {
                    switch (element.material.Name)
                    {
                        case string x when x.Contains("Atmo"):
                            search = "AtmoSuit";
                            break;
                        case string x when x.Contains("Jet"):
                            search = "JetSuit";
                            break;
                        case string x when x.Contains("Lead"):
                            search = "LeadSuit";
                            break;
                        case string x when x.Contains("Oxygen"):
                            search = "OxygenMask";
                            break;
                        default:
                            return true;
                    }
                }
                if (!String.IsNullOrEmpty(search) && CheckItemList(search))
                    __result = true;
                else
                    __result = false;
                return false;
            }
        }

        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch(nameof(Db.Initialize))]
        public static class Initialize_Patch
        {
            public static void Postfix(Db __instance)
            {
                ArchipelagoNotIncluded.ArchipelagoConnected = new("ArchipelagoConnected", __instance.StatusItemCategories, "ArchipelagoConnected");
            }
        }

            /*[HarmonyPatch(typeof(Techs))]
            [HarmonyPatch(nameof(Techs.Load))]
            public static class Techs_Load_Patch
            {
                public static MethodInfo TargetMethod()
                {
                    return AccessTools.Method(typeof(ResourceLoader<ResourceTreeNode>), "Load", new Type[] { typeof(TextAsset) });
                    return typeof(ResourceTreeLoader<>).GetMethod("Load", new Type[] { typeof(TextAsset) });
                }
                public static bool Prefix(Techs __instance, List<List<Tuple<string, float>>> ___TECH_TIERS)
                {
                    TextAsset tree_file = Db.Get().researchTreeFileExpansion1;
                    ResourceTreeLoader<ResourceTreeNode> resourceTreeLoader = new ResourceTreeLoader<ResourceTreeNode>(tree_file);
                    List<TechTreeTitle> techTreeTitleList = new List<TechTreeTitle>();
                    for (int idx = 0; idx < Db.Get().TechTreeTitles.Count; ++idx)
                        techTreeTitleList.Add(Db.Get().TechTreeTitles[idx]);
                    techTreeTitleList.Sort((Comparison<TechTreeTitle>)((a, b) => a.center.y.CompareTo(b.center.y)));

                    ResourceTreeNode newnode = new ResourceTreeNode
                    {
                        Id = "_TestId",
                        Name = "Test",
                        nodeX = 200.0f,
                        nodeY = -5000f,
                        height = 72f,
                        width = 250f
                    };
                    newnode.references.Add(new ResourceTreeNode
                    {
                        Id = "AtmoSuit",
                        Name = "Test",
                        nodeX = 200.0f,
                        nodeY = 45f,
                        height = 72f,
                        width = 250f
                    });
                    resourceTreeLoader.resources.Add(newnode);
                    foreach (ResourceTreeNode node in (ResourceLoader<ResourceTreeNode>)resourceTreeLoader)
                    {
                        //Debug.Log(GetLogFor(node));
                        int count = 0;
                        foreach (ResourceTreeNode refNode in node.references)
                        {
                            count++;
                            //Debug.Log($"{node.Id} ref #{count}: {GetLogFor(refNode)}");
                        }
                        count = 0;
                        foreach (ResourceTreeNode.Edge edge in node.edges)
                        {
                            count++;
                            //Debug.Log($"{node.Id} edge #{count}: {GetLogFor(edge)}");
                        }
                        //Debug.Log(GetLogFor(node.edges));
                        if (!string.Equals(node.Id.Substring(0, 1), "_"))
                        {
                            Tech tech1 = __instance.TryGet(node.Id);
                            if (tech1 != null)
                            {
                                string categoryID1 = "";
                                for (int index = 0; index < techTreeTitleList.Count; ++index)
                                {
                                    if ((double)techTreeTitleList[index].center.y >= (double)node.center.y)
                                    {
                                        categoryID1 = techTreeTitleList[index].Id;
                                        break;
                                    }
                                }
                                tech1.SetNode(node, categoryID1);
                                foreach (ResourceTreeNode reference in node.references)
                                {
                                    Tech tech2 = __instance.TryGet(reference.Id);
                                    if (tech2 != null)
                                    {
                                        string categoryID2 = "";
                                        for (int index = 0; index < techTreeTitleList.Count; ++index)
                                        {
                                            if ((double)techTreeTitleList[index].center.y >= (double)node.center.y)
                                            {
                                                categoryID2 = techTreeTitleList[index].Id;
                                                break;
                                            }
                                        }
                                        tech2.SetNode(reference, categoryID2);
                                        tech2.requiredTech.Add(tech1);
                                        tech1.unlockedTech.Add(tech2);
                                    }
                                }
                            }
                        }
                    }
                    foreach (Tech resource in __instance.resources)
                    {
                        resource.tier = Techs.GetTier(resource);
                        foreach (Tuple<string, float> tuple in ___TECH_TIERS[resource.tier])
                        {
                            if (!resource.costsByResearchTypeID.ContainsKey(tuple.first))
                                resource.costsByResearchTypeID.Add(tuple.first, tuple.second);
                        }
                    }
                    for (int idx = __instance.Count - 1; idx >= 0; --idx)
                    {
                        if (!((Tech)__instance.GetResource(idx)).FoundNode)
                            __instance.Remove(__instance.GetResource(idx));
                    }
                    return false;
                }
                {
                    //Debug.Log("Found update method");
                    //Debug.Log(___def.PrefabID);
                    //if (!CheckItemList(___def.PrefabID))
                    //requirementsState = PlanScreen.RequirementsState.Invalid;

                    ResourceTreeNode node = new ResourceTreeNode
                    {
                        Id = "TestId",
                        Name = "Test",
                        nodeX = 200.0f,
                        nodeY = 4500f,
                        height = 72f,
                        width = 250f
                    };
                    ___resources.Add(node);
                    Debug.Log("Code is ran");
                }
            }*/
            public static string GetLogFor(object objectToGetStateOf)
        {
            if (objectToGetStateOf == null)
            {
                const string PARAMETER_NAME = "objectToGetStateOf";
                throw new ArgumentException(string.Format("Parameter {0} cannot be null", PARAMETER_NAME), PARAMETER_NAME);
            }
            var builder = new StringBuilder();

            foreach (var property in objectToGetStateOf.GetType().GetProperties())
            {
                object value = property.GetValue(objectToGetStateOf, null);

                builder.Append(property.Name)
                .Append(" = ")
                .Append((value ?? "null"))
                .AppendLine();
            }
            foreach (var property in objectToGetStateOf.GetType().GetFields())
            {
                object value = property.GetValue(objectToGetStateOf);

                builder.Append(property.Name)
                .Append(" = ")
                .Append((value ?? "null"))
                .AppendLine();

                //if (objectToGetStateOf.GetType() == typeof(ResourceTreeNode))
            }
            return builder.ToString();
        }
    }

}
