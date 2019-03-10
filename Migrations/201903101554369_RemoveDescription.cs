namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveDescription : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Rooms", "Description");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Rooms", "Description", c => c.String());
        }
    }
}
