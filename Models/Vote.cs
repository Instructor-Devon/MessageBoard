using System;

namespace FakeReddit.Models
{
    public class Vote
    {
        public int VoteId {get;set;}
        public bool IsUpvote {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public int UserId {get;set;}
        public int PostId {get;set;}
        public User Voter {get;set;}
        public Post VotedPost {get;set;}
    }
}