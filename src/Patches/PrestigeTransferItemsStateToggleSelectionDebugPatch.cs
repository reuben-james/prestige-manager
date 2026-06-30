using System;
using System.Reflection;
using EFT.InventoryLogic;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace PrestigeManager.Patches
{
    internal class PrestigeTransferItemsStateToggleSelectionDebugPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(PrestigeTransferItemsState), nameof(PrestigeTransferItemsState.ToggleSelection));
        }

        [PatchPrefix]
        static void Prefix(GClass3455 context)
        {
            var item = context?.Item;
            if (item == null)
            {
                Plugin.Log.LogInfo("PrestigeManager: ToggleSelection called with null item.");
                return;
            }

            if (item is CompoundItem compoundItem)
            {
                Plugin.Log.LogInfo(
                    $"PrestigeManager: ToggleSelection called for compound item {item.Template?._id}, IsEmpty={compoundItem.IsEmpty}");
            }
            else
            {
                Plugin.Log.LogInfo(
                    $"PrestigeManager: ToggleSelection called for item {item.Template?._id}");
            }
        }

        [PatchFinalizer]
        static Exception Finalizer(Exception __exception, GClass3455 context)
        {
            if (__exception != null)
            {
                Plugin.Log.LogError($"PrestigeManager: ToggleSelection threw for item {context?.Item?.Template?._id}: {__exception}");
            }

            return __exception;
        }
    }
}