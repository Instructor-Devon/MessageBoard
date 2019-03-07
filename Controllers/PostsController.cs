using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FakeReddit.Models;
using DbConnection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace FakeReddit.Controllers
{
    [Route("posts")]
    public class PostsController : Controller
    {
        private RedditContext dbContext;
        private bool InSession
        {
            get { return HttpContext.Session.GetInt32("UserId") != null; }
        }

        public int UserId
        {
            get { return (int)HttpContext.Session.GetInt32("UserId"); }
        }
        public PostsController(RedditContext context)
        {
            dbContext = context;
        }
        // localhost:5000/posts/
        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
         
            List<Post> model = dbContext.Posts
                // JOIN 
                .Include(p => p.Creator)
                .Include(p => p.Votes)
                .OrderBy(p => p.Topic)
                .ToList();

            ViewBag.UserId = UserId;

            return View(model);
        }
        // localhost:5000/posts/post/:id
        [HttpGet("{postId}")]
        public IActionResult PostDetails(int postId)
        {

            Post thePost = dbContext.Posts
            // include Votes
            // we also want include Votes User
                .Include(p => p.Votes)
                .ThenInclude(v => v.Voter)
                .FirstOrDefault(p => p.PostId == postId);

            // if no post was found
            if(thePost == null)
            {
                return RedirectToAction("Index");
            }
            return View(thePost);

        }

        [HttpGet("new")]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost("create")]
        public IActionResult Create(Post newPost)
        {
            if(!InSession)
                return RedirectToAction("Index", "Home");

            if(ModelState.IsValid)
            {
                // TODO: insert data into db...
                int userIdInSession = (int)HttpContext.Session.GetInt32("UserId");
                newPost.UserId = userIdInSession;

                dbContext.Posts.Add(newPost);
                dbContext.SaveChanges();

                
                return RedirectToAction("Index");
            }
            return View("New");
        }

        [HttpPost("update/{postId}")]
        public IActionResult Update(Post newPost, int postId)
        {
            if(ModelState.IsValid)
            {
                // first query for the thing to update!
                Post toUpdate = dbContext.Posts.FirstOrDefault(p => p.PostId == postId);

                // modify queried object with new values
                toUpdate.Content = newPost.Content;
                toUpdate.Topic = newPost.Topic;
                toUpdate.UpdatedAt = DateTime.Now;

                dbContext.SaveChanges();

                
                return RedirectToAction("Index");
            }
            return View("PostDetails", newPost);
        }

        [HttpGet("delete/{postId}")]
        public IActionResult Delete(int postId)
        {
            // query for post to delete
            Post toDelete = dbContext.Posts.FirstOrDefault(p => p.PostId == postId);

            dbContext.Posts.Remove(toDelete);
            dbContext.SaveChanges();

            return RedirectToAction("Index");

            
        }
        [HttpGet("vote/{postId}/{isUpvote}")]
        public IActionResult Vote(int postId, int isUpvote)
        // isUpvote: 1 == upvote, 0 == downvote
        {
            bool upvote = isUpvote == 1;
            Vote newVote = new Vote()
            {
                UserId = UserId,
                PostId = postId,
                IsUpvote = upvote,

            };

            dbContext.Votes.Add(newVote);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
