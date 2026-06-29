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
        static void Postfix(PrestigeTransferItemsState __instance)
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

            if (__instance == null || __instance.StashConfig == null || __instance.FakeController == null)
            {
                Plugin.Log.LogWarning("PrestigeManager: Missing state objects for size patch.");
                return;
            }

            try
            {
                int width = levelConfig.XCellCount ?? __instance.StashConfig.Size.x;
                int height = levelConfig.YCellCount ?? __instance.StashConfig.Size.y;

                var fakeControllerType = __instance.FakeController.GetType();
                var stashField = AccessTools.Field(fakeControllerType, "_stash")
                              ?? AccessTools.Field(fakeControllerType, "Stash")
                              ?? AccessTools.Field(fakeControllerType, "stash");

                if (stashField == null)
                {
                    Plugin.Log.LogWarning("PrestigeManager: Could not find stash field on FakeController.");
                    return;
                }

                var fakeStash = stashField.GetValue(__instance.FakeController) as StashItemClass;
                if (fakeStash == null)
                {
                    Plugin.Log.LogWarning("PrestigeManager: FakeController stash is null or not StashItemClass.");
                    return;
                }

                var newGrid = new GClass3116(
                    "transferGrid",
                    width,
                    height,
                    false,
                    false,
                    Array.Empty<ItemFilter>(),
                    fakeStash,
                    true);

                fakeStash.Grids = new StashGridClass[]
                {
                    newGrid
                };

                __instance.TransferGrid = newGrid;

                Plugin.Log.LogInfo($"PrestigeManager: Rebuilt transfer grid to {width}x{height}.");
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"PrestigeManager: Exception while rebuilding transfer grid size: {ex}");
            }
        }
    }
}