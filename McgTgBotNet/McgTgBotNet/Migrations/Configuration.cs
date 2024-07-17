using McgTgBotNet.Models;
using System.Data.Entity.Migrations;

namespace McgTgBotNet.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<McgBotContext>
    {
        public Configuration() => AutomaticMigrationsEnabled = false;

        protected override void Seed(McgBotContext context) { }
    }
}
