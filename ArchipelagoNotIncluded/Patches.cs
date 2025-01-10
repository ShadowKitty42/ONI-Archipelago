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

namespace ArchipelagoNotIncluded
{
    [HarmonyPatch(typeof(Techs))]
    [HarmonyPatch("Init")]
    public class Patches
    {
        public class Techs_Init_Patch
        {
            [HarmonyPriority(Priority.VeryHigh)]
            public static bool Prefix(Techs __instance)
            {

                //If there is no info, run the normal tech init function
                if (ArchipelagoNotIncluded.info == null)
                {
                    Debug.Log("No mod json could be loaded. Skipping mod override");
                    return true;
                }

                foreach (KeyValuePair<string, List<string>> pair in ArchipelagoNotIncluded.info.technologies)
                {
                    Debug.Log($"Generating research for {pair.Key}, ({pair.Value.Join(s => s, ",")})");
                    new Tech(pair.Key, pair.Value.ToList(), __instance);
                }

                foreach (KeyValuePair<string, List<string>> pair in ArchipelagoNotIncluded.Sciences)
                {
                    if (!ArchipelagoNotIncluded.info.technologies.ContainsKey(pair.Key))
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
        [HarmonyPatch("IsPOIUnlocked")]
        public static class IsPOIUnlocked_Patch
        {
            public static void Postfix(TechItem __instance, ref bool __result)
            {
                //Debug.Log("Found update method");
                __result = __result | CheckItemList(__instance);
                //__result = true;
            }
        }

        [HarmonyPatch(typeof(Tech))]
        [HarmonyPatch("ArePrerequisitesComplete")]
        public static class ArePrerequisitesComplete_Patch
        {
            public static bool Prefix(TechItem __instance, ref bool __result)
            {
                //Debug.Log("Found update method");
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

        static bool CheckItemList(string InternalName)
        {
            if (ArchipelagoNotIncluded.StarterTech.Contains(InternalName))
                return true;

            //Debug.Log($"InternalName: {InternalName}");
            DefaultItem defItem = ArchipelagoNotIncluded.AllDefaultItems.Find(i => i.internal_name == InternalName);
            if (defItem == null)
                return false;
            if (ArchipelagoNotIncluded.netmon?.session?.Items?.AllItemsReceived.Count() == 0)
                return false;
            foreach (ItemInfo item in ArchipelagoNotIncluded.netmon?.session?.Items?.AllItemsReceived)
                if (item.ItemDisplayName == defItem.name)
                    return true;
            return false;
        }

        static bool CheckItemList(TechItem TechItem)
        {
            char[] delimiters = { '<', '>' };
            string name = "";
            if (TechItem.Name.Contains('<'))
            {
                name = TechItem.Name.Split(delimiters)[2];
            }
            else
            {
                name = TechItem.Name;
            }
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
            if (ArchipelagoNotIncluded.netmon?.session?.Items?.AllItemsReceived.Count() == 0)
                return false;
            foreach (ItemInfo item in ArchipelagoNotIncluded.netmon?.session?.Items?.AllItemsReceived)
                if (item.ItemDisplayName == name)
                    return true;
            return false;
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

        [HarmonyPatch(typeof(SaveManager))]
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
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
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
        }

        /*[HarmonyDebug]
        [HarmonyPatch(typeof(SuitFabricatorConfig))]
        [HarmonyPatch("ConfigureRecipes")]
        public class ConfigureRecipes_Patch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                bool methodFound = false;
                bool startPatch = false;
                MethodInfo method = AccessTools.Method(typeof(Db), nameof(Db.Get));
                FieldInfo atmosuit = AccessTools.Field(typeof(TechItems), nameof(TechItems.atmoSuit));
                FieldInfo jetsuit = AccessTools.Field(typeof(TechItems), nameof(TechItems.jetSuit));
                FieldInfo leadsuit = AccessTools.Field(typeof(TechItems), nameof(TechItems.leadSuit));
                FieldInfo field = AccessTools.Field(typeof(TechItem), nameof(TechItem.parentTechId));

                foreach (CodeInstruction instruction in instructions)
                {
                    if (!startPatch && instruction.Calls(method))
                    {
                        //Debug.Log("Found method");
                        startPatch = true;
                    }
                    if (startPatch)
                    {
                        if (instruction.LoadsField(atmosuit))
                            yield return new CodeInstruction("required tech here");
                        if (instruction.LoadsField(jetsuit))
                            yield return new CodeInstruction("required tech here");
                        if (instruction.LoadsField(leadsuit))
                            yield return new CodeInstruction("required tech here");
                        if (instruction.LoadsField(field))
                            startPatch = false;
                        continue;
                    }
                    yield return instruction;
                }
            }
        }*/

        [HarmonyPatch(typeof(BuildingDef))]
        [HarmonyPatch("IsValidDLC")]
        public static class IsValidDLC_Patch
        {
            public static void Postfix(BuildingDef __instance, ref bool __result)
            {
                //Debug.Log("Found update method");
                //Debug.Log(__instance.PrefabID);
                __result = __result & CheckItemList(__instance.PrefabID);
            }
        }

        [HarmonyPatch(typeof(PlanBuildingToggle))]
        [HarmonyPatch("RefreshFG")]
        public static class RefreshFG_Patch
        {
            public static bool Prefix(PlanBuildingToggle __instance, BuildingDef ___def, PlanScreen.RequirementsState requirementsState)
            {
                //Debug.Log("Found update method");
                //Debug.Log(___def.PrefabID);
                //if (!CheckItemList(___def.PrefabID))
                    //requirementsState = PlanScreen.RequirementsState.Invalid;
                return true;
            }
        }
    }
}
