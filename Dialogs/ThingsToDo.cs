﻿using Microsoft.Bot.Builder.Dialogs;
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
using Google.Apis.Services;

namespace dialogs_basic
{

    // this class lets the user go through a list of their sTasks and take actions on the tasks

    [Serializable]
    public class ThingsToDo : IDialog<object>
    {
        static IMessageActivity response;
        const string APIKey = "AIzaSyABqTGW4kDjOotodnWN-SgWje7eL3ivfqA";
        const string idSearch = "017246257753004761731:f1f_x9vqzmo";
        public static CustomsearchService customSearchService;
        public static IEnumerable<HtmlNode> locationsToVisit;
        public static CseResource.ListRequest listRequest;
        public static HtmlDocument htmlDocPlaces;
        
        public static string ciudad;
        HtmlWeb web;
        string query;
        
        public ThingsToDo(string c)
        {
            customSearchService = new CustomsearchService(new BaseClientService.Initializer { ApiKey = APIKey });
            ciudad = c;
            locationsToVisit = Enumerable.Empty<HtmlNode>();
        }
        public async Task StartAsync(IDialogContext context)
        {
            response = context.MakeMessage();
            response.InputHint = InputHints.ExpectingInput;
            web = new HtmlWeb();
            IEnumerable<HtmlNode> locationsToVisit = Enumerable.Empty<HtmlNode>();
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
            foreach (var location in locationsToVisit)
            {
                htmlDocLocation.LoadHtml(location.InnerHtml);
                var loc = htmlDocLocation.DocumentNode
                    .Descendants().Where(e2 => e2.Name == "a").First();
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
                    context.Call<object>(new Review(locationsToVisit,getIntent.Result.entities), AfterChildDialogIsDone);
                    break;
                case "AnotherCity":
                    ciudad = getIntent.Result.entities[0].city;
                    await this.Attracions(context);
                    break;
                case "Local":
                    ciudad = EchoDialog.munic;
                    await this.Attracions(context);
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
            response.Speak = response.Text = "You came back";
            await context.PostAsync(response);
            await this.Attracions(context);
        }

    }
}