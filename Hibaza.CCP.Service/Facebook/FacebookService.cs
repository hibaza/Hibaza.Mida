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

namespace Hibaza.CCP.Service.Facebook
{
    public interface IFacebookService
    {
        void SaveWebhookData(string business_id, string channel_id, FacebookWebhookData data, bool real_time_update);
        void ImportDataNodes(string business_id, string channel_id, IEnumerable<Node> nodes, bool real_time_update);
        void SaveWebhookData(IEnumerable<FacebookWebhookData> list);
        string GetLonglivedToken(string clientId, string clientSecret, string token);
        bool SubscribeToAppWebhook(string pageId, string pageAccessToken);
        bool UnSubscribeToAppWebhook(string pageId, string pageAccessToken);
        Task<FacebookPost> GetPost(string postId, string pageAccessToken);
        Task<FacebookComment> GetComment(string commentId, string pageAccessToken);
        Task<FacebookWebsite> GetWebsite(string websiteId, string pageAccessToken);
        bool BlockUserFromPage(string pageId, string asid, string pageAccessToken);
        System.Threading.Tasks.Task<int> BatchImportPendingNodes(string business_id, string channel_id, string type, int max, int size, bool real_time_update);
        Task<bool> DeleteMessage(string business_id, string id, bool real_time_udpate);
        Task<bool> DeleteMessage(string business_id, string channel_id, string pageId, string message_ext_id, bool real_time_udpate);
        Task<FacebookShares> GetMessagesShares(string msgId, string pageAccessToken);


    }

    public class FacebookService : IFacebookService
    {
        private readonly IFacebookConversationService _facebookConversationService;
        private readonly IFacebookCommentService _facebookCommentService;
        private readonly IMessageService _messageService;
        private readonly IChannelService _channelService;
        private readonly ICustomerService _customerService;
        private readonly IThreadService _threadService;
        private readonly INodeService _nodeService;
        private readonly ILoggingService _logService;
        private readonly string _facebookGraphApiUrl = "https://graph.facebook.com/v2.10";


        public FacebookService(INodeService nodeService, ICustomerService customerService, IThreadService threadService, IChannelService channelService, IFacebookConversationService facebookConversationService, IFacebookCommentService facebookCommentService, IMessageService messageService, ILoggingService logService)
        {
            _facebookCommentService = facebookCommentService;
            _facebookConversationService = facebookConversationService;
            _messageService = messageService;
            _channelService = channelService;
            _customerService = customerService;
            _threadService = threadService;
            _nodeService = nodeService;
            _logService = logService;
        }


        public async Task<FacebookPost> GetPost(string postId, string pageAccessToken)
        {
            var url = string.Format(_facebookGraphApiUrl + "/{0}/?fields=id,parent_id,object_id,type,link,message,attachments,picture,permalink_url,source,from,created_time,updated_time&access_token={1}", postId, pageAccessToken);
            return await Core.Helpers.WebHelper.HttpGetAsync<FacebookPost>(url);
        }

        public async Task<FacebookShares> GetMessagesShares(string msgId, string pageAccessToken)
        {
            try
            {
                var url = _facebookGraphApiUrl + "/" + msgId + "?fields=shares{description,id,link,name}&access_token=" + pageAccessToken;
                return await Core.Helpers.WebHelper.HttpGetAsync<FacebookShares>(url);
            }
            catch (Exception ex) { return null; }
        }

        public async Task<FacebookComment> GetComment(string commentId, string pageAccessToken)
        {
            string url = string.Format(_facebookGraphApiUrl + "/{0}/?fields=object,message,attachment,created_time,permalink_url&access_token={1}", commentId, pageAccessToken);
            try
            {
                try
                {
                    var c = await Core.Helpers.WebHelper.HttpGetAsync<FacebookComment>(url);
                    return c;
                }
                catch
                {
                    try
                    {
                        url = string.Format(_facebookGraphApiUrl + "/{0}/?fields=message,created_time,permalink_url&access_token={1}", commentId, pageAccessToken);
                        var c = await Core.Helpers.WebHelper.HttpGetAsync<FacebookComment>(url);
                        return c;
                    }
                    catch {
                        url = string.Format(_facebookGraphApiUrl + "/{0}/?fields=message,created_time&access_token={1}", commentId, pageAccessToken);
                        var c = await Core.Helpers.WebHelper.HttpGetAsync<FacebookComment>(url);
                        return c;
                    }
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
                    name = string.Format("Get comments: {0}", commentId)
                });
                return null;
            }
        }

        public async Task<FacebookWebsite> GetWebsite(string websiteId, string pageAccessToken)
        {
            var url = string.Format(_facebookGraphApiUrl + "/{0}/?fields=id,title,site_name,image,application&access_token={1}", websiteId, pageAccessToken);
            return await Core.Helpers.WebHelper.HttpGetAsync<FacebookWebsite>(url);
        }

        public bool BlockUserFromPage(string pageId, string asid, string pageAccessToken)
        {
            var url = string.Format(_facebookGraphApiUrl + "/{0}/blocked?access_token={1}", pageId, pageAccessToken);
            try
            {
                List<string> list = new List<string>();
                list.Add(asid);
                return Core.Helpers.WebHelper.HttpPostAsync(url, list).Result;
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Facebook",
                    name = string.Format("Blocked user {0} from page: {1}", asid, pageId),
                    link = url
                });
            }
            return false;
        }

        public bool SubscribeToAppWebhook(string pageId, string pageAccessToken)
        {
            var url = string.Format(_facebookGraphApiUrl + "/{0}/subscribed_apps?access_token={1}", pageId, pageAccessToken);
            try
            {
                return Core.Helpers.WebHelper.HttpPostAsync(url, "").Result;
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Facebook",
                    name = string.Format("Subscribed page: {0} to app", pageId),
                    link = url
                });
                throw ex;
            }
        }

        public bool UnSubscribeToAppWebhook(string pageId, string pageAccessToken)
        {
            var url = string.Format(_facebookGraphApiUrl + "/{0}/subscribed_apps?access_token={1}", pageId, pageAccessToken);
            try
            {
                return Core.Helpers.WebHelper.HttpDeleteAsync(url).Result;
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Facebook",
                    name = string.Format("UnSubscribed page: {0} from app", pageId),
                    link = url
                });
            }
            return false;
        }

        public string GetLonglivedToken(string clientId, string clientSecret, string token)
        {
            var url = string.Format(_facebookGraphApiUrl + "/oauth/access_token?grant_type=fb_exchange_token&client_id={0}&client_secret={1}&fb_exchange_token={2}", clientId, clientSecret, token);
            string longLivedToken = "";
            try
            {
                longLivedToken = Core.Helpers.WebHelper.HttpGetAsync<dynamic>(url).Result.access_token;
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Facebook",
                    name = "Get long lived token",
                    link = url
                });
                throw ex;
            }
            return longLivedToken;
        }

        public void SaveWebhookData(IEnumerable<FacebookWebhookData> list)
        {
            foreach (var obj in list) SaveWebhookData("", "", obj, false);
        }

        public void ImportDataNodes(string business_id, string channel_id, IEnumerable<Node> nodes, bool real_time_update)
        {
            foreach (var node in nodes)
            {
                try
                {
                    FacebookWebhookData obj = JsonConvert.DeserializeObject<FacebookWebhookData>(node.data);
                    SaveWebhookData(business_id, channel_id, obj, real_time_update);
                    _nodeService.UpdateStatus(business_id, node.id, "done");
                }
                catch (Exception ex)
                {
                    _logService.Create(new Log { name = "Import data node: " + node.id, message = ex.Message + JsonConvert.SerializeObject(ex.StackTrace).ToString(), category = "Node", details = JsonConvert.SerializeObject(node).ToString() });
                }
            }

        }

        public async System.Threading.Tasks.Task<int> BatchImportPendingNodes(string business_id, string channel_id, string type, int max, int size, bool real_time_update)
        {

            List<Channel> list = new List<Channel>();
            if (!string.IsNullOrWhiteSpace(channel_id))
            {
                list.Add(_channelService.GetById(business_id, channel_id));
            }
            else
            {
                list = _channelService.GetChannels(business_id, 0, 2000).Result.Where(a => a.active).ToList();
            }
            int count = 0;
            var jobId = "";
            foreach (var bc in list)
            {
                var endAt = long.MaxValue;
                for (int i = 0; i < max; i++)
                {
                    count++;
                    var nodes = await _nodeService.GetPendingNodes(bc.business_id, bc.id, type, new Paging { Limit = size, Next = endAt.ToString() });
                    if (nodes != null && nodes.Count() > 0)
                    {
                        if (string.IsNullOrWhiteSpace(jobId))
                        {
                            //ImportDataNodes(bc.business_id, bc.id, nodes, real_time_update);
                            jobId = BackgroundJob.Enqueue<FacebookService>(x => x.ImportDataNodes(bc.business_id, bc.id, nodes, real_time_update));
                        }
                        else
                        {
                            jobId = BackgroundJob.ContinueWith<FacebookService>(jobId, x => x.ImportDataNodes(bc.business_id, bc.id, nodes, real_time_update));
                        }
                    }
                    endAt = nodes.Count() > 0 ? nodes.LastOrDefault().timestamp - 1 : 0;
                    if (endAt == 0) break;
                }
            }

            return count;
        }

        public async Task<bool> DeleteMessage(string business_id, string channel_id, string pageId, string message_ext_id, bool real_time_udpate)
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
            foreach (var c in list)
            {
                await DeleteMessage(c.business_id, MessageService.FormatId(c.business_id, message_ext_id), real_time_udpate);
            }
            return true;
        }

        public async Task<bool> DeleteMessage(string business_id, string id, bool real_time_update)
        {
            var message = _messageService.GetById(business_id, id);
            if (message != null && !message.deleted)
            {
                var channel = _channelService.GetById(business_id, message.channel_id);
                if (channel != null && channel.active)
                {
                    if (message.thread_type == "message")
                    {
                        await _facebookConversationService.DeleteMessage(message.ext_id, channel.token);
                    }
                    if (message.thread_type == "comment")
                    {
                        await _facebookCommentService.DeleteComment(message.ext_id, channel.token);
                    }
                    var ok = _messageService.MarkAsDeleted(business_id, id);
                    var thread = _threadService.RefreshThread(business_id, message.thread_id, true, true);
                    if (thread != null)
                    {
                        _customerService.RefreshCustomer(business_id, thread.customer_id);
                        if (real_time_update) BackgroundJob.Enqueue<CustomerService>(x => x.BatchUpdateUnreadCounters(business_id));
                    }
                    return ok;
                }
            }
            return false;
        }

        public void SaveWebhookData(string business_id, string channel_id, FacebookWebhookData data, bool real_time_update)
        {
            // Make sure this is a page subscription
            if (data.@object == "page")
            {
                // Iterate over each entry - there may be multiple if batched
                foreach (var entry in data.entry)
                {
                    var pageId = entry.id;
                    var timeOfEvent = entry.time;
                    try
                    {
                        // Iterate over each messaging event
                        if (entry.messaging != null)
                            foreach (var e in entry.messaging)
                            {
                                if (e.message != null)
                                {
                                    // _facebookConversationService.SaveWebhookMessaging(business_id, channel_id, pageId, e, real_time_update);
                                    //  BackgroundJob.Enqueue<FacebookConversationService>(x => x.SaveWebhookMessaging(business_id, channel_id, pageId, e, real_time_update));

                                    #region them truong hop neu la tin nhan tu cua hang, va click vao nut chat tren fb. lay ra xem tin nhan do lay duoc dang quan tam san pham nao
                                    //System.Threading.Tasks.Task.Factory.StartNew(() =>
                                    //{
                                    try
                                    {
                                        // neu khach hang nhan tin tu cua la cua hang
                                        if (e.message != null && e.message.attachments != null && e.message.attachments.Count > 0 && e.sender.id != entry.id)
                                        {
                                            var str = "";
                                            foreach (var item in e.message.attachments)
                                            {
                                                if (item.type == "fallback")
                                                {
                                                    var id = "m_" + e.message.mid;
                                                    var options = new FindOptions<FacebookShares>();
                                                    //options.Projection = "{_id: 0}";
                                                    options.Limit = 1;
                                                    var query = "{id:\"" + id + "\"}";
                                                    var rs = _facebookConversationService.GetDataMongo(query, options, "Posts").Result;
                                                    FacebookShares shares = null;
                                                    if (rs == null || rs.Count == 0)
                                                    {
                                                        List<Channel> channel = _channelService.GetChannelsByExtId(entry.id).Result;
                                                        shares = GetMessagesShares(id, channel[0].token).Result;
                                                        if (shares != null)
                                                        {
                                                            var option = new UpdateOptions { IsUpsert = true };
                                                            shares._id = id;
                                                            var filter = Builders<FacebookShares>.Filter.Where(x => x.id == id);
                                                            _facebookConversationService.UpsertAnyMongo<FacebookShares>(shares, option, filter, "Posts");
                                                        }
                                                    }
                                                    else shares = rs[0];

                                                    if (shares != null)
                                                    {
                                                        for (var i = 0; i < shares.shares.data.Count; i++)
                                                        {
                                                            // e.message.mid = shares.shares.data[i].id;
                                                            str += shares.shares.data[i].description + "\r\n_" + shares.shares.data[i].name + "\r\n_" + shares.shares.data[i].link;
                                                            // entry.time = entry.time + 1 + i;
                                                            // e.timestamp = e.timestamp + 1 + i;
                                                        }

                                                    }
                                                }
                                            }
                                            e.message.text = e.message.text + str;
                                            BackgroundJob.Enqueue<FacebookConversationService>(x => x.SaveWebhookMessaging(business_id, channel_id, pageId, e, real_time_update));
                                        }
                                        else
                                        {
                                            //_facebookConversationService.SaveWebhookMessaging(business_id, channel_id, pageId, e, real_time_update);
                                              BackgroundJob.Enqueue<FacebookConversationService>(x => x.SaveWebhookMessaging(business_id, channel_id, pageId, e, real_time_update));
                                        }
                                            

                                    }
                                    catch { }
                                    // });
                                    #endregion
                                }
                                if (e.postback != null && e.postback.referral != null || e.referral != null)
                                {
                                    BackgroundJob.Enqueue<FacebookConversationService>(x => x.SaveWebhookReferral(business_id, channel_id, pageId, e));
                                }
                                if (e.message == null && e.postback != null && e.postback.payload != null)
                                {
                                    BackgroundJob.Enqueue<FacebookConversationService>(x => x.SaveWebhookMessaging(business_id, channel_id, pageId, e, real_time_update));
                                }
                            }

                        if (entry.changes != null)
                        {
                            foreach (var e in entry.changes)
                                if (e.value != null)
                                {
                                    if (e.field == "feed")
                                    {
                                        if ((e.value.item == "comment" || e.value.item == "post") && (e.value.verb == "remove" || e.value.verb == "add" || e.value.verb == "edited"))
                                        {
                                            if (e.value.item == "comment" && e.value.verb == "remove")
                                            {
                                                BackgroundJob.Enqueue<FacebookService>(x => x.DeleteMessage(business_id, channel_id, pageId, e.value.comment_id, true));
                                            }
                                            else
                                            {
                                                BackgroundJob.Enqueue<FacebookCommentService>(x => x.SaveWebhookComment(business_id, channel_id, pageId, e.value, true));
                                                //// neu comment vao quang cao dong . anh se an . nhung lay qua link
                                                //if (e.value.item == "comment" && e.value.verb == "add" && e.value.sender_id != entry.id)
                                                //{
                                                //    #region them truong hop neu la comment tư cửa hàng. lay ra comment do lay duoc dang quan tam san pham nao
                                                //    try
                                                //    {
                                                //        List<Channel> channel = _channelService.GetChannelsByExtId(entry.id).Result;
                                                //        var comment = GetComment(e.value.comment_id, channel[0].token).Result;

                                                //        e.value.message = comment.message + "\r\n - " + comment.permalink_url;

                                                //        //_facebookCommentService.SaveWebhookComment(business_id, channel_id, pageId, e.value, false);
                                                //        BackgroundJob.Enqueue<FacebookCommentService>(x => x.SaveWebhookComment(business_id, channel_id, pageId, e.value, real_time_update));
                                                //    }
                                                //    catch { }
                                                //    #endregion
                                                //}
                                                //else
                                                //{
                                                // _facebookCommentService.SaveWebhookComment(business_id, channel_id, pageId, e.value, false);
                                                //  BackgroundJob.Enqueue<FacebookCommentService>(x => x.SaveWebhookComment(business_id, channel_id, pageId, e.value, true));
                                                //}
                                                // else
                                                //{
                                                //    if (e.value.item == "comment" && e.value.verb == "hide" && e.value.sender_id != entry.id)
                                                //    {
                                                //        #region them truong hop neu la comment tư cửa hàng. lay ra comment do lay duoc dang quan tam san pham nao
                                                //        try
                                                //        {
                                                //            List<Channel> channel = _channelService.GetChannelsByExtId(entry.id).Result;
                                                //            var options1 = new FindOptions<FacebookShares>();
                                                //            //options.Projection = "{_id: 0}";
                                                //            options1.Limit = 10;
                                                //            var comment = GetComment(e.value.comment_id, channel[0].token).Result;
                                                //            var sp = comment.permalink_url.Split('?');
                                                //            var query1 = "{\"shares.data\": { $elemMatch :  { link : {$regex:\"" + sp[0] + "\" }}  }  }";
                                                //            var rs1 = _facebookConversationService.GetDataMongo(query1, options1, "Posts").Result;
                                                //            var list = new List<FacebookShares>();

                                                //            var newMess = e.value.message;
                                                //            foreach (var d in rs1)
                                                //            {
                                                //                for (var i = 0; i < d.shares.data.Count; i++)
                                                //                {
                                                //                    //e.value.comment_id = e.value.comment_id + i;
                                                //                    newMess += "\r\n" + d.shares.data[i].description + "\r\n" + d.shares.data[i].name + "\r\n" + d.shares.data[i].link;
                                                //                    //e.value.created_time = e.value.created_time + 3;
                                                //                }
                                                //            }
                                                //        //_facebookCommentService.SaveWebhookComment(business_id, channel_id, pageId, e.value, true);
                                                //            BackgroundJob.Enqueue<FacebookCommentService>(x => x.SaveWebhookComment(business_id, channel_id, pageId, e.value, true));
                                                //        }
                                                //        catch { }
                                                //        #endregion
                                                //    }
                                                //    else
                                                //    {
                                                //        //_facebookCommentService.SaveWebhookComment(business_id, channel_id, pageId, e.value, real_time_update);
                                                //        BackgroundJob.Enqueue<FacebookCommentService>(x => x.SaveWebhookComment(business_id, channel_id, pageId, e.value, real_time_update));
                                                //    }
                                                //}
                                            }
                                            //var a = _facebookCommentService.SaveWebhookComment(business_id,channel_id, pageId, e.value, real_time_update);
                                        }
                                    }
                                    if (e.field == "conversations")
                                    {
                                        BackgroundJob.Enqueue<FacebookConversationService>(x => x.DownloadSingleConversation(business_id, channel_id, e.value.page_id, e.value.thread_id, 100, true));
                                    }
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logService.Create(new Log { name = "Webhook calledback 2", message = ex.Message + JsonConvert.SerializeObject(ex.StackTrace).ToString(), details = JsonConvert.SerializeObject(data).ToString() });
                        throw ex;
                    }
                }
            }
        }

    }
}