using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Newcatchy
{
    [Serializable]
    [LuisModel("fc5a91ba-2fe7-41ec-968b-7c4b7809e91c", "79b44584a48c4a52be74d52000841911")]
    public class Luis : LuisDialog<object>
    {
        public string message;
        public static int savings = 0;
        public static string username = "";
        public int mon_savings = 0, mon_exp = 0;

        public Luis(string msg)
        {
            username = msg;
        }
        public int Savings()
        {
            Models.DatabaseEntities db = new Models.DatabaseEntities();
            Models.save_expenditure se = new Models.save_expenditure();
            Database db_cls = new Database(username);
            int bal = 0;
            int pre_month = DateTime.UtcNow.Month;
            mon_savings = db_cls.Income(pre_month);
            mon_exp = db_cls.Total_month(pre_month);
            savings = (mon_savings - mon_exp);
            se.savings = savings;
            /*se.UserName = username;
            se.Date = DateTime.UtcNow;
            var save = (from save_expenditure in db.save_expenditure where save_expenditure.UserName == username
                       select save_expenditure.savings);
            foreach (var sc in save)
            {
                bal = int.Parse(sc.ToString());
            }
            savings = savings + bal;*/
            return savings;
        }

        public int overall_amt()
        {
            int total = 0;
            Database db_cls = new Database(username);
            int[] ovr = new int[] { db_cls.total_amtFood(), db_cls.total_amtEducation(), db_cls.total_amtHealth(), db_cls.total_amtTravel(), db_cls.total_amtOthers() };
            for (int i = 0; i < ovr.Length; i++)
            {
                total += ovr[i];
            }
            return total;
        }

        [LuisIntent("Greetings")]
        public async Task GreetingsIntent(IDialogContext context, LuisResult result)
        {

            PromptDialog.Confirm(context, ConfirmationAboutPrompt, $"" + message + "  Do you want to know about me?");

        }
        private async Task ConfirmationAboutPrompt(IDialogContext context, IAwaitable<bool> result)
        {
            if (await result)
            {
                await context.PostAsync($"I help you in having an eye on your expenses." + Environment.NewLine + Environment.NewLine +
                   "by taking your expenditure and categorising" + Environment.NewLine + Environment.NewLine + "I suggest you where to reduce your expenses to increase savings /U0001F604 " + Environment.NewLine + Environment.NewLine +
                   "So please give me your income");
                await context.PostAsync($"Please enter your monthly income....");
            }
            else
            {
                await context.PostAsync($"Hope you already heard about me" + Environment.NewLine + Environment.NewLine +
                    "If not, you can know about me any time when you are free");
                await context.PostAsync($"Please enter your monthly income......");
            }
        }

        [LuisIntent("Incomesuggestion")]
        public async Task Incomesuggestin(IDialogContext context, LuisResult result)
        {
            DateTime dt = DateTime.UtcNow;
            int mon = dt.Month;
            Database db_cls = new Database(username);
            //int month =Total_month(6);
            Savings();
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
                await context.PostAsync($"**I would like to suggest you according to your expenditure**" + Environment.NewLine + Environment.NewLine +
                    "amount you spent in this month is" + month + Environment.NewLine + Environment.NewLine + "your spending wisely \U0001F44D" + Environment.NewLine + Environment.NewLine +
                    "If you spend like this you can have a lot of savings \U0001F601 Your expenses in these categories are low,you can increase in" + "" + s[0] + "and" + s[1]);

            }
            else if (month >= savings / 2 && month <= (savings * 3) / 4)
            {
                await context.PostAsync($"**I would like to suggest you according to expenditure**" + Environment.NewLine + Environment.NewLine +
                    "amount you spent in this month is {month}" + Environment.NewLine + Environment.NewLine + "your spending well but be careful coz ur expenses are almost close to your earnings /U0001F60A" + Environment.NewLine + Environment.NewLine +
                    "I suggest you to reduce your expenses in catgories like " + " " + s[4] + "and" + " " + s[3]);

            }
            else
            {
                await context.PostAsync($"**I would like to suggest you according to your expenditure**" +
                    Environment.NewLine + Environment.NewLine + "amount you spent in this month is" + month +
                    Environment.NewLine + Environment.NewLine + "your not spending wisely" + Environment.NewLine + Environment.NewLine +
                    "If you spend like this you cant have any savings \U0001F610 " + Environment.NewLine + Environment.NewLine + "");

            }
            context.Wait(MessageReceived);
        }

        [LuisIntent("Month suggestion")]
        public async Task MonthIntent(IDialogContext context, LuisResult result)
        {
            Database db_cls = new Database(username);
            Dictionary<string, int> months = new Dictionary<string, int>();
            months.Add("first", 1);
            months.Add("second", 2);
            months.Add("thrid", 3);
            months.Add("fourth", 4);
            months.Add("fifth", 5);
            months.Add("sixth", 6);
            months.Add("seventh", 7);
            months.Add("eighth", 8);
            months.Add("ninth", 9);
            months.Add("tenth", 10);
            months.Add("eleventh", 11);
            months.Add("twelfth", 12);
            months.Add("twelth", 12);
            string mnth = "";
            string numberPart = "";
            int monthvalue = 0;
            int month_total = 0;

            EntityRecommendation rec;
            if (result.TryFindEntity("builtin.number", out rec))
            {
                mnth = rec.Entity;
                if (int.TryParse(mnth, out monthvalue))
                {
                    //incs.Id = 3;
                    DateTime presentdate = DateTime.UtcNow;
                    int Mm = presentdate.Month;
                    await context.PostAsync($"" + mnth);
                    while (monthvalue > 0)
                    {

                        month_total += db_cls.Total_month(Mm);
                        await context.PostAsync($"" + month_total);
                        await context.PostAsync($"" + Mm);
                        await context.PostAsync($"" + monthvalue);
                        Mm--;
                        monthvalue--;

                    }
                    await context.PostAsync($"" + month_total);
                }
            }
            else if (result.TryFindEntity("builtin.ordinal", out rec))
            {
                mnth = rec.Entity;
                if (months.ContainsKey(mnth.ToLower()))
                {
                    monthvalue = months[mnth.ToLower()];
                    month_total = db_cls.Total_month(monthvalue);
                    await context.PostAsync(monthvalue.ToString());
                }
                else
                {
                    var re = new Regex("(?<Numeric>[0-9]*)(?<Alpha>[a-zA-Z]*)");
                    Match resu = re.Match(mnth);
                    numberPart = resu.Groups[1].ToString();
                    string alphaPart = resu.Groups[2].ToString();
                    await context.PostAsync(numberPart);
                    monthvalue = int.Parse(numberPart);
                    month_total = db_cls.Total_month(monthvalue);
                }
                await context.PostAsync($"" + mnth);
            }
            await context.PostAsync($"amount spent in {mnth} months is {month_total}");
            context.Wait(MessageReceived);
        }

        [LuisIntent("About")]
        public async Task AboutIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"I help you in having an eye on your expenses." + Environment.NewLine + Environment.NewLine +
                    "by taking your expenditure and categorising" + Environment.NewLine + Environment.NewLine + "I suggest you where to reduce your expenses to increase savings /U0001F604 " + Environment.NewLine + Environment.NewLine +
                    "So please give me your income");
        }

        [LuisIntent("Category Wise")]
        public async Task CategoryNull(IDialogContext context, LuisResult result)
        {
            Database db_cls = new Database(username);
            int[] t_amt = new int[] { db_cls.total_amtFood(), db_cls.total_amtEducation(), db_cls.total_amtHealth(), db_cls.total_amtTravel(), db_cls.total_amtOthers() };
            string[] arr = new string[] { "Food", "Education", "Health", "Travel", "Others" };
            string Zero = "hai";
            for (int i = 0; i < t_amt.Length; i++)
            {
                if (t_amt[i].Equals(0))
                {
                    Zero = arr[i];
                }
            }
            await context.PostAsync($"the category in which you havent spent amount is " + Zero);
            context.Wait(MessageReceived);
        }
        [LuisIntent("More Expense")]
        public async Task Highestamount(IDialogContext context, LuisResult result)
        {
            Database db_cls = new Database(username);
            int[] high_amt = new int[] { db_cls.total_amtFood(), db_cls.total_amtEducation(), db_cls.total_amtHealth(), db_cls.total_amtTravel(), db_cls.total_amtOthers() };
            string[] arr = new string[] { "Food", "Education", "Health", "Travel", "Others" };
            int high = high_amt[0];
            string cat = arr[0];
            for (int i = 1; i < high_amt.Length; i++)
            {
                if (high < high_amt[i])
                {
                    cat = arr[i];
                }
            }
            await context.PostAsync($"the category in which you spent more amount is \U0001F62E {cat}");
            context.Wait(MessageReceived);
        }

        [LuisIntent("AmountSpent")]
        public async Task AmountSpentIntent(IDialogContext context, LuisResult result)
        {
            string catg = "";
            Database db_cls = new Database(username);
            EntityRecommendation rec;
            if (result.TryFindEntity("category", out rec))
            {
                catg = rec.Entity;
                await context.PostAsync($"" + catg);
            }
            await context.PostAsync($"user name is" + username);
            if (catg.ToLower() == "food")
            {
                int fd = db_cls.total_amtFood();
                await context.PostAsync($"the amount you spent in {catg} is \U0001F354 \U0001F356 \U0001F373" + fd);
            }
            else if (catg.ToLower() == "education")
            {
                int ed = db_cls.total_amtEducation();
                await context.PostAsync($"the amount you spent in {catg} is \U0001F468 \U0001F4D6 \U0001F469 \U0001F4D2" + ed);
            }
            else if (catg.ToLower() == "health")
            {
                int hth = db_cls.total_amtHealth();
                await context.PostAsync($"the amount you spent in {catg} is \U0001F468 \U0001F469 \U0001F34c" + hth);
            }
            else if (catg.ToLower() == "travel")
            {
                int tr = db_cls.total_amtTravel();
                await context.PostAsync($"the amount you spent in {catg} is \U0001F686 \U0001F680 \U0001F682" + tr);
            }
            else
            {
                int ot = db_cls.total_amtOthers();
                await context.PostAsync($"the amount you spent in others is \U0001F453 \U0001F460 \U0001F455" + ot);
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("Income")]
        public async Task IncomeIntent(IDialogContext context, LuisResult result)
        {

            string inc = "";
            EntityRecommendation rec;
            Models.DatabaseEntities db = new Models.DatabaseEntities();
            if (result.TryFindEntity("builtin.number", out rec))
            {
                int numb = 0;
                //double num = parseNumber(res);
                inc = rec.Entity;
                string income = inc;
                var re = new Regex("(?<Numeric>[0-9]*)(?<Alpha>[a-zA-Z]*)");
                Match resu = re.Match(income);
                string numberPart = resu.Groups[1].ToString();
                string alphaPart = resu.Groups[2].ToString();
                Models.inc_store incs = new Models.inc_store();
                if (int.TryParse(inc, out numb))
                {
                    //incs.Id = 3;
                    incs.income = numb;
                    incs.Date = DateTime.UtcNow;
                    incs.UserName = username;
                    db.inc_store.Add(incs);
                    db.SaveChanges();
                    await context.PostAsync($"The income is added to your previous income");
                }
                else if (int.TryParse(numberPart, out numb))
                {
                    incs.income = numb * 1000;
                    incs.Date = DateTime.UtcNow;
                    incs.UserName = username;
                    db.inc_store.Add(incs);
                    db.SaveChanges();
                    await context.PostAsync($"The income is added to your previous income");
                }
            }
            else
            {
                await context.PostAsync($"You have not entered your income");
                context.Wait(MessageReceived);
            }
            string[] category = new string[] { "Food", "Education", "Health", "Travel", "Others" };
            List<CardAction> cardButtons = new List<CardAction>();
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
            Activity replyToConversation = (Activity)context.MakeMessage();
            replyToConversation.Recipient = replyToConversation.Recipient;
            replyToConversation.Type = "message";

            HeroCard plCard = new HeroCard()
            {
                Title = "enter your expenses according to these categories",
                Buttons = cardButtons,
                Text = "click on the below buttons to enter expenses"
            };
            Attachment plAttachment = plCard.ToAttachment();
            replyToConversation.Attachments.Add(plAttachment);
            replyToConversation.AttachmentLayout = "list";
            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
         }

        public List<CardAction> button()
        {
            string[] category = new string[] { "Food", "Education", "Health", "Travel", "Others" };
            List<CardAction> cardButtons = new List<CardAction>();
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
            return cardButtons;
        }

        [LuisIntent("Button")]
        public async Task buttonintent(IDialogContext context, LuisResult result)
        {
            //await context.PostAsync("you can enter your expenditure according to these categories ");
            List<CardAction> cardButtons = new List<CardAction>();
            cardButtons = button();
            Activity replyToConversation = (Activity)context.MakeMessage();
            replyToConversation.Recipient = replyToConversation.Recipient;
            replyToConversation.Type = "message";

            HeroCard plCard = new HeroCard()
            {
                Title = "enter your expenses according to these categories",
                Buttons = cardButtons,
                Text = "click on the below buttons to enter expenses"
            };
            Attachment plAttachment = plCard.ToAttachment();
            replyToConversation.Attachments.Add(plAttachment);
            replyToConversation.AttachmentLayout = "list";
            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        [LuisIntent("FinalSuggestion")]
        public async Task Finalsuggestion(IDialogContext context, LuisResult result)
        {
            Database db_cls = new Database(username);
            int yr_income = db_cls.Income_yr(DateTime.UtcNow.Year);
            Savings();
            await context.PostAsync($"Your annual income is :" + yr_income + Environment.NewLine + Environment.NewLine +
                                     "Your savings are: " + savings);
            int Mon_income = db_cls.Income(DateTime.UtcNow.Month);
            int Food_Month = db_cls.month_food(DateTime.UtcNow.Month);
            int Education_Month = db_cls.month_Education(DateTime.UtcNow.Month);
            int Health_month = db_cls.month_Health(DateTime.UtcNow.Month);
            int Travel_month = db_cls.month_Travel(DateTime.UtcNow.Month);
            int Others_Month = db_cls.month_Others(DateTime.UtcNow.Month);
            await context.PostAsync($"**your monthly expenditure is**" + Environment.NewLine + Environment.NewLine + "total income of this month is: " + Mon_income + Environment.NewLine + Environment.NewLine +
                                      "Amount spent in Food in this month is: " + Food_Month + Environment.NewLine + Environment.NewLine +
                                      "Amount spent in Education in this month is: " + Education_Month + Environment.NewLine + Environment.NewLine +
                                       "Amount spent in Health in this month is: " + Health_month + Environment.NewLine + Environment.NewLine +
                                       "Amount spent in Travel in this month is: " + Travel_month + Environment.NewLine + Environment.NewLine +
                                       "Amount spent in Others in this month is: " + Others_Month);
            int tmp;
            string tmp1;
            int month_spent = db_cls.Total_month(DateTime.UtcNow.Month);
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
            if (month_spent <= savings / 2)
            {
                await context.PostAsync($"**I would like to suggest you according to your expenditure**" + Environment.NewLine + Environment.NewLine +
                    "amount you spent in this month is" + month_spent + Environment.NewLine + Environment.NewLine + "your spending wisely \U0001F44D" + Environment.NewLine + Environment.NewLine +
                    "If you spend like this you can have a lot of savings \U0001F601 Your expenses in these categories are low,you can increase in" + "" + s[0] + "and" + s[1]);

            }
            else if (month_spent >= savings / 2 && month_spent <= (savings * 3) / 4)
            {
                await context.PostAsync($"**I would like to suggest you according to expenditure**" + Environment.NewLine + Environment.NewLine +
                    "amount you spent in this month is {month}" + Environment.NewLine + Environment.NewLine + "your spending well but be careful coz ur expenses are almost close to your earnings /U0001F60A" + Environment.NewLine + Environment.NewLine +
                    "I suggest you to reduce your expenses in catgories like " + " " + s[4] + "and" + " " + s[3]);

            }
            else
            {
                await context.PostAsync($"**I would like to suggest you according to your expenditure**" +
                    Environment.NewLine + Environment.NewLine + "amount you spent in this month is" + month_spent +
                    Environment.NewLine + Environment.NewLine + "your not spending wisely" + Environment.NewLine + Environment.NewLine +
                    "If you spend like this you cant have any savings \U0001F610 " + Environment.NewLine + Environment.NewLine + "");

            }
            context.Wait(MessageReceived);

        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"invalid entry");
            context.Wait(MessageReceived);
        }
    }
}