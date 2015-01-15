﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDBTests.Models;

namespace MongoDBTests
{
    [TestClass]
    public class Linq : DbConnectedTest
    {

        [TestMethod]
        public void get_first_or_default()
        {
            var post = postCollection.AsQueryable().FirstOrDefault(p => p.Subject.Equals("Post 10"));
            Assert.IsNotNull(post);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void get_single_failure()
        {
            var _ = postCollection.AsQueryable().Single(p => p.Subject.StartsWith("Post"));
        }

        [TestMethod]
        public void where_query_by_subject_has_results()
        {
            var posts = postCollection.AsQueryable().Where(p => p.Subject.StartsWith("Post 9"));
            Assert.AreEqual(11, posts.Count());
        }
    }
}
