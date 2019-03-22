using System.Data.Entity;

namespace ESTest.DAL
{
    public interface IQueryContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Conversation> Conversations { get; set; }
        DbSet<Message> Messages { get; set; }
        DbSet<ConversationUser> ConversationUsers { get; set; }
    }
}
