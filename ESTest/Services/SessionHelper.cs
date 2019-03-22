using ESTest.Models;
using System.Web;

namespace ESTest.Services
{
    public class SessionHelper
    {
        public static UserToken UserTokenGet(HttpSessionStateBase session)
        {
            var userToken = session["current_user"];
            if (userToken != null && userToken is UserToken)
                return userToken as UserToken;
            else
                return null;
        }
        public static void UserTokenSet(HttpSessionStateBase session, UserToken userToken)
        {
            session["current_user"] = userToken;
        }

        public static string UserDisplayNameGet(HttpSessionStateBase session)
        {
            var displayName = session["current_user_displayname"];
            if (displayName != null)
                return displayName.ToString();
            else
                return null;
        }
        public static void UserDisplayNameSet(HttpSessionStateBase session, string userDisplayName)
        {
            session["current_user_displayname"] = userDisplayName;
        }
    }
}