using System;
using System.Reflection;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace PrestigeManager.Patches
{
    internal class PrestigeTransferGridRuntimeTypeProbePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            var type = AccessTools.TypeByName("EFT.UI.PrestigeTransferItemsState");
            return AccessTools.Method(type, nameof(PrestigeTransferItemsState.Init));
        }

        [PatchPostfix]
        static void Postfix(PrestigeTransferItemsState __instance)
        {
            try
            {
                var fields = __instance.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (var f in fields)
                {
                    object value = null;
                    try { value = f.GetValue(__instance); } catch { value = "<unreadable>"; }

                    if (value != null)
                    {
                        var valueType = value.GetType().FullName;
                        if (valueType.Contains("Grid") || valueType.Contains("View"))
                        {
                            Plugin.Log.LogInfo($"PrestigeManager: field {f.Name} => {valueType}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Plugin.Log.LogError($"PrestigeManager: runtime type probe failed: {e}");
            }
        }
    }
}