using System.Reflection;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace PrestigeManager.Patches
{
    internal class PrestigeTransferItemsStateClosePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(PrestigeTransferItemsState), nameof(PrestigeTransferItemsState.Close));
        }

        [PatchPostfix]
        static void Postfix(PrestigeTransferItemsState __instance)
        {
            PrestigeTransferStateContext.ClearLevel(__instance);
            Plugin.Log.LogInfo("PrestigeManager: Cleared stored prestige level for transfer state instance.");
        }
    }
}