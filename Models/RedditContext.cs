using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FakeReddit.Models
{
    public class RedditContext : DbContext
    {
        public RedditContext(DbContextOptions options) : base(options) {}
        
        public DbSet<Post> Posts {get;set;}
        public DbSet<User> Users {get;set;}
        public DbSet<Vote> Votes {get;set;}
    }
}