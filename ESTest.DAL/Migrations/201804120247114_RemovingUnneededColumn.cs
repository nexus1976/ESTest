namespace ESTest.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovingUnneededColumn : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Messages", "CreatedByDisplayName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Messages", "CreatedByDisplayName", c => c.String());
        }
    }
}
