using System;
using System.Linq;
using System.Reflection;
using EFT.InventoryLogic;
using HarmonyLib;
using PrestigeManager.Config;
using SPT.Reflection.Patching;

namespace PrestigeManager.Patches
{
    internal class PrestigeTransferArenaGridCanAcceptPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            var type = AccessTools.TypeByName("EFT.UI.ArenaEftItemTransferGridView");
            if (type == null)
            {
                Plugin.Log.LogError("PrestigeManager: Could not resolve EFT.UI.ArenaEftItemTransferGridView.");
                return null;
            }

            var target = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(m =>
                {
                    if (m.Name != "CanAccept")
                        return false;

                    var p = m.GetParameters();
                    return p.Length == 3
                        && p[0].ParameterType.Name == "ItemContextClass"
                        && p[1].ParameterType.Name == "ItemContextAbstractClass"
                        && p[2].IsOut;
                });

            Plugin.Log.LogInfo($"PrestigeManager: Arena grid CanAccept target => {target}");
            return target;
        }

        [PatchPrefix]
        static bool Prefix(
            object __instance,
            ItemContextClass itemContext,
            ItemContextAbstractClass targetItemContext,
            ref GStruct153 operation,
            ref bool __result)
        {
            if (itemContext?.Item?.Template?._id == null)
            {
                return true;
            }

            int prestigeLevel = PrestigeTransferStateContext.GetLevel(__instance);
            if (prestigeLevel <= 0)
            {
                return true;
            }

            var levelConfig = ConfigManager.GetLevelConfig(prestigeLevel);
            var included = levelConfig?.Filters?.IncludedItems;
            if (included == null || included.Count == 0)
            {
                return true;
            }

            string templateId = itemContext.Item.Template._id;
            if (!included.Contains(templateId))
            {
                return true;
            }

            var compoundItem = itemContext.Item as CompoundItem;
            if (compoundItem != null && !compoundItem.IsEmpty)
            {
                Plugin.Log.LogInfo($"PrestigeManager: Forcing CanAccept true for allowed non-empty container {templateId}");
                operation = default;
                __result = true;
                return false;
            }

            return true;
        }
    }
}