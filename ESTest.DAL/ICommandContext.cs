using System.Data.Entity;
using System.Threading.Tasks;

namespace ESTest.DAL
{
    public interface ICommandContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Conversation> Conversations { get; set; }
        DbSet<Message> Messages { get; set; }
        DbSet<ConversationUser> ConversationUsers { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
