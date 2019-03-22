using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESTest.DAL
{
    public partial class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessageId { get; set; }
        public long CreatedByUserId { get; set; }
        [StringLength(2000)]
        public string MessageText { get; set; }
        public int ConversationId { get; set; }
        public DateTime CreatedDateTime { get; set; }

        public virtual Conversation Conversation { get; set; }
    }
}
