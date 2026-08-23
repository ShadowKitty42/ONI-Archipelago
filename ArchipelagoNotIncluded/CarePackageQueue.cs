using System;
using System.Collections.Generic;
using System.Linq;

namespace ArchipelagoNotIncluded
{
    public static class CarePackageQueue
    {
        private static readonly Queue<string> queue = new Queue<string>();

        public static void Enqueue(string packageName)
        {
            if (APSaveData.Instance == null)
                return;

            queue.Enqueue(packageName);
            APSaveData.Instance.PendingCarePackages.Add(packageName);
        }

        public static string Peek() =>
            queue.Count > 0 ? queue.Peek() : null;

        public static string GetAt(int index)
        {
            if (index < 0 || index >= queue.Count)
                return null;

            int i = 0;
            foreach (string item in queue)
            {
                if (i == index)
                    return item;
                i++;
            }

            return null;
        }

        public static string DequeueAt(int index)
        {
            if (queue.Count == 0 || APSaveData.Instance == null || index < 0 || index >= queue.Count)
                return null;

            var items = new List<string>(queue);
            string item = items[index];
            items.RemoveAt(index);

            queue.Clear();
            foreach (string remaining in items)
                queue.Enqueue(remaining);

            APSaveData.Instance.PendingCarePackages.RemoveAt(index);
            return item;
        }

        public static string DequeueFirst() => DequeueAt(0);

        public static bool IsEmpty => queue.Count == 0;

        public static int Count => queue.Count;

        public static void RebuildFromSave()
        {
            queue.Clear();
            if (APSaveData.Instance?.PendingCarePackages == null)
                return;

            foreach (string package in APSaveData.Instance.PendingCarePackages)
                queue.Enqueue(package);
        }

        public static void ReconcileCarePackages()
        {
            APSaveData save = APSaveData.Instance;
            if (save == null)
                return;

            // Crash/disconnect can persist LocalItemList before Pending/Received are written.
            foreach (var group in save.LocalItemList
                .Where(x => x.StartsWith("Care Package"))
                .Select(ParseShortName)
                .Where(x => x != null)
                .GroupBy(x => x))
            {
                string shortName = group.Key;
                int listed = group.Count();
                int received = save.ReceivedCarePackages.Count(x => x == shortName);
                for (int i = received; i < listed; i++)
                    save.ReceivedCarePackages.Add(shortName);
            }

            if (save.ReceivedCarePackages == null || save.ReceivedCarePackages.Count == 0)
                return;

            foreach (string shortName in save.ReceivedCarePackages.Distinct())
            {
                int received = save.ReceivedCarePackages.Count(x => x == shortName);
                int accounted = save.DeliveredCarePackages.Count(x => x == shortName)
                    + save.ForfeitedCarePackages.Count(x => x == shortName)
                    + save.PendingCarePackages.Count(x => x == shortName);
                int missing = received - accounted;
                for (int i = 0; i < missing; i++)
                    save.PendingCarePackages.Add(shortName);
            }
        }

        // AP item names look like "Care Package: Sandstone".
        public static string ParseShortName(string apItemName)
        {
            if (string.IsNullOrEmpty(apItemName) || !apItemName.StartsWith("Care Package"))
                return null;

            string[] parts = apItemName.Split(new[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 1 ? parts[1] : null;
        }
    }
}
