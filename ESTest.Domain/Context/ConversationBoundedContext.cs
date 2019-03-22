using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTest.Domain
{
    public class ConversationBoundedContext : IConversationBoundedContext
    {
        private readonly IUserSummaryRepository _userSummaryRepository = null;
        private readonly IConversationRepository _conversationRepository = null;
        private readonly IMessageRepository _messageRepository = null;
        
        public ConversationBoundedContext(IConversationRepository conversationRepository, IUserSummaryRepository userSummaryRepository, IMessageRepository messageRepository)
        {
            this._userSummaryRepository = userSummaryRepository ?? throw new ArgumentNullException("userSummaryRepository");
            this._conversationRepository = conversationRepository ?? throw new ArgumentNullException("conversationRepository");
            this._messageRepository = messageRepository ?? throw new ArgumentNullException("messageRepository");
        }
        public async Task<IConversation> GetById(int conversationId)
        {
            var activeUsers = await _userSummaryRepository.GetActiveUsersByConversationId(conversationId);
            var conversation = await _conversationRepository.GetById(conversationId, activeUsers);
            return conversation;
        }
        public async Task<Tuple<IConversation, IUserSummary>> GetBySignalRConnectionIdWithActiveUser(string signalRConnectionId)
        {
            Tuple<IConversation, IUserSummary> result = null;
            var user = await _userSummaryRepository.GetUserBySignalRId(signalRConnectionId);
            var conversationId = await _conversationRepository.GetConversationIdBySignalRId(signalRConnectionId);

            if (user != null && conversationId > 0)
            {
                var conversation = await GetById(conversationId);
                result = new Tuple<IConversation, IUserSummary>(conversation, user);
            }
            return result;
        }
        public async Task<IConversation> JoinUserToConversation(IConversation conversation, long userId, string signalRConnectionId)
        {
            bool exists = await _conversationRepository.ConversationHasActiveUser(conversation.ConversationId, userId);
            if (!exists)
            {
                var activeUser = await _userSummaryRepository.GetById(userId);
                if (activeUser != null && !string.IsNullOrWhiteSpace(activeUser.DisplayName))
                {
                    _conversationRepository.JoinUserToConversation(conversation.ConversationId, userId, signalRConnectionId);
                    conversation.UserJoined(activeUser);
                }
            }
            return conversation;
        }
        public Task<IConversation> LeaveUserFromConversation(IConversation conversation, long userId)
        {
            _conversationRepository.LeaveUserFromConversation(conversation, userId);
            conversation.UserLeft(userId);
            return Task.FromResult(conversation);
        }
        public async Task<IConversation> Save(IConversation conversation)
        {
            return await _conversationRepository.Persist(conversation);
        }
        public Task CloseConversation(IConversation conversation)
        {
            _conversationRepository.CloseConversation(conversation);
            return null;
        }
        public async Task CloseConversation(int conversationId)
        {
            var conversation = await GetById(conversationId);
            await CloseConversation(conversation);
            return;
        }

        public async Task<IMessage> GetMessage(int messageId)
        {
            var message = await _messageRepository.GetById(messageId);
            if(message != null)
            {
                var user = await _userSummaryRepository.GetById(message.CreatedByUserId);
                message.SetCreatedUser(user);
            }
            return message;
        }
        public async Task<IMessage> CreateMessage(long userId, string messageText, int conversationId)
        {
            var user = await _userSummaryRepository.GetById(userId);
            if (user != null)
            {
                var message = await _messageRepository.Create(user, messageText, conversationId);
                return message;
            }
            else
                return null;
        }
        public async Task<IEnumerable<IMessage>> GetMessagesByConversationId(int conversationId)
        {
            var messages = await _messageRepository.GetByConversationId(conversationId);
            var userIds = messages.Select(d => d.CreatedByUserId).Distinct();
            var users = (await _userSummaryRepository.GetByIds(userIds)).ToDictionary(d => d.Id);
            foreach (var message in messages)
            {
                message.SetCreatedUser(users[message.CreatedByUserId]);
            }
            return messages;
        }
        public async Task<IEnumerable<IMessage>> GetMessagesByConversationId(int conversationId, int getLastNumberOfMessages)
        {
            var messages = await _messageRepository.GetByConversationId(conversationId, getLastNumberOfMessages);
            var userIds = messages.Select(d => d.CreatedByUserId).Distinct();
            var users = (await _userSummaryRepository.GetByIds(userIds)).ToDictionary(d => d.Id);
            foreach (var message in messages)
            {
                message.SetCreatedUser(users[message.CreatedByUserId]);
            }
            return messages;
        }
    }
}
