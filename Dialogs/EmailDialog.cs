using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace dialogs_basic
{
    [Serializable]
    public class EmailDialog : IDialog<object>
    {
        public static Email em;
        static IMessageActivity response;
        string id;
        
        public EmailDialog(string Address,string body)
        {
            em = new Email();
            EmailAddress aux = new EmailAddress(Address);
            em.Message.ToRecipients.Add(
                new ToRecipients
                {
                    EmailAddress = aux
                }
            );
            em.Message.Subject = "TripAdvisor recomendation with Cortana";
            em.Message.Body.Content = body;


        }


        static JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver(),
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };
        public async Task StartAsync(IDialogContext context)
        {
            response = context.MakeMessage();
            response.InputHint = InputHints.ExpectingInput;
            await this.PostEmail(context);
        }
        


        //Sends Email
        private async Task PostEmail(IDialogContext context)
        {
            string json = await Task.Run(() => JsonConvert.SerializeObject(em, settings));
            Task<string> PostEmail = Task.Run(() => APIController.Post("https://outlook.office.com/api/v2.0/me/sendmail", json));
            PostEmail.Wait();
            response.Text = "Email sent!";
            response.Speak = "Email sent!";
            response.InputHint = InputHints.IgnoringInput;
            await context.PostAsync(response);
            context.Done<object>(new object());
        }

    }
}