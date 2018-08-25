using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace dialogs_basic
{
    public static class APIController
    {
        public static string token {set; get;}
        public static string email {set; get;}
        public static HttpClient client = new HttpClient();
        ///<summary>
        ///Method for HTTP GET request. HttpResponseMessage used.
        ///</summary>
        public static async Task<String> Get(string uri)
        {
            HttpResponseMessage response = await client.GetAsync(uri);
            HttpContent content = response.Content;
            string result = await content.ReadAsStringAsync();
            return result;
        }
        //"https://outlook.office.com/api/v2.0/me/events"
        //var json = await Task.Run(() => JsonConvert.SerializeObject(jsonR));
        ///<summary>
        ///Method for HTTP POST request. Encodes to ASCII.
        ///</summary>
        public static async Task<String> Post(string uri,string json)
        {
            var content = new StringContent(json,Encoding.ASCII,"application/json");
            var response = await client.PostAsync(uri, content);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
        public static async Task<String> Delete(string uri)
        {
            var response = await client.DeleteAsync(uri);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
        public static async Task<string> PatchAsync(string uri, string json)
        {
            var method = new HttpMethod("PATCH");
            var iContent = new StringContent(json, Encoding.ASCII, "application/json");
            var request = new HttpRequestMessage(method, uri)
            {
                Content = iContent
            };
 
            HttpResponseMessage response = new HttpResponseMessage();
            // In case you want to set a timeout
            //CancellationToken cancellationToken = new CancellationTokenSource(60).Token;
            
            try
            {
                response = await client.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();
                return result;
                // If you want to use the timeout you set
                //response = await client.SendRequestAsync(request).AsTask(cancellationToken);
            }
            catch (TaskCanceledException e)
            {
                return e.ToString();
            }
            
        }
        
    }
}