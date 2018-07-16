using Dapper;
using Hibaza.CCP.Data.Providers.Firebase;
using Hibaza.CCP.Data.Providers.SQLServer;
using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Hibaza.CCP.Domain.Models;
using Firebase.Database.Query;
using Firebase.Database;

namespace Hibaza.CCP.Data.Repositories.Firebase
{
    public class FirebaseCustomerRepository : IFirebaseCustomerRepository
    {
        private const string CUSTOMERS = "customers";
        FirebaseClient _client;
        public FirebaseCustomerRepository(IFirebaseFactory connectionFactory)
        {
            _client = connectionFactory.GetConnection;
        }


        public Customer GetById(string business_id, string id)
        {
            var c = _client.Child(business_id)
            .Child(CUSTOMERS).Child(id).OnceSingleAsync<Customer>().Result;

            return c;
        }

        public Customer GetById(string id)
        {
            var c = _client.Child(CUSTOMERS).Child(id).OnceSingleAsync<Customer>().Result;
            return c;
        }

        public string GetPageReferalParam(string business_id, string puid)
        {
            var rs = _client.Child(business_id).Child("app-page-customer-map").Child(puid).OnceSingleAsync<dynamic>().Result;
            return rs.Object;
        }

        public string GetAppUIDByPageUID(string business_id, string puid)
        {
            var rs = _client.Child(business_id).Child("app-page-customer-map").Child(puid).OnceSingleAsync<dynamic>().Result;
            return rs.Key;
        }

        public void UpdateAppPageMapping(string business_id, string puid, string auid, string @ref)
        {
            var rs = _client.Child(business_id).Child("app-page-customer-map").Child(puid).Child(auid).PutAsync<string>(@ref);
        }


        public void UpdatePageBusinessMapping(string buid, string puid, string @ref)
        {
            var rs = _client.Child("page-business-usermap").Child(puid).PutAsync(new { buid = buid, @ref = @ref });
        }


        public string GetBusinessUIDByPageUID(string puid)
        {
            var rs = _client.Child("page-business-usermap").Child(puid).OnceSingleAsync<dynamic>().Result;
            return rs == null ? "" : (rs.buid ?? "");
        }




        public void Update(string business_id, Customer entity)
        {
            var rs = _client.Child(business_id).Child(CUSTOMERS).Child(entity.id).PutAsync(entity);
        }

        public async Task<dynamic> GetCustomers(string business_id, Paging page)
        {
            long endAt = long.Parse(page.Next ?? "99999999999");
            return await _client.Child(business_id)
            .Child(CUSTOMERS).OrderBy("timestamp").EndAt(endAt).LimitToLast(page.Limit).OnceAsync<Customer>();
        }

        public async Task<dynamic> GetCustomersByFlag(string business_id, Paging page, string flag)
        {
            string startAt = flag + "0000000000";
            string endAt = flag + (page.Next ?? "9999999999");

            return await _client.Child(business_id)
            .Child(CUSTOMERS).OrderBy("flag_timestamp").StartAt(startAt).EndAt(endAt).LimitToLast(page.Limit).OnceAsync<Customer>();
        }

        public async Task<dynamic> GetCustomersByChannelAndFlag(string business_id, Paging page, string channelId, string flag)
        {
            string startAt = channelId + flag + "0000000000";
            string endAt = channelId + flag + (page.Next ?? "9999999999");

            return await _client.Child(business_id)
            .Child(CUSTOMERS).OrderBy("channel_flag_timestamp").StartAt(startAt).EndAt(endAt).LimitToLast(page.Limit).OnceAsync<Customer>();
        }

        public async Task<dynamic> GetCustomersByChannelAndAgentAndFlag(string business_id, Paging page, string channelId, string agentId, string flag)
        {
            string startAt = channelId + agentId + flag + "0000000000";
            string endAt = channelId + agentId + flag + (page.Next ?? "9999999999");

            return await _client.Child(business_id)
            .Child(CUSTOMERS).OrderBy("channel_agent_flag_timestamp").StartAt(startAt).EndAt(endAt).LimitToLast(page.Limit).OnceAsync<Customer>();
        }

        public async Task<dynamic> GetCustomersByAgentAndFlag(string business_id, Paging page, string agentId, string flag)
        {
            string startAt = agentId + flag + "0000000000";
            string endAt = agentId + flag + (page.Next ?? "9999999999");

            return await _client.Child(business_id)
            .Child(CUSTOMERS).OrderBy("agent_flag_timestamp").StartAt(startAt).EndAt(endAt).LimitToLast(page.Limit).OnceAsync<Customer>();
        }


        public async Task<dynamic> GetCustomersByChannel(string business_id, Paging page, string channelId)
        {

            string startAt = channelId + "0000000000";
            string endAt = channelId + (page.Next ?? "9999999999");

            return await _client.Child(business_id)
            .Child(CUSTOMERS).OrderBy("channel_timestamp").StartAt(startAt).EndAt(endAt).LimitToLast(page.Limit).OnceAsync<Customer>();
        }

        public async Task<dynamic> GetCustomersByChannelAndAgent(string business_id, Paging page, string channelId, string agentId)
        {
            string startAt = channelId + agentId + "0000000000";
            string endAt = channelId + agentId + (page.Next ?? "9999999999");
            return await _client.Child(business_id)
            .Child(CUSTOMERS).OrderBy("channel_agent_timestamp").StartAt(startAt).EndAt(endAt).LimitToLast(page.Limit).OnceAsync<Customer>();
        }

        public async Task<dynamic> GetCustomersByChannelAndStatus(string business_id, Paging page, string channelId, string status)
        {
            string startAt = channelId + status + "0000000000";
            string endAt = channelId + status + (page.Next ?? "9999999999");
            return await _client.Child(business_id)
            .Child(CUSTOMERS).OrderBy("channel_status_timestamp").StartAt(startAt).EndAt(endAt).LimitToLast(page.Limit).OnceAsync<Customer>();
        }

        public async Task<dynamic> GetCustomersByAgent(string business_id, Paging page, string agentId)
        {
            string startAt = agentId + "0000000000";
            string endAt = agentId + (page.Next ?? "9999999999");
            return await _client.Child(business_id)
            .Child(CUSTOMERS).OrderBy("agent_timestamp").StartAt(startAt).EndAt(endAt).LimitToLast(page.Limit).OnceAsync<Customer>();
        }

        public async Task<dynamic> GetCustomersByStatus(string business_id, Paging page, string status)
        {
            string startAt = status + "0000000000";
            string endAt = status + (page.Next ?? "9999999999");
            return await _client.Child(business_id)
            .Child(CUSTOMERS).OrderBy("status_timestamp").StartAt(startAt).EndAt(endAt).LimitToLast(page.Limit).OnceAsync<Customer>();
        }


        public async Task<dynamic> GetCustomersByChannelAndAgentAndStatus(string business_id, Paging page, string channelId, string agentId, string status)
        {
            string startAt = channelId + agentId + status + "0000000000";
            string endAt = channelId + agentId + status + (page.Next ?? "9999999999");
            return await _client.Child(business_id)
            .Child(CUSTOMERS).OrderBy("channel_agent_status_timestamp").StartAt(startAt).EndAt(endAt).LimitToLast(page.Limit).OnceAsync<Customer>();
        }

        public async Task<dynamic> GetCustomersByAgentAndStatus(string business_id, Paging page, string agentId, string status)
        {
            string startAt = agentId + status + "0000000000";
            string endAt = agentId + status + (page.Next ?? "9999999999");
            return await _client.Child(business_id)
            .Child(CUSTOMERS).OrderBy("agent_status_timestamp").StartAt(startAt).EndAt(endAt).LimitToLast(page.Limit).OnceAsync<Customer>();
        }

        public bool  Delete(string business_id, string id)
        {
            var rs = _client.Child(business_id).Child(CUSTOMERS).Child(id).DeleteAsync();
            return true;
        }


        public IEnumerable<Customer> GetAll(string business_id)
        {
            return _client.Child(business_id).Child(CUSTOMERS).OnceAsync<Customer>().Result.Select(t => t.Object);
        }

        public IEnumerable<Customer> GetAll()
        {
            return _client.Child(CUSTOMERS).OnceAsync<Customer>().Result.Select(t => t.Object);
        }

        public void Add(Customer entity)
        {
            var rs = _client.Child(CUSTOMERS).Child(entity.id).PutAsync(entity);
        }

        public void Upsert(string business_id, Customer entity)
        {
            var rs = _client.Child(business_id).Child(CUSTOMERS).Child(entity.id).PutAsync(entity);
        }

        public void Update(Customer entity)
        {
            throw new NotImplementedException();
        }

    }
}



        //public string GetAppRefParam(string auid)
        //{
        //    var rs = _connectionFactory.GetConnection.Child("app-page-customer-map").Child(auid).OnceSingleAsync<dynamic>().Result;
        //    return rs == null ? "" : (rs.@ref ?? "");
        //}

        //public string GetPageUIDByAppUID(string auid)
        //{
        //    var rs = _connectionFactory.GetConnection.Child("app-page-customer-map").Child(auid).OnceSingleAsync<dynamic>().Result;
        //    return rs == null ? "" : (rs.puid ?? "");
        //}

        //public void UpdateAppPageMapping(string puid, string auid, string @ref)
        //{
        //    //var rs = _connectionFactory.GetConnection.Child("app-page-customer-map").Child(auid).PostAsync(new { puid = puid, @ref = @ref }).Result;
        //    var rs = _connectionFactory.GetConnection.Child("app-page-customer-map").Child(auid).PutAsync(new { puid = puid, @ref = @ref });
        //}


        //public void UpdatePageBusinessMapping(string buid, string puid, string @ref)
        //{
        //    var rs = _connectionFactory.GetConnection.Child("page-business-usermap").Child(puid).PutAsync(new { buid = buid, @ref = @ref });
        //}


        //public string GetBusinessUIDByPageUID(string puid)
        //{
        //    var rs = _connectionFactory.GetConnection.Child("page-business-usermap").Child(puid).OnceSingleAsync<dynamic>().Result;
        //    return rs == null ? "" : (rs.buid ?? "");
        //}



        //public void Update(Customer entity)
        //{
        //    var rs = _connectionFactory.GetConnection.Child(CUSTOMERS).Child(entity.id).PutAsync(entity);
        //}


        //public Customer GetById(string id)
        //{
        //    var c = _connectionFactory.GetConnection
        //    .Child(CUSTOMERS).Child(id).OnceSingleAsync<Customer>().Result;
        //    return c;
        //}


        //public Customer GetById(string business_id, string id)
        //{
        //    var c = _connectionFactory.GetConnection.Child(business_id)
        //    .Child(CUSTOMERS).Child(id).OnceSingleAsync<Customer>().Result;
        //    return c;
        //}


        //public void Add(string business_id, Customer entity)
        //{
        //    var rs = _connectionFactory.GetConnection.Child(business_id).Child(CUSTOMERS).Child(entity.id).PutAsync(entity);
        //}

        //public void Add(Customer entity)
        //{
        //    var rs = _connectionFactory.GetConnection.Child(CUSTOMERS).Child(entity.id).PutAsync(entity);
        //}
