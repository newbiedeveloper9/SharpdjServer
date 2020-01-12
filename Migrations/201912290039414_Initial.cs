namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Claims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserConnections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Ip = c.String(),
                        Port = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        ConnectionType = c.Int(nullable: false),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        Email = c.String(),
                        Rank = c.Int(nullable: false),
                        AvatarUrl = c.String(),
                        User_Id = c.Int(),
                        UserAuth_Id = c.Int(),
                        Conversation_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .ForeignKey("dbo.UserAuths", t => t.UserAuth_Id)
                .ForeignKey("dbo.Conversations", t => t.Conversation_Id)
                .Index(t => t.User_Id)
                .Index(t => t.UserAuth_Id)
                .Index(t => t.Conversation_Id);
            
            CreateTable(
                "dbo.UserAuths",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Login = c.String(),
                        Hash = c.String(),
                        Salt = c.String(),
                        AuthenticationKey = c.String(),
                        AuthenticationExpiration = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ConversationMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Author_Id = c.Int(),
                        Conversation_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Author_Id)
                .ForeignKey("dbo.Conversations", t => t.Conversation_Id)
                .Index(t => t.Author_Id)
                .Index(t => t.Conversation_Id);
            
            CreateTable(
                "dbo.Conversations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        ConnectionType = c.Int(nullable: false),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.MediaHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MediaType = c.Int(nullable: false),
                        Url = c.String(),
                        Room_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Rooms", t => t.Room_Id)
                .Index(t => t.Room_Id);
            
            CreateTable(
                "dbo.RoleClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Role_Id = c.Int(),
                        Type_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ServerRoles", t => t.Role_Id)
                .ForeignKey("dbo.Claims", t => t.Type_Id)
                .Index(t => t.Role_Id)
                .Index(t => t.Type_Id);
            
            CreateTable(
                "dbo.ServerRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RoomChatPosts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Color = c.String(),
                        Author_Id = c.Int(),
                        Room_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Author_Id)
                .ForeignKey("dbo.Rooms", t => t.Room_Id)
                .Index(t => t.Author_Id)
                .Index(t => t.Room_Id);
            
            CreateTable(
                "dbo.RoomConfigs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ChatType = c.Int(nullable: false),
                        PublicEnterMessage = c.String(),
                        PublicLeaveMessage = c.String(),
                        LocalEnterMessage = c.String(),
                        LocalLeaveMessage = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Rooms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ImagePath = c.String(),
                        Author_Id = c.Int(),
                        RoomConfig_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Author_Id)
                .ForeignKey("dbo.RoomConfigs", t => t.RoomConfig_Id)
                .Index(t => t.Author_Id)
                .Index(t => t.RoomConfig_Id);
            
            CreateTable(
                "dbo.UserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type_Id = c.Int(),
                        User_Id = c.Int(),
                        Room_Id = c.Int(),
                        Room_Id1 = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Claims", t => t.Type_Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .ForeignKey("dbo.Rooms", t => t.Room_Id)
                .ForeignKey("dbo.Rooms", t => t.Room_Id1)
                .Index(t => t.Type_Id)
                .Index(t => t.User_Id)
                .Index(t => t.Room_Id)
                .Index(t => t.Room_Id1);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserClaims", "Room_Id1", "dbo.Rooms");
            DropForeignKey("dbo.Rooms", "RoomConfig_Id", "dbo.RoomConfigs");
            DropForeignKey("dbo.UserClaims", "Room_Id", "dbo.Rooms");
            DropForeignKey("dbo.UserClaims", "User_Id", "dbo.Users");
            DropForeignKey("dbo.UserClaims", "Type_Id", "dbo.Claims");
            DropForeignKey("dbo.RoomChatPosts", "Room_Id", "dbo.Rooms");
            DropForeignKey("dbo.MediaHistories", "Room_Id", "dbo.Rooms");
            DropForeignKey("dbo.Rooms", "Author_Id", "dbo.Users");
            DropForeignKey("dbo.RoomChatPosts", "Author_Id", "dbo.Users");
            DropForeignKey("dbo.RoleClaims", "Type_Id", "dbo.Claims");
            DropForeignKey("dbo.RoleClaims", "Role_Id", "dbo.ServerRoles");
            DropForeignKey("dbo.Logs", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Users", "Conversation_Id", "dbo.Conversations");
            DropForeignKey("dbo.ConversationMessages", "Conversation_Id", "dbo.Conversations");
            DropForeignKey("dbo.ConversationMessages", "Author_Id", "dbo.Users");
            DropForeignKey("dbo.UserConnections", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Users", "UserAuth_Id", "dbo.UserAuths");
            DropForeignKey("dbo.Users", "User_Id", "dbo.Users");
            DropIndex("dbo.UserClaims", new[] { "Room_Id1" });
            DropIndex("dbo.UserClaims", new[] { "Room_Id" });
            DropIndex("dbo.UserClaims", new[] { "User_Id" });
            DropIndex("dbo.UserClaims", new[] { "Type_Id" });
            DropIndex("dbo.Rooms", new[] { "RoomConfig_Id" });
            DropIndex("dbo.Rooms", new[] { "Author_Id" });
            DropIndex("dbo.RoomChatPosts", new[] { "Room_Id" });
            DropIndex("dbo.RoomChatPosts", new[] { "Author_Id" });
            DropIndex("dbo.RoleClaims", new[] { "Type_Id" });
            DropIndex("dbo.RoleClaims", new[] { "Role_Id" });
            DropIndex("dbo.MediaHistories", new[] { "Room_Id" });
            DropIndex("dbo.Logs", new[] { "User_Id" });
            DropIndex("dbo.ConversationMessages", new[] { "Conversation_Id" });
            DropIndex("dbo.ConversationMessages", new[] { "Author_Id" });
            DropIndex("dbo.Users", new[] { "Conversation_Id" });
            DropIndex("dbo.Users", new[] { "UserAuth_Id" });
            DropIndex("dbo.Users", new[] { "User_Id" });
            DropIndex("dbo.UserConnections", new[] { "User_Id" });
            DropTable("dbo.UserClaims");
            DropTable("dbo.Rooms");
            DropTable("dbo.RoomConfigs");
            DropTable("dbo.RoomChatPosts");
            DropTable("dbo.ServerRoles");
            DropTable("dbo.RoleClaims");
            DropTable("dbo.MediaHistories");
            DropTable("dbo.Logs");
            DropTable("dbo.Conversations");
            DropTable("dbo.ConversationMessages");
            DropTable("dbo.UserAuths");
            DropTable("dbo.Users");
            DropTable("dbo.UserConnections");
            DropTable("dbo.Claims");
        }
    }
}
