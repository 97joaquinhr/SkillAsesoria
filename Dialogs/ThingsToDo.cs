using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using HtmlAgilityPack;
using Google.Apis.Customsearch.v1;
using System.Text.RegularExpressions;
using System.Globalization;
using Microsoft.Bot.Sample.SimpleEchoBot;

namespace dialogs_basic
{

    // this class lets the user go through a list of their sTasks and take actions on the tasks

    [Serializable]
    public class ThingsToDo : IDialog<object>
    {
        const string APIKey = "AIzaSyABqTGW4kDjOotodnWN-SgWje7eL3ivfqA";
        const string idSearch = "017246257753004761731:f1f_x9vqzmo";
        HtmlWeb web;
        HtmlDocument htmlDocPlaces;
        HtmlDocument htmlDocReview;
        public static CustomsearchService customSearchService;
        CseResource.ListRequest listRequest;
        IEnumerable<HtmlNode> locationsToVisit = Enumerable.Empty<HtmlNode>();

        public static string query;
        public static string ciudad;
        string[] places = new string[3];
        int indexOption;
        static IMessageActivity response;
        public ThingsToDo(string c)
        {
            ciudad = c;
        }
        public async Task StartAsync(IDialogContext context)
        {
            web = new HtmlWeb();
            response = context.MakeMessage();
            response.InputHint = InputHints.ExpectingInput;
            await this.Attracions(context);
            
        }

        //------ The Setup---------------------       
        public async Task Attracions(IDialogContext context)
        {
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
        }

        //------ Posting the Tasks ----------------
        public async Task Selection(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            Task<Intent> getIntent = Task.Run(() => LUISAPI.GetAsync(LUISAPI.Selection + message.Text));
            getIntent.Wait();
            switch (getIntent.Result.topScoringIntent.intent)
            {
                case "Review":
                    context.Call<object>(new Reviews(), AfterChildDialogIsDone);
                    break;
                default:
                    response.Speak = response.Text = "I did not get that.";
                    await context.PostAsync(response);
                    context.Wait(Selection);
                    break;

            }
        }



        private async Task AfterChildDialogIsDone(IDialogContext context, IAwaitable<object> result)
        {
            counter++;
            await this.Attracions(context);
        }

    }
}