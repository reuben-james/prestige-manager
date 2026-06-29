using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using PrestigeManager.Models;

namespace PrestigeManager.Config
{
    public static class ConfigManager
    {
        public static PrestigeConfig Config { get; private set; } = new PrestigeConfig();

        private const string ConfigFileName = "prestige_config.json";

        public static void Load()
        {
            try
            {
                string pluginDir  = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string modDir     = Path.Combine(pluginDir, "PrestigeManager");
                string configPath = Path.Combine(modDir, ConfigFileName);

                Plugin.Log.LogInfo($"PrestigeManager: Looking for config at {configPath}");

                if (!File.Exists(configPath))
                {
                    Plugin.Log.LogWarning($"PrestigeManager: Config file not found at {configPath}. Using game defaults for all prestige levels.");
                    Config = new PrestigeConfig();
                    return;
                }

                string json = File.ReadAllText(configPath);
                Config = JsonConvert.DeserializeObject<PrestigeConfig>(json) ?? new PrestigeConfig();

                Plugin.Log.LogInfo($"PrestigeManager: Config loaded with {Config.PrestigeLevels.Count} prestige level(s) configured.");
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"PrestigeManager: Failed to load config: {ex.Message}");
                Plugin.Log.LogWarning("PrestigeManager: Falling back to empty config, game defaults will be used.");
                Config = new PrestigeConfig();
            }
        }

        public static PrestigeLevelConfig GetLevelConfig(int prestigeLevel)
        {
            if (Config.PrestigeLevels.TryGetValue(prestigeLevel, out var levelConfig))
                return levelConfig;

            return null;
        }
    }
}
