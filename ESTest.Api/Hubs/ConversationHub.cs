using Microsoft.AspNet.SignalR;
using ESTest.DAL;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;
using ESTest.Domain;

namespace ESTest.Api.Hubs
{
    [HubName("conversationHub")]
    public class ConversationHub : Hub
    {
        private IQueryContext _queryContext = null;
        private ICommandContext _commandContext = null;
        private IUserSummaryRepository _userSummaryRepository = null;
        private IConversationRepository _conversationRepository = null;
        private IConversationBoundedContext _conversationBoundedContext = null;

        public ConversationHub(IQueryContext queryContext, ICommandContext commandContext, IUserSummaryRepository userSummaryRepository, IConversationRepository conversationRepository, IConversationBoundedContext conversationBoundedContext)
        {
            this._queryContext = queryContext;
            this._commandContext = commandContext;
            this._userSummaryRepository = userSummaryRepository;
            this._conversationRepository = conversationRepository;
            this._conversationBoundedContext = conversationBoundedContext;
        }

        public void Subscribe(int conversationId, long userId)
        {
            Groups.Add(Context.ConnectionId, conversationId.ToString());
            var conversationTask = _conversationBoundedContext.GetById(conversationId);

            conversationTask.ContinueWith(d =>
            {
                var joinUserTask = _conversationBoundedContext.JoinUserToConversation(d.Result, userId, Context.ConnectionId);
                joinUserTask.ContinueWith(jut =>
                {
                    if (jut.Result != null)
                    {
                        var userTask = _userSummaryRepository.GetById(userId);
                        userTask.ContinueWith(u =>
                        {
                            Clients.Group(conversationId.ToString()).userJoined(u.Result);
                        });
                    }
                });
            });
        }
        public void Unsubscribe(int conversationId, long userId)
        {
            Groups.Remove(Context.ConnectionId, conversationId.ToString());
            var conversationTask = _conversationBoundedContext.GetById(conversationId);

            conversationTask.ContinueWith(d =>
            {
                var leaveUserTask = _conversationBoundedContext.LeaveUserFromConversation(d.Result, userId);
                leaveUserTask.ContinueWith(lut =>
                {
                    if (lut.Result != null)
                    {
                        var userTask = _userSummaryRepository.GetById(userId);
                        userTask.ContinueWith(u =>
                        {
                            Clients.Group(conversationId.ToString()).userLeft(u.Result);
                        });
                    }
                });
            });
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var conversationTask = _conversationBoundedContext.GetBySignalRConnectionIdWithActiveUser(Context.ConnectionId);
            conversationTask.ContinueWith(d =>
            {
                if(d.Result != null && d.Result.Item1 != null && d.Result.Item2 != null)
                {
                    IConversation conversation = d.Result.Item1;
                    IUserSummary user = d.Result.Item2;
                    Clients.Group(conversation.ConversationId.ToString()).userLeft(user);
                    _conversationRepository.LeaveUserFromConversation(conversation, user.Id);
                }
            });
            return base.OnDisconnected(stopCalled);
        }
    }
}