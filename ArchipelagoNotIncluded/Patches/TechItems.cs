using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchipelagoNotIncluded.Patches
{
    [HarmonyPatch(typeof(TechItem))]
    [HarmonyPatch(nameof(TechItem.IsPOIUnlocked))]
    public static class IsPOIUnlocked_Patch
    {
        public static void Postfix(TechItem __instance, ref bool __result)
        {
            if (!APSaveData.Instance.AllowResourceChecks)
                return;

            if (APPatches.isModItem(__instance.Id))
                return;
            __result = APPatches.CheckItemList(__instance);
            //__result = true;
        }
    }

    [HarmonyPatch(typeof(TechItem))]
    [HarmonyPatch(nameof(TechItem.IsComplete))]
    public static class IsComplete_Patch
    {
        public static bool Prefix(TechItem __instance, ref bool __result)
        {
            if (!APSaveData.Instance.AllowResourceChecks)
                return true;

            if (APPatches.isModItem(__instance.Id))
                return true;
            __result = APPatches.CheckItemList(__instance);
            return false;
        }
    }

    [HarmonyPatch(typeof(PlanBuildingToggle))]
    [HarmonyPatch(nameof(PlanBuildingToggle.StandardDisplayFilter))]
    public static class DisplayFilter_Patch
    {
        public static bool Prefix(PlanBuildingToggle __instance, ref bool __result)
        {
            if (!APSaveData.Instance.AllowResourceChecks)
                return true;

            if (APPatches.isModItem(__instance.def.PrefabID))
                return true;
            __result = APPatches.CheckItemList(__instance.def.PrefabID) && (__instance.planScreen.ActiveCategoryToggleInfo == null || __instance.buildingCategory == (HashedString)__instance.planScreen.ActiveCategoryToggleInfo.userData);
            return false;
        }
    }

    [HarmonyPatch(typeof(Tech))]
    [HarmonyPatch(nameof(Tech.ArePrerequisitesComplete))]
    public static class ArePrerequisitesComplete_Patch
    {
        public static bool Prefix(TechItem __instance, ref bool __result)
        {
            if (!APSaveData.Instance.AllowResourceChecks)
                return true;

            if (APPatches.isModItem(__instance.Id))
                return true;
            //__result = true;
            __result = APPatches.CheckItemList(__instance);
            return false;
        }
    }

    [HarmonyPatch(typeof(BuildingDef))]
    [HarmonyPatch(nameof(BuildingDef.IsAvailable))]
    public static class IsAvailable_Patch
    {
        public static void Postfix(BuildingDef __instance, ref bool __result)
        {
            if (!APSaveData.Instance.AllowResourceChecks)
                return;
            //Debug.Log("Found update method");
            //Debug.Log(__instance.PrefabID);
            if (APPatches.isModItem(__instance.PrefabID))
                return;
            __result = APPatches.CheckItemList(__instance.PrefabID);
            //if (__result)
            //    Debug.Log($"{__instance.PrefabID} isAvailable");
        }
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
            if (APPatches.CheckItemList(def.PrefabID))
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
            if (!String.IsNullOrEmpty(search) && APPatches.CheckItemList(search))
                __result = true;
            else
                __result = false;
            return false;
        }
    }
}
