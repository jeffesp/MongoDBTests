using MongoDB.Bson;

namespace MongoDBTests.Models
{
    public class Comment
    {
        public Comment()
        {
            PostedBy = new User();
        }

        public ObjectId Id { get; set; }
        public int Level { get; set; }
        public User PostedBy { get; set; }
        public string CommentText { get; set; }
    }
}