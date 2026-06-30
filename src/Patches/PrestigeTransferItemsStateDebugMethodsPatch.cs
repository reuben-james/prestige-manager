using System.Reflection;
using EFT.InventoryLogic;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace PrestigeManager.Patches
{
    internal class PrestigeTransferItemsStateDebugMethodsPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(PrestigeTransferItemsState), nameof(PrestigeTransferItemsState.Init));
        }

        [PatchPostfix]
        static void Postfix(PrestigeTransferItemsState __instance)
        {
            Plugin.Log.LogInfo($"PrestigeManager: PrestigeTransferItemsState runtime type = {__instance.GetType().FullName}");
        }
    }
}