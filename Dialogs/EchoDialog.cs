using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using Microsoft.Bot.Sample.SimpleEchoBot;
using Newtonsoft.Json;
using System.Collections.Generic;
using HtmlAgilityPack;
using Google.Apis.Customsearch.v1;
using Google.Apis.Customsearch.v1.Data;
using Google.Apis.Services;
using System.Linq;

namespace dialogs_basic
{
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        static IMessageActivity response;
        
        public static string location;
        protected int count = 1;
        public static string munic;
        
        public EchoDialog(string jsonLocation)
        {//si funciona, eliminar addresses
            
            try
            {
                List<Addresses> temp1 = JsonConvert.DeserializeObject<List<Addresses>>(jsonLocation);
                munic = temp1[0].address.municipality;
                
            }
            catch (Exception ex)
            {
                munic = ex.Message;
            }
            
            TimeZoneInfo targetZone = TimeZoneInfo.FindSystemTimeZoneById(MessagesController.timeZone);

            // Get local machine's value of Now
            DateTime utcDatetime = DateTime.UtcNow;
            // Get Central Standard Time value of Now
            DateTime userDatetime = TimeZoneInfo.ConvertTimeFromUtc(utcDatetime, targetZone);

            // Find timezoneOffset
            LUISAPI.timezoneOffset = (int)((userDatetime - utcDatetime).TotalMinutes);
            LUISAPI.Date = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/9190d074-1220-4156-95f2-b443bfd406a4?subscription-key=5e7fa924eefb4a619812fefca88e73cd&verbose=true&timezoneOffset=" + LUISAPI.timezoneOffset + "&q=";
            location = jsonLocation;
        }
        public async Task StartAsync(IDialogContext context)
        {
            response = context.MakeMessage();
            response.InputHint = InputHints.ExpectingInput;
            context.Wait(Selection);
        }

        public async Task Selection(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            try
            {
                Task<Intent> getIntent = Task.Run(() => LUISAPI.GetAsync(LUISAPI.Selection + message.Text));
                getIntent.Wait();
                switch (getIntent.Result.topScoringIntent.intent)
                {
                    case "AnotherCity":
                        string ciudad = getIntent.Result.entities[0].city;
                        context.Call<object>(new ThingsToDo(ciudad), AfterChildDialogIsDone);
                        break;
                    case "Local":
                        context.Call<object>(new ThingsToDo(munic), AfterChildDialogIsDone);
                        break;
                    case "PriceFlightAnotherCity":
                        response.Text = response.Speak = "Flight with 2 cities"+ getIntent.Result.entities[0].city+" "+ getIntent.Result.entities[1].city;
                        await context.PostAsync(response);
                        context.Wait(Selection);
                        break;
                    case "PriceFlightLocal":
                        response.Text = response.Speak = "flight from "+munic;
                        await context.PostAsync(response);
                        context.Wait(Selection);
                        break;
                    default:
                        response.Text = response.Speak = "I did not quite get that. Echo Dialog";
                        await context.PostAsync(response);
                        context.Wait(Selection);
                        break;
                }
            }
            catch (Exception ex1)
            {
                response.Speak = response.Text = "Luis stopped working. Exception: " + ex1.Message;
                await context.PostAsync(response);
                context.Wait(Selection);
            }
            
        }

        private async Task AfterChildDialogIsDone(IDialogContext context, IAwaitable<object> result)
        {
            response.Text = response.Speak = "Where do you want to go?";
            await context.PostAsync(response);
            context.Wait(Selection);
        }

    }
}