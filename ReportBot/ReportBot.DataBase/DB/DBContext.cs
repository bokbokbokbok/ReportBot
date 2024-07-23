using McgTgBotNet.Models;
using Telegram.Bot.Types;

namespace McgTgBot.DB
{
    public static class DBContext
    {
        public static bool CreateReport (Message message)
        {
            var report = new Report
            {
                Message = message.Text,
                Created = DateTime.Now,
                ChatId = (int)message.Chat.Id,
                UserName = message.From.Username,
                UserId = (int)message.From.Id
            };

            //using (var db = new ApplicationDbContext())
            //{
            //    db.Report.Add(report);
            //    db.SaveChanges();
            //}

            return true;
        }

        public static ReportUser CreateUser(Message message, string customName = "")
        {
            ReportUser user = new ReportUser
            {
                Created = DateTime.Now,
                UserName = customName.Length > 0 ? customName : message.From.Username,
                UserId = (int)message.From.Id
            };

            //using (var db = new ApplicationDbContext())
            //{
            //    user = db.ReportUser.Add(user);
            //    db.SaveChanges();
            //}

            return user;
        }

        public static ReportUser UpdateUser(ReportUser user)
        {
            //using (var db = new ApplicationDbContext())
            //{
            //    db.ReportUser.AddOrUpdate(user);
            //    db.SaveChanges();
            //}

            return user;
        }

        public static List<Report> GetReports(DateTime from, DateTime to)
        {
            var list = new List<Report>();

            //using (var db = new ApplicationDbContext())
            //{
            //    list = db.Report.Where(p => p.Created > from && p.Created < to).ToList();
            //}

            return list;
        }

        public static List<ReportUser> GetReportsUsers()
        {
            var list = new List<ReportUser>();

            //using (var db = new ApplicationDbContext())
            //{
            //    list = db.ReportUser.ToList();
            //}

            return list;
        }

        public static ReportUser GetReportsUser(int externalId)
        {
            ReportUser item = null!;
            //using (var db = new ApplicationDbContext())
            //{
            //    item = db.ReportUser.FirstOrDefault(p => p.UserId == externalId);
            //}

            return item;
        }
    }
}
