using Dapper;
using Hibaza.CCP.Data.Providers.SQLServer;
using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Hibaza.CCP.Domain.Models;
using Hibaza.CCP.Domain.Models.Report;
using Hibaza.CCP.Data.Providers.Mongo;
using Microsoft.Extensions.Options;
using Hibaza.CCP.Core;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Hibaza.CCP.Data.Repositories
{
    public class MongoReportRepository : IReportRepository
    {
        public IConnectionFactory _connectionFactory;
        public MongoFactory _mongoClient;
        IOptions<AppSettings> _appSettings;
        string report = "Reports";
        public MongoReportRepository(IConnectionFactory connectionFactory, IOptions<AppSettings> appSettings)
        {
                _connectionFactory = connectionFactory;
            _appSettings = appSettings;
                _mongoClient = new MongoFactory(appSettings);
        }

        public async Task<IEnumerable<ReportAgentDataLine>> GetAgentChartData(string business_id, Paging page)
        {
            List<ReportAgentDataLine> list = new List<ReportAgentDataLine>();

            var match = "{$match:{$and:[{business_id:\"" + business_id + "\"},{key:\"GetAgentChartDataJob\"},{timestamp: { $gte : " + page.Previous + "}},{timestamp : { $lt : " + (page.Next) + "} }]}}";
            var group = "{$group: {" +
                                "_id: \"$id\", " +
                                "customer_ids: {$sum: \"$customers\"}, " +
                                "thread_ids: {$sum: \"$conversations\"}," +
                                "messagess: {$sum: \"$messages\"}" +
                                "}}";
            var project = "{$project: {" +
                                        "_id: 0, " +
                                        "id: \"$_id\", " +
                                        "customers: \"$customer_ids\", " +
                                        "conversations: \"$thread_ids\"  ," +
                                        "messages: \"$messagess\"" +
                                    "}}";
            var sort = "{$sort: { messages: -1}}";
            var Pipeline = new[] { BsonDocument.Parse(match), BsonDocument.Parse(group), BsonDocument.Parse(project), BsonDocument.Parse(sort) };

            var client = new MongoClient(_appSettings.Value.MongoDB.ConnectionString);
            var db = client.GetDatabase(_appSettings.Value.MongoDB.Database);
            var collection = db.GetCollection<BsonDocument>(report);
            var d = collection.Aggregate<BsonDocument>(Pipeline, null).ToList();

            list = await _mongoClient.DeserializeBsonToEntity<ReportAgentDataLine>(d);
            return list;
        }

        public async Task<IEnumerable<ReportChatDataLine>> GetAgentChatChartData(string business_id, Paging page)
        {
            List<ReportChatDataLine> list = new List<ReportChatDataLine>();

            var match = "{$match:{$and:[{business_id:\"" + business_id + "\"},{key:\"GetAgentChatChartDataJob\"},{timestamp: { $gte : " + page.Previous + "}},{timestamp : { $lt : " + (page.Next) + "} }]}}";
            var group = "{$group: {" +
                                "_id: \"$date\", " +
                                "customers: {$sum: \"$customers\"}, " +
                                "conversations: {$sum: \"$conversations\"}," +
                                "inboxes: {$sum: \"$inboxes\"}," +
                                "comments: {$sum: \"$comments\"}," +
                                "}}";
            var project = "{$project: {" +
                                        "_id: 0, " +
                                        "date: \"$_id\", " +
                                        "customers: \"$customers\", " +
                                        "conversations: \"$conversations\"  ," +
                                        "inboxes: \"$inboxes\"," +
                                        "comments: \"$comments\"," +
                                    "}}";
            var sort = "{$sort: { date: -1}}";
            var Pipeline = new[] { BsonDocument.Parse(match), BsonDocument.Parse(group), BsonDocument.Parse(project), BsonDocument.Parse(sort) };

            var client = new MongoClient(_appSettings.Value.MongoDB.ConnectionString);
            var db = client.GetDatabase(_appSettings.Value.MongoDB.Database);
            var collection = db.GetCollection<BsonDocument>(report);
            var d = collection.Aggregate<BsonDocument>(Pipeline, null).ToList();

            list = await _mongoClient.DeserializeBsonToEntity<ReportChatDataLine>(d);
            return list;

        }

        public async Task<IEnumerable<ReportChatDataLine>> GetCustomerChatChartData(string business_id, Paging page)
        {
            List<ReportChatDataLine> list = new List<ReportChatDataLine>();

            var match = "{$match:{$and:[{business_id:\"" + business_id + "\"},{key:\"GetCustomerChatChartDataJob\"},{timestamp: { $gte : " + page.Previous + "}},{timestamp : { $lt : " + (page.Next) + "} }]}}";
            var group = "{$group: {" +
                                "_id: \"$date\", " +
                                "customers: {$sum: \"$customers\"}, " +
                                "conversations: {$sum: \"$conversations\"}," +
                                "inbox_replies: {$sum: \"$inbox_replies\"}," +
                                "comment_replies: {$sum: \"$comment_replies\"}," +
                                "inboxes: {$sum: \"$inboxes\"}," +
                                "comments: {$sum: \"$comments\"}," +
                                "}}";
            var project = "{$project: {" +
                                        "_id: 0, " +
                                        "date: \"$_id\", " +
                                        "customers: \"$customers\", " +
                                        "conversations: \"$conversations\"  ," +
                                        "inbox_replies: \"$inbox_replies\"," +
                                        "comment_replies: \"$comment_replies\"," +
                                        "inboxes: \"$inboxes\"," +
                                        "comments: \"$comments\"," +
                                    "}}";
            var sort = "{$sort: { date: -1}}";
            var Pipeline = new[] { BsonDocument.Parse(match), BsonDocument.Parse(group), BsonDocument.Parse(project), BsonDocument.Parse(sort) };

            var client = new MongoClient(_appSettings.Value.MongoDB.ConnectionString);
            var db = client.GetDatabase(_appSettings.Value.MongoDB.Database);
            var collection = db.GetCollection<BsonDocument>(report);
            var d = collection.Aggregate<BsonDocument>(Pipeline, null).ToList();

            list = await _mongoClient.DeserializeBsonToEntity<ReportChatDataLine>(d);
            return list;

        }

        public async Task<IEnumerable<ReportTicketDataLine>> GetTicketChartData(string business_id, Paging page)
        {
            List<ReportTicketDataLine> list = new List<ReportTicketDataLine>();

            var match = "{$match:{$and:[{business_id:\"" + business_id + "\"},{key:\"GetTicketChartDataJob\"},{timestamp: { $gte : " + page.Previous + "}},{timestamp : { $lt : " + (page.Next) + "} }]}}";
            var group = "{$group: {" +
                                "_id: \"$date\", " +
                                "pending_tickets: {$sum: \"$pending_tickets\"}, " +
                                "attention_tickets: {$sum: \"$attention_tickets\"}," +
                                "completed_tickets: {$sum: \"$completed_tickets\"}," +
                                "tickets: {$sum: \"$tickets\"}," +
                                "}}";
            var project = "{$project: {" +
                                        "_id: 0, " +
                                        "date: \"$_id\", " +
                                        "pending_tickets: \"$pending_tickets\", " +
                                        "attention_tickets: \"$attention_tickets\"  ," +
                                        "completed_tickets: \"$completed_tickets\"," +
                                        "tickets: \"$tickets\"," +
                                    "}}";
            var sort = "{$sort: { date: -1}}";
            var Pipeline = new[] { BsonDocument.Parse(match), BsonDocument.Parse(group), BsonDocument.Parse(project), BsonDocument.Parse(sort) };

            var client = new MongoClient(_appSettings.Value.MongoDB.ConnectionString);
            var db = client.GetDatabase(_appSettings.Value.MongoDB.Database);
            var collection = db.GetCollection<BsonDocument>(report);
            var d = collection.Aggregate<BsonDocument>(Pipeline, null).ToList();

            list = await _mongoClient.DeserializeBsonToEntity<ReportTicketDataLine>(d);
            return list;
        }

        #region job chu ky 30 phut day vao bang report

        public async Task<List<BsonDocument>> GetCustomerChatChartDataJob(string business_id, Paging page)
        {
            var match = "{$match:{$and:[{timestamp:{$gte:" + page.Previous + "}}," +
                                 "{timestamp:{$lte: " + page.Next + "}}," +
                                 "{business_id:\"" + business_id + "\"}" +
                                "]}}";
            var group = "{$group: {_id:  { $dateToString: { format: \"%Y-%m-%d\", date: \"$created_time\" }} ," +
                                "customer_ids: {$addToSet: \"$customer_id\"}," +
                                "thread_ids: {$addToSet: \"$thread_id\"}," +
                                "inbox_replies: {$sum: {$cond: { if: {$and:[ {$eq: [\"$thread_type\", \"message\"]},{$ne: [\"$agent_id\", null]},{$ne: [\"$agent_id\", \"\"]} ]}, then: 1, else: 0 } } }," +
                                "comment_replies: {$sum: {$cond: { if: {$and:[ {$eq: [\"$thread_type\", \"comment\"]},{$ne: [\"$agent_id\", null]},{$ne: [\"$agent_id\", \"\"]} ]}, then: 1, else: 0 } } }," +
                                "inboxes: {$sum: {$cond: { if: {$and:[ {$eq: [\"$thread_type\", \"message\"]},{$ne: [\"$sender_ext_id\", \"$channel_ext_id\"]} ]}, then: 1, else: 0 } } }," +
                                "comments: {$sum: {$cond: { if: {$and:[ {$eq: [\"$thread_type\", \"comment\"]},{$ne: [\"$sender_ext_id\", \"$channel_ext_id\"]} ]}, then: 1, else: 0 } } }," +
                            "}}";
            var project = "{$project: {" +
                                        "_id: 0, " +
                                        "date: \"$_id\"," +
                                        "customers: {$size: \"$customer_ids\"}, " +
                                        "conversations: {$size: \"$thread_ids\"}," +
                                        "inbox_replies: \"$inbox_replies\"," +
                                        "comment_replies:\"$comment_replies\"," +
                                        "inboxes:  \"$inboxes\"," +
                                        "comments: \"$comments\"" +
                                "}}";
            var sort = "{$sort: { date: -1}}";
            var Pipeline = new[] { BsonDocument.Parse(match), BsonDocument.Parse(group), BsonDocument.Parse(project), BsonDocument.Parse(sort) };
            var result = await _mongoClient.Messages.AggregateAsync<BsonDocument>(Pipeline);

            var d = result.ToList();
            return d;
        }


        public async Task<List<BsonDocument>> GetTicketChartDataJob(string business_id, Paging page)
        {
            var match = "{$match:{$and:[{timestamp:{$gte:" + page.Previous + "}}," +
                                 "{timestamp:{$lte: " + page.Next + "}}," +
                                 "{business_id:\"" + business_id + "\"}" +
                                "]}}";
            var group = "{$group: {_id:  { $dateToString: { format: \"%Y-%m-%d\", date: \"$created_time\" }} ," +
                                "pending_tickets: {$sum: {$cond: { if: { $eq: [\"$status\", 0] }, then: 1, else: 0 } } }," +
                                "attention_tickets: {$sum: {$cond: { if: { $eq: [\"$status\", 1] }, then: 1, else: 0 } } }," +
                                "completed_tickets: {$sum: {$cond: { if: { $eq: [\"$status\", 2] }, then: 1, else: 0 } } }," +
                                "count: {$sum: 1}" +
                                "}" +
                                "}";
            var project = "{$project: {_id: 0, date: \"$_id\", " +
                                "pending_tickets: \"$pending_tickets\", " +
                                "attention_tickets: \"$attention_tickets\", " +
                                "completed_tickets: \"$completed_tickets\", " +
                                "tickets: \"$count\"" +
                                "}" +
                                "}";
            var sort = "{$sort: { date: -1}}";
            var Pipeline = new[] { BsonDocument.Parse(match), BsonDocument.Parse(group), BsonDocument.Parse(project), BsonDocument.Parse(sort) };
            var result = await _mongoClient.Tickets.AggregateAsync<BsonDocument>(Pipeline);

            var d = result.ToList();

            return d;

        }

        public async Task<List<BsonDocument>> GetAgentChatChartDataJob(string business_id, Paging page)
        {
            var match = "{$match:{$and:[{timestamp:{$gte:" + page.Previous + "}}," +
                                 "{timestamp:{$lte: " + page.Next + "}}," +
                                 "{agent_id:{$ne: \"\"}}," +
                                 "{business_id:\"" + business_id + "\"}" +
                                "]}}";
            var group = "{$group: {_id:  { $dateToString: { format: \"%Y-%m-%d\", date: \"$created_time\" }} ," +
                                "customer_ids: {$addToSet: \"$customer_id\"}," +
                                "thread_ids: {$addToSet: \"$thread_id\"}," +
                                "message: {$sum: {$cond: { if: { $eq: [\"$thread_type\", \"message\"] }, then: 1, else: 0 } } }," +
                                "comment: {$sum: {$cond: { if: { $eq: [\"$thread_type\", \"comment\"] }, then: 1, else: 0 } } }," +
                            "}}";
            var project = "{$project: {" +
                                        "_id: 0, " +
                                        "date: \"$_id\"," +
                                        "customers: {$size: \"$customer_ids\"}, " +
                                        "conversations: {$size: \"$thread_ids\"}," +
                                        "inboxes: \"$message\"," +
                                        "comments:\"$comment\"" +
                                    "}}";
            var sort = "{$sort: { inboxes: -1}}";
            var Pipeline = new[] { BsonDocument.Parse(match), BsonDocument.Parse(group), BsonDocument.Parse(project), BsonDocument.Parse(sort) };
            var result = await _mongoClient.Messages.AggregateAsync<BsonDocument>(Pipeline);

            return result.ToList();
        }


        public async Task<List<BsonDocument>> GetAgentChartDataJob(string business_id, Paging page)
        {
            var match = "{$match:{$and:[{timestamp:{$gte:" + page.Previous + "}}," +
                                 "{timestamp:{$lte: " + page.Next + "}}," +
                                 "{agent_id:{$ne: \"\"}}," +
                                 "{business_id:\"" + business_id + "\"}" +
                                "]}}";
            var group = "{$group: {" +
                                "_id: \"$agent_id\", " +
                                "customer_ids: {$addToSet: \"$customer_id\"}, " +
                                "thread_ids: {$addToSet: \"$thread_id\"}," +
                                "count: {$sum: 1}" +
                                "}}";
            var project = "{$project: {" +
                                        "_id: 0, " +
                                        "id: \"$_id\", " +
                                        "customers: {$size: \"$customer_ids\"}, " +
                                        "conversations: {$size: \"$thread_ids\"}  ," +
                                        "messages: \"$count\"" +
                                    "}}";
            var sort = "{$sort: { count: -1}}";
            var Pipeline = new[] { BsonDocument.Parse(match), BsonDocument.Parse(group), BsonDocument.Parse(project), BsonDocument.Parse(sort) };
            var result = await _mongoClient.Messages.AggregateAsync<BsonDocument>(Pipeline);

            return result.ToList();
        }


        public void UpsertReportAll()
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                try
                {
                    var w = new BsonDocument();
                    var business = _mongoClient.Businesses.FindAsync(w, null).Result;
                    var bs = business.ToList();

                    //for (var i = 0; i < 60; i++)
                    //{
                    long start = Hibaza.CCP.Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.Today);
                    long end = Hibaza.CCP.Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.Now);
                    //long start = Hibaza.CCP.Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.Today.AddDays(-(i+1)));
                    //    long end = Hibaza.CCP.Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.Today.AddDays(-i));
                    Paging page = new Paging();
                    page.Previous = start + "";
                    page.Next = end + "";

                    //#region xoa het du lieu trong ngay 
                    //try
                    //{
                    //    var query = "{$and:[{timestamp: { $gte : " + start + "}},{timestamp : { $lt: " + end + "} }]}";

                    //    var client = new MongoClient(_appSettings.Value.MongoDB.ConnectionString);
                    //    var db = client.GetDatabase(_appSettings.Value.MongoDB.Database);
                    //    var collection = db.GetCollection<BsonDocument>(report);
                    //    collection.DeleteMany(query);
                    //}
                    //catch { }

                    //#endregion
                    foreach (var b in bs)
                    {
                        var business_id = b.id;
                        var tickets = GetTicketChartDataJob(business_id, page).Result;
                        var customers = GetCustomerChatChartDataJob(business_id, page).Result;
                        var agents = GetAgentChatChartDataJob(business_id, page).Result;
                        var agents01 = GetAgentChartDataJob(business_id, page).Result;

                        #region them du lieu moi
                        foreach (var t in tickets)
                        {
                            try
                            {
                                t.Add("key", "GetTicketChartDataJob");
                                t.Add("created_time", DateTime.Now);
                                t.Add("timestamp", start);
                                t.Add("business_id", business_id);

                                var option = new UpdateOptions { IsUpsert = true };
                                var filter = Builders<BsonDocument>.Filter.Where(x => x["timestamp"] == start
                                && x["key"] == "GetTicketChartDataJob"
                                && x["business_id"] == business_id
                                && x["date"] == t["date"]
                                );
                                _mongoClient.excuteMongoLinqUpdate<BsonDocument>(filter, t, option,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, report,
                    false, "", DateTime.Now.AddMinutes(10));
                            }
                            catch { }
                        }

                        foreach (var t in customers)
                        {
                            try
                            {
                                t.Add("key", "GetCustomerChatChartDataJob");
                                t.Add("created_time", DateTime.Now);
                                t.Add("timestamp", start);
                                t.Add("business_id", business_id);

                                var option = new UpdateOptions { IsUpsert = true };
                                var filter = Builders<BsonDocument>.Filter.Where(x => x["timestamp"] == start
                                && x["key"] == "GetCustomerChatChartDataJob"
                                && x["business_id"] == business_id
                                );
                                _mongoClient.excuteMongoLinqUpdate<BsonDocument>(filter, t, option,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, report,
                    false, "", DateTime.Now.AddMinutes(10));
                            }
                            catch { }
                        }


                        foreach (var t in agents)
                        {
                            try
                            {
                                t.Add("key", "GetAgentChatChartDataJob");
                                t.Add("created_time", DateTime.Now);
                                t.Add("timestamp", start);
                                t.Add("business_id", business_id);

                                var option = new UpdateOptions { IsUpsert = true };
                                var filter = Builders<BsonDocument>.Filter.Where(x => x["timestamp"] == start
                                && x["key"] == "GetAgentChatChartDataJob"
                                && x["business_id"] == business_id
                                );
                                _mongoClient.excuteMongoLinqUpdate<BsonDocument>(filter, t, option,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, report,
                    false, "", DateTime.Now.AddMinutes(10));
                            }
                            catch { }
                        }

                        foreach (var t in agents01)
                        {
                            try
                            {
                                t.Add("key", "GetAgentChartDataJob");
                                t.Add("created_time", DateTime.Now);
                                t.Add("timestamp", start);
                                t.Add("business_id", business_id);

                                var option = new UpdateOptions { IsUpsert = true };
                                var filter = Builders<BsonDocument>.Filter.Where(x => x["timestamp"] == start
                                && x["key"] == "GetAgentChartDataJob"
                                && x["business_id"] == business_id
                                && x["id"] == t["id"]
                                );
                                _mongoClient.excuteMongoLinqUpdate<BsonDocument>(filter, t, option,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, report,
                    false, "", DateTime.Now.AddMinutes(10));
                            }
                            catch { }
                        }
                        #endregion
                        //  }
                    }

                }
                catch (Exception ex)
                {
                }
            });
        }
        #endregion
    }
}
