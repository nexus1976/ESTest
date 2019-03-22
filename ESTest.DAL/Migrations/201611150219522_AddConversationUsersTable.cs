namespace ESTest.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddConversationUsersTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ConversationUsers",
                c => new
                    {
                        ConversationUserId = c.Int(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        ConversationId = c.Int(nullable: false),
                        JoinedDate = c.DateTime(nullable: false),
                        LeftDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ConversationUserId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .ForeignKey("dbo.Conversations", t => t.ConversationId)
                .Index(t => t.UserId)
                .Index(t => t.ConversationId);
            
            AlterColumn("dbo.Messages", "CreatedByUserId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ConversationUsers", "ConversationId", "dbo.Conversations");
            DropForeignKey("dbo.ConversationUsers", "UserId", "dbo.Users");
            DropIndex("dbo.ConversationUsers", new[] { "ConversationId" });
            DropIndex("dbo.ConversationUsers", new[] { "UserId" });
            AlterColumn("dbo.Messages", "CreatedByUserId", c => c.Int(nullable: false));
            DropTable("dbo.ConversationUsers");
        }
    }
}
