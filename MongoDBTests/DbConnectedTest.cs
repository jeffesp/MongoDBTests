using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using MongoDBTests.Models;

namespace MongoDBTests
{
    public class DbConnectedTest
    {
        private string connection = ConfigurationManager.ConnectionStrings["mongolab"].ConnectionString;

        protected MongoCollection<Post> postCollection;
        protected List<int> postCounts;

        [TestInitialize]
        public void TestInitialize()
        {
            var client = new MongoClient(connection);
            postCollection = client.GetServer().GetDatabase("something").GetCollection<Post>(Guid.NewGuid().ToString("N"));
            postCollection.RemoveAll();

            CreatePostCounts();
            CreatePosts();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            postCollection.Drop();
        }

        protected void CreatePostCounts()
        {
            postCounts = new List<int>();
            var random = new Random();
            for (int i = 0; i < 100; i++)
            {
                postCounts.Add(random.Next(0,100)); 
            }
        }

        public void CreatePosts()
        {
            List<Post> posts = new List<Post>();
            for (int i = 0; i < 100; i++)
            {
                posts.Add(new Post { Subject = "Post " + i, Body = "Post Body.", Count = postCounts[i]});
            }
            postCollection.InsertBatch(posts);
        }
    }
}