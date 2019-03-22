using System;
using System.ComponentModel.DataAnnotations;

namespace ESTest.Domain
{
    public interface IMessage
    {
        int ConversationId { get; set; }
        string CreatedByDisplayName { get; set; }
        long CreatedByUserId { get; set; }
        DateTime CreatedDateTime { get; set; }
        int MessageId { get; set; }
        [StringLength(2000)]
        string MessageText { get; set; }
        void SetCreatedUser(IUserSummary user);
    }
}