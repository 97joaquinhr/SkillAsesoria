using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using Microsoft.Bot.Sample.SimpleEchoBot;

namespace dialogs_basic
{
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        public static string location;
        protected int count = 1;
        public EchoDialog(string jsonLocation)
        {
            TimeZoneInfo targetZone = TimeZoneInfo.FindSystemTimeZoneById(MessagesController.timeZone);

            // Get local machine's value of Now
            DateTime utcDatetime = DateTime.UtcNow;

            // Get Central Standard Time value of Now
            DateTime userDatetime = TimeZoneInfo.ConvertTimeFromUtc(utcDatetime, targetZone);

            // Find timezoneOffset
            LUISAPI.timezoneOffset = (int)((userDatetime - utcDatetime).TotalMinutes);
            LUISAPI.Date = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/9190d074-1220-4156-95f2-b443bfd406a4?subscription-key=5e7fa924eefb4a619812fefca88e73cd&verbose=true&timezoneOffset=" + LUISAPI.timezoneOffset + "&q=";
        }
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            if (message.Text == "reset")
            {
                PromptDialog.Confirm(
                    context,
                    AfterResetAsync,
                    "Are you sure you want to reset the count?",
                    "Didn't get that!",
                    promptStyle: PromptStyle.Auto);
            }
            else
            {
                await context.PostAsync($"{this.count++}: You said vazquez {message.Text}. Location: "+location);
                context.Wait(MessageReceivedAsync);
            }
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }

    }
}