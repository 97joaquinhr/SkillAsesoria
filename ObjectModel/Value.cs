using Newtonsoft.Json;
using System;
using System.Collections.Generic;
namespace dialogs_basic
{
    class Value
    {
        [JsonProperty("type")]
        public string type { get; set; }
		[JsonProperty("value")]
        public DateTime value { get; set; }
        [JsonProperty("start")]
        public DateTime start { get; set; }

    }
}