using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dialogs_basic
{
    public class TimeZone
    {
        [JsonProperty("Id")]
        public string Id { get; set; }
        [JsonProperty("Names")]
        public Names Names { get; set; }

    }
}
