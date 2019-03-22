using System.Linq;
using ESTest.DAL;

namespace ESTest.Domain
{
    public partial class UserSummary : IUserSummary
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string DisplayName
        {
            get
            {
                return ((string.IsNullOrWhiteSpace(this.FirstName) ? string.Empty : this.FirstName.Trim() + " ")
                        + (string.IsNullOrWhiteSpace(this.LastName) ? string.Empty : this.LastName.Trim())).Trim();
            }
        }

        public UserSummary() { }
    }
}
