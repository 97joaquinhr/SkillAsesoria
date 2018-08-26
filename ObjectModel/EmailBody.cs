using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace dialogs_basic
{
    public class EmailBody
    {
        [JsonProperty("ContentType")]
        public string ContentType { get; set; }

        [JsonProperty("Content")]
        public string Content { get; set; }
    }
}