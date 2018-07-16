using Hibaza.CCP.Data.Infrastructure;
using Hibaza.CCP.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hibaza.CCP.Domain.Models;
using Firebase.Database;
using Hibaza.CCP.Domain.Models.Facebook;
using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Data.Repositories.Firebase;

namespace Hibaza.CCP.Service.Firebase
{
    public class FirebaseCustomerService : IFirebaseCustomerService
    {
        private readonly IFirebaseCustomerRepository _customerRepository;
        private readonly ICounterRepository _counterRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IAgentService _agentService;
        public FirebaseCustomerService(IFirebaseCustomerRepository customerRepository, IAgentService agentService, ICounterRepository counterRepository, IMessageRepository messageRepository)
        {
            _customerRepository = customerRepository;
            _messageRepository = messageRepository;
            _counterRepository = counterRepository;
            _agentService = agentService;
        }

        public static string FormatId(string parent, string key)
        {
            return Core.Helpers.CommonHelper.FormatKey("", key);
        }

        public Domain.Entities.Customer GetById(string business_id, string id)
        {
            return _customerRepository.GetById(business_id, id);
        }


        public string GetAppIdByExtId(string business_id, string ext_id)
        {
            return _customerRepository.GetAppUIDByPageUID(business_id, ext_id);
        }

        public void MapExtIdAndAppId(string business_id, string ext_id, string app_id, string @ref)
        {
            _customerRepository.UpdateAppPageMapping(business_id, ext_id, app_id, @ref);
        }

        public bool UpdateReferral(FacebookMessagingEvent referralEvent)
        {
            var puid = referralEvent.sender.id;
            var auid = _customerRepository.GetAppUIDByPageUID("", puid);

            string @ref = referralEvent.postback != null && referralEvent.postback.referral != null && !string.IsNullOrWhiteSpace(referralEvent.postback.referral.Ref) ? referralEvent.postback.referral.Ref : referralEvent.referral != null && !string.IsNullOrWhiteSpace(referralEvent.referral.Ref) ? referralEvent.referral.Ref : _customerRepository.GetPageReferalParam("", puid);
            _customerRepository.UpdateAppPageMapping("", puid, auid, @ref);
            //if (!string.IsNullOrWhiteSpace(puid))
            //{
            //    var buid = _customerRepository.GetBusinessUIDByPageUID(puid);
            //    if (!string.IsNullOrWhiteSpace(buid))
            //    {
            //        _customerRepository.UpdatePageBusinessMapping(buid, puid, @ref);
            //    }
            //}
            return true;
        }


        public bool CreateCustomer(Domain.Entities.Customer customer)
        {
            _customerRepository.Upsert(customer.business_id, customer);
            return true;
        }

        public async Task<IEnumerable<Customer>> All(string business_id)
        {
            return _customerRepository.GetAll(business_id);
        }

        public Customer CreateCustomer(string business_id, string customer_id, Message message)
        {
            string owner_id = (message.author == message.channel_id ? business_id + "_" + message.recipient_id : message.author);
            var customer = GetById(business_id, customer_id);
            if (customer == null)
            {
                customer = new Domain.Entities.Customer
                {
                    id = customer_id,
                    business_id = business_id
                };
            }
            customer.ext_id = customer.ext_id == message.author ? message.sender_id : message.channel_ext_id;
            customer.avatar = customer.ext_id == message.author ? message.sender_avatar ?? customer.avatar : customer.avatar;
            customer.name = customer.ext_id == message.author ? message.sender_name ?? customer.name : customer.name;
            customer.updated_time = message.created_time;
            customer.timestamp = message.timestamp;

            CreateCustomer(customer);

            return customer;
        }

        public Task<IEnumerable<Customer>> GetCustomers(Paging page, string channelId, string agentId, string status)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Customer>> GetCustomers(string business_id, Paging page, string channelId, string agentId, string status, string flag)
        {
            List<Customer> list = new List<Customer>();
            foreach (var c in await _customerRepository.GetCustomers(business_id, page))
            {
                list.Add(c.Object);
            }
            return list;
        }

        public void Delete(string business_id, string id)
        {
            _customerRepository.Delete(business_id, id);
        }
    }
}




