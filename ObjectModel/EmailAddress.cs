using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dialogs_basic
{
    public class EmailAddress
    {
        [JsonProperty("Address")]
        public string Address { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        public EmailAddress(string a)
        {
            Address = a;
        }
    }
}