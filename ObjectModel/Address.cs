using Newtonsoft.Json;
using System.Collections.Generic;
namespace dialogs_basic
{
    public class Address
    {
        [JsonProperty("municipality")]
        public string municipality { get; set; }
    }
}