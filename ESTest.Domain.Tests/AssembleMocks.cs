using EntityFramework.Testing;
using ESTest.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using global::Moq;

namespace ESTest.Domain.Tests
{
    public static class AssembleMocks
    {
        
        public static Mock<DbSet<DAL.User>> GetUsers()
        {
            return new Mock<DbSet<DAL.User>>().SetupData(getUsersData());
        }
        public static Mock<DbSet<DAL.Conversation>> GetConversations(bool excludeConversationId1 = false)
        {
            var data = getConversationData(excludeConversationId1);
            var mock = new Mock<DbSet<DAL.Conversation>>().SetupData(data);
            mock.Setup(m => m.Attach(It.IsAny<DAL.Conversation>())).Callback<DAL.Conversation>(t =>
            {
                var entity = data.Where(x => x.ConversationId == t.ConversationId).FirstOrDefault();
                if(entity == null)
                {
                    data.Add(t);
                }
                else
                {
                    entity.CloseDateTime = t.CloseDateTime;
                    entity.IsActive = t.IsActive;
                    entity.OpenDateTime = t.OpenDateTime;
                    entity.Topic = t.Topic;
                }
            });
            mock.Setup(m => m.Add(It.IsAny<DAL.Conversation>())).Callback<DAL.Conversation>(t =>
            {
                if(t != null)
                {
                    var maxId = data.Select(d => d.ConversationId).Max();
                    maxId++;
                    t.ConversationId = maxId;
                    data.Add(t);
                }
            });
            mock.Setup(x => x.AsNoTracking()).Returns(mock.Object);
            return mock;
        }
        public static Mock<DbSet<DAL.ConversationUser>> GetConversationUsers(bool isUser1ActiveOnConversationId4 = false)
        {
            return new Mock<DbSet<DAL.ConversationUser>>().SetupData(getConversationUserData(isUser1ActiveOnConversationId4));
        }
        public static Mock<DbSet<DAL.Message>> GetMessages()
        {
            return new Mock<DbSet<DAL.Message>>().SetupData(getMessageData());
        }
        public static Mock<IQueryContext> GetQueryContext(bool isUser1ActiveOnConversationId4 = false)
        {
            var mockUsers = GetUsers();
            var mockConversations = GetConversations();
            var mockConversationUsers = GetConversationUsers(isUser1ActiveOnConversationId4);
            var mockMessages = GetMessages();

            Mock<IQueryContext> context = new Mock<IQueryContext>();
            context.Setup(m => m.Users).Returns(mockUsers.Object);
            context.Setup(m => m.Conversations).Returns(mockConversations.Object);
            context.Setup(m => m.ConversationUsers).Returns(mockConversationUsers.Object);
            context.Setup(m => m.Messages).Returns(mockMessages.Object);

            return context;
        }
        public static Mock<ICommandContext> GetCommandContext(bool isUser1ActiveOnConversationId4 = false)
        {
            var mockUsers = GetUsers();
            var mockConversations = GetConversations();
            var mockConversationUsers = GetConversationUsers(isUser1ActiveOnConversationId4);
            var mockMessages = GetMessages();

            Mock<ICommandContext> context = new Mock<ICommandContext>();
            context.Setup(m => m.Users).Returns(mockUsers.Object);
            context.Setup(m => m.Conversations).Returns(mockConversations.Object);
            context.Setup(m => m.ConversationUsers).Returns(mockConversationUsers.Object);
            context.Setup(m => m.Messages).Returns(mockMessages.Object);
            return context;
        }
        public static Mock<ICommandContext> GetCommandContextNoConversation1(bool isUser1ActiveOnConversationId4 = false)
        {
            var mockUsers = GetUsers();
            var mockConversations = GetConversations();
            var mockConversationUsers = GetConversationUsers(isUser1ActiveOnConversationId4);
            var mockMessages = GetMessages();

            Mock<ICommandContext> context = new Mock<ICommandContext>();
            context.Setup(m => m.Users).Returns(mockUsers.Object);
            context.Setup(m => m.Conversations).Returns(mockConversations.Object);
            context.Setup(m => m.ConversationUsers).Returns(mockConversationUsers.Object);
            context.Setup(m => m.Messages).Returns(mockMessages.Object);
            return context;
        }
        public static Mock<UserSummaryRepository> GetUserSummaryRepo(Mock<IQueryContext> mockQueryContext, Mock<ICommandContext> mockCommandContext)
        {
            return new Mock<UserSummaryRepository>(mockQueryContext.Object);
        }
        public static Mock<ConversationRepository> GetConversationRepo(Mock<IQueryContext> mockQueryContext, Mock<ICommandContext> mockCommandContext)
        {
            return new Mock<ConversationRepository>(mockQueryContext.Object, mockCommandContext.Object);
        }
        public static Mock<ConversationBoundedContext> GetConversationBoundedContext(Mock<ConversationRepository> mockConversationRepo, Mock<UserSummaryRepository> mockUserSummaryRepo)
        {
            return new Mock<ConversationBoundedContext>(mockConversationRepo.Object, mockUserSummaryRepo.Object);
        }

        private static List<DAL.User> getUsersData()
        {
            var data = new List<DAL.User>
            {
                new DAL.User
                {
                    Id = 1,
                    FirstName = "Daniel",
                    LastName = "Graham",
                    EmailAddress = "daniel.graham@whatever.com",
                    Password = "EKgqJ825D/8QFWQyzpoKmB1SsEdHUmul0yD0aHXf/6s=",
                    Salt = "KUnFRl48X0rIZduX39bcaBfS7mkHS12J",
                    Address1 = "101 Happy Street",
                    Address2 = null,
                    City = "Camarillo",
                    Province = "CA",
                    PostalCode = "93012",
                    MainPhone = "8055991234",
                    CreatedDate = DateTime.UtcNow
                },
                new DAL.User
                {
                    Id = 2,
                    FirstName = "Mary",
                    LastName = "Graham",
                    EmailAddress = "mary.graham@whatever.com",
                    Password = "EKgqJ825D/8QFWQyzpoKmB1SsEdHUmul0yD0aHXf/6s=",
                    Salt = "KUnFRl48X0rIZduX39bcaBfS7mkHS12J",
                    Address1 = "101 Happy Street",
                    Address2 = null,
                    City = "Camarillo",
                    Province = "CA",
                    PostalCode = "93012",
                    MainPhone = "8055994321",
                    CreatedDate = DateTime.UtcNow
                }
            };
            return data;
        }
        private static List<DAL.Conversation> getConversationData(bool excludeConversationId1 = false)
        {
            var data = new List<DAL.Conversation>
            {
                new DAL.Conversation
                {
                    ConversationId = 4,
                    Topic = "Active Chat",
                    IsActive = true,
                    OpenDateTime = DateTime.UtcNow,
                    CloseDateTime = null
                },
                new DAL.Conversation
                {
                    ConversationId = 5,
                    Topic = "Test Chat Room",
                    IsActive = true,
                    OpenDateTime = DateTime.UtcNow,
                    CloseDateTime = null
                }
            };
            if(!excludeConversationId1)
            {
                data.Add(new DAL.Conversation
                {
                    ConversationId = 1,
                    Topic = "New Topic",
                    IsActive = true,
                    OpenDateTime = DateTime.UtcNow,
                    CloseDateTime = null
                });
            }
            return data;
        }
        public static List<DAL.ConversationUser> getConversationUserData(bool isUser1ActiveOnConversationId4 = false)
        {
            var data = new List<DAL.ConversationUser>
            {
                new DAL.ConversationUser
                {
                    ConversationUserId = 45,
                    UserId = 1,
                    ConversationId = 4,
                    JoinedDate = DateTime.UtcNow,
                    LeftDate = isUser1ActiveOnConversationId4 ? null : (DateTime?)DateTime.UtcNow,
                    SignalRConnectionId = "bc0444a5-c967-4d9f-86eb-0d9359d1bda6"
                },
                new DAL.ConversationUser
                {
                    ConversationUserId = 46,
                    UserId = 2,
                    ConversationId = 4,
                    JoinedDate = DateTime.UtcNow,
                    LeftDate = DateTime.UtcNow,
                    SignalRConnectionId = "96ae017d-1cc3-4159-864a-4619272a00b7"
                },
                new DAL.ConversationUser
                {
                    ConversationUserId = 47,
                    UserId = 1,
                    ConversationId = 1,
                    JoinedDate = DateTime.UtcNow,
                    LeftDate = null,
                    SignalRConnectionId = "630A9A83-9233-497E-A943-44F8FC3C6F5D"
                }
            };
            return data;
        }
        public static List<DAL.Message> getMessageData()
        {
            var data = new List<DAL.Message>
            {
                new DAL.Message
                {
                    MessageId = 1,
                    CreatedByUserId = 1,
                    MessageText = "Hi mom!",
                    ConversationId = 4,
                    CreatedDateTime = DateTime.UtcNow
                },
                new DAL.Message
                {
                    MessageId = 2,
                    CreatedByUserId = 2,
                    MessageText = "Yo!",
                    ConversationId = 4,
                    CreatedDateTime = DateTime.UtcNow
                }
            };
            return data;
        }
    }
}
