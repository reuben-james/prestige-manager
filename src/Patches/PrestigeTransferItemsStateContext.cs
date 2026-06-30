using System.Runtime.CompilerServices;

namespace PrestigeManager.Patches
{
    internal static class PrestigeTransferStateContext
    {
        private static readonly ConditionalWeakTable<object, LevelHolder> Table = new();

        public static void SetLevel(object instance, int level)
        {
            if (instance == null)
            {
                return;
            }

            Table.Remove(instance);
            Table.Add(instance, new LevelHolder(level));
        }

        public static int GetLevel(object instance)
        {
            if (instance == null)
            {
                return 0;
            }

            return Table.TryGetValue(instance, out var holder) ? holder.Level : 0;
        }

        public static void ClearLevel(object instance)
        {
            if (instance == null)
            {
                return;
            }

            Table.Remove(instance);
        }

        private sealed class LevelHolder
        {
            public int Level { get; }

            public LevelHolder(int level)
            {
                Level = level;
            }
        }
    }
}