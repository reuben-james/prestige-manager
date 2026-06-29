using System.Reflection;
using EFT.InventoryLogic;
using EFT.UI;
using HarmonyLib;
using PrestigeManager.Config;
using SPT.Reflection.Patching;

namespace PrestigeManager.Patches
{
    internal class PrestigeTransferItemsStateFilterPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(PrestigeTransferItemsState), nameof(PrestigeTransferItemsState.IsActive));
        }

        [PatchPostfix]
        static void Postfix(
            PrestigeTransferItemsState __instance,
            ItemContextAbstractClass context,
            ref string tooltip,
            ref bool __result)
        {
            if (__result)
            {
                return;
            }

            if (context?.Item?.Template == null)
            {
                return;
            }

            int prestigeLevel = PrestigeTransferStateContext.GetLevel(__instance);
            if (prestigeLevel <= 0)
            {
                return;
            }

            var levelConfig = ConfigManager.GetLevelConfig(prestigeLevel);
            if (levelConfig?.Filters?.IncludedItems == null || levelConfig.Filters.IncludedItems.Count == 0)
            {
                return;
            }

            string templateId = context.Item.Template._id;
            if (string.IsNullOrEmpty(templateId))
            {
                return;
            }

            if (levelConfig.Filters.IncludedItems.Contains(templateId))
            {
                __result = true;
                tooltip = null;
                Plugin.Log.LogInfo($"PrestigeManager: Allowed template {templateId} via filter patch.");
            }
        }
    }
}