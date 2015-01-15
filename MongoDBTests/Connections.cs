using System.Collections.Generic;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;

namespace MongoDBTests
{
    [TestClass]
    public class Connections
    {
        private readonly string connection = ConfigurationManager.ConnectionStrings["mongolab"].ConnectionString;
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
        [Ignore] // will flip back and forth between ds062097 and ds062097-a
        public void client_can_get_server()
        {
            var client = new MongoClient(connection);
            Assert.AreEqual("ds062097.mongolab.com:62097", client.GetServer().Instance.Address.ToString());
        }

        [TestMethod]
        public void client_can_get_server_database()
        {
            var client = new MongoClient(connection);
            var database = client.GetServer().GetDatabase("something");
            Assert.AreEqual("something", database.Name);
        }

        [TestMethod]
        public void client_can_get_document_collection()
        {
            var client = new MongoClient(connection);
            var collection = client.GetServer().GetDatabase("data"); // note that "data" and "Data" are different collections
            Assert.AreEqual("data", collection.Name);
        }

        [TestMethod]
        public void client_can_connect_to_database()
        {
            var client = new MongoClient(connection);
            client.GetServer().Connect();
        }

        [TestMethod]
        public void server_disconnect()
        {
            // docs say only to do if app is terminating, client maintains a collection pool internally so we don't 
            // have to worry about when to connect/disconnect
            var client = new MongoClient(connection);
            var server = client.GetServer();
            server.Disconnect();

            // assuming success without exception
        }
    }
}
