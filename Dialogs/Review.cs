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

    // this class lets the user go through a list of their sTasks and take actions on the tasks

    [Serializable]
    public class Review : IDialog<object>
    {
        int indexOption;
        HtmlWeb web;
        public static HtmlDocument htmlDocReview;
        static IMessageActivity response;
        private IEnumerable<HtmlNode> locationsToVisit;
        List<Entity> entities;

        public Review(IEnumerable<HtmlNode> locationsToVisit, List<Entity> entities)
        {
            this.locationsToVisit = locationsToVisit;
            this.entities = entities;
        }

        public async Task StartAsync(IDialogContext context)
        {
            response = context.MakeMessage();
            response.InputHint = InputHints.ExpectingInput;
            await this.Title(context);
        }

        private async Task Title(IDialogContext context)
        {
            response.Text = response.Speak = "on title";
            await context.PostAsync(response);
            bool ordinal = false;
            int indexFound = 0;
            foreach (var types in entities)
            {
                if (types.type.Contains("ordinal"))
                {
                    ordinal = true;
                    break;
                }
                indexFound++;
            }

            if (ordinal)
            {
                indexOption = entities[indexFound].resolution.value;
            }
            else
            {
                indexOption = entities[0].resolution.value;
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
        }
        public async Task Selection(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            Task<Intent> getIntent = Task.Run(() => LUISAPI.GetAsync(LUISAPI.Reviews + message.Text));
            getIntent.Wait();
            switch (getIntent.Result.topScoringIntent.intent)
            {
                case "Back":
                    context.Done<object>(new object());
                    break;
                case "Details":
                    response.Speak = response.Text = "Details adsf";
                    await context.PostAsync(response);
                    context.Done<object>(new object());
                    break;
                default:
                    response.Speak = response.Text = "I did not get that.";
                    await context.PostAsync(response);
                    context.Wait(Selection);
                    break;

            }
        }
    }
}