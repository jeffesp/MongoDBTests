using System;
using MongoDB.Bson;

namespace MongoDBTests.Models
{
    public class Post
    {
        public ObjectId Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public int Count { get; set;  }
        public DateTime PostDate { get; set; }
    }
}
