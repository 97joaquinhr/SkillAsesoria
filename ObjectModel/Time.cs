using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetCalendar
{
    public class Time
    {
        [JsonProperty("DateTime")]
        public DateTime DateTime { get; set; }

        [JsonProperty("TimeZone")]
        public string TimeZone { get; set; }
    }
}
