using Newtonsoft.Json;
using System.Collections.Generic;
namespace dialogs_basic
{
    class Entity
    {
        [JsonProperty("resolution")]
        public Resolution resolution { get; set; }

    }
}