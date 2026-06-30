using System;
using System.Reflection;
using EFT.InventoryLogic;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace PrestigeManager.Patches
{
    internal class PrestigeTransferItemsStateMethod2DebugPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(PrestigeTransferItemsState), "method_2");
        }

        [PatchPrefix]
        static void Prefix(Item item, object to = null)
        {
            if (item == null)
            {
                Plugin.Log.LogInfo("PrestigeManager: method_2 called with null item.");
                return;
            }

            if (item is CompoundItem compoundItem)
            {
                Plugin.Log.LogInfo(
                    $"PrestigeManager: method_2 called for compound item {item.Template?._id}, IsEmpty={compoundItem.IsEmpty}, toNull={to == null}");
            }
            else
            {
                Plugin.Log.LogInfo(
                    $"PrestigeManager: method_2 called for item {item.Template?._id}, toNull={to == null}");
            }
        }

        [PatchFinalizer]
        static Exception Finalizer(Exception __exception, Item item)
        {
            if (__exception != null)
            {
                Plugin.Log.LogError($"PrestigeManager: method_2 threw for item {item?.Template?._id}: {__exception}");
            }

            return __exception;
        }
    }
}