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
    public class MongoHotlineRepository : IHotlineRepository
    {
        const string hotlines = "Hotlines";
        public MongoFactory _mongoClient ;
        IOptions<AppSettings> _appSettings = null;
        public MongoHotlineRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
                _mongoClient = new MongoFactory(appSettings);
        }

        public async Task<Phone> GetById(string business_id, string id)
        {
            var hotline = new Phone();
            try
            {
                var key = "Hotline_GetById" + business_id + id;                
                var options = new FindOptions<Phone, Phone>();
                options.Limit = 1;
                //options.Sort = Builders<Note>.Sort.Descending("created_time");
                var query = "{business_id:\"" + business_id + "\",agent_id:\"" + id + "\"}";
                var rs = await _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, hotlines,
                     false, key, DateTime.Now.AddMinutes(10), false);
                if (rs != null && rs.Count > 0)
                    return rs[0];
                return hotline;
            }
            catch { return hotline; }
        }
        
        public async Task<dynamic> Upsert(Phone hotline)
        {
            try
            {
                var option = new UpdateOptions { IsUpsert = true };
                var filter = Builders<Phone>.Filter.Where(x => x.id == hotline.id && x.business_id == hotline.business_id);

                return await _mongoClient.excuteMongoLinqUpdate<Phone>(filter, hotline, option,
      _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, hotlines,
      true, "", DateTime.Now.AddMinutes(10));
            }
            catch
            {
                return null;
            }
        }


        public async Task<List<Phone>> GetAll(string business_id, Paging page)
        {
            var list = new List<Phone>();
            try
            {
                var key = "Hotline_GetAll" + business_id;

                var options = new FindOptions<Phone, Phone>();
                options.Limit = 1;
                var query = "{business_id:\"" + business_id + "\"}";
                return await _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, hotlines,
                     true, key, DateTime.Now.AddMinutes(10), true);
            }
            catch { return list; }
        }

        public async Task<dynamic> getCustomerInfoFromPhone(string phone)
        {
            try
            {
                var dic = new Dictionary<string, object>();
                var customer = new List<Customer>();

                var key = "getCustomerFormPhone" + phone;
                var options = new FindOptions<Customer, Customer>();
                options.Limit = 1;
                options.Projection = "{phone:1,phone_list:1,name:1,email:1,business_id:1:sex:1,id:1,_id:0,weight:1,height:1,address:1}";
                //options.Sort = Builders<Note>.Sort.Descending("created_time");
                var query = "{phone_list:\"" + phone + "\"}";
                customer = await _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, "Customers",
                     false, key, DateTime.Now.AddMinutes(10), false);


                if (customer == null || customer.Count == 0)
                    return null;
                // orders

                var options1 = new FindOptions<BsonDocument, BsonDocument>();
                options1.Limit = 1;
                options1.Projection = "{_id:0,id:1,orderid:1,order_status:1,products:1,transact_status:1,email:1,message:1,created_time:1,timestamp:1}";
                options1.Sort = Builders<BsonDocument>.Sort.Descending("created_time");
                var query1 = "{customer_id:\"" + customer[0].id + "\"}";
                var orders = await _mongoClient.excuteMongoLinqSelect(query1, options1,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, "Orders",
                     false, "", DateTime.Now.AddMinutes(10), false);

                //Carts

                var options2 = new FindOptions<BsonDocument, BsonDocument>();
                options2.Limit = 1;
                options2.Projection = "{_id:0,id:1,orderid:1,order_status:1,products:1,transact_status:1,email:1,message:1,created_time:1,timestamp:1}";
                options2.Sort = Builders<BsonDocument>.Sort.Descending("created_time");
                var query2 = "{customer_id:\"" + customer[0].id + "\"}";
                var carts = await _mongoClient.excuteMongoLinqSelect(query2, options2,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, "Carts",
                     false, "", DateTime.Now.AddMinutes(10), false);

                // them ben bara

                //var orders = await rf.GetAsync(webSetting["shoporderurl"] + "api/order/list?logonId=" + qr + "&imei=&token=@bazavietnam");
                //var orderBs = JsonConvert.DeserializeObject(orders);
                //foreach (var orderId in (JArray)orderBs)
                //{
                //    if (orderId["OrderId"].ToString() != "")
                //    {
                //        var urlDetail = webSetting["shoporderurl"] + "api/order/detail2?id=" + orderId["OrderId"] + "&logonId=" + qr + "&imei=&token=@bazavietnam";
                //        var orderDetail = await rf.GetAsync(urlDetail);

                //        var rs = await mg.ConvertOrderToJsonAsync(orderDetail, strShort, "orderview", "");
                //        lst += rs + ",";
                //    }
                //}

                if (customer == null || customer.Count == 0)
                    return null;

                dic.Add("customer", customer[0]);
                dic.Add("orders", orders);
                dic.Add("carts", carts);
                return JsonConvert.SerializeObject(dic);
            }
            catch (Exception) { return null; }
        }

       
    }
}