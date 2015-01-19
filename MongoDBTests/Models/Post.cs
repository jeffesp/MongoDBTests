using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace MongoDBTests.Models
{
    public class Post
    {
        public Post()
        {
            PostedBy = new User();
            Comments = new List<Comment>();
        }

        public ObjectId Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public int Count { get; set; }
        public User PostedBy { get; set; }
        public DateTime PostedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        public List<Comment> Comments { get; set; }
    }
}