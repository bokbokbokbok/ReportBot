using System;
using System.CodeDom;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Xml;
using Telegram.Bot.Types;

namespace McgTgBotNet.Models
{
    public partial class McgBotContext : DbContext
    {
        public McgBotContext()
            : base("name=ReportBotDb")
        {
        }
        
        public virtual DbSet<Report> Report { get; set; }
        public virtual DbSet<ReportUser> ReportUser { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<McgBotContext>(null);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
