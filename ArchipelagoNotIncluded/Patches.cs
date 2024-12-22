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

namespace ArchipelagoNotIncluded
{
    public class Patches
    {
        [HarmonyPatch(typeof(Techs))]
        [HarmonyPatch("Init")]
        public class Techs_Init_Patch
        {
            public static bool Prefix(Techs __instance)
            {
                DirectoryInfo modDirectory = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                APSeedInfo info = null;
                foreach (FileInfo jsonFile in modDirectory.EnumerateFiles("*.json").OrderByDescending(f => f.LastWriteTime))
                {
                    try
                    {
                        if (jsonFile.Name == "DefaultItemList.json")
                            continue;
                        string json = File.ReadAllText(jsonFile.FullName);
                        info = JsonConvert.DeserializeObject<APSeedInfo>(json);
                        break;
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        Debug.LogWarning($"Failed to parse JSON file {jsonFile.FullName}");
                        continue;
                    }
                }

                //If there is no info, run the normal tech init function
                if (info == null)
                {
                    Debug.Log("No mod json could be loaded. Skipping mod override");
                    return true;
                }

                foreach (KeyValuePair<string, List<string>> pair in info.technologies)
                {
                    Debug.Log($"Generating research for {pair.Key}, ({pair.Value.Join(s => s, ",")})");
                    new Tech(pair.Key, pair.Value.ToList(), __instance);
                }

                foreach (KeyValuePair<string, List<string>> pair in ArchipelagoNotIncluded.Sciences)
                {
                    if (!info.technologies.ContainsKey(pair.Key))
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

        [HarmonyDebug]
        [HarmonyPatch(typeof(Research))]
        [HarmonyPatch("CheckBuyResearch")]
        public static class Research_CheckBuyResearch_Patch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                bool startPatch = false;

                foreach (CodeInstruction instruction in instructions)
                {
                    if (!startPatch && instruction.Calls(AccessTools.Method(typeof(TechInstance), nameof(TechInstance.Purchased))))
                    {
                        //Debug.Log("Found method");
                        startPatch = true;
                        yield return instruction;
                        continue;
                    }
                    if (startPatch)
                    {
                        if (instruction.Calls(AccessTools.Method(typeof(KMonoBehaviour), nameof(KMonoBehaviour.Trigger))))
                            startPatch = false;
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
    }
}
