using System.Reflection;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace PrestigeManager.Patches
{
    internal class PrestigeTransferItemsStateInitPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(PrestigeTransferItemsState), nameof(PrestigeTransferItemsState.Init));
        }

        [PatchPrefix]
        static void Prefix(PrestigeTransferItemsState __instance, int prestigeLevelToOpen)
        {
            PrestigeTransferStateContext.SetLevel(__instance, prestigeLevelToOpen);

            var transferViewField = AccessTools.Field(__instance.GetType(), "_transferView");
            var transferView = transferViewField?.GetValue(__instance);
            if (transferView != null)
            {
                PrestigeTransferStateContext.SetLevel(transferView, prestigeLevelToOpen);
            }
            Plugin.Log.LogInfo($"PrestigeManager: Stored prestige level {prestigeLevelToOpen} for transfer state instance.");
        }
    }
}