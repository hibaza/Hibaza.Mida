using Hibaza.CCP.Data.Infrastructure;
using Hibaza.CCP.Data.Repositories;
using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hibaza.CCP.Domain.Models;
using MongoDB.Driver;

namespace Hibaza.CCP.Service
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _conversationRepository;

        public static string FormatId(string parent, string key)
        {
            return Core.Helpers.CommonHelper.FormatKey("", key);
        }

        public ConversationService(IConversationRepository conversationRepository)
        {
            _conversationRepository = conversationRepository;
        }

        public bool Insert(Conversation conversation)
        {
            var c = GetById(conversation.business_id, conversation.id);
            if (c == null)
            {
               return  _conversationRepository.Insert(conversation);
            }
            return false;
        }

        public void Upsert(Conversation conversation)
        {
            _conversationRepository.Upsert(conversation);
        }

        public bool UpdateOwner(string business_id, string id, string owner_ext_id, string owner_app_id)
        {
            return _conversationRepository.UpdateOwner(business_id, id, owner_ext_id, owner_app_id);
        }

        public void UpsertTimestamp(Conversation conversation)
        {
            _conversationRepository.UpsertTimestamp(conversation);
        }


        public Conversation GetById(string business_id, string id)
        {
            return _conversationRepository.GetById(business_id, id);
        }

        public bool UpdateStatus(string business_id, string id, string status)
        {
            _conversationRepository.UpdateStatus(business_id, id, status);
            return true;
        }

        public async Task<IEnumerable<Conversation>> GetConversations(string business_id, string channel_id, Paging page)
        {
            return await _conversationRepository.GetConversations(business_id, channel_id, page);
        }

        public async Task<IEnumerable<Conversation>> GetConversationWhereExtIdIsNull(string business_id, string channel_id, int limit)
        {
            return await _conversationRepository.GetConversationsWhereExtIdIsNull(business_id, channel_id, limit);
        }

        public Conversation GetByOwnerExtId(string business_id, string owner_ext_id)
        {
            return _conversationRepository.GetByOwnerExtId(business_id, owner_ext_id);
        }
        public Conversation GetByOwnerAppId(string business_id, string channel_id, string owner_app_id)
        {
            return _conversationRepository.GetByOwnerAppId(business_id, channel_id, owner_app_id);
        }
        public async Task<ReplaceOneResult> UpsertAnyMongo<T>(T obj, UpdateOptions option, FilterDefinition<T> filter, string collectionName) where T : class
        {
            return await _conversationRepository.UpsertAnyMongo<T>(obj,option,filter, collectionName);
        }
        public async Task<List<T>> GetDataMongo<T>(string query, FindOptions<T> options, string collectionName) where T : class
        {
            return await _conversationRepository.GetDataMongo<T>(query, options, collectionName);
        }
    }
}
