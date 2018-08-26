using Newtonsoft.Json;
using System.Collections.Generic;
namespace dialogs_basic
{
    public class Resolution
    {
        [JsonProperty("values")]
        public List<Value> values { get; set; }
    }
}