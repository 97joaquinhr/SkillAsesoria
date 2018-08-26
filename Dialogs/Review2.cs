using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using HtmlAgilityPack;

namespace dialogs_basic
{
    [Serializable]
    public class Review2 : IDialog<object>
    {
        public static string note;
        static IMessageActivity response;
        public static IEnumerable<HtmlNode> locationsToVisit;
        public static List<Entity> entities;
        public Review2(IEnumerable<HtmlNode> lV, List<Entity> e)
        {
            locationsToVisit = lV;
            entities = e;
        }
        public async Task StartAsync(IDialogContext context)
        {
            response = context.MakeMessage();
            response.InputHint = InputHints.ExpectingInput;
            response.Speak = response.Text = "What do you want the note to say?";
            await context.PostAsync(response);
            context.Wait(CreateNote);
        }


        public async Task CreateNote(IDialogContext context, IAwaitable<IMessageActivity> argument)
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
                    await this.PostTask(context);
                    break;
                default:
                    response.Speak = response.Text = "I did not get that. Review " + getIntent.Result.topScoringIntent.intent;
                    await context.PostAsync(response);
                    context.Wait(CreateNote);
                    break;

            }
            
        }
        public async Task PostTask(IDialogContext context)
        {
            response.Speak = response.Text = "Done!";
            response.InputHint = InputHints.IgnoringInput;
            await context.PostAsync(response);
            context.Done<object>(new object());
        }
    }
}