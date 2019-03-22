namespace ESTest.DAL.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddUserTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FirstName = c.String(maxLength: 100),
                        LastName = c.String(maxLength: 250),
                        EmailAddress = c.String(maxLength: 254),
                        Password = c.String(),
                        Salt = c.String(),
                        Address1 = c.String(maxLength: 500),
                        Address2 = c.String(maxLength: 500),
                        City = c.String(maxLength: 150),
                        Province = c.String(maxLength: 50),
                        PostalCode = c.String(maxLength: 50),
                        MainPhone = c.String(maxLength: 25),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
        }
    }
}
