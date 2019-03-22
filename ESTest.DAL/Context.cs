using System.Data.Entity;
using System.Threading.Tasks;

namespace ESTest.DAL
{
    public class Context : DbContext, IQueryContext, ICommandContext
    {
        public Context() : base("ESTest") { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Conversation> Conversations { get; set; }
        public virtual DbSet<ConversationUser> ConversationUsers { get; set; }
        public virtual DbSet<Message> Messages { get; set; }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
        public override Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().Property(e => e.Address1).IsUnicode(true);
            modelBuilder.Entity<User>().Property(e => e.Address2).IsUnicode(true);
            modelBuilder.Entity<User>().Property(e => e.City).IsUnicode(true);
            modelBuilder.Entity<User>().Property(e => e.EmailAddress).IsUnicode(true);
            modelBuilder.Entity<User>().Property(e => e.FirstName).IsUnicode(true);
            modelBuilder.Entity<User>().Property(e => e.LastName).IsUnicode(true);
            modelBuilder.Entity<User>().Property(e => e.MainPhone).IsUnicode(true);
            modelBuilder.Entity<User>().Property(e => e.Password).IsUnicode(true);
            modelBuilder.Entity<User>().Property(e => e.PostalCode).IsUnicode(true);
            modelBuilder.Entity<User>().Property(e => e.Province).IsUnicode(true);
            modelBuilder.Entity<User>().Property(e => e.Salt).IsUnicode(true);
            modelBuilder.Entity<Message>().Property(e => e.MessageText).IsUnicode(true);
            modelBuilder.Entity<Conversation>().Property(e => e.Topic).IsUnicode(true);

            modelBuilder.Entity<Conversation>().HasMany(d => d.Messages).WithRequired(d => d.Conversation).HasForeignKey(d => d.ConversationId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Conversation>().HasMany(d => d.ConversationUsers).WithRequired(d => d.Conversation).HasForeignKey(d => d.ConversationId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(d => d.ConversationUsers).WithRequired(d => d.User).HasForeignKey(d => d.UserId).WillCascadeOnDelete(false);
        }
    }
}
