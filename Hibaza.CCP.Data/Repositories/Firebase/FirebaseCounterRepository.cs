﻿using Dapper;
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
    public class FirebaseCounterRepository : ICounterRepository
    {
        IFirebaseFactory _connectionFactory;
        private const string COUNTERS = "counters";
        private const string THREADS_ALL = "threads-counters-all";
        private const string THREADS_UNREAD = "threads-counters-unread";
        private const string COUNTERS_CHANNELS = "channels";
        private const string COUNTERS_CHANNELS_UNREAD = "channels_unread";
        private const string COUNTERS_AGENTS = "agents";
        private const string COUNTERS_AGENTS_UNREAD = "attention_unread";
        private const string COUNTERS_CHANNELS_UNASSIGNED_UNREAD = "pending_unread";

        public FirebaseCounterRepository(IFirebaseFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Upsert(string business_id, Counter entity)
        {
            var rs = _connectionFactory.GetConnection.Child(business_id).Child(COUNTERS).Child(entity.id).PutAsync(entity);
        }

        public void Update(string business_id, Counter entity)
        {
            throw new NotImplementedException();
        }

        public Counter GetById(string business_id, string id)
        {
            var c = _connectionFactory.GetConnection.Child(business_id)
              .Child(COUNTERS).Child(id).OnceSingleAsync<Counter>().Result;
            return c;
        }


        public bool Delete(string business_id, string id)
        {
            var c = _connectionFactory.GetConnection.Child(business_id).Child(COUNTERS).Child(id).DeleteAsync();
            return true;
        }

        public bool DeleteAll(string business_id)
        {
            var c = _connectionFactory.GetConnection.Child(business_id).Child(COUNTERS).DeleteAsync();
            return true;
        }

        public IEnumerable<Counter> GetAll(string business_id)
        {
            throw new NotImplementedException();
        }


        public void AddThreadToChannels(string business_id, string channel_id, Counter entity)
        {
            //var rs = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_ALL).Child(THREADS_ALL + "-data").Child(COUNTERS).Child(COUNTERS_CHANNELS).Child(channel_id).Child(entity.id).PutAsync(entity).ContinueWith((t) =>
            //{
            //    var count = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_ALL).Child(THREADS_ALL + "-data").Child(COUNTERS).Child(COUNTERS_CHANNELS).Child(channel_id).OnceAsync<Counter>().Result;
            //    var rs1 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_ALL).Child(THREADS_ALL + "-count").Child(COUNTERS).Child(COUNTERS_CHANNELS).Child(channel_id).PutAsync(count == null ? 0 : count.Count());
            //});

            var count = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_ALL).Child(THREADS_ALL + "-count").Child(COUNTERS).Child(COUNTERS_CHANNELS).Child(channel_id).OnceSingleAsync<int>().Result;
            var rs1 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_ALL).Child(THREADS_ALL + "-count").Child(COUNTERS).Child(COUNTERS_CHANNELS).Child(channel_id).PutAsync(count + 1);

        }

        public void AddThreadToChannelsUnread(string business_id, string channel_id, Counter entity)
        {
            //var rs = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-data").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNREAD).Child(channel_id).Child(entity.id).PutAsync(entity).ContinueWith((t) =>
            //{
            //    var count = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-data").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNREAD).Child(channel_id).OnceAsync<Counter>().Result;
            //    var rs1 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNREAD).Child(channel_id).PutAsync(count == null ? 0 : count.Count());
            //});


            var count = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNREAD).Child(channel_id).OnceSingleAsync<int>().Result;
            var rs1 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNREAD).Child(channel_id).PutAsync(count + 1);
        }

        public void AddThreadToChannelsUnassignedUnread(string business_id, string channel_id, Counter entity)
        {
            //var rs = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-data").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNASSIGNED_UNREAD).Child(channel_id).Child(entity.id).PutAsync(entity).ContinueWith((t) =>
            //{
            //    var count = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-data").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNASSIGNED_UNREAD).Child(channel_id).OnceAsync<Counter>().Result;
            //    var rs1 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNASSIGNED_UNREAD).Child(channel_id).PutAsync(count == null ? 0: count.Count());
            //});
            var count = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNASSIGNED_UNREAD).Child(channel_id).OnceSingleAsync<int>().Result;
            var rs1 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNASSIGNED_UNREAD).Child(channel_id).PutAsync(count + 1);
        }

        public void AddThreadToAgents(string business_id, string agent_id, Counter entity)
        {
            //var rs = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_ALL).Child(THREADS_ALL + "-data").Child(COUNTERS).Child(COUNTERS_AGENTS).Child(agent_id).Child(entity.id).PutAsync(entity).ContinueWith((t) =>
            //{
            //    var count = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_ALL).Child(THREADS_ALL + "-data").Child(COUNTERS).Child(COUNTERS_AGENTS).Child(agent_id).OnceAsync<Counter>().Result;
            //    var rs1 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_ALL).Child(THREADS_ALL + "-count").Child(COUNTERS).Child(COUNTERS_AGENTS).Child(agent_id).PutAsync(count == null ? 0 : count.Count());
            //});
            var count = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_ALL).Child(THREADS_ALL + "-count").Child(COUNTERS).Child(COUNTERS_AGENTS).Child(agent_id).OnceSingleAsync<int>().Result;
            var rs1 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_ALL).Child(THREADS_ALL + "-count").Child(COUNTERS).Child(COUNTERS_AGENTS).Child(agent_id).PutAsync(count + 1);

        }

        public void AddTheadToAgentsUnread(string business_id, string agent_id, Counter entity)
        {
            //var rs = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-data").Child(COUNTERS).Child(COUNTERS_AGENTS_UNREAD).Child(agent_id).Child(entity.id).PutAsync(entity).ContinueWith((t) =>
            //{
            //    var r = GetThreadUnreadCountByAgent(business_id, agent_id);
            //    var rs1 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_AGENTS_UNREAD).Child(agent_id).PutAsync(r == null ? 0 : r.Count());
            //});

            var count = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_AGENTS_UNREAD).Child(agent_id).OnceSingleAsync<int>().Result;
            var rs1 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_AGENTS_UNREAD).Child(agent_id).PutAsync(count + 1);
        }

        public async Task<Dictionary<string, int>> GetAgentsUnreadCount(string business_id)
        {
            var rs = await _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_AGENTS_UNREAD).OnceAsync<int>();
            Dictionary<string, int> counters = new Dictionary<string, int>();
            foreach (var c in rs)
            {
                counters.Add(c.Key, c.Object);
            }
            return counters;
        }

        public void DeleteThreadFromChannels(string business_id, string channel_id, string id)
        {
            //var rs = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_ALL).Child(THREADS_ALL + "-data").Child(COUNTERS).Child(COUNTERS_CHANNELS).Child(channel_id).Child(id).DeleteAsync().ContinueWith((t) =>
            //{
            //    var count = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_ALL).Child(THREADS_ALL + "-data").Child(COUNTERS).Child(COUNTERS_CHANNELS).Child(channel_id).OnceAsync<Counter>().Result;
            //    var rs1 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_ALL).Child(THREADS_ALL + "-count").Child(COUNTERS).Child(COUNTERS_CHANNELS).Child(channel_id).PutAsync(count == null ? 0 : count.Count());
            //});

            var count = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_ALL).Child(THREADS_ALL + "-count").Child(COUNTERS).Child(COUNTERS_CHANNELS).Child(channel_id).OnceSingleAsync<int>().Result;
            var rs1 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_ALL).Child(THREADS_ALL + "-count").Child(COUNTERS).Child(COUNTERS_CHANNELS).Child(channel_id).PutAsync(count > 0 ? count - 1 : 0);

        }

        public void DeleteThreadFromChannelsUnread(string business_id, string channel_id, string id)
        {
            //var rs = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-data").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNREAD).Child(channel_id).Child(id).DeleteAsync().ContinueWith((t) =>
            //{
            //    var count = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-data").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNREAD).Child(channel_id).OnceAsync<Counter>().Result;
            //    var rs1 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNREAD).Child(channel_id).PutAsync(count == null ? 0 : count.Count());
            //});

            var count = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNREAD).Child(channel_id).OnceSingleAsync<int>().Result;
            var rs1 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNREAD).Child(channel_id).PutAsync(count > 0 ? count - 1 : 0);

        }

        public void DeleteThreadFromChannelsUnassignedUnread(string business_id, string channel_id, string id)
        {
            //var rs = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-data").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNASSIGNED_UNREAD).Child(channel_id).Child(id).DeleteAsync().ContinueWith((t) =>
            //{
            //    var count = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-data").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNASSIGNED_UNREAD).Child(channel_id).OnceAsync<Counter>().Result;
            //    var rs1 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNASSIGNED_UNREAD).Child(channel_id).PutAsync(count == null ? 0 : count.Count());
            //});

            var count = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNASSIGNED_UNREAD).Child(channel_id).OnceSingleAsync<int>().Result;
            var rs1 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNASSIGNED_UNREAD).Child(channel_id).PutAsync(count > 0 ? count - 1 : 0);


        }

        public void DeleteThreadFromAgentsAll(string business_id, string agent_id, string id)
        {
            //var rs = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_ALL).Child(THREADS_ALL + "-data").Child(COUNTERS).Child(COUNTERS_AGENTS).Child(agent_id).Child(id).DeleteAsync().ContinueWith((t) =>
            //{
            //    var count = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_ALL).Child(THREADS_ALL + "-data").Child(COUNTERS).Child(COUNTERS_AGENTS).Child(agent_id).OnceAsync<Counter>().Result;
            //    var rs1 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_ALL).Child(THREADS_ALL + "-count").Child(COUNTERS).Child(COUNTERS_AGENTS).Child(agent_id).PutAsync(count == null ? 0 : count.Count());
            //});

            var count = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_ALL).Child(THREADS_ALL + "-count").Child(COUNTERS).Child(COUNTERS_AGENTS).Child(agent_id).OnceSingleAsync<int>().Result;
            var rs1 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_ALL).Child(THREADS_ALL + "-count").Child(COUNTERS).Child(COUNTERS_AGENTS).Child(agent_id).PutAsync(count > 0 ? count - 1 : 0);

        }

        public void DeleteThreadFromAgentsUnread(string business_id, string agent_id, string id)
        {
            //var r = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-data").Child(COUNTERS).Child(COUNTERS_AGENTS_UNREAD).Child(agent_id).Child(id).DeleteAsync();
            //var rs = GetThreadUnreadCountByAgent(business_id, agent_id);
            //var rs1 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_AGENTS_UNREAD).Child(agent_id).PutAsync(rs == null ? 0 : rs.Count());

            var count = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_AGENTS_UNREAD).Child(agent_id).OnceSingleAsync<int>().Result;
            var rs1 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_AGENTS_UNREAD).Child(agent_id).PutAsync(count > 0 ? count - 1 : 0);

        }


        public async System.Threading.Tasks.Task DeleteAllThreadCountersFromAgents(string business_id, string agent_id)
        {
            //var rs = await _connectionFactory.GetConnection.Child(business_id).Child(THREADS_ALL).Child(THREADS_ALL + "-data").Child(COUNTERS).Child(COUNTERS_AGENTS).Child(agent_id).DeleteAsync().ContinueWith(async (t) =>
            //{
            //    var r = GetThreadUnreadCountByAgent(business_id, agent_id);
            //    await _connectionFactory.GetConnection.Child(business_id).Child(THREADS_ALL).Child(THREADS_ALL + "-count").Child(COUNTERS).Child(COUNTERS_AGENTS).Child(agent_id).PutAsync(r == null ? 0 : r.Count());
            //});

            var count = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_ALL).Child(THREADS_ALL + "-count").Child(COUNTERS).Child(COUNTERS_AGENTS).Child(agent_id).OnceSingleAsync<int>().Result;
            await _connectionFactory.GetConnection.Child(business_id).Child(THREADS_ALL).Child(THREADS_ALL + "-count").Child(COUNTERS).Child(COUNTERS_AGENTS).Child(agent_id).PutAsync(count > 0 ? count - 1 : 0);

        }


        public async System.Threading.Tasks.Task DeleteAllUnreadThreadCountersFromAgent(string business_id, string agent_id)
        {
            //var rs = await _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-data").Child(COUNTERS).Child(COUNTERS_AGENTS_UNREAD).Child(agent_id).DeleteAsync().ContinueWith(async (t) =>
            //{
            //    var r = GetThreadUnreadCountByAgent(business_id, agent_id);
            //    await _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_AGENTS_UNREAD).Child(agent_id).PutAsync(r == null ? 0 : r.Count());
            //});

            var count = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_AGENTS_UNREAD).Child(agent_id).OnceSingleAsync<int>().Result; 
            await _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_AGENTS_UNREAD).Child(agent_id).PutAsync(count > 0 ? count - 1 : 0);
        }


        public IEnumerable<Counter>  GetThreadUnreadCountByAgent(string business_id, string agent_id)
        {

            var rs = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-data").Child(COUNTERS).Child(COUNTERS_AGENTS_UNREAD).Child(agent_id).OnceAsync<Counter>().Result;
            return rs == null ? null : rs.Select(c => c.Object);
        }

        public Counter GetById(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Counter> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Add(Counter entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(string id)
        {
            throw new NotImplementedException();
        }

        public void Update(Counter entity)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAllUnreadCountersData(string business_id)
        {
            var rs = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-data").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNASSIGNED_UNREAD).DeleteAsync();
            var rs1 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-data").Child(COUNTERS).Child(COUNTERS_AGENTS_UNREAD).DeleteAsync();
            var rs2 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-data").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNREAD).DeleteAsync();
            return true;
        }

        public void UpdateChannelsUnreadThreadsCount(string business_id, string channel_id)
        {
            var count1 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-data").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNASSIGNED_UNREAD).Child(channel_id).OnceAsync<Counter>().Result;
            var rs1 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNASSIGNED_UNREAD).Child(channel_id).PutAsync(count1 == null ? 0 : count1.Count());

            var count2 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-data").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNREAD).Child(channel_id).OnceAsync<Counter>().Result;
            var rs2 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNREAD).Child(channel_id).PutAsync(count2 == null ? 0 : count2.Count());
        }

        public void UpdateAgentsUnreadThreadsCount(string business_id, string agent_id)
        {
            var rs = GetThreadUnreadCountByAgent(business_id, agent_id);
            var rs1 = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_AGENTS_UNREAD).Child(agent_id).PutAsync(rs == null ? 0 : rs.Count());

        }

        public void SetChannelsUnreadThreadsCount(string business_id, string channel_id, int count)
        {
            var rs = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_CHANNELS_UNREAD).Child(channel_id).PutAsync(count);

        }

        public void SetAgentsUnreadThreadsCount(string business_id, string agent_id, int count)
        {
            var rs = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_UNREAD).Child(THREADS_UNREAD + "-count").Child(COUNTERS).Child(COUNTERS_AGENTS_UNREAD).Child(agent_id).PutAsync(count);
        }
    }
}
