namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Claims : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Claims",
                c => new
                    {
                        ClaimId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ClaimId);
            
            CreateTable(
                "dbo.RoleClaims",
                c => new
                    {
                        RoleClaimId = c.Int(nullable: false, identity: true),
                        Role_Id = c.Int(),
                        Type_Id = c.Int(),
                    })
                .PrimaryKey(t => t.RoleClaimId)
                .ForeignKey("dbo.ServerRoles", t => t.Role_Id)
                .ForeignKey("dbo.Claims", t => t.Type_Id)
                .Index(t => t.Role_Id)
                .Index(t => t.Type_Id);
            
            CreateTable(
                "dbo.ServerRoles",
                c => new
                    {
                        ServerRoleId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ServerRoleId);
            
            CreateTable(
                "dbo.UserClaims",
                c => new
                    {
                        UserClaimId = c.Int(nullable: false, identity: true),
                        Type_Id = c.Int(),
                        User_Id = c.Int(),
                        Room_Id = c.Int(),
                        Room_Id1 = c.Int(),
                    })
                .PrimaryKey(t => t.UserClaimId)
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
            DropForeignKey("dbo.UserClaims", "Room_Id", "dbo.Rooms");
            DropForeignKey("dbo.UserClaims", "User_Id", "dbo.Users");
            DropForeignKey("dbo.UserClaims", "Type_Id", "dbo.Claims");
            DropForeignKey("dbo.RoleClaims", "Type_Id", "dbo.Claims");
            DropForeignKey("dbo.RoleClaims", "Role_Id", "dbo.ServerRoles");
            DropIndex("dbo.UserClaims", new[] { "Room_Id1" });
            DropIndex("dbo.UserClaims", new[] { "Room_Id" });
            DropIndex("dbo.UserClaims", new[] { "User_Id" });
            DropIndex("dbo.UserClaims", new[] { "Type_Id" });
            DropIndex("dbo.RoleClaims", new[] { "Type_Id" });
            DropIndex("dbo.RoleClaims", new[] { "Role_Id" });
            DropTable("dbo.UserClaims");
            DropTable("dbo.ServerRoles");
            DropTable("dbo.RoleClaims");
            DropTable("dbo.Claims");
        }
    }
}
