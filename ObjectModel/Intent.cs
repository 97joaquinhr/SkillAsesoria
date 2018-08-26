using Newtonsoft.Json;
using System.Collections.Generic;

namespace dialogs_basic
{
    public class Intent
    {
        [JsonProperty("query")]
        public string query { get; set; }
        [JsonProperty("topScoringIntent")]
        public ScoringIntent topScoringIntent { get; set; }
        [JsonProperty("entities")]
        public List<Entity> entities { get; set; }
        
    }
}