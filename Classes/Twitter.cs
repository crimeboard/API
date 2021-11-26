using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

//From: https://gist.github.com/anova/9674023786ab31df43c07fd9bd53bc39

//Use instead: https://github.com/rhargreaves/oauth-dotnetcore

namespace BackendAPI.Classes
{
    public class Twitter
    {
        public string BearerToken { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }

        public Twitter(string BearerToken)
        {
            this.BearerToken = BearerToken;
        }

        public Twitter(string ConsumerKey, string ConsumerSecret)
        {
            this.ConsumerKey = ConsumerKey;
            this.ConsumerSecret = ConsumerSecret;
            this.setBearerToken();
        }

        private void setBearerToken()
        {
            //https://dev.twitter.com/oauth/application-only
            //Step 1
            string strBearerRequest = HttpUtility.UrlEncode(this.ConsumerKey) + ":" + HttpUtility.UrlEncode(this.ConsumerSecret);
            //http://stackoverflow.com/a/11743162
            strBearerRequest = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(strBearerRequest));

            //Step 2
            WebRequest request = WebRequest.Create("https://api.twitter.com/oauth2/token");
            request.Headers.Add("Authorization", "Basic " + strBearerRequest);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";

            string strRequestContent = "grant_type=client_credentials";
            byte[] bytearrayRequestContent = System.Text.Encoding.UTF8.GetBytes(strRequestContent);
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytearrayRequestContent, 0, bytearrayRequestContent.Length);
            requestStream.Close();

            string responseJson = string.Empty;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = response.GetResponseStream();
                responseJson = new StreamReader(responseStream).ReadToEnd();
            }


            JObject jobjectResponse = JObject.Parse(responseJson);

            this.BearerToken = jobjectResponse["access_token"].ToString();
        }

        private string GetTweetIdFromUrl(string strTweetUrl)
        {
            string[] strLinkParts = strTweetUrl.Split(new[] { '/' });
            return strLinkParts[strLinkParts.Length - 1];
        }

        public TwitterTweetDetails getTweetDetails(string strTweetUrl)
        {
            string strTweetId = this.GetTweetIdFromUrl(strTweetUrl);
            TwitterTweetDetails d = new TwitterTweetDetails();
            d.TweetLink = strTweetUrl;
            d.TweetId = strTweetId;
            JObject jTweet = this.GetTweetDetails(strTweetId);
            JObject jUser = null;
            string strUserId = string.Empty;
            if (jTweet["user"] != null && jTweet["user"]["id_str"] != null)
            {
                strUserId = jTweet["user"]["id_str"].ToString();
                d.UserId = strUserId;
                jUser = GetUserDetails(strUserId);
            }
            if (jUser != null)
            {
                if (jUser["name"] != null) d.AuthorName = jUser["name"].ToString();
                if (jUser["screen_name"] != null) d.AuthorNick = jUser["screen_name"].ToString();
                if (jUser["profile_image_url"] != null) d.AuthorPhoto = jUser["profile_image_url"].ToString().Replace("_normal.", ".");
            }
            if (jTweet["text"] != null)
            {
                d.Content = Regex.Replace(jTweet["text"].ToString(), @"\p{Cs}", string.Empty);//remove emoji
            }
            /*
            if (jTweet["entities"] != null
                && jTweet["entities"]["media"] != null
                && jTweet["entities"]["media"][0] != null
                && jTweet["entities"]["media"][0]["media_url"] != null)
            {
                d.Picture = jTweet["entities"]["media"][0]["media_url"].ToString();
            }
            */
            return d;
        }

        public JObject TwitterApiGetCall(string address)
        {
            WebRequest request = WebRequest.Create(address);
            request.Headers.Add("Authorization", "Bearer " + this.BearerToken);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            string responseJson = string.Empty;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = response.GetResponseStream();
                responseJson = new StreamReader(responseStream).ReadToEnd();
            }

            JObject jobjectResponse = JObject.Parse(responseJson);
            return jobjectResponse;
        }

        public JObject GetTweetDetails(string tweet)
        {
            string address = string.Format("https://api.twitter.com/1.1/statuses/show/{0}.json?trim_user=true", tweet);
            return TwitterApiGetCall(address);
        }

        public JObject GetUserDetails(string user_id)
        {
            string address = string.Format("https://api.twitter.com/1.1/users/show.json?user_id={0}&include_entities=false", user_id);
            return TwitterApiGetCall(address);
        }
    }
}
