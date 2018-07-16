using Hibaza.CCP.Data.Infrastructure;
using Hibaza.CCP.Data.Repositories;
using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hibaza.CCP.Domain.Models;
using Firebase.Database;
using Hibaza.CCP.Data.Repositories.Firebase;

namespace Hibaza.CCP.Service.Firebase
{
    public class FirebaseMessageService : IFirebaseMessageService
    {
        private readonly IFirebaseMessageRepository _messageRepository;
        private readonly ICounterRepository _counterRepository;
        public FirebaseMessageService(IFirebaseMessageRepository messageRepository, ICounterRepository counterRepository)
        {
            _messageRepository = messageRepository;
            _counterRepository = counterRepository;
        }
        public static string FormatId(string parent, string key)
        {
            return Core.Helpers.CommonHelper.FormatKey("", key);
        }

        public MessageModel GetById(string business_id, string id)
        {
            return _messageRepository.GetById(business_id, id);
        }

        public IEnumerable<T> All<T>(string business_id, Paging page)
        {
            return _messageRepository.GetAll<T>(business_id, page);
        }

        public IEnumerable<MessageModel> GetByUser(string business_id, Paging page, string id)
        {
            var data = _messageRepository.GetMessagesByUser(business_id, page, id).Result;
            var list = new List<MessageModel>();
            foreach (var m in data)
            {
                list.Add(m.Object);
            }
            return list;
        }
        public void DeleteByThread(string business_id, string thread_id)
        {
            _messageRepository.DeleteAllForThread(business_id, thread_id);
        } 

        public Task<dynamic> GetByThread(string business_id, Paging page, string id)
        {
            var rs = _messageRepository.GetMessagesByThread(business_id, page, id);
            return rs;
        }

        public void Delete(string business_id, string id)
        {
            _messageRepository.Delete(business_id, id);
        }

        public void Delete(string business_id, string thread_id, string id)
        {
            _messageRepository.Delete(business_id, thread_id, id);
        }


        public bool CreateMessage(string business_id, MessageModel message)
        {
            //_messageRepository.Add(message);
            _messageRepository.Upsert(business_id, message);
            _messageRepository.AddGroupedByThread(business_id, message, message.thread_id);

            //if (message.sender_id != message.channel_id && message.sender_id != message.agent_id)
            //{
            //    var counter = new Counter { id = message.sender_id, count = 1 };
            //    _counterRepository.AddChannels(message.channel_id, counter);
            //    _counterRepository.AddChannelsUnread(message.channel_id, counter);

            //    if (string.IsNullOrWhiteSpace(message.agent_id))
            //    {
            //        _counterRepository.AddChannelsUnassignedUnread(message.channel_id, counter);
            //    }
            //    else
            //    {
            //        _counterRepository.AddAgents(message.agent_id, counter);
            //        _counterRepository.AddAgentsUnread(message.agent_id, counter);
            //    }
            //}
            return true;
        }

        public bool AssignMessageToAgent(string business_id, MessageModel message, string agentId)
        {
            _messageRepository.AddGroupedByUser(business_id, message, agentId);
            if (message.sender_id != message.channel_id && message.sender_id != message.agent_id && !string.IsNullOrWhiteSpace(agentId) && agentId != "ALL")
            {
                var counter = new Counter { id = message.sender_id, count = 1};
                _counterRepository.AddThreadToAgents(business_id, agentId, counter);
                _counterRepository.AddTheadToAgentsUnread(business_id, agentId, counter);
            }
            return true;
        }

        public bool SendMessageToCustomer(string business_id, MessageModel message, Domain.Models.Facebook.FacebookUserFeed to)
        {
            if (to != null && to.data != null)
            {
                foreach (var u in to.data)
                {
                    _messageRepository.AddGroupedByUser(business_id, message, u.id);
                }
            }
            return true;
        }

        public bool SendMessageToThread(string business_id, MessageModel message, string thread_id)
        {
            //_messageRepository.Add(message);
            _messageRepository.Upsert(business_id, message);
            _messageRepository.AddGroupedByThread(business_id, message, thread_id);
            //var counter = new Counter { id = thread_id, count = 1 };
            //_counterRepository.AddChannels(business_id, message.channel_id, counter);
            //if (!string.IsNullOrWhiteSpace(thread.agent_id))
            //{
            //    _counterRepository.AddAgents(business_id, thread.agent_id, counter);
            //}
            //if (thread.unread)
            //{
            //    _counterRepository.AddChannelsUnread(business_id, message.channel_id, counter);
            //    if (string.IsNullOrWhiteSpace(thread.agent_id))
            //    {
            //        _counterRepository.AddChannelsUnassignedUnread(business_id, message.channel_id, counter);
            //    }
            //    else
            //    {
            //        _counterRepository.AddAgentsUnread(business_id, thread.agent_id, counter);
            //    }
            //}
            //else
            //{
            //    _counterRepository.DeleteChannelsUnread(business_id, message.channel_id, counter.id);
            //    if (string.IsNullOrWhiteSpace(thread.agent_id))
            //    {
            //        _counterRepository.DeleteChannelsUnassignedUnread(business_id, message.channel_id, counter.id);
            //    }
            //    else
            //    {
            //        _counterRepository.DeleteAgentsUnread(business_id, thread.agent_id, counter.id);
            //    }
            //}

            return true;
        }

        public bool SendMessageToThreadWithCountersUpdate(string business_id, MessageModel message, Thread thread)
        {
            //_messageRepository.Add(message);
            _messageRepository.Upsert(business_id, message);
            _messageRepository.AddGroupedByThread(business_id, message, thread.id);
            var counter = new Counter { id = thread.id, count = 1 };
            _counterRepository.AddThreadToChannels(business_id, message.channel_id, counter);
            if (!string.IsNullOrWhiteSpace(thread.agent_id))
            {
                _counterRepository.AddThreadToAgents(business_id, thread.agent_id, counter);
            }
            if (thread.unread)
            {
                _counterRepository.AddThreadToChannelsUnread(business_id, message.channel_id, counter);
                if (string.IsNullOrWhiteSpace(thread.agent_id))
                {
                    _counterRepository.AddThreadToChannelsUnassignedUnread(business_id, message.channel_id, counter);
                }
                else
                {
                    _counterRepository.AddTheadToAgentsUnread(business_id, thread.agent_id, counter);
                }
            }
            else
            {
                try
                {
                    _counterRepository.DeleteThreadFromChannelsUnread(business_id, message.channel_id, counter.id);
                }
                catch { }
                if (string.IsNullOrWhiteSpace(thread.agent_id))
                {
                    try
                    {
                        _counterRepository.DeleteThreadFromChannelsUnassignedUnread(business_id, message.channel_id, counter.id);
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        _counterRepository.DeleteThreadFromAgentsUnread(business_id, thread.agent_id, counter.id);
                    }
                    catch { }
                }
            }

            return true;
        }

        public string CreateMessageAgentMap(string business_id, string message_id, string agent_id)
        {
            return _messageRepository.CreateMessageAgentMap(business_id, message_id, agent_id);
        }
        public string GetMessageAgentMap(string business_id, string message_id)
        {
            return _messageRepository.GetMessageAgentMap(business_id, message_id);
        }

    }

}
