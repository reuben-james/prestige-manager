using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;
using PrestigeManager.Config;

namespace PrestigeManager.Patches
{
    internal class PrestigeTransferItemsStatePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(PrestigeTransferItemsState), nameof(PrestigeTransferItemsState.Init));
        }

        [PatchPostfix]
        static void Postfix(PrestigeTransferItemsState __instance, int prestigeLevelToOpen)
        {
            Plugin.Log.LogInfo($"PrestigeManager: Init called for prestige level {prestigeLevelToOpen}.");

            var levelConfig = ConfigManager.GetLevelConfig(prestigeLevelToOpen);
            if (levelConfig == null)
            {
                Plugin.Log.LogInfo($"PrestigeManager: No config entry for level {prestigeLevelToOpen}, leaving game defaults.");
                return;
            }

            var stashConfig = __instance.StashConfig;
            if (stashConfig == null)
            {
                Plugin.Log.LogWarning("PrestigeManager: StashConfig on instance is null, skipping patch.");
                return;
            }

            // Dump all fields so we can see real field names
            Plugin.Log.LogInfo($"PrestigeManager: StashConfig type is {stashConfig.GetType().FullName}");
            foreach (var field in stashConfig.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                Plugin.Log.LogInfo($"PrestigeManager: StashConfig field: [{field.FieldType.Name}] {field.Name} = {field.GetValue(stashConfig)}");
            }

            if (levelConfig.XCellCount.HasValue)
            {
                Traverse.Create(stashConfig).Field("xCellCount").SetValue(levelConfig.XCellCount.Value);
                Plugin.Log.LogInfo($"PrestigeManager: Set xCellCount to {levelConfig.XCellCount.Value}");
            }

            if (levelConfig.YCellCount.HasValue)
            {
                Traverse.Create(stashConfig).Field("yCellCount").SetValue(levelConfig.YCellCount.Value);
                Plugin.Log.LogInfo($"PrestigeManager: Set yCellCount to {levelConfig.YCellCount.Value}");
            }

            if (levelConfig.Filters?.IncludedItems != null && levelConfig.Filters.IncludedItems.Count > 0)
            {
                var filters = Traverse.Create(stashConfig).Field("Filters").GetValue();
                if (filters == null)
                {
                    Plugin.Log.LogWarning("PrestigeManager: filters field is null — check field dump above.");
                    return;
                }

                var includedItemsTraverse = Traverse.Create(filters).Field("includedItems");
                var existing = includedItemsTraverse.GetValue<List<string>>();

                var merged = existing != null
                    ? new List<string>(existing)
                    : new List<string>();

                int beforeCount = merged.Count;
                merged.AddRange(levelConfig.Filters.IncludedItems);
                merged = merged.Distinct().ToList();
                int added = merged.Count - beforeCount;

                includedItemsTraverse.SetValue(merged);

                Plugin.Log.LogInfo($"PrestigeManager: Merged includedItems for level {prestigeLevelToOpen} — {beforeCount} default + {levelConfig.Filters.IncludedItems.Count} config = {merged.Count} unique items ({added} new added).");
            }
        }
    }
}
