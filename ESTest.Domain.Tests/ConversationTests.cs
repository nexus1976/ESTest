using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Linq;

namespace ESTest.Domain.Tests
{
    [TestClass]
    public class ConversationTests
    {
        [TestMethod]
        public async Task UserJoined_UserNotNull()
        {
            //assemble
            var queryContext = AssembleMocks.GetQueryContext();
            var commandContext = AssembleMocks.GetCommandContext();
            var userSummaryRepo = new UserSummaryRepository(queryContext.Object);
            var conversationRepo = new ConversationRepository(queryContext.Object, commandContext.Object);
            var messageRepo = new MessageRepository(queryContext.Object, commandContext.Object);
            var conversationContext = new ConversationBoundedContext(conversationRepo, userSummaryRepo, messageRepo);
            var conversation = await conversationContext.GetById(1);
            var userSummary = await userSummaryRepo.GetById(2);

            //act
            conversation.UserJoined(userSummary);

            //assert
            Assert.IsNotNull(userSummary);
            Assert.IsTrue(conversation.ActiveUsers.Count() == 2);
        }
        [TestMethod]
        public async Task UserJoined_UserNull()
        {
            //assemble
            var queryContext = AssembleMocks.GetQueryContext();
            var commandContext = AssembleMocks.GetCommandContext();
            var userSummaryRepo = new UserSummaryRepository(queryContext.Object);
            var conversationRepo = new ConversationRepository(queryContext.Object, commandContext.Object);
            var messageRepo = new MessageRepository(queryContext.Object, commandContext.Object);
            var conversationContext = new ConversationBoundedContext(conversationRepo, userSummaryRepo, messageRepo);
            var conversation = await conversationContext.GetById(5);
            var userSummary = await userSummaryRepo.GetById(5);

            //act
            conversation.UserJoined(userSummary);

            //assert
            Assert.IsNull(userSummary);
            Assert.IsTrue(conversation.ActiveUsers.Count() == 0);
        }
        [TestMethod]
        public async Task UserLeft_UserNotNull()
        {
            //assemble
            var queryContext = AssembleMocks.GetQueryContext();
            var commandContext = AssembleMocks.GetCommandContext();
            var userSummaryRepo = new UserSummaryRepository(queryContext.Object);
            var conversationRepo = new ConversationRepository(queryContext.Object, commandContext.Object);
            var messageRepo = new MessageRepository(queryContext.Object, commandContext.Object);
            var conversationContext = new ConversationBoundedContext(conversationRepo, userSummaryRepo, messageRepo);
            var conversation = await conversationContext.GetById(4);
            var userSummary = await userSummaryRepo.GetById(1);
            conversation.UserJoined(userSummary); //workaround since can't seem to mock a working Include()
            Assert.IsTrue(conversation.ActiveUsers.Count() == 1);

            //act
            conversation.UserLeft(userSummary);

            //assert
            Assert.IsNotNull(userSummary);
            Assert.IsTrue(conversation.ActiveUsers.Count() == 0);
        }
        [TestMethod]
        public async Task UserLeft_UserNull()
        {
            //assemble
            var queryContext = AssembleMocks.GetQueryContext();
            var commandContext = AssembleMocks.GetCommandContext();
            var userSummaryRepo = new UserSummaryRepository(queryContext.Object);
            var conversationRepo = new ConversationRepository(queryContext.Object, commandContext.Object);
            var messageRepo = new MessageRepository(queryContext.Object, commandContext.Object);
            var conversationContext = new ConversationBoundedContext(conversationRepo, userSummaryRepo, messageRepo);
            var conversation = await conversationContext.GetById(4);
            var userSummary = await userSummaryRepo.GetById(1);
            var userSummaryTest = await userSummaryRepo.GetById(5);
            conversation.UserJoined(userSummary); //workaround since can't seem to mock a working Include()
            Assert.IsTrue(conversation.ActiveUsers.Count() == 1);

            //act
            conversation.UserLeft(userSummaryTest);

            //assert
            Assert.IsNull(userSummaryTest);
            Assert.IsTrue(conversation.ActiveUsers.Count() == 1);
        }
        [TestMethod]
        public async Task UserLeft_UserIdFound()
        {
            //assemble
            var queryContext = AssembleMocks.GetQueryContext();
            var commandContext = AssembleMocks.GetCommandContext();
            var userSummaryRepo = new UserSummaryRepository(queryContext.Object);
            var conversationRepo = new ConversationRepository(queryContext.Object, commandContext.Object);
            var messageRepo = new MessageRepository(queryContext.Object, commandContext.Object);
            var conversationContext = new ConversationBoundedContext(conversationRepo, userSummaryRepo, messageRepo);
            var conversation = await conversationContext.GetById(4);
            var userSummary = await userSummaryRepo.GetById(1);
            conversation.UserJoined(userSummary); //workaround since can't seem to mock a working Include()
            Assert.IsTrue(conversation.ActiveUsers.Count() == 1);

            //act
            conversation.UserLeft(1);

            //assert
            Assert.IsTrue(conversation.ActiveUsers.Count() == 0);
        }
        [TestMethod]
        public async Task UserLeft_UserIdNotFound()
        {
            //assemble
            var queryContext = AssembleMocks.GetQueryContext();
            var commandContext = AssembleMocks.GetCommandContext();
            var userSummaryRepo = new UserSummaryRepository(queryContext.Object);
            var conversationRepo = new ConversationRepository(queryContext.Object, commandContext.Object);
            var messageRepo = new MessageRepository(queryContext.Object, commandContext.Object);
            var conversationContext = new ConversationBoundedContext(conversationRepo, userSummaryRepo, messageRepo);
            var conversation = await conversationContext.GetById(4);
            var userSummary = await userSummaryRepo.GetById(1);
            conversation.UserJoined(userSummary); //workaround since can't seem to mock a working Include()
            Assert.IsTrue(conversation.ActiveUsers.Count() == 1);

            //act
            conversation.UserLeft(5);

            //assert
            Assert.IsTrue(conversation.ActiveUsers.Count() == 1);
        }
    }
}
