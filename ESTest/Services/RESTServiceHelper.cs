using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ESTest.Services
{
    public class RESTServiceHelper
    {
        private static RESTServiceHelper _instance = null;
        public static RESTServiceHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RESTServiceHelper();
                }
                return _instance;
            }
        }

        private RESTServiceHelper()
        { }

        public async Task<object> Post(string url, string payload)
        {
            return await Post(url, payload, null);
        }
        public async Task<object> Post(string url, string payload, string token)
        {
            object jsonResponse = null;
            if (url.Trim().ToLower().StartsWith(@"https"))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            }
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage())
                {
                    request.RequestUri = new Uri(url);
                    request.Method = HttpMethod.Post;
                    request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        request.Headers.Add("Authorization", "Bearer " + token);
                    }
                    request.Content = new StringContent(payload);
                    using (var response = await client.SendAsync(request))
                    {
                        if (response != null)
                        {
                            string responseString = await response.Content.ReadAsStringAsync();
                            if (!string.IsNullOrWhiteSpace(responseString))
                            {
                                jsonResponse = JsonConvert.DeserializeObject(responseString);
                            }
                        }
                    }
                }
            }
            return Task.FromResult(jsonResponse);
        }
        public async Task<object> Get(string url, string querystring)
        {
            return await Get(url, querystring, null);
        }
        public async Task<object> Get(string url, string querystring, string token)
        {
            object jsonResponse = null;
            if (url.Trim().ToLower().StartsWith(@"https"))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            }
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage())
                {
                    string fullUrl = url + (string.IsNullOrWhiteSpace(querystring) ? string.Empty : "?" + querystring);
                    request.RequestUri = new Uri(fullUrl);
                    request.Method = HttpMethod.Get;
                    request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        request.Headers.Add("Authorization", "Bearer " + token);
                    }
                    using (var response = await client.SendAsync(request))
                    {
                        if (response != null)
                        {
                            string responseString = await response.Content.ReadAsStringAsync();
                            if (!string.IsNullOrWhiteSpace(responseString))
                            {
                                jsonResponse = JsonConvert.DeserializeObject(responseString);
                            }
                        }
                    }
                }
            }
            return Task.FromResult(jsonResponse);
        }

        public string GetSystemAPIUrl()
        {
            string apiURL = ConfigurationManager.AppSettings["API.Endpoint"];
            if (string.IsNullOrWhiteSpace(apiURL))
            {
                throw new MissingMemberException("The API.Endpoint was not found when attempting to communicate with the API.");
            }
            apiURL = apiURL.Trim();
            if (!apiURL.EndsWith(@"/"))
                apiURL += @"/";
            return apiURL;
        }
    }
}