using Newtonsoft.Json;
using System.Collections.Generic;

namespace dialogs_basic
{
    class Hub
    {
        [JsonProperty("Latitude")]
        public string Latitude { get; set; }
		[JsonProperty("Longitude")]
        public string Longitude { get; set; }
        
    }
}