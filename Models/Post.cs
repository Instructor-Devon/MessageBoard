using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FakeReddit.Models
{
    // You can manally map your Model class to a table name like so:
    public class Post
    {
        [Key]
        public int PostId {get;set;}
        public int UserId {get;set;}
        [Required]
        [MaxLength(45, ErrorMessage="That way too long!")]
        public string Topic {get;set;}
        [Required]
        public string Content {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        // NAVIGATION PROPERTIES
        public User Creator {get;set;}
        public List<Vote> Votes {get;set;}       
    }
}