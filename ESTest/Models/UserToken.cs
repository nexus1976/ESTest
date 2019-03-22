namespace ESTest.Models
{
    public class UserToken
    {
        public long UserId { get; set; }
        public string DisplayName { get; set; }
        public string OAuthToken { get; set; }
    }
}