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
    [Serializable]
    public class Reviews : IDialog<object>
    {
        static IMessageActivity response;

        public async Task StartAsync(IDialogContext context)
        {
            response = context.MakeMessage();
            response.InputHint = InputHints.ExpectingInput;
            await this.Comments(context);

        }

        private Task Comments(IDialogContext context)
        {
            bool ordinal = false;
            int indexFound = 0;
            foreach (var types in getIntent.Result.entities)
            {
                if (types.type.Contains("ordinal"))
                {
                    ordinal = true;
                    break;
                }
                indexFound++;
            }
            await context.PostAsync(response);
            if (ordinal)
            {
                indexOption = getIntent.Result.entities[indexFound].resolution.value;
            }
            else
            {
                indexOption = getIntent.Result.entities[0].resolution.value;
            }
            response.Speak = response.Text = "Hello " + indexFound;
            await context.PostAsync(response);
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
    }
}