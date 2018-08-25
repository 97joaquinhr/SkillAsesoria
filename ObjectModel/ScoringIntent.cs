using Newtonsoft.Json;

namespace dialogs_basic
{
    class ScoringIntent
    {
        [JsonProperty("intent")]
        public string intent { get; set; }
        [JsonProperty("score")]
        public float score { get; set; }
    }
}