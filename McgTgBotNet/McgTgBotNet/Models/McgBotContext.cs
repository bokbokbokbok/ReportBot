using McgTgBotNet.DB.Entities;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace McgTgBotNet.Models
{
    public partial class McgBotContext : DbContext
    {
        public McgBotContext() : base("name=ReportBotDb") { }
        
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
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
