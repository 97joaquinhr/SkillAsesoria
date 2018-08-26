using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dialogs_basic
{
    public class Messages
    {
        [JsonProperty("Subject")]
        public string Subject { get; set; }

        [JsonProperty("Body")]
        public EmailBody Body { get; set; }

        [JsonProperty("ToRecipients")]
        public List<ToRecipients> ToRecipients { get; set; }


    }
}