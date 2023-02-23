using System.Net;
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with EchoBot .NET Template version v4.17.1

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.IO;
using System;
using AdaptiveCards;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;

namespace EchoBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var replyText = $"Echo: {turnContext.Activity.Text}";
            await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var option = new SearchOptions()
            {
                QueryType = SearchQueryType.Semantic
            };
            var heroCard = new HeroCard
            {
                Title = "This is a title",
                Text = "This is a text.",
            };
            var imagePath = Path.Combine(Environment.CurrentDirectory, @"img", "sample.jpg");
            var imageData = Convert.ToBase64String(File.ReadAllBytes(imagePath));
            heroCard.Images = new List<CardImage>{
                new CardImage{
                    Url= $"data:image/png;base64,{imageData}"
                }
            };

            var welcomeCard = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {
                Body = new List<AdaptiveElement>{
                    new AdaptiveTextBlock("Hello") { },
                    new AdaptiveChoiceSetInput{
                        Type = AdaptiveChoiceSetInput.TypeName,
                        Id = "MultiSelect",
                        // Value = cardData.MultiSelect,
                        IsMultiSelect = false,
                        Choices = new List<AdaptiveChoice>
                            {
                                new AdaptiveChoice() { Title = "True", Value = "true" },
                                new AdaptiveChoice() { Title = "False", Value = "false" },
                            },
                    }
                },
                Actions = new List<AdaptiveAction>{
                    new AdaptiveSubmitAction
                    {
                        Type = AdaptiveSubmitAction.TypeName,
                        Title = "Submit",
                        Data = new JObject { { "submitLocation", "messagingExtensionFetchTask" } },
                    }
                }
            };

            var openUrlCard = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {
                Body = new List<AdaptiveElement>{
                    new AdaptiveTextBlock("Title") { Size=AdaptiveTextSize.Medium},
                    new AdaptiveTextBlock("one **two** three") { },
                },
                Actions = new List<AdaptiveAction>{
                    new AdaptiveOpenUrlAction
                    {
                        Title="open",
                        Url=new Uri("https://adaptivecards.io/")
                    }
                }
            };

            var showMoreCard = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {
                Body = new List<AdaptiveElement>{
                    new AdaptiveTextBlock("This is text") { },
                },
                Actions = new List<AdaptiveAction>{
                    new AdaptiveShowCardAction
                    {
                        Title="show more",
                        Card=new AdaptiveCard(new AdaptiveSchemaVersion(1, 0)){
                            Body=new List<AdaptiveElement>{
                                new AdaptiveTextBlock("This is text") { },
                                new AdaptiveTextBlock("This is text") { },
                            }
                        }
                    }
                }
            };

            var activity = MessageFactory.Attachment(heroCard.ToAttachment());
            activity.Attachments.Add(new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(welcomeCard.ToJson()),
            });
            activity.Attachments.Add(new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(openUrlCard.ToJson()),
            });
            activity.Attachments.Add(new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(showMoreCard.ToJson()),
            });
            activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(activity, cancellationToken);
                }
            }
        }
    }
}
