using System.Collections.Generic;
using EFT.UI;

namespace PrestigeManager.Patches
{
    internal static class PrestigeTransferStateContext
    {
        private static readonly Dictionary<PrestigeTransferItemsState, int> LevelsByState = new();

        public static void SetLevel(PrestigeTransferItemsState state, int prestigeLevel)
        {
            if (state == null)
            {
                return;
            }

            LevelsByState[state] = prestigeLevel;
        }

        public static int GetLevel(PrestigeTransferItemsState state)
        {
            if (state == null)
            {
                return 0;
            }

            return LevelsByState.TryGetValue(state, out var level) ? level : 0;
        }

        public static void ClearLevel(PrestigeTransferItemsState state)
        {
            if (state == null)
            {
                return;
            }

            LevelsByState.Remove(state);
        }
    }
}