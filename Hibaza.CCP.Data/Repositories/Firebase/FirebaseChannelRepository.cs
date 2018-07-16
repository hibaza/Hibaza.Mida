using Dapper;
using Firebase.Database.Query;
using Hibaza.CCP.Data.Providers.Firebase;
using Hibaza.CCP.Data.Providers.SQLServer;
using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories.Firebase
{
    public class FirebaseChannelRepository : IFirebaseChannelRepository
    {
        IFirebaseFactory _connectionFactory;
        private const string CHANNELS = "channels";
        public FirebaseChannelRepository(IFirebaseFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Add(Channel entity)
        {
            var rs = _connectionFactory.GetConnection.Child(CHANNELS).Child(entity.id).PutAsync(entity);
        }

        public void Upsert(string business_id, Channel data)
        {
            var rs = _connectionFactory.GetConnection.Child(business_id).Child(CHANNELS).Child(data.id).PutAsync(data);
        }


        public void Update(Channel entity)
        {
            throw new NotImplementedException();
        }

        public Channel GetById(string id)
        {
            var c = _connectionFactory.GetConnection
              .Child(CHANNELS).Child(id).OnceSingleAsync<Channel>().Result;
            return c;
        }

        public Channel GetById(string business_id, string id)
        {
            var c = _connectionFactory.GetConnection.Child(business_id)
              .Child(CHANNELS).Child(id).OnceSingleAsync<Channel>().Result;
            return c;
        }

        public async Task<dynamic> GetChannels(Paging page)
        {
            return _connectionFactory.GetConnection.Child(CHANNELS).OrderBy("created_time").LimitToLast(page.Limit).OnceAsync<Channel>().Result;
        }

        public async Task<dynamic> GetChannelsByExtId(string ext_id, Paging page)
        {
            return _connectionFactory.GetConnection.Child(CHANNELS).OrderBy("ext_id").EqualTo(ext_id).LimitToLast(page.Limit).OnceAsync<Channel>().Result;
        }

        public async Task<dynamic> GetChannels(string business_id, Paging page)
        {
            return _connectionFactory.GetConnection.Child(business_id).Child(CHANNELS).OrderBy("created_time").LimitToLast(page.Limit).OnceAsync<Channel>().Result;
        }

        public bool Delete(string id)
        {
            var c = _connectionFactory.GetConnection.Child(CHANNELS).Child(id).DeleteAsync();
            return true;
        }

        public bool Delete(string bussiness_id, string id)
        {
            var c = _connectionFactory.GetConnection.Child(bussiness_id).Child(CHANNELS).Child(id).DeleteAsync();
            return true;
        }

        public IEnumerable<Channel> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Channel> GetAll(string business_id)
        {
            throw new NotImplementedException();
        }


        public void Update(string business_id, Channel entity)
        {
            throw new NotImplementedException();
        }
    }
}
