using HarmonyLib;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ArchipelagoNotIncluded.Patches
{
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
}
