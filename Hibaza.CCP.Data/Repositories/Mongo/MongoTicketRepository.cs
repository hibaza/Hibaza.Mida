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
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories
{
    public class MongoTicketRepository : ITicketRepository
    {
        public  MongoFactory _mongoClient;
        IOptions<AppSettings> _appSettings = null;
        const string tickets = "Tickets";
        public MongoTicketRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
                _mongoClient = new MongoFactory(appSettings);
        }

        public Ticket GetById(string id)
        {
            try
            {
                var key = "Ticket_GetById" + id;

                var options = new FindOptions<Ticket, Ticket>();
                options.Projection = "{'_id': 0}";
                options.Limit = 1;
                //options.Sort = Builders<Ticket>.Sort.Descending("created_time");
                var query = "{id:\"" + id + "\"}";
                var rs = _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, tickets,
                     false, key, DateTime.Now.AddMinutes(10), false).Result;
                if (rs != null && rs.Count > 0)
                    return rs[0];
                return null;
            }
            catch { return null; }
        }

        public Ticket GetById(string business_id, string id)
        {
            try
            {
                var key = "Ticket_GetById01" + id;

                var options = new FindOptions<Ticket, Ticket>();
                options.Projection = "{'_id': 0}";
                options.Limit = 1;
                //options.Sort = Builders<Ticket>.Sort.Descending("created_time");
                var query = "{business_id:\"" + business_id + "\",id:\"" + id + "\"}";
                var rs = _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, tickets,
                     false, key, DateTime.Now.AddMinutes(10), false).Result;
                if (rs != null && rs.Count > 0)
                    return rs[0];
                return null;
            }
            catch { return null; }
        }

        public int UpdateCustomerId()
        {
            try
            {
                return 1;
                // dung job
                //System.Threading.Tasks.Task.Factory.StartNew(() =>
                //{
                //    _mongoClient.excuteProceduceMongoUpsert("Tickets_UpdateCustomerId", null, null, false);
                //});
                //return 1;
            }
            catch { return 0; }
        }

        public async Task<Ticket> GetCustomerLastTicket(string business_id, string customer_id, int status)
        {

            try
            {
                var key = "Tickets_GetCustomerLastTicket" + customer_id;

                var options1 = new FindOptions<Ticket, Ticket>();
                options1.Projection = "{'_id': 0}";
                options1.Limit = 1;
                options1.Sort = Builders<Ticket>.Sort.Descending("created_time");
                //var query1 = "{$and:[{customer_id:\"" + customer_id + "\"}, {$or:{status="+status+" or status < 0 )}}]}";

                var query = "";
                if (status < 0)
                    query = "{$and:[{business_id:\"" + business_id + "\"},{customer_id:\"" + customer_id + "\"}]}";
                else
                    query = "{$and:[{business_id:\"" + business_id + "\"},{customer_id:\"" + customer_id + "\"}, {$or:[{status:" + status + "} , {status :{$lte: 0 }}]}]}";
                var rs = await _mongoClient.excuteMongoLinqSelect(query, options1,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, tickets,
                     false, key, DateTime.Now.AddMinutes(10), false);
                if (rs != null && rs.Count > 0)
                    return rs[0];

                //// neu khong co tim app.hibaza.com thong qua customer_id
                //var rr1 = await GetTiketOld("000000809935934", customer_id);
                //if (rr1 != null)
                //    return rr1;

                //// neu khong co tim app.hibaza.com thong qua phone

                //var phonee =await GetCustomerPhone(customer_id);
                //if (phonee == null) return null;
                //var rr= await GetTiketOld("000000809935934", (string)phonee.customers[0].id);
                //if (rr != null)
                //    return rr;

            }
            catch (Exception ex)
            { return null; }
            return null;
        }

        //public async Task<Ticket> GetTiketOld(string business_id, string customer_id)
        //{
        //    var ticket = new Ticket();
        //    try
        //    {

        //        //var url = "http://api.hibaza.com/" + "tickets/last/000000809935934/541853329335522_2543047931763588586/?access_token=1";
        //        var url = "http://api.hibaza.com/" + "tickets/last/000000809935934/" + customer_id + "/?access_token=1";
        //        var model = await Core.Helpers.WebHelper.HttpGetAsync<TicketModel>(url);
        //        if (model != null)
        //        {
        //            ticket.business_id = business_id;
        //            ticket.channel_id = model.channel_id;
        //            ticket.created_time = DateTime.ParseExact(model.created_time, "hh:mm dd/MM/yyyy",
        //                                    System.Globalization.CultureInfo.InvariantCulture);
        //            ticket.customer_id = customer_id;
        //            ticket.customer_name = model.customer_name;
        //            ticket.description = model.description;
        //            ticket.id = model.id;
        //            ticket.order_id = model.order_id;
        //            ticket.sender_avatar = model.sender_avatar;
        //            ticket.sender_id = model.sender_id;
        //            ticket.sender_name = model.sender_name;
        //            ticket.short_description = model.short_description;
        //            ticket.status = model.status;
        //            ticket.tags = model.tags;
        //            ticket.thread_id = model.thread_id;
        //            ticket.timestamp = model.timestamp;
        //            ticket.type = model.type;
        //            ticket.updated_time = DateTime.Now;
        //            ticket._id = model.id;


        //            var option = new UpdateOptions { IsUpsert = true };
        //            var filter = Builders<Ticket>.Filter.Where(x => x.id == ticket.id);

        //            _mongoClient.excuteMongoLinqUpdate<Ticket>(filter, ticket, option,
        //_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, tickets,
        //true, "", DateTime.Now.AddMinutes(10));

        //            //CacheBase.cacheModifyAllKeyLinq(new List<string>() { ticket.customer_id });
        //            return ticket;
        //        }
        //    }
        //    catch { return null; }
        //    return null;
        //}

        //public async Task<dynamic> GetCustomerPhone(string customer_id)
        //{
        //    try
        //    {
        //        // neu khong co tim app.hibaza.com thong qua phone
        //        var options = new FindOptions<Customer, Customer>();
        //        options.Projection = "{'_id': 0}";
        //        options.Limit = 1;
        //        var query = "{id:\"" + customer_id + "\"}";

        //        var customer = await _mongoClient.excuteMongoLinqSelect(query, options,
        //            _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, "Customers",
        //             false, "", DateTime.Now.AddMinutes(10), false);

        //        if (customer[0].phone == null || customer[0].phone == "")
        //            return null;

        //        var url = "http://api.hibaza.com/" + "customers/search/000000809935934/?keywords=" + customer[0].phone + "&access_token=1";

        //        var cs= await Core.Helpers.WebHelper.HttpPostAsync<dynamic>(url, "");
        //        var tt = (string)cs.customers[0].id;
        //        return cs;

        //    }
        //    catch { }
        //    return null;
        //}
        public async Task<IEnumerable<Ticket>> GetCustomerTickets(string business_id, string customer_id, int status, Paging page)
        {
            List<Ticket> list = new List<Ticket>();
            try
            {
                var key = "Tickets_GetCustomerTickets" + customer_id;

                var options1 = new FindOptions<Ticket, Ticket>();
                options1.Projection = "{'_id': 0}";
                options1.Limit = 10;
                options1.Sort = Builders<Ticket>.Sort.Descending("created_time");
                //var query1 = "{customer_id:\"" + customer_id + "\"}";
                //var query1 = "{$and:[{business_id:\"" + business_id + "\"},{customer_id:\"" + customer_id + "\"}, {$or:[{status:" + status + "} , {status :{$lte: 0 }}]}]}";
                var query = "";
                if (status < 0)
                    query = "{$and:[{business_id:\"" + business_id + "\"},{customer_id:\"" + customer_id + "\"}]}";
                else
                    query = "{$and:[{business_id:\"" + business_id + "\"},{customer_id:\"" + customer_id + "\"}, {$or:[{status:" + status + "} , {status :{$lte: 0 }}]}]}";

                var rs = await _mongoClient.excuteMongoLinqSelect(query, options1,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, tickets,
                     false, key, DateTime.Now.AddMinutes(10), false);
                if (rs != null && rs.Count > 0)
                    return rs;

                //// neu khong co tim app.hibaza.com thong qua customer_id
                //var rr1 = await GetTiketOld("000000809935934", customer_id);
                //if (rr1 != null)
                //{
                //    list.Add(rr1);
                //    return list;
                //}
                //// neu khong co tim app.hibaza.com thong qua phone
                //var phonee = await GetCustomerPhone(customer_id);
                //if (phonee == null) return list;
                //var rr = await GetTiketOld("000000809935934", (string)phonee.customers[0].id);
                //if (rr != null)
                //{
                //    list.Add(rr);
                //    return list;
                //}

            }
            catch (Exception ex)
            { return list; }
            return list;
        }


        public async Task<IEnumerable<Ticket>> GetTickets(string business_id, int status, Paging page)
        {
            var list = new List<Ticket>();
            try
            {
                var key = "Tickets_GetTickets" + business_id + status;

                var options = new FindOptions<Ticket, Ticket>();
                options.Projection = "{'_id': 0}";
                options.Limit = page.Limit;
                options.Sort = Builders<Ticket>.Sort.Descending("created_time");
                //var query = "{business_id:\"" + business_id + "\",status:" + status + "}";
                var query = "";
                if (status < 0)
                    query = "{business_id:\"" + business_id + "\",timestamp:{$lte:" + page.Next + "}}";
                else
                    query = "{$and:[{business_id:\"" + business_id + "\"},{$or:[{status:" + status + "} , {status :{$lte: 0 }}]},{timestamp:{$lte:" + page.Next + "}}]}";
                return await _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, tickets,
                     true, key, DateTime.Now.AddMinutes(10), true);
                //para.Add("start", DateTime.Parse(page.Previous));
                //para.Add("end", DateTime.Parse(page.Next));

            }
            catch { return list; }
        }

        public void Upsert(Ticket ticket)
        {
            var option = new UpdateOptions { IsUpsert = true };
            var filter = Builders<Ticket>.Filter.Where(x => x.id == ticket.id && x.business_id == ticket.business_id);

            _mongoClient.excuteMongoLinqUpdate<Ticket>(filter, ticket, option,
 _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, tickets,
 true, "", DateTime.Now.AddMinutes(10)).Wait();

            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            //    try
            //    {
            //        CacheBase.cacheModifyAllKeyLinq(new List<string>() { ticket.customer_id });

            //        var customerOld = GetCustomerPhone(ticket.customer_id).Result;
            //        if (customerOld != null)
            //        {
            //            ticket.sender_id = "000000618019832";
            //            ticket.sender_name = "Hibaza";
            //            ticket.business_id = "000000809935934";
            //            ticket.customer_id = (string)customerOld.customers[0].id;
            //            ticket.customer_name = (string)customerOld.customers[0].name;
            //            ticket.id = Core.Helpers.CommonHelper.GenerateNineDigitUniqueNumber();
            //            var url = "http://api.hibaza.com/" + "tickets/add?access_token=1";
            //            var rs = Core.Helpers.WebHelper.HttpPostAsync<ApiResponse>(url, ticket).Result;
            //        }
            //        else
            //        {
            //            ticket.sender_id = "000000618019832";
            //            ticket.sender_name = "Hibaza";
            //            ticket.business_id = "000000809935934";
            //            var url = "http://api.hibaza.com/" + "tickets/add?access_token=1";
            //            var rs = Core.Helpers.WebHelper.HttpPostAsync<ApiResponse>(url, ticket).Result;
            //        }
            //    }
            //    catch { }
            //});
        }

        public bool Delete(string business_id, string id)
        {
            try
            {
                var key = "Ticket_Delete" + business_id + id;

                var query = "{business_id:\"" + business_id + "\",id:\"" + id + "\"}";
                _mongoClient.excuteMongoLinqDelete<Ticket>(query,
                   _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, tickets,
                    true, key, DateTime.Now.AddMinutes(10), true).Wait();
                CacheBase.cacheModifyAllKeyLinq(new List<string>() { id });
                return true;
            }
            catch { return false; }
        }

        public async Task<List<Ticket>> GetAll(string business_id, Paging page)
        {
            var list = new List<Ticket>();
            try
            {
                var key = "Ticket_GetAll" + business_id;

                var options = new FindOptions<Ticket, Ticket>();
                options.Projection = "{'_id': 0}";
                options.Limit = page.Limit;
                options.Sort = Builders<Ticket>.Sort.Descending("created_time");
                // var query = "{business_id:\"" + business_id + "\",created_time: {$gte: ISODate(\"2010-12-15T00:00:00.000Z\"),$lt: ISODate(\"2017-12-25T00:00:00.000Z\"}";
                var query = "{business_id:\"" + business_id + "\"}";
                //var query = "{}";
                return await _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, tickets,
                     true, key, DateTime.Now.AddMinutes(10), true);
            }
            catch { return list; }
        }

        public bool Update(Ticket ticket)
        {
            throw new NotImplementedException();
        }

        public bool Insert(Ticket ticket)
        {
            throw new NotImplementedException();
        }
    }
}
