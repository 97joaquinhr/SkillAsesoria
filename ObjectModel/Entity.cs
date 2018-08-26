using Newtonsoft.Json;
using System.Collections.Generic;
namespace dialogs_basic
{
    public class Entity
    {
        [JsonProperty("entity")]
        public string city { get; set; }
        [JsonProperty("resolution")]
        public Resolution resolution { get; set; }

    }
}