using Dapper;
using Hibaza.CCP.Core;
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
    public class MongoAttachmentRepository : IAttachmentRepository
    {
        public MongoFactory _mongoClient;
        const string attachments = "Attachments";
        IOptions<AppSettings> _appSettings = null;
        public MongoAttachmentRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
                _mongoClient = new MongoFactory(appSettings);
        }
        public async Task<IEnumerable<Attachment>> GetAttachments(string business_id, string channel_id, string product_id, Paging page)
        {
            List<Attachment> list = new List<Attachment>();
            var key = "GetAttachments" + business_id + channel_id + product_id;

            var options = new FindOptions<Attachment, Attachment>();
            options.Projection = "{'_id': 0}";
            options.Limit = page.Limit;
            var query = "{business_id:\"" + business_id + "\"" +
                        ",channel_id:\"" + channel_id + "\"" +
                        ",product_id:\"" + product_id + "\"" +
                "}";
            //",timestamp:{$lte:" + long.Parse(page.Next ?? "9999999999") + "}" +
            //           ",timestamp:{$gte:" + long.Parse(page.Previous ?? "0") + "}" +
            return await _mongoClient.excuteMongoLinqSelect(query, options,
                  _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, attachments,
                   true, key, DateTime.Now.AddMinutes(10), true);

        }


        public Attachment GetById(string business_id, string channel_id, string id)
        {
            var key = "Attachment_GetById" + business_id + channel_id + id;

            var options = new FindOptions<Attachment>();
            options.Projection = "{'_id': 0}";
            options.Limit = 1;
            var query = "{business_id:\"" + business_id + "\"" +
                        ",channel_id:\"" + channel_id + "\"" +
                        ",id:\"" + id + "\"" +
                "}";
            var rs = _mongoClient.excuteMongoLinqSelect(query, options,
               _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, attachments,
                true, key, DateTime.Now.AddMinutes(10), true).Result;
            if (rs != null && rs.Count > 0)
                return rs[0];
            return null;

        }


        public async Task<IEnumerable<Attachment>> GetAll(string business_id, string channel_id, Paging page)
        {
            List<Attachment> list = new List<Attachment>();
            var key = "Attachment_GetAll" + business_id + channel_id;

            var options = new FindOptions<Attachment>();
            options.Projection = "{'_id': 0}";
            options.Limit = page.Limit;
            var query = "{business_id:\"" + business_id + "\"" +
                        ",channel_id:\"" + channel_id + "\"" +
                "}";

            return await _mongoClient.excuteMongoLinqSelect(query, options,
               _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, attachments,
                true, key, DateTime.Now.AddMinutes(10), true);
        }

        public bool Update(Attachment attachment)
        {
            throw new NotImplementedException();
        }

        public bool Insert(Attachment attachment)
        {
            throw new NotImplementedException();
        }


        public bool Upsert(Attachment attachment)
        {
            var option = new UpdateOptions { IsUpsert = true };
            var filter = Builders<Attachment>.Filter.Where(x => x.id == attachment.id && x.business_id == attachment.business_id);

            _mongoClient.excuteMongoLinqUpdate<Attachment>(filter, attachment, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, attachments,
true, "", DateTime.Now.AddMinutes(10)).Wait();

            CacheBase.cacheModifyAllKeyLinq(new List<string>() { attachment.id });

            return true;
        }

        public bool Delete(string business_id, string channel_id, string id)
        {
            throw new NotImplementedException();
        }

    }
}
