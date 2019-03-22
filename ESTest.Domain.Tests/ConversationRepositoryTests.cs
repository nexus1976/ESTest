using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Linq;

//REFACTOR UNIT TESTS
namespace ESTest.Domain.Tests
{
    [TestClass]
    public class ConversationRepositoryTests
    {
        [TestMethod]
        public async Task GetAllActive()
        {
            //assemble
            var queryContext = AssembleMocks.GetQueryContext();
            var commandContext = AssembleMocks.GetCommandContext();
            var conversationRepo = AssembleMocks.GetConversationRepo(queryContext, commandContext);

            //act
            var conversations = await conversationRepo.Object.GetAllActive();

            //assert
            Assert.IsNotNull(conversations);
            Assert.IsTrue(conversations.Count() == 3);
        }
        [TestMethod]
        public async Task GetById_Found()
        {
            //assemble
            var queryContext = AssembleMocks.GetQueryContext();
            var commandContext = AssembleMocks.GetCommandContext();
            var conversationRepo = new ConversationRepository(queryContext.Object, commandContext.Object);

            //act
            var conversation = await conversationRepo.GetById(1, null);

            //assert
            Assert.IsNotNull(conversation);
        }

        [TestMethod]
        public async Task GetById_NotFound()
        {
            //assemble
            var queryContext = AssembleMocks.GetQueryContext();
            var commandContext = AssembleMocks.GetCommandContext();
            var conversationRepo = new ConversationRepository(queryContext.Object, commandContext.Object);

            //act
            var conversation = await conversationRepo.GetById(15, null);

            //assert
            Assert.IsNull(conversation);
        }
        [TestMethod]
        public async Task GetBySignalRConnectionIdWithActiveUser_Found()
        {
            //assemble
            var queryContext = AssembleMocks.GetQueryContext(true);
            var commandContext = AssembleMocks.GetCommandContext(true);
            var conversationRepo = new ConversationRepository(queryContext.Object, commandContext.Object);

            //act
            var conversationId = await conversationRepo.GetConversationIdBySignalRId("bc0444a5-c967-4d9f-86eb-0d9359d1bda6");

            //assert
            Assert.IsFalse(conversationId == 0);
        }
        [TestMethod]
        public async Task GetBySignalRConnectionIdWithActiveUser_NotFound()
        {
            //assemble
            var queryContext = AssembleMocks.GetQueryContext(true);
            var commandContext = AssembleMocks.GetCommandContext(true);
            var conversationRepo = new ConversationRepository(queryContext.Object, commandContext.Object);

            //act
            var conversationId = await conversationRepo.GetConversationIdBySignalRId("bc0444a5-c967-4d9f-86eb-0d9359d1bda3");

            //assert
            Assert.IsTrue(conversationId == 0);
        }
        [TestMethod]
        public async Task Create_NotNullTopic()
        {
            //assemble
            var queryContext = AssembleMocks.GetQueryContext();
            var commandContext = AssembleMocks.GetCommandContext();
            var conversationRepo = new ConversationRepository(queryContext.Object, commandContext.Object);

            //act
            var conversation = await conversationRepo.Create("New Topic");

            //assert
            Assert.IsNotNull(conversation);    
        }
        [TestMethod]
        public async Task Create_NullTopic()
        {
            //assemble
            var queryContext = AssembleMocks.GetQueryContext();
            var commandContext = AssembleMocks.GetCommandContext();
            var conversationRepo = new ConversationRepository(queryContext.Object, commandContext.Object);

            //act
            var conversation = await conversationRepo.Create(null);

            //assert
            Assert.IsNull(conversation);
        }
        [TestMethod]
        public async Task Persist()
        {
            //assemble
            var queryContext = AssembleMocks.GetQueryContext();
            var commandContext = AssembleMocks.GetCommandContext();
            var conversationRepo = new ConversationRepository(queryContext.Object, commandContext.Object);
            var conversation = await conversationRepo.GetById(1, null);
            string newTopic = Guid.NewGuid().ToString();

            //act
            conversation.Topic = newTopic;
            var response = await conversationRepo.Persist(conversation);

            //assert
            Assert.IsTrue(response.Topic == newTopic);
        }
    }
}
