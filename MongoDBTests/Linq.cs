using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver.Linq;
using MongoDBTests.Models;

namespace MongoDBTests
{
    [TestClass]
    public class Linq : DbConnectedTest
    {
        [TestMethod]
        public void get_first_or_default_gets_first()
        {
            Post post = postCollection.AsQueryable().FirstOrDefault(p => p.Subject.Equals("Post 10"));
            Assert.IsNotNull(post);
            Assert.AreEqual("Post 10", post.Subject);
        }

        [TestMethod]
        public void get_first_or_default_gets_default_when_not_found()
        {
            Post post = postCollection.AsQueryable().FirstOrDefault(p => p.Subject.Equals("Post Not Found"));
            Assert.IsNull(post);
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void get_single_failure()
        {
            Post _ = postCollection.AsQueryable().Single(p => p.Subject.StartsWith("Post"));
        }

        [TestMethod]
        public void where_query_by_subject_has_results()
        {
            IQueryable<Post> posts = postCollection.AsQueryable().Where(p => p.Subject.StartsWith("Post 9"));
            Assert.AreEqual(11, posts.Count());
        }

        [TestMethod]
        public void where_subclass_is_queried()
        {
            IQueryable<Post> user5Posts =
                postCollection.AsQueryable().Where(p => p.PostedBy.Email == "user5@example.com");
            // Hacky use of `.Count`
            Assert.AreEqual(posts.Where(p => p.PostedBy.Email == "user5@example.com").ToList().Count,
                user5Posts.ToList().Count);
        }

        [TestMethod]
        [Ignore] // SelectMany isn't supported - maybe MapReduce is what is needed here? Or could we use a GroupBy?
        public void select_many_to_get_from_member_collection()
        {
            IQueryable<Comment> user2Comments =
                postCollection.AsQueryable()
                    .SelectMany(p => p.Comments.Where(c => c.PostedBy.Username == "user2@example.com"));
        }

        [TestMethod]
        public void order_posts_by_view_count_descending()
        {
            List<Post> posts = postCollection.AsQueryable().OrderByDescending(p => p.Count).ToList();
            for (int i = 0; i < posts.Count - 1; i++)
            {
                Assert.IsTrue(posts[i].Count >= posts[i + 1].Count, "{0} < {1} when should be ordered greater",
                    posts[i].Count, posts[i + 1].Count);
            }
        }

        [TestMethod]
        public void paged_query_returns_slice_of_collection()
        {
            List<Post> posts = postCollection.AsQueryable().OrderBy(p => p.Id).Skip(10).Take(10).ToList();
            Assert.AreEqual(10, posts.Count);
            Assert.AreEqual("Post 10", posts[0].Subject);
            Assert.AreEqual("Post 19", posts[9].Subject);
        }
    }
}