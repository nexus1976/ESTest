namespace ESTest.Domain
{
    public interface IUserSummary
    {
        string DisplayName { get; }
        string EmailAddress { get; set; }
        string FirstName { get; set; }
        long Id { get; set; }
        string LastName { get; set; }
    }
}