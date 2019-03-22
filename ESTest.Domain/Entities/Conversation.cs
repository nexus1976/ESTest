using ESTest.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ESTest.Domain
{
    public partial class Conversation : IConversation
    {
        public int ConversationId { get; set; }
        public string Topic { get; set; }
        public bool IsActive { get; set; }
        public DateTime OpenDateTime { set; get; }
        public DateTime? CloseDateTime { get; set; }
        public IEnumerable<IUserSummary> ActiveUsers { get; internal set; }

        public Conversation()
        {
            ActiveUsers = new List<IUserSummary>();
        }
        public Conversation(IEnumerable<IUserSummary> activeUsers)
        {
            this.ActiveUsers = activeUsers ?? new List<IUserSummary>();
        }

        public void UserJoined(IUserSummary user)
        {
            if (user != null && !this.ActiveUsers.Any(d => d.Id == user.Id))
            {
                var activeUsers = this.ActiveUsers.ToList();
                activeUsers.Add(user);
                this.ActiveUsers = activeUsers;
            }
        }
        public void UserLeft(IUserSummary user)
        {
            if(user != null)
            {
                var activeUsers = this.ActiveUsers.ToList();
                activeUsers.RemoveAll(d => d.Id == user.Id);
                this.ActiveUsers = activeUsers;
            }
        }
        public void UserLeft(long userId)
        {
            var activeUsers = this.ActiveUsers.ToList();
            activeUsers.RemoveAll(d => d.Id == userId);
            this.ActiveUsers = activeUsers;
        }
    }
}
