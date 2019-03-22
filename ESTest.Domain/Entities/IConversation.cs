using System;
using System.Collections.Generic;

namespace ESTest.Domain
{
    public interface IConversation
    {
        IEnumerable<IUserSummary> ActiveUsers { get; }
        DateTime? CloseDateTime { get; set; }
        int ConversationId { get; set; }
        bool IsActive { get; set; }
        DateTime OpenDateTime { get; set; }
        string Topic { get; set; }

        void UserLeft(long userId);
        void UserLeft(IUserSummary user);
        void UserJoined(IUserSummary user);
    }
}