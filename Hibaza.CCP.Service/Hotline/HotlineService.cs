using Firebase.Storage;
using Hangfire;
using Hibaza.CCP.Core;
using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using Hibaza.CCP.Domain.Models.Facebook;
using Hibaza.CCP.Service.SQL;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Hibaza.CCP.Service.Hotline
{
    public interface IHotlineService
    {
        Task<dynamic> SavePhoneHookData(Phone data);
    }

    public class HotlineService : IHotlineService
    {
        private readonly IMessageService _messageService;
        private readonly IChannelService _channelService;
        private readonly ICustomerService _customerService;
        private readonly IThreadService _threadService;
        private readonly ILoggingService _logService;
        private readonly IBusinessService _businessService;

        public HotlineService(ICustomerService customerService, IThreadService threadService, IChannelService channelService,
             IMessageService messageService, ILoggingService logService, IBusinessService businessService)
        {
            _messageService = messageService;
            _channelService = channelService;
            _customerService = customerService;
            _threadService = threadService;
            _logService = logService;
            _businessService = businessService;
        }

        public async Task<dynamic> SavePhoneHookData(Phone data)
        {
            try
            {
                if (data == null || string.IsNullOrWhiteSpace(data.customer_phone))
                    return new { status = false };
               
                var channel = _channelService.GetById(data.business_id,data.channel_id);
                var business = _businessService.GetById(data.business_id);

                var customer = await CreateCustomer(data, business, channel);
                if (customer == null)
                    return new { status = false };
                var thread = await CreateThread(data, customer, business, channel);

                var messager = await CreateMessage(data, thread, customer, business, channel);


                if (customer != null)
                {
                    customer.active_thread = JsonConvert.SerializeObject(thread);
                    customer.agent_id = data.agent_id;
                }
                if (customer != null && thread != null && messager != null)
                {
                    _customerService.CreateCustomer(customer, true);
                    _threadService.CreateThread(thread, true);
                    _messageService.CreateMessage(data.business_id, messager, true);

                    return new
                    {
                        type = "upsert_hibaza",
                        data = new
                        {
                            status = true,
                            customer_id = customer.id,
                            thread_id = thread.id,
                            message_id = messager.id,
                            agent_id = data.agent_id
                        }
                    };
                }
                else
                {
                    return new
                    {
                        type = "upsert_hibaza",
                        data = new
                        {
                            status = false,
                            customer_id = customer.id,
                            thread_id = thread.id,
                            message_id = messager.id,
                            agent_id = data.agent_id
                        }
                    };
                }


                //if (string.IsNullOrWhiteSpace(data.business_id) || string.IsNullOrWhiteSpace(data.channel_id))
                //{
                //    var customers = await _customerService.GetCustomerFromPhone(data.customer_phone);
                //    if (customers != null && customers.Count == 1)
                //    {
                //        data.business_id = customers[0].business_id;
                //        data.channel_id = customers[0].channel_id;
                //    }

                //    if (customers != null && customers.Count > 1)
                //    {
                //        var business_id = customers[0].business_id;
                //        var channel_id = customers[0].channel_id;
                //        foreach (var cus in customers)
                //        {
                //            if (string.IsNullOrWhiteSpace(cus.business_id) && cus.business_id != business_id)
                //            {
                //                business_id = "";
                //                break;
                //            }

                //        }
                //        if (!string.IsNullOrWhiteSpace(business_id))
                //        {
                //            data.business_id = business_id;
                //            data.channel_id = channel_id;
                //        }
                //    }

                //}
                //if (string.IsNullOrWhiteSpace(data.business_id))
                //{
                //    return new
                //    {
                //        type = "upsert_hibaza",
                //        data = new
                //        {
                //            status = false,
                //            customer_id = "",
                //            thread_id = "",
                //            message_id = "",
                //            agent_id = data.agent_id
                //        }
                //    };
                //}
            }
            catch (Exception ex) {
                _logService.CreateAsync(new Log { name = "Webhook hotline..", message = ex.Message, details = JsonConvert.SerializeObject(ex.StackTrace) });

                return null; }
        }

        public async Task<bool> upsertCustomerCallRegistor(string customer_phone, string customer_fullname, string token_client)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token_client))
                    return false;
                var business = await _businessService.GetBusinessFromTokenClient(token_client);

                //await CreateCustomer(business.id, customer_phone, customer_fullname);
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public async Task<Message> CreateMessage(Phone data, Thread thread, Customer customer, Business business, Channel channel)
        {
            try
            {
                var info = data.incoming ? "Cuộc gọi đến" : "Cuộc gọi đi";
                if (data.state.ToLower() == "busy")
                    info += " - busy";
                if (data.state.ToLower() == "up")
                    info += " - ok";

                var messager_id = Core.Helpers.CommonHelper.FormatKeyMessageHotline(data.business_id, data.customer_phone);
                var messager = _messageService.GetById(data.business_id, messager_id);

                if (messager != null)
                {
                    messager.updated_time = Core.Helpers.CommonHelper.UnixTimestampToDateTime(data.timestamp);
                    messager.timestamp = data.timestamp;
                    return messager;
                }
                else
                {
                    var time = Core.Helpers.CommonHelper.UnixTimestampToDateTime(data.timestamp);
                    var urls = new List<string>();
                    urls.Add(data.url_audio);

                    var mess = new Message();
                    mess._id = messager_id;
                    mess.id = messager_id;
                    mess.created_time = time;
                    mess.updated_time = time;
                    mess.parent_id = "";
                    mess.root_ext_id = "";
                    mess.parent_ext_id = "";
                    mess.ext_id = data.ext_id;
                    mess.url = data.url_audio;
                    mess.file_name = "";
                    mess.size = 0;
                    mess.subject = "";
                    mess.message = info;
                    mess.agent_id = data.agent_id;
                    mess.thread_id = thread.id;
                    mess.conversation_ext_id = "";
                    mess.sender_id = data.incoming ? customer.id : business.id;
                    mess.sender_ext_id = data.incoming ? customer.ext_id : business.ext_id;
                    mess.sender_name = data.incoming ? customer.name : business.name;
                    mess.sender_avatar = data.incoming ? customer.avatar : business.logo;
                    mess.recipient_id = data.incoming ? business.id : customer.id;
                    mess.recipient_ext_id = data.incoming ? business.ext_id : customer.ext_id;
                    mess.recipient_avatar = data.incoming ? business.logo : customer.avatar;
                    mess.recipient_name = data.incoming ? business.name : customer.name;
                    mess.author = customer.ext_id;
                    mess.read = false;
                    mess.deleted = false;
                    mess.liked = false;
                    mess.hidden = true;
                    mess.starred = false;
                    mess.customer_id = customer.id;
                    mess.owner_id = customer.id;
                    mess.type = "audio";
                    mess.timestamp = data.timestamp;
                    mess.business_id = data.business_id;
                    mess.channel_id = channel.id;
                    mess.channel_ext_id = channel.ext_id;
                    mess.thread_type = thread.type;
                    mess.channel_type = "hotline";
                    mess.replied = false;
                    mess.replied_at = 0;
                    mess.urls = "";
                    mess.tag = "";
                    mess.template = "";
                    mess.titles = "";
                    mess.extention = data.extention;
                    mess.state = data.state;
                    mess.trunk = data.trunk;
                    mess.state = data.state;
                    mess.incoming = data.incoming;
                    return mess;
                }
            }
            catch (Exception ex) { return null; }
        }

        public async Task<Thread> CreateThread(Phone data, Customer customer, Business business, Channel channel)
        {
            try
            {
                var thread_id = Core.Helpers.CommonHelper.FormatKeyThreadHotline(data.business_id, data.customer_phone);
                var thread = _threadService.GetById(data.business_id, thread_id);

                if (thread != null)
                {
                    thread.updated_time = Core.Helpers.CommonHelper.UnixTimestampToDateTime(data.timestamp);
                    thread.timestamp = data.timestamp;
                    return thread;
                }
                else
                {
                    var created_time = Core.Helpers.CommonHelper.UnixTimestampToDateTime(data.timestamp);
                    var th = new Thread();
                    th._id = thread_id;
                    th.id = thread_id;
                    th.created_time = created_time;
                    th.updated_time = created_time;
                    th.ext_id = thread_id;
                    th.channel_ext_id = channel.ext_id;
                    th.archived = false;
                    th.status = "active";
                    th.unread = false;
                    th.nonreply = false;
                    th.read_at = data.timestamp;
                    th.read_by = data.agent_id;
                    th.type = "hotline";
                    th.business_id = data.business_id;
                    th.channel_id = channel.id;
                    th.channel_type = "hotline";
                    th.agent_id = data.agent_id;
                    th.customer_id = customer.id;
                    th.link_ext_id = "";
                    th.owner_id = customer.id;
                    th.owner_ext_id = customer.ext_id;
                    th.owner_app_id = customer.app_id;
                    th.owner_name = customer.name;
                    th.owner_avatar = customer.avatar;
                    th.owner_timestamp = customer.timestamp;
                    th.owner_last_message_ext_id = data.ext_id;
                    th.owner_last_message_parent_ext_id = "";
                    th.last_message = "";
                    th.last_message_ext_id = "";
                    th.last_message_parent_ext_id = "";
                    th.sender_id = customer.id;
                    th.sender_ext_id = customer.ext_id;
                    th.sender_name = customer.name;
                    th.sender_avatar = customer.avatar;
                    th.timestamp = data.timestamp;
                    th.last_visits = "";
                    return th;
                }
            }
            catch (Exception ex) { return null; }
        }

        public async Task<Customer> CreateCustomer(Phone data, Business business, Channel channel)
        {
            try
            {
                var customer = await _customerService.GetCustomerFromPhone(data.business_id, data.customer_phone);

                if (customer != null && customer.Count > 0)
                {
                    customer[0].updated_time = Core.Helpers.CommonHelper.UnixTimestampToDateTime(data.timestamp);
                    customer[0].timestamp = data.timestamp;
                    return customer[0];
                }
                else
                {
                    var customer_id = Core.Helpers.CommonHelper.FormatKeyCustomerHotline(data.business_id, data.customer_phone);
                    var phones = new List<string>();
                    phones.Add(data.customer_phone);
                    var created_time = Core.Helpers.CommonHelper.UnixTimestampToDateTime(data.timestamp);

                    var cus = new Customer();
                    cus._id = customer_id;
                    cus.id = customer_id;
                    cus.created_time = created_time;
                    cus.updated_time = created_time;
                    cus.ext_id = customer_id;
                    cus.global_id = customer_id;
                    cus.first_name = data.customer_phone;
                    cus.last_name = "";
                    cus.name = data.customer_phone;
                    cus.email = "";
                    cus.avatar = "";
                    cus.phone = data.customer_phone;
                    cus.archived = false;
                    cus.business_id = data.business_id;
                    cus.key = 0;
                    cus.app_id = "";
                    cus.status = "active";
                    cus.timestamp = data.timestamp;
                    cus.business_name = business.name;
                    cus.agent_id = data.agent_id;
                    cus.assigned_by = data.agent_id;
                    cus.assigned_at = created_time;
                    cus.channel_id = channel.id;
                    cus.unread = false;
                    cus.nonreply = false;
                    cus.open = false;
                    cus.active_thread = "";
                    cus.active_ticket = "";
                    cus.phone_list = phones;
                    cus.address = "";
                    cus.city = "";
                    cus.zipcode = "";
                    cus.blocked = false;
                    cus.birthdate = DateTime.MinValue;
                    cus.sex = "";
                    cus.age = 0;
                    cus.weight = 0;
                    cus.height = 0;
                    return cus;
                }
            }
            catch (Exception ex) { return null; }
        }


        public async Task<Customer> CreateCustomer(string business_id, string customer_phone, string customer_fullname, Channel channel)
        {
            try
            {
                var customer = await _customerService.GetCustomerFromPhone(business_id, customer_phone);
                if (customer != null)
                {
                    return customer[0];
                }
                else
                {
                    var customer_id = Core.Helpers.CommonHelper.FormatKeyCustomerHotline(business_id, customer_phone);
                    var phones = new List<string>();
                    phones.Add(customer_phone);

                    var cus = new Customer();
                    cus._id = customer_id;
                    cus.id = customer_id;
                    cus.created_time = DateTime.Now;
                    cus.updated_time = DateTime.Now;
                    cus.ext_id = customer_id;
                    cus.global_id = customer_id;
                    cus.first_name = customer_phone;
                    cus.last_name = "";
                    cus.name = customer_phone;
                    cus.email = "";
                    cus.avatar = "";
                    cus.phone = customer_phone;
                    cus.archived = false;
                    cus.business_id = business_id;
                    cus.key = 0;
                    cus.app_id = "";
                    cus.status = "active";
                    cus.timestamp = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow);
                    cus.business_name = "";
                    cus.agent_id = "";
                    cus.assigned_by = "";
                    cus.assigned_at = DateTime.Now;
                    cus.channel_id = channel.id;
                    cus.unread = false;
                    cus.nonreply = false;
                    cus.open = false;
                    cus.active_thread = "";
                    cus.active_ticket = "";
                    cus.phone_list = phones;
                    cus.address = "";
                    cus.city = "";
                    cus.zipcode = "";
                    cus.blocked = false;
                    cus.birthdate = DateTime.MinValue;
                    cus.sex = "";
                    cus.age = 0;
                    cus.weight = 0;
                    cus.height = 0;
                    return cus;
                }
            }
            catch (Exception ex) { return null; }
        }

        public async Task<Channel> CreateChannel(Phone data)
        {
            try
            {
                var channels = await _channelService.GetByIdFromTrunk(data.business_id, data.trunk);
                if (channels != null && channels.Count > 0)
                {
                    return channels[0];
                }
                else
                {
                    var channel_id = Core.Helpers.CommonHelper.FormatKeyChannelHotline(data.business_id, "");
                    var chan = new Channel();
                    chan._id = channel_id;
                    chan.id = channel_id;
                    chan.created_time = Core.Helpers.CommonHelper.UnixTimestampToDateTime(data.timestamp);
                    chan.updated_time = Core.Helpers.CommonHelper.UnixTimestampToDateTime(data.timestamp);
                    chan.name = "Hotline unknow";
                    chan.type = "hotline";
                    chan.token = "";
                    chan.ext_id = channel_id;
                    chan.active = true;
                    chan.business_id = data.business_id;
                    chan.key = 0;
                    chan.phones = new List<string>();
                    _channelService.Create(chan);
                    return chan;
                }
            }
            catch (Exception ex) { return null; }
        }
    }
}