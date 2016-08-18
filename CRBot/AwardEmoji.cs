using System.Linq;
using System.Runtime.Serialization;
using NGitLab.Models;

namespace CRBot
{
    [DataContract]
    public class AwardEmoji
    {
        private static readonly string[] Downvotes = new[] {"-1", "thumbsdown"};
        private static readonly string[] Upvotes = new[] { "+1", "thumbsup" };

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "user")]
        public User User { get; set; }

        public bool IsDownvote => Downvotes.Contains(Name);
        public bool IsUpvote => Upvotes.Contains(Name);
    }
}