using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
namespace dialogs_basic
{
	public static class LUISAPI
    {
        public static HttpClient client = new HttpClient();
        public static int timezoneOffset{set; get;}
        public static string Home = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/cbe0a54a-b1b0-4f2f-9c36-ae03c97c5f34?subscription-key=09b02f791be64f148810e01ca24ff1f5&verbose=true&timezoneOffset="+timezoneOffset+"&q=";
        public static string Action = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/c33491b4-9464-4f71-bb82-d81862c8738d?subscription-key=5e7fa924eefb4a619812fefca88e73cd&verbose=true&timezoneOffset="+timezoneOffset+"&q=";
        public static string Write = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/cba5cff9-20a1-4a40-8c22-7ad5a75ef696?subscription-key=5e7fa924eefb4a619812fefca88e73cd&verbose=true&timezoneOffset=0&q=";
        public static string Date {set; get;}//program base
        public static string SendEmail = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/cf063e6e-5868-456f-ab4e-391c8fafdf17?subscription-key=09b02f791be64f148810e01ca24ff1f5&verbose=true&timezoneOffset="+timezoneOffset+"&q=";
        public static string HelpApp = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/d09fcaac-9212-429b-b05d-527872f72e6e?subscription-key=09b02f791be64f148810e01ca24ff1f5&verbose=true&timezoneOffset=0&q=";
        public static string CalendarCheckApp = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/48b7caca-abf8-48bf-aac9-d0184dd41ca7?subscription-key=09b02f791be64f148810e01ca24ff1f5&verbose=true&timezoneOffset="+timezoneOffset+"&q=";
        public static string SubTask ="https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/d9d4d30c-cd57-48b2-a7e2-838bdcfa185b?subscription-key=5e7fa924eefb4a619812fefca88e73cd&verbose=true&timezoneOffset=0&q=";
        public static string History ="https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/d1f22846-4a2f-4d42-938c-ee4cda85b190?subscription-key=09b02f791be64f148810e01ca24ff1f5&verbose=true&timezoneOffset=0&q=";
        
        public static async Task<string> GetAsync(string uri)
        {
            string json=null;
            Intent aux=null;
            HttpResponseMessage response = await client.GetAsync(uri);
            HttpContent content = response.Content;
            json = await content.ReadAsStringAsync();
            aux = JsonConvert.DeserializeObject<Intent>(json);
            return aux.topScoringIntent.intent;
        }
    }
}