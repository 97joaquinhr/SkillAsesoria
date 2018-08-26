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
        const string APIKey = "AIzaSyABqTGW4kDjOotodnWN-SgWje7eL3ivfqA";
        const string idSearch = "017246257753004761731:f1f_x9vqzmo";
        public static string location;
        protected int count = 1;
        public static string munic;
        public static CustomsearchService customSearchService;
        public EchoDialog(string jsonLocation)
        {//si funciona, eliminar addresses
            customSearchService = new CustomsearchService(new BaseClientService.Initializer { ApiKey = APIKey });
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
            CseResource.ListRequest listRequest;
            var message = await argument;
            try
            {
                Task<Intent> getIntent = Task.Run(() => LUISAPI.GetAsync(LUISAPI.Selection + message.Text));
                getIntent.Wait();
                HtmlWeb web = new HtmlWeb();
                HtmlDocument htmlDocPlaces;
                HtmlDocument htmlDocReview;
                IEnumerable<HtmlNode> locationsToVisit = Enumerable.Empty<HtmlNode>();

                string query;
                string[] places = new string[3];
                int indexOption;
                switch (getIntent.Result.topScoringIntent.intent)
                {
                    case "AnotherCity":
                        string ciudad = getIntent.Result.entities[0].city;
                        query = "Tripadvisor attractions " + ciudad;
                        listRequest = customSearchService.Cse.List(query);
                        listRequest.Cx = idSearch;

                        var search = listRequest.Execute();

                        web = new HtmlWeb();
                        htmlDocPlaces = web.Load(search.Items.ElementAt(0).Link);
                        locationsToVisit = htmlDocPlaces.DocumentNode
                            .Descendants()
                            .Where(n => n.NodeType == HtmlNodeType.Element)
                            .Where(e => e.Name == "div" && e.GetAttributeValue("class", "") == "listing_title ");
                        response.Text = "";
                        locationsToVisit = locationsToVisit.Take(3);
                        var htmlDocLocation = new HtmlDocument();
                        int i = 0;
                        foreach (var location in locationsToVisit)
                        {
                            htmlDocLocation.LoadHtml(location.InnerHtml);
                            var loc = htmlDocLocation.DocumentNode
                                .Descendants().Where(e2 => e2.Name == "a").First();
                            places[i++] = loc.InnerText;
                            response.Text += loc.InnerText + "\n\n";
                        }
                        response.Speak = response.Text;
                        await context.PostAsync(response);
                        context.Wait(Selection);
                        break;
                    case "Local":
                        query = "Tripadvisor attractions " + munic;
                        listRequest = customSearchService.Cse.List(query);
                        listRequest.Cx = idSearch;

                        search = listRequest.Execute();

                        web = new HtmlWeb();

                        htmlDocPlaces = web.Load(search.Items.ElementAt(0).Link);
                        locationsToVisit = htmlDocPlaces.DocumentNode
                            .Descendants()
                            .Where(n => n.NodeType == HtmlNodeType.Element)
                            .Where(e => e.Name == "div" && e.GetAttributeValue("class", "") == "listing_title ");
                        response.Text = "";
                        locationsToVisit = locationsToVisit.Take(3);
                        foreach (var location in locationsToVisit)
                        {
                            response.Text += location.InnerText.Replace("\n", "") + "\n\n";
                        }
                        response.Speak = response.Text;
                        await context.PostAsync(response);
                        context.Wait(Selection);
                        break;
                    case "Reviews":
                        bool ordinal = false;
                        int indexFound = 0;
                        foreach(var types in getIntent.Result.entities)
                        {
                            if(types.type.Contains("ordinal"))
                            {
                                ordinal = true;
                                break;
                            }
                            indexFound++;
                        }
                        if (ordinal)
                        {
                            indexOption = getIntent.Result.entities[indexFound].resolution.value;
                        } else
                        {
                            indexOption = getIntent.Result.entities[0].resolution.value;
                        }

                        web = new HtmlWeb();
                        string link = locationsToVisit.ElementAt(indexOption - 1).InnerHtml;
                        var index1 = link.IndexOf('"');
                        link = link.Remove(index1, 1);
                        var index2 = link.IndexOf('"');
                        link = link.Substring(index1, index2 - index1);
                        htmlDocReview = web.Load("https://www.tripadvisor.com" + link);
                        var reviews = htmlDocReview.DocumentNode
                            .Descendants()
                            .Where(n => n.NodeType == HtmlNodeType.Element)
                            .Where(e => e.Name == "div" && e.GetAttributeValue("class", "") == "review-container");
                        response.Text = "";
                        reviews = reviews.Take(3);
                        var htmlDocReviewTitle = new HtmlDocument();
                        foreach (var review in reviews)
                        {
                            string htmlTemporal = (string)review.InnerHtml;
                            htmlDocReviewTitle.LoadHtml(htmlTemporal);
                            var reviewTitle = htmlDocReviewTitle.DocumentNode
                                .Descendants()
                                .Where(n => n.NodeType == HtmlNodeType.Element)
                                .Where(e => e.Name == "span" && e.GetAttributeValue("class", "") == "noQuotes").First();
                            response.Text += reviewTitle.InnerText + "\n\n";
                        }
                        response.Speak = response.Text;
                        await context.PostAsync(response);
                        context.Wait(Selection);
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
                        response.Text = response.Speak = "I did not quite get that.";
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
            response.Text = "Home Base for Zen Commitments. \n\n Options: \n\n -See suggested tasks \n\n -Saved Tasks \n\n Go to History \n\n What's on my calendar? \n\n Look at my goals";
            response.Speak = "You have returned to home base. What would you like to do?";
            await context.PostAsync(response);
            context.Wait(Selection);
        }

    }
}