using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dialogs_basic.Dialogs.Home
{
    public class Body
    {
        [JsonProperty("ContentType")]
        public string ContentType { get; set; }
        [JsonProperty("Content")]
        public string Content { get; set; }
    }
}