namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AuthenticationKey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserAuths", "AuthenticationKey", c => c.String());
            AddColumn("dbo.UserAuths", "AuthenticationExpiration", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserAuths", "AuthenticationExpiration");
            DropColumn("dbo.UserAuths", "AuthenticationKey");
        }
    }
}
