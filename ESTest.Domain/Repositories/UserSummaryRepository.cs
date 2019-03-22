using System;
using System.Threading.Tasks;
using ESTest.DAL;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;

namespace ESTest.Domain
{
    public class UserSummaryRepository : IUserSummaryRepository
    {
        private readonly IQueryContext _queryContext = null;

        public UserSummaryRepository(IQueryContext queryContext)
        {
            this._queryContext = queryContext ?? throw new ArgumentNullException("queryContext");
        }
        public async Task<IUserSummary> GetById(long userId)
        {
            var user = await _queryContext.Users.AsNoTracking().Where(d => d.Id == userId).FirstOrDefaultAsync();
            if (user != null)
            {
                return new UserSummary()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmailAddress = user.EmailAddress
                };
            }
            else
                return null;
        }
        public async Task<IEnumerable<IUserSummary>> GetByIds(IEnumerable<long> userIds)
        {
            var users = await (from user in _queryContext.Users.AsNoTracking()
                               join ID in userIds on user.Id equals ID into user_ID
                               from id in user_ID
                               select new UserSummary()
                               {
                                   Id = user.Id,
                                   FirstName = user.FirstName,
                                   LastName = user.LastName,
                                   EmailAddress = user.EmailAddress
                               }).ToListAsync();
            return users;
        }
        public async Task<IEnumerable<IUserSummary>> GetActiveUsersByConversationId(int conversationId)
        {
            var query = await (from cu in _queryContext.ConversationUsers.AsNoTracking().Where(d => d.ConversationId == conversationId && d.LeftDate == null).Distinct()
                               join U in _queryContext.Users.AsNoTracking() on cu.UserId equals U.Id into cu_U
                               from user in cu_U
                               select new UserSummary()
                               {
                                   Id = user.Id,
                                   FirstName = user.FirstName,
                                   LastName = user.LastName,
                                   EmailAddress = user.EmailAddress
                               }).ToListAsync();
            return query;
        }
        public async Task<IUserSummary> GetUserBySignalRId(string signalRConnectionId)
        {
            return await (from cu in _queryContext.ConversationUsers.AsNoTracking().Where(d => d.SignalRConnectionId == signalRConnectionId && d.LeftDate == null)
                          join U in _queryContext.Users.AsNoTracking() on cu.UserId equals U.Id into cu_U
                          from user in cu_U
                          select new UserSummary()
                          {
                              Id = user.Id,
                              FirstName = user.FirstName,
                              LastName = user.LastName,
                              EmailAddress = user.EmailAddress
                          }).FirstOrDefaultAsync();
        }
    }
}
