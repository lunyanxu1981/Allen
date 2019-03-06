using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPITest1.Models;
using MongoDB;
using MongoDB.Driver;
using System.Configuration;
using MongoDB.Bson;

namespace WebAPITest1.Controllers
{
    public class ProductsController : ApiController
    {
        Product[] products = new Product[]
        {
            new Product { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1 },
            new Product { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M },
            new Product { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M }
        };

        public IEnumerable<Product> GetAllProducts()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["mongodbConn"].ConnectionString;
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(new MongoUrl(connectionString).DatabaseName);
            var collectionTable = database.GetCollection<BsonDocument>("products");
            FilterDefinitionBuilder<BsonDocument> builderFilter = Builders<BsonDocument>.Filter;
            FilterDefinition<BsonDocument> filter = builderFilter.Eq("Name", "Pencil");
            var result = collectionTable.Find<BsonDocument>(filter).ToList();
            string dumpStr = string.Empty;
            foreach (var item in result)
            {
                dumpStr += item.AsBsonValue;
            }

            return products;
        }

        public IHttpActionResult GetProduct(int id)
        {
            var test = GetAllProducts();
            var product = products.FirstOrDefault((p) => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
    }
}
