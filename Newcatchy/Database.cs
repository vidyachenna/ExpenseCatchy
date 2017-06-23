using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Newcatchy
{
    public class Database
    {
        public static string username = "";
        public Database(string msg)
        {
            username = msg;
        }
        public int Income(int month)
        {
            Models.DatabaseEntities db = new Models.DatabaseEntities();
            int mnth_income = 0;
            var inc = (from inc_store in db.inc_store
                       where inc_store.Date.Month == DateTime.UtcNow.Month && inc_store.UserName == username
                       select inc_store.income);
            foreach (var val in inc)
            {
                int sc = int.Parse(val.ToString());
                mnth_income += sc;
            }
            //savings = mnth_income;
            return mnth_income;
        }

        public int Income_yr(int year)
        {
            Models.DatabaseEntities db = new Models.DatabaseEntities();
            int Yr_income = 0;
            var inc = (from inc_store in db.inc_store
                       where inc_store.Date.Year == DateTime.UtcNow.Year && inc_store.UserName == username
                       select inc_store.income);
            foreach (var val in inc)
            {
                int sc = int.Parse(val.ToString());
                Yr_income += sc;
            }
            //savings = mnth_income;
            return Yr_income;
        }

        public int month_food(int mnth)
        {
            Models.DatabaseEntities db = new Models.DatabaseEntities();
            var dd = (from Category in db.Categories select Category.Date);
            int total = 0;
            var amt = (from Category in db.Categories where Category.Date.Month == mnth && Category.UserName == username select Category.Food);
            foreach (var fd in amt)
            {
                int sc = int.Parse(fd.ToString());
                total += sc;
            }

            return total;
        }
        public int month_Travel(int mnth)
        {
            Models.DatabaseEntities db = new Models.DatabaseEntities();
            var dd = (from Category in db.Categories select Category.Date);
            int total = 0;
            var amt = (from Category in db.Categories where Category.Date.Month == mnth && Category.UserName == username select Category.Travel);
            foreach (var fd in amt)
            {
                int sc = int.Parse(fd.ToString());
                total += sc;
            }

            return total;
        }
        public int month_Education(int mnth)
        {
            Models.DatabaseEntities db = new Models.DatabaseEntities();
            var dd = (from Category in db.Categories select Category.Date);
            int total = 0;
            var amt = (from Category in db.Categories where Category.Date.Month == mnth && Category.UserName == username select Category.Education);
            foreach (var fd in amt)
            {
                int sc = int.Parse(fd.ToString());
                total += sc;
            }
            return total;
        }
        public int month_Health(int mnth)

        {
            Models.DatabaseEntities db = new Models.DatabaseEntities();
            var dd = (from Category in db.Categories select Category.Date);
            int total = 0;
            var amt = (from Category in db.Categories where Category.Date.Month == mnth && Category.UserName == username select Category.Health);
            foreach (var fd in amt)
            {
                int sc = int.Parse(fd.ToString());
                total += sc;
            }

            return total;
        }

        public int month_Others(int mnth)
        {
            Models.DatabaseEntities db = new Models.DatabaseEntities();
            var dd = (from Category in db.Categories select Category.Date);
            int total = 0;
            var amt = (from Category in db.Categories where Category.Date.Month == mnth && Category.UserName == username select Category.Others);
            foreach (var fd in amt)
            {
                int sc = int.Parse(fd.ToString());
                total += sc;
            }

            return total;
        }

        public int Total_month(int monthvalue)
        {
            int total = 0;
            int num1, num2, num3, num4, num5 = 0;
            num1 = month_food(monthvalue);
            num2 = month_Travel(monthvalue);
            num3 = month_Education(monthvalue);
            num4 = month_Health(monthvalue);
            num5 = month_Others(monthvalue);
            total = num1 + num2 + num3 + num4 + num5;
            return total;
        }

        public int total_amtFood()
        {
            Models.DatabaseEntities db = new Models.DatabaseEntities();
            int total = 0;
            var amt = (from Category in db.Categories
                       where Category.UserName == username
                       select Category.Food)
                            .ToList();

            foreach (var score in amt)
            {
                int sc = int.Parse(score.ToString());
                total += sc;
            }
            return total;
        }
        public int total_amtEducation()
        {
            Models.DatabaseEntities db = new Models.DatabaseEntities();
            int total = 0;
            var amt = (from Category in db.Categories
                       where Category.UserName == username
                       select Category.Education)
                            .ToList();

            foreach (var score in amt)
            {
                int sc = int.Parse(score.ToString());
                total += sc;
            }
            return total;
        }
        public int total_amtHealth()
        {
            Models.DatabaseEntities db = new Models.DatabaseEntities();
            int total = 0;
            var amt = (from Category in db.Categories
                       where Category.UserName == username
                       select Category.Health)
                            .ToList();

            foreach (var score in amt)
            {
                // Add the High Score to the response
                int sc = int.Parse(score.ToString());
                total += sc;
            }
            return total;
        }
        public int total_amtTravel()
        {
            Models.DatabaseEntities db = new Models.DatabaseEntities();
            int total = 0;
            var amt = (from Category in db.Categories
                       where Category.UserName == username
                       select Category.Travel)
                            .ToList();

            foreach (var score in amt)
            {
                // Add the High Score to the response
                int sc = int.Parse(score.ToString());
                total += sc;
            }
            return total;
        }
        public int total_amtOthers()
        {
            Models.DatabaseEntities db = new Models.DatabaseEntities();
            int total = 0;
            var amt = (from Category in db.Categories
                       where Category.UserName == username
                       select Category.Others)
                            .ToList();

            foreach (var score in amt)
            {
                // Add the High Score to the response
                int sc = int.Parse(score.ToString());
                total += sc;
            }
            return total;
        }
    }
}