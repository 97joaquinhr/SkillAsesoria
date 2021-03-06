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
        const string APIKey = "AIzaSyAoGtOIqQQevctOw5mDLUrpqQW1TeZV6Jk";
        const string idSearch = "017246257753004761731:f1f_x9vqzmo";
        public static CustomsearchService customSearchService;
        public static IEnumerable<HtmlNode> locationsToVisit;
        public static CseResource.ListRequest listRequest;
        public static List<Entity> entities;
        public static HtmlDocument htmlDocPlaces;
        public static string places;
        public static string ciudad;
        public static HtmlWeb web;
        public static string query;
        
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
            places = "This are the top 5 things to do in " + ciudad + "\n\n";
            locationsToVisit = locationsToVisit.Take(5);
            var htmlDocLocation = new HtmlDocument();
            foreach (var location in locationsToVisit)
            {
                htmlDocLocation.LoadHtml(location.InnerHtml);
                var loc = htmlDocLocation.DocumentNode
                    .Descendants().Where(e2 => e2.Name == "a").First();
                places += loc.InnerText + "\n\n";
            }
            response.Speak = response.Text = places;
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
                case "Reviews":
                    entities = getIntent.Result.entities;
                    context.Call<object>(new Review(), AfterChildDialogIsDone);
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
                    response.Speak = response.Text = "I did not get that. Things to do";
                    await context.PostAsync(response);
                    context.Wait(Selection);
                    break;

            }
        }



        public async Task AfterChildDialogIsDone(IDialogContext context, IAwaitable<object> result)
        {
            response.Text = places;
            await context.PostAsync(response);
            context.Wait(Selection);
        }

    }
}