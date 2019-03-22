namespace ESTest.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingSignalRConnectionIdToConversation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ConversationUsers", "SignalRConnectionId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ConversationUsers", "SignalRConnectionId");
        }
    }
}
