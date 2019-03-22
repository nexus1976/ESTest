namespace ESTest.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<ESTest.DAL.Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ESTest.DAL.Context context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            context.Users.AddOrUpdate(u => u.EmailAddress, new User
            {
                EmailAddress = "nexu76@danielgraham.com",
                Password = "EKgqJ825D/8QFWQyzpoKmB1SsEdHUmul0yD0aHXf/6s=",//Beavis02
                Salt = "KUnFRl48X0rIZduX39bcaBfS7mkHS12J",
                FirstName = "Daniel",
                LastName = "Graham",
                Address1 = "101 Happy Street.",
                City = "Camarillo",
                Province = "CA",
                PostalCode = "93012",
                MainPhone = "8055991234",
                CreatedDate = DateTime.UtcNow
            });
            context.Users.AddOrUpdate(u => u.EmailAddress, new User
            {
                EmailAddress = "mary.graham@familyguy.com",
                Password = "EKgqJ825D/8QFWQyzpoKmB1SsEdHUmul0yD0aHXf/6s=",//Beavis02
                Salt = "KUnFRl48X0rIZduX39bcaBfS7mkHS12J",
                FirstName = "Mary",
                LastName = "Graham",
                Address1 = "101 Happy Street.",
                City = "Camarillo",
                Province = "CA",
                PostalCode = "93012",
                MainPhone = "8055994321",
                CreatedDate = DateTime.UtcNow
            });
            context.Conversations.AddOrUpdate(c => c.ConversationId, new Conversation
            {
                Topic = "Test Chat Room",
                OpenDateTime = DateTime.UtcNow,
                IsActive = true                
            });
        }
    }
}
