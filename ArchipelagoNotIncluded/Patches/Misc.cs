using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace ArchipelagoNotIncluded.Patches
{
    public static class Misc
    {
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

    [HarmonyPatch(typeof(PauseScreen))]
    [HarmonyPatch(nameof(PauseScreen.RefreshDLCButton))]
    public static class RefreshDLCButton_Patch
    {
        public static void Postfix(MultiToggle button)
        {
            if (!APSaveData.Instance.AllowResourceChecks)
                return;
            button.onClick = null;
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

    [HarmonyPatch(typeof(BionicMinionConfig))]
    [HarmonyPatch(nameof(BionicMinionConfig.BionicFreeDiscoveries))]
    public static class BionicFreeDiscoveries_Patch
    {
        public static bool Prefix()
        {
            if (!APSaveData.Instance.AllowResourceChecks)
                return true;
            APSaveData.Instance.HadBionicDupe = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(Localization))]
    [HarmonyPatch(nameof(Localization.Initialize))]
    public static class Localization_Initialize_Patch
    {
        public static void Postfix()
        {
            LocalisationUtil.Translate(typeof(STRINGS), true);
        }
    }
}
