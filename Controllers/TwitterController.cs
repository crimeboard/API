using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Microsoft.AspNetCore.Authentication;
//using BackendAPI.Classes;
using OAuth;

/*******************************************
 * Author: https://twitter.com/kenmurrayx4 *
 * Date: 2020/21                           *
 * Note: Not currently used                *
 ******************************************/


/************************************************************************************
 * IMPORTANT  IMPORTANT  IMPORTANT  IMPORTANT  IMPORTANT                            *
 *                                                                                  *
 * Twitter Auth may not be best - would be convenient, but it would                 *
 * also grant unfettered access to that users twitter account - or                  *
 * maybe I misunderstand the example code I read.                                   *
 * OAuthHelper needs fixing + it's insecure.                                        *
 * 1. Needs to be fixed to get working e.g. 403 error from twitter.                 *
 * 2. Login details need to be per session e.g. if one user is                      *
 *      authenticated will another user also share their login???                   *
 *                                                                                  *
 * Alternatively, just write my own salted password code to store                   *
 * user credentials securely in database?                                           *
 *                                                                                  *
 * Good tutorial:                                                                   *
 * https://www.quod.ai/post/how-to-integrate-twitter-login-api-into-your-react-app  *
 *                                                                                  *
 ***********************************************************************************/

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TwitterController : ControllerBase
    {
//        private readonly ILogger<TwitterController> _logger;
//        private readonly IAuthenticationSchemeProvider authenticationSchemeProvider;
        private readonly string APP_CONSUMER_KEY = "XXXXXXXXXXXXX";
        private readonly string APP_CONSUMER_SECRET = "XXXXXXXXXXXX";

        //        private readonly string request_token_url = "https://api.twitter.com/oauth/request_token";
        //        private readonly string access_token_url = "https://api.twitter.com/oauth/access_token";
        //        private readonly string authorize_url = "https://api.twitter.com/oauth/authorize";
        //        private readonly string show_user_url = "https://api.twitter.com/1.1/users/show.json";
        //        private readonly string get_username_url = "https://api.twitter.com/2/users/";

        //public TwitterController(IAuthenticationSchemeProvider authenticationSchemeProvider, ILogger<TwitterController> logger)
        //{
        //    _logger = logger;
        //    this.authenticationSchemeProvider = authenticationSchemeProvider;
        //}

        //        private OAuthHelper oauthhelper = new OAuthHelper();

        //public TwitterController(ILogger<TwitterController> logger)
        //{
        //    _logger = logger;
        //}

        //User is not yet Authorised - Stage 1: request token from Twitter


        //[HttpGet]
        //public string Get()
        //{
        //    return "";      //TODO: temporary, saves me from commenting all the code below..



        //var allSchemeProvider = (await authenticationSchemeProvider.GetAllSchemesAsync())
        //       .Select(n => n.DisplayName).Where(n => !String.IsNullOrEmpty(n));

        //ChallengeResult result = Challenge(new AuthenticationProperties { RedirectUri = "/" });
        //return result.ToString();

        //Twitter _twitter = new Twitter("igxbx3Z9QpdmxBJQ4P0yvTBhs", "RRrjnICokR1DJrUPkwuhzIRSrJVoSrrORyARJJ5Zlg4C5zgdGP");
        //return "";

        //// Creating a new instance with a helper method
        //OAuthRequest client = OAuthRequest.ForRequestToken(APP_CONSUMER_KEY, APP_CONSUMER_SECRET);
        //client.RequestUrl = request_token_url;      // "http://twitter.com/oauth/request_token";

        //// For HTTP header authorization
        //string auth = client.GetAuthorizationHeader();
        //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(client.RequestUrl);

        //request.Headers.Add("Authorization", auth);
        //HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        //return response.StatusDescription;




        //give you both a request token (oauth_token) and a request token secret (not visible - resides in the backend)

        /*
        Request includes:
        oauth_callback="https%3A%2F%2FyourCallbackUrl.com"
        oauth_consumer_key="cChZNFj6T5R0TigYB9yd1w"
        */


        //string requestToken = oauthhelper.LoginWithTwitter();
        //            return requestToken;

        /*
        //var auth = new MvcSignInAuthorizer
        var auth = new MvcAuthorizer
        {
            CredentialStore = new SessionStateCredentialStore(HttpContext.Session)
            {
                ConsumerKey = configuration["Twitter:ConsumerKey"],
                ConsumerSecret = configuration["Twitter:ConsumerSecret"]
            }
        };

        string twitterCallbackUrl = Request.GetDisplayUrl().Replace("Begin", "Complete");
        return await auth.BeginAuthorizationAsync(new Uri(twitterCallbackUrl));

        //if (string.IsNullOrEmpty(oauthhelper.oauth_error))
        //Response.Redirect(oauthhelper.GetAuthorizeUrl(requestToken));
        //else
        //Response.Write(oauthhelper.oauth_error);
        */

        /*
        int timestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        string nonce = Convert.ToBase64String(Encoding.ASCII.GetBytes(timestamp.ToString()));

        string consumerKey = twitterConfiguration.ConsumerKey;
        string oAuthCallback = twitterConfiguration.OAuthCallback;

        string requestString =
            twitterConfiguration.EndpointUrl +
            OAuthRequestTokenRoute;

        string parameterString =
            $"oauth_callback={WebUtility.UrlEncode(twitterConfiguration.OAuthCallback)}&" +
            $"oauth_consumer_key={WebUtility.UrlEncode(twitterConfiguration.ConsumerKey)}&" +
            $"oauth_nonce={WebUtility.UrlEncode(nonce)}&" +
            $"oauth_signature_method={WebUtility.UrlEncode(OAuthSigningAlgorithm)}&" +
            $"oauth_timestamp={WebUtility.UrlEncode(timestamp.ToString())}&" +
            $"oauth_version={WebUtility.UrlEncode("1.0")}";

        string signatureBaseString =
            "POST&" +
            WebUtility.UrlEncode(requestString) +
            "&" +
            WebUtility.UrlEncode(parameterString);

        string signingKey =
            WebUtility.UrlEncode(twitterConfiguration.ConsumerSecret) +
            "&";

        byte[] signatureBaseStringBytes = Encoding.ASCII.GetBytes(signatureBaseString);
        byte[] signingKeyBytes = Encoding.ASCII.GetBytes(signingKey);

        HMACSHA1 hmacSha1 = new HMACSHA1(signingKeyBytes);
        byte[] signature = hmacSha1.ComputeHash(signatureBaseStringBytes);
        string base64Signature = Convert.ToBase64String(signature);

        string authenticationHeaderValue =
            $"oauth_nonce=\"{WebUtility.UrlEncode(nonce)}\", " +
            $"oauth_callback=\"{WebUtility.UrlEncode(twitterConfiguration.OAuthCallback)}\", " +
            $"oauth_signature_method=\"{WebUtility.UrlEncode(OAuthSigningAlgorithm)}\", " +
            $"oauth_timestamp=\"{WebUtility.UrlEncode(timestamp.ToString())}\", " +
            $"oauth_consumer_key=\"{WebUtility.UrlEncode(twitterConfiguration.ConsumerKey)}\", " +
            $"oauth_signature=\"{WebUtility.UrlEncode(base64Signature)}\", " +
            $"oauth_version=\"{WebUtility.UrlEncode("1.0")}\"";

        HttpRequestMessage request = new HttpRequestMessage();
        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri(
            baseUri: new Uri(twitterConfiguration.EndpointUrl),
            relativeUri: OAuthRequestTokenRoute);
        request.Headers.Authorization = new AuthenticationHeaderValue("OAuth",
            authenticationHeaderValue);

        HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(request);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            string response = await httpResponseMessage.Content.ReadAsStringAsync();
            return response;
        }
        else
        {
            return null;
        }
        */

        //_logger.LogInformation("requestToken="+ requestToken);

        //           return "";
        //}

        [HttpGet("{id}")]
        public User Get(string id)      //Returning database ID and social media UserID would be better
        {
            var context = new CrimeBoardContext();
            
            /*
            User returnUser = null;

            List<CbUser> listUsers = context.CbUsers.ToList();

            foreach (var user in listUsers)
            {
                if (user.TwitterId.Trim() == id.Trim()) {   //.Equals(id.Trim())) {
                    returnUser = new User()
                    {
                        TwitterID = user.TwitterId,
                        ID = user.Id,
                        ParentID = user.ParentId,
                        TwitterHandle = user.TwitterHandle,
                        GoogleHandle = user.GoogleHandle,
                        AutoApprove = user.AutoApprove,
                        Active = user.Active ?? false

                        // No need to send this..
                        //GuidVerificationCode = user.GuidVerificationCode,
                        //Creator = user.Creator,
                        //Created = user.Created,
                        //UpdatedBy = user.UpdatedBy,
                        //LastUpdated = user.LastUpdated
                    };
                }
            }
            
            return returnUser;    // NotFound();
            */
            //CbUser userXS = test.FirstOrDefault(x => x.TwitterId == id);

            // Bizarre that this doesn't work..
            var users = (from user in context.CbUsers
                            where user.TwitterId.Equals(id)
                            select new User
                            {
                                TwitterID = user.TwitterId,
                                ID = user.Id,
                                ParentID = user.ParentId,
                                TwitterHandle = user.TwitterHandle,
                                GoogleHandle = user.GoogleHandle,
                                AutoApprove = user.AutoApprove,
                                GuidVerificationCode = user.GuidVerificationCode,
                                Active = user.Active ?? false

                                // No need to send this..
                                //Creator = user.Creator,
                                //Created = user.Created,
                                //UpdatedBy = user.UpdatedBy,
                                //LastUpdated = user.LastUpdated
                            }).ToList();    //.FirstOrDefault();
            
            Response.Headers.Add("X-Total-Count", users.Count == 1 ? "1" : "0");
            if (users.Count == 1)
            {
                return users[0] as User;
            }
            else
            {
                return null;    // NotFound();
            }
        }

        //See: https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-5.0&tabs=visual-studio
        // PUT: User/5
        [HttpPut("{id}")]
        public /*async*/ Task<IActionResult> Put(int id, PutDataUser putDataUser)
        {
            return null;
        }

        
 
        //// POST: api/Products
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for
        //// more details see https://aka.ms/RazorPagesCRUD.
        //[HttpPost]
        ////public async Task<IActionResult> Post(PostDataUser postDataUser)
        //public IActionResult Post(string postDataUser)
        //{
        //    _logger.LogInformation("stage 1 data=" + postDataUser);
        //    return null;    //TODO
        //}        
        
    }
}
