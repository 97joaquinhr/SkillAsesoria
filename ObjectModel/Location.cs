using Newtonsoft.Json;
using System.Collections.Generic;

namespace dialogs_basic
{
    class Location
    {
        [JsonProperty("Hub")]
        public Hub Hub { get; set; }
        
    }
}