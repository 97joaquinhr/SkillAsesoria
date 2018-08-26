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
        public static string Selection = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/101da748-af0f-4bb5-89a9-f0c60a3c7354?subscription-key=e3c4ac7e56f94feba02ffd8ec72e377b&verbose=true&timezoneOffset=0&q=";
        public static string Reviews = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/f28f2330-211e-450c-ad76-20d9efd45451?subscription-key=e3c4ac7e56f94feba02ffd8ec72e377b&verbose=true&timezoneOffset=0&q=";
        public static string Write = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/cba5cff9-20a1-4a40-8c22-7ad5a75ef696?subscription-key=5e7fa924eefb4a619812fefca88e73cd&verbose=true&timezoneOffset=0&q=";
        public static string Date {set; get;}//program base
        
        public static async Task<Intent> GetAsync(string uri)
        {
            string json=null;
            Intent aux=null;
            HttpResponseMessage response = await client.GetAsync(uri);
            HttpContent content = response.Content;
            json = await content.ReadAsStringAsync();
            aux = JsonConvert.DeserializeObject<Intent>(json);
            return aux;
        }
    }
}