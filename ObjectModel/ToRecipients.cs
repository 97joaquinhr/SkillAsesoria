using dialogs_basic.Dialogs.Home;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dialogs_basic
{
    public class ToRecipients
    {
        [JsonProperty("EmailAddress")]
        public EmailAddress EmailAddress { get; set; }
    }
}