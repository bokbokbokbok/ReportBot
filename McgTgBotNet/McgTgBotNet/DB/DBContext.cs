using System;
using System.Linq;
using System.Collections.Generic;
using McgTgBotNet.Models;
using Telegram.Bot.Types;
using System.Data.Entity.Migrations;
using Microsoft.EntityFrameworkCore;

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
                var entity = db.Users.FirstOrDefault(p => p.WorksnapsId == user.WorksnapsId);

                if(entity != null)
                    throw new Exception($"👋 Hi {user.Username}, you are already registered");

                db.Users.Add(user);
                db.SaveChanges();
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

        public static McgTgBotNet.DB.Entities.User GetUserWithProject(int worksnapsId)
        {
            McgTgBotNet.DB.Entities.User item;
            using (var db = new McgBotContext())
            {
                item = db.Users.FirstOrDefault(p => p.WorksnapsId == worksnapsId);
                item.Projects = db.Projects.Where(p => p.Users.Any(x => x.Id == item.Id)).ToList();
            }

            return item;
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

        public static Project InsertProject(Project project)
        {
            using (var db = new McgBotContext())
            {
                db.Projects.Add(project);
                db.SaveChanges();
            }

            return project;
        }

        public static List<Project> InsertProjectMany(List<Project> projects)
        {
            using (var db = new McgBotContext())
            {
                foreach (var project in projects)
                {
                    if (db.Projects.FirstOrDefault(x => x.Name == project.Name) == null)
                    {
                        db.Projects.Add(project);
                    }
                }
                db.SaveChanges();
            }

            return projects;
        }

        public static bool AddUserToProjects(int userId, List<string> projectNames)
        {
            using (var db = new McgBotContext())
            {
                var user = db.Users.FirstOrDefault(p => p.WorksnapsId == userId);
                var projects = new List<Project>();

                foreach (var projectName in projectNames)
                {
                    var project = db.Projects.FirstOrDefault(p => p.Name == projectName);
                    if (project != null)
                        projects.Add(project);
                }

                if (user == null)
                    return false;

                user.Projects.AddRange(projects);
                db.Users.AddOrUpdate(user);
                db.SaveChanges();
            }

            return true;
        }
    }
}
