using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using MongoDBTests.Models;

namespace MongoDBTests
{
    [TestClass]
    public class Indexing : DbConnectedTest
    {
        [TestMethod]
        public void applies_index_on_single_object_prop()
        {
            var query = Query<Post>.GT(x => x.Count, postCounts.Count*0.9);

            BsonDocument explainNoIndex = postCollection.Find(query).Explain(verbose: true);
            Assert.AreEqual("BasicCursor", explainNoIndex["cursor"]);
            Assert.AreEqual(posts.Count, explainNoIndex["nscanned"]);

            IndexKeysBuilder<Post> index = new IndexKeysBuilder<Post>();
            index.Descending(p => p.Count); 
            WriteConcernResult createResult = postCollection.CreateIndex(index);
            Assert.IsTrue(createResult.Ok);

            BsonDocument explain = postCollection.Find(query).Explain(verbose: true);
            Assert.AreEqual("BtreeCursor Count_-1", explain["cursor"]);
            Assert.AreEqual(explain["nscanned"], (int)postCollection.Find(query).Count());
        }

        [TestMethod]
        public void applies_index_on_multiple_object_prop()
        {
            var query = Query<Post>.GT(x => x.Count, postCounts.Count*0.9);
            var sort = SortBy<Post>.Descending(p => p.PostedOn);

            BsonDocument explainNoIndex = postCollection.Find(query).SetSortOrder(sort).Explain(verbose: true);
            Assert.AreEqual("BasicCursor", explainNoIndex["cursor"]);
            Assert.AreEqual(posts.Count, explainNoIndex["nscanned"]);
            
            IndexKeysBuilder<Post> index = new IndexKeysBuilder<Post>();
            index.Descending(p => p.Count);
            index.Descending(p => p.PostedOn);
            WriteConcernResult createResult = postCollection.CreateIndex(index);
            Assert.IsTrue(createResult.Ok);

            BsonDocument explain = postCollection.Find(query).SetSortOrder(sort).Explain(verbose: true);
            Assert.AreEqual("BtreeCursor Count_-1_PostedOn_-1", explain["cursor"]);
            Assert.AreEqual((int)postCollection.Find(query).SetSortOrder(sort).Count(), explain["nscanned"]);
        }

        [TestMethod]
        public void applies_unique_index_on_single_object_prop()
        {
            var query = Query<Post>.EQ(x => x.Subject, "Post 10");

            BsonDocument explainNoIndex = postCollection.Find(query).Explain(verbose: true);
            Assert.AreEqual("BasicCursor", explainNoIndex["cursor"]);
            Assert.AreEqual(posts.Count, explainNoIndex["nscanned"]);

            IndexKeysBuilder<Post> index = new IndexKeysBuilder<Post>();
            index.Ascending(p => p.Subject);
            IndexOptionsBuilder<Post> options = new IndexOptionsBuilder<Post>();
            options.SetUnique(true);
            WriteConcernResult createResult = postCollection.CreateIndex(index, options);
            Assert.IsTrue(createResult.Ok);

            BsonDocument explain = postCollection.Find(query).Explain(verbose: true);
            Assert.AreEqual("BtreeCursor Subject_1", explain["cursor"]);
            Assert.AreEqual(explain["nscanned"], (int)postCollection.Find(query).Count());
        }

        [TestMethod]
        [ExpectedException(typeof(MongoWriteConcernException))]
        public void applies_unique_index_on_non_unique_field_fails()
        {
            IndexKeysBuilder<Post> index = new IndexKeysBuilder<Post>();
            index.Ascending(p => p.PostedBy.Email);
            IndexOptionsBuilder<Post> options = new IndexOptionsBuilder<Post>();
            options.SetUnique(true);
            WriteConcernResult createResult = postCollection.CreateIndex(index, options);
            Assert.IsTrue(createResult.Ok);
        }

        //TODO: what are the other index options?

        [TestMethod]
        public void applies_index_on_inner_object_prop()
        {
            var query = Query<Post>.EQ(x => x.PostedBy.Email, "user1@example.com");

            BsonDocument explainNoIndex = postCollection.Find(query).Explain(verbose: true);
            Assert.AreEqual("BasicCursor", explainNoIndex["cursor"]);
            Assert.AreEqual(posts.Count, explainNoIndex["nscanned"]);

            IndexKeysBuilder<Post> index = new IndexKeysBuilder<Post>();
            index.Ascending(p => p.PostedBy.Email); 
            WriteConcernResult createResult = postCollection.CreateIndex(index);
            Assert.IsTrue(createResult.Ok);

            BsonDocument explain = postCollection.Find(query).Explain(verbose: true);
            Assert.AreEqual("BtreeCursor PostedBy.Email_1", explain["cursor"]);
            Assert.AreEqual(explain["nscanned"], (int)postCollection.Find(query).Count());
        }

        [TestMethod]
        public void applies_index_on_inner_array_object()
        {

        }

        [TestMethod]
        public void applies_index_on_text_field()
        {
            
        }

        //TODO: actual full text indexing would require a real corpus
    }
}
