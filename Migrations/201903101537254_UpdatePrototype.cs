namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePrototype : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RoomConfigs", "PublicEnterMessage", c => c.String());
            AddColumn("dbo.RoomConfigs", "PublicLeaveMessage", c => c.String());
            AddColumn("dbo.RoomConfigs", "LocalEnterMessage", c => c.String());
            AddColumn("dbo.RoomConfigs", "LocalLeaveMessage", c => c.String());
            AddColumn("dbo.Rooms", "Description", c => c.String());
            AlterColumn("dbo.Rooms", "Name", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Rooms", "Name", c => c.Int(nullable: false));
            DropColumn("dbo.Rooms", "Description");
            DropColumn("dbo.RoomConfigs", "LocalLeaveMessage");
            DropColumn("dbo.RoomConfigs", "LocalEnterMessage");
            DropColumn("dbo.RoomConfigs", "PublicLeaveMessage");
            DropColumn("dbo.RoomConfigs", "PublicEnterMessage");
        }
    }
}
