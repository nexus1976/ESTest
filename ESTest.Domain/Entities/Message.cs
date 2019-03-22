using System;
using System.Data.Entity;
using System.Linq;
using ESTest.DAL;
using System.ComponentModel.DataAnnotations;

namespace ESTest.Domain
{
    public partial class Message : IMessage
    {
        public int MessageId { get; set; }
        public long CreatedByUserId { get; set; }
        public string CreatedByDisplayName { get; set; }
        [StringLength(2000)]
        public string MessageText { get; set; }
        public int ConversationId { get; set; }
        public DateTime CreatedDateTime { get; set; }

        public Message() { }
        public Message(IUserSummary user)
        {
            this.CreatedByUserId = user == null ? 0 : user.Id;
            this.CreatedByDisplayName = user?.DisplayName;
            
        }
        public void SetCreatedUser(IUserSummary user)
        {
            this.CreatedByUserId = user == null ? 0 : user.Id;
            this.CreatedByDisplayName = user?.DisplayName;
        }
    }
}
