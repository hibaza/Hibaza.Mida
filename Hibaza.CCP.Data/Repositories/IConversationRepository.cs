using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories
{
    public interface IConversationRepository
    {
        bool Insert(Conversation conversation);
        void Upsert(Conversation conversation);
        void UpsertTimestamp(Conversation conversation);
        bool UpdateStatus(string business_id, string id, string status);
        bool UpdateOwner(string business_id, string id, string owner_ext_id, string owner_app_id);
        Conversation GetById(string business_id, string conversation_id);
        Task<IEnumerable<Conversation>> GetConversationsWhereExtIdIsNull(string business_id, string channel_id, int limit);
        Conversation GetByOwnerExtId(string business_id, string owner_ext_id);
        Conversation GetByOwnerAppId(string business_id, string channel_id, string owner_app_id);
        Task<IEnumerable<Conversation>> GetConversations(string business_id, string channel_id, Paging page);
        Task<ReplaceOneResult> UpsertAnyMongo<T>(T obj, UpdateOptions option, FilterDefinition<T> filter, string collectionName) where T:class;
        Task<List<T>> GetDataMongo<T>(string query, FindOptions<T> options, string collectionName) where T : class;
    }
}
