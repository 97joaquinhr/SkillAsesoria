using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dialogs_basic
{
    /*
     * I think we should order this classes into a folder called ObjectModel.
     */
    public class Attendee
    {
        /// <summary>
        /// Object for email 
        /// </summary>
        [JsonProperty("EmailAddress")]
        public EmailAddress EmailAddress { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }
    }
}