using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESTest.DAL;
using System.Data.Entity;

namespace ESTest.Domain
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly IQueryContext _queryContext = null;
        private readonly ICommandContext _commandContext = null;

        public ConversationRepository(IQueryContext queryContext, ICommandContext commandContext)
        {
            this._commandContext = commandContext ?? throw new ArgumentNullException("commandContext");
            this._queryContext = queryContext ?? throw new ArgumentNullException("queryContext");
        }

        public async Task<IEnumerable<IConversation>> GetAllActive()
        {
            var query = _queryContext.Conversations.AsNoTracking().Where(d => d.IsActive == true);
            return await (from c in query
                          select new Conversation()
                          {
                              ConversationId = c.ConversationId,
                              CloseDateTime = c.CloseDateTime,
                              IsActive = c.IsActive,
                              OpenDateTime = c.OpenDateTime,
                              Topic = c.Topic
                          }
                         ).ToListAsync();
        }
        public async Task<IConversation> GetById(int conversationId, IEnumerable<IUserSummary> activeUsers)
        {
            var entity = await _queryContext.Conversations.AsNoTracking().Where(d => d.ConversationId == conversationId).FirstOrDefaultAsync();
            if (entity != null)
            {
                Conversation conversation = new Conversation(activeUsers)
                {
                    ConversationId = entity.ConversationId,
                    Topic = entity.Topic,
                    IsActive = entity.IsActive,
                    OpenDateTime = entity.OpenDateTime,
                    CloseDateTime = entity.CloseDateTime
                };
                return conversation;
            }
            else
                return null;
        }
        public async Task<int> GetConversationIdBySignalRId(string signalRConnectionId)
        {
            return await (from cu in _queryContext.ConversationUsers.AsNoTracking().Where(d => d.SignalRConnectionId == signalRConnectionId && d.LeftDate == null)
                          join C in _queryContext.Conversations.AsNoTracking() on cu.ConversationId equals C.ConversationId into cu_C
                          from c in cu_C
                          select c.ConversationId).FirstOrDefaultAsync();
        }
        public async Task<IConversation> Create(string topic)
        {
            if (string.IsNullOrWhiteSpace(topic)) return null;
            DAL.Conversation entity = new DAL.Conversation
            {
                IsActive = true,
                OpenDateTime = DateTime.UtcNow,
                Topic = topic
            };
            this._commandContext.Conversations.Add(entity);
            await this._commandContext.SaveChangesAsync();
            var conversation = new Conversation
            {
                ConversationId = entity.ConversationId,
                IsActive = true,
                OpenDateTime = entity.OpenDateTime,
                Topic = topic
            };
            return conversation;
        }
        public async Task<IConversation> Persist(IConversation conversation)
        {
            if (conversation == null)
                return null;

            DAL.Conversation entity = null;
            bool addEntity = false;
            if (conversation.ConversationId > 0)
            {
                entity = await _queryContext.Conversations.AsNoTracking().Where(d => d.ConversationId == conversation.ConversationId).FirstOrDefaultAsync();
            }
            if (entity == null)
            {
                addEntity = true;
                entity = new DAL.Conversation
                {
                    OpenDateTime = DateTime.UtcNow,
                    IsActive = true
                };
            }
            else
            {
                this._commandContext.Conversations.Attach(entity);
            }
            entity.Topic = conversation.Topic;
            if (addEntity)
                this._commandContext.Conversations.Add(entity);
            await this._commandContext.SaveChangesAsync();
            conversation.ConversationId = entity.ConversationId;
            return conversation;
        }
        public async Task<IConversation> CloseConversation(IConversation conversation)
        {
            if (conversation == null)
                return null;
            var entity = await _queryContext.Conversations.AsNoTracking().Where(d => d.ConversationId == conversation.ConversationId).FirstOrDefaultAsync();
            if (entity == null)
                return null;
            this._commandContext.Conversations.Attach(entity);
            entity.IsActive = false;
            conversation.IsActive = false;
            entity.CloseDateTime = DateTime.UtcNow;
            conversation.CloseDateTime = entity.CloseDateTime;
            await this._commandContext.SaveChangesAsync();
            return conversation;
        }
        public async void CloseConversation(int conversationId)
        {
            var entity = await _queryContext.Conversations.AsNoTracking().Where(d => d.ConversationId == conversationId).FirstOrDefaultAsync();
            if (entity != null)
            {
                this._commandContext.Conversations.Attach(entity);
                entity.IsActive = false;
                entity.CloseDateTime = DateTime.UtcNow;
                await this._commandContext.SaveChangesAsync();
            }
        }
        public async Task<bool> ConversationHasActiveUser(int conversationId, long userId)
        {
            return await _queryContext.ConversationUsers.AsNoTracking()
                .Where(d => d.ConversationId == conversationId && d.LeftDate == null && d.UserId == userId)
                .AnyAsync();
        }
        public async void JoinUserToConversation(int conversationId, long userId, string signalRConnectionId)
        {
            var cu = new ConversationUser()
            {
                JoinedDate = DateTime.UtcNow,
                ConversationId = conversationId,
                UserId = userId,
                SignalRConnectionId = signalRConnectionId
            };
            _commandContext.ConversationUsers.Add(cu);
            await _commandContext.SaveChangesAsync();
        }
        public async void LeaveUserFromConversation(IConversation conversation, long userId)
        {
            var entities = await _queryContext.ConversationUsers.AsNoTracking().Where(d => d.ConversationId == conversation.ConversationId && d.UserId == userId && d.LeftDate == null).ToListAsync();
            if (entities != null && entities.Any())
            {
                foreach (var entity in entities)
                {
                    var commandEntity = await this._commandContext.ConversationUsers.FirstAsync(d => d.ConversationUserId == entity.ConversationUserId);
                    if (commandEntity == null)
                    {
                        commandEntity = entity;
                        this._commandContext.ConversationUsers.Attach(commandEntity);
                    }
                    commandEntity.LeftDate = DateTime.UtcNow;
                }
                await this._commandContext.SaveChangesAsync();
            }
        }
    }
}
