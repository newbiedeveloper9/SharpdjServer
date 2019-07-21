namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoomChatPostUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RoomChatPosts", "Color", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RoomChatPosts", "Color");
        }
    }
}
