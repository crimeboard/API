using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackendAPI.Classes
{
    [Serializable]
    public class TwitterTweetDetails
    {
        public string UserId { get; set; }
        public string TweetId { get; set; }
        public string AuthorNick { get; set; }
        public string AuthorName { get; set; }
        public string AuthorPhoto { get; set; }
        public string Content { get; set; }
        public string Picture { get; set; }
        public string TweetLink { get; set; }
    }
}
