namespace ESTest.DAL.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class MessageAndConvoTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Conversations",
                c => new
                    {
                        ConversationId = c.Int(nullable: false, identity: true),
                        Topic = c.String(maxLength: 500),
                        IsActive = c.Boolean(nullable: false),
                        OpenDateTime = c.DateTime(nullable: false),
                        CloseDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ConversationId);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        MessageId = c.Int(nullable: false, identity: true),
                        CreatedByUserId = c.Int(nullable: false),
                        CreatedByDisplayName = c.String(),
                        MessageText = c.String(maxLength: 2000),
                        ConversationId = c.Int(nullable: false),
                        CreatedDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MessageId)
                .ForeignKey("dbo.Conversations", t => t.ConversationId)
                .Index(t => t.ConversationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "ConversationId", "dbo.Conversations");
            DropIndex("dbo.Messages", new[] { "ConversationId" });
            DropTable("dbo.Messages");
            DropTable("dbo.Conversations");
        }
    }
}
