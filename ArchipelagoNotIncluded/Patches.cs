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

                if (ArchipelagoNotIncluded.info != null)
                {
                    string planet = ArchipelagoNotIncluded.info.planet;
                    string cluster = string.Empty;
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
                //foreach (string cluster in __instance.clusterKeys)
                //    Debug.Log($"Cluster name: {cluster}");

                ArchipelagoNotIncluded.ItemList.Clear();
                ArchipelagoNotIncluded.ItemListDetailed.Clear();
                ArchipelagoNotIncluded.DebugWasUsed = false;
                ArchipelagoNotIncluded.lastIndexSaved = 0;
                ArchipelagoNotIncluded.runCount = 0;
                ArchipelagoNotIncluded.planetText = string.Empty;
                return false;
            }
        }

        [HarmonyPatch(typeof(ClusterCategorySelectionScreen))]
        [HarmonyPatch(nameof(ClusterCategorySelectionScreen.OnSpawn))]
        public static class Cluster_OnSpawn_Patch
        {
            public static void Postfix(ClusterCategorySelectionScreen __instance)
            {
                ArchipelagoNotIncluded.allowResourceChecks = true;
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
                ArchipelagoNotIncluded.allowResourceChecks = false;
            }
        }

        [HarmonyPatch(typeof(ColonyDestinationSelectScreen))]
        [HarmonyPatch(nameof(ColonyDestinationSelectScreen.LaunchClicked))]
        public static class LaunchClicked_Patch
        {
            public static void Postfix()
            {
                if (ArchipelagoNotIncluded.Options.CreateModList)
                    return;
                if (ArchipelagoNotIncluded.info.teleporter)
                    CustomGameSettings.Instance.SetQualitySetting(CustomGameSettingConfigs.Teleporters, "Enabled");
            }
        }

        [HarmonyPatch(typeof(DiscoveredResources))]
        [HarmonyPatch(nameof(DiscoveredResources.Discover), new[] { typeof(Tag), typeof(Tag) })]
        public static class Discover_Patch
        {
            public static bool Prefix(DiscoveredResources __instance, out int __state)
            {
                __state = __instance.newDiscoveries.Count;
                return true;
            }

            public static void Postfix(DiscoveredResources __instance, Tag tag, int __state)
            {
                if (ArchipelagoNotIncluded.allowResourceChecks && __instance.newDiscoveries.Count > __state)      // New Discovery was added
                {
                    string ResourceName = tag.ProperNameStripLink();
                    Debug.Log($"New Discovery: Name: {tag.Name}, StripLink: {ResourceName}");
                    //foreach (string resource in ArchipelagoNotIncluded.info.resourceChecks)
                    //Debug.Log(resource);
                    string location = $"Discover Resource: {ResourceName}";
                    if (ArchipelagoNotIncluded.info.resourceChecks.Contains( location ) )
                        ArchipelagoNotIncluded.netmon.SendResourceCheck(location);
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
                string goal = ArchipelagoNotIncluded.info.goal;
                Debug.Log($"New Achievement: {achievement} Goal: {goal}");
                switch (achievement)
                {
                    case "CompleteResearchTree":
                        if (goal == "research_all")
                            ArchipelagoNotIncluded.netmon.session.SetGoalAchieved();
                        goto default;
                    case "space_race":
                        if (goal == "launch_rocket")
                            ArchipelagoNotIncluded.netmon.session.SetGoalAchieved();
                        goto default;
                    case "thriving":
                        if (goal == "home_sweet_home")
                            ArchipelagoNotIncluded.netmon.session.SetGoalAchieved();
                        goto default;
                    case "ReachedDistantPlanet":
                        if (goal == "great_escape")
                            ArchipelagoNotIncluded.netmon.session.SetGoalAchieved();
                        goto default;
                    case "CollectedArtifacts":
                        if (goal == "cosmic_archaeology")
                            ArchipelagoNotIncluded.netmon.session.SetGoalAchieved();
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
                if (ArchipelagoNotIncluded.info.goal == "monument" && __result)
                    ArchipelagoNotIncluded.netmon.session.SetGoalAchieved();

            }
        }

        [HarmonyPatch(typeof(Research))]
        [HarmonyPatch(nameof(Research.CheckBuyResearch))]
        public static class Research_CheckBuyResearch_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                CodeMatcher matcher = new CodeMatcher(instructions);
                MethodInfo techPurchased = AccessTools.Method(typeof(TechInstance), nameof(TechInstance.Purchased));
                FieldInfo activeResearch = AccessTools.Field(typeof(Research), nameof(Research.activeResearch));
                MethodInfo sendCheck = AccessTools.Method(typeof(Research_CheckBuyResearch_Patch), nameof(Research_CheckBuyResearch_Patch.SendArchipelagoCheck));
                MethodInfo kmonoTrigger = AccessTools.Method(typeof(KMonoBehaviour), nameof(KMonoBehaviour.Trigger));

                int startRemove = matcher.MatchStartForward( new CodeMatch(OpCodes.Callvirt, techPurchased) )
                    .ThrowIfNotMatch($"Could not find entry point for CheckBuyResearch")
                    .Advance(1)
                    .InsertAndAdvance(
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, activeResearch),
                        new CodeInstruction(OpCodes.Call, sendCheck)
                    ).Pos;

                int endRemove = matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Callvirt, kmonoTrigger)
                    ).Pos;

                matcher.RemoveInstructionsInRange(startRemove, endRemove);

                return matcher.InstructionEnumeration();
            }

            static void SendArchipelagoCheck(TechInstance instance)
            {
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
                long[] locationIds = new long[count];
                for (int i = 0; i < count; i++)
                {
                    Debug.Log($"Location: {name} - {i + 1}");
                    long id = ArchipelagoNotIncluded.netmon.session.Locations.GetLocationIdFromName("Oxygen Not Included", $"{name} - {i + 1}");
                    locationIds[i] = id;
                }
                ArchipelagoNotIncluded.netmon.session.Locations.CompleteLocationChecks(locationIds);
            }
        }

        [HarmonyPatch(typeof(TechItem))]
        [HarmonyPatch(nameof(TechItem.IsPOIUnlocked))]
        public static class IsPOIUnlocked_Patch
        {
            public static void Postfix(TechItem __instance, ref bool __result)
            {
                if (isModItem(__instance.Id))
                    return;
                __result = __result | CheckItemList(__instance);
                //__result = true;
            }
        }

        [HarmonyPatch(typeof(TechItem))]
        [HarmonyPatch(nameof(TechItem.IsComplete))]
        public static class IsComplete_Patch
        {
            public static bool Prefix(TechItem __instance, ref bool __result)
            {
                if (isModItem(__instance.Id))
                    return true;
                __result = CheckItemList(__instance);
                return false;
            }
        }

        [HarmonyPatch(typeof(Tech))]
        [HarmonyPatch(nameof(Tech.ArePrerequisitesComplete))]
        public static class ArePrerequisitesComplete_Patch
        {
            public static bool Prefix(TechItem __instance, ref bool __result)
            {
                if (isModItem(__instance.Id))
                    return true;
                //__result = true;
                __result = __result | CheckItemList(__instance);
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
            if (ArchipelagoNotIncluded.hadBionicDupe && InternalName == "CraftingTable")
                return true;

            DefaultItem defItem = ArchipelagoNotIncluded.AllDefaultItems.Find(i => i.internal_name == InternalName);
            ModItem modItem = ArchipelagoNotIncluded.AllModItems.Find(i => i.internal_name == InternalName);
            if (defItem != null)
            {
                bool ItemFound = (bool)ArchipelagoNotIncluded.netmon?.session?.Items?.AllItemsReceived.Any<ItemInfo>(i => i.ItemDisplayName == defItem.name/* && i.Player.Name == ArchipelagoNotIncluded.netmon.SlotName*/);
                //Debug.Log($"CheckItemList: {ItemFound}");
                return ItemFound;
            }
            else if (modItem != null)
            {
                bool ItemFound = (bool)ArchipelagoNotIncluded.netmon?.session?.Items?.AllItemsReceived.Any<ItemInfo>(i => i.ItemDisplayName == modItem.name/* && i.Player.Name == ArchipelagoNotIncluded.netmon.SlotName*/);
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

            if (ArchipelagoNotIncluded.hadBionicDupe && TechItem.Id == "CraftingTable")
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
            return (bool)ArchipelagoNotIncluded.netmon?.session?.Items?.AllItemsReceived.Any<ItemInfo>(i => i.ItemDisplayName == name/* && i.Player.Name == ArchipelagoNotIncluded.netmon.SlotName*/);
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
                ArchipelagoNotIncluded.hadBionicDupe = true;
                return false;
            }
        }

        //[HarmonyPatch(typeof(SaveLoader), nameof(SaveLoader.Load), new[] {typeof(IReader)})]
        [HarmonyPatch(typeof(Game), nameof(Game.OnSpawn))]
        public static class SaveLoader_Load_Patch
        {
            public static void Postfix()
            {
                if (ArchipelagoNotIncluded.Options.CreateModList)
                    return;

                Techs instance = Db.Get().Techs;

                Dictionary<string, int> idCounts = new Dictionary<string, int>();
                foreach (KeyValuePair<string, List<string>> pair in ArchipelagoNotIncluded.info?.technologies)
                {
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

                int apItems = ArchipelagoNotIncluded.netmon.session.Items.AllItemsReceived.Count;
                Debug.Log($"apItems: {apItems}, lastItem: {ArchipelagoNotIncluded.lastItem}");
                /*if (apItems == 0)
                    return;

                if (apItems == ArchipelagoNotIncluded.lastItem)
                    return;*/

                ArchipelagoNotIncluded.netmon.UpdateAllItems(true);
            }
        }

        [HarmonyPatch(typeof(SaveLoader))]
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
        }

        

        [HarmonyPatch(typeof(BuildingDef))]
        [HarmonyPatch(nameof(BuildingDef.IsAvailable))]
        public static class IsAvailable_Patch
        {
            public static void Postfix(BuildingDef __instance, ref bool __result)
            {
                //Debug.Log("Found update method");
                //Debug.Log(__instance.PrefabID);
                if (isModItem(__instance.PrefabID))
                    return;
                __result = __result & CheckItemList(__instance.PrefabID);
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
                if ((UnityEngine.Object)def == (UnityEngine.Object)null)
                    return true;
                if (__instance._buildableStatesByID.ContainsKey(def.PrefabID) && __instance._buildableStatesByID[def.PrefabID] == PlanScreen.RequirementsState.Complete)
                {
                    __result = PlanScreen.RequirementsState.Complete;
                    return false;
                }
                if (CheckItemList(def.PrefabID))
                {
                    if (__instance._buildableStatesByID.ContainsKey(def.PrefabID))
                        __instance._buildableStatesByID[def.PrefabID] = PlanScreen.RequirementsState.Complete;
                    else
                        __instance._buildableStatesByID.Add(def.PrefabID, PlanScreen.RequirementsState.Complete);
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(ComplexRecipe))]
        [HarmonyPatch(nameof(ComplexRecipe.IsRequiredTechUnlocked))]
        public static class IsRequiredTechUnlocked_Patch
        {
            public static bool Prefix(ComplexRecipe __instance, ref bool __result)
            {
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
                    }
                }
                if (!String.IsNullOrEmpty(search) && CheckItemList(search))
                    __result = true;
                else
                    __result = false;
                return false;
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
