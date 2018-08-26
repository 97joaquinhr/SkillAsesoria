using System;
using Newtonsoft.Json;
namespace dialogs_basic
{
    class User
    {
        [JsonProperty("Value")]
        public string Email { get; set; }
    }
}
