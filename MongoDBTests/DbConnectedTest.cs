using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBTests.Models;

namespace MongoDBTests
{
    public class DbConnectedTest
    {
        private const int postCount = 100;
        private const int commentCount = postCount*3;
        private const int userCount = postCount/10;

        private readonly string connection = ConfigurationManager.ConnectionStrings["mongolab"].ConnectionString;
        protected List<Comment> comments;

        protected MongoCollection<Post> postCollection;
        protected List<int> postCounts;
        protected List<Post> posts;
        protected List<User> users;

        [TestInitialize]
        public void TestInitialize()
        {
            var client = new MongoClient(connection);
            postCollection =
                client.GetServer().GetDatabase("something").GetCollection<Post>(Guid.NewGuid().ToString("N"));
            postCollection.RemoveAll();

            CreatePostCounts();
            CreateUsers();
            CreateComments();
            CreatePosts();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            postCollection.Drop();
        }

        private void CreateComments()
        {
            var random = new Random();
            comments = new List<Comment>();

            for (int i = 0; i < commentCount; i++)
            {
                comments.Add(new Comment
                {
                    Id = ObjectId.GenerateNewId(),
                    CommentText = String.Format("this is test comment #{0}", i),
                    PostedBy = users.Skip(random.Next(0, userCount)).Take(1).First()
                });
            }
        }

        private void CreateUsers()
        {
            var random = new Random();
            users = new List<User>();

            for (int i = 0; i < userCount; i++)
            {
                users.Add(new User
                {
                    Id = ObjectId.GenerateNewId(),
                    DisplayName = String.Format("user #{0}", i),
                    Username = String.Format("user{0}", i),
                    Email = String.Format("user{0}@example.com", i)
                });
            }
        }

        protected void CreatePostCounts()
        {
            postCounts = new List<int>();
            var random = new Random();
            for (int i = 0; i < 100; i++)
            {
                postCounts.Add(random.Next(0, 100));
            }
        }

        public void CreatePosts()
        {
            var random = new Random();
            posts = new List<Post>();
            for (int i = 0; i < 100; i++)
            {
                posts.Add(new Post
                {
                    Subject = "Post " + i,
                    Body = "Post Body.",
                    Count = postCounts[i],
                    Comments = comments.Skip(random.Next(0, commentCount - 10)).Take(random.Next(0, 9)).ToList(),
                    PostedBy = users.Skip(random.Next(0, userCount - 1)).Take(1).Single(),
                    PostedOn = DateTime.Now,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now
                });
            }
            postCollection.InsertBatch(posts);
        }
    }
}