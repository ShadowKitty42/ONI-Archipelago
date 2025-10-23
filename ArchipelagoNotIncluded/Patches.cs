using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.DataPackage;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using ArchipelagoNotIncluded.Patches;
using Database;
using Epic.OnlineServices.Platform;
using HarmonyLib;
using Klei.AI;
using Klei.CustomSettings;
using KMod;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PeterHan.PLib.Options;
using ProcGen;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using UnityEngineInternal;
using UtilLibs;
using static MathUtil;
using static SeedProducer;

namespace ArchipelagoNotIncluded
{

    public class Patches_old
    {

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

    }
}
