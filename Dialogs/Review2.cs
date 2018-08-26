using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;


namespace dialogs_basic
{
    [Serializable]
    public class Review2 : IDialog<object>
    {
        public static string note;
        static IMessageActivity response;
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
            response.Speak = response.Text = message.Text;
            await context.PostAsync(response);
            await this.PostTask(context);
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