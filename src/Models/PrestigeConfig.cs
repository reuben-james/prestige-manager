using System.Collections.Generic;
using Newtonsoft.Json;

namespace PrestigeManager.Models
{
    public class PrestigeConfig
    {
        [JsonProperty("prestigeLevels")]
        public Dictionary<int, PrestigeLevelConfig> PrestigeLevels { get; set; } = new Dictionary<int, PrestigeLevelConfig>();
    }

    public class PrestigeLevelConfig
    {
        [JsonProperty("xCellCount")]
        public int? XCellCount { get; set; } = null;

        [JsonProperty("yCellCount")]
        public int? YCellCount { get; set; } = null;

        [JsonProperty("filters")]
        public FiltersConfig Filters { get; set; } = new FiltersConfig();
    }

    public class FiltersConfig
    {
        [JsonProperty("includedItems")]
        public List<string> IncludedItems { get; set; } = new List<string>();
    }
}
