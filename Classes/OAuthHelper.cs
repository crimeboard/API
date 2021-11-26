using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Text;

//from: https://www.dotnetbull.com/2013/05/login-with-twitter-using-oauth.html


//  TODO - IMPORTANT: THIS CODE APPEARS INSECURE.  MAYBE DESIGNED FOR STAND-ALONE APP.  
//  =>  TODO: MAYBE TwitterController CHANGE WILL RESOLVE POSSIBLE ISSUE.
//      OTHERWISE, STORE ACCESS_TOKEN IN DATA DICTIONARY.


namespace BackendAPI.Controllers
{
    public class OAuthHelper
    {
        public OAuthHelper() 
        { 
        }

        static string oauth_consumer_key = "w5LCgZnBYBmEFGVCv99g";
        static string oauth_consumer_secret = "sO1R4AgXdI8h87f8yPAG7CZd5eNtUXOSWl7g2YaQPog";
        static string callbackUrl = "http://localhost:51846/Twitter/";   // TODO

        #region (Changable) Do Not Change It
        static string REQUEST_TOKEN = "https://api.twitter.com/oauth/request_token";
        static string AUTHORIZE = "https://api.twitter.com/oauth/authorize";
        static string ACCESS_TOKEN = "https://api.twitter.com/oauth/access_token";

        //public static string GetConsumerKey { get { return oauth_consumer_key; } }
        //public static string GetConsumerSecret { get { return oauth_consumer_secret; } }
        //public static string GetAUTHORIZE { get { return AUTHORIZE; } }

        public enum httpMethod
        {
            POST, GET
        }
        public string oauth_request_token { get; set; }
        public string oauth_access_token { get; set; }
        public string oauth_access_token_secret { get; set; }
        public string user_id { get; set; }
        public string screen_name { get; set; }
        public string oauth_error { get; set; }


        public string LoginWithTwitter()
        {
            HttpWebRequest request = FetchRequestToken(httpMethod.POST, oauth_consumer_key, oauth_consumer_secret);
            string result = getResponce(request);
            Dictionary<string, string> resultData = OAuthUtility.GetQueryParameters(result);
            if (resultData.Keys.Contains("oauth_token"))
            {
                this.oauth_request_token = resultData["oauth_token"];
                this.oauth_access_token_secret = resultData["oauth_token_secret"];
            }
            else
            {
                this.oauth_error = result;
            }

            return this.oauth_request_token;
        }

        public void GetUserTwAccessToken(string oauth_token, string oauth_verifier)
        {
            HttpWebRequest request = FetchAccessToken(httpMethod.POST, oauth_consumer_key, oauth_consumer_secret, oauth_token, oauth_verifier);
            string result = getResponce(request);

            Dictionary<string, string> resultData = OAuthUtility.GetQueryParameters(result);
            if (resultData.Keys.Contains("oauth_token"))
            {
                this.oauth_access_token = resultData["oauth_token"];
                this.oauth_access_token_secret = resultData["oauth_token_secret"];
                this.user_id = resultData["user_id"];
                this.screen_name = resultData["screen_name"];
            }
            else
                this.oauth_error = result;
        }

        public static string getResponce(HttpWebRequest request)
        {
            try
            {
                HttpWebResponse resp = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(resp.GetResponseStream());
                string result = reader.ReadToEnd();
                reader.Close();
                return result + "&status=200";
            }
            catch (Exception ex)
            {
                string statusCode = "";
                if (ex.Message.Contains("403"))
                    statusCode = "403";
                else if (ex.Message.Contains("401"))
                    statusCode = "401";
                return string.Format("status={0}&error={1}", statusCode, ex.Message);
            }
        }


        static HttpWebRequest FetchRequestToken(httpMethod method, string oauth_consumer_key, string oauth_consumer_secret)
        {
            string OutUrl = "";
            string OAuthHeader = OAuthUtility.GetAuthorizationHeaderForPost_OR_QueryParameterForGET(new Uri(REQUEST_TOKEN), callbackUrl, method.ToString(), oauth_consumer_key, oauth_consumer_secret, "", "", out OutUrl);

            if (method == httpMethod.GET)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(OutUrl + "?" + OAuthHeader);
                request.Method = method.ToString();
                return request;
            }
            else if (method == httpMethod.POST)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(OutUrl);
                request.Method = method.ToString();
                request.Headers["Authorization"] = OAuthHeader;
                return request;
            }
            else
                return null;
        }

        static HttpWebRequest FetchAccessToken(httpMethod method, string oauth_consumer_key, string oauth_consumer_secret, string oauth_token, string oauth_verifier)
        {
            string postData = "oauth_verifier=" + oauth_verifier;
            string AccessTokenURL = string.Format("{0}?{1}", ACCESS_TOKEN, postData);
            string OAuthHeader = OAuthUtility.GetAuthorizationHeaderForPost_OR_QueryParameterForGET(new Uri(AccessTokenURL), callbackUrl, method.ToString(), oauth_consumer_key, oauth_consumer_secret, oauth_token, "", out AccessTokenURL);

            if (method == httpMethod.GET)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(AccessTokenURL + "?" + OAuthHeader);
                request.Method = method.ToString();
                return request;
            }
            else if (method == httpMethod.POST)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(AccessTokenURL);
                request.Method = method.ToString();
                request.Headers["Authorization"] = OAuthHeader;

                byte[] array = Encoding.ASCII.GetBytes(postData);
                request.GetRequestStream().Write(array, 0, array.Length);
                return request;
            }
            else
                return null;
        }

        #endregion
    }
}