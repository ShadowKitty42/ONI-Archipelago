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
                        //InjectionMethods.AddItemToTechnologySprite
                        Tech tech = __instance.TryGet(pair.Key);
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
                            tech = new Tech(pair.Key, new List<string>(), __instance, researchCost);
                        foreach (string techitemidplayer in pair.Value)
                        {
                            string[] splits = techitemidplayer.Split(new string[] { ">>" }, StringSplitOptions.RemoveEmptyEntries);
                            string techitemid = splits[0];
                            int playerid = int.Parse(splits[1]);
                            //DefaultItem defItem = ArchipelagoNotIncluded.AllDefaultItems.Find(i => i.internal_name == techitemid);
                            //if (defItem != null || ArchipelagoNotIncluded.info.apModItems.Contains(techitemid))
                            if (ArchipelagoNotIncluded.info.AP_PlayerID == playerid)
                            {
                                //ArchipelagoNotIncluded.apItems.Add(new KeyValuePair<string, string>(techitemid, pair.Key));
                                tech.AddUnlockedItemIDs(new string[] { techitemid });
                            }
                            else
                            {
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

        [HarmonyPatch(typeof(ColonyAchievementTracker))]
        [HarmonyPatch("TriggerNewAchievementCompleted")]
        public static class TriggerNewAchievementCompleted_Patch
        {
            public static bool Prefix(string achievement)
            {
                Debug.Log($"New Achievement: {achievement}");
                if (achievement == "CompleteResearchTree")
                    ArchipelagoNotIncluded.netmon.session.SetGoalAchieved();
                return true;
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
                //Debug.Log($"Name: {name}");
                //Debug.Log($"Count: {instance.tech.unlockedItemIDs.Count}");
                List<DefaultItem> defItem = ArchipelagoNotIncluded.info.spaced_out ? ArchipelagoNotIncluded.AllDefaultItems.FindAll(i => i.tech == name) : ArchipelagoNotIncluded.AllDefaultItems.FindAll(i => i.tech_base == name);
                int count = defItem.Count;
                long[] locationIds = new long[count];
                for (int i = 0; i < count; i++)
                {
                    //Debug.Log($"Location: {name} - {i + 1}");
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
                __result = __result | CheckItemList(__instance);
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
                __result = true;
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
            Debug.Log($"Checking ModItem InternalName: {InternalName}");
            ModItem modItem = ArchipelagoNotIncluded.AllModItems.Find(i => i.internal_name == InternalName);
            if (modItem != null && !modItem.randomized)
                return true;
            Debug.Log("Not ModItem");
            return false;
        }
        static bool CheckItemList(string InternalName)
        {
            if (ArchipelagoNotIncluded.StarterTech.Contains(InternalName))
                return true;

            if (ArchipelagoNotIncluded.Options.CreateModList)
                return false;

            DefaultItem defItem = ArchipelagoNotIncluded.AllDefaultItems.Find(i => i.internal_name == InternalName);
            if (defItem == null)
                return false;
            return (bool)ArchipelagoNotIncluded.netmon?.session?.Items?.AllItemsReceived.Any<ItemInfo>(i => i.ItemDisplayName == defItem.name && i.Player.Name == ArchipelagoNotIncluded.netmon.SlotName);
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
            return (bool)ArchipelagoNotIncluded.netmon?.session?.Items?.AllItemsReceived.Any<ItemInfo>(i => i.ItemDisplayName == name && i.Player.Name == ArchipelagoNotIncluded.netmon.SlotName);
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
        [HarmonyPatch("Update")]
        public static class PlayerController_Update_Patch
        {
            public static void Postfix()
            {
                //Debug.Log("Found update method");
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
        [HarmonyPatch(typeof(Game), "OnPrefabInit")]
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

                ArchipelagoNotIncluded.netmon.UpdateAllItems();
            }
        }

        /*[HarmonyPatch(typeof(SaveManager))]
        [HarmonyPatch("Save")]
        public class Save_Patch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
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
            }
        }

        [HarmonyPatch(typeof(SaveManager))]
        [HarmonyPatch("Load")]
        public class Load_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
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
            }
        }*/

        [HarmonyPatch(typeof(SuitFabricatorConfig))]
        [HarmonyPatch("ConfigureRecipes")]
        public class ConfigureRecipes_Patch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                //bool methodFound = false;
                bool startPatch = false;
                string tech = "Placeholder";
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
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                //bool methodFound = false;
                //bool firstLineFound = false;
                //bool secondLineFound = false;
                bool startPatch = false;
                string tech = "Placeholder";
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

        [HarmonyPatch(typeof(BuildingDef))]
        [HarmonyPatch("IsValidDLC")]
        public static class IsValidDLC_Patch
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

        [HarmonyPatch(typeof(DlcManager))]
        [HarmonyPatch("IsContentSubscribed")]
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
                    Debug.Log(GetLogFor(node));
                    int count = 0;
                    foreach (ResourceTreeNode refNode in node.references)
                    {
                        count++;
                        Debug.Log($"{node.Id} ref #{count}: {GetLogFor(refNode)}");
                    }
                    count = 0;
                    foreach (ResourceTreeNode.Edge edge in node.edges)
                    {
                        count++;
                        Debug.Log($"{node.Id} edge #{count}: {GetLogFor(edge)}");
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
