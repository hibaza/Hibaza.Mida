using Hangfire;
using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using Hibaza.CCP.Domain.Models.Facebook;
using Hibaza.CCP.Service.SQL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service.Facebook
{
    public interface IFacebookCommentService
    {
        System.Threading.Tasks.Task SaveWebhookComment(string business_id, string channel_id, string pageId, FacebookChangesValue e, bool real_time_update);
        Task<FacebookCommentPostResponse> PostComment(string object_id, FacebookCommentPost data, string accessToken);
        Task<FacebookCommentPostResponse> PostPhoto(string pageId, FacebookPhotoPost data, string accessToken);
        Task<FacebookCommentPostResponse> PostPrivateReply(string comment_id, FacebookCommentPost data, string accessToken);
        System.Threading.Tasks.Task<bool> ShowHideComment(string commentId, bool is_hidden, string accessToken);
        System.Threading.Tasks.Task<bool> LikeComment(string commentId, string accessToken);
        System.Threading.Tasks.Task<bool> UnLikeComment(string commentId, string accessToken);
        System.Threading.Tasks.Task<bool> DeleteComment(string commentId, string accessToken);
        string GetPostLink(string postId, string accessToken);
        System.Threading.Tasks.Task<int> DownloadPosts(Channel bc, string endPoint, string url, string job_id, DateTime from, DateTime to, bool real_time_update);
        Task<int> SaveComments(Channel bc, string rootId, string objectId, string url, string job_id, int limit, bool real_time_update);
        System.Threading.Tasks.Task<int> DownloadSinglePost(string pageId, string postId, int limit, bool real_time_update);
        Task<FacebookComment> GetComment(string commentId, string pageAccessToken);
        System.Threading.Tasks.Task<int> DownloadComments(Channel bc, string jobId, int limit, long since, long until, bool real_time_update);
    }

    public class FacebookCommentService : IFacebookCommentService
    {
        private readonly ILinkService _linkService;
        private readonly INodeService _nodeService;
        private readonly IConversationService _conversationService;
        private readonly IMessageService _messageService;
        private readonly ICustomerService _customerService;
        private readonly IChannelService _channelService;
        private readonly IThreadService _threadService;
        private readonly ILoggingService _logService;
        private readonly IFacebookConversationService _facebookConvesrationService;
        private readonly string _facebookGraphApiUrl = "https://graph.facebook.com/v2.10";
        private readonly IBusinessService _businessService;

        public FacebookCommentService(ICustomerService customerService, INodeService nodeService, IThreadService threadService, IFacebookConversationService facebookConversationService, IChannelService channelService, ILinkService linkService, IConversationService conversationService, IMessageService messageService, ILoggingService logService,IBusinessService businessService)
        {
            _linkService = linkService;
            _nodeService = nodeService;
            _conversationService = conversationService;
            _messageService = messageService;
            _customerService = customerService;
            _channelService = channelService;
            _threadService = threadService;
            _logService = logService;
            _facebookConvesrationService = facebookConversationService;
            _businessService = businessService;
        }

        public async Task<FacebookComment> GetComment(string commentId, string pageAccessToken)
        {
            string url = string.Format(_facebookGraphApiUrl + "/{0}/?fields=object,message,attachment,from,created_time&access_token={1}", commentId, pageAccessToken);
            try
            {
                try
                {
                    var c = await Core.Helpers.WebHelper.HttpGetAsync<FacebookComment>(url);
                    return c;
                }
                catch
                {
                    url = string.Format(_facebookGraphApiUrl + "/{0}/?fields=message,created_time&access_token={1}", commentId, pageAccessToken);
                    var c = await Core.Helpers.WebHelper.HttpGetAsync<FacebookComment>(url);
                    return c;
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

        public async System.Threading.Tasks.Task SaveWebhookComment(string business_id, string channel_id, string pageId, FacebookChangesValue e, bool real_time_update)
        {
            //if (e.sender_id == pageId && e.post_id == e.parent_id) return;

            string owner_ext_id = "";
            string owner_app_id = "";
            if (e.sender_id != pageId)
            {
                owner_app_id = e.sender_id;
            }

            e.post_id = e.post_id ?? "";
            e.post_id = e.post_id.Contains("_") ? e.post_id : pageId + "_" + e.post_id;
            var fm = new FacebookMessageObject
            {
                msgId = string.IsNullOrWhiteSpace(e.comment_id) ? e.post_id : e.comment_id,
                msgParentId = e.parent_id,
                msgRootId = e.post_id,
                conversationId = "",
                senderId = e.sender_id,
                sender_name = e.sender_name,
                recipientId = "",
                recipient_name = "",
                timestamp = e.created_time > 9999999999 ? e.created_time / 1000 : e.created_time,
                text = e.message,
                type = string.IsNullOrWhiteSpace(e.photo) ? "text" : "image",
                thread_type = "comment",
                channel_type = "facebook",
                liked = true,
                hidden = true,
                url = e.photo,
                urls = new List<string>()
            };
            int count = 0;

            List<Channel> list = new List<Channel>();
            if (!string.IsNullOrWhiteSpace(channel_id) && !string.IsNullOrWhiteSpace(business_id))
            {
                list.Add(_channelService.GetById(business_id, channel_id));
            }
            else
            {
                list = _channelService.GetChannelsByExtId(pageId).Result.Where(b => b.active).ToList();
            }

            Customer customer = null;
            Thread thread = null;

            foreach (var bc in list)
            {
                business_id = bc.business_id;
                count++;

                if (string.IsNullOrWhiteSpace(fm.text) && string.IsNullOrWhiteSpace(fm.url))
                {
                    var cm = await GetComment(e.comment_id, bc.token);
                    if (cm == null)
                    {
                        try
                        {
                            var cid = string.Format("{0}_{1}", e.post_id.Split('_').Last(), e.comment_id.Split('_').Last());
                            cm = await GetComment(cid, bc.token);
                            if (cm != null)
                            {
                                e.comment_id = cid;
                                fm.msgId = cid;
                            }
                        }
                        catch { }
                    }
                    fm.text = cm == null ? null : cm.message;
                    if (cm != null && cm.attachment != null && cm.attachment.media != null && cm.attachment.media.image != null)
                    {
                        fm.url = (cm.attachment.media.image.src ?? cm.attachment.media.image.url);
                        fm.type = string.IsNullOrWhiteSpace(fm.url) ? "text" : "image";
                    }
                }

                if (!string.IsNullOrWhiteSpace(e.comment_id) && real_time_update)
                {
                    var business = _businessService.GetById(business_id);
                    try
                    {
                       
                        await ShowHideComment(e.comment_id, business.auto_hide, bc.token);
                    }
                    catch
                    {
                    }
                    try
                    {
                        if(business.auto_like)
                        await LikeComment(e.comment_id, bc.token);
                    }
                    catch
                    {
                    }
                }

                if (string.IsNullOrWhiteSpace(owner_app_id))
                {
                    Message msg = _messageService.GetById(business_id, MessageService.FormatId(business_id, fm.msgParentId));
                    if (msg == null)
                    {
                        if (fm.msgParentId.IndexOf(business_id) < 0)
                        {
                            msg = _messageService.GetById(business_id, MessageService.FormatId01(business_id, fm.msgParentId));
                        }
                    }
                    if (msg != null)
                        owner_app_id = msg.sender_ext_id == bc.ext_id ? null : msg.sender_ext_id;
                    else
                    {
                        var m = GetComment(fm.msgParentId, bc.token).Result;
                        if (m != null)
                        {
                            owner_app_id = m.from == null || m.from.id == bc.ext_id ? null : m.from.id;
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(owner_app_id)) continue;


                fm.conversationId = owner_app_id + "_" + fm.msgRootId;

                channel_id = bc.id;
                var thread_id = ThreadService.FormatId(business_id, fm.conversationId);
                var message_id = MessageService.FormatId(business_id, fm.msgId);

                var owner_id = CustomerService.FormatId(business_id, owner_app_id);

                if (thread == null)
                {
                    thread = _threadService.GetById(business_id, thread_id);
                    if (thread == null)
                    {
                        if (thread_id.IndexOf(business_id) < 0)
                        {
                            thread_id = ThreadService.FormatId01(business_id, fm.conversationId);
                            if (owner_app_id.IndexOf(business_id) < 0)
                                owner_id = CustomerService.FormatId01(business_id, owner_app_id);
                            if (fm.msgId.IndexOf(business_id) < 0)
                                message_id = MessageService.FormatId01(business_id, fm.msgId);
                            thread = _threadService.GetById(business_id, thread_id);
                        }
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
                    customer = _facebookConvesrationService.GetCustomer(bc, fm.customer_id, fm.msgId, owner_ext_id, owner_app_id);
                    if (customer != null && string.IsNullOrWhiteSpace(fm.sender_name)) fm.sender_name = customer.name;
                }
                if (customer != null) fm.customer_id = customer.id;

                bool is_new_message = false;
                string tid = "";
                var message = _facebookConvesrationService.CreateMessage(bc, fm, message_id, "", owner_id, real_time_update, out is_new_message, out tid);
                if (message != null)
                {
                    await _facebookConvesrationService.CreateThread(bc, message, null, customer, owner_id, owner_ext_id, owner_app_id, real_time_update);
                    if (real_time_update) BackgroundJob.Enqueue<CustomerService>(x => x.BatchUpdateUnreadCounters(business_id));
                }

            }
        }

        public async Task<FacebookCommentPostResponse> PostPhoto(string pageId, FacebookPhotoPost data, string accessToken)
        {
            try
            {
                var url = _facebookGraphApiUrl + "/" + pageId + "/photos?access_token=" + accessToken;
                var r = await Core.Helpers.WebHelper.HttpPostAsync<FacebookCommentPostResponse>(url, data);
                return r;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<FacebookCommentPostResponse> PostComment(string object_id, FacebookCommentPost data, string accessToken)
        {
            try
            {
                var url = _facebookGraphApiUrl + "/" + object_id + "/comments?access_token=" + accessToken;
                var r = await Core.Helpers.WebHelper.HttpPostAsync<FacebookCommentPostResponse>(url, data);
                return r;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<FacebookCommentPostResponse> PostPrivateReply(string comment_id, FacebookCommentPost data, string accessToken)
        {
            try
            {
                var url = _facebookGraphApiUrl + "/" + comment_id + "/private_replies?access_token=" + accessToken;
                var r = await Core.Helpers.WebHelper.HttpPostAsync<FacebookCommentPostResponse>(url, data);
                return r;
            }
            catch
            {
                return null;
            };
        }

        public string GetPostLink(string postId, string accessToken)
        {

            var url = _facebookGraphApiUrl + "/" + postId + "?fields=permalink_url&access_token=" + accessToken;
            var dataFb = Core.Helpers.WebHelper.HttpGetAsync<FacebookPostLink>(url).Result;
            if (dataFb != null) return dataFb.permalink_url;
            return null;
        }


        public async System.Threading.Tasks.Task<bool> ShowHideComment(string commentId, bool is_hidden, string accessToken)
        {
            try
            {
                //var url = _facebookGraphApiUrl + "/" + commentId + "/?is_hidden=" + (is_hidden ? "true" : "false") + "&access_token=" + accessToken;
                var url = _facebookGraphApiUrl + "/" + commentId + "/?is_hidden=" + (is_hidden ? "true" : "false") + "&access_token=" + accessToken;
                var r = await Core.Helpers.WebHelper.HttpPostAsync<dynamic>(url, null);
                return r.success;
            }
            catch (Exception ex)
            {
                return false;
            };
        }

        public async System.Threading.Tasks.Task<bool> UnLikeComment(string commentId, string accessToken)
        {
            try
            {
                var url = _facebookGraphApiUrl + "/" + commentId + "/likes?access_token=" + accessToken;
                return await Core.Helpers.WebHelper.HttpDeleteAsync(url);
            }
            catch (Exception ex)
            {
                return false;
            };
        }

        public async System.Threading.Tasks.Task<bool> LikeComment(string commentId, string accessToken)
        {
            try
            {
                var url = _facebookGraphApiUrl + "/" + commentId + "/likes?access_token=" + accessToken;
                var r = await Core.Helpers.WebHelper.HttpPostAsync<dynamic>(url, null);
                return r.success;
            }
            catch (Exception ex)
            {
                return false;
            };
        }

        public async System.Threading.Tasks.Task<bool> DeleteComment(string comment_id, string accessToken)
        {
            try
            {
                var url = _facebookGraphApiUrl + "/" + comment_id + "?access_token=" + accessToken;
                return await Core.Helpers.WebHelper.HttpDeleteAsync(url);
            }
            catch (Exception ex)
            {
                return false;
            };
        }

        public async System.Threading.Tasks.Task<int> DownloadPosts(Channel bc, string endPoint, string url, string job_id, DateTime from, DateTime to, bool real_time_update)
        {
            var total = 0;
            string pageId = bc.ext_id;
            try
            {
                string business_id = bc.business_id;
                long until = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(to);
                long since = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(from);
                url = string.IsNullOrWhiteSpace(url) ? _facebookGraphApiUrl + "/" + pageId + "/" + endPoint + "/?fields=id,permalink_url,message,type,from,created_time&limit=100&include_hidden=true&until" + until + "&since=" + since + "&access_token=" + bc.token : url;
                var dataFb = await Core.Helpers.WebHelper.HttpGetAsync<FacebookFeed>(url);
                if (dataFb == null || dataFb.data == null) return total;
                foreach (var post in dataFb.data)
                {
                    DateTime now = DateTime.UtcNow;
                    _linkService.Insert(new Domain.Entities.Link { business_id = business_id, id = post.id, url = post.permalink_url, author_id = post.from.id, author_name = post.from.name, message = post.message, channel_ext_id = pageId, created_time = post.created_time, timestamp = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(post.created_time), channel_id = bc.id, objectId = endPoint, type = "facebook." + (string.IsNullOrWhiteSpace(post.type) ? "post" : post.type) });
                }

                if (dataFb.paging != null && !string.IsNullOrWhiteSpace(dataFb.paging.Next))
                {
                    var nextUrl = dataFb.paging.Next;
                    //total += await DownloadPosts(bc, endPoint, nextUrl, "", from, to, real_time_update);
                    BackgroundJob.Enqueue<FacebookCommentService>(x => x.DownloadPosts(bc, endPoint, nextUrl, "", from, to, real_time_update));
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
                    name = string.Format("Get facebook posts for pageId: {0}", pageId)
                });
            }
            return total;
        }


        public async System.Threading.Tasks.Task<int> DownloadComments(Channel bc, IEnumerable<Link> links, int limit, bool real_time_update)
        {
            int count = 0;
            foreach (var post in links)
                if (post.status != "processing")
                {
                    count++;
                    _linkService.UpdateStatus(post.business_id, post.id, "processing");
                    await SaveComments(bc, post.id, post.id, "", "", limit, real_time_update);

                    //BackgroundJob.Enqueue<FacebookCommentService>(x => x.SaveComments(bc, post.id, post.id, "", "", limit, real_time_update));
                    await SaveAttachmentComments(bc, post.id, "", limit, real_time_update);
                    //BackgroundJob.Enqueue<FacebookCommentService>(x => x.SaveAttachmentComments(bc, post.id, "", limit, real_time_update));

                }
            return count;
        }

        public async System.Threading.Tasks.Task<int> DownloadComments(Channel bc, string jobId, int limit, long since, long until, bool real_time_update)
        {
            var total = 0;
            string pageId = bc.ext_id;
            try
            {
                var next = until;

                while (next >= since)
                {
                    Paging page = new Paging { Limit = limit, Previous = since.ToString(), Next = next.ToString() };
                    IEnumerable<Link> links = await _linkService.GetLinks(bc.business_id, bc.id, page);


                    if (string.IsNullOrWhiteSpace(jobId))
                    {
                        jobId = BackgroundJob.Enqueue<FacebookCommentService>(x => x.DownloadComments(bc, links, limit, real_time_update));
                    }
                    else
                    {
                        jobId = BackgroundJob.ContinueWith<FacebookCommentService>(jobId, x => x.DownloadComments(bc, links, limit, real_time_update));
                    }


                    if (links != null && links.Count() > 0)
                    {
                        next = links.Last().timestamp - 1;
                        ////total += await DownloadComments(bc, limit, since, next, real_time_update);

                        if (string.IsNullOrWhiteSpace(jobId))
                        {
                            jobId = BackgroundJob.Enqueue<FacebookCommentService>(x => x.DownloadComments(bc, "", limit, since, next, real_time_update));
                        }
                        else
                        {
                            jobId = BackgroundJob.ContinueWith<FacebookCommentService>(jobId, x => x.DownloadComments(bc, jobId, limit, since, next, real_time_update));
                        }
                    }
                    else
                    {
                        next = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Link",
                    details = JsonConvert.SerializeObject(ex.StackTrace),
                    name = string.Format("downoad comments for pageId: {0}", pageId)
                });
            }
            return total;
        }

        public async Task<int> SaveAttachmentComments(Channel bc, string objectId, string url, int limit, bool real_time_update)
        {
            var count = 0;
            try
            {
                url = string.IsNullOrWhiteSpace(url) ? _facebookGraphApiUrl + "/" + objectId + "/attachments?fields=url,target,type,description,subattachments{url,target,type,description}&limit=100&access_token=" + bc.token : url;
                var dataFb = await Core.Helpers.WebHelper.HttpGetAsync<FacebookAttachmentFeed>(url);
                if (dataFb == null || dataFb.data == null) return count;
                foreach (var att in dataFb.data)
                    if (att.target != null)
                    {
                        if (!string.IsNullOrWhiteSpace(att.target.id))
                        {
                            var attId = att.target.id.Contains('_') ? att.target.id : string.Format("{0}_{1}", bc.ext_id, att.target.id);
                            count += await SaveComments(bc, attId, attId, "", "", limit, real_time_update);
                            //BackgroundJob.Enqueue<FacebookCommentService>(x => x.SaveComments(bc, attId, attId, "", "", limit, real_time_update));
                        }
                        if (att.subattachments != null && att.subattachments.data != null)
                        {
                            foreach (var s in att.subattachments.data)
                                if (s.target != null && !string.IsNullOrWhiteSpace(s.target.id))
                                {
                                    var attId1 = s.target.id.Contains('_') ? s.target.id : string.Format("{0}_{1}", bc.ext_id, s.target.id);
                                    count += await SaveComments(bc, attId1, attId1, "", "", limit, real_time_update);
                                    //BackgroundJob.Enqueue<FacebookCommentService>(x => x.SaveComments(bc, attId1, attId1, "", "", limit, real_time_update));
                                }
                        }
                    }


                if (dataFb.paging != null && !string.IsNullOrWhiteSpace(dataFb.paging.Next))
                {
                    var nextUrl = dataFb.paging.Next.ToString();
                    await SaveAttachmentComments(bc, objectId, nextUrl, limit, real_time_update);
                }
                return count;
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Attachment",
                    link = url,
                    details = JsonConvert.SerializeObject(ex.StackTrace),
                    name = string.Format("downoad attachment for objectId: {0}", objectId)
                });

            }

            return count;
        }

        //public async Task<int> GetFacebookComments(Channel bc, string rootId, string objectId, string url, string job_id, bool real_time_update)
        //{
        //    string pageId = bc.ext_id;
        //    string business_id = bc.business_id;
        //    var channel_id = bc.id;
        //    int count = 0;
        //    url = string.IsNullOrWhiteSpace(url) ? _facebookGraphApiUrl + "/" + objectId + "/comments?fields=comments.order(reverse_chronological),id,message,parent,object,from,created_time&limit=100&access_token=" + bc.token : url;
        //    var comments = await Core.Helpers.WebHelper.HttpGetAsync<FacebookCommentFeed>(url);
        //    if (comments == null || comments.data == null || comments.data.Count() == 0)
        //    {
        //        _linkService.UpdateStatus(business_id, rootId, "succeeded");
        //        return count;
        //    }

        //    foreach (var msg in comments.data)
        //    {
        //        count++;
        //        string owner_ext_id = "";
        //        string owner_app_id = "";
        //        if (msg.from.id != pageId)
        //        {
        //            owner_app_id = msg.from.id;
        //        }
        //        var fm = new FacebookMessageObject
        //        {
        //            msgId = msg.id,
        //            msgParentId = objectId,
        //            msgRootId = msg.@object != null ? msg.@object.id : rootId,
        //            conversationId = "",
        //            senderId = msg.from.id,
        //            sender_name = msg.from.name,
        //            recipientId = "",
        //            recipient_name = "",
        //            timestamp = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(msg.created_time.AddHours(-7)),
        //            text = msg.message,
        //            type = "text",
        //            thread_type = "comment",
        //            channel_type = bc.type,
        //            urls = new List<string>()
        //        };


        //        if (msg.attachment != null)
        //        {
        //            if (msg.attachment.media != null && msg.attachment.media.image != null && !string.IsNullOrWhiteSpace(msg.attachment.media.image.url))
        //            {
        //                fm.urls.Add(msg.attachment.media.image.url);
        //                fm.type = msg.attachment.type;
        //            }
        //            fm.url = fm.urls.FirstOrDefault();
        //            fm.type = fm.urls.Count > 1 ? "multiple_images" : fm.urls.Count == 1 ? "image" : fm.type;
        //        }

        //        if (string.IsNullOrWhiteSpace(owner_app_id))
        //        {
        //            Message m = _messageService.GetById(business_id, MessageService.FormatId(business_id, fm.msgParentId));
        //            if (m != null)
        //                owner_app_id = m.sender_ext_id == bc.ext_id ? "" : m.sender_ext_id;
        //        }

        //        if (string.IsNullOrWhiteSpace(owner_app_id)) continue;

        //        fm.conversationId = owner_app_id + "_" + fm.msgRootId;

        //        var thread_id = ThreadService.FormatId(business_id, fm.conversationId);
        //        var message_id = MessageService.FormatId(business_id, fm.msgId);

        //        var owner_id = CustomerService.FormatId(business_id, owner_app_id);
        //        bool is_new_message = false;
        //        string tid = "";
        //        var message = _facebookConvesrationService.CreateMessage(bc, fm, message_id, "", owner_id, true, out is_new_message, out tid);
        //        if (message != null)
        //        {
        //            await _facebookConvesrationService.CreateThread(bc, message, owner_id, owner_ext_id, owner_app_id, true);

        //        }
        //        if (msg.comments != null && msg.comments.data != null)
        //        {
        //            //await GetFacebookComments(bc, rootId, msg.id, "", "", real_time_update);
        //            BackgroundJob.Enqueue<FacebookCommentService>(x => x.GetFacebookComments1(bc, rootId, msg.id, "", "", real_time_update));
        //        }
        //    }


        //    if (comments.paging != null && !string.IsNullOrWhiteSpace(comments.paging.Next))
        //    {
        //        var nextUrl = comments.paging.Next;
        //        //await GetFacebookComments(bc, rootId, objectId, nextUrl, job_id, real_time_update);
        //        BackgroundJob.Enqueue<FacebookCommentService>(x => x.GetFacebookComments1(bc, rootId, objectId, nextUrl, job_id, real_time_update));
        //    }

        //    return count;
        //}

        private async Task<int> SaveComments(Channel bc, IEnumerable<FacebookComment> data, string rootId, string objectId, string url, bool real_time_update)
        {
            int count = 0;

            string pageId = bc.ext_id;
            if (data == null || data.Count() == 0)
            {
                return count;
            }

            foreach (var msg in data)
            {
                try
                {
                    count++;
                    if (count == 1)
                    {
                        _linkService.UpdatTimestamp(bc.business_id, rootId, Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(msg.created_time));
                    }
                    var message_id = MessageService.FormatId(bc.business_id, msg.id);
                    var message = _messageService.GetById(bc.business_id, message_id);

                    if (!(message == null || (string.IsNullOrWhiteSpace(message.message) && string.IsNullOrWhiteSpace(message.url)))) continue;

                    DateTime now = DateTime.UtcNow;
                    Node node = new Node
                    {
                        id = msg.id,
                        business_id = bc.business_id,
                        channel_id = bc.id,
                        type = bc.type + ".comment",
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
                        changes = new List<FacebookChangesEvent>()
                    };

                    FacebookChangesEvent item = new FacebookChangesEvent
                    {
                        field = "feed",
                        value = new FacebookChangesValue
                        {
                            item = "comment",
                            comment_id = msg.id,
                            post_id = msg.@object != null ? msg.@object.id : rootId,
                            parent_id = msg.parent != null ? msg.parent.id : objectId,
                            page_id = pageId,
                            sender_id = msg.from.id,
                            sender_name = msg.from.name,
                            verb = "add",
                            created_time = entry.time,
                            message = msg.message
                        }
                    };

                    string type = "text";
                    if (msg.attachment != null)
                    {
                        if (msg.attachment.media != null && msg.attachment.media.image != null && !string.IsNullOrWhiteSpace(msg.attachment.media.image.url))
                        {
                            item.value.photo = msg.attachment.media.image.url;
                            type = msg.attachment.type;
                        }
                    }

                    entry.changes.Add(item);
                    obj.entry.Add(entry);
                    node.data = JsonConvert.SerializeObject(obj);
                    _nodeService.CreateNode(node);

                    if (msg.comments != null && msg.comments.data != null)
                    {
                        await SaveComments(bc, msg.comments.data, rootId, msg.id, url, real_time_update);
                        //BackgroundJob.Enqueue<FacebookCommentService>(x => x.SaveComments(bc, rootId, msg.id, "", "", 100, real_time_update));
                    }
                }
                catch (Exception ex)
                {
                    _logService.Create(new Log
                    {
                        message = ex.Message,
                        category = "Comment",
                        link = url,
                        details = JsonConvert.SerializeObject(ex.StackTrace),
                        name = string.Format("downoad comments for objectId: {0}", objectId)
                    });
                }
            }

            return count;
        }


        public async Task<int> SaveComments(Channel bc, string rootId, string objectId, string url, string job_id, int limit, bool real_time_update)
        {
            int count = 0;
            //if (objectId == "1501957926758549_1863624617258543")
            //{
            //    var x = 1;
            //}
            try
            {
                string pageId = bc.ext_id;
                url = string.IsNullOrWhiteSpace(url) ? _facebookGraphApiUrl + "/" + objectId + "/comments?fields=comments.order(reverse_chronological),id,message,parent,object,from,created_time&order=reverse_chronological&limit=" + limit + "&access_token=" + bc.token : url;
                var comments = await Core.Helpers.WebHelper.HttpGetAsync<FacebookCommentFeed>(url);


                if (comments == null || comments.data == null || comments.data.Count() == 0)
                {
                    return count;
                }

                count += await SaveComments(bc, comments.data, rootId, objectId, url, real_time_update);


                if (comments.paging != null && !string.IsNullOrWhiteSpace(comments.paging.Next) && limit > comments.data.Count())
                {
                    var nextUrl = comments.paging.Next;
                    await SaveComments(bc, rootId, objectId, nextUrl, job_id, limit - count, real_time_update);
                    //BackgroundJob.Enqueue<FacebookCommentService>(x => x.SaveComments(bc, rootId, objectId, nextUrl, job_id, limit - comments.data.Count(), real_time_update));
                }
                else
                {
                    _linkService.UpdateStatus(bc.business_id, rootId, "succeeded");
                }
                return count;
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Comment",
                    link = url,
                    details = JsonConvert.SerializeObject(ex.StackTrace),
                    name = string.Format("downoad comments for objectId: {0}", objectId)
                });
            }
            return count;
        }

        public async System.Threading.Tasks.Task<int> DownloadSinglePost(string pageId, string postId, int limit, bool real_time_update)
        {
            int count = 0;
            foreach (var bc in _channelService.GetChannelsByExtId(pageId).Result)
            {
                await SaveComments(bc, postId, postId, "", "", limit, real_time_update);
                count++;
            }
            return count;
        }

    }
}
