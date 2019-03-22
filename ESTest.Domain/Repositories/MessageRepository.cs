using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESTest.DAL;
using System.Data.Entity;

namespace ESTest.Domain
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IQueryContext _queryContext = null;
        private readonly ICommandContext _commandContext = null;

        public MessageRepository(IQueryContext queryContext, ICommandContext commandContext)
        {
            this._commandContext = commandContext ?? throw new ArgumentNullException("commandContext");
            this._queryContext = queryContext ?? throw new ArgumentNullException("queryContext");
        }

        public async Task<IEnumerable<IMessage>> GetByConversationId(int conversationId)
        {
            var query = await GetMessagesBaseQuery().Where(d => d.ConversationId == conversationId).ToListAsync();
            return query;
        }
        public async Task<IEnumerable<IMessage>> GetByConversationId(int conversationId, int getLastNumberOfMessages)
        {
            var query = await GetMessagesBaseQuery()
                .Where(d => d.ConversationId == conversationId)
                .Take(getLastNumberOfMessages)
                .OrderBy(d => d.CreatedDateTime)
                .ToListAsync();
            return query;
        }
        public async Task<IMessage> GetById(int messageId)
        {
            var query = await GetMessagesBaseQuery().Where(d => d.MessageId == messageId).FirstOrDefaultAsync();
            return query;
        }
        public async Task<IMessage> Create(IUserSummary user, string messageText, int conversationId)
        {
            if (string.IsNullOrWhiteSpace(messageText))
                throw new ArgumentNullException("messageText");
            if (conversationId <= 0)
                throw new ArgumentOutOfRangeException("conversationId", conversationId, "The conversationId must be a valid value greater than 0.");
            if (user == null)
                throw new ArgumentNullException("user");
            if (user.Id <= 0 || string.IsNullOrWhiteSpace(user.DisplayName))
                throw new ArgumentException("The IUserSummary could not be read. Please ensure to pass a valid instance with a hydrated id and display name.", "user");

            var entity = new DAL.Message()
            {
                ConversationId = conversationId,
                CreatedByUserId = user.Id,
                CreatedDateTime = DateTime.UtcNow,
                MessageText = messageText
            };
            this._commandContext.Messages.Add(entity);
            await this._commandContext.SaveChangesAsync();
            var message = await this.GetById(entity.MessageId);
            message.SetCreatedUser(user);
            return message;
        }

        private IQueryable<Message> GetMessagesBaseQuery()
        {
            var query = from m in _queryContext.Messages.AsNoTracking()
                        join U in _queryContext.Users.AsNoTracking() on m.CreatedByUserId equals U.Id into m_U
                        from u in m_U
                        orderby m.CreatedDateTime descending
                        select new Message()
                        {
                            MessageId = m.MessageId,
                            ConversationId = m.ConversationId,
                            //CreatedByDisplayName = ((string.IsNullOrEmpty(u.FirstName) ? string.Empty : u.FirstName.Trim() + " ") + (string.IsNullOrEmpty(u.LastName) ? string.Empty : u.LastName.Trim())).Trim(), //refactor to use domain entity
                            CreatedByUserId = m.CreatedByUserId,
                            CreatedDateTime = m.CreatedDateTime,
                            MessageText = m.MessageText
                        };
            return query;
        }
    }
}
