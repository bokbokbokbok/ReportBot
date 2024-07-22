namespace McgTgBotNet.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserProjectMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserProject",
                c => new
                    {
                        User_Id = c.Int(nullable: false),
                        Project_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_Id, t.Project_Id })
                .ForeignKey("dbo.User", t => t.User_Id, cascadeDelete: true)
                .ForeignKey("dbo.Project", t => t.Project_Id, cascadeDelete: true)
                .Index(t => t.User_Id)
                .Index(t => t.Project_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserProject", "Project_Id", "dbo.Project");
            DropForeignKey("dbo.UserProject", "User_Id", "dbo.User");
            DropIndex("dbo.UserProject", new[] { "Project_Id" });
            DropIndex("dbo.UserProject", new[] { "User_Id" });
            DropTable("dbo.UserProject");
        }
    }
}
