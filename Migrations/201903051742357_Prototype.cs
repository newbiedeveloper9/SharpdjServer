namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Prototype : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        LogId = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        ConnectionType = c.Int(nullable: false),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.LogId)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
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
                "dbo.RoomConfigs",
                c => new
                    {
                        RoomConfigId = c.Int(nullable: false, identity: true),
                        ChatType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RoomConfigId);
            
            CreateTable(
                "dbo.Rooms",
                c => new
                    {
                        RoomId = c.Int(nullable: false, identity: true),
                        Name = c.Int(nullable: false),
                        ImagePath = c.String(),
                        Author_Id = c.Int(),
                        RoomConfig_Id = c.Int(),
                    })
                .PrimaryKey(t => t.RoomId)
                .ForeignKey("dbo.Users", t => t.Author_Id)
                .ForeignKey("dbo.RoomConfigs", t => t.RoomConfig_Id)
                .Index(t => t.Author_Id)
                .Index(t => t.RoomConfig_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Rooms", "RoomConfig_Id", "dbo.RoomConfigs");
            DropForeignKey("dbo.Rooms", "Author_Id", "dbo.Users");
            DropForeignKey("dbo.Logs", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Users", "UserAuth_Id", "dbo.UserAuths");
            DropForeignKey("dbo.Users", "User_Id", "dbo.Users");
            DropIndex("dbo.Rooms", new[] { "RoomConfig_Id" });
            DropIndex("dbo.Rooms", new[] { "Author_Id" });
            DropIndex("dbo.Users", new[] { "UserAuth_Id" });
            DropIndex("dbo.Users", new[] { "User_Id" });
            DropIndex("dbo.Logs", new[] { "User_Id" });
            DropTable("dbo.Rooms");
            DropTable("dbo.RoomConfigs");
            DropTable("dbo.UserAuths");
            DropTable("dbo.Users");
            DropTable("dbo.Logs");
        }
    }
}
