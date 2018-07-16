using Firebase.Storage;
using Hangfire;
using Hibaza.CCP.Core;
using Hibaza.CCP.Data;
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


namespace Hibaza.CCP.Service.Facebook
{
    public interface IFacebookConversationService
    {
        System.Threading.Tasks.Task<int> DownloadSingleConversation(string business_id, string channel_id, string pageId, string conversationId, int limit, bool real_time_update);
        System.Threading.Tasks.Task<int> DownloadConversations(Channel bc, string url, string job_id, DateTime from, DateTime to, bool real_time_update);
        System.Threading.Tasks.Task<int> AutoDownloadConversations(string business_id, int offset, bool real_time_update = true);
        Task<FacebookMessagePostResponse> PostFacebookMessage(FacebookMessagePostData data, string accessToken);
        Task<FacebookMessagePostResponse> PostFacebookAttachment(FacebookMessagePostData data, string accessToken);
        Task<FacebookTextMessageResponse> PostFacebookTextMessageUsingConversation(string conversationExtId, string message, string accessToken);
        System.Threading.Tasks.Task SaveWebhookMessaging(string business_id, string channel_id, string pageId, FacebookMessagingEvent e, bool real_time_update);
        void SaveWebhookReferral(string business_id, string channel_id, string pageId, FacebookMessagingEvent referralEvent);
        System.Threading.Tasks.Task SaveWebhookDelivery(string pageId, FacebookMessagingEvent deliveryEvent);
        FacebookUserProfile GetProfile(string userId, string pageAccessToken);
        System.Threading.Tasks.Task<bool> DeleteMessage(string message_id, string pageAccessToken);
        System.Threading.Tasks.Task CreateThread(Channel bc, Message message, Thread thread, Customer customer, string owner_id, string owner_ext_id, string owner_app_id, bool real_time_update);
        Message CreateMessage(Channel bc, FacebookMessageObject fm, string message_id, string agent_id, string owner_id, bool real_time_update, out bool is_new, out string thread_id);
        string GetUserAppScopedId(Channel channel, string msgId);
        Task<int> SaveMessages(Channel bc, Conversation conversation, string url, string job_id, int limit, bool real_time_update);
        System.Threading.Tasks.Task<int> DownloadMessages(Channel bc, int limit, long since, long until, bool real_time_update);
        Task<Attachment> UploadAttachments(Channel channel, Thread thread, string product_id, string image_url);
        System.Threading.Tasks.Task SendMessage(Channel channel, Thread thread, MessageFormData data, string msgId);
        Task<int> SendPromotionMessage(string business_id, string channel_id, string message, int limit, int day = 1);
        System.Threading.Tasks.Task FixMessagePhotoUrls(string business_id, string message_id, bool real_time_update);
        Customer GetCustomer(Channel channel, string customer_id, string msgId, string pageScopedId, string appScopedId);
        Task<string> GetPageScopedId(string asid, string pageId);
        System.Threading.Tasks.Task<int> FixConversationExtId(string business_id, string channel_id, int limit);
        //void Test();
        void AutoUpdateCustomerIdNull();
        Task<ReplaceOneResult> UpsertAnyMongo<T>(T obj, UpdateOptions option, FilterDefinition<T> filter, string collectionName) where T : class;
        Task<List<T>> GetDataMongo<T>(string query, FindOptions<T> options, string collectionName) where T : class;
        void autoUpdateThreadsCusomterIdNull();
    }

    public class FacebookConversationService : IFacebookConversationService
    {
        private readonly ILinkService _linkService;
        private readonly INodeService _nodeService;
        private readonly IConversationService _conversationService;
        private readonly IMessageService _messageService;
        private readonly ICustomerService _customerService;
        private readonly ILoggingService _logService;
        private readonly IChannelService _channelService;
        private readonly IThreadService _threadService;
        private readonly IAgentService _agentService;
        private readonly ICustomerCounterService _counterService;
        private readonly IReferralService _referralService;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly string _facebookGraphApiUrl = "https://graph.facebook.com/v2.10";


        public FacebookConversationService(IChannelService channelService, INodeService nodeService, IReferralService referralService, IThreadService threadService, IAgentService agentService, ICustomerService customerService, ILinkService linkService, IConversationService conversationService, IMessageService messageService, ICustomerCounterService counterService, IOptions<AppSettings> appSettings, ILoggingService logService)
        {
            _linkService = linkService;
            _nodeService = nodeService;
            _conversationService = conversationService;
            _messageService = messageService;
            _customerService = customerService;
            _channelService = channelService;
            _threadService = threadService;
            _agentService = agentService;
            _counterService = counterService;
            _referralService = referralService;
            _appSettings = appSettings;
            _logService = logService;
        }


        private Message BuildMessage(string business_id, string channel_id, string owner_id, string agent_id, string pageId, FacebookMessageObject msg)
        {
            var thread_id = "";
            var message_id = "";
            var is_new = false;

            thread_id = ThreadService.FormatId(business_id, msg.conversationId);
            var thread = _threadService.GetById(business_id, msg.conversationId);
            if (thread == null)
            {
                if (msg.conversationId.IndexOf(business_id) < 0)
                {
                    is_new = true;
                    thread_id = ThreadService.FormatId01(business_id, msg.conversationId);
                    message_id = MessageService.FormatId01(business_id, msg.msgId);
                }
            }
            else
                message_id = MessageService.FormatId(business_id, msg.msgId);

            var sender_ext_id = msg.senderId;
            var recipient_ext_id = msg.recipientId;
            var timestamp = msg.timestamp > 9999999999 ? msg.timestamp / 1000 : msg.timestamp;
            var created_time = Core.Helpers.CommonHelper.UnixTimestampToDateTime(timestamp);

            var message = new Domain.Entities.Message
            {
                id = message_id,
                parent_id = is_new && !string.IsNullOrWhiteSpace(msg.msgParentId) && msg.msgParentId.IndexOf(business_id) < 0 ? MessageService.FormatId01(business_id, msg.msgParentId) : MessageService.FormatId(business_id, msg.msgParentId),
                parent_ext_id = msg.msgParentId,
                root_ext_id = msg.msgRootId,
                conversation_ext_id = msg.conversationId,
                ext_id = msg.msgId,
                thread_id = thread_id,
                thread_type = msg.thread_type,
                sender_id = is_new && !string.IsNullOrWhiteSpace(sender_ext_id) && sender_ext_id.IndexOf(business_id) < 0 ? CustomerService.FormatId01(business_id, sender_ext_id) : CustomerService.FormatId(business_id, sender_ext_id),
                sender_ext_id = sender_ext_id,
                sender_name = msg.sender_name,
                sender_avatar = msg.sender_avatar,
                recipient_id = is_new && !string.IsNullOrWhiteSpace(recipient_ext_id) && recipient_ext_id.IndexOf(business_id) < 0 ? CustomerService.FormatId01(business_id, recipient_ext_id) : CustomerService.FormatId(business_id, recipient_ext_id),
                recipient_ext_id = recipient_ext_id,
                recipient_name = msg.recipient_name,
                recipient_avatar = msg.recipient_avatar,
                author = is_new && !string.IsNullOrWhiteSpace(sender_ext_id) && sender_ext_id.IndexOf(business_id) < 0 ? CustomerService.FormatId01(business_id, sender_ext_id) : CustomerService.FormatId(business_id, sender_ext_id),
                customer_id = msg.customer_id,
                message = msg.text,
                tag = msg.tag,
                template = msg.template,
                url = msg.url,
                timestamp = timestamp,
                updated_time = created_time,
                created_time = created_time,
                business_id = business_id,
                channel_id = channel_id,
                channel_ext_id = pageId,
                channel_type = msg.channel_type,
                owner_id = owner_id,
                agent_id = agent_id,
                type = msg.type,
                liked = msg.liked,
                hidden = msg.hidden,
                urls = JsonConvert.SerializeObject(msg.urls)
            };
            return message;
            // return message;
        }

        public Message CreateMessage(Channel bc, FacebookMessageObject fm, string message_id, string agent_id, string owner_id, bool real_time_update, out bool is_new, out string thread_id)
        {
            string pageId = bc.ext_id;
            var business_id = bc.business_id;
            var channel_id = bc.id;
            thread_id = "";
            is_new = false;
            bool is_updated = false;
            var message = _messageService.GetById(business_id, message_id);
            if (message == null)
            {
                is_new = true;
                message = BuildMessage(business_id, channel_id, owner_id, agent_id, pageId, fm);
                if (message.sender_id == owner_id)
                {
                    message.recipient_name = bc.name;
                }
                else
                {
                    message.sender_name = bc.name;
                }
                if (fm.titles != null && fm.titles.Count > 0)
                {
                    message.titles = JsonConvert.SerializeObject(fm.titles);
                }
            }
            else
            {
                thread_id = ThreadService.FormatId(business_id, fm.conversationId);

                if (string.IsNullOrWhiteSpace(message.thread_id) && !string.IsNullOrWhiteSpace(thread_id))
                {
                    message.thread_id = thread_id;
                    is_updated = true;
                }
                else
                {
                    thread_id = message.thread_id;
                }

                if (message.conversation_ext_id != fm.conversationId && message.conversation_ext_id == thread_id)
                {
                    message.conversation_ext_id = fm.conversationId;
                    is_updated = true;
                }

                if (message.sender_name != fm.sender_name && !string.IsNullOrWhiteSpace(fm.sender_name))
                {
                    is_updated = true;
                    message.sender_name = fm.sender_name;
                }

                if (string.IsNullOrWhiteSpace(message.customer_id) && !string.IsNullOrWhiteSpace(fm.customer_id))
                {
                    message.customer_id = fm.customer_id;
                    is_updated = true;
                }

                if (message.message != fm.text)
                {
                    is_updated = true;
                    message.message = fm.text;
                }

                if (string.IsNullOrWhiteSpace(message.url) && !string.IsNullOrWhiteSpace(fm.url))
                {
                    is_updated = true;
                    message.url = fm.url;
                }
                if (fm.titles != null && fm.titles.Count > 0)
                {
                    is_updated = true;
                    message.titles = JsonConvert.SerializeObject(fm.titles);
                }

                if (string.IsNullOrWhiteSpace(message.urls))
                {
                    string fmUrls;
                    try
                    {
                        fmUrls = JsonConvert.SerializeObject(fm.urls);
                    }
                    catch { fmUrls = null; }
                    if (!string.IsNullOrWhiteSpace(fmUrls))
                    {
                        is_updated = true;
                        message.urls = fmUrls;
                    }
                }



                //if (message.url != fm.url && !string.IsNullOrWhiteSpace(fm.url))
                //{
                //    if (string.IsNullOrWhiteSpace(message.url) || (!message.url.Contains("hibaza") && !message.url.Contains("firebase")))
                //    {
                //        is_updated = true;
                //        var un = fm.url;
                //        if (!fm.url.Contains("hibaza") && !fm.url.Contains("firebase"))
                //        {
                //            try
                //            {
                //                un = DownloadToLocalAsync(bc.business_id, "attachments", thread_id, new Uri(fm.url)).Result;
                //            }
                //            catch { un = fm.url; }
                //        }
                //        message.url = un;
                //        List<string> urls = new List<string>();
                //        try
                //        {
                //            foreach (var url in fm.urls)
                //            {
                //                var u = url;
                //                if (!url.Contains("hibaza") && !url.Contains("firebase"))
                //                {
                //                    u = DownloadToLocalAsync(bc.business_id, "attachments", thread_id, new Uri(url)).Result;
                //                }
                //                urls.Add(u);
                //            }
                //        }
                //        catch
                //        {
                //            urls = fm.urls;
                //        }
                //        message.urls = JsonConvert.SerializeObject(urls);
                //    }
                //}


                if (!string.IsNullOrWhiteSpace(agent_id) && message.agent_id != agent_id)
                {
                    is_updated = true;
                    message.agent_id = agent_id;
                }

                if (message.timestamp != fm.timestamp && real_time_update)
                {
                    is_updated = true;
                    message.timestamp = fm.timestamp;
                    message.created_time = Core.Helpers.CommonHelper.UnixTimestampToDateTime(fm.timestamp);
                }
            }

            if (is_new || is_updated)
            {
                try
                {
                    _messageService.CreateMessage(business_id, message, real_time_update);
                }
                catch (Exception ex)
                {
                }

                BackgroundJob.Enqueue<FacebookConversationService>(x => x.FixMessagePhotoUrls(business_id, message.id, false));
            }
            return message;
        }

        public async System.Threading.Tasks.Task CreateThread(Channel bc, Message message, Thread thread, Customer customer, string owner_id, string owner_ext_id, string owner_app_id, bool real_time_update)
        {
            string pageId = bc.ext_id;
            var business_id = bc.business_id;
            var channel_id = bc.id;
            if (thread == null)
            {
                thread = _threadService.GetById(business_id, message.thread_id);
                if (thread == null)
                {
                    var id = !string.IsNullOrWhiteSpace(message.thread_id) ? message.thread_id : ThreadService.FormatId(message.business_id, message.conversation_ext_id);
                    thread = new Thread
                    {
                        id = id.IndexOf(business_id) < 0 ? ThreadService.FormatId01(message.business_id, id) : id,
                        owner_id = owner_id,
                        owner_ext_id = owner_ext_id,
                        owner_app_id = owner_app_id
                    };
                }
            }
            thread.owner_app_id = !string.IsNullOrWhiteSpace(owner_app_id) ? owner_app_id : thread.owner_app_id;
            thread.owner_ext_id = !string.IsNullOrWhiteSpace(owner_ext_id) ? owner_ext_id : thread.owner_ext_id;
            thread.link_ext_id = thread.type == "comment" ? (message.root_ext_id ?? message.parent_ext_id) : null;

            if (customer == null)
            {
                if (!string.IsNullOrWhiteSpace(thread.customer_id))
                    customer = _customerService.GetById(thread.business_id, thread.customer_id);
                else
                    customer = GetCustomer(bc, thread.customer_id, message.ext_id, thread.owner_ext_id, thread.owner_app_id);
            }

            if (customer != null)
            {
                thread.owner_name = customer.name;
                thread.owner_avatar = customer.avatar;
                thread.customer_id = customer.id;
                thread.sender_name = customer.name;
            }
            thread.agent_id = (customer == null || !string.IsNullOrWhiteSpace(thread.agent_id)) ? thread.agent_id : customer.agent_id;
            thread.owner_app_id = (customer == null || !string.IsNullOrWhiteSpace(thread.owner_app_id)) ? thread.owner_app_id : customer.app_id;
            thread.owner_ext_id = (customer == null || !string.IsNullOrWhiteSpace(thread.owner_ext_id)) ? thread.owner_ext_id : customer.ext_id;

            //thread.customer_id = customer == null ? thread.customer_id : customer.id;

            thread = _threadService.CreateThread(business_id, thread, message, real_time_update);

            if (thread != null && customer != null)
            {
                //customer.unread = thread.unread;
                // customer.nonreply = thread.nonreply;
                // customer.active_thread = JsonConvert.SerializeObject(thread);
                _customerService.CreateCustomer(customer, thread, real_time_update);
            }
            checkPhoneWeightHeight(message, customer);
        }

        public async Task<int> checkPhoneWeightHeight(Message message, Customer customer)
        {
            try
            {
                var newPhones = new List<string>();

                if (!string.IsNullOrWhiteSpace(message.message) && Core.Helpers.CommonHelper.ExistsDigits(message.message)
                    && message.channel_ext_id != message.sender_ext_id)
                {
                    try
                    {
                        var dicConfig = new Dictionary<string, string>();
                        dicConfig.Add("mongoconnect", "ConnAi");
                        dicConfig.Add("mongodb", "AiDb");
                        dicConfig.Add("collectionname", "");
                        dicConfig.Add("type", "procedure");

                        message.message = Core.Helpers.CommonHelper.replaceSpecial(message.message);
                        var dicPara = "GetPhoneWeightHeight('" + customer.id + "','" + message.message + "')";
                        var json = new Dictionary<string, string>();
                        json.Add("config", JsonConvert.SerializeObject(dicConfig));
                        json.Add("para", JsonConvert.SerializeObject(dicPara));

                        var client = new HttpClient();
                        var check = client.PostAsync(_appSettings.Value.BaseUrls.ApiAi + "api/procedure/execute",
                              new StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8, "application/json")).Result;
                        var contents = check.Content.ReadAsStringAsync().Result;
                        if (contents != null && contents != "null" && contents != "\"[]\"")
                        {
                            var val = JsonConvert.DeserializeObject<dynamic>(JsonConvert.DeserializeObject<string>(contents));
                            var phone = (string)val.phone;
                            if (!string.IsNullOrWhiteSpace(phone))
                                newPhones.Add(phone);
                            if ((int)val.weight > 0)
                                customer.weight = (int)val.weight;
                            if ((int)val.height > 0)
                                customer.height = (int)val.height;
                            if (!string.IsNullOrWhiteSpace((string)val.address))
                                customer.address = (string)val.address;
                            if (!string.IsNullOrWhiteSpace((string)val.city) && string.IsNullOrWhiteSpace(customer.city))
                                customer.city = (string)val.city;
                        }

                        List<string> currentPhoneList = (customer == null || customer.phone_list == null ? new List<string>() : customer.phone_list);
                        currentPhoneList.AddRange(newPhones);
                        currentPhoneList = currentPhoneList.Distinct().ToList();
                        customer.phone_list = currentPhoneList;
                        customer.phone = newPhones.Count > 0 ? newPhones[0] : customer.phone;
                    }
                    catch
                    {
                        newPhones.AddRange(Core.Helpers.CommonHelper.FindPhoneNumbers(message.message));
                    }
                    _customerService.CreateCustomer(customer, false);
                }
            }
            catch { }
            return 1;
        }

        public async System.Threading.Tasks.Task SaveWebhookMessaging(string business_id, string channel_id, string pageId, FacebookMessagingEvent e, bool real_time_update)
        {
            List<Channel> list = new List<Channel>();

            if (!string.IsNullOrWhiteSpace(channel_id) && !string.IsNullOrWhiteSpace(business_id))
            {
                list.Add(_channelService.GetById(business_id, channel_id));
            }
            else
            {
                list = _channelService.GetChannelsByExtId(pageId).Result.Where(b => b.active).ToList();
            }

            Thread threadss = null;
            FacebookMessageGet msg = null;
            if (e.message == null)
            {
                threadss = _threadService.GetByIdFromCustomerId(list[0].business_id, e.sender.id);
                if (threadss == null)
                    return;

                msg = new FacebookMessageGet
                {
                    text = e.postback.payload,
                    mid = (threadss.last_message_ext_id == null ? "_postback" : (threadss.last_message_ext_id + "_postback"))
                };
            }
            else
            {
                msg = e.message;
            }
            //var msg = e.message;
            var msgId = msg.mid.StartsWith("m_") ? msg.mid : "m_" + msg.mid;
            string owner_ext_id = e.sender.id;
            string owner_app_id = "";
            if (pageId == owner_ext_id)
            {
                owner_ext_id = e.recipient.id;
            }
            var fm = new FacebookMessageObject
            {
                msgId = msgId,
                conversationId = owner_ext_id,
                senderId = e.sender.id,
                sender_name = e.sender.name,
                recipientId = e.recipient.id,
                recipient_name = e.recipient.name,
                timestamp = e.timestamp > 9999999999 ? e.timestamp / 1000 : e.timestamp,
                // text = e.message != null ? e.message.text : e.postback.payload,
                text = msg.text,
                type = "text",
                thread_type = "message",
                channel_type = "facebook",
                urls = new List<string>(),
                titles = new List<string>()
            };


            if (msg.attachments != null)
            {
                foreach (var att in msg.attachments)
                {
                    switch (att.type)
                    {
                        case "template":
                            if (att.payload != null && att.payload.elements != null && att.payload.template_type == "generic") //only support generic type at the moment
                            {
                                foreach (var element in att.payload.elements)
                                {
                                    string attUrl = element.image_url;
                                    fm.urls.Add(attUrl);
                                    fm.text = element.title == null ? "" :
                                        (element.title + " " + (string.IsNullOrWhiteSpace(element.subtitle) ? "" : element.subtitle));
                                    fm.titles.Add(element.title == null ? "" :
                                        (element.title + " " + (string.IsNullOrWhiteSpace(element.subtitle) ? "" : element.subtitle)));
                                    fm.type = "multiple_images";
                                }
                                fm.template = "generic";
                            }

                            break;
                        default:
                            if (att.payload != null)
                            {
                                string attUrl = att.payload.url;
                                fm.urls.Add(attUrl);
                                fm.titles.Add(msg.text == null ? "" : msg.text);
                                fm.type = att.type;
                            }
                            break;
                    }
                }

                fm.url = fm.urls.FirstOrDefault();
                fm.type = fm.urls.Count > 1 ? "multiple_images" : fm.type;
                fm.attachements = msg.attachments;
            }

            //if (string.IsNullOrWhiteSpace(fm.text) && string.IsNullOrWhiteSpace(fm.url)) return;

            Customer customer = null;
            Thread thread = null;
            foreach (var bc in list)
            {
                business_id = bc.business_id;
                channel_id = bc.id;
                var thread_id = ThreadService.FormatId(business_id, fm.conversationId);
                var message_id = MessageService.FormatId(business_id, fm.msgId);

                var owner_id = CustomerService.FormatId(business_id, owner_ext_id);
                bool is_new_message = false;
                string tid = "";

                thread = _threadService.GetById(business_id, thread_id);
                if (thread == null)
                {
                    if (fm.conversationId.IndexOf(business_id) < 0)
                    {
                        thread_id = ThreadService.FormatId01(business_id, fm.conversationId);
                        if (owner_ext_id.IndexOf(business_id) < 0)
                            owner_id = CustomerService.FormatId01(business_id, owner_ext_id);
                        thread = _threadService.GetById(business_id, thread_id);
                    }
                }

                if (thread != null)
                {
                    owner_app_id = string.IsNullOrWhiteSpace(owner_app_id) ? thread.owner_app_id : owner_app_id;
                    owner_ext_id = string.IsNullOrWhiteSpace(owner_ext_id) ? thread.owner_ext_id : owner_ext_id;
                    fm.customer_id = string.IsNullOrWhiteSpace(fm.customer_id) ? thread.customer_id : fm.customer_id;
                }

                if ((string.IsNullOrWhiteSpace(fm.sender_name) && fm.senderId != pageId) || string.IsNullOrWhiteSpace(fm.customer_id))
                {
                    customer = GetCustomer(bc, fm.customer_id, fm.msgId, owner_ext_id, owner_app_id);
                    if (customer != null) fm.sender_name = customer.name;
                }
                if (customer != null) fm.customer_id = customer.id;

                var message = CreateMessage(bc, fm, message_id, "", owner_id, real_time_update, out is_new_message, out tid);

                //if (message != null && (real_time_update || thread is null))
                if (message != null)
                {
                    try
                    {
                        await CreateThread(bc, message, null, customer, owner_id, owner_ext_id, owner_app_id, real_time_update);
                        if (real_time_update) BackgroundJob.Enqueue<CustomerService>(x => x.BatchUpdateUnreadCounters(business_id));
                    }
                    catch (Exception ex)
                    {
                        _logService.Create(new Log
                        {
                            message = ex.Message,
                            category = "Facebook",
                            details = JsonConvert.SerializeObject(ex.StackTrace),
                            name = string.Format("Create thread for message_id {0}", message.id)
                        });
                        throw ex;
                    }
                }
            }
        }


        public string GetAppAccessToken(string clientId, string clientSecret)
        {
            var url = string.Format(_facebookGraphApiUrl + "/oauth/access_token?client_id={0}&client_secret={1}&grant_type=client_credentials", clientId, clientSecret);
            string token = "";
            try
            {
                token = Core.Helpers.WebHelper.HttpGetAsync<dynamic>(url).Result.access_token;
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Facebook",
                    name = "Get app access token",
                    link = url
                });
            }
            return token;
        }

        public async Task<string> GetPageScopedId(string asid, string pageId)
        {
            var url = _facebookGraphApiUrl + "/" + asid + "/ids_for_pages?page=" + pageId + "&access_token=" + _appSettings.Value.AppAccessToken;
            try
            {
                FacebookIdForPageFeed rs = null;
                try
                {
                    rs = await Core.Helpers.WebHelper.HttpGetAsync<FacebookIdForPageFeed>(url);
                }
                catch
                {
                    url = _facebookGraphApiUrl + "/" + asid + "/ids_for_pages?page=" + pageId + "&access_token=" + GetAppAccessToken(_appSettings.Value.ClientId, _appSettings.Value.ClientSecret);
                    rs = await Core.Helpers.WebHelper.HttpGetAsync<FacebookIdForPageFeed>(url);
                }

                if (rs != null && rs.data != null && rs.data.Count() == 1 && !string.IsNullOrWhiteSpace(rs.data.First().id))
                {
                    return rs.data.First().id;
                }
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Facebook",
                    details = JsonConvert.SerializeObject(ex.StackTrace),
                    name = string.Format("Get facebook psid of page {0} for asid {1}", asid, pageId),
                    link = url
                });
            }
            return null;
        }


        public async Task<string> GetAppScopedId(string psid, string access_token)
        {
            var url = _facebookGraphApiUrl + "/" + psid + "/ids_for_apps?app=" + _appSettings.Value.ClientId + "&access_token=" + access_token + "&appsecret_proof=" + Core.Helpers.CommonHelper.Hmacsha256(access_token, _appSettings.Value.ClientSecret);
            try
            {
                FacebookIdForAppFeed rs = null;
                rs = await Core.Helpers.WebHelper.HttpGetAsync<FacebookIdForAppFeed>(url);
                //try
                //{
                //    rs = await Core.Helpers.WebHelper.HttpGetAsync<FacebookIdForAppFeed>(url);
                //}
                //catch
                //{
                //    var act = GetAppAccessToken(_appSettings.Value.ClientId, _appSettings.Value.ClientSecret);
                //    url = _facebookGraphApiUrl + "/" + psid + "/ids_for_apps?app=" + _appSettings.Value.ClientId + "&access_token=" + access_token + "&appsecret_proof=" + Core.Helpers.CommonHelper.Hmacsha256(act, _appSettings.Value.ClientSecret);
                //    rs = await Core.Helpers.WebHelper.HttpGetAsync<FacebookIdForAppFeed>(url);
                //}

                if (rs != null && rs.data != null && rs.data.Count() == 1 && !string.IsNullOrWhiteSpace(rs.data.First().id))
                {
                    return rs.data.First().id;
                }
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Facebook",
                    details = JsonConvert.SerializeObject(ex.StackTrace),
                    name = string.Format("Get facebook asid of app {0} for psid {1}", _appSettings.Value.ClientId, psid),
                    link = url
                });
            }
            return null;
        }

        private FacebookUserProfile GetPageScopedProfile(string pageScopedUserId, string pageAccessToken)
        {
            try
            {
                var url = _facebookGraphApiUrl + "/" + pageScopedUserId + "?fields=first_name,last_name,profile_pic&access_token=" + pageAccessToken;
                //var url = _facebookGraphApiUrl + "/" + pageScopedUserId + "?fields=last_name,first_name,gender,profile_pic,locale,timezone,id&access_token=" + pageAccessToken;
                var profile = Core.Helpers.WebHelper.HttpGetAsync<FacebookProfile>(url).Result;
                return new FacebookUserProfile
                {
                    avatar = profile.profile_pic,
                    first_name = profile.first_name,
                    last_name = profile.last_name,
                    name = profile.first_name + " " + profile.last_name
                };
            }
            catch (Exception ex)
            {
                var a = ex;
            }
            return null;
        }


        public async System.Threading.Tasks.Task<bool> DeleteMessage(string message_id, string pageAccessToken)
        {
            try
            {
                var url = _facebookGraphApiUrl + "/" + message_id + "?access_token=" + pageAccessToken;
                return await Core.Helpers.WebHelper.HttpDeleteAsync(url);
            }
            catch { }
            return false;
        }


        public FacebookUserProfile GetProfile(string userId, string pageAccessToken)
        {
            try
            {
                //var url = _facebookGraphApiUrl + "/" + userId + "?fields=id,name,email,first_name,last_name,picture{url}&access_token=" + pageAccessToken;
                var url = _facebookGraphApiUrl + "/" + userId + "?fields=id,first_name,last_name,gender,profile_pic{url}&access_token=" + pageAccessToken;
                var profile = Core.Helpers.WebHelper.HttpGetAsync<FacebookProfile>(url).Result;
                return new FacebookUserProfile
                {
                    avatar = profile.profile_pic,
                    first_name = profile.first_name,
                    last_name = profile.last_name,
                    name = string.IsNullOrWhiteSpace(profile.name) ? profile.first_name + " " + profile.last_name : profile.name,
                    sex = profile.gender,
                    id = profile.id
                };

            }
            catch (Exception ex)
            {
                try
                {
                    //var url = _facebookGraphApiUrl + "/" + userId + "?fields=id,name,email,first_name,last_name,picture{url}&access_token=" + pageAccessToken;
                    var url = _facebookGraphApiUrl + "/" + userId + "?fields=id,first_name,last_name,gender,picture{url}&access_token=" + pageAccessToken;
                    var profile = Core.Helpers.WebHelper.HttpGetAsync<FacebookProfile>(url).Result;
                    return new FacebookUserProfile
                    {
                        avatar = profile.picture.data.url,
                        first_name = profile.first_name,
                        last_name = profile.last_name,
                        name = string.IsNullOrWhiteSpace(profile.name) ? profile.first_name + " " + profile.last_name : profile.name,
                        sex = profile.gender,
                        id = profile.id
                    };

                }
                catch
                {
                    try
                    {
                        var url = _facebookGraphApiUrl + "/" + userId + "?fields=id,first_name,last_name&access_token=" + pageAccessToken;
                        var profile = Core.Helpers.WebHelper.HttpGetAsync<FacebookProfile>(url).Result;
                        return new FacebookUserProfile
                        {
                            avatar = profile.profile_pic,
                            first_name = profile.first_name,
                            last_name = profile.last_name,
                            name = string.IsNullOrWhiteSpace(profile.name) ? profile.first_name + " " + profile.last_name : profile.name,
                            sex = profile.gender,
                            id = profile.id
                        };
                    }
                    catch
                    {
                    }
                }
            }
            return null;
        }

        public FacebookUserProfile GetProfileFromMessages(string MessagesId, string pageAccessToken)
        {
            try
            {
                //var url = _facebookGraphApiUrl + "/" + userId + "?fields=id,name,email,first_name,last_name,picture{url}&access_token=" + pageAccessToken;
                var url = _facebookGraphApiUrl + "/" + MessagesId + "?fields=from&access_token=" + pageAccessToken;
                var profile = Core.Helpers.WebHelper.HttpGetAsync<FacebookCommentChildenData>(url).Result;
                return new FacebookUserProfile
                {
                    avatar = "",
                    first_name = "",
                    last_name = "",
                    name = profile.from.name,
                    sex = "",
                    id = profile.from.id
                };
            }
            catch (Exception ex)
            { return null; }
        }

        public FacebookUserProfile GetProfileFromMessagesTo(string MessagesId, string pageAccessToken)
        {
            try
            {
                //var url = _facebookGraphApiUrl + "/" + userId + "?fields=id,name,email,first_name,last_name,picture{url}&access_token=" + pageAccessToken;
                var url = _facebookGraphApiUrl + "/" + MessagesId + "?fields=to&access_token=" + pageAccessToken;
                var profile = Core.Helpers.WebHelper.HttpGetAsync<FacebookCommentChildenData>(url).Result;
                return new FacebookUserProfile
                {
                    avatar = "",
                    first_name = "",
                    last_name = "",
                    name = profile.from.name,
                    sex = "",
                    id = profile.from.id
                };
            }
            catch (Exception ex)
            { return null; }
        }

        public static string GetUniqueUserId(string url, string pageScopedId, string appScopedId)
        {
            string userId = "";
            try
            {
                if (!string.IsNullOrWhiteSpace(url))
                {
                    userId = Core.Helpers.CommonHelper.GetFileNameFromUrl(url);
                    userId = string.IsNullOrWhiteSpace(userId) && userId.IndexOf("_") >= 0 ? userId.Substring(0, userId.LastIndexOf('_')) : "";
                    userId = string.IsNullOrWhiteSpace(userId) && userId.IndexOf("_") >= 0 ? (userId.Substring(userId.IndexOf('_') + 1)) : "";
                    if (string.IsNullOrWhiteSpace(userId) || userId.IndexOf("/") >= 0 || userId.IndexOf("profilepic") >= 0)
                    {
                        var paraUrl = Core.Helpers.CommonHelper.GetPrameterFromUrl(url);
                        if (paraUrl != null && paraUrl.Count > 0)
                        {
                            foreach (var pa in paraUrl)
                            {
                                userId = pa.Value;
                                break;
                            }
                        }
                    }
                    userId = (userId == "10150004552801856_220367501106153455" || userId == "235267080260830_4959840936903883224" || userId == "10150004552801901_469209496895221757") ? string.IsNullOrWhiteSpace(appScopedId) ? pageScopedId : appScopedId : userId;
                }
            }
            catch { }
            if (string.IsNullOrWhiteSpace(userId))
            {
                userId = !string.IsNullOrWhiteSpace(pageScopedId) ? pageScopedId : appScopedId;
            }
            return userId;
        }


        public string GetUserAppScopedId(Channel channel, string msgId)
        {
            var sender_id = "";
            try
            {
                string pageId = channel.ext_id;
                var url = _facebookGraphApiUrl + "/" + msgId + "?fields=id,from,to,created_time&access_token=" + channel.token;
                var msg = Core.Helpers.WebHelper.HttpGetAsync<FacebookMessage>(url).Result;
                sender_id = msg.from.id != pageId ? msg.from.id : msg.to.data[0].id;
            }
            catch
            {
            }
            if (string.IsNullOrWhiteSpace(sender_id))
            {
                try
                {
                    string pageId = channel.ext_id;
                    var url = _facebookGraphApiUrl + "/" + msgId + "?fields=id,from&access_token=" + channel.token;
                    var msg = Core.Helpers.WebHelper.HttpGetAsync<FacebookMessage>(url).Result;
                    sender_id = msg.from.id != pageId ? msg.from.id : "";
                }
                catch { }
            }
            if (string.IsNullOrWhiteSpace(sender_id))
            {
                try
                {
                    string pageId1 = channel.ext_id;
                    var url1 = _facebookGraphApiUrl + "/" + msgId + "?fields=id,parent&access_token=" + channel.token;
                    var msg1 = Core.Helpers.WebHelper.HttpGetAsync<FacebookMessageParent>(url1).Result;
                    sender_id = msg1.parent.from.id != pageId1 ? msg1.parent.from.id : "";
                }
                catch { }
            }
            if (string.IsNullOrWhiteSpace(sender_id))
            {
                try
                {
                    string pageId = channel.ext_id;
                    var url = _facebookGraphApiUrl + "/" + msgId + "?fields=comments{from}&access_token=" + channel.token;
                    var msg = Core.Helpers.WebHelper.HttpGetAsync<FacebookCommnet>(url).Result;
                    sender_id = msg.comments.data[0].from.id != pageId ? msg.comments.data[0].from.id : "";
                }
                catch { }
            }
            return sender_id;
        }

        public Customer GetCustomer(Channel channel, string customer_id, string msgId, string pageScopedId, string appScopedId)
        {
            string business_id = channel.business_id;
            string channel_id = channel.id;
            string pageId = channel.ext_id;

            if (string.IsNullOrWhiteSpace(appScopedId) && !string.IsNullOrWhiteSpace(pageScopedId))
            {
                appScopedId = GetAppScopedId(pageScopedId, channel.token).Result;
            }

            if (string.IsNullOrWhiteSpace(appScopedId) && !string.IsNullOrWhiteSpace(msgId))
            {
                appScopedId = GetUserAppScopedId(channel, msgId);
            }

            if (string.IsNullOrWhiteSpace(pageScopedId) && !string.IsNullOrWhiteSpace(appScopedId))
            {
                pageScopedId = GetPageScopedId(appScopedId, pageId).Result;
            }


            FacebookUserProfile profile = null;
            string user_id = null;

            if (string.IsNullOrWhiteSpace(customer_id))
            {
                profile = string.IsNullOrWhiteSpace(appScopedId) ? null : GetProfile(appScopedId, channel.token);
                if (profile == null && !string.IsNullOrWhiteSpace(pageScopedId)) profile = GetPageScopedProfile(pageScopedId, channel.token);

                user_id = profile == null ? null : FacebookConversationService.GetUniqueUserId(profile.avatar, pageScopedId, appScopedId);
                //user_id = appScopedId != null ? appScopedId : profile != null ? profile.id : null;
                if (profile == null)
                {
                    profile = GetProfile(msgId, channel.token);
                    if (profile == null)
                    {
                        profile = GetProfileFromMessages(msgId, channel.token);
                        if (profile != null && profile.id == channel.ext_id)
                            profile = GetProfileFromMessagesTo(msgId, channel.token);
                    }
                    user_id = profile == null ? null : FacebookConversationService.GetUniqueUserId(profile.avatar, pageScopedId, appScopedId);
                    // user_id = appScopedId != null ? appScopedId : profile != null ? profile.id : null;
                }
                if (string.IsNullOrWhiteSpace(user_id) ||
                    (profile == null || string.IsNullOrWhiteSpace(profile.name)
                    && string.IsNullOrWhiteSpace(profile.first_name)
                    && string.IsNullOrWhiteSpace(profile.last_name)))
                    return null;

                // customer_id = CustomerService.FormatId(business_id, user_id);
                customer_id = user_id;
            }

            Customer customer = _customerService.GetById(business_id, customer_id);
            //if (customer != null && customer.id.IndexOf("_") < 0)
            //{
            ////Customer customer = null;
            //    var cus = _customerService.GetCustomersAppId(business_id,user_id).Result;
            //    if (cus != null && cus.Count > 0)
            //        customer = cus[0];
            //}
            DateTime now = DateTime.UtcNow;

            if (customer == null)
            {
                if (customer_id.IndexOf(business_id) < 0)
                {
                    customer_id = CustomerService.FormatId01(business_id, customer_id);
                    customer = _customerService.GetById(business_id, customer_id);
                }

                if (customer == null)
                    customer = new Domain.Entities.Customer
                    {
                        id = customer_id,
                        global_id = user_id,
                        ext_id = pageScopedId,
                        app_id = appScopedId,
                        business_id = business_id,
                        channel_id = channel_id,
                        status = "pending",
                        created_time = now,
                        updated_time = now,
                        unread = true,
                        nonreply = true
                    };
            }
            else
            {
                customer.ext_id = string.IsNullOrWhiteSpace(customer.ext_id) ? pageScopedId : customer.ext_id; ;
                customer.app_id = string.IsNullOrWhiteSpace(customer.app_id) ? appScopedId : customer.app_id;
            }
            if ((string.IsNullOrWhiteSpace(customer.name) || string.IsNullOrWhiteSpace(customer.avatar) || string.IsNullOrWhiteSpace(customer.sex)))
            {
                if (profile == null)
                {
                    //profile = string.IsNullOrWhiteSpace(appScopedId) ? null : GetProfile(appScopedId, channel.token);
                    //if (profile == null && !string.IsNullOrWhiteSpace(pageScopedId)) profile = GetPageScopedProfile(pageScopedId, channel.token);
                    if (!string.IsNullOrWhiteSpace(pageScopedId))
                        profile = GetPageScopedProfile(pageScopedId, channel.token);
                    if (profile == null)
                        profile = string.IsNullOrWhiteSpace(appScopedId) ? null : GetProfile(appScopedId, channel.token);
                }
                if (profile != null)
                {
                    customer.updated_time = now;
                    customer.avatar = string.IsNullOrWhiteSpace(customer.avatar) ? profile.avatar : customer.avatar;

                    var url = customer.avatar;
                    if (!string.IsNullOrWhiteSpace(url) && !url.Contains("hibaza") && !url.Contains("firebase"))
                    {
                        try
                        {
                            customer.avatar = DownloadToLocalAsync(business_id, "customer_avatars", customer.id, new Uri(url)).Result;
                        }
                        catch { customer.avatar = profile.avatar; }
                    }

                    customer.name = profile.name ?? profile.first_name + " " + profile.last_name;
                    customer.first_name = profile.first_name;
                    customer.last_name = profile.last_name;
                    customer.email = profile.email;
                    customer.sex = profile.sex;
                }
            }
            return customer;

        }


        private string GetLocalUrl(string business_id, string folder, string message_id, string fileName, Uri uri)
        {
            try
            {
                return $"{_appSettings.Value.BaseUrls.Web}{business_id}/{folder}/{message_id}/{fileName}";
            }
            catch { return uri.AbsoluteUri; }
        }

        private async System.Threading.Tasks.Task<string> DownloadToLocalAsync(string business_id, string folder, string thread_id, Uri requestUri)
        {
            try
            {
                if (requestUri != null)
                {
                    var rs = await ImagesService.UpsertImageStore(requestUri.AbsoluteUri, _appSettings.Value);
                    return rs;
                }
            }
            catch { }
            return requestUri.AbsoluteUri;
            //try
            //{
            //    string fileName = requestUri.Segments.Last();

            //    fileName = Core.Helpers.CommonHelper.removeSpecialFile(fileName);

            //    var fileId = "";
            //    if ((thread_id + fileName).Length < 100)
            //        fileId = thread_id + "_" + Core.Helpers.CommonHelper.GenerateDigitUniqueNumber() + "_" + (fileName ?? "");
            //    else
            //        fileId = thread_id + "_" + fileName;

            //    var dir = Path.Combine(Directory.GetCurrentDirectory(), _appSettings.Value.PathToFileDocuments);
            //    var fullName = Path.Combine(dir, fileId);

            //    if (!Directory.Exists(dir))
            //        Directory.CreateDirectory(dir);

            //    bool useFirbase = false;
            //    if (useFirbase)
            //    {
            //        using (var client = new HttpClient())
            //        using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
            //        using (Stream contentStream = await (await client.SendAsync(request)).Content.ReadAsStreamAsync())
            //        {
            //            //var task = new FirebaseStorage(_appSettings.Value.FirebaseDB.StorageBucket).Child(root_id).Child(folder).Child(item_id).Child(fileName).PutAsync(contentStream);
            //            var task = _messageService.UploadAttachmentToFirebaseStorage(business_id, folder, fileId, contentStream);
            //            return await task;
            //        }
            //    }
            //    else
            //    {
            //        // var path = Path.Combine(Directory.GetCurrentDirectory(), @"Documents", "Attachments", fileId);

            //        using (var client = new HttpClient())
            //        using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
            //        using (Stream contentStream = await (await client.SendAsync(request)).Content.ReadAsStreamAsync(),
            //         stream = new FileStream(fullName, FileMode.Create, FileAccess.Write, FileShare.None, 3145728, true))
            //        {
            //            await contentStream.CopyToAsync(stream);
            //        }
            //        //_appSettings.Value.BaseUrls.Api + 
            //        var url = _appSettings.Value.BaseUrls.Api + "Documents/" + fileId;
            //        return url;
            //    }
            //}
            //catch {
            //    return requestUri.AbsoluteUri; }
        }

        //public async Task<int> GetFacebookMessages(Channel bc, string conversationId, string conversationLink, string url, string job_id, bool real_time_update)
        //{
        //    string pageId = bc.ext_id;
        //    string business_id = bc.business_id;
        //    string conversationId_Old = conversationId;
        //    int count = 0;
        //    FacebookMessageFeed item = null;
        //    string last_url = url;
        //    url = string.IsNullOrWhiteSpace(url) ? _facebookGraphApiUrl + "/" + conversationId + "/messages?fields=id,message,attachments,from,to,created_time&limit=20&access_token=" + bc.token : url;
        //    if (string.IsNullOrWhiteSpace(url)) return count;
        //    item = await Core.Helpers.WebHelper.HttpGetAsync<FacebookMessageFeed>(url);

        //    if (item == null || item.data == null) return count;

        //    var channel_id = ChannelService.FormatId(business_id, pageId);
        //    bool conversation_is_mapped = false;

        //    var conversation_id = ThreadService.FormatId(business_id, conversationId);
        //    var conversation = _conversationService.GetById(business_id, conversation_id);
        //    string owner_ext_id = conversation == null ? "" : conversation.owner_ext_id;
        //    if (conversationId != owner_ext_id && !string.IsNullOrWhiteSpace(owner_ext_id))
        //    {
        //        conversationId = owner_ext_id;
        //        conversation_is_mapped = true;
        //        _threadService.Delete(business_id, ThreadService.FormatId(business_id, conversationId_Old));
        //    }


        //    foreach (var msg in item.data)
        //    {
        //        count++;
        //        var msgId = msg.id;
        //        string senderId = msg.from.id;
        //        string recipientId = msg.to.data[0].id;
        //        var owner_app_id = senderId == pageId ? recipientId : senderId;

        //        var owner_id = CustomerService.FormatId(business_id, owner_app_id);
        //        var fm = new FacebookMessageObject
        //        {
        //            msgId = msgId,
        //            conversationId = conversationId,
        //            senderId = senderId,
        //            sender_name = msg.from.name,
        //            recipientId = recipientId,
        //            recipient_name = msg.to.data[0].name,
        //            timestamp = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(msg.created_time.AddHours(-7)),
        //            text = msg.message,
        //            type = "text",
        //            thread_type = "message",
        //            channel_type = bc.type,
        //            urls = new List<string>()
        //        };
        //        var thread_id = ThreadService.FormatId(business_id, fm.conversationId);
        //        var message_id = MessageService.FormatId(thread_id, fm.msgId);

        //        if (msg.attachments != null && msg.attachments.data != null)
        //        {
        //            foreach (var att in msg.attachments.data)
        //                if (att.image_data != null && !string.IsNullOrWhiteSpace(att.image_data.url))
        //                {
        //                    fm.urls.Add(att.image_data.url);
        //                    fm.type = att.type;
        //                }
        //            fm.url = fm.urls.FirstOrDefault();
        //            fm.type = fm.urls.Count > 1 ? "multiple_images" : fm.urls.Count == 1 ? "image" : fm.type;
        //        }


        //        bool is_new_messsage = false;
        //        string tid = "";
        //        var message = CreateMessage(bc, fm, message_id, "", owner_id, real_time_update, out is_new_messsage, out tid);
        //        if (message != null && !is_new_messsage && string.IsNullOrWhiteSpace(owner_ext_id))
        //        {
        //            owner_ext_id = (message.sender_ext_id == pageId ? message.recipient_ext_id : message.sender_ext_id);
        //            owner_ext_id = owner_ext_id == owner_app_id ? "" : owner_ext_id;
        //        }

        //        if (message != null && count == 1 && string.IsNullOrWhiteSpace(last_url) && conversation_is_mapped)
        //        {
        //            await CreateThread(bc, message, owner_id, owner_ext_id, owner_app_id, real_time_update);
        //        }

        //        if (!conversation_is_mapped && !string.IsNullOrWhiteSpace(owner_ext_id))
        //        {
        //            conversation_is_mapped = true;
        //            DateTime now = DateTime.UtcNow;
        //            _conversationService.Upsert(new Conversation { business_id = business_id, id = conversation_id, owner_ext_id = owner_ext_id, owner_app_id = owner_app_id, link = conversationLink, timestamp = fm.timestamp, created_time = now, updated_time = now });

        //        }
        //        else
        //        {

        //            if (count == 1 && string.IsNullOrWhiteSpace(last_url) && (conversation == null || string.IsNullOrWhiteSpace(conversation.link)))
        //            {
        //                DateTime now = DateTime.UtcNow;
        //                _conversationService.Upsert(new Conversation { business_id = business_id, id = conversation_id, owner_ext_id = owner_ext_id, owner_app_id = owner_app_id, link = conversationLink, timestamp = fm.timestamp, created_time = now, updated_time = now });
        //            }

        //        }
        //    }


        //    if (item.paging != null && !string.IsNullOrWhiteSpace(item.paging.Next))
        //    {
        //        var nextUrl = item.paging.Next;
        //        try
        //        {
        //            await GetFacebookMessages(bc, conversationId_Old, conversationLink, nextUrl, job_id, real_time_update);
        //        }
        //        catch
        //        {
        //            BackgroundJob.Enqueue<FacebookConversationService>(x => x.GetFacebookMessages(bc, conversationId_Old, conversationLink, nextUrl, job_id, real_time_update));
        //        }
        //    }

        //    return count;
        //}


        public async Task<int> SaveMessages(Channel bc, Conversation conversation, string url, string job_id, int limit, bool real_time_update)
        {
            int count = 0;

            try
            {
                conversation = _conversationService.GetById(bc.business_id, conversation.id);

                if (conversation == null || string.IsNullOrWhiteSpace(conversation.owner_ext_id)) return count;

                string pageId = bc.ext_id;
                url = string.IsNullOrWhiteSpace(url) ? _facebookGraphApiUrl + "/" + conversation.ext_id + "/messages?fields=id,message,attachments,from,to,created_time&limit=" + limit + "&access_token=" + bc.token : url;
                FacebookMessageFeed messages = await Core.Helpers.WebHelper.HttpGetAsync<FacebookMessageFeed>(url);

                if (messages == null || messages.data == null || messages.data.Count() == 0)
                {
                    return count;
                }


                foreach (var msg in messages.data)
                {
                    count++;
                    var message_id = MessageService.FormatId(bc.business_id, msg.id);
                    var message = _messageService.GetById(bc.business_id, message_id);
                    //if (message != null && (string.IsNullOrWhiteSpace(conversation.owner_app_id) || string.IsNullOrWhiteSpace(conversation.owner_ext_id)))
                    //{
                    //    string owner_ext_id = message.sender_ext_id == message.channel_ext_id ? message.recipient_ext_id : message.sender_ext_id;
                    //    string owner_app_id = message.sender_ext_id == message.channel_ext_id ? msg.to.data[0].id : msg.from.id;
                    //    owner_app_id = string.IsNullOrWhiteSpace(owner_app_id) ? conversation.owner_app_id : owner_app_id;
                    //    owner_ext_id = string.IsNullOrWhiteSpace(owner_ext_id) ? conversation.owner_ext_id : owner_ext_id;
                    //    owner_ext_id = owner_ext_id == owner_app_id ? null : owner_ext_id;
                    //    if ((conversation.owner_ext_id != owner_ext_id && !string.IsNullOrWhiteSpace(owner_ext_id)) || (conversation.owner_app_id != owner_app_id && !string.IsNullOrWhiteSpace(owner_app_id)))
                    //    {
                    //        _conversationService.UpdateOwner(bc.business_id, conversation.id, owner_ext_id, owner_app_id);
                    //        conversation.owner_ext_id = owner_ext_id;
                    //        conversation.owner_app_id = owner_app_id;
                    //    }
                    //}

                    // if (message != null || string.IsNullOrWhiteSpace(conversation.owner_ext_id)) continue;

                    if ((message != null && message.customer_id != null) || string.IsNullOrWhiteSpace(conversation.owner_ext_id)) continue;

                    DateTime now = DateTime.UtcNow;
                    Node node = new Node
                    {
                        id = msg.id,
                        business_id = bc.business_id,
                        channel_id = bc.id,
                        type = bc.type + ".message",
                        created_time = msg.created_time,
                        timestamp = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(msg.created_time),
                        updated_time = now
                    };

                    FacebookWebhookData obj = new FacebookWebhookData
                    {
                        @object = "page",
                        entry = new List<FacebookEntry>()
                    };

                    FacebookEntry entry = new FacebookEntry
                    {
                        id = pageId,
                        time = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(msg.created_time.AddHours(-7)),
                        messaging = new List<FacebookMessagingEvent>()

                    };

                    FacebookMessagingEvent item = new FacebookMessagingEvent
                    {
                        sender = new FacebookUser { id = msg.from.id == pageId ? pageId : conversation.owner_ext_id },
                        recipient = new FacebookUser { id = msg.to.data[0].id == pageId ? pageId : conversation.owner_ext_id },
                        timestamp = entry.time,
                        message = new FacebookMessageGet { text = msg.message, mid = msg.id.StartsWith("m_") ? msg.id.Substring(2) : msg.id }
                    };

                    if (msg.attachments != null && msg.attachments.data != null)
                    {
                        item.message.attachments = new List<FacebookAttachmentGet>();
                        foreach (var att in msg.attachments.data)
                            if (att.image_data != null && !string.IsNullOrWhiteSpace(att.image_data.url))
                            {
                                //item.message.attachments.Add(new FacebookAttachmentPost { type = att.type, payload = new FacebookAttachmentPayload { url = att.image_data.url } });
                                item.message.attachments.Add(new FacebookAttachmentGet { type = att.type, payload = new FacebookPayload { url = att.image_data.url } });
                            }
                    }

                    entry.messaging.Add(item);
                    obj.entry.Add(entry);
                    node.data = JsonConvert.SerializeObject(obj);
                    _nodeService.CreateNode(node);

                }


                if (messages.paging != null && !string.IsNullOrWhiteSpace(messages.paging.Next) && limit > count)
                {
                    var nextUrl = messages.paging.Next;
                    await SaveMessages(bc, conversation, nextUrl, job_id, limit - count, real_time_update);
                    //BackgroundJob.Enqueue<FacebookConversationService>(x => x.SaveMessages(bc, conversation, nextUrl, job_id, limit - count, real_time_update));
                }
                else
                {
                    _conversationService.UpdateStatus(bc.business_id, conversation.id, "succeeded");
                }
                return count;
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Message",
                    link = url,
                    details = JsonConvert.SerializeObject(ex.StackTrace),
                    name = string.Format("downoad messages for conversationId: {0}", conversation.id)
                });
            }
            return count;
        }

        public async System.Threading.Tasks.Task<int> FixConversationExtId(string business_id, string channel_id, int limit)
        {
            int count = 0;
            List<Channel> list = new List<Channel>();
            if (!string.IsNullOrWhiteSpace(channel_id) && !string.IsNullOrWhiteSpace(business_id))
            {
                list.Add(_channelService.GetById(business_id, channel_id));
            }
            else
            {
                list = (await _channelService.GetChannels(business_id, 0, 2000)).Where(c => c.active).ToList();
            }
            foreach (var channel in list)
            {
                foreach (var conversation in await _conversationService.GetConversationWhereExtIdIsNull(business_id, channel.id, limit))
                {
                    if (string.IsNullOrWhiteSpace(conversation.owner_ext_id) && !string.IsNullOrWhiteSpace(conversation.ext_id))
                    {
                        var mid = MessageService.FormatId(business_id, conversation.ext_id.Replace("t_mid", "m_mid"));
                        var message = _messageService.GetById(business_id, mid);
                        if (message != null && message.thread_type == "message" && message.sender_ext_id != channel.ext_id && !string.IsNullOrWhiteSpace(message.sender_ext_id))
                        {
                            conversation.owner_ext_id = message.sender_ext_id;
                        }

                    }

                    //if (string.IsNullOrWhiteSpace(conversation.owner_ext_id) && !string.IsNullOrWhiteSpace(conversation.owner_app_id))
                    //{
                    //    conversation.owner_ext_id = await GetPageScopedId(conversation.owner_app_id, channel.ext_id);
                    //}

                    if (!string.IsNullOrWhiteSpace(conversation.owner_ext_id))
                    {
                        count++;
                        _conversationService.Upsert(conversation);
                    }

                }
            }
            return count;
        }

        public async System.Threading.Tasks.Task<int> DownloadSingleConversation(string business_id, string channel_id, string pageId, string conversationId, int limit, bool real_time_update)
        {
            try
            {
                List<Channel> list = new List<Channel>();
                if (!string.IsNullOrWhiteSpace(channel_id) && !string.IsNullOrWhiteSpace(business_id))
                {
                    list.Add(_channelService.GetById(business_id, channel_id));
                }
                else
                {
                    list = _channelService.GetChannelsByExtId(pageId).Result.Where(b => b.active).ToList();
                }

                int count = 0;
                foreach (var bc in list)
                {
                    var conversation = _conversationService.GetById(bc.business_id, ConversationService.FormatId(bc.business_id, conversationId));
                    var url = _facebookGraphApiUrl + "/" + conversationId + "/?fields=id,senders,link,updated_time&access_token=" + bc.token;
                    var item = await Core.Helpers.WebHelper.HttpGetAsync<FacebookConversation>(url);
                    var now = DateTime.UtcNow;
                    if (conversation == null && item != null)
                    {
                        conversation = new Conversation { business_id = bc.business_id, channel_id = bc.id, id = ConversationService.FormatId(bc.business_id, item.id), ext_id = item.id, link = item.link, timestamp = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(item.updated_time), created_time = now, updated_time = now };
                    }
                    else
                    {
                        conversation.status = "pending";
                        conversation.channel_id = bc.id;
                        conversation.ext_id = item.id;
                        conversation.link = item.link;
                        conversation.timestamp = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(item.updated_time);
                    }
                    if (string.IsNullOrWhiteSpace(conversation.owner_app_id) && item.senders != null && item.senders.data != null)
                        foreach (var u in item.senders.data)
                        {
                            if (u.id != pageId)
                            {
                                conversation.owner_app_id = u.id;
                                break;
                            }
                        }


                    if (string.IsNullOrWhiteSpace(conversation.owner_ext_id) && !string.IsNullOrWhiteSpace(conversation.ext_id))
                    {
                        var mid = MessageService.FormatId(business_id, conversation.ext_id.Replace("t_", "m_mid"));
                        var message = _messageService.GetById(bc.business_id, mid);
                        if (message != null && message.thread_type == "message" && message.sender_ext_id != bc.ext_id && !string.IsNullOrWhiteSpace(message.sender_ext_id))
                        {
                            conversation.owner_ext_id = message.sender_ext_id;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(conversation.owner_ext_id) && !string.IsNullOrWhiteSpace(conversation.owner_app_id))
                    {
                        conversation.owner_ext_id = await GetPageScopedId(conversation.owner_app_id, bc.ext_id);
                    }

                    _conversationService.Upsert(conversation);

                    await SaveMessages(bc, conversation, "", "", limit, real_time_update);
                    count++;
                }
                return count;
            }
            catch { }
            return 0;
        }

        public async System.Threading.Tasks.Task<int> AutoDownloadConversations(string business_id, int offset, bool real_time_update = true)
        {
            int count = 0;
            foreach (var bc in _channelService.GetChannels(business_id, 0, 2000).Result)
            {
                await DownloadConversations(bc, "", "", DateTime.UtcNow.AddMinutes(-offset), DateTime.UtcNow, real_time_update);
                count++;
            }
            return count;
        }

        public async System.Threading.Tasks.Task<int> DownloadConversations(Channel bc, string url, string job_id, DateTime from, DateTime to, bool real_time_update)
        {
            var total = 0;
            string pageId = bc.ext_id;
            try
            {
                string business_id = bc.business_id;
                long until = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(to);
                long since = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(from);
                url = string.IsNullOrWhiteSpace(url) ? _facebookGraphApiUrl + "/" + pageId + "/conversations?fields=id,link,subject,senders,updated_time&limit=50&until=" + until + "&since=" + since + "&access_token=" + bc.token : url;
                var dataFb = await Core.Helpers.WebHelper.HttpGetAsync<FacebookConversationFeed>(url);
                if (dataFb == null || dataFb.Data == null) return total;
                foreach (var item in dataFb.Data)
                {
                    var conversation = _conversationService.GetById(bc.business_id, ConversationService.FormatId(bc.business_id, item.id));
                    DateTime now = DateTime.UtcNow;

                    if (conversation == null)
                    {
                        conversation = new Conversation { business_id = bc.business_id, channel_id = bc.id, id = ConversationService.FormatId(bc.business_id, item.id), ext_id = item.id, link = item.link, timestamp = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(item.updated_time), created_time = now, updated_time = now };
                    }
                    else
                    {
                        conversation.status = "pending";
                        conversation.channel_id = bc.id;
                        conversation.ext_id = item.id;
                        conversation.link = item.link;
                        conversation.timestamp = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(item.updated_time);
                    }
                    if (string.IsNullOrWhiteSpace(conversation.owner_app_id) && item.senders != null && item.senders.data != null)
                        foreach (var u in item.senders.data)
                        {
                            if (u.id != pageId)
                            {
                                conversation.owner_app_id = u.id;
                                break;
                            }
                        }

                    if (string.IsNullOrWhiteSpace(conversation.owner_ext_id) && !string.IsNullOrWhiteSpace(conversation.ext_id))
                    {
                        var mid = MessageService.FormatId(business_id, conversation.ext_id.Replace("t_", "m_mid"));
                        var message = _messageService.GetById(bc.business_id, mid);
                        if (message != null && message.thread_type == "message" && message.sender_ext_id != bc.ext_id && !string.IsNullOrWhiteSpace(message.sender_ext_id))
                        {
                            conversation.owner_ext_id = message.sender_ext_id;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(conversation.owner_ext_id) && !string.IsNullOrWhiteSpace(conversation.owner_app_id))
                    {
                        conversation.owner_ext_id = await GetPageScopedId(conversation.owner_app_id, pageId);
                    }
                    _conversationService.Upsert(conversation);
                }

                if (dataFb.Paging != null && !string.IsNullOrWhiteSpace(dataFb.Paging.Next))
                {
                    var nextUrl = dataFb.Paging.Next;
                    //total += await DownloadConversations(bc, nextUrl, "", from, to, real_time_update);
                    BackgroundJob.Enqueue<FacebookConversationService>(x => x.DownloadConversations(bc, nextUrl, "", from, to, real_time_update));
                }
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Facebook",
                    link = url,
                    details = JsonConvert.SerializeObject(ex.StackTrace),
                    name = string.Format("Get facebook conversations for pageId: {0}", pageId)
                });
            }
            return total;
        }


        public async System.Threading.Tasks.Task<int> DownloadMessages(Channel bc, int limit, long since, long until, bool real_time_update)
        {
            var total = 0;
            string pageId = bc.ext_id;
            try
            {
                Paging page = new Paging { Limit = limit, Previous = since.ToString(), Next = until.ToString() };
                IEnumerable<Conversation> conversations = await _conversationService.GetConversations(bc.business_id, bc.id, page);
                foreach (var item in conversations)
                    if (item.status != "processing")
                    {
                        _conversationService.UpdateStatus(item.business_id, item.id, "processing");
                        //await SaveMessages(bc, item, "", "", limit, real_time_update);
                        BackgroundJob.Enqueue<FacebookConversationService>(x => x.SaveMessages(bc, item, "", "", limit, real_time_update));
                    }

                if (conversations != null && conversations.Count() > 0)
                {
                    var next = conversations.Last().timestamp - 1;
                    //total += await DownloadMessages(bc, limit, since, next, real_time_update);
                    BackgroundJob.Enqueue<FacebookConversationService>(x => x.DownloadMessages(bc, limit, since, next, real_time_update));
                }
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Conversation",
                    details = JsonConvert.SerializeObject(ex.StackTrace),
                    name = string.Format("downoad messages for pageId: {0}", pageId)
                });
            }
            return total;
        }

        public async Task<FacebookMessagePostResponse> PostFacebookAttachment(FacebookMessagePostData data, string accessToken)
        {
            var url = _facebookGraphApiUrl + "/me" + "/message_attachments?access_token=" + accessToken;
            var r = await Core.Helpers.WebHelper.HttpPostAsync<FacebookMessagePostResponse>(url, data);
            return r;
        }

        public async Task<FacebookTextMessageResponse> PostFacebookTextMessageUsingConversation(string conversationExtId, string message, string accessToken)
        {
            var url = _facebookGraphApiUrl + "/" + conversationExtId + "/messages?access_token=" + accessToken;
            var r = await Core.Helpers.WebHelper.HttpPostAsync<FacebookTextMessageResponse>(url, new { message });
            return r;
        }

        public async Task<FacebookMessagePostResponse> PostFacebookMessage(FacebookMessagePostData data, string accessToken)
        {
            var url = _facebookGraphApiUrl + "/me" + "/messages?access_token=" + accessToken;
            var r = await Core.Helpers.WebHelper.HttpPostAsync<FacebookMessagePostResponse>(url, data);
            return r;
        }

        public void SaveWebhookReferral(string business_id, string channel_id, string pageId, FacebookMessagingEvent referralEvent)
        {
            List<Channel> list = new List<Channel>();

            if (!string.IsNullOrWhiteSpace(channel_id) && !string.IsNullOrWhiteSpace(business_id))
            {
                list.Add(_channelService.GetById(business_id, channel_id));
            }
            else
            {
                list = _channelService.GetChannelsByExtId(pageId).Result.Where(b => b.active).ToList();
            }

            foreach (var bc in list)
            {
                var thread_id = ThreadService.FormatId(bc.business_id, referralEvent.sender.id);
                string data = referralEvent.postback != null && referralEvent.postback.referral != null && !string.IsNullOrWhiteSpace(referralEvent.postback.referral.Ref) ? referralEvent.postback.referral.Ref : referralEvent.referral != null && !string.IsNullOrWhiteSpace(referralEvent.referral.Ref) ? referralEvent.referral.Ref : "";
                data = data ?? "";
                data = data.Trim();
                var threads = _threadService.GetById(business_id, thread_id);

                if (!string.IsNullOrWhiteSpace(thread_id) && !string.IsNullOrWhiteSpace(bc.business_id) && !string.IsNullOrWhiteSpace(data))
                {
                    string[] ps = data.Split(',');
                    var type = ps.Length > 0 ? ps[0] : "";
                    var sku = ps.Length > 1 ? ps[1] : "";
                    var pid = ps.Length > 2 ? ps[2] : "";
                    var url = ps.Length > 3 ? ps[3] : "";
                    if (ps.Length > 4)
                    {
                        url = data.Substring(type.Length + sku.Length + pid.Length + 3);
                    }

                    var referral = _referralService.CreateReferral(bc.business_id, threads, referralEvent.timestamp.ToString(), referralEvent.sender.id, referralEvent.recipient.id, data, thread_id);

                }

            }
        }
        public void PostAsync(string url, string jsonContent)
        {
            try
            {
                var client = new HttpClient();
                client.PostAsync(url, new StringContent(jsonContent, Encoding.UTF8, "application/json"));

            }
            catch (Exception ex) { }
        }

        public async System.Threading.Tasks.Task SaveWebhookDelivery(string pageId, FacebookMessagingEvent deliveryEvent)
        {
            var senderId = deliveryEvent.sender.id;
            var recipientId = deliveryEvent.recipient.id;

            string owner_ext_id = senderId;
            string owner_app_id = "";
            if (pageId == owner_ext_id)
            {
                owner_ext_id = recipientId;
            }

            foreach (var bc in _channelService.GetChannelsByExtId(pageId).Result.Where(b => b.active))
            {
                var business_id = bc.business_id;
                string channel_id = bc.id;
                var thread_id = ThreadService.FormatId(channel_id, owner_ext_id);


                var owner_id = CustomerService.FormatId(business_id, owner_ext_id);
                bool is_new = false;
                string tid = "";

                foreach (var id in deliveryEvent.delivery.mids)
                {
                    var msgId = "m_" + id;
                    var message_id = MessageService.FormatId(thread_id, msgId);
                    //var message = _messageService.GetById(business_id, message_id);
                    //var fm = new FacebookMessageObject
                    //{
                    //    msgId = msgId,
                    //    conversationId = owner_ext_id,
                    //    senderId = senderId,
                    //    recipientId = recipientId,
                    //    timestamp = deliveryEvent.delivery.watertermark > 9999999999 ? deliveryEvent.delivery.watertermark / 1000 : deliveryEvent.delivery.watertermark,
                    //    text = "hibaza",
                    //    type = "text",
                    //    urls = new List<string>()
                    //};
                    //if (message == null)
                    //{
                    //    fm.text = "hibaza_new";
                    //}
                    //else
                    //{
                    //    fm.text = "hibaza_udpate";
                    //}
                    //CreateMessage(bc, fm, message_id, "", owner_id, false, out is_new, out tid);
                }
            }
        }

        public async Task<Attachment> UploadAttachments(Channel channel, Thread thread, string source_url, string image_url)
        {
            string id = MessageService.FormatAttachmentId(channel.id + "_facebook_message", image_url);
            var attachment = _messageService.GetAttachmentById(channel.business_id, channel.id, id);
            if (attachment == null)
            {
                FacebookMessagePostResponse res = await PostFacebookAttachment(new FacebookMessagePostData
                {
                    message = new FacebookMessagePost
                    {
                        attachment = new FacebookAttachmentPost { type = "image", payload = new FacebookAttachmentPayload { is_reusable = true, url = image_url } }
                    }
                }, channel.token);

                if (!string.IsNullOrWhiteSpace(res.attachment_id))
                {
                    attachment = new Attachment
                    {
                        id = id,
                        business_id = channel.business_id,
                        channel_id = channel.id,
                        attachment_id = res.attachment_id,
                        attachment_url = image_url,
                        target = "facebook_message",
                        source_url = source_url,
                        timestamp = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow),
                        type = "image"
                    };
                    _messageService.SaveAttachment(attachment);
                }
            }
            return attachment;
        }

        public async Task<int> SendPromotionMessage(string business_id, string channel_id, string message, int limit, int day = 1)
        {
            int count = 0;
            Paging page = new Paging { Limit = limit, Previous = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow.Date.AddDays(-day).AddHours(-12)).ToString(), Next = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow.Date.AddDays(-day).AddHours(12)).ToString() };
            foreach (var t in await _threadService.GetNoReponseThreads(business_id, channel_id, page))
                if (t.type == "message")
                {
                    var form = new MessageFormData
                    {
                        business_id = business_id,
                        channel_id = t.channel_id,
                        thread_id = t.id,
                        agent_id = null,
                        recipient_id = t.owner_ext_id,
                        tag = "",
                        image_urls = "",
                        description = "",
                        message = message
                    };
                    t.agent_id = null;
                    try
                    {
                        var data = new MessageData { image_urls = new List<string>(), attachment_urls = new List<string>(), message = form.message };
                        var channel = _channelService.GetById(business_id, t.channel_id);
                        SendMessage(channel, t, null, null);
                        count++;
                    }
                    catch (Exception ex)
                    {
                        _logService.Create(new Log
                        {
                            message = ex.Message,
                            category = "Facebook",
                            details = JsonConvert.SerializeObject(ex.StackTrace),
                            name = string.Format("Send promotion message to psid {0}", t.owner_ext_id)
                        });

                    }
                }
            return count;
        }

        public async System.Threading.Tasks.Task SendMessage(Channel channel, Thread thread, MessageFormData form, string msgId)
        {
            string recipientId = thread.owner_ext_id;
            var timestamp = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow);

            string channel_id = channel.id;
            var thread_id = ThreadService.FormatId(channel.business_id, recipientId);
            string message_id = MessageService.FormatId(channel.business_id, msgId);
            string pageId = channel.ext_id;

            var para = JsonConvert.DeserializeObject<dynamic>(form.para);
            var config = JsonConvert.DeserializeObject<dynamic>(form.config);

            List<payloadFb> elements = new List<payloadFb>();

            if (form.channel_format == "image-text" || form.channel_format == "image" || form.channel_format == "receipt")
            {
                elements = JsonConvert.DeserializeObject<List<payloadFb>>(JsonConvert.SerializeObject(para.message.attachment.payload.elements));
            }

            var fm = new FacebookMessageObject
            {
                msgId = msgId,
                conversationId = recipientId,
                ownerId = string.IsNullOrWhiteSpace(thread.owner_app_id) ? thread.owner_ext_id : thread.owner_app_id,
                senderId = pageId,
                sender_name = channel.name,
                recipientId = recipientId,
                recipient_name = "",
                customer_id = thread.customer_id,
                timestamp = timestamp,
                text = form.channel_format == "text" || form.channel_format == "message" ? para.message.text : "",
                type = elements.Count > 0 ? "multiple_images" : "text",
                template = config.template,
                tag = config.tag,
                thread_type = thread.type,
                channel_type = channel.type,
                url = elements.Count == 0 ? "" : elements[0].image_url,
                urls = elements.Select(x => x.image_url).ToList(),
                titles = elements.Select(x => x.title).ToList()
            };

            if (thread.type == "comment")
            {
                fm.msgParentId = form.recipient_id;
                fm.msgRootId = thread.link_ext_id;
                fm.conversationId = thread.ext_id;
                fm.liked = true;
                fm.hidden = true;
            }

            var owner_id = CustomerService.FormatId(channel.business_id, recipientId);
            bool is_new = false;
            string tid = "";
            var message = CreateMessage(channel, fm, message_id, thread.agent_id, owner_id, true, out is_new, out tid);

            string owner_ext_id = message.sender_ext_id;
            string owner_app_id = "";
            if (pageId == owner_ext_id)
            {
                owner_ext_id = message.recipient_ext_id;
            }
            thread.owner_app_id = !string.IsNullOrWhiteSpace(owner_app_id) ? owner_app_id : thread.owner_app_id;
            thread.owner_ext_id = !string.IsNullOrWhiteSpace(owner_ext_id) ? owner_ext_id : thread.owner_ext_id;
            thread.link_ext_id = thread.type == "comment" ? (message.root_ext_id ?? message.parent_ext_id) : null;

            var customer = _customerService.GetById(message.business_id, thread.customer_id);
            if (customer == null)
                customer = GetCustomer(channel, null, message.ext_id, thread.owner_ext_id, thread.owner_app_id);
            if (thread.customer_id == null)
                thread.customer_id = thread.customer_id == null ? customer.id : thread.customer_id;

            customer.unread = false;
            customer.nonreply = false;

            thread.unread = false;
            thread.nonreply = false;

            thread.owner_name = customer.name;
            thread.sender_name = customer.name;
            thread.agent_id = (customer == null || !string.IsNullOrWhiteSpace(thread.agent_id)) ? thread.agent_id : customer.agent_id;

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

            _threadService.CreateThread(thread, false);
            customer.active_thread = JsonConvert.SerializeObject(thread);
            _customerService.CreateCustomer(customer, thread, false);

            //CreateThread(channel, message, null, customer,owner_id,owner_ext_id,owner_app_id,true);
            if (!string.IsNullOrWhiteSpace(message.agent_id))
            {
                BackgroundJob.Enqueue<AgentService>(x => x.SetLastActivityTime(message.agent_id, Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow)));
            }
        }


        public async System.Threading.Tasks.Task FixMessagePhotoUrls(string business_id, string message_id, bool real_time_update)
        {
            try
            {
                var message = _messageService.GetById(business_id, message_id);
                if (message == null && message.url.Length < 10) return;
                //bool is_updated = true;
                //if (!string.IsNullOrWhiteSpace(message.url) && !message.url.ToLower().Contains("documents/"))
                //{
                if (!string.IsNullOrWhiteSpace(message.url))
                {
                    {
                        var un = message.url;

                        try
                        {
                            un = await DownloadToLocalAsync(message.business_id, "attachments", message.thread_id, new Uri(message.url));
                            //is_updated = true;
                        }
                        catch { un = message.url; }
                        message.url = un;
                        List<string> urls = new List<string>();
                        if (!string.IsNullOrWhiteSpace(message.urls) && message.urls != "[]")
                            try
                            {
                                List<string> imgs = JsonConvert.DeserializeObject<List<string>>(message.urls);
                                foreach (var url in imgs)
                                {
                                    var u = url;
                                    //if (!url.ToLower().Contains("documents/"))
                                    //{
                                    if (u != un)
                                        u = await DownloadToLocalAsync(message.business_id, "attachments", message.thread_id, new Uri(url));
                                    //  is_updated = false;
                                    //}
                                    urls.Add(u);
                                }
                                message.urls = JsonConvert.SerializeObject(urls);
                            }
                            catch
                            {
                            }

                    }
                }
                //.tif, .tiff .bmp .jpg, .c .gif .png .eps .raw, .cr2, .nef, .orf, .sr2
                if (!string.IsNullOrWhiteSpace(message.url) &&
                    (
                    message.url.IndexOf(".tif") >= 0 ||
                    message.url.IndexOf(".tiff") >= 0 ||
                    message.url.IndexOf(".bmp") >= 0 ||
                    message.url.IndexOf(".jpg") >= 0 ||
                    message.url.IndexOf(".jpeg") >= 0 ||
                    message.url.IndexOf(".gif") >= 0 ||
                    message.url.IndexOf(".png") >= 0
                    )
                    )
                    _messageService.CreateMessage(business_id, message, true);
            }
            catch { }
        }

        public void AutoUpdateCustomerIdNull()
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                try
                {
                    var messages = _messageService.GetMessagesWhereCustomerIsNull("", 1000).Result;
                    if (messages == null) return;

                    foreach (var m in messages)
                    {
                        if (string.IsNullOrWhiteSpace(m.message) && (
                        string.IsNullOrWhiteSpace(m.url) || m.url.Length < 10)
                        )
                            _messageService.deleteMessageidIfNull(m.business_id, m.id).Wait();
                        else
                        {

                            var channel = _channelService.GetById(m.business_id, m.channel_id);

                            Customer customer = null;
                            Thread thread = null;

                            thread = _threadService.GetById(m.business_id, m.thread_id);
                            if (thread == null)
                            {
                                customer = GetCustomer(channel, null, m.thread_type == "message" ? m.ext_id : m.id, null, null);
                                if (customer == null)
                                {
                                    var id1 = m.ext_id.Split('_');
                                    var id2 = m.id.Split('_');
                                    customer = GetCustomer(channel, null, m.thread_type == "message" ? id1[0] : id2[0], null, null);
                                }
                                if (customer != null && !string.IsNullOrWhiteSpace(customer.active_thread))
                                    thread = JsonConvert.DeserializeObject<Thread>(customer.active_thread);
                                if (thread == null)
                                {
                                    string owner_ext_id = m.sender_ext_id;
                                    string owner_app_id = "";
                                    if (channel.ext_id == owner_ext_id)
                                    {
                                        owner_ext_id = m.recipient_ext_id;
                                    }
                                    var owner_id = CustomerService.FormatId(m.business_id, owner_ext_id);

                                    CreateThread(channel, m, null, customer, owner_id, owner_ext_id, owner_app_id, false).Wait();
                                    thread = _threadService.GetById(m.business_id,
                                        !string.IsNullOrWhiteSpace(m.thread_id) ? m.thread_id : ThreadService.FormatId(m.business_id, m.conversation_ext_id));
                                }
                            }
                            if (thread != null)
                            {
                                if (customer == null)
                                {
                                    if (!string.IsNullOrWhiteSpace(thread.customer_id))
                                        customer = _customerService.GetById(m.business_id, thread.customer_id);

                                    if (customer == null)
                                    {
                                        var sender_id = GetUserAppScopedId(channel, m.thread_type == "message" ? m.ext_id : m.id);
                                        if (!string.IsNullOrWhiteSpace(sender_id) || customer == null)
                                        {
                                            if (string.IsNullOrWhiteSpace(sender_id))
                                                sender_id = m.channel_ext_id == m.sender_ext_id ? m.recipient_ext_id : m.sender_ext_id;

                                            customer = GetCustomer(channel, null, m.thread_type == "message" ? m.ext_id : m.id, null, sender_id);
                                        }
                                    }
                                }
                                if (customer != null && thread == null && !string.IsNullOrWhiteSpace(customer.active_thread))
                                    thread = JsonConvert.DeserializeObject<Thread>(customer.active_thread);

                                if (customer != null)
                                {
                                    m.customer_id = customer.id;
                                    m.thread_id = string.IsNullOrWhiteSpace(m.thread_id) ? thread.id : m.thread_id;
                                    thread.customer_id = customer.id;
                                    if (string.IsNullOrWhiteSpace(customer.active_thread))
                                    {
                                        customer.active_thread = JsonConvert.SerializeObject(thread);
                                    }
                                    customer.unread = thread.unread;
                                    customer.nonreply = thread.nonreply;

                                    _messageService.CreateMessage(m.business_id, m, false);
                                    _threadService.CreateThread(thread, false);
                                    _customerService.CreateCustomer(customer, false);
                                }
                                if (customer == null)
                                    _messageService.deleteMessageidIfNull(m.business_id, m.id).Wait();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            });
        }

        public void autoUpdateReferralsCusomterIdNull()
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                var referrals = _referralService.GetReferralsByCustomerIsNull(1000).Result;
                if (referrals == null) return;
                foreach (var r in referrals)
                {
                    var thread = _threadService.GetById(r.business_id, r.thread_id);
                    if (thread != null && !string.IsNullOrWhiteSpace(thread.customer_id))
                    {
                        r.customer_id = thread.customer_id;
                        _referralService.Create(r);
                    }
                }
            });
        }

        public void autoUpdateThreadsCusomterIdNull()
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                var page = new Paging();
                page.Limit = 1000;
                var threads = _threadService.GetThreadsWhereCustomerIsNull("", page).Result;
                if (threads == null) return;
                foreach (var t in threads)
                {
                    if (t != null && t.business_id != null)
                    {
                        var channel = _channelService.GetById(t.business_id, t.channel_id);

                        if (channel != null && t.sender_ext_id != channel.ext_id)
                        {
                            Customer customer = null;
                            var customers = _customerService.GetCustomersActiveThreadLikeThread(channel.business_id, t.id, page).Result;
                            if (customers != null && customers.Count == 1)
                            {
                                t.customer_id = customers[0].id;
                                _threadService.CreateThread(t, false);
                            }
                            else
                            {
                                customer = GetCustomer(channel, null, t.sender_ext_id, null, null);
                                if (customer == null)
                                {
                                    customer = GetCustomer(channel, null, t.owner_last_message_ext_id, null, null);
                                }
                                if (customer == null)
                                {
                                    customer = GetCustomer(channel, null, t.last_message_ext_id, null, null);
                                }
                                if (customer != null)
                                {
                                    t.customer_id = customer.id;
                                    if (string.IsNullOrWhiteSpace(customer.active_thread))
                                    {
                                        customer.active_thread = JsonConvert.SerializeObject(t);
                                    }
                                    customer.unread = t.unread;
                                    customer.nonreply = t.nonreply;

                                    _threadService.CreateThread(t, false);
                                    _customerService.CreateCustomer(customer, false);
                                }
                            }
                        }
                    }
                }
            });
        }

        //public void Test()
        //{
        //    var Customers = _customerService.GetCustomerActiveThread().Result;
        //    foreach (var customer in Customers)
        //    {
        //        Thread thread = null;
        //        thread = _threadService.GetByIdFromCustomerId(customer.id);
        //        if (thread != null)
        //        {
        //            if (customer.active_thread == null)
        //            {
        //                customer.active_thread = JsonConvert.SerializeObject(thread);
        //            }
        //            _customerService.CreateCustomer(customer, false);
        //        }
        //    }
        //}

        public async Task<ReplaceOneResult> UpsertAnyMongo<T>(T obj, UpdateOptions option, FilterDefinition<T> filter, string collectionName) where T : class
        {
            return await _conversationService.UpsertAnyMongo<T>(obj, option, filter, collectionName);
        }
        public async Task<List<T>> GetDataMongo<T>(string query, FindOptions<T> options, string collectionName) where T : class
        {
            return await _conversationService.GetDataMongo<T>(query, options, collectionName);
        }
    }
}