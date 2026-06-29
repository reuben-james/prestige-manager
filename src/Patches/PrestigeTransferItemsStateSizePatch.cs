using System;
using System.Reflection;
using EFT.InventoryLogic;
using EFT.UI;
using HarmonyLib;
using PrestigeManager.Config;
using SPT.Reflection.Patching;

namespace PrestigeManager.Patches
{
    internal class PrestigeTransferItemsStateSizePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(PrestigeTransferItemsState), "method_3");
        }

        [PatchPostfix]
        static void Postfix(PrestigeTransferItemsState __instance, ref StashItemClass __result)
        {
            int prestigeLevel = PrestigeTransferStateContext.GetLevel(__instance);
            var levelConfig = ConfigManager.GetLevelConfig(prestigeLevel);

            if (levelConfig == null)
            {
                return;
            }

            if (!levelConfig.XCellCount.HasValue && !levelConfig.YCellCount.HasValue)
            {
                return;
            }

            if (__instance == null || __instance.StashConfig == null || __result == null)
            {
                Plugin.Log.LogWarning("PrestigeManager: Missing state objects for size patch.");
                return;
            }

            try
            {
                int width = levelConfig.XCellCount ?? __instance.StashConfig.Size.x;
                int height = levelConfig.YCellCount ?? __instance.StashConfig.Size.y;

                var newGrid = new GClass3116(
                    "transferGrid",
                    width,
                    height,
                    false,
                    false,
                    Array.Empty<ItemFilter>(),
                    __result,
                    true);

                __result.Grids = new StashGridClass[]
                {
                    newGrid
                };

                __instance.TransferGrid = newGrid;

                Plugin.Log.LogInfo($"PrestigeManager: Rebuilt transfer grid to {width}x{height} using method_3 result stash.");
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"PrestigeManager: Exception while rebuilding transfer grid size: {ex}");
            }
        }
    }
}