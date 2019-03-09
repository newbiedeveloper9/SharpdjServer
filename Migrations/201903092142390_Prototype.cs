namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Prototype : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserConnections",
                c => new
                    {
                        UserConnectionId = c.Int(nullable: false, identity: true),
                        Ip = c.String(),
                        Port = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        ConnectionType = c.Int(nullable: false),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.UserConnectionId)
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
                        Conversation_Id = c.Int(),
                    })
                .PrimaryKey(t => t.UserId)
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
                        UserAuthId = c.Int(nullable: false, identity: true),
                        Login = c.String(),
                        Hash = c.String(),
                        Salt = c.String(),
                    })
                .PrimaryKey(t => t.UserAuthId);
            
            CreateTable(
                "dbo.ConversationMessages",
                c => new
                    {
                        ConversationMessageId = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Author_Id = c.Int(),
                        Conversation_Id = c.Int(),
                    })
                .PrimaryKey(t => t.ConversationMessageId)
                .ForeignKey("dbo.Users", t => t.Author_Id)
                .ForeignKey("dbo.Conversations", t => t.Conversation_Id)
                .Index(t => t.Author_Id)
                .Index(t => t.Conversation_Id);
            
            CreateTable(
                "dbo.Conversations",
                c => new
                    {
                        ConversationId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.ConversationId);
            
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
                "dbo.MediaHistories",
                c => new
                    {
                        MediaHistoryId = c.Int(nullable: false, identity: true),
                        MediaType = c.Int(nullable: false),
                        Url = c.String(),
                        Room_Id = c.Int(),
                    })
                .PrimaryKey(t => t.MediaHistoryId)
                .ForeignKey("dbo.Rooms", t => t.Room_Id)
                .Index(t => t.Room_Id);
            
            CreateTable(
                "dbo.RoomChatPosts",
                c => new
                    {
                        RoomChatPostId = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Author_Id = c.Int(),
                        Room_Id = c.Int(),
                    })
                .PrimaryKey(t => t.RoomChatPostId)
                .ForeignKey("dbo.Users", t => t.Author_Id)
                .ForeignKey("dbo.Rooms", t => t.Room_Id)
                .Index(t => t.Author_Id)
                .Index(t => t.Room_Id);
            
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
            DropForeignKey("dbo.RoomChatPosts", "Room_Id", "dbo.Rooms");
            DropForeignKey("dbo.MediaHistories", "Room_Id", "dbo.Rooms");
            DropForeignKey("dbo.Rooms", "Author_Id", "dbo.Users");
            DropForeignKey("dbo.RoomChatPosts", "Author_Id", "dbo.Users");
            DropForeignKey("dbo.Logs", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Users", "Conversation_Id", "dbo.Conversations");
            DropForeignKey("dbo.ConversationMessages", "Conversation_Id", "dbo.Conversations");
            DropForeignKey("dbo.ConversationMessages", "Author_Id", "dbo.Users");
            DropForeignKey("dbo.UserConnections", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Users", "UserAuth_Id", "dbo.UserAuths");
            DropForeignKey("dbo.Users", "User_Id", "dbo.Users");
            DropIndex("dbo.Rooms", new[] { "RoomConfig_Id" });
            DropIndex("dbo.Rooms", new[] { "Author_Id" });
            DropIndex("dbo.RoomChatPosts", new[] { "Room_Id" });
            DropIndex("dbo.RoomChatPosts", new[] { "Author_Id" });
            DropIndex("dbo.MediaHistories", new[] { "Room_Id" });
            DropIndex("dbo.Logs", new[] { "User_Id" });
            DropIndex("dbo.ConversationMessages", new[] { "Conversation_Id" });
            DropIndex("dbo.ConversationMessages", new[] { "Author_Id" });
            DropIndex("dbo.Users", new[] { "Conversation_Id" });
            DropIndex("dbo.Users", new[] { "UserAuth_Id" });
            DropIndex("dbo.Users", new[] { "User_Id" });
            DropIndex("dbo.UserConnections", new[] { "User_Id" });
            DropTable("dbo.Rooms");
            DropTable("dbo.RoomConfigs");
            DropTable("dbo.RoomChatPosts");
            DropTable("dbo.MediaHistories");
            DropTable("dbo.Logs");
            DropTable("dbo.Conversations");
            DropTable("dbo.ConversationMessages");
            DropTable("dbo.UserAuths");
            DropTable("dbo.Users");
            DropTable("dbo.UserConnections");
        }
    }
}
