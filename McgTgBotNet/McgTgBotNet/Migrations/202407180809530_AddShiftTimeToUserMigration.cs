namespace McgTgBotNet.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddShiftTimeToUserMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "ShiftTime", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "ShiftTime");
        }
    }
}
