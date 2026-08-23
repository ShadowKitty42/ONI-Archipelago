using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace ArchipelagoNotIncluded.Patches
{
    public class TelepadApCarePackageManager : KMonoBehaviour
    {
        private Telepad telepad;
        private readonly List<ApCarePackageSlotControl> slots = new List<ApCarePackageSlotControl>();

        public override void OnSpawn()
        {
            base.OnSpawn();
            telepad = GetComponent<Telepad>();
        }

        public void SyncSlots()
        {
            slots.Clear();
            if (!ArchipelagoNotIncluded.ReplacePrintCycle)
                return;

            int count = CarePackageQueue.Count;
            if (count == 0)
            {
                slots.Add(new ApCarePackageSlotControl(telepad, -1, null, showTitle: true));
                return;
            }

            for (int i = 0; i < count; i++)
                slots.Add(new ApCarePackageSlotControl(telepad, i, CarePackageQueue.GetAt(i), showTitle: i == 0));
        }

        public IReadOnlyList<ISidescreenButtonControl> GetButtonControls() => slots;

        public void NotifyQueueChanged()
        {
            SyncSlots();
            if (DetailsScreen.Instance != null && telepad != null)
                DetailsScreen.Instance.Refresh(telepad.gameObject);
        }
    }

    internal sealed class ApCarePackageSlotControl : ISidescreenButtonControl
    {
        private readonly Telepad telepad;
        private readonly int queueIndex;
        private readonly string packageName;
        private readonly bool showTitle;

        public ApCarePackageSlotControl(Telepad telepad, int queueIndex, string packageName, bool showTitle)
        {
            this.telepad = telepad;
            this.queueIndex = queueIndex;
            this.packageName = packageName;
            this.showTitle = showTitle;
        }

        public string SidescreenButtonText
        {
            get
            {
                if (string.IsNullOrEmpty(packageName))
                    return "No AP care packages waiting";

                int count = CarePackageQueue.Count;
                if (count > 1)
                    return $"{queueIndex + 1}. Print AP: {packageName}";

                return $"Print AP: {packageName}";
            }
        }

        public string SidescreenButtonTooltip
        {
            get
            {
                if (string.IsNullOrEmpty(packageName))
                    return "Archipelago care packages will appear here when received.";

                int count = CarePackageQueue.Count;
                return count > 1
                    ? $"Deliver {packageName} ({queueIndex + 1} of {count} waiting)."
                    : $"Deliver {packageName}.";
            }
        }

        public string SidescreenTitle => showTitle ? "Archipelago care packages" : null;

        public void SetButtonTextOverride(ButtonMenuTextOverride textOverride)
        {
        }

        public bool SidescreenEnabled() => ArchipelagoNotIncluded.ReplacePrintCycle;

        public bool SidescreenButtonInteractable() =>
            ArchipelagoNotIncluded.ReplacePrintCycle
            && telepad != null
            && queueIndex >= 0
            && !string.IsNullOrEmpty(packageName);

        public void OnSidescreenButtonPressed()
        {
            if (telepad == null || queueIndex < 0)
                return;

            TelepadApCarePackagePatches.DeliverPackageAtIndex(telepad, queueIndex);
        }

        public int HorizontalGroupID() => -1;

        public int ButtonSideScreenSortOrder() => 100;
    }

    internal class ApButtonMenuScrollAddon : KMonoBehaviour
    {
        private const float MaxViewportHeight = 210f;
        private const float DefaultButtonHeight = 36f;
        private const float ScrollbarWidth = 14f;

        private Transform originalParent;
        private int originalSiblingIndex;
        private RectTransform buttonContainer;
        private KScrollRect scrollRect;
        private RectTransform viewport;
        private LayoutElement scrollRootLayout;
        private Scrollbar verticalScrollbar;
        private bool built;

        public void Configure(ButtonMenuSideScreen screen, int buttonCount)
        {
            if (screen?.buttonContainer == null)
                return;

            if (!built)
                Build(screen.buttonContainer);

            float buttonHeight = screen.buttonPrefab != null && screen.buttonPrefab.minHeight > 0f
                ? screen.buttonPrefab.minHeight
                : DefaultButtonHeight;

            LayoutRebuilder.ForceRebuildLayoutImmediate(buttonContainer);

            float contentHeight = Mathf.Max(buttonHeight, buttonCount * buttonHeight);
            bool needsScroll = contentHeight > MaxViewportHeight;
            float viewportHeight = needsScroll ? MaxViewportHeight : contentHeight;

            scrollRootLayout.preferredHeight = viewportHeight;
            scrollRootLayout.minHeight = viewportHeight;

            scrollRect.vertical = needsScroll;
            scrollRect.enabled = true;
            if (verticalScrollbar != null)
                verticalScrollbar.gameObject.SetActive(needsScroll);

            if (needsScroll)
                scrollRect.normalizedPosition = new Vector2(0f, 1f);
        }

        public void Restore()
        {
            if (!built || buttonContainer == null)
                return;

            buttonContainer.SetParent(originalParent, false);
            buttonContainer.SetSiblingIndex(originalSiblingIndex);

            if (scrollRect != null)
                Destroy(scrollRect.gameObject);

            built = false;
            buttonContainer = null;
            scrollRect = null;
            viewport = null;
            scrollRootLayout = null;
            verticalScrollbar = null;
        }

        private void Build(RectTransform container)
        {
            originalParent = container.parent;
            originalSiblingIndex = container.GetSiblingIndex();

            var scrollRootGo = new GameObject("ApCarePackageScrollRoot");
            scrollRootGo.transform.SetParent(originalParent, false);
            scrollRootGo.transform.SetSiblingIndex(originalSiblingIndex);

            scrollRootLayout = scrollRootGo.AddComponent<LayoutElement>();
            scrollRootLayout.flexibleWidth = 1f;

            scrollRootGo.AddComponent<RectTransform>();

            var viewportGo = new GameObject("Viewport");
            viewportGo.transform.SetParent(scrollRootGo.transform, false);
            viewport = viewportGo.AddComponent<RectTransform>();
            viewport.anchorMin = Vector2.zero;
            viewport.anchorMax = Vector2.one;
            viewport.offsetMin = Vector2.zero;
            viewport.offsetMax = new Vector2(-ScrollbarWidth, 0f);
            viewportGo.AddComponent<RectMask2D>();

            container.SetParent(viewport, false);
            container.anchorMin = new Vector2(0f, 1f);
            container.anchorMax = new Vector2(1f, 1f);
            container.pivot = new Vector2(0.5f, 1f);
            container.anchoredPosition = Vector2.zero;
            container.sizeDelta = Vector2.zero;

            var layout = container.GetComponent<VerticalLayoutGroup>();
            if (layout == null)
                layout = container.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var fitter = container.GetComponent<ContentSizeFitter>();
            if (fitter == null)
                fitter = container.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRect = scrollRootGo.AddComponent<KScrollRect>();
            scrollRect.content = container;
            scrollRect.viewport = viewport;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 24f;
            scrollRect.allowVerticalScrollWheel = true;

            verticalScrollbar = TryCloneScrollbar(scrollRootGo);
            if (verticalScrollbar != null)
            {
                scrollRect.verticalScrollbar = verticalScrollbar;
                scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            }
            else
            {
                viewport.offsetMax = Vector2.zero;
            }

            buttonContainer = container;
            built = true;
        }

        private static Scrollbar TryCloneScrollbar(GameObject scrollRoot)
        {
            if (DetailsScreen.Instance == null)
                return null;

            foreach (ScrollRect template in DetailsScreen.Instance.GetComponentsInChildren<ScrollRect>(true))
            {
                if (template.verticalScrollbar == null)
                    continue;

                GameObject clone = Util.KInstantiateUI(template.verticalScrollbar.gameObject, scrollRoot, force_active: true);
                clone.name = "ApCarePackageScrollbar";
                var rect = clone.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(1f, 0f);
                rect.anchorMax = new Vector2(1f, 1f);
                rect.pivot = new Vector2(1f, 1f);
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = new Vector2(ScrollbarWidth, 0f);
                return clone.GetComponent<Scrollbar>();
            }

            return null;
        }
    }

    public static class TelepadApCarePackagePatches
    {
        public static void RefreshUi()
        {
            if (Components.Telepads == null)
                return;

            foreach (Telepad telepad in Components.Telepads)
            {
                if (telepad == null)
                    continue;

                EnsureTelepadManager(telepad);
                telepad.GetComponent<TelepadApCarePackageManager>()?.NotifyQueueChanged();
            }
        }

        public static bool DeliverPackageAtIndex(Telepad telepad, int queueIndex)
        {
            if (!ArchipelagoNotIncluded.ReplacePrintCycle || telepad == null || CarePackageQueue.IsEmpty)
                return false;

            string packageName = CarePackageQueue.GetAt(queueIndex);
            if (string.IsNullOrEmpty(packageName))
                return false;

            if (ArchipelagoNotIncluded.netmon?.CarePackages == null
                || !ArchipelagoNotIncluded.netmon.CarePackages.TryGetValue(packageName, out CarePackageInfo packageInfo))
            {
                Debug.Log($"Unknown queued care package: {packageName}");
                return false;
            }

            if (CarePackageQueue.DequeueAt(queueIndex) == null)
                return false;

            telepad.smi.sm.openPortal.Trigger(telepad.smi);
            packageInfo.Deliver(telepad.transform.GetPosition());
            if (APSaveData.Instance != null)
                APSaveData.Instance.DeliveredCarePackages.Add(packageName);
            telepad.smi.sm.closePortal.Trigger(telepad.smi);

            Debug.Log($"Delivered AP care package: {packageName} ({CarePackageQueue.Count} remaining)");
            RefreshUi();
            return true;
        }

        public static void EnsureTelepadManager(Telepad telepad)
        {
            if (telepad == null)
                return;

            if (telepad.GetComponent<TelepadApCarePackageManager>() != null)
                return;

            telepad.gameObject.AddComponent<TelepadApCarePackageManager>();
            Debug.Log(
                $"TelepadApCarePackageManager installed on {telepad.name}; "
                + $"ReplacePrintCycle={ArchipelagoNotIncluded.ReplacePrintCycle}, queued={CarePackageQueue.Count}");
        }

        private static readonly MethodInfo ButtonMenuRefreshMethod =
            AccessTools.Method(typeof(ButtonMenuSideScreen), "Refresh");

        public static void RefreshButtonMenuSideScreen(ButtonMenuSideScreen screen, GameObject target)
        {
            if (screen == null || target == null)
                return;

            var manager = target.GetComponent<TelepadApCarePackageManager>();
            manager?.SyncSlots();

            var traverse = Traverse.Create(screen);
            var targets = traverse.Field<List<ISidescreenButtonControl>>("targets").Value;
            if (targets == null)
                targets = new List<ISidescreenButtonControl>();

            targets.Clear();
            targets.AddRange(target.GetAllSMI<ISidescreenButtonControl>());
            targets.AddRange(target.GetComponents<ISidescreenButtonControl>());
            if (manager != null)
                targets.AddRange(manager.GetButtonControls());

            traverse.Field<List<ISidescreenButtonControl>>("targets").Value = targets;
            ButtonMenuRefreshMethod?.Invoke(screen, null);

            if (manager != null && target.GetComponent<Telepad>() != null)
            {
                var scroll = screen.GetComponent<ApButtonMenuScrollAddon>();
                if (scroll == null)
                    scroll = screen.gameObject.AddComponent<ApButtonMenuScrollAddon>();
                scroll.Configure(screen, targets.Count);
            }

            if (target.GetComponent<Telepad>() != null && ArchipelagoNotIncluded.ReplacePrintCycle)
            {
                Debug.Log(
                    $"AP care package ButtonMenuSideScreen ready ({targets.Count} control(s), "
                    + $"queued={CarePackageQueue.Count}).");
            }
        }
    }

    [HarmonyPatch(typeof(Telepad), nameof(Telepad.OnSpawn))]
    public static class Telepad_OnSpawn_ApCarePackage_Patch
    {
        public static void Postfix(Telepad __instance)
        {
            TelepadApCarePackagePatches.EnsureTelepadManager(__instance);
        }
    }

    [HarmonyPatch(typeof(DetailsScreen), nameof(DetailsScreen.Refresh))]
    public static class DetailsScreen_Refresh_ApCarePackage_Patch
    {
        public static void Prefix(GameObject go)
        {
            TelepadApCarePackagePatches.EnsureTelepadManager(go?.GetComponent<Telepad>());
        }
    }

    [HarmonyPatch(typeof(ButtonMenuSideScreen), nameof(ButtonMenuSideScreen.IsValidForTarget))]
    public static class ButtonMenuSideScreen_IsValidForTarget_ApCarePackage_Patch
    {
        public static void Postfix(GameObject target, ref bool __result)
        {
            if (__result || target?.GetComponent<Telepad>() == null)
                return;

            if (target.GetComponent<TelepadApCarePackageManager>() != null
                && ArchipelagoNotIncluded.ReplacePrintCycle)
            {
                __result = true;
            }
        }
    }

    [HarmonyPatch(typeof(ButtonMenuSideScreen), nameof(ButtonMenuSideScreen.SetTarget))]
    public static class ButtonMenuSideScreen_SetTarget_ApCarePackage_Patch
    {
        public static void Prefix(GameObject new_target)
        {
            TelepadApCarePackagePatches.EnsureTelepadManager(new_target?.GetComponent<Telepad>());
        }

        public static void Postfix(ButtonMenuSideScreen __instance, GameObject new_target)
        {
            var scrollAddon = __instance.GetComponent<ApButtonMenuScrollAddon>();
            if (new_target?.GetComponent<Telepad>() == null || !ArchipelagoNotIncluded.ReplacePrintCycle)
            {
                scrollAddon?.Restore();
                return;
            }

            TelepadApCarePackagePatches.RefreshButtonMenuSideScreen(__instance, new_target);
        }
    }
}
