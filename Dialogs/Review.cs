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
        public static int indexOption;
        public static HtmlWeb web;
        public static HtmlDocument htmlDocReview;
        static IMessageActivity response;
        public static List<Entity> entitiesDetails;

        public Review()
        {

            entitiesDetails = new List<Entity>();
        }

        public async Task StartAsync(IDialogContext context)
        {
            response = context.MakeMessage();
            response.InputHint = InputHints.ExpectingInput;
            await this.Title(context);
        }

        public async Task Options(IDialogContext context, IAwaitable<IMessageActivity> argument)
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
                    entitiesDetails = getIntent.Result.entities;
                    await this.DetailsComments(context);
                    break;
                case "Email":
                    if (getIntent.Result.entities[0].city.ToLower().Contains("daniel"))
                    {
                        context.Call<object>(new EmailDialog("luis_060198@hotmail.com", "Let's go here\n" + ThingsToDo.places+"\nBy TripAdvisor."), AfterChildDialogIsDone);
                    }
                    else
                    {
                        response.Speak = response.Text = "I don't know that contact";
                        await context.PostAsync(response);
                        context.Wait(Options);
                    }
                    break;
                default:
                    response.Speak = response.Text = "I did not get that option.";
                    await context.PostAsync(response);
                    context.Wait(Options);
                    break;

            }

        }

        public async Task Title(IDialogContext context)

        {
            bool ordinal = false;
            int indexFound = 0;
            foreach (var types in ThingsToDo.entities)
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
                indexOption = ThingsToDo.entities[indexFound].resolution.value;
            }
            else
            {
                indexOption = ThingsToDo.entities[0].resolution.value;
            }

            web = new HtmlWeb();
            string link = ThingsToDo.locationsToVisit.ElementAt(indexOption - 1).InnerHtml;
            var index1 = link.IndexOf('"');
            link = link.Remove(index1, 1);
            var index2 = link.IndexOf('"');
            link = link.Substring(index1, index2 - index1);
            htmlDocReview = web.Load("https://www.tripadvisor.com" + link);
            var reviews = htmlDocReview.DocumentNode
                .Descendants()
                .Where(n => n.NodeType == HtmlNodeType.Element)
                .Where(e => e.Name == "div" && e.GetAttributeValue("class", "") == "review-container");
            response.Text = "This are some user reviews of that place";
            reviews = reviews.Take(5);
            var htmlDocReviewTitle = new HtmlDocument();
            foreach (var review in reviews)
            {
                string htmlTemporal = (string)review.InnerHtml;
                htmlDocReviewTitle.LoadHtml(htmlTemporal);
                var reviewTitle = htmlDocReviewTitle.DocumentNode
                    .Descendants()
                    .Where(n => n.NodeType == HtmlNodeType.Element)
                    .Where(e => e.Name == "span" && e.GetAttributeValue("class", "") == "noQuotes").First();
                response.Text += reviewTitle.InnerText.Replace("&#39;", "'") + "\n\n";
            }
            response.Speak = response.Text;
            await context.PostAsync(response);
            context.Wait(Options);
        }

        public async Task DetailsComments(IDialogContext context)
        {
            bool ordinal = false;
            int indexFound = 0;
            foreach (var types in entitiesDetails)
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
                indexOption = entitiesDetails[indexFound].resolution.value;
            }
            else
            {
                indexOption = entitiesDetails[0].resolution.value;
            }

            response.Text = "";
            var comments = htmlDocReview.DocumentNode
                .Descendants()
                .Where(n => n.NodeType == HtmlNodeType.Element)
                .Where(e => e.Name == "div" && e.GetAttributeValue("class", "") == "review-container");
            //var userNames = htmlDocReview.DocumentNode
            //                .Descendants()
            //                .Where(n => n.NodeType == HtmlNodeType.Element)
            //                .Where(e => e.Name == "div" && e.GetAttributeValue("class", "") == "info_text");
            //var htmlDocUserName = new HtmlDocument();
            //htmlDocUserName.LoadHtml(userNames.ElementAt(indexOption - 1).InnerHtml);
            //response.Text += htmlDocUserName.DocumentNode.Descendants().First().InnerText + " said ";
            var comment = comments.ElementAt(indexOption - 1);
            var htmlDocCommentDetail = new HtmlDocument();
            htmlDocCommentDetail.LoadHtml(comment.InnerHtml);
            var commentDetail = htmlDocCommentDetail.DocumentNode
                .Descendants()
                .Where(n => n.NodeType == HtmlNodeType.Element)
                .Where(e => e.Name == "p").First();
            if (commentDetail.InnerHtml.Contains("<span"))
            {
                response.Text += commentDetail.InnerText.Replace("&#39;", "'").Substring(0, commentDetail.InnerHtml.IndexOf("<span")) + "\n\n";
            }
            else
            {
                response.Text += commentDetail.InnerText.Replace("&#39;", "'") + "\n\n";
            }
            
            
            response.Speak = response.Text;
            await context.PostAsync(response);
            context.Wait(Options);
        }

        public async Task AfterChildDialogIsDone(IDialogContext context, IAwaitable<object> result)
        {
            context.Done<object>(new object());
        }
    }
}