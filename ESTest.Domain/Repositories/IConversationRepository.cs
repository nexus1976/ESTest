using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESTest.Domain
{
    public interface IConversationRepository
    {
        Task<IEnumerable<IConversation>> GetAllActive();
        Task<IConversation> GetById(int conversationId, IEnumerable<IUserSummary> activeUsers);
        Task<int> GetConversationIdBySignalRId(string signalRConnectionId);
        Task<IConversation> Create(string topic);
        Task<IConversation> Persist(IConversation conversation);
        Task<IConversation> CloseConversation(IConversation conversation);
        void CloseConversation(int conversationId);
        Task<bool> ConversationHasActiveUser(int conversationId, long userId);
        void JoinUserToConversation(int conversationId, long userId, string signalRConnectionId);
        void LeaveUserFromConversation(IConversation conversation, long userId);
    }
}