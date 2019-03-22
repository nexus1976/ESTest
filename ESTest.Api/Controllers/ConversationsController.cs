using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using ESTest.Api.Hubs;
using ESTest.DAL;
using ESTest.Domain;

namespace ESTest.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/conversations")]
    public class ConversationsController : ApiControllerWithHub<ConversationHub>
    {
        private readonly IConversationBoundedContext _conversationBoundedContext = null;
        private readonly IConversationRepository _conversationRepository = null;
        private readonly IMessageRepository _messageRepository = null;

        public ConversationsController(IConversationBoundedContext conversationBoundedContext, 
            IConversationRepository conversationRepository,
            IMessageRepository messageRepository)
        {
            this._conversationBoundedContext = conversationBoundedContext;
            this._conversationRepository = conversationRepository;
            this._messageRepository = messageRepository;
        }

        // GET: /api/conversations
        public async Task<IHttpActionResult> Get()
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();
            var conversations = await _conversationRepository.GetAllActive();
            if (conversations == null)
                return NotFound();
            return Ok(conversations);
        }

        // GET: /api/conversations/1
        public async Task<IHttpActionResult> Get(int id)
        {
            if(User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();
            var conversation = await _conversationBoundedContext.GetById(id);
            if (conversation == null)
                return NotFound();
            return Ok(conversation);
        }

        // POST: /api/conversations
        public async Task<IHttpActionResult> Post([FromBody]Domain.Conversation conversation)
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();
            var response = await _conversationBoundedContext.Save(conversation);
            return Ok(response);
        }

        // DELETE: /api/conversations/1
        public async Task<IHttpActionResult> Delete([FromUri]int id)
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();
            await _conversationBoundedContext.CloseConversation(id);
            return Ok();
        }

        // GET: /api/conversations/1/messages
        [HttpGet]
        [Route("{id:int}/messages")]
        public async Task<IHttpActionResult> GetLastFiveMessages([FromUri]int id)
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();
            var messages = await _conversationBoundedContext.GetMessagesByConversationId(id, 5);
            if (messages == null)
                return NotFound();
            return Ok(messages);
        }

        // POST: /api/conversations/1/messages
        [HttpPost]
        [Route("{id:int}/messages")]
        public async Task<IHttpActionResult> PostNewMessage([FromUri]int id, [FromBody] Domain.Message message)
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();
            long userId = User.Identity.GetUserId<long>();
            if (userId <= 0)
                return Unauthorized();
            if (message == null || string.IsNullOrWhiteSpace(message.MessageText))
                return BadRequest("The message cannot be empty");

            var newMessage = await _conversationBoundedContext.CreateMessage(userId, message.MessageText, id);
            Hub.Clients.Group(id.ToString()).addItem(newMessage);
            return Ok(newMessage);
        }
    }
}
