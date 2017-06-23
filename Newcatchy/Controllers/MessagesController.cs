using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;


namespace Newcatchy
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public static string username = "";
        public static int savings = 0;
        public static string cat = "";
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
             if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                string mg = activity.Text;
                int num = 0;
                Activity reply;
                username = activity.From.Name;
                if (mg == "Food" || mg == "Education" || mg == "Health" || mg == "Travel" || mg == "Others")
                {
                    cat = mg;
                    reply = activity.CreateReply($"enter your expenses in " + mg);

                    //await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
                    await connector.Conversations.ReplyToActivityAsync(reply);
                    //await Conversation.SendAsync(activity, () => new Luis(mg));
                    var res = Request.CreateResponse(HttpStatusCode.OK);
                    return res;
                }
                else if (mg == "Categorise")
                {
                    reply = activity.CreateReply($"");
                    Luis ls = new Luis(username);
                    List<CardAction> cardButtons = new List<CardAction>();
                    cardButtons = ls.button();
                    reply.Recipient = reply.Recipient;
                    reply.Type = "message";

                    HeroCard plCard = new HeroCard()
                    {
                        Title = "enter your expenses according to these categories",
                        Buttons = cardButtons,
                        Text = "click on the below buttons to enter expenses"
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    reply.Attachments.Add(plAttachment);
                    reply.AttachmentLayout = "list";
                    await connector.Conversations.ReplyToActivityAsync(reply);
                    var res = Request.CreateResponse(HttpStatusCode.OK);
                    return res;
                }
                else if(mg == "Not categorise")
                {
                    reply = activity.CreateReply($"Now I am having information about your expenditure,I want to suggest you");
                    await connector.Conversations.ReplyToActivityAsync(reply);
                    DateTime dt = DateTime.UtcNow;
                    int mon = dt.Month;
                    Database db_cls = new Database(username);
                    Luis l = new Luis(username);
                    //int month =Total_month(6);
                    savings = l.Savings();
                    int tmp;
                    string tmp1;
                    int month = db_cls.Total_month(mon);
                    int[] q = new int[] { db_cls.total_amtFood(), db_cls.total_amtHealth(), db_cls.total_amtEducation(), db_cls.total_amtTravel(), db_cls.total_amtOthers() };
                    string[] s = new string[] { "Food", "Health", "Education", "Travel", "Others" };

                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 5 - i - 1; j++)
                        {
                            if (q[j] > q[j + 1])
                            {
                                tmp = q[j];
                                tmp1 = s[j];
                                q[j] = q[j + 1];
                                s[j] = s[j + 1];
                                q[j + 1] = tmp;
                                s[j + 1] = tmp1;
                            }
                        }
                    }
                    if (month <= savings / 2)
                    {
                        reply = activity.CreateReply($"**I would like to suggest you according to your expenditure**" + Environment.NewLine + Environment.NewLine +
                            "amount you spent in this month is" + month + Environment.NewLine + Environment.NewLine + "your spending wisely \U0001F44D" + Environment.NewLine + Environment.NewLine +
                            "If you spend like this you can have a lot of savings \U0001F601 Your expenses in these categories are low,you can increase in" + "" + s[0] + "and" + s[1]);

                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }
                    else if (month >= savings / 2 && month <= (savings * 3) / 4)
                    {
                        reply = activity.CreateReply($"**I would like to suggest you according to expenditure**" + Environment.NewLine + Environment.NewLine +
                            "amount you spent in this month is {month}" + Environment.NewLine + Environment.NewLine + "your spending well but be careful coz ur expenses are almost close to your earnings /U0001F60A" + Environment.NewLine + Environment.NewLine +
                            "I suggest you to reduce your expenses in catgories like " + " " + s[4] + "and" + " " + s[3]);
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }
                    else
                    {
                        reply = activity.CreateReply($"**I would like to suggest you according to your expenditure**" +
                            Environment.NewLine + Environment.NewLine + "amount you spent in this month is" + month +
                            Environment.NewLine + Environment.NewLine + "your not spending wisely" + Environment.NewLine + Environment.NewLine +
                            "If you spend like this you cant have any savings \U0001F610 " + Environment.NewLine + Environment.NewLine + "");
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }
                    reply = activity.CreateReply($"If you need some more suggestions you can ask queries");
                    await connector.Conversations.ReplyToActivityAsync(reply);
                    var res = Request.CreateResponse(HttpStatusCode.OK);
                    return res;
                }

                else if (cat == "Food" && int.TryParse(mg, out num))
                {
                    Models.DatabaseEntities db = new Models.DatabaseEntities();
                    Models.Category ctg = new Models.Category();
                    //num = Convert.ToInt32(mg);

                    ctg.Date = DateTime.UtcNow;
                    ctg.Food = num;
                    ctg.UserName = username;
                    db.Categories.Add(ctg);
                    db.SaveChanges();
                    reply = activity.CreateReply($"your expenses {mg} in {cat} are taken.");
                    await connector.Conversations.ReplyToActivityAsync(reply);
                    //await Conversation.SendAsync(activity, () => new Prompt());
                    Database db_cls = new Database(username);
                    int inc = db_cls.Income(DateTime.UtcNow.Month);
                    int ed = db_cls.month_Education(DateTime.UtcNow.Month);
                    if (ed > inc / 5)
                    {
                        reply = activity.CreateReply($"the amount you are spending on {cat} is increasing" + Environment.NewLine + Environment.NewLine +
                            "It is about " + ed + ", try to reduce");
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }
                    reply = activity.CreateReply($"");
                    Luis ls = new Luis(username);
                    List<CardAction> cardButtons = new List<CardAction>();
                    string[] category = new string[] { "Categorise", "Not categorise" };
                    for (int i = 0; i < category.Length; i++)
                    {
                        string CurrentNumber = Convert.ToString(i);
                        CardAction CardButton = new CardAction()
                        {
                            Type = "imBack",
                            Title = category[i],
                            Value = category[i]
                        };
                        cardButtons.Add(CardButton);
                    }
                    reply.Recipient = reply.Recipient;
                    reply.Type = "message";

                    HeroCard plCard = new HeroCard()
                    {
                        Title = "Do you still want to categorise ?",
                        Buttons = cardButtons,
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    reply.Attachments.Add(plAttachment);
                    reply.AttachmentLayout = "list";
                    //await context.PostAsync(replyToConversation);
                    //context.Wait(MessageReceived);
                    await connector.Conversations.ReplyToActivityAsync(reply);
                    var res = Request.CreateResponse(HttpStatusCode.OK);
                    return res;
                }

                else if (cat == "Education" && int.TryParse(mg, out num))
                {
                    Models.DatabaseEntities db = new Models.DatabaseEntities();
                    Models.Category ctg = new Models.Category();
                    //num = Convert.ToInt32(mg);

                    ctg.Date = DateTime.UtcNow;
                    ctg.Education = num;
                    ctg.UserName = username;
                    db.Categories.Add(ctg);
                    db.SaveChanges();

                    reply = activity.CreateReply($"your expenses {mg} in {cat} are taken.");
                    await connector.Conversations.ReplyToActivityAsync(reply);
                    Database db_cls = new Database(username);
                    int inc = db_cls.Income(DateTime.UtcNow.Month);
                    int ed = db_cls.month_Education(DateTime.UtcNow.Month);
                    if (ed > inc / 5)
                    {
                        reply = activity.CreateReply($"the amount you are spending on {cat} is increasing" + Environment.NewLine + Environment.NewLine +
                            "It is about " + ed + ", try to reduce");
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }
                    reply = activity.CreateReply($"");
                    Luis ls = new Luis(username);
                    List<CardAction> cardButtons = new List<CardAction>();
                    string[] category = new string[] { "Categorise", "Not categorise" };
                    for (int i = 0; i < category.Length; i++)
                    {
                        string CurrentNumber = Convert.ToString(i);
                        CardAction CardButton = new CardAction()
                        {
                            Type = "imBack",
                            Title = category[i],
                            Value = category[i]
                        };
                        cardButtons.Add(CardButton);
                    }
                    reply.Recipient = reply.Recipient;
                    reply.Type = "message";

                    HeroCard plCard = new HeroCard()
                    {
                        Title = "Do you still want to categorise ?",
                        Buttons = cardButtons,
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    reply.Attachments.Add(plAttachment);
                    reply.AttachmentLayout = "list";
                    //await context.PostAsync(replyToConversation);
                    //context.Wait(MessageReceived);
                    await connector.Conversations.ReplyToActivityAsync(reply);
                    var res = Request.CreateResponse(HttpStatusCode.OK);
                    return res;
                }

                else if (cat == "Health" && int.TryParse(mg, out num))
                {
                    Models.DatabaseEntities db = new Models.DatabaseEntities();
                    Models.Category ctg = new Models.Category();
                    Models.inc_store incs = new Models.inc_store();
                    //num = Convert.ToInt32(mg);
                    ctg.Date = DateTime.UtcNow;
                    ctg.Health = num;
                    ctg.UserName = username;
                    db.Categories.Add(ctg);
                    db.SaveChanges();
                    reply = activity.CreateReply($"your expenses {mg} in {cat} are taken.");
                    await connector.Conversations.ReplyToActivityAsync(reply);
                    Database db_cls = new Database(username);
                    int inc = db_cls.Income(DateTime.UtcNow.Month);
                    int ht = db_cls.month_Health(DateTime.UtcNow.Month);
                    if (ht > inc / 5)
                    {
                        reply = activity.CreateReply($"the amount you are spending on {cat} is increasing" + Environment.NewLine + Environment.NewLine +
                            "It is about " + ht + ",try to reduce");
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }
                    reply = activity.CreateReply($"");
                    Luis ls = new Luis(username);
                    List<CardAction> cardButtons = new List<CardAction>();
                    string[] category = new string[] { "Categorise", "Not categorise" };
                    for (int i = 0; i < category.Length; i++)
                    {
                        string CurrentNumber = Convert.ToString(i);
                        CardAction CardButton = new CardAction()
                        {
                            Type = "imBack",
                            Title = category[i],
                            Value = category[i]
                        };
                        cardButtons.Add(CardButton);
                    }
                    reply.Recipient = reply.Recipient;
                    reply.Type = "message";

                    HeroCard plCard = new HeroCard()
                    {
                        Title = "Do you still want to categorise ?",
                        Buttons = cardButtons,
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    reply.Attachments.Add(plAttachment);
                    reply.AttachmentLayout = "list";
                    //await context.PostAsync(replyToConversation);
                    //context.Wait(MessageReceived);
                    await connector.Conversations.ReplyToActivityAsync(reply);
                    var res = Request.CreateResponse(HttpStatusCode.OK);
                    return res;
                }

                else if (cat == "Travel" && int.TryParse(mg, out num))
                {

                    Models.DatabaseEntities db = new Models.DatabaseEntities();
                    Models.Category ctg = new Models.Category();
                    //num = Convert.ToInt32(mg);
                    ctg.Date = DateTime.UtcNow;
                    ctg.Travel = num;
                    ctg.UserName = username;
                    db.Categories.Add(ctg);
                    db.SaveChanges();
                    reply = activity.CreateReply($"your expenses {mg} in {cat} are taken.");
                    Database db_cls = new Database(username);
                    int inc = db_cls.Income(DateTime.UtcNow.Month);
                    int tr = db_cls.month_Travel(DateTime.UtcNow.Month);
                    if (tr > inc / 5)
                    {
                        reply = activity.CreateReply($"the amount you are spending on {cat} is increasing" + Environment.NewLine + Environment.NewLine +
                            "It is about ," + tr + " try to reduce");
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }
                    reply = activity.CreateReply($"");
                    Luis ls = new Luis(username);
                    List<CardAction> cardButtons = new List<CardAction>();
                    string[] category = new string[] { "Categorise", "Not categorise" };
                    for (int i = 0; i < category.Length; i++)
                    {
                        string CurrentNumber = Convert.ToString(i);
                        CardAction CardButton = new CardAction()
                        {
                            Type = "imBack",
                            Title = category[i],
                            Value = category[i]
                        };
                        cardButtons.Add(CardButton);
                    }
                    reply.Recipient = reply.Recipient;
                    reply.Type = "message";

                    HeroCard plCard = new HeroCard()
                    {
                        Title = "Do you still want to categorise ?",
                        Buttons = cardButtons,
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    reply.Attachments.Add(plAttachment);
                    reply.AttachmentLayout = "list";
                    //await context.PostAsync(replyToConversation);
                    //context.Wait(MessageReceived);
                    await connector.Conversations.ReplyToActivityAsync(reply);
                    var res = Request.CreateResponse(HttpStatusCode.OK);
                    return res;
                }

                else if (cat == "Others" && int.TryParse(mg, out num))
                {
                    Models.DatabaseEntities db = new Models.DatabaseEntities();
                    Models.Category ctg = new Models.Category();
                    //num = Convert.ToInt32(mg);

                    ctg.Date = DateTime.UtcNow;
                    ctg.Others = num;
                    ctg.UserName = username;
                    db.Categories.Add(ctg);
                    db.SaveChanges();

                    reply = activity.CreateReply($"your expenses {mg} in {cat} are taken.");
                    //await Conversation.SendAsync(activity, () => new Prompt());
                    await connector.Conversations.ReplyToActivityAsync(reply);
                    Database db_cls = new Database(username);
                    int inc = db_cls.Income(DateTime.UtcNow.Month);
                    int ot = db_cls.month_Others(DateTime.UtcNow.Month);
                    if (ot > inc / 5)
                    {
                        reply = activity.CreateReply($"the amount you are spending on {cat} is increasing" + Environment.NewLine + Environment.NewLine +
                            "It is about ," + ot + " try to reduce");
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }
                    reply = activity.CreateReply($"");
                    Luis ls = new Luis(username);
                    List<CardAction> cardButtons = new List<CardAction>();
                    string[] category = new string[] { "Categorise", "Not categorise" };
                    for (int i = 0; i < category.Length; i++)
                    {
                        string CurrentNumber = Convert.ToString(i);
                        CardAction CardButton = new CardAction()
                        {
                            Type = "imBack",
                            Title = category[i],
                            Value = category[i]
                        };
                        cardButtons.Add(CardButton);
                    }
                    reply.Recipient = reply.Recipient;
                    reply.Type = "message";

                    HeroCard plCard = new HeroCard()
                    {
                        Title = "Do you still want to categorise ?",
                        Buttons = cardButtons,
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    reply.Attachments.Add(plAttachment);
                    reply.AttachmentLayout = "list";
                    //await context.PostAsync(replyToConversation);
                    //context.Wait(MessageReceived);
                    await connector.Conversations.ReplyToActivityAsync(reply);
                    var res = Request.CreateResponse(HttpStatusCode.OK);
                    return res;
                }
                if (cat == "Yes" && int.TryParse(mg, out num))
                {

                    Models.DatabaseEntities db = new Models.DatabaseEntities();
                    Models.inc_store incs = new Models.inc_store();
                    incs.income = num;
                    incs.Date = DateTime.UtcNow;
                    incs.UserName = username;
                    db.inc_store.Add(incs);
                    db.SaveChanges();
                    reply = activity.CreateReply($"The income is added to your previous income");
                    await connector.Conversations.ReplyToActivityAsync(reply);
                    List<CardAction> cardButtons = new List<CardAction>();
                    Luis ls = new Luis(username);
                    cardButtons = ls.button();
                    // Activity replyToConversation = (Activity)context.MakeMessage();
                    reply.Recipient = reply.Recipient;
                    reply.Type = "message";

                    HeroCard plCard = new HeroCard()
                    {
                        Title = "enter your expenses according to these categories",
                        Buttons = cardButtons,
                        Text = "click on the below buttons to enter expenses"
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    reply.Attachments.Add(plAttachment);
                    reply.AttachmentLayout = "list";
                    await connector.Conversations.ReplyToActivityAsync(reply);
                    var res = Request.CreateResponse(HttpStatusCode.OK);
                    return res;

                    //reply = activity.CreateReply($"You have entered wrong income");
                    //await connector.Conversations.ReplyToActivityAsync(reply);
                    //var res = Request.CreateResponse(HttpStatusCode.OK);
                    //return res;

                }
                if (cat == "No" && int.TryParse(mg, out num))
                {
                    Models.DatabaseEntities db = new Models.DatabaseEntities();
                    Models.inc_store incs = new Models.inc_store();

                    incs.income = num;
                    incs.Date = DateTime.UtcNow;
                    incs.UserName = username;
                    db.inc_store.Add(incs);
                    db.SaveChanges();
                    reply = activity.CreateReply($"The income is added to your previous income" + mg);
                    await connector.Conversations.ReplyToActivityAsync(reply);
                    List<CardAction> cardButtons = new List<CardAction>();
                    Luis ls = new Luis(username);
                    cardButtons = ls.button();
                    reply.Recipient = reply.Recipient;
                    reply.Type = "message";

                    HeroCard plCard = new HeroCard()
                    {
                        Title = "enter your expenses according to these categories",
                        Buttons = cardButtons,
                        Text = "click on the below buttons to enter expenses"
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    reply.Attachments.Add(plAttachment);
                    reply.AttachmentLayout = "list";
                    await connector.Conversations.ReplyToActivityAsync(reply);
                    var res = Request.CreateResponse(HttpStatusCode.OK);
                    return res;
                }
                else
                {
                    await Conversation.SendAsync(activity, () => new Luis(username));
                }

                if (mg == "Yes" || mg == "No")
                {
                    cat = mg;
                    reply = activity.CreateReply($"enter your expenses in " + mg);
                    await connector.Conversations.ReplyToActivityAsync(reply);
                    var res = Request.CreateResponse(HttpStatusCode.OK);
                    return res;
                }


            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}