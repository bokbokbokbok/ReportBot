using System;
using System.Linq;
using System.Collections.Generic;
using McgTgBotNet.Models;
using Telegram.Bot.Types;
using System.Data.Entity.Migrations;

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

            using (var db = new McgBotContext())
            {
                db.Report.Add(report);
                db.SaveChanges();
            }

            return true;
        }

        public static McgTgBotNet.DB.Entities.User AddUser (McgTgBotNet.DB.Entities.User user)
        {
            using (var db = new McgBotContext())
            {
                if(db.Users.FirstOrDefault(p => p.WorksnapsId == user.WorksnapsId) == null)
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                }
            }

            return user;
        }

        public static bool UpdateUserShiftTime(long chatId, int shiftTime)
        {
            using (var db = new McgBotContext())
            {
                var user = db.Users.FirstOrDefault(p => p.ChatId == chatId);
                user.ShiftTime = shiftTime;
                db.Users.AddOrUpdate(user);
                db.SaveChanges();
            }

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

            using (var db = new McgBotContext())
            {
                user = db.ReportUser.Add(user);
                db.SaveChanges();
            }

            return user;
        }

        public static ReportUser UpdateUser(ReportUser user)
        {
            using (var db = new McgBotContext())
            {
                db.ReportUser.AddOrUpdate(user);
                db.SaveChanges();
            }

            return user;
        }

        public static McgTgBotNet.DB.Entities.User GetUserByWorksnapsId(int worksnapsId)
        {
            McgTgBotNet.DB.Entities.User item;
            using (var db = new McgBotContext())
            {
                item = db.Users.FirstOrDefault(p => p.WorksnapsId == worksnapsId);
            }

            return item;
        }

        public static List<Report> GetReports(DateTime from, DateTime to)
        {
            var list = new List<Report>();

            using (var db = new McgBotContext())
            {
                list = db.Report.Where(p => p.Created > from && p.Created < to).ToList();
            }

            return list;
        }

        public static List<ReportUser> GetReportsUsers()
        {
            var list = new List<ReportUser>();

            using (var db = new McgBotContext())
            {
                list = db.ReportUser.ToList();
            }

            return list;
        }

        public static ReportUser GetReportsUser(int externalId)
        {
            ReportUser item;
            using (var db = new McgBotContext())
            {
                item = db.ReportUser.FirstOrDefault(p => p.UserId == externalId);
            }

            return item;
        }
    }
}
