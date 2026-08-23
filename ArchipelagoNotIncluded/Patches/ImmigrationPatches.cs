using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace ArchipelagoNotIncluded.Patches
{
    public static class ImmigrationPatches
    {
        public static bool ShouldPatch =>
            ArchipelagoNotIncluded.ReplacePrintCycle
            && !ArchipelagoNotIncluded.Options.CreateModList;

        public static bool IsApColonyAwaitingSlotData() =>
            APSaveData.Instance?.AllowResourceChecks == true
            && ArchipelagoNotIncluded.info == null
            && APSaveData.Instance?.APSeedInfo == null;

        public static void StripVanillaCarePackages(CharacterSelectionController controller)
        {
            var traverse = Traverse.Create(controller);
            var containers = traverse.Field("containers").GetValue<List<ITelepadDeliverableContainer>>();
            if (containers == null)
                return;

            for (int i = containers.Count - 1; i >= 0; i--)
            {
                if (containers[i] is not CarePackageContainer)
                    continue;

                if (containers[i] is Component component && component.gameObject != null)
                    Object.Destroy(component.gameObject);
                containers.RemoveAt(i);
            }

            traverse.Field("containers").SetValue(containers);
        }
    }

    [HarmonyPatch(typeof(Immigration), "Sim200ms")]
    public static class Immigration_Sim200ms_Patch
    {
        public static void Postfix()
        {
            ArchipelagoNotIncluded.netmon?.TryDrainPendingItemQueue();
        }
    }

    [HarmonyPatch(typeof(Immigration), "ImmigrantsAvailable", MethodType.Getter)]
    public static class Immigration_ImmigrantsAvailable_Patch
    {
        public static void Postfix(ref bool __result)
        {
            if (ArchipelagoNotIncluded.Options.CreateModList)
                return;

            if (ImmigrationPatches.IsApColonyAwaitingSlotData())
                __result = false;
        }
    }

    [HarmonyPatch(typeof(CharacterSelectionController), "InitializeContainers")]
    public static class CharacterSelectionController_InitializeContainers_Patch
    {
        public static void Postfix(CharacterSelectionController __instance)
        {
            if (!ImmigrationPatches.ShouldPatch)
                return;

            ImmigrationPatches.StripVanillaCarePackages(__instance);
        }
    }

    [HarmonyPatch(typeof(CarePackageContainer), "GenerateCharacter", new[] { typeof(bool) })]
    public static class CarePackageContainer_GenerateCharacter_Patch
    {
        public static bool Prefix()
        {
            if (!ImmigrationPatches.ShouldPatch)
                return true;

            // AP packages are delivered via the telepad side screen, not immigrant UI.
            return false;
        }
    }

    // Vanilla dupe cycles stay on the normal timer; AP packages are delivered separately.
    [HarmonyPatch(typeof(Immigration), "RandomCarePackage")]
    public static class Immigration_RandomCarePackage_Patch
    {
        public static bool Prefix(ref CarePackageInfo __result)
        {
            if (!ImmigrationPatches.ShouldPatch)
                return true;

            __result = null;
            return false;
        }
    }
}
