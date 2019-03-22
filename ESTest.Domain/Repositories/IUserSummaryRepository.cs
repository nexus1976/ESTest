using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESTest.Domain
{
    public interface IUserSummaryRepository
    {
        Task<IUserSummary> GetById(long userId);
        Task<IEnumerable<IUserSummary>> GetByIds(IEnumerable<long> userIds);
        Task<IEnumerable<IUserSummary>> GetActiveUsersByConversationId(int conversationId);
        Task<IUserSummary> GetUserBySignalRId(string signalRConnectionId);
    }
}