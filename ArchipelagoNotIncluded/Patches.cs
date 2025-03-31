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

namespace ArchipelagoNotIncluded
{
    
    public class Patches
    {
        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch(nameof(Db.Initialize))]
        public class Db_Initialize_Patch
        {
            [HarmonyPriority(Priority.Last)]
            public static void Postfix()
            {
                //Debug.Log("DB_Intitialize");
                if (ArchipelagoNotIncluded.info?.apModItems.Count > 0)
                {
                    Debug.Log($"There are {ArchipelagoNotIncluded.info.apModItems.Count} items added by mods");
                    foreach (string modItemID in ArchipelagoNotIncluded.info.apModItems)
                    {
                        ModItem modItem = ArchipelagoNotIncluded.AllModItems.Find(i => i.internal_name == modItemID);
                        if (modItem != null)
                        {
                            Tech tech = Db.Get().Techs.TryGet(modItem.internal_tech);
                            if (tech != null)
                                tech.RemoveUnlockedItemIDs(modItemID);
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Techs))]
        [HarmonyPatch("Init")]
        public class Techs_Init_Patch
        {
            [HarmonyPriority(Priority.First)]
            public static bool Prefix(Techs __instance)
            {
                Debug.Log("Techs_Init");
                //If there is no info, run the normal tech init function
                if (ArchipelagoNotIncluded.info is null || ArchipelagoNotIncluded.info.technologies is null)
                {
                    Debug.Log("No mod json could be loaded. Skipping mod override");
                    return true;
                }

                if (!ArchipelagoNotIncluded.Options.CreateModList)
                {
                    foreach (KeyValuePair<string, List<string>> pair in ArchipelagoNotIncluded.info.technologies)
                    {
                        Debug.Log($"Generating research for {pair.Key}, ({pair.Value.Join(s => s, ",")})");
                        Tech tech = __instance.TryGet(pair.Key);
                        Dictionary<string, float> researchCost = null;
                        //List<string> idList = new List<string>();
                        if (ArchipelagoNotIncluded.cheatmode)
                            researchCost = new Dictionary<string, float>
                            {
                                {"basic", 1f },
                                {"advanced", 0f },
                                {"nuclear", 0f },
                                {"orbital", 0f }
                            };
                        if (tech == null)
                            tech = new Tech(pair.Key, new List<string>(), __instance, researchCost);
                        foreach (string techitemidplayer in pair.Value)
                        {
                            string[] splits = techitemidplayer.Split(new string[] { ">>" }, StringSplitOptions.RemoveEmptyEntries);
                            string techitemid = splits[0];
                            int playerid = int.Parse(splits[1]);
                            //idList.Add(techitemid);
                            //Debug.Log($"Player: {ArchipelagoNotIncluded.info.AP_PlayerID} ItemID: {techitemid} PlayerID: {playerid}");
                            if (ArchipelagoNotIncluded.info.AP_PlayerID == playerid && !techitemid.StartsWith("Care Package"))
                            {
                                //Debug.Log("Item was given default sprite");
                                tech.AddUnlockedItemIDs(new string[] { techitemid });
                            }
                            else
                            {
                                //Debug.Log("Item was given custom sprite");
                                techitemid += playerid;
                                if (ArchipelagoNotIncluded.cheatmode)
                                    InjectionMethods.AddItemToTechnologyKanim(techitemid, pair.Key, techitemid, "A mysterious item from another world", "apItemSprite_kanim");
                                else
                                    InjectionMethods.AddItemToTechnologyKanim(techitemid, pair.Key, "Unknown Artifact", "A mysterious item from another world", "apItemSprite_kanim");
                            }
                        }

                        //ArchipelagoNotIncluded.TechList.Add(tech.Id, new List<string>(tech.unlockedItemIDs));
                        //new Tech(pair.Key, pair.Value.ToList(), __instance);
                    }
                }

                foreach (KeyValuePair<string, List<string>> pair in ArchipelagoNotIncluded.Sciences)
                {
                    if (ArchipelagoNotIncluded.Options.CreateModList || !ArchipelagoNotIncluded.info.technologies.ContainsKey(pair.Key))
                    {
                        Debug.Log($"Generating Default Research for {pair.Key}, ({pair.Value.Join(s => s, ",")})");
                        new Tech(pair.Key, pair.Value.ToList(), __instance);
                    }
                }

                Tech preUnlockedTechs = new Tech("PreUnlockedTechs", ArchipelagoNotIncluded.PreUnlockedTech, __instance);
                Db.Get().Techs.resources.Add(preUnlockedTechs);
                
                return false;
            }
        }

        [HarmonyPatch(typeof(SelectModuleSideScreen))]
        [HarmonyPatch(nameof(SelectModuleSideScreen.SpawnButtons))]
        public static class SpawnButtons_Patch
        {   
            public static bool Prefix(SelectModuleSideScreen __instance)
            {
                __instance.ClearButtons();
                GameObject gameObject1 = Util.KInstantiateUI(__instance.categoryPrefab, __instance.categoryContent, true);
                HierarchyReferences component = gameObject1.GetComponent<HierarchyReferences>();
                __instance.categories.Add(gameObject1);
                component.GetReference<LocText>("label");
                Transform reference = component.GetReference<Transform>("content");
                List<GameObject> prefabsWithComponent = Assets.GetPrefabsWithComponent<RocketModuleCluster>();
                foreach (string str in SelectModuleSideScreen.moduleButtonSortOrder)
                {
                    if (!CheckItemList(str))
                        continue;
                    string id = str;
                    GameObject part = prefabsWithComponent.Find((Predicate<GameObject>)(p => p.PrefabID().Name == id));
                    if ((UnityEngine.Object)part == (UnityEngine.Object)null)
                    {
                        Debug.LogWarning((object)("Found an id [" + id + "] in moduleButtonSortOrder in SelectModuleSideScreen.cs that doesn't have a corresponding rocket part!"));
                    }
                    else
                    {
                        GameObject gameObject2 = Util.KInstantiateUI(__instance.moduleButtonPrefab, reference.gameObject, true);
                        gameObject2.GetComponentsInChildren<UnityEngine.UI.Image>()[1].sprite = Def.GetUISprite((object)part).first;
                        LocText componentInChildren = gameObject2.GetComponentInChildren<LocText>();
                        componentInChildren.text = part.GetProperName();
                        componentInChildren.alignment = TextAlignmentOptions.Bottom;
                        componentInChildren.enableWordWrapping = true;
                        gameObject2.GetComponent<MultiToggle>().onClick += (System.Action)(() => __instance.SelectModule(part.GetComponent<Building>().Def));
                        __instance.buttons.Add(part.GetComponent<Building>().Def, gameObject2);
                        if ((UnityEngine.Object)__instance.selectedModuleDef != (UnityEngine.Object)null)
                            __instance.SelectModule(__instance.selectedModuleDef);
                    }
                }
                __instance.UpdateBuildableStates();
                return false;
            }
        }

        [HarmonyPatch(typeof(DestinationSelectPanel))]
        [HarmonyPatch(nameof(DestinationSelectPanel.RePlaceAsteroids))]
        public static class RePlaceAsteroids_Patch
        {
            public static bool Prefix(DestinationSelectPanel __instance)
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
                    __instance.clusterKeys.Add(cluster);
                    __instance.dragTarget.onBeginDrag -= new System.Action(__instance.BeginDrag);
                    __instance.dragTarget.onDrag -= new System.Action(__instance.Drag);
                    __instance.dragTarget.onEndDrag -= new System.Action(__instance.EndDrag);
                    __instance.leftArrowButton.gameObject.SetActive(false);
                    __instance.rightArrowButton.gameObject.SetActive(false);
                }
                //foreach (string cluster in __instance.clusterKeys)
                //    Debug.Log($"Cluster name: {cluster}");

                ArchipelagoNotIncluded.ItemList.Clear();
                ArchipelagoNotIncluded.ItemListDetailed.Clear();
                ArchipelagoNotIncluded.DebugWasUsed = false;
                ArchipelagoNotIncluded.lastIndexSaved = 0;
                ArchipelagoNotIncluded.runCount = 0;
                ArchipelagoNotIncluded.planetText = string.Empty;
                return true;
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
                if (ArchipelagoNotIncluded.info.teleporter)
                    CustomGameSettings.Instance.SetQualitySetting(CustomGameSettingConfigs.Teleporters, "Enabled");
                else
                    CustomGameSettings.Instance.SetQualitySetting(CustomGameSettingConfigs.Teleporters, "Disabled");
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
                        "Unobtanium", "OxyRock", "Snow", "SolidCarbonDioxide", "CrushedIce", "Brine Ice", "Slickster"
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
            public static bool Prefix(string achievement)
            {
                string goal = ArchipelagoNotIncluded.info.goal;
                Debug.Log($"New Achievement: {achievement} Goal: {goal}");
                if (goal == "research_all" && achievement == "CompleteResearchTree")
                    ArchipelagoNotIncluded.netmon.session.SetGoalAchieved();
                else if (goal == "space" && achievement == "space_race")
                    ArchipelagoNotIncluded.netmon.session.SetGoalAchieved();
                else if (goal == "home_sweet_home" && achievement == "thriving")
                    ArchipelagoNotIncluded.netmon.session.SetGoalAchieved();
                else if (goal == "great_escape" && achievement == "ReachedDistantPlanet")
                    ArchipelagoNotIncluded.netmon.session.SetGoalAchieved();
                else if (goal == "cosmic_archaeology" && achievement == "CollectedArtifacts")
                    ArchipelagoNotIncluded.netmon.session.SetGoalAchieved();
                return true;
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
        [HarmonyPatch("CheckBuyResearch")]
        public static class Research_CheckBuyResearch_Patch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                bool startPatch = false;
                var fieldInfo = typeof(Research).GetFields(AccessTools.all).First(field => field.FieldType == typeof(TechInstance));

                foreach (CodeInstruction instruction in instructions)
                {
                    if (!startPatch && instruction.Calls(AccessTools.Method(typeof(TechInstance), nameof(TechInstance.Purchased))))
                    {
                        //Debug.Log("Found method");
                        startPatch = true;
                        yield return instruction;
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, fieldInfo);
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Research_CheckBuyResearch_Patch), nameof(Research_CheckBuyResearch_Patch.SendArchipelagoCheck)));
                        continue;
                    }
                    if (startPatch)
                    {
                        if (instruction.Calls(AccessTools.Method(typeof(KMonoBehaviour), nameof(KMonoBehaviour.Trigger))))
                            //if (instruction.Calls(AccessTools.Method(typeof(Research), nameof(Research.GetNextTech))))
                            startPatch = false;
                        yield return instruction;
                        continue;
                    }
                    yield return instruction;
                }

                /*var startIndex = -1;
                var endIndex = -1;

                List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
                for (var i = 0; i < codes.Count - 1; i++)
                {
                    // Debug.Log(i);
                    // Debug.Log(codes[i].operand);
                    if (codes[i].operand == null)
                        continue;
                    var strOperand = codes[i].operand.ToString();
                    // Find Unique line right before code to be removed
                    if (strOperand.Contains("Purchased"))
                        startIndex = i + 1;

                    // Find Unique line at the end of what needs to be removed
                    if (strOperand.Contains("Trigger"))
                        endIndex = i;
                }
                Debug.Log(startIndex);
                Debug.Log(endIndex);
                if (startIndex > -1 && endIndex > -1)
                {
                    // we cannot remove the first code of our range since some jump actually jumps to
                    // it, so we replace it with a no-op instead of fixing that jump (easier).
                    codes[startIndex].opcode = OpCodes.Nop;
                    codes.RemoveRange(startIndex + 1, endIndex - startIndex);
                }

                return codes.AsEnumerable();*/
            }

            static void SendArchipelagoCheck(TechInstance instance)
            {
                char[] delimiters = { '<', '>' };
                string name = instance.tech.Name.Split(delimiters)[2];
                Debug.Log($"Name: {name}");
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
                //Debug.Log("Found update method");
                if (isModItem(__instance.Id))
                    return;
                __result = __result & CheckItemList(__instance);
                //__result = true;
            }
        }

        [HarmonyPatch(typeof(Tech))]
        [HarmonyPatch(nameof(Tech.ArePrerequisitesComplete))]
        public static class ArePrerequisitesComplete_Patch
        {
            public static bool Prefix(TechItem __instance, ref bool __result)
            {
                //Debug.Log("Found update method");
                if (isModItem(__instance.Id))
                    return true;
                //__result = true;
                __result = __result & CheckItemList(__instance);
                return false;
            }
        }

        /*[HarmonyPatch(typeof(BuildMenu))]
        [HarmonyPatch("BuildableState")]
        public static class BuildableState_Patch
        {
            public static void Postfix(ref PlanScreen.RequirementsState __result, BuildingDef def)
            {
                if (__result == PlanScreen.RequirementsState.Tech)
                {
                    Debug.Log($"ID: {def.PrefabID}");
                }
            }
        }*/

        [HarmonyPatch(typeof(PlanBuildingToggle))]
        [HarmonyPatch("Config")]
        public static class Config_Patch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                bool startPatch = false;
                bool patchDone = false;
                var fieldInfo = typeof(PlanBuildingToggle).GetFields(AccessTools.all).First(field => field.FieldType == typeof(TechItem));
                var copy = new List<CodeInstruction>();

                foreach (CodeInstruction instruction in instructions)
                {
                    if (patchDone)
                    {
                        yield return instruction;
                        continue;
                    }
                    if (!startPatch && instruction.StoresField(fieldInfo))
                    {
                        //Debug.Log("Found method");
                        startPatch = true;
                        yield return instruction;
                        continue;
                    }
                    if (startPatch)
                    {
                        if (instruction.Calls(AccessTools.Method(typeof(List<int>), nameof(List<int>.Add))))
                        {
                            //if (instruction.Calls(AccessTools.Method(typeof(Research), nameof(Research.GetNextTech))))
                            startPatch = false;
                            yield return instruction;
                            foreach (CodeInstruction call in copy)
                                yield return call;
                            patchDone = true;
                        }
                        if (instruction.opcode == OpCodes.Ldc_I4)
                            copy.Add(new CodeInstruction(OpCodes.Ldc_I4, (int)11390976));
                        else
                            copy.Add(instruction);
                        yield return instruction;
                        continue;
                    }
                    yield return instruction;
                }
            }
        }

        /*[HarmonyPatch(typeof(BuildMenuBuildingsScreen))]
        [HarmonyPatch("RefreshToggle")]
        public static class RefreshToggle_Patch
        {
            public static bool Prefix(KIconToggleMenu.ToggleInfo info)
            {
                if (info == null || (UnityEngine.Object)info.toggle == (UnityEngine.Object)null)
                    return false;
                BuildingDef def = (info.userData as BuildMenuBuildingsScreen.UserData).def;
                TechItem techItem = Db.Get().TechItems.TryGet(def.PrefabID);
            }
        }*/

        [HarmonyPatch(typeof(PlanScreen))]
        [HarmonyPatch("OnPrefabInit")]
        public static class OnPrefabInit_Patch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                bool startPatch = false;
                bool patchDone = false;
                var copy = new List<CodeInstruction>();

                foreach (CodeInstruction instruction in instructions)
                {
                    if (patchDone)
                    {
                        yield return instruction;
                        continue;
                    }
                    if (!startPatch && instruction.Calls(AccessTools.DeclaredPropertyGetter(typeof(Game), nameof(Game.Instance))))
                    {
                        //Debug.Log("Found method");
                        startPatch = true;
                        copy.Add(instruction);
                        yield return instruction;
                        continue;
                    }
                    if (startPatch)
                    {
                        if (instruction.opcode == OpCodes.Pop)
                        {
                            //if (instruction.Calls(AccessTools.Method(typeof(Research), nameof(Research.GetNextTech))))
                            startPatch = false;
                            yield return instruction;
                            foreach (CodeInstruction call in copy)
                                yield return call;
                            patchDone = true;
                        }
                        if (instruction.opcode == OpCodes.Ldc_I4)
                            copy.Add(new CodeInstruction(OpCodes.Ldc_I4, (int)11390976));
                        else
                            copy.Add(instruction);
                        yield return instruction;
                        continue;
                    }
                    yield return instruction;
                }
            }
        }

        [HarmonyPatch(typeof(BuildMenuBuildingsScreen))]
        [HarmonyPatch("OnSpawn")]
        public static class OnSpawn_Patch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                bool startPatch = false;
                bool patchDone = false;
                var copy = new List<CodeInstruction>();

                foreach (CodeInstruction instruction in instructions)
                {
                    if (patchDone)
                    {
                        yield return instruction;
                        continue;
                    }
                    if (!startPatch && instruction.Calls(AccessTools.DeclaredPropertyGetter(typeof(Game), nameof(Game.Instance))))
                    {
                        //Debug.Log("Found method");
                        startPatch = true;
                        copy.Add(instruction);
                        yield return instruction;
                        continue;
                    }
                    if (startPatch)
                    {
                        if (instruction.opcode == OpCodes.Pop)
                        {
                            //if (instruction.Calls(AccessTools.Method(typeof(Research), nameof(Research.GetNextTech))))
                            startPatch = false;
                            yield return instruction;
                            foreach (CodeInstruction call in copy)
                                yield return call;
                            patchDone = true;
                        }
                        if (instruction.opcode == OpCodes.Ldc_I4)
                            copy.Add(new CodeInstruction(OpCodes.Ldc_I4, (int)11390976));
                        else
                            copy.Add(instruction);
                        yield return instruction;
                        continue;
                    }
                    yield return instruction;
                }
            }
        }

        [HarmonyPatch(typeof(BuildMenuBuildingsScreen))]
        [HarmonyPatch("RefreshToggle")]
        public static class RefreshToggle_Patch
        {
            /*static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                bool startPatch = false;
                bool patchDone = false;
                var fieldInfo = typeof(DebugHandler).GetFields(AccessTools.all).First(field => field.Name == "InstantBuildMode");
                var copy = new List<CodeInstruction>();

                foreach (CodeInstruction instruction in instructions)
                {
                    if (patchDone)
                    {
                        yield return instruction;
                        continue;
                    }
                    if (!startPatch && instruction.Calls(AccessTools.Method(typeof(TechItem), nameof(TechItem.IsComplete))))
                    {
                        Debug.Log("Found method");
                        startPatch = true;
                        yield return instruction;
                        continue;
                    }
                    if (startPatch)
                    {
                        if (instruction.opcode == OpCodes.Pop)
                        {
                            //if (instruction.Calls(AccessTools.Method(typeof(Research), nameof(Research.GetNextTech))))
                            startPatch = false;
                            yield return instruction;
                            foreach (CodeInstruction call in copy)
                                yield return call;
                            patchDone = true;
                        }
                        if (instruction.opcode == OpCodes.Ldc_I4)
                            copy.Add(new CodeInstruction(OpCodes.Ldc_I4, (int)11390976));
                        else
                            copy.Add(instruction);
                        //Debug.Log(instruction.opcode.ToString());
                        if (instruction.opcode == OpCodes.Brfalse_S)
                        {
                            instruction.opcode = OpCodes.Brtrue_S;
                            patchDone = true;
                        }
                        yield return instruction;
                        continue;
                    }
                    yield return instruction;
                }
            }*/
        }

        [HarmonyPatch(typeof(BuildMenu))]
        [HarmonyPatch("OnCmpEnable")]
        public static class OnCmpEnable_Patch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                bool startPatch = false;
                bool patchDone = false;
                var copy = new List<CodeInstruction>();

                foreach (CodeInstruction instruction in instructions)
                {
                    if (patchDone)
                    {
                        yield return instruction;
                        continue;
                    }
                    if (!startPatch && instruction.Calls(AccessTools.DeclaredPropertyGetter(typeof(Game), nameof(Game.Instance))))
                    {
                        //Debug.Log("Found method");
                        startPatch = true;
                        copy.Add(instruction);
                        yield return instruction;
                        continue;
                    }
                    if (startPatch)
                    {
                        if (instruction.opcode == OpCodes.Pop)
                        {
                            //if (instruction.Calls(AccessTools.Method(typeof(Research), nameof(Research.GetNextTech))))
                            startPatch = false;
                            yield return instruction;
                            foreach (CodeInstruction call in copy)
                                yield return call;
                            patchDone = true;
                        }
                        if (instruction.opcode == OpCodes.Ldc_I4)
                            copy.Add(new CodeInstruction(OpCodes.Ldc_I4, (int)11390976));
                        else
                            copy.Add(instruction);
                        yield return instruction;
                        continue;
                    }
                    yield return instruction;
                }
            }
        }

        [HarmonyPatch(typeof(BuildMenu))]
        [HarmonyPatch("OnCmpDisable")]
        public static class OnCmpDisable_Patch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                bool startPatch = false;
                bool patchDone = false;
                var copy = new List<CodeInstruction>();

                foreach (CodeInstruction instruction in instructions)
                {
                    if (patchDone)
                    {
                        yield return instruction;
                        continue;
                    }
                    if (!startPatch && instruction.Calls(AccessTools.DeclaredPropertyGetter(typeof(Game), nameof(Game.Instance))))
                    {
                        //Debug.Log("Found method");
                        startPatch = true;
                        copy.Add(instruction);
                        yield return instruction;
                        continue;
                    }
                    if (startPatch)
                    {
                        if (instruction.operand != (object)null && instruction.operand.ToString().Contains("Unsubscribe"))
                        {
                            //if (instruction.Calls(AccessTools.Method(typeof(Research), nameof(Research.GetNextTech))))
                            startPatch = false;
                            yield return instruction;
                            foreach (CodeInstruction call in copy)
                                yield return call;
                            patchDone = true;
                        }
                        if (instruction.opcode == OpCodes.Ldc_I4)
                            copy.Add(new CodeInstruction(OpCodes.Ldc_I4, (int)11390976));
                        else
                            copy.Add(instruction);
                        yield return instruction;
                        continue;
                    }
                    yield return instruction;
                }
            }
        }

        [HarmonyPatch(typeof(ConsumerManager))]
        [HarmonyPatch("OnSpawn")]
        public static class Consumer_OnSpawn_Patch
        {
            public static void Postfix(ConsumerManager __instance)
            {
                //Debug.Log("Found update method");
                Game.Instance.Subscribe(11390976, new Action<object>(__instance.RefreshDiscovered));
            }
        }

        [HarmonyPatch(typeof(MaterialSelectionPanel))]
        [HarmonyPatch("OnPrefabInit")]
        public static class MaterialSelectionPanel_OnPrefabInit_Patch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                bool startPatch = false;
                bool patchDone = false;
                bool fixLdarg = false;
                var fieldInfo = typeof(MaterialSelectionPanel).GetFields(AccessTools.all).First(field => field.FieldType == typeof(List<int>));
                var copy = new List<CodeInstruction>();

                foreach (CodeInstruction instruction in instructions)
                {
                    if (patchDone)
                    {
                        yield return instruction;
                        continue;
                    }
                    if (!startPatch && instruction.operand != (object)null && instruction.operand.ToString().Contains("SetAsLastSibling"))
                    //if (!startPatch && instruction.opcode.Name == "Label2")
                    {
                        //Debug.Log("Found method");
                        startPatch = true;
                        yield return instruction;
                        continue;
                    }
                    if (startPatch)
                    {
                        if (instruction.Calls(AccessTools.Method(typeof(List<int>), nameof(List<int>.Add))))
                        {
                            //if (instruction.Calls(AccessTools.Method(typeof(Research), nameof(Research.GetNextTech))))
                            startPatch = false;
                            yield return instruction;
                            foreach (CodeInstruction call in copy)
                                yield return call;
                            patchDone = true;
                        }
                        if (!fixLdarg && instruction.opcode == OpCodes.Ldarg_0)
                        {
                            var code = new CodeInstruction(OpCodes.Ldarg_0);
                            copy.Add(code);
                            yield return instruction;
                            fixLdarg = true;
                            continue;
                        }
                        if (instruction.opcode == OpCodes.Ldc_I4)
                            copy.Add(new CodeInstruction(OpCodes.Ldc_I4, (int)11390976));
                        else
                            copy.Add(instruction);
                        yield return instruction;
                        continue;
                    }
                    yield return instruction;
                }
            }
        }

        [HarmonyPatch(typeof(SelectModuleSideScreen))]
        [HarmonyPatch("OnCmpEnable")]
        public static class SelectModuleSideScreen_OnCmpEnable_Patch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                bool startPatch = false;
                bool patchDone = false;
                var fieldInfo = typeof(MaterialSelectionPanel).GetFields(AccessTools.all).First(field => field.FieldType == typeof(List<int>));
                var copy = new List<CodeInstruction>();

                foreach (CodeInstruction instruction in instructions)
                {
                    if (patchDone)
                    {
                        yield return instruction;
                        continue;
                    }
                    if (!startPatch && instruction.Calls(AccessTools.Method(typeof(SelectModuleSideScreen), "ClearSubscriptionHandles")))
                    //if (!startPatch && instruction.opcode.Name == "Label2")
                    {
                        //Debug.Log("Found method");
                        startPatch = true;
                        yield return instruction;
                        continue;
                    }
                    if (startPatch)
                    {
                        if (instruction.Calls(AccessTools.Method(typeof(List<int>), nameof(List<int>.Add))))
                        {
                            //if (instruction.Calls(AccessTools.Method(typeof(Research), nameof(Research.GetNextTech))))
                            startPatch = false;
                            yield return instruction;
                            foreach (CodeInstruction call in copy)
                                yield return call;
                            patchDone = true;
                        }
                        if (instruction.opcode == OpCodes.Ldc_I4)
                            copy.Add(new CodeInstruction(OpCodes.Ldc_I4, (int)11390976));
                        else
                            copy.Add(instruction);
                        yield return instruction;
                        continue;
                    }
                    yield return instruction;
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
            if (ArchipelagoNotIncluded.Options.CreateModList)
                return false;

            char[] delimiters = { '<', '>' };
            string name = ArchipelagoNotIncluded.CleanName(TechItem.Name);
            //Debug.Log($"Name: {TechItem.Name}, Id: {TechItem.Id}");

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

        [HarmonyPatch(typeof(PlayerController))]
        [HarmonyPatch(nameof(PlayerController.Update))]
        //[HarmonyPatch(typeof(AutoDisinfectableManager))]
        //[HarmonyPatch(nameof(AutoDisinfectableManager.Sim1000ms))]
        public static class PlayerController_Update_Patch
        {
            public static void Postfix()
            {
                //Debug.Log("Found update method");
                if (APNetworkMonitor.packageQueue.Count == 0)
                    return;

                //CarePackageInfo package = new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Dirt).tag.ToString(), 500f, (Func<bool>)null);
                Telepad telepad = Components.Telepads[0];
                telepad.smi.sm.openPortal.Trigger(telepad.smi);
                //package.Deliver(telepad.transform.GetPosition());
                while (APNetworkMonitor.packageQueue.TryDequeue(out string package))
                {
                    ArchipelagoNotIncluded.netmon.CarePackages[package].Deliver(telepad.transform.GetPosition());
                }
                telepad.smi.sm.closePortal.Trigger(telepad.smi);
            }
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(PlanScreen))]
        [HarmonyPatch("OnResearchComplete")]
        public class OnResearchComplete_Patch
        {
            public static void AddTechItem(object instance, object data)
            {
                //Debug.Log("Found update method");

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

                //Debug.Log("Found update method");
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
                bool allow = ArchipelagoNotIncluded.allowResourceChecks;
                if (allow)
                    writer.Write(1);
                else
                    writer.Write(0);
                writer.Write(ArchipelagoNotIncluded.getLastIndex());
            }
            /*static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                bool methodFound = false;
                bool patchDone = false;
                MethodInfo firstMethod = AccessTools.Method(typeof(IDisposable), nameof(IDisposable.Dispose));
                MethodInfo secondMethod = AccessTools.Method(typeof(BinaryWriter), nameof(BinaryWriter.Write), parameters: new[] { typeof(int) });
                MethodInfo myMethod = AccessTools.Method(typeof(ArchipelagoNotIncluded), nameof(ArchipelagoNotIncluded.getLastIndex));
                
                foreach (CodeInstruction instruction in instructions)
                {
                    if (instruction.Calls(myMethod))
                        patchDone = true;
                }

                foreach (CodeInstruction instruction in instructions)
                {
                    if (patchDone)
                    {
                        yield return instruction;
                        continue;
                    }
                    if (!methodFound && instruction.Calls(firstMethod))
                    {
                        methodFound = true;
                        yield return instruction;
                        continue;
                    }
                    if (methodFound && instruction.Calls(secondMethod))
                    {
                        //Debug.Log("Found method");
                        patchDone = true;
                        yield return instruction;
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        yield return new CodeInstruction(OpCodes.Call, myMethod);
                        yield return new CodeInstruction(OpCodes.Callvirt, secondMethod);
                        continue;
                    }
                    yield return instruction;
                }
            }*/
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
                    int index = reader.ReadInt32();
                    ArchipelagoNotIncluded.setLastIndex(index);
                    APNetworkMonitor.HighestCount = index;
                }
                catch { }
            }
            /*public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                if (ArchipelagoNotIncluded.Options.CreateModList)
                {
                    foreach (CodeInstruction instruction in instructions)
                        yield return instruction;
                    yield break;
                }

                bool methodFound = false;
                bool startPatch = false;
                bool patchDone = false;
                MethodInfo firstMethod = AccessTools.Method(typeof(SaveManager), "ClearScene");
                MethodInfo secondMethod = AccessTools.Method(typeof(IReader), nameof(IReader.ReadInt32));
                MethodInfo myMethod = AccessTools.Method(typeof(ArchipelagoNotIncluded), nameof(ArchipelagoNotIncluded.setLastIndex));

                foreach (CodeInstruction instruction in instructions)
                {
                    if (instruction.Calls(myMethod))
                        patchDone = true;
                }

                foreach (CodeInstruction instruction in instructions)
                {
                    if (patchDone)
                    {
                        yield return instruction;
                        continue;
                    }
                    if (startPatch)
                    {
                        yield return instruction;
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        yield return new CodeInstruction(OpCodes.Callvirt, secondMethod);
                        yield return new CodeInstruction(OpCodes.Callvirt, myMethod);
                        patchDone = true;
                        continue;
                    }
                    if (!methodFound && instruction.Calls(firstMethod))
                    {
                        methodFound = true;
                        yield return instruction;
                        continue;
                    }
                    if (methodFound && instruction.Calls(secondMethod))
                    {
                        //Debug.Log("Found method");
                        startPatch = true;
                        yield return instruction;
                        continue;
                    }
                    yield return instruction;
                }
            }*/
        }

        [HarmonyPatch(typeof(SuitFabricatorConfig))]
        [HarmonyPatch("ConfigureRecipes")]
        public class ConfigureRecipes_Patch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                //bool methodFound = false;
                bool startPatch = false;
                string tech = "Jobs";
                MethodInfo method = AccessTools.Method(typeof(Db), nameof(Db.Get));
                FieldInfo atmosuit = AccessTools.Field(typeof(TechItems), nameof(TechItems.atmoSuit));
                FieldInfo jetsuit = AccessTools.Field(typeof(TechItems), nameof(TechItems.jetSuit));
                FieldInfo leadsuit = AccessTools.Field(typeof(TechItems), nameof(TechItems.leadSuit));
                FieldInfo field = AccessTools.Field(typeof(TechItem), nameof(TechItem.parentTechId));
                /*string atmo = ArchipelagoNotIncluded.info.technologies.FirstOrDefault(p => p.Value.Contains("AtmoSuit")).Key;
                string jet = ArchipelagoNotIncluded.info.technologies.FirstOrDefault(p => p.Value.Contains("JetSuit")).Key;
                string lead = ArchipelagoNotIncluded.info.technologies.FirstOrDefault(p => p.Value.Contains("LeadSuit")).Key;*/

                foreach (CodeInstruction instruction in instructions)
                {
                    if (!startPatch && instruction.Calls(method))
                    {
                        //Debug.Log("Found method");
                        startPatch = true;
                    }
                    if (startPatch)
                    {
                        if (instruction.LoadsField(atmosuit) || instruction.LoadsField(jetsuit) || instruction.LoadsField(leadsuit))
                            yield return new CodeInstruction(OpCodes.Ldstr, tech);
                        /*if (instruction.LoadsField(jetsuit))
                            yield return new CodeInstruction(OpCodes.Ldstr, jet);
                        if (instruction.LoadsField(leadsuit))
                            yield return new CodeInstruction(OpCodes.Ldstr, lead);*/
                        if (instruction.LoadsField(field))
                            startPatch = false;
                        continue;
                    }
                    yield return instruction;
                }
            }
        }

        //[HarmonyDebug]
        [HarmonyPatch(typeof(CraftingTableConfig))]
        [HarmonyPatch("ConfigureRecipes")]
        public class ConfigureRecipes2_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                //bool methodFound = false;
                //bool firstLineFound = false;
                //bool secondLineFound = false;
                bool startPatch = false;
                string tech = "Jobs";
                MethodInfo method = AccessTools.Method(typeof(Db), nameof(Db.Get));
                FieldInfo electrobank = AccessTools.Field(typeof(TechItems), nameof(TechItems.disposableElectrobankUraniumOre));
                FieldInfo oxygenmask = AccessTools.Field(typeof(TechItems), nameof(TechItems.oxygenMask));
                FieldInfo field = AccessTools.Field(typeof(TechItem), nameof(TechItem.parentTechId));

                foreach (CodeInstruction instruction in instructions)
                {
                    // BEGIN SKIP FOR BIONIC RECIPES
                    /*if (!firstLineFound)
                    {
                        if (instruction.LoadsField(field))
                            firstLineFound = true;
                        continue;
                    }
                    if (!secondLineFound)
                    {
                        if (instruction.opcode == OpCodes.Ldc_I4_1)
                        {
                            secondLineFound = true;
                            yield return instruction;
                        }
                        continue;
                    }*/
                    // END SKIP
                    if (!startPatch && instruction.Calls(method))
                    {
                        //Debug.Log("Found method");
                        startPatch = true;
                    }
                    if (startPatch)
                    {
                        if (instruction.LoadsField(electrobank))
                            yield return new CodeInstruction(OpCodes.Ldstr, tech);
                        if (instruction.LoadsField(oxygenmask))
                            yield return new CodeInstruction(OpCodes.Ldstr, tech);
                        if (instruction.LoadsField(field))
                            startPatch = false;
                        continue;
                    }
                    yield return instruction;
                }
            }
        }

        [HarmonyPatch(typeof(AdvancedCraftingTableConfig))]
        [HarmonyPatch("ConfigureRecipes")]
        public class ConfigureRecipes3_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                //bool methodFound = false;
                //bool firstLineFound = false;
                //bool secondLineFound = false;
                bool startPatch = false;
                string tech = "Jobs";
                MethodInfo method = AccessTools.Method(typeof(Db), nameof(Db.Get));
                FieldInfo electrobank = AccessTools.Field(typeof(TechItems), nameof(TechItems.electrobank));
                FieldInfo fetchdrone = AccessTools.Field(typeof(TechItems), nameof(TechItems.fetchDrone));
                FieldInfo field = AccessTools.Field(typeof(TechItem), nameof(TechItem.parentTechId));

                foreach (CodeInstruction instruction in instructions)
                {
                    // BEGIN SKIP FOR BIONIC RECIPES
                    /*if (!firstLineFound)
                    {
                        if (instruction.LoadsField(field))
                            firstLineFound = true;
                        continue;
                    }
                    if (!secondLineFound)
                    {
                        if (instruction.opcode == OpCodes.Ldc_I4_1)
                        {
                            secondLineFound = true;
                            yield return instruction;
                        }
                        continue;
                    }*/
                    // END SKIP
                    if (!startPatch && instruction.Calls(method))
                    {
                        //Debug.Log("Found method");
                        startPatch = true;
                    }
                    if (startPatch)
                    {
                        if (instruction.LoadsField(electrobank))
                            yield return new CodeInstruction(OpCodes.Ldstr, tech);
                        if (instruction.LoadsField(fetchdrone))
                            yield return new CodeInstruction(OpCodes.Ldstr, tech);
                        if (instruction.LoadsField(field))
                            startPatch = false;
                        continue;
                    }
                    yield return instruction;
                }
            }
        }

        [HarmonyPatch(typeof(LubricationStickConfig))]
        [HarmonyPatch("CreatePrefab")]
        public class CreatePrefab_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                //bool methodFound = false;
                //bool firstLineFound = false;
                //bool secondLineFound = false;
                bool startPatch = false;
                string tech = "Jobs";
                MethodInfo method = AccessTools.Method(typeof(Db), nameof(Db.Get));
                FieldInfo electrobank = AccessTools.Field(typeof(TechItems), nameof(TechItems.lubricationStick));
                FieldInfo field = AccessTools.Field(typeof(TechItem), nameof(TechItem.parentTechId));

                foreach (CodeInstruction instruction in instructions)
                {
                    // BEGIN SKIP FOR BIONIC RECIPES
                    /*if (!firstLineFound)
                    {
                        if (instruction.LoadsField(field))
                            firstLineFound = true;
                        continue;
                    }
                    if (!secondLineFound)
                    {
                        if (instruction.opcode == OpCodes.Ldc_I4_1)
                        {
                            secondLineFound = true;
                            yield return instruction;
                        }
                        continue;
                    }*/
                    // END SKIP
                    if (!startPatch && instruction.Calls(method))
                    {
                        //Debug.Log("Found method");
                        startPatch = true;
                    }
                    if (startPatch)
                    {
                        if (instruction.LoadsField(electrobank))
                            yield return new CodeInstruction(OpCodes.Ldstr, tech);
                        if (instruction.LoadsField(field))
                            startPatch = false;
                        continue;
                    }
                    yield return instruction;
                }
            }
        }

        [HarmonyPatch(typeof(SupermaterialRefineryConfig))]
        [HarmonyPatch("ConfigureBuildingTemplate")]
        public class ConfigureBuildingTemplate_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                //bool methodFound = false;
                //bool firstLineFound = false;
                //bool secondLineFound = false;
                bool startPatch = false;
                string tech = "Jobs";
                MethodInfo method = AccessTools.Method(typeof(Db), nameof(Db.Get));
                FieldInfo electrobank = AccessTools.Field(typeof(TechItems), nameof(TechItems.selfChargingElectrobank));
                FieldInfo field = AccessTools.Field(typeof(TechItem), nameof(TechItem.parentTechId));

                foreach (CodeInstruction instruction in instructions)
                {
                    // BEGIN SKIP FOR BIONIC RECIPES
                    /*if (!firstLineFound)
                    {
                        if (instruction.LoadsField(field))
                            firstLineFound = true;
                        continue;
                    }
                    if (!secondLineFound)
                    {
                        if (instruction.opcode == OpCodes.Ldc_I4_1)
                        {
                            secondLineFound = true;
                            yield return instruction;
                        }
                        continue;
                    }*/
                    // END SKIP
                    if (!startPatch && instruction.Calls(method))
                    {
                        //Debug.Log("Found method");
                        startPatch = true;
                    }
                    if (startPatch)
                    {
                        if (instruction.LoadsField(electrobank))
                            yield return new CodeInstruction(OpCodes.Ldstr, tech);
                        if (instruction.LoadsField(field))
                            startPatch = false;
                        continue;
                    }
                    yield return instruction;
                }
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

        /*[HarmonyPatch(typeof(DlcManager))]
        [HarmonyPatch("ShouldLoadDLCAssets")]
        public static class ShouldLoadDLCAssets_Patch
        {
            public static void Postfix(ref bool __result)
            {
                //Debug.Log("Found update method");
                //Debug.Log(__instance.PrefabID);
                __result = true;
            }
        }*/

        [HarmonyPatch(typeof(SpeedControlScreen))]
        [HarmonyPatch(nameof(SpeedControlScreen.Unpause))]
        public static class Unpause_Patch
        {
            public static void Postfix()
            {
                /*if (APNetworkMonitor.packageQueue.Count == 0)
                    return;

                //CarePackageInfo package = new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Dirt).tag.ToString(), 500f, (Func<bool>)null);
                Telepad telepad = Components.Telepads[0];
                telepad.smi.sm.openPortal.Trigger(telepad.smi);
                //package.Deliver(telepad.transform.GetPosition());
                while (APNetworkMonitor.packageQueue.TryDequeue(out string package))
                {
                    ArchipelagoNotIncluded.lastItem++;
                    ArchipelagoNotIncluded.netmon.CarePackages[package].Deliver(telepad.transform.GetPosition());
                }
                telepad.smi.sm.closePortal.Trigger(telepad.smi);*/
            }
        }

        [HarmonyPatch(typeof(DlcManager))]
        [HarmonyPatch(nameof(DlcManager.IsContentSubscribed))]
        public static class IsContentSubscribed_Patch
        {
            public static void Postfix(string dlcId, ref bool __result)
            {
                //Debug.Log("Found update method");
                //Debug.Log(__instance.PrefabID);
                if (dlcId == "DLC3_ID")
                    __result = false;
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
                //Debug.Log("Found update method");
                //Debug.Log(___def.PrefabID);
                //if (!CheckItemList(___def.PrefabID))
                //requirementsState = PlanScreen.RequirementsState.Invalid;
                string search = null;
                foreach (ComplexRecipe.RecipeElement element in __instance.ingredients)
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
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Techs))]
        [HarmonyPatch(nameof(Techs.Load))]
        public static class Techs_Load_Patch
        {
            /*public static MethodInfo TargetMethod()
            {
                return AccessTools.Method(typeof(ResourceLoader<ResourceTreeNode>), "Load", new Type[] { typeof(TextAsset) });
                return typeof(ResourceTreeLoader<>).GetMethod("Load", new Type[] { typeof(TextAsset) });
            }*/
            public static bool Prefix(Techs __instance, List<List<Tuple<string, float>>> ___TECH_TIERS)
            {
                TextAsset tree_file = Db.Get().researchTreeFileExpansion1;
                ResourceTreeLoader<ResourceTreeNode> resourceTreeLoader = new ResourceTreeLoader<ResourceTreeNode>(tree_file);
                List<TechTreeTitle> techTreeTitleList = new List<TechTreeTitle>();
                for (int idx = 0; idx < Db.Get().TechTreeTitles.Count; ++idx)
                    techTreeTitleList.Add(Db.Get().TechTreeTitles[idx]);
                techTreeTitleList.Sort((Comparison<TechTreeTitle>)((a, b) => a.center.y.CompareTo(b.center.y)));

                /*ResourceTreeNode newnode = new ResourceTreeNode
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
                resourceTreeLoader.resources.Add(newnode);*/
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
            /*{
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
            }*/
        }
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
