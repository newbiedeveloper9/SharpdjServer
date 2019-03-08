namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Prototype : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserAuths",
                c => new
                    {
                        UserAuthId = c.Int(nullable: false, identity: true),
                        Login = c.String(),
                        Hash = c.String(),
                        Salt = c.String(),
                    })
                .PrimaryKey(t => t.UserAuthId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        Email = c.String(),
                        Rank = c.Int(nullable: false),
                        AvatarUrl = c.String(),
                        User_Id = c.Int(),
                        UserAuth_Id = c.Int(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .ForeignKey("dbo.UserAuths", t => t.UserAuth_Id)
                .Index(t => t.User_Id)
                .Index(t => t.UserAuth_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "UserAuth_Id", "dbo.UserAuths");
            DropForeignKey("dbo.Users", "User_Id", "dbo.Users");
            DropIndex("dbo.Users", new[] { "UserAuth_Id" });
            DropIndex("dbo.Users", new[] { "User_Id" });
            DropTable("dbo.Users");
            DropTable("dbo.UserAuths");
        }
    }
}
