using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchipelagoNotIncluded.Patches
{
    [HarmonyPatch(typeof(Db))]
    [HarmonyPatch(nameof(Db.Initialize))]
    public static class Initialize_Patch
    {
        public static void Postfix(Db __instance)
        {
            ArchipelagoNotIncluded.ArchipelagoConnection = new("ArchipelagoConnection", __instance.StatusItemCategories, "ArchipelagoConnection");
        }
    }

    [HarmonyPatch(typeof(BuildingStatusItems))]
    [HarmonyPatch(nameof(BuildingStatusItems.CreateStatusItems))]
    public static class CreateStatusItems_Patch
    {
        public static void Postfix(BuildingStatusItems __instance)
        {
            ArchipelagoNotIncluded.ArchipelagoConnected = __instance.CreateStatusItem("ArchipelagoConnected", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
            ArchipelagoNotIncluded.ArchipelagoDisconnected = __instance.CreateStatusItem("ArchipelagoDisconnected", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
            ArchipelagoNotIncluded.ArchipelagoDisconnected.AddNotification();
            ArchipelagoNotIncluded.ArchipelagoDisconnected.notificationClickCallback = async delegate (object data) { await Task.Run(() => ArchipelagoNotIncluded.netmon.TryConnectArchipelago()); };
        }
    }

    [HarmonyPatch(typeof(Telepad))]
    [HarmonyPatch(nameof(Telepad.Update))]
    public static class Telepad_Update_Patch
    {
        public static void Postfix(Telepad __instance)
        {
            if (ArchipelagoNotIncluded.info == null)
                return;
            if (ArchipelagoNotIncluded.netmon.session == null)
            {
                __instance.selectable.SetStatusItem(ArchipelagoNotIncluded.ArchipelagoConnection, ArchipelagoNotIncluded.ArchipelagoDisconnected);
            }
            else
            {
                __instance.selectable.SetStatusItem(ArchipelagoNotIncluded.ArchipelagoConnection, ArchipelagoNotIncluded.ArchipelagoConnected);
            }
        }
    }
}
