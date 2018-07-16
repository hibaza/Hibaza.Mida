using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Data.Repositories;
using Hibaza.CCP.Service;
using Hibaza.CCP.Service.Facebook;
using Hibaza.CCP.Core;
using Microsoft.Extensions.Options;
using Hibaza.CCP.Domain.Models;
using Hibaza.CCP.Domain.Models.Facebook;
using Firebase.Storage;
using Microsoft.Net.Http.Headers;
using System.IO;
using Newtonsoft.Json;
using Hangfire;
using Hibaza.CCP.Data.Repositories.Firebase;
using Hibaza.CCP.Data.Providers.Firebase;
using Hibaza.CCP.Service.SQL;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Collections;

namespace Hibaza.CCP.Api.Controllers
{

    public class MessageFilter
    {
        public int first { get; set; }
        public int quantity { get; set; }
        public string search { get; set; }
        public string channel_id { get; set; }
    }

    [Route("messages")]
    public class MessageController : Controller
    {
        private readonly IMessageService _messageService;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ILoggingService _logService;
        private readonly IFacebookConversationService _facebookConversationService;
        private readonly IFacebookCommentService _facebookCommentService;
        private readonly IFacebookService _facebookService;
        private readonly IChannelService _channelService;
        private readonly IAgentService _agentService;
        private readonly IBusinessService _businessService;
        private readonly IThreadService _threadService;
        private readonly ICustomerService _customerService;
        private readonly IConversationService _conversationService;
        private readonly ITicketService _ticketService;


        public MessageController(IBusinessService businessService, IFacebookService facebookService, ICustomerService customerService, ITicketService ticketService, IFacebookCommentService facebookCommentService, IConversationService conversationService, IThreadService threadService, IAgentService agentService, IMessageService messageService, IFacebookConversationService facebookConversationService, IChannelService channelService, IOptions<AppSettings> appSettings, ILoggingService logService)
        {
            _messageService = messageService;
            _appSettings = appSettings;
            _channelService = channelService;
            _facebookConversationService = facebookConversationService;
            _agentService = agentService;
            _businessService = businessService;
            _threadService = threadService;
            _customerService = customerService;
            _conversationService = conversationService;
            _facebookCommentService = facebookCommentService;
            _facebookService = facebookService;
            _ticketService = ticketService;
            _logService = logService;
        }

        [HttpGet("fb_reset")]
        public void DeleteFirebaseMessages([FromQuery]string access_token)
        {
            if (access_token != "@bazavietnam") return;
            var fb = new FirebaseMessageRepository(new FirebaseFactory(_appSettings));
            fb.DeleteFolder();
        }

        [HttpGet("copyto/{business_id}")]
        public int CopToSQL(string business_id, [FromQuery]string access_token, [FromQuery]int offset, [FromQuery]int limit)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;
            var fb = new FirebaseMessageRepository(new FirebaseFactory(_appSettings));
            var jobId = "";
            foreach (var t in _threadService.GetThreads(business_id, "", "", "", "", "", new Paging { Limit = 1000000 }).Result)
            {
                count++;
                jobId = BackgroundJob.Enqueue<MessageService>(x => x.CopyFromRealtimeDB(business_id, t.id, new Paging { Limit = limit }));

            }
            return count;
        }

        public class ProductAttachment
        {
            public string business_id { get; set; }
            public string thread_id { get; set; }
            public string source_url { get; set; }
            public string image_url { get; set; }
        }

        [HttpPost("send_files")]
        public async Task<dynamic> SendFiles([FromBody]ProductAttachment data)
        {

            //data = new ProductAttachment
            //{
            //    business_id = "000000022265693",
            //    thread_id = "1235072053278327",
            //    image_url = "http://img.baza.vn/upload/products-var-GycllWIe/uKcVnSSwlarge.jpg?v=635352227557979625&width=500",
            //    source_url = "http://shop.hibaza.com/dong-ho-nam-sieu-mong-bestdon-bd9951ag-co-vua/p/GycllWIe?bzCatId=BeWykYV7&bzSid=i9OgnzdY&bzSku=BD021-03&showcommentfirst=1"
            //};

            var thread = _threadService.GetById(data.business_id, data.thread_id);
            if (thread != null)
            {
                var channel = _channelService.GetById(thread.business_id, thread.channel_id);
                var attachment = await _facebookConversationService.UploadAttachments(channel, thread, data.source_url, data.image_url);
                if (attachment != null)
                {
                    return new { attachment_id = attachment.attachment_id };
                }
            }
            return new { };
        }


        [HttpGet("autoupdatecusomterid")]
        public int AutoUpdateCustomerId([FromQuery]int minutes, [FromQuery]string access_token)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;

            RecurringJob.AddOrUpdate<MessageService>("AutoUpdateCustomerIdForAllMessages", x => x.UpdateCustomerId(), Cron.MinuteInterval(minutes));
            return count;
        }

        [HttpPost("hide/{business_id}/{id}")]
        public bool Hide(string business_id, string id)
        {
            var message = _messageService.GetById(business_id, id);
            message.hidden = true;
            _messageService.CreateMessage(business_id, message, true);
            var channel = _channelService.GetById(business_id, message.channel_id);
            return _facebookCommentService.ShowHideComment(message.ext_id, true, channel.token).Result;
        }


        [HttpPost("read/{business_id}/{id}")]
        public void Read(string business_id, string id)
        {
            var message = _messageService.GetById(business_id, id);
            if (!message.read)
            {
                message.read = true;
                _messageService.CreateMessage(business_id, message, true);
            }
        }

        [HttpPost("star/{business_id}/{id}")]
        public bool Star(string business_id, string id, [FromQuery]bool starred)
        {
            var message = _messageService.GetById(business_id, id);
            message.read = true;
            message.starred = starred;
            _messageService.CreateMessage(business_id, message, true);
            return true;
        }

        [Authorize(Policy = "AgentOrAdmin")]
        [HttpPost("delete/{business_id}/{id}")]
        public ApiResponse Delete(string business_id, string id)
        {
            BackgroundJob.Enqueue<FacebookService>(x => x.DeleteMessage(business_id, id, true));
            ApiResponse response = new ApiResponse { ok = true };
            return response;
        }

        [HttpPost("like/{business_id}/{id}")]
        public ApiResponse Like(string business_id, string id, [FromQuery]bool liked)
        {
            ApiResponse response = new ApiResponse();
            if (liked)
                try
                {
                    var message = _messageService.GetById(business_id, id);
                    var channel = _channelService.GetById(business_id, message.channel_id);
                    message.read = true;

                    if (_facebookCommentService.LikeComment(message.ext_id, channel.token).Result)
                    {
                        message.liked = liked;
                        response.ok = true;
                    }
                    _messageService.CreateMessage(business_id, message, true);
                }
                catch
                {

                }
            else

                try
                {
                    var message = _messageService.GetById(business_id, id);
                    var channel = _channelService.GetById(business_id, message.channel_id);
                    message.read = true;
                    if (_facebookCommentService.UnLikeComment(message.ext_id, channel.token).Result)
                    {
                        message.liked = liked;
                        response.ok = true;
                    }
                    _messageService.CreateMessage(business_id, message, true);
                }
                catch
                {

                }
            return response;
        }

        [HttpGet("get/{business_id}/{id}")]
        public MessageModel GetById(string business_id, string id)
        {
            return new MessageModel(_messageService.GetById(business_id, id));
        }

        [HttpPost("list/{business_id}/{thread_id}")]
        public async Task<ApiResponse> SearchThreads(string business_id, string thread_id, [FromBody]MessageFilter filter)
        {
            ApiResponse response = new ApiResponse();
            var data = await _messageService.GetByThread(business_id, new Paging { Limit = filter.quantity, Next = filter.first <= 0 ? "9999999999" : filter.first.ToString() }, thread_id);
            //if (data != null && data.Count() > 0)
            //    foreach (var message in data)
            //    {
            //        BackgroundJob.Enqueue<FacebookConversationService>(x => x.FixMessagePhotoUrls(business_id, message.id, true));
            //    }

            response.data = data == null ? new List<MessageModel>() : data.OrderBy(m => m.timestamp).Select(t => new MessageModel(t));
            response.ok = true;
            return response;
        }

        private string GetCommentLink(string business_id, string itemId)
        {
            string link = string.Format("https://business.facebook.com/{0}", itemId);
            return link;
        }

        private string GetInboxLink(string business_id, Thread thread)
        {
            string link = "";
            try
            {
                //var conversation = _conversationService.GetByChannelId(business_id, thread.channel_id);
                //if (conversation != null && !string.IsNullOrWhiteSpace(conversation.link)) link = "https://business.facebook.com" + conversation.link;

                if (!string.IsNullOrWhiteSpace(thread.owner_ext_id))
                {
                    var conversation = _conversationService.GetByOwnerExtId(business_id, thread.owner_ext_id);
                    if (conversation != null && !string.IsNullOrWhiteSpace(conversation.link)) link = "https://business.facebook.com" + conversation.link + "?selected_item_id=" + conversation.ext_id.Replace("t_","");
                }

                if (string.IsNullOrWhiteSpace(link) && !string.IsNullOrWhiteSpace(thread.owner_app_id))
                {
                    var conversation = _conversationService.GetByOwnerAppId(business_id, thread.channel_id, thread.owner_app_id);
                    if (conversation != null && !string.IsNullOrWhiteSpace(conversation.link)) link = "https://business.facebook.com" + conversation.link + "?selected_item_id=" + conversation.ext_id.Replace("t_", "");
                }

                if (string.IsNullOrWhiteSpace(link))
                {
                    link = string.Format("https://business.facebook.com/{0}/messages/?folder=inbox&section=messages&timestamp={1}", thread.channel_ext_id, thread.timestamp * 1000);
                }
            }
            catch (Exception ex)
            {
                link = "https://business.facebook.com";
            }

            return link;
        }

        [HttpGet("openlink/{business_id}")]
        public ApiResponse LinkFacebookPost(string business_id, [FromQuery]string item_id)
        {
            ApiResponse response = new ApiResponse();
            var message = _messageService.GetById(business_id, item_id);
            var itemType = message == null ? null : message.thread_type;
            var itemId = message == null ? null : message.ext_id;
            var pageId = message == null ? null : message.channel_ext_id;
            var thread_id = message == null ? null : message.thread_id;
            Thread thread = null;
            if (message == null)
            {
                thread_id = item_id;
                thread = _threadService.GetById(business_id, thread_id);
                itemType = thread == null ? null : thread.type;
                itemId = thread == null ? null : MessageService.FormatId(business_id, thread.last_message_ext_id);
                pageId = thread == null ? null : thread.channel_ext_id;
            }
            string link = itemType == "comment" ? GetCommentLink(business_id, itemId) : GetInboxLink(business_id, thread == null ? _threadService.GetById(business_id, thread_id) : thread);
            response.ok = !string.IsNullOrWhiteSpace(link);
            response.data = link;

            return response;
        }

        //private string GetFirebaseStorageMessageAttachmentDownloadUrl(string userId, string fileName)
        //{
        //    var fbs = new FirebaseStorage(_appSettings.Value.FirebaseDB.StorageBucket);
        //    var url = fbs.Child("messages").Child(userId).Child(fileName).GetFullDownloadUrl();
        //    return url;
        //}

        //private async Task<string> UploadMessageAttachmentToFirebaseStorage(string userId, string fileName, Stream stream)
        //{
        //    var fbs = new FirebaseStorage(_appSettings.Value.FirebaseDB.StorageBucket);
        //    var task = fbs.Child("messages").Child(userId).Child(fileName).PutAsync(stream);
        //    return await task;
        //}

        [HttpPost("sendpromotion")]
        public int SendPromotionMessageDayN([FromQuery]string business_id, [FromQuery]string channel_id, [FromQuery]string message, [FromQuery]int limit, [FromQuery]int from_day, [FromQuery]int to_day, [FromQuery]string access_token)
        {
            if (access_token != "marketing@Baza@2017") return 0;
            int count = 0;
            string jobId = "";
            for (int day = from_day; day <= to_day; day++)
            {
                count++;
                if (string.IsNullOrWhiteSpace(jobId))
                {
                    //await _facebookConversationService.SendPromotionMessage(business_id, channel_id ?? "", message, limit, day);
                    jobId = BackgroundJob.Enqueue<FacebookConversationService>(x => x.SendPromotionMessage(business_id, channel_id ?? "", message, limit, day));
                }
                else
                {
                    jobId = BackgroundJob.ContinueWith<FacebookConversationService>(jobId, x => x.SendPromotionMessage(business_id, channel_id ?? "", message, limit, day));
                }
            }
            return count;
        }


        [HttpPost("sendpromotion_day1")]
        public async Task<int> SendPromotionMessageDay1([FromQuery]string business_id, [FromQuery]string channel_id, [FromQuery]string message, [FromQuery]int limit, [FromQuery]string access_token)
        {
            if (access_token != "marketing@Baza@2017") return 0;
            return await _facebookConversationService.SendPromotionMessage(business_id, channel_id ?? "", message, limit, 1);
        }

        [HttpPost("sendpromotion_day2")]
        public async Task<int> SendPromotionMessageDay2([FromQuery]string business_id, [FromQuery]string channel_id, [FromQuery]string message, [FromQuery]int limit, [FromQuery]string access_token)
        {
            if (access_token != "marketing@Baza@2017") return 0;
            return await _facebookConversationService.SendPromotionMessage(business_id, channel_id ?? "", message, limit, 2);
        }


        //private async Task<int> SendPromotionMessage(string business_id, string channel_id, string message, int limit, int day = 1)
        //{
        //    int count = 0;
        //    Paging page = new Paging { Limit = limit, Previous = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow.Date.AddDays(-day).AddHours(-12)).ToString(), Next = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow.Date.AddDays(-day).AddHours(12)).ToString() };
        //    foreach (var t in await _threadService.GetNoReponseThreads(business_id, channel_id, page))
        //        if (t.type == "message")
        //        {
        //            var form = new MessageFormData
        //            {
        //                business_id = business_id,
        //                channel_id = t.channel_id,
        //                thread_id = t.id,
        //                agent_id = null,
        //                recipient_id = t.owner_ext_id,
        //                tag = "",
        //                image_urls = "",
        //                description = "",
        //                message = message
        //            };
        //            t.agent_id = null;
        //            try
        //            {
        //                var data = new MessageData { image_urls = new List<string>(), attachment_urls = new List<string>(), message = form.message };
        //                var channel = _channelService.GetById(business_id, t.channel_id);
        //                await _facebookConversationService.SendMessage(channel, t, data);
        //                count++;
        //            }
        //            catch { }
        //        }
        //    return count;
        //}


        [HttpPost("send")]
        public async Task<ApiResponse> Send([FromForm]MessageFormData form)
        {
            try
            {
                var thread = _threadService.GetById(form.business_id, form.thread_id);
                thread.agent_id = string.IsNullOrWhiteSpace(form.agent_id) ? thread.agent_id : form.agent_id;
                return await SendMessage(thread, null, form);
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Thread",
                    link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
                    details = JsonConvert.SerializeObject(ex.StackTrace),
                    name = string.Format("Send message for thread: {0}", form.recipient_id)
                });
                throw ex;
            }
        }

        private class CommentData
        {
            public string fileId { get; set; }
            public Stream stream { get; set; }
            public string attachment_url { get; set; }
            public FacebookCommentPost comment { get; set; }
        }

        private async System.Threading.Tasks.Task SendComment(Channel channel, Thread thread, CommentData data, string object_ext_id, string parent_ext_id)
        {

            if (data.stream != null)
            {
                //var rs = await UploadMessageAttachmentToFirebaseStorage(channel.id, data.FileId, data.Stream);
                var rs = await _messageService.UploadAttachmentToFirebaseStorage(channel.business_id, "attachments", data.fileId, data.stream);
            }

            var timestamp = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow);
            if (data.comment.message == null || data.comment.message == "")
                data.comment.message = thread.sender_name;

            var t = await _facebookCommentService.PostComment(object_ext_id, data.comment, channel.token);
            var r = _facebookCommentService.PostPrivateReply(object_ext_id, data.comment, channel.token);
            if ((t == null || string.IsNullOrWhiteSpace(t.id)))
            {
                if (object_ext_id != parent_ext_id && !string.IsNullOrWhiteSpace(parent_ext_id))
                {
                    t = await _facebookCommentService.PostComment(parent_ext_id, data.comment, channel.token);
                }
                if (t == null || string.IsNullOrWhiteSpace(t.id))
                {
                    throw new Exception("Failed to post the comment! " + object_ext_id + " " + parent_ext_id);
                }
            }
            else
            {
                BackgroundJob.Enqueue<MessageService>(x => x.MarkAsReplied(channel.business_id, MessageService.FormatId(channel.business_id, object_ext_id), timestamp));
            }

            string channel_id = channel.id;
            var thread_id = thread.id;
            var msgId = t.id;
            string message_id = MessageService.FormatId(channel.business_id, msgId);

            string msgType = string.IsNullOrWhiteSpace(data.attachment_url) ? "text" : "image";
            string pageId = channel.ext_id;

            var fm = new FacebookMessageObject
            {
                msgId = msgId,
                msgParentId = object_ext_id,
                msgRootId = thread.link_ext_id,
                conversationId = thread.ext_id,
                ownerId = string.IsNullOrWhiteSpace(thread.owner_app_id) ? thread.owner_ext_id : thread.owner_app_id,
                senderId = pageId,
                sender_name = channel.name,
                recipientId = "",
                recipient_name = "",
                customer_id = thread.customer_id,
                timestamp = timestamp,
                text = data.comment.message,
                type = msgType,
                liked = true,
                hidden = true,
                thread_type = "comment",
                channel_type = channel.type,
                url = data.attachment_url,
                urls = new List<string>(),
                attachements = new List<FacebookAttachmentGet>()
            };
            var owner_id = thread.owner_id;
            bool is_new = false;
            string tid = "";
            var message = _facebookConversationService.CreateMessage(channel, fm, message_id, thread.agent_id, owner_id, true, out is_new, out tid);
            BackgroundJob.Enqueue<FacebookConversationService>(x => x.CreateThread(channel, message, null, null, owner_id, thread.owner_ext_id, thread.owner_app_id, true));
            if (!string.IsNullOrWhiteSpace(message.agent_id))
            {
                BackgroundJob.Enqueue<AgentService>(x => x.SetLastActivityTime(message.agent_id, Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow)));
            }
        }

        private async Task<ApiResponse> SendComment(Thread thread, MessageFormData form, string object_ext_id, string parent_ext_id)
        {

            ApiResponse response = new ApiResponse();
            List<CommentData> comments = new List<CommentData>();
            try
            {
                var channel = _channelService.GetById(thread.business_id, thread.channel_id);
                var access_token = channel.token;
                List<bool> image_flags = string.IsNullOrWhiteSpace(form.image_flags) ? null : JsonConvert.DeserializeObject<List<bool>>(form.image_flags);
                List<string> image_urls = string.IsNullOrWhiteSpace(form.image_urls) ? null : JsonConvert.DeserializeObject<List<string>>(form.image_urls);
                List<string> titles = string.IsNullOrWhiteSpace(form.titles) ? null : JsonConvert.DeserializeObject<List<string>>(form.titles);
                int count = 0;
                if (image_flags != null)
                {
                    var files = Request.Form.Files;
                    int i = 0;
                    foreach (var file in files)
                    {
                        if (i < image_flags.Count && image_flags[i])
                        {

                            var filename = ContentDispositionHeaderValue
                                            .Parse(file.ContentDisposition)
                                            .FileName;
                            var stream = file.OpenReadStream();
                            if (file.Length > 0)
                            {
                                count++;
                                var fileId = Core.Helpers.CommonHelper.GenerateDigitUniqueNumber() + "_" + i;
                                var fileUrl = _messageService.GetFirebaseStorageAttachmentUrl(thread.business_id, "attachments", fileId);
                                comments.Add(new CommentData { fileId = fileId, stream = stream, attachment_url = fileUrl, comment = new FacebookCommentPost { attachment_url = fileUrl, message = i < titles.Count ? titles[i] : "" } });//               
                            }
                        }
                        i++;
                        if (count >= 6) break;
                    }
                }
                if (image_urls != null)
                {
                    var i = 0;
                    foreach (var url in image_urls)
                    {
                        if (!string.IsNullOrWhiteSpace(url))
                        {
                            //string id = MessageService.FormatAttachmentId(channel.id + "_facebook_comment", url);
                            //var attachment = _messageService.GetAttachmentById(channel.business_id, channel.id, id);
                            //if (attachment == null)
                            //{
                            //    FacebookCommentPostResponse res = await _facebookCommentService.PostPhoto(channel.ext_id, new FacebookPhotoPost
                            //    {
                            //        url = url,
                            //        published = false,
                            //        no_story = true
                            //    }
                            //   , channel.token);
                            //    if (!string.IsNullOrWhiteSpace(res.id))
                            //    {
                            //        attachment = new Attachment
                            //        {
                            //            id = id,
                            //            business_id = channel.business_id,
                            //            channel_id = channel.id,
                            //            attachment_id = res.id,
                            //            attachment_url = url,
                            //            target = "facebook_comment",
                            //            timestamp = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow),
                            //            type = "photo"
                            //        };
                            //        _messageService.SaveAttachment(attachment);
                            //    }
                            //}

                            //if (attachment != null && !string.IsNullOrWhiteSpace(attachment.attachment_id))
                            //{
                            //    comments.Add(new CommentData { attachment_url = url, comment = new FacebookCommentPost { attachment_id = attachment.attachment_id } });
                            //}
                            //else
                            //{
                            //    comments.Add(new CommentData { attachment_url = url, comment = new FacebookCommentPost { attachment_url = url } });
                            //}

                            comments.Add(new CommentData { attachment_url = url, comment = new FacebookCommentPost { attachment_url = url, message = i < titles.Count ? titles[i] : "" } });
                            i++;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(form.message))
                {
                    comments.Add(new CommentData { comment = new FacebookCommentPost { message = form.message } });
                }


                await System.Threading.Tasks.Task.WhenAll(comments.Select(data => SendComment(channel, thread, data, object_ext_id, parent_ext_id)));


                response.ok = true;
                return response;
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Comment",
                    link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
                    details = JsonConvert.SerializeObject(ex.StackTrace) + JsonConvert.SerializeObject(comments),
                    name = string.Format("Send comment to object Id: {0}", object_ext_id)
                });
                throw ex;
            }
        }


        private async Task<ApiResponse> SendMessage(Thread thread, Ticket ticket, MessageFormData form)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Channel channel = _channelService.GetById(form.business_id, form.channel_id);
                var access_token = channel.token;
                var para = JsonConvert.DeserializeObject<dynamic>(form.para);
                Hibaza.CCP.Data.Common _cm = new Hibaza.CCP.Data.Common();


                #region luu hinh anh den server rieg
                try
                {
                    if (form.channel_format == "image-text"  || form.channel_format == "image" || form.channel_format == "receipt")
                    {
                        if (para.message != null && para.message.attachment != null && para.message.attachment.payload != null && para.message.attachment.payload.elements != null)
                        {
                            List<payloadFb> elements = JsonConvert.DeserializeObject<List<payloadFb>>(JsonConvert.SerializeObject(para.message.attachment.payload.elements));
                            for (var i = 0; i < elements.Count; i++)
                            {
                                try
                                {
                                    if (!string.IsNullOrWhiteSpace(elements[i].image_url))
                                    {
                                        var newImageUrl = await ImagesService.UpsertImageStore(elements[i].image_url, _appSettings.Value);
                                        para.message.attachment.payload.elements[i].image_url = newImageUrl;
                                        para.message.attachment.payload.elements[i].default_action.url = newImageUrl;
                                    }
                                }
                                catch(Exception ex) { }
                            }
                        }
                    }
                }
                catch { }
                #endregion


                #region file attachment
                // var data = new MessageData { images = new List<MessageFile>() };
                var files = Request.Form.Files;
                var j = 0;
                if (files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = ContentDispositionHeaderValue
                                      .Parse(file.ContentDisposition)
                                      .FileName.ToString();
                            fileName = Core.Helpers.CommonHelper.removeSpecialFile(fileName);

                            var fileId = "";
                            if ((thread.id + fileName).Length < 100)
                                fileId = thread.id + "_" + Core.Helpers.CommonHelper.GenerateDigitUniqueNumber() + "_" + fileName;
                            else
                                fileId = thread.id + "_" + fileName;
                            // var path = Path.Combine(Directory.GetCurrentDirectory(), @"Documents", "Attachments", fileId);
                            var dir = Path.Combine(Directory.GetCurrentDirectory(), _appSettings.Value.PathToFileDocuments);
                            var fullName = Path.Combine(dir, fileId);

                            if (!Directory.Exists(dir))
                                Directory.CreateDirectory(dir);

                            using (FileStream fs = System.IO.File.Create(fullName))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }
                            var bytes = _cm.ConvertToBytes(file);
                            var newUrl = await _cm.HtmlPostBytesAsync<string>(_appSettings.Value.BaseUrls.ApiSaveImage
                               + "api/UploadFile", thread.id + "_" + file.FileName, bytes);
                          
                            var elements = JsonConvert.DeserializeObject<List<payloadFb>>(JsonConvert.SerializeObject(para.message.attachment.payload.elements));
                          
                            para.message.attachment.payload.elements[j].image_url = newUrl;
                            para.message.attachment.payload.elements[j].title = para.message.attachment.payload.elements[j].title == "" ?
                                channel.name : para.message.attachment.payload.elements[j].title;
                            para.message.attachment.payload.elements[j].default_action.url = newUrl;
                            j++;

                            //var bytes = _cm.ConvertToBytes(file);
                            //var newUrl = await _cm.HtmlPostBytesAsync<string>(_appSettings.Value.BaseUrls.ApiSaveImage
                            //    + "File_DocumentsView/UploadFile", thread.id + "_" + file.FileName, bytes);

                            //var elements = JsonConvert.DeserializeObject<List<payloadFb>>(JsonConvert.SerializeObject(para.message.attachment.payload.elements));

                            //para.message.attachment.payload.elements[j].image_url = newUrl;
                            //para.message.attachment.payload.elements[j].default_action.url = newUrl;
                            //para.message.attachment.payload.elements[j].title = para.message.attachment.payload.elements[j].title == "" ?
                            //    channel.name : para.message.attachment.payload.elements[j].title;
                            //j++;
                        }
                    }
                }
                #endregion
                #region send

                string object_ext_id = "", parent_ext_id = "";
                string url = "";
                List<string> msgIds = new List<string>();
                var client = new HttpClient();
                if (thread.type == "comment")
                {
                    object_ext_id = thread.owner_last_message_ext_id;
                   parent_ext_id = thread.owner_last_message_parent_ext_id != thread.link_ext_id ? thread.owner_last_message_parent_ext_id : null;
                   // parent_ext_id = thread.owner_last_message_parent_ext_id;

                    //var message = _messageService.GetById(thread.business_id, (string)para.recipient.id);
                    //if (message != null)
                    //{
                    //    object_ext_id = message.ext_id;
                    //    parent_ext_id = message.parent_ext_id != message.root_ext_id ? message.parent_ext_id : null;
                    //}

                    url = "https://graph.facebook.com/v2.10/" + (object_ext_id != null && object_ext_id != "" ? object_ext_id : parent_ext_id)
                    + "/comments?access_token=" + channel.token;

                    List<payloadFb> elements = new List<payloadFb>();
                    List<FacebookCommentPost> comments = new List<FacebookCommentPost>();
                    if (form.channel_format == "image-text" || form.channel_format == "image" || form.channel_format == "receipt")
                    {
                        elements = JsonConvert.DeserializeObject<List<payloadFb>>(JsonConvert.SerializeObject(para.message.attachment.payload.elements));
                        for (var i = 0; i < elements.Count; i++)
                        {
                            FacebookCommentPost item = new FacebookCommentPost();
                            item.message = elements[i].title == "" ? thread.sender_name : elements[i].title;
                            item.attachment_url = elements[i].image_url;
                            comments.Add(item);
                        }
                    }
                    else
                    {
                        FacebookCommentPost item = new FacebookCommentPost();
                        item.message = para.message.text == "" ? thread.sender_name : para.message.text;
                        comments.Add(item);
                    }

                    for (var i = 0; i < comments.Count; i++)
                    {
                        form.recipient_id = object_ext_id;
                        var t = await _facebookCommentService.PostComment(object_ext_id, comments[i], access_token);
                        var r = _facebookCommentService.PostPrivateReply(object_ext_id, comments[i], access_token);
                        if (t == null || t.id == null)
                        {
                            var sp = object_ext_id.Split('_');
                            form.recipient_id =  parent_ext_id;
                            t = await _facebookCommentService.PostComment(parent_ext_id==null ? sp[0]:parent_ext_id, comments[i], access_token);
                            // them truong hop dac biet khong gui duoc comment. thu them
                            if (t == null || string.IsNullOrWhiteSpace(t.id))
                            {
                                var p1 = object_ext_id.Split('_');
                                var p2 = parent_ext_id.Split('_');
                                form.recipient_id = p1[0] + "_" + p2[1];
                                t = await _facebookCommentService.PostComment(form.recipient_id, comments[i], access_token);

                                if (t == null || string.IsNullOrWhiteSpace(t.id))
                                {
                                    throw new Exception("Failed to send the comment to " + form.recipient_id);
                                }
                            }
                        }
                        if (t.id != null)
                        {
                            msgIds.Add((string)t.id);
                            var timestamp = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow);
                            BackgroundJob.Enqueue<MessageService>(x => x.MarkAsReplied(channel.business_id, MessageService.FormatId(channel.business_id, object_ext_id), timestamp));
                        }
                    }
                }
                else
                {
                    if (form.channel_format == "image-text" || form.channel_format == "image" || form.channel_format == "receipt")
                    {
                        var elements = JsonConvert.DeserializeObject<List<payloadFb>>(JsonConvert.SerializeObject(para.message.attachment.payload.elements));
                        for (var i = 0; i < elements.Count; i++)
                        {
                            var tt = para.message.attachment.payload.elements[i].title == "" ?
                             channel.name : para.message.attachment.payload.elements[i].title;
                            para.message.attachment.payload.elements[i].title = tt;
                            para.message.attachment.payload.elements[i].buttons[0].payload = tt;
                        }
                    }

                    url = "https://graph.facebook.com/me/messages?access_token=" + channel.token;
                    var rs = client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(para), Encoding.UTF8, "application/json")).Result;
                    var contents = rs.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<dynamic>(contents);
                    if (result.message_id == null)
                    {
                        throw new Exception("Failed to send the message to " + para.recipient);
                    }
                    msgIds.Add("m_" + result.message_id);
                }

                #endregion
                //System.Threading.Tasks.Task.Factory.StartNew(() =>
                // {
                try
                {
                    for (var i = 0; i < msgIds.Count; i++)
                    {
                        string p = JsonConvert.SerializeObject(para);
                        form.para = p.Replace(_appSettings.Value.BaseUrls.Api, "");
                        
                         _facebookConversationService.SendMessage(channel, thread, form, msgIds[i]);
                    }
                }
                catch(Exception ex) {
                }
                //});
               // var page =new Paging();
               // page.Limit = Convert.ToInt32(_appSettings.Value.LimitAssign);
                //_customerService.AutoAssignToAvailableAgents(thread.business_id, page);
                response.ok = true;
                return response;
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Message",
                    link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
                    details = JsonConvert.SerializeObject(ex.StackTrace),
                    name = string.Format("Send message to recipient_id: {0}", thread.owner_ext_id)
                });
                throw ex;
            }
        }
        //private async Task<ApiResponse> SendMessage1(Thread thread, Ticket ticket, MessageFormData form)
        //{
        //    return null;
        //    //ApiResponse response = new ApiResponse();
        //    //List<MessageData> messages = new List<MessageData>();
        //    //var data = new MessageData { images = new List<MessageFile>(), image_urls = new List<string>(), attachment_urls = new List<string>() ,titles= new List<string>()};
        //    //bool not_use_template = string.IsNullOrWhiteSpace(form.tag);
        //    //try
        //    //{
        //    //    var channel = _channelService.GetById(form.business_id, form.channel_id);
        //    //    var access_token = channel.token;
        //    //    List<bool> image_flags = string.IsNullOrWhiteSpace(form.image_flags) ? null : JsonConvert.DeserializeObject<List<bool>>(form.image_flags);
        //    //    List<string> image_urls = string.IsNullOrWhiteSpace(form.image_urls) ? null : JsonConvert.DeserializeObject<List<string>>(form.image_urls);
        //    //    List<string> titles = string.IsNullOrWhiteSpace(form.titles) ? null : JsonConvert.DeserializeObject<List<string>>(form.titles);
        //    //    int count = 0;
        //    //    if (image_flags != null && image_flags.Count > 0)
        //    //    {
        //    //        var files = Request.Form.Files;
        //    //        int i = 0;
        //    //        foreach (var file in files)
        //    //        {
        //    //            if (i < image_flags.Count && image_flags[i])
        //    //            {
        //    //                var fileName = ContentDispositionHeaderValue
        //    //                                .Parse(file.ContentDisposition)
        //    //                                .FileName
        //    //                                .Trim('"');
        //    //                var stream = file.OpenReadStream();
        //    //                var fileId = thread.id + "_" + Core.Helpers.CommonHelper.GenerateDigitUniqueNumber() + "_" + fileName;
        //    //                var fileUrl = _messageService.GetFirebaseStorageAttachmentUrl(form.business_id, "attachments", fileId);
        //    //                if (file.Length > 0)
        //    //                {
        //    //                    count++;
        //    //                    if (not_use_template)
        //    //                    {
        //    //                        var d = new MessageData { images = new List<MessageFile>(), image_urls = new List<string>(), attachment_urls = new List<string>()};
        //    //                        d.images.Add(new MessageFile { Stream = stream, FileId = fileId });
        //    //                        d.image_urls.Add(fileUrl);
        //    //                        d.titles.Add(titles !=null && i < titles.Count ? titles[i] : form.message !=null ? form.message : thread.sender_name);
        //    //                        messages.Add(d);
        //    //                    }
        //    //                    else
        //    //                    {
        //    //                        data.images.Add(new MessageFile { Stream = stream, FileId = fileId });
        //    //                        data.image_urls.Add(fileUrl);
        //    //                        data.titles.Add(titles !=null && i < titles.Count ? titles[i] : form.message != null ? form.message : thread.sender_name);
        //    //                    }
        //    //                }
        //    //            }
        //    //            i++;
        //    //            if (count >= 6) break;
        //    //        }
        //    //    }

        //    //    if (image_urls != null)
        //    //    {
        //    //        var j = 0;
        //    //        foreach (var url in image_urls)
        //    //        {
        //    //            if (!string.IsNullOrWhiteSpace(url))
        //    //            {
        //    //                if (not_use_template)
        //    //                {
        //    //                    var d = new MessageData { image_urls = new List<string>(), attachment_urls = new List<string>() };
        //    //                    d.attachment_urls.Add(url);
        //    //                    messages.Add(d);
        //    //                }
        //    //                else
        //    //                {
        //    //                    data.attachment_urls.Add(url);
        //    //                    data.titles.Add(titles !=null && j <  titles.Count ? titles[j] : form.message != null ? form.message : thread.sender_name);
        //    //                    j++;
        //    //                }
        //    //            }
        //    //        }
        //    //    }
        //    //    data.button_title = form.button_title;
        //    //    data.tag = form.tag;
        //    //    data.message = form.message;
        //    //    data.description = form.description;
        //    //    if (not_use_template)
        //    //    {
        //    //        if (!string.IsNullOrWhiteSpace(form.message))
        //    //        {
        //    //            messages.Add(new MessageData { image_urls = new List<string>(), attachment_urls = new List<string>(), message = form.message });
        //    //        }
        //    //        await System.Threading.Tasks.Task.WhenAll(messages.Select(d => _facebookConversationService.SendMessage(channel, thread, d)));
        //    //    }
        //    //    else
        //    //    {
        //    //        data.template = "generic";
        //    //        await _facebookConversationService.SendMessage(channel, thread, data);
        //    //    }

        //    //    response.ok = true;
        //    //    return response;
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    _logService.Create(new Log
        //    //    {
        //    //        message = ex.Message,
        //    //        category = "Message",
        //    //        link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
        //    //        details = JsonConvert.SerializeObject(ex.StackTrace) + JsonConvert.SerializeObject(messages),
        //    //        name = string.Format("Send message to recipient_id: {0}", thread.owner_ext_id)
        //    //    });
        //    //    throw ex;
        //    //}
        //}


        #region gửi message tổng quan

        [HttpPost("sendtemplate")]
        public string SendTemplate([FromBody]receiveInfo value)
        {
            Common cm = new Common();
            var configDic = cm.ConvertConfigToDic(value.config);
            var channel = _channelService.GetById(configDic["business_id"], configDic["channel_id"]);
            #region send
            var url = "https://graph.facebook.com/me/messages?access_token=" + channel.token;
            var client = new HttpClient();
            var response = client.PostAsync(url, new StringContent(value.para, Encoding.UTF8, "application/json")).Result;
            var contents = response.Content.ReadAsStringAsync().Result;
            #endregion
            return contents;
        }

        [HttpPost("savemessage")]
        public string SaveMessage([FromBody]receiveInfo value)
        {
            var listPara = JsonConvert.DeserializeObject<List<receiveList>>(value.para);

            Common cm = new Common();
            var dicConfig = cm.ConvertConfigToDic(value.config);
            var thread = _threadService.GetById(dicConfig["business_id"], dicConfig["thread_id"]);
            var resutl = false;
            foreach (var val in listPara)
            {
                var listUrls = new List<string>();
                listUrls.Add(val.image_url);

                Message m = new Message();
                if (thread.last_message_ext_id != null)
                    m.id = thread.last_message_ext_id.Replace(".$", "____s_");
                m.created_time = thread.created_time;
                m.updated_time = thread.updated_time <= DateTime.MinValue ? null : thread.updated_time;
                m.ext_id = thread.last_message_ext_id;
                m.url = val.image_url;
                m.urls = JsonConvert.SerializeObject(listUrls);
                m.file_name = "";
                m.size = 0;
                m.subject = "";
                m.message = val.title;
                m.agent_id = thread.agent_id;
                m.thread_id = thread.ext_id;
                m.conversation_ext_id = thread.ext_id;
                m.sender_id = thread.channel_ext_id;
                m.sender_ext_id = thread.channel_ext_id;
                m.sender_name = thread.sender_name;
                m.sender_avatar = thread.sender_avatar;
                m.recipient_id = thread.ext_id;
                m.recipient_ext_id = thread.ext_id;
                m.recipient_avatar = "";
                m.recipient_name = "";
                m.author = thread.channel_ext_id;
                m.customer_id = thread.customer_id;
                m.type = val.image_url == "" ? "text" : "image";
                m.timestamp = thread.timestamp > 9999999999 ? thread.timestamp / 1000 : thread.timestamp;
                m.business_id = thread.business_id;
                m.channel_id = thread.channel_id;
                m.channel_ext_id = thread.channel_ext_id;
                m.channel_type = thread.channel_type;
                m.template = "generic";
                m.thread_type = "message";
                resutl = _messageService.CreateMessage(dicConfig["business_id"], m, true);
                if (!resutl)
                    return "{result:ERROR}";
            }
            return "{result:OK}";
        }

        public class sendInfo
        {
            public string config { get; set; }
            public string para { get; set; }
        }

        public class receiveInfo
        {
            public string config { get; set; }
            public string para { get; set; }
        }
        public class receiveList
        {
            public string title { get; set; }
            public string image_url { get; set; }
        }
        #endregion
    }
}