using ESTest.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace ESTest.Services
{
    public class UserService
    {
        private static UserService _instance = null;
        public static UserService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UserService();
                }
                return _instance;
            }
        }

        private UserService() { }

        public async Task<UserToken> GetUserToken(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("The userName parameter cannot be empty when getting the UserToken.");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("The password parameter cannot be empty when getting the UserToken.");
            }
            string apiURL = RESTServiceHelper.Instance.GetSystemAPIUrl();
            string access_token = await GetNewOAuthToken(apiURL, userName, password);
            UserToken userToken = await HydrateUserToken(apiURL, access_token, null);

            return userToken;
        }

        internal async Task<UserToken> HydrateUserToken(string apiURL, string oauthToken, UserToken userToken)
        {
            if (string.IsNullOrWhiteSpace(oauthToken) || string.IsNullOrWhiteSpace(apiURL)) return null;
            if (userToken == null)
                userToken = new UserToken();
            try
            {
                userToken.OAuthToken = oauthToken;
                string urlUser = apiURL + "api/SystemUser";
                var response = await RESTServiceHelper.Instance.Get(urlUser, null, userToken.OAuthToken);
                if (response != null)
                {
                    Task<object> task = (response as Task<object>);
                    if (task != null)
                    {
                        JObject jobject = task.Result as JObject;
                        if (jobject != null)
                        {
                            JToken jtoken = null;
                            if (jobject.TryGetValue("DisplayName", out jtoken))
                            {
                                userToken.DisplayName = jtoken.ToString();
                            }
                            if (jobject.TryGetValue("Id", out jtoken))
                            {
                                long id = 0;
                                if(long.TryParse(jtoken.ToString(), out id))
                                    userToken.UserId = id;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return userToken;
        }
        internal async Task<string> GetNewOAuthToken(string apiURL, string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(apiURL) || string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password)) return null;
            string urlToken = apiURL + "Token";
            string tokenPayload = string.Format("grant_type=password&username={0}&password={1}", userName, password);
            string access_token = null;
            var response = await RESTServiceHelper.Instance.Post(urlToken, tokenPayload);
            if (response != null)
            {
                Task<object> task = (response as Task<object>);
                if (task != null)
                {
                    JObject jobject = task.Result as JObject;
                    if (jobject != null)
                    {
                        JToken jtoken = null;
                        if (jobject.TryGetValue("access_token", out jtoken))
                        {
                            access_token = jtoken.ToString();
                        }
                    }
                }
            }
            return access_token;
        }
    }
}