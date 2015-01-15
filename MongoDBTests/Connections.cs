using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;

namespace MongoDBTests
{
    [TestClass]
    public class Connections
    {
        [TestMethod]
        public void client_created()
        {
            MongoClientSettings settings = new MongoClientSettings
            {
                Server = new MongoServerAddress("ds062097.mongolab.com", 62079),
                Credentials = new List<MongoCredential>
                {
                    new MongoCredential(new MongoInternalIdentity("something", "admin"), new PasswordEvidence("admin"))
                }
            };

            var client = new MongoClient(settings);
            Assert.AreEqual("ds062097.mongolab.com", client.Settings.Server.Host);
            Assert.AreEqual(62079, client.Settings.Server.Port);
        }

        [TestMethod]
        public void client_can_get_server()
        {
            var client = new MongoClient("mongodb://admin:admin@ds062097.mongolab.com:62097/something");
            Assert.AreEqual("ds062097-a.mongolab.com:62097", client.GetServer().Instance.Address.ToString());
        }

        [TestMethod]
        public void client_can_get_server_database()
        {
            var client = new MongoClient("mongodb://admin:admin@ds062097.mongolab.com:62097/something");
            var database = client.GetServer().GetDatabase("something");
            Assert.AreEqual("something", database.Name);
        }

        [TestMethod]
        public void client_can_get_document_collection()
        {
            var client = new MongoClient("mongodb://admin:admin@ds062097.mongolab.com:62097/something");
            var collection = client.GetServer().GetDatabase("data"); // note that "data" and "Data" are different collections
            Assert.AreEqual("data", collection.Name);
        }

        [TestMethod]
        public void client_can_connect_to_database()
        {
            var client = new MongoClient("mongodb://admin:admin@ds062097.mongolab.com:62097/something");
            client.GetServer().Connect();
        }

        [TestMethod]
        [ExpectedException(typeof(MongoConnectionException))]
        public void client_cannot_connect_without_database_in_connectionstring()
        {
            // this doesn't necessarily make sense as the user is database specific
            var client = new MongoClient("mongodb://admin:admin@ds062097.mongolab.com:62097/");
            client.GetServer().GetDatabase("something").Server.Connect();
        }

        [TestMethod]
        public void server_disconnect()
        {
            // docs say only to do if app is terminating, client maintains a collection pool internally so we don't 
            // have to worry about when to connect/disconnect
            var client = new MongoClient("mongodb://admin:admin@ds062097.mongolab.com:62097/");
            var server = client.GetServer();
            server.Disconnect();

            // assuming success without exception
        }
    }
}
