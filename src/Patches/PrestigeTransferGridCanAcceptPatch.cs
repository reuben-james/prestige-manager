using System;
using System.Linq;
using System.Reflection;
using EFT.InventoryLogic;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace PrestigeManager.Patches
{
    internal class PrestigeTransferGridCanAcceptPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            var gridViewType = AccessTools.TypeByName("EFT.UI.GridView");
            if (gridViewType == null)
            {
                Plugin.Log.LogError("PrestigeManager: Could not resolve EFT.UI.GridView.");
                return null;
            }

            var methods = AccessTools.GetDeclaredMethods(gridViewType)
                .Where(m => m.Name == "CanAccept")
                .ToArray();

            foreach (var method in methods)
            {
                var p = method.GetParameters();
                Plugin.Log.LogInfo(
                    $"PrestigeManager: Candidate CanAccept => {method}, params = {string.Join(", ", p.Select(x => x.ParameterType.Name))}, return = {method.ReturnType.Name}");
            }

            var target = methods.FirstOrDefault(m =>
            {
                var p = m.GetParameters();
                return p.Length == 3
                    && typeof(ItemContextAbstractClass).IsAssignableFrom(p[0].ParameterType)
                    && typeof(ItemContextAbstractClass).IsAssignableFrom(p[1].ParameterType);
            });

            Plugin.Log.LogInfo($"PrestigeManager: Selected CanAccept target => {target}");
            return target;
        }

        [PatchPostfix]
        static void Postfix(object __instance, ItemContextAbstractClass itemContext, ItemContextAbstractClass targetItemContext, ref GStruct153 __result)
        {
            Plugin.Log.LogInfo($"PrestigeManager: CanAccept fired on {__instance?.GetType().FullName}");
            Plugin.Log.LogInfo($"PrestigeManager: itemContext = {itemContext?.GetType().FullName}, targetItemContext = {targetItemContext?.GetType().FullName}");
            Plugin.Log.LogInfo($"PrestigeManager: CanAccept result = {__result}");
        }
    }
}