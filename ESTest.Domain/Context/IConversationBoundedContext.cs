using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESTest.Domain
{
    public interface IConversationBoundedContext
    {
        Task<IConversation> GetById(int conversationId);
        Task<Tuple<IConversation, IUserSummary>> GetBySignalRConnectionIdWithActiveUser(string signalRConnectionId);
        Task<IConversation> JoinUserToConversation(IConversation conversation, long userId, string signalRConnectionId);
        Task<IConversation> LeaveUserFromConversation(IConversation conversation, long userId);
        Task<IConversation> Save(IConversation conversation);
        Task CloseConversation(IConversation conversation);
        Task CloseConversation(int conversationId);
        Task<IMessage> GetMessage(int messageId);
        Task<IMessage> CreateMessage(long userId, string messageText, int conversationId);
        Task<IEnumerable<IMessage>> GetMessagesByConversationId(int conversationId);
        Task<IEnumerable<IMessage>> GetMessagesByConversationId(int conversationId, int getLastNumberOfMessages);
    }
}