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

        [PatchPrefix]
        static bool Prefix(
            PrestigeTransferItemsState __instance,
            ItemContextAbstractClass context,
            ref string tooltip,
            ref bool __result)
        {
            if (__instance != null)
            {
                Plugin.Log.LogInfo($"PrestigeManager: IsActive __instance runtime type = {__instance.GetType().FullName}");
            }

            if (context == null)
            {
                return true;
            }

            Plugin.Log.LogInfo($"PrestigeManager: IsActive context runtime type = {context.GetType().FullName}");

            if (context.GetType().Name == "ItemContextClass")
            {
                var ctxType = context.GetType();

                LogMember(ctxType, context, "ItemAddress");
                LogMember(ctxType, context, "ModificationAvailable");
                LogMember(ctxType, context, "ViewType");
                LogMember(ctxType, context, "Searched");
                LogMember(ctxType, context, "DragAvailable");
                LogMember(ctxType, context, "MergeAvailable");
                LogMember(ctxType, context, "SplitAvailable");
                LogMember(ctxType, context, "Item");
                LogMember(ctxType, context, "Error");
            }
            else
            {
                var ctxType = context.GetType();
                LogMember(ctxType, context, "ModificationAvailable");
                LogMember(ctxType, context, "ViewType");
                LogMember(ctxType, context, "Searched");
                LogMember(ctxType, context, "DragAvailable");
                LogMember(ctxType, context, "MergeAvailable");
                LogMember(ctxType, context, "SplitAvailable");
                LogMember(ctxType, context, "Item");
                LogMember(ctxType, context, "Error");
            }

            if (context.Item?.Template == null)
            {
                return true;
            }

            int prestigeLevel = PrestigeTransferStateContext.GetLevel(__instance);
            if (prestigeLevel <= 0)
            {
                return true;
            }

            var levelConfig = ConfigManager.GetLevelConfig(prestigeLevel);
            if (levelConfig?.Filters?.IncludedItems == null)
            {
                return true;
            }

            string templateId = context.Item.Template._id;
            if (string.IsNullOrEmpty(templateId) || !levelConfig.Filters.IncludedItems.Contains(templateId))
            {
                return true;
            }

            __result = true;
            tooltip = null;

            if (context.Item is CompoundItem compoundItem && !compoundItem.IsEmpty)
            {
                Plugin.Log.LogInfo($"PrestigeManager: Prefix-allowed non-empty container {templateId}.");
            }
            else
            {
                Plugin.Log.LogInfo($"PrestigeManager: Prefix-allowed template {templateId}.");
            }

            return false;
        }

        private static void LogMember(System.Type type, object instance, string name)
        {
            var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null)
            {
                object value = null;
                try { value = field.GetValue(instance); } catch { value = "<unreadable>"; }
                Plugin.Log.LogInfo($"PrestigeManager: Context field [{field.FieldType.Name}] {field.Name} = {value}");
                return;
            }

            var prop = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (prop != null && prop.GetIndexParameters().Length == 0)
            {
                object value = null;
                try { value = prop.GetValue(instance, null); } catch { value = "<unreadable>"; }
                Plugin.Log.LogInfo($"PrestigeManager: Context prop [{prop.PropertyType.Name}] {prop.Name} = {value}");
            }
        }
    }
}