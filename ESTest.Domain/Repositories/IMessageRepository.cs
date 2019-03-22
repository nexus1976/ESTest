using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESTest.Domain
{
    public interface IMessageRepository
    {
        Task<IMessage> Create(IUserSummary user, string messageText, int conversationId);
        Task<IEnumerable<IMessage>> GetByConversationId(int conversationId);
        Task<IEnumerable<IMessage>> GetByConversationId(int conversationId, int getLastNumberOfMessages);
        Task<IMessage> GetById(int messageId);
    }
}