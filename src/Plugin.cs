using BepInEx;
using BepInEx.Logging;
using PrestigeManager.Config;
using PrestigeManager.Patches;

namespace PrestigeManager
{
    [BepInPlugin("com.hoobastank.prestigemanager", "PrestigeManager", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;
            Log.LogInfo("PrestigeManager loading...");

            ConfigManager.Load();

            new PrestigeTransferItemsStateInitPatch().Enable();
            new PrestigeTransferItemsStateSizePatch().Enable();
            new PrestigeTransferItemsStateFilterPatch().Enable();
            new PrestigeTransferItemsStateClosePatch().Enable();

            Log.LogInfo("PrestigeManager loaded successfully.");
        }
    }
}
