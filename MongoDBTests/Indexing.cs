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

            IndexKeysBuilder<Post> index = new IndexKeysBuilder<Post>();
            index.Descending(p => p.Count); 
            WriteConcernResult createResult = postCollection.CreateIndex(index);
            Assert.IsTrue(createResult.Ok);

            BsonDocument explain = postCollection.Find(query).Explain(verbose: true);
            Assert.AreEqual("BtreeCursor Count_-1", explain["cursor"]);
        }

        private bool PostIsPopular(int count)
        {
            return count > postCounts.Count*0.9;
        }

        [TestMethod]
        public void applies_index_on_multiple_object_prop()
        {


        }

        [TestMethod]
        public void applies_index_on_inner_object_prop()
        {
            
        }

        [TestMethod]
        public void applies_index_on_inner_array_object()
        {
            
        }

        [TestMethod]
        public void applies_index_on_text_field()
        {
            
        }
    }
}
