using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace dialogs_basic
{
    public class Email
    {
        [JsonProperty("Message")]
        public Messages Message { get; set; }

        [JsonProperty("SaveToSentItems")]
        public bool SaveToSentItems { get; set; }

        public static implicit operator Email(EmailBody v)
        {
            throw new NotImplementedException();
        }
        public Email()
        {
            Message = new Messages {
                Body = new EmailBody {
                    ContentType="Text"
                },
                ToRecipients = new List<ToRecipients>()
            };
            //You can not save it to sent items if it is a draft. I think.
            SaveToSentItems = true;
        }
    }
}