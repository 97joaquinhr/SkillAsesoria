using System.Threading.Tasks;
using System.Web.Http;
using dialogs_basic;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Web.Http.Description;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Collections.Generic;


namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        static Location location;
        public static string timeZone;
        /// <summary>
        /// Cleans json to be handled and parsed into classes
        /// </summary>
        /// <param name="json1"></param>
        /// <returns></returns>
        static string cleanJson(string json1)
        {
            //delete first appearance of [ and last char
            int i = json1.IndexOf("[");
            json1 = json1.Substring(i, json1.Length - i - 1);
            i = json1.LastIndexOf("]");
            json1 = json1.Substring(0, i + 1);
            return json1;
        }

        /// <summary>
        /// POST: api/Messages
        /// receive a message from a user and send replies
        /// </summary>
        /// <param name="activity"></param>
        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            APIController.token = null;
            APIController.email = null;
            APIController.client.DefaultRequestHeaders.Authorization = null;
            APIController.client.DefaultRequestHeaders.Clear();
            APIController.client.DefaultRequestHeaders.Accept.Clear();
            APIController.client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            string jsonLocation = "";
            // check if activity is of type message
            if (activity != null && activity.GetActivityType() == ActivityTypes.Message)
            {
                for (int i = 0; i < activity.Entities.Count; ++i)
                {
                    if (activity.Entities[i].Type == "UserInfo")
                    {
                        foreach (JProperty property in activity.Entities[i].Properties.Properties())
                        {
                            if (property.Name == "current_location")
                            {
                                string temp = "";
                                temp += property.Value;
                                location = JsonConvert.DeserializeObject<Location>(temp);
                            }
                        }
                    }
                }
                jsonLocation = await APIController.Get("https://atlas.microsoft.com/timezone/byCoordinates/JSON?subscription-key=V3ekeP_nmT_hKt80gNvIfnna_GOnkAwR_BhZhKZl03s&api-version=1.0&query=" + location.Hub.Latitude + "," + location.Hub.Longitude).ConfigureAwait(false);
                jsonLocation = cleanJson(jsonLocation);
                var timezone = JsonConvert.DeserializeObject<List<TimeZone>>(jsonLocation);
                timeZone = timezone[0].Names.Standard;
                jsonLocation = await APIController.Get("https://atlas.microsoft.com/search/address/reverse/json?api-version=1.0&query=" + location.Hub.Latitude + "," + location.Hub.Longitude+"&subscription-key=JuXE_YnPpA-2G0TZx0tB2OcXQesyxznARhFZS5P6_g4");
                APIController.client.DefaultRequestHeaders.Add("Prefer", "outlook.timezone=\"" + timeZone + "\"");
                await Conversation.SendAsync(activity, () => new EchoDialog(jsonLocation));
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }
    }
}