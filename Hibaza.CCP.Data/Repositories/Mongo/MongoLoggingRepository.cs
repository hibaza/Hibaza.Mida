using Dapper;
using Firebase.Database.Query;
using Hibaza.CCP.Core;
using Hibaza.CCP.Data.Providers.Firebase;
using Hibaza.CCP.Data.Providers.Mongo;
using Hibaza.CCP.Data.Providers.SQLServer;
using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories
{
    public class MongoLoggingRepository : ILoggingRepository
    {
        
        private const string LOGS = "applogs";
        public MongoFactory _mongoClient;
        public MongoLoggingRepository(IOptions<AppSettings> appSettings)
        {
                _mongoClient = new MongoFactory(appSettings);
        }

        public void Add(Log log)
        {
            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            try
            {
                var dic = log.ToBsonDocument();
                _mongoClient.Logs.InsertOne(dic);
            }
            catch (Exception ex) { }
            //});
        }

        public void Update(Log entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Log> GetLogs(Paging page)
        {
            List<Log> list = new List<Log>();
            //var key = "GetLogs";
            //var rs = CacheBase.cacheManagerGet<List<BsonDocument>>(key).Result;
            //if (rs != null && rs.Count > 0)
            //{
            //    return  _mongoClient.DeserializeBsonToEntity<Log>(rs).Result;
            //}

            //var options = new FindOptions<BsonDocument>();
            //options.Projection = "{'_id': 0}";
            //options.Limit = page.Limit;
            //var query = "{message:\"Webhook\",key:{$lte:" + long.Parse(page.Next) + "}}";
            //var result = _mongoClient.Logs.FindAsync<BsonDocument>(query, options).Result;
            //if (result != null )
            //{
            //    var d = result.ToList();
            //    list =  _mongoClient.DeserializeBsonToEntity<Log>(d).Result;
            //    CacheBase.cacheManagerSet(key, d, DateTime.Now.AddMinutes(30),
            //        _mongoClient.Logs, options, query, false, new List<string>() {  });
            //}
            return list;
        }

        public IEnumerable<Log> GetAll()
        {
            throw new NotImplementedException();
        }

        public Log GetById(string id)
        {
            throw new NotImplementedException();
        }

        public bool Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}
