using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDBTests.Models;

namespace MongoDBTests
{
    [TestClass]
    public class CRUD : DbConnectedTest
    {
        [TestMethod]
        public void insert_new_document_given_id()
        {
            WriteConcernResult result =
                postCollection.Insert(new Post
                {
                    Id = new ObjectId(),
                    Subject = "Test",
                    Body = "Test post body.",
                    Count = 0
                });
            Assert.IsTrue(result.Ok);
        }

        [TestMethod]
        public void insert_new_document_autoid()
        {
            var post = new Post {Subject = "Test", Body = "Test post body.", Count = 0};
            WriteConcernResult result = postCollection.Insert(post);
            Assert.IsTrue(result.Ok);
            Assert.IsNotNull(post.Id);
        }

        [TestMethod]
        public void update_existing_document_with_save()
        {
            var post = new Post {Subject = "Test", Body = "Test post body.", Count = 0};
            postCollection.Insert(post);
            post.Subject = String.Format("{0} - Updated {1}", post.Subject, DateTime.Now);
            WriteConcernResult result = postCollection.Save(post); // will update all fields even if not changed.
            Assert.IsTrue(result.Ok);
        }

        [TestMethod]
        public void update_existing_document_with_query()
        {
            var post = new Post {Subject = "Test", Body = "Test post body.", Count = 0};
            postCollection.Insert(post);
            IMongoQuery query = Query<Post>.EQ(e => e.Id, post.Id);
            UpdateBuilder<Post> update = Update<Post>.Set(e => e.Subject,
                String.Format("{0} - Updated {1}", post.Subject, DateTime.Now));
            WriteConcernResult result = postCollection.Update(query, update); // will update only the Subject field
            Assert.IsTrue(result.Ok);
        }

        [TestMethod]
        public void delete_document_by_id()
        {
            var post = new Post {Subject = "Test", Body = "Test post body.", Count = 0};
            postCollection.Insert(post); // note that the post should have the Id set at this point.
            IMongoQuery query = Query<Post>.EQ(e => e.Id, post.Id);
            WriteConcernResult result = postCollection.Remove(query, RemoveFlags.Single);
            Assert.IsTrue(result.Ok);
        }
    }
}