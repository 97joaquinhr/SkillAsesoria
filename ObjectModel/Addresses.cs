using Newtonsoft.Json;
using System.Collections.Generic;
namespace dialogs_basic
{
    public class Addresses
    {
        [JsonProperty("Address")]
        public Address address { get; set; }

    }
}