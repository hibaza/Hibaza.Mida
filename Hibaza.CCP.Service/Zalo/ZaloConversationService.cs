using Firebase.Storage;
using Hangfire;
using Hibaza.CCP.Core;
using Hibaza.CCP.Data;
using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using Hibaza.CCP.Domain.Models.Zalo;
using Hibaza.CCP.Service.SQL;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service.Facebook
{
    public interface IZaloConversationService
    {
        System.Threading.Tasks.Task SaveWebhookMessaging(ZaloMessage data, string agent_id, bool real_time_update, Channel channel);
        Task<string> DownloadToLocalAsync(string url);

    }

    public class ZaloConversationService : IZaloConversationService
    {
        private readonly IMessageService _messageService;
        private readonly ICustomerService _customerService;
        private readonly ILoggingService _logService;
        private readonly IChannelService _channelService;
        private readonly IThreadService _threadService;
        private readonly IAgentService _agentService;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly string _zaloGraphApiUrl = "https://openapi.zaloapp.com/oa/v1/";


        public ZaloConversationService(IChannelService channelService, IThreadService threadService, IAgentService agentService, ICustomerService customerService, IMessageService messageService, IOptions<AppSettings> appSettings, ILoggingService logService)
        {
            _messageService = messageService;
            _customerService = customerService;
            _channelService = channelService;
            _threadService = threadService;
            _agentService = agentService;
            _appSettings = appSettings;
            _logService = logService;
        }
        public async System.Threading.Tasks.Task SaveWebhookMessaging(ZaloMessage data, string agent_id, bool real_time_update, Channel channel)
        {
            var is_new = false;
            if (channel == null)
            {
                var channels = await _channelService.GetChannelsByExtId(data.oaid.ToString());
                if (channels.Count > 0)
                    channel = channels[0];
            }
            if (channel == null)
                return;

            var customer_ext_id = "";
            if (Convert.ToInt64(data.fromuid) > 0 && data.fromuid.ToString() != channel.ext_id)
                customer_ext_id = data.fromuid.ToString();
            else
                customer_ext_id = data.touid.ToString();

            var customer_id = CustomerService.FormatId01(channel.business_id, customer_ext_id);

            Customer customer = _customerService.GetById(channel.business_id, customer_id);
            if (customer == null)
                is_new = true;

            if (string.IsNullOrWhiteSpace(data.message) && string.IsNullOrWhiteSpace(data.href))
                return;

            customer = await GetCustomer(channel, data, customer, customer_ext_id);

            
            //if (is_new && string.IsNullOrWhiteSpace(data.message) && string.IsNullOrWhiteSpace(data.href))
            //    data.message = customer.name + " cần hỗ trợ";

            var message = await CreateMessage(channel, data, customer, true);
            var thread = await CreateThread(channel, message, customer, true);
            //customer.unread = false;
            //customer.nonreply = false;
            customer.active_thread = JsonConvert.SerializeObject(thread);
            customer.agent_id = agent_id;

            _customerService.CreateCustomer(customer, true);
            _messageService.CreateMessage(customer.business_id, message, true);
            _threadService.CreateThread(thread, true);
        }


        public async Task<Customer> GetCustomer(Channel channel, ZaloMessage msg, Customer customer, string customer_ext_id)
        {
            string business_id = channel.business_id;
            string channel_id = channel.id;
            var timestamp = msg.timestamp;
            var time = Core.Helpers.CommonHelper.UnixTimestampToDateTime(timestamp);
            dynamic profile = null;
            if (customer == null)
            {
                var customer_id = CustomerService.FormatId01(business_id, customer_ext_id);

                profile = await GetProfile(customer_ext_id, Convert.ToInt64(channel.ext_id), channel.token);
                if (profile == null)
                    return null;
                DateTime birthDate = DateTime.MinValue;
                try
                {
                    var birth = (long)profile.data.birthDate;
                    birthDate = Core.Helpers.CommonHelper.UnixTimestampToDateTime(birth);
                }
                catch { }
                customer = new Domain.Entities.Customer
                {
                    id = customer_id,
                    global_id = customer_id,
                    ext_id = customer_ext_id,
                    app_id = msg.appid.ToString(),
                    business_id = business_id,
                    channel_id = channel_id,
                    status = "pending",
                    created_time = time,
                    updated_time = time,
                    unread = true,
                    nonreply = true,
                    birthdate = birthDate,
                    timestamp = timestamp
                };
            }

            if ((string.IsNullOrWhiteSpace(customer.name) || string.IsNullOrWhiteSpace(customer.avatar) || string.IsNullOrWhiteSpace(customer.sex)))
            {
                if (profile != null)
                {
                    customer.updated_time = time;
                    customer.timestamp = timestamp;
                    if (string.IsNullOrWhiteSpace(customer.avatar))
                        customer.avatar = string.IsNullOrWhiteSpace(customer.avatar) ? profile.data.avatar : customer.avatar;

                    var url = customer.avatar;
                    if (!string.IsNullOrWhiteSpace(url) && !url.Contains("hibaza") && !url.Contains("firebase"))
                    {
                        try
                        {
                            customer.avatar = await DownloadToLocalAsync(url);
                        }
                        catch { }
                    }
                    if (string.IsNullOrWhiteSpace(customer.name))
                        customer.name = profile.data.displayName;
                    if (string.IsNullOrWhiteSpace(customer.sex))
                        customer.sex = (string)profile.data.userGender == "1" ? "male" : "female";
                }
            }
            return customer;

        }

        public async Task<Message> CreateMessage(Channel bc, ZaloMessage fm, Customer customer, bool real_time_update)
        {
            var business_id = bc.business_id;
            var message_id = MessageService.FormatId01(business_id, fm.msgid);
            var message = _messageService.GetById(business_id, message_id);
            if (message == null)
            {
                message = await BuildMessage(bc, fm, customer);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(fm.message) && fm.message != message.message)
                    message.message = fm.message;

                if (!string.IsNullOrWhiteSpace(fm.href) && fm.href != message.url)
                {
                    var imageUri = fm.href;
                    if (!fm.href.Contains("hibaza") && !fm.href.Contains("firebase"))
                    {
                        try
                        {
                            imageUri = await DownloadToLocalAsync(fm.href);
                        }
                        catch { }
                    }
                    message.url = imageUri;
                    message.urls = "[]";
                }


            }

            return message;
        }

        public async Task<Thread> CreateThread(Channel bc, Message message, Customer customer, bool real_time_update)
        {
            var business_id = bc.business_id;
            var channel_id = bc.id;

            var thread_id = message.thread_id;
            var thread = _threadService.GetById(business_id, thread_id);

            if (thread == null)
            {
                thread = new Thread();
                thread.id = thread_id;
                thread.customer_id = customer.id;
                thread.business_id = business_id;
                thread.channel_ext_id = bc.ext_id;
                thread.channel_id = bc.id;
                thread.channel_type = bc.type;
                thread.type = !string.IsNullOrWhiteSpace(message.thread_type) ? message.thread_type : thread.type;

                //if (thread.timestamp <= message.timestamp && !message.deleted && (!string.IsNullOrWhiteSpace(message.agent_id) || message.sender_ext_id!=message.channel_ext_id))
                thread.updated_time = message.created_time;
                thread.last_message_ext_id = message.ext_id;
                thread.last_message_parent_ext_id = message.parent_ext_id;
                thread.owner_last_message_ext_id = message.sender_ext_id == message.channel_ext_id ? thread.owner_last_message_ext_id : message.ext_id;
                thread.owner_last_message_parent_ext_id = message.sender_ext_id == message.channel_ext_id ? thread.owner_last_message_parent_ext_id : message.parent_ext_id;
                //thread.last_message = (string.IsNullOrWhiteSpace(thread.last_message) || message.sender_ext_id != message.channel_ext_id || !string.IsNullOrWhiteSpace(message.agent_id)) ? message.message : thread.last_message;
                thread.last_message = message.message;
                thread.owner_timestamp = message.sender_id == thread.owner_id ? message.timestamp : thread.owner_timestamp;
                thread.sender_id = message.sender_id;
                thread.sender_ext_id = message.sender_ext_id;
                thread.sender_name = thread.owner_name;
                thread.sender_avatar = thread.sender_avatar;
                thread.timestamp = message.timestamp;
                thread.link_ext_id = message.root_ext_id;

                //thread.unread = thread.channel_ext_id != message.sender_ext_id || (string.IsNullOrWhiteSpace(message.agent_id) && thread.type == "message");
                thread.unread = thread.channel_ext_id != message.sender_ext_id;
                // thread.nonreply = thread.channel_ext_id != message.sender_ext_id || (string.IsNullOrWhiteSpace(message.agent_id) && thread.type == "message");
                thread.nonreply = thread.channel_ext_id != message.sender_ext_id;

                var oavatar = (message.sender_ext_id == message.channel_ext_id ? message.recipient_avatar : message.sender_avatar);
                thread.owner_avatar = string.IsNullOrWhiteSpace(oavatar) ? thread.owner_avatar : oavatar;
                var oname = message.sender_ext_id == message.channel_ext_id ? message.recipient_name : message.sender_name;
                thread.owner_name = string.IsNullOrWhiteSpace(oname) ? thread.owner_name : oname;

                thread.ext_id = string.IsNullOrWhiteSpace(thread.ext_id) ? message.conversation_ext_id : thread.ext_id;
                thread.status = string.IsNullOrWhiteSpace(thread.agent_id) ? "pending" : "active";
                thread.sender_name = thread.owner_name;
                thread.owner_ext_id = customer.ext_id;
                thread.owner_id = customer.id;
            }
            else
            {
                if (thread.customer_id == null)
                    thread.customer_id = customer.id ;

                thread.unread = thread.channel_ext_id != message.sender_ext_id;
                thread.nonreply = thread.channel_ext_id != message.sender_ext_id;

                thread.agent_id = customer.agent_id;

                thread.updated_time = message.created_time;
                thread.last_message_ext_id = message.ext_id;
                thread.last_message_parent_ext_id = message.parent_ext_id;
                thread.owner_last_message_ext_id = message.sender_ext_id == message.channel_ext_id ? thread.owner_last_message_ext_id : message.ext_id;
                thread.owner_last_message_parent_ext_id = message.sender_ext_id == message.channel_ext_id ? thread.owner_last_message_parent_ext_id : message.parent_ext_id;
                thread.last_message = message.message;
                thread.owner_timestamp = message.sender_id == thread.owner_id ? message.timestamp : thread.owner_timestamp;
                thread.sender_id = message.sender_id;
                thread.sender_ext_id = message.sender_ext_id;
                thread.sender_avatar = thread.sender_avatar;
                thread.timestamp = message.timestamp;
                thread.link_ext_id = message.root_ext_id;

                var oavatar = (message.sender_ext_id == message.channel_ext_id ? message.recipient_avatar : message.sender_avatar);
                thread.owner_avatar = string.IsNullOrWhiteSpace(oavatar) ? thread.owner_avatar : oavatar;
                var oname = message.sender_ext_id == message.channel_ext_id ? message.recipient_name : message.sender_name;
                // thread.owner_name = string.IsNullOrWhiteSpace(oname) ? thread.owner_name : oname;

                thread.ext_id = string.IsNullOrWhiteSpace(thread.ext_id) ? message.conversation_ext_id : thread.ext_id;
                thread.status = string.IsNullOrWhiteSpace(thread.agent_id) ? "pending" : "active";
            }

            return thread;

        }


        public async Task<dynamic> GetProfile(string uid, long oaId, string oaSecret)
        {
            var timestamp = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow);
            var mac = Core.Helpers.CommonHelper.sha256(oaId + uid + timestamp + oaSecret);
            //var uri = "https://openapi.zaloapp.com/oa/v1/getprofile?oaid=" + _appSettings.Zalos.OA_Id + "&uid=84979354170&timestamp=" + timestame + "&mac=" + mac;
            var url = _zaloGraphApiUrl + "getprofile?oaid=" + oaId + "&uid=" + uid + "&timestamp=" + timestamp + "&mac=" + mac;
            return await Core.Helpers.WebHelper.HttpGetAsync01<dynamic>(url);

        }

        public async Task<string> DownloadToLocalAsync(string url)
        {
            try
            {
                var rs = await ImagesService.UpsertImageStore(url, _appSettings.Value); 
                return rs;
            }
            catch { }
            return url;
        }

        public async Task<Message> BuildMessage(Channel channel, ZaloMessage msg, Customer customer)
        {
            var message_id = MessageService.FormatId01(channel.business_id, msg.msgid);
            var sender_ext_id = "";
            var recipient_ext_id = "";
            if (Convert.ToInt64(msg.fromuid) > 0 && msg.fromuid.ToString() != channel.ext_id)
            {
                sender_ext_id = msg.fromuid.ToString();
                recipient_ext_id = msg.oaid.ToString();
            }
            else
            {
                sender_ext_id = msg.oaid.ToString();
                recipient_ext_id = msg.touid.ToString();
            }
            if (sender_ext_id.Length < 5)
                return null;

            var thread_id = ThreadService.FormatId01(channel.business_id, customer.ext_id);

            var timestamp = msg.timestamp > 9999999999 ? msg.timestamp / 1000 : msg.timestamp;
            var created_time = Core.Helpers.CommonHelper.UnixTimestampToDateTime(timestamp);

            var imageUrl = msg.href;
            if (!string.IsNullOrWhiteSpace(imageUrl) && !imageUrl.Contains("hibaza") && !imageUrl.Contains("firebase"))
                imageUrl = await DownloadToLocalAsync(imageUrl);

            var message = new Domain.Entities.Message
            {
                //"sendmsg"  "sendimagemsg")
                id = message_id,
                parent_id = "",
                parent_ext_id = "",
                root_ext_id = "",
                conversation_ext_id = sender_ext_id,
                ext_id = msg.msgid,
                thread_id = thread_id,
                thread_type = "message",
                sender_id = CustomerService.FormatId01(channel.business_id, sender_ext_id),
                sender_ext_id = sender_ext_id,
                sender_name = sender_ext_id == customer.ext_id ? customer.name : channel.name,
                sender_avatar = sender_ext_id == customer.ext_id ? customer.avatar : "",
                recipient_id = CustomerService.FormatId01(channel.business_id, recipient_ext_id),
                recipient_ext_id = recipient_ext_id,
                recipient_name = recipient_ext_id == channel.ext_id ? channel.name : customer.name,
                recipient_avatar = recipient_ext_id == channel.ext_id ? "" : customer.avatar,
                author = CustomerService.FormatId01(channel.business_id, sender_ext_id),
                customer_id = customer.id,
                message = msg.message,
                tag = "",
                template = "",
                url = imageUrl,
                timestamp = timestamp,
                updated_time = created_time,
                created_time = created_time,
                business_id = channel.business_id,
                channel_id = channel.id,
                channel_ext_id = channel.ext_id,
                channel_type = channel.type,
                owner_id = sender_ext_id,
                agent_id = customer.agent_id,
                type = msg.@event == "sendimagemsg" ? "image" : "text",
                liked = false,
                hidden = false,
                deleted = false,
                urls = "[]"
            };

            return message;
        }
    }
}