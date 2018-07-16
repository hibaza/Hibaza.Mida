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
    public class MongoNoteRepository : INoteRepository
    {
        const string notes = "Notes";
        public MongoFactory _mongoClient;
        IOptions<AppSettings> _appSettings = null;
        public MongoNoteRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
                _mongoClient = new MongoFactory(appSettings);
        }

        public Note GetById(string business_id, string id)
        {
            try
            {
                var key = "Note_GetById" + business_id + id;

                var options = new FindOptions<Note, Note>();
                options.Projection = "{'_id': 0}";
                options.Limit = 1;
                //options.Sort = Builders<Note>.Sort.Descending("created_time");
                var query = "{business_id:\"" + business_id + "\",id:\"" + id + "\"}";
                var rs = _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, notes,
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
                // dùng job
                // _mongoClient.excuteProceduceMongoUpsert("Notes_UpdateCustomerId", null,null, false);

            }
            catch { return 0; }
        }


        public async Task<IEnumerable<Note>> GetCustomerNotes(string business_id, string customer_id, Paging page)
        {
            var list = new List<Note>();

            try
            {
                var key = "Notes_GetCustomerNotes" + customer_id;

                var options1 = new FindOptions<Note, Note>();
                options1.Projection = "{'_id': 0}";
                options1.Limit = 1;
                options1.Sort = Builders<Note>.Sort.Descending("created_time");
                var query1 = "{business_id:\"" + business_id + "\",customer_id:\"" + customer_id + "\"}";
                var rs = await _mongoClient.excuteMongoLinqSelect(query1, options1,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, "Notes",
                     false, key, DateTime.Now.AddMinutes(10), false);
                if (rs != null && rs.Count > 0)
                    return rs;

                //// neu khong co tim app.hibaza.com thong qua customer_id
                //var rr1 = await GetNoteOld("000000809935934", customer_id);
                //if (rr1 != null && rr1.Count>0)
                //{
                //    return rr1;
                //}
                //// neu khong co tim app.hibaza.com thong qua phone
                //var phonee = await GetCustomerPhone(customer_id);
                //if (phonee == null) return list;
                //var rr = await GetNoteOld("000000809935934", (string)phonee.customers[0].id);
                //if (rr != null && rr.Count > 0)
                //{
                //    return rr;
                //}

            }
            catch (Exception ex)
            { return list; }
            return list;
        }


        public void Upsert(Note note)
        {
            var option = new UpdateOptions { IsUpsert = true };
            var filter = Builders<Note>.Filter.Where(x => x.id == note.id && x.business_id == note.business_id);

            _mongoClient.excuteMongoLinqUpdate<Note>(filter, note, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, notes,
true, "", DateTime.Now.AddMinutes(10)).Wait();

            CacheBase.cacheModifyAllKeyLinq(new List<string>() { note.customer_id });

            //var customerOld = GetCustomerPhone(note.customer_id).Result;
            //if (customerOld != null)
            //{
            //    var url = "http://api.hibaza.com/notes/send/?access_token=1";
            //    var rs = Core.Helpers.WebHelper.HttpPostAsync<ApiResponse>(url, new Note { customer_id = (string)customerOld.customers[0].id, customer_name = (string)customerOld.customers[0].name, type = "customer", business_id = "000000809935934", thread_id = note.thread_id, text = note.text, featured = note.featured, sender_id = "000000618019832", sender_name = "Hibaza", sender_avatar = ""  }).Result;
            //}
            //else
            //{
            //    var url = "http://api.hibaza.com/notes/send/?access_token=1";
            //    var rs = Core.Helpers.WebHelper.HttpPostAsync<ApiResponse>(url, new Note { type = "customer", business_id = "000000809935934", thread_id = note.thread_id, text = note.text, featured = note.featured, sender_id = "000000618019832", sender_name = "Hibaza", sender_avatar = "" }).Result;
            //}


        }

        public bool Delete(string business_id, string id)
        {
            try
            {
                var key = "Notes_Delete" + id;
                var query = "{business_id:\"" + business_id + "\",id:\"" + id + "\"}";
                // var query = "{id:\"" + id + "\"}";
                _mongoClient.excuteMongoLinqDelete<Note>(query,
                   _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, notes,
                    true, key, DateTime.Now.AddMinutes(10), true);
                CacheBase.cacheModifyAllKeyLinq(new List<string>() { id });
                return true;
            }
            catch { return false; }
        }

        public async Task<IEnumerable<Note>> GetAll(string business_id, Paging page)
        {
            var list = new List<Note>();
            try
            {
                var key = "Note_GetAll" + business_id;

                var options = new FindOptions<Note, Note>();
                options.Projection = "{'_id': 0}";
                options.Limit = 1;
                options.Sort = Builders<Note>.Sort.Descending("created_time");
                var query = "{business_id:\"" + business_id + "\"}";
                // var query = "{}";
                return await _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, notes,
                     true, key, DateTime.Now.AddMinutes(10), true);
            }
            catch { return list; }
        }

        public bool Update(Note note)
        {
            throw new NotImplementedException();
        }

        public bool Insert(Note note)
        {
            throw new NotImplementedException();
        }

        //public async Task<List<Note>> GetNoteOld(string business_id, string customer_id)
        //{
        //    List<Note> list = new List<Note>();
        //    try
        //    {   
        //        var url = "http://api.hibaza.com/notes/" + "list/customer/000000809935934/" + customer_id + "/?access_token=1";
        //        var response = await Core.Helpers.WebHelper.HttpGetAsync<ApiResponse>(url);

        //        JArray array = (JArray)(response.data);
        //        foreach (var val in array)
        //        {
        //            Note note = new Note();
        //            note.id = val["id"].ToString();
        //            note.created_time = (DateTime)val["created_time"];
        //            note.sender_id = val["sender_id"].ToString();
        //            note.sender_name = val["sender_name"].ToString();
        //            note.sender_avatar = val["sender_avatar"].ToString();
        //            note.text = val["text"].ToString();
        //            note.featured = (bool)val["featured"];
        //            note.thread_id = val["thread_id"].ToString();
        //            note.customer_id = val["customer_id"].ToString();
        //            note.type = val["type"].ToString();
        //            note.business_id = business_id;
        //            list.Add(note);

        //            var option = new UpdateOptions { IsUpsert = true };
        //            var filter = Builders<Note>.Filter.Where(x => x.id == note.id);

        //            _mongoClient.excuteMongoLinqUpdate<Note>(filter, note, option,
        //_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, notes,
        //true, "", DateTime.Now.AddMinutes(10));
        //            list.Add(note);
        //        }
        //    }
        //    catch { }
        //    return list;
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

        //       var cs= await Core.Helpers.WebHelper.HttpPostAsync<dynamic>(url, "");
        //        var tt = (string)cs.customers[0].id;
        //        return cs;
        //    }
        //    catch { }
        //    return null;
        //}
    }
}