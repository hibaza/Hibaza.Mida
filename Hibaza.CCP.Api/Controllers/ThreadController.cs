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
using Hibaza.CCP.Service.SQL;
using Hibaza.CCP.Data.Repositories.Firebase;
using Hibaza.CCP.Data.Providers.Firebase;
using Hibaza.CCP.Data;
using System.Text;
using MongoDB.Driver;

namespace Hibaza.CCP.Api.Controllers
{

    public class ThreadFilter
    {
        public long first { get; set; }
        public int quantity { get; set; }
        public string agent_id { get; set; }
        public string channel_id { get; set; }
        public string status { get; set; }
        public string flag { get; set; }
        public string search { get; set; }
        public string from_date { get; set; }
        public string to_date { get; set; }
    }

    public class LastVisit
    {
        public string business_id { get; set; }
        public string thread_id { get; set; }
        public string agent_id { get; set; }
        public string url { get; set; }
    }

    [Route("threads")]
    public class ThreadController : Controller
    {
        private readonly IThreadService _threadService;
        private readonly IMessageService _messageService;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ILoggingService _logService;
        private readonly IFacebookService _facebookService;
        private readonly IChannelService _channelService;
        private readonly IAgentService _agentService;
        private readonly IBusinessService _businessService;
        private readonly IConversationService _conversationService;
        private readonly ICustomerService _customerService;
        private readonly IFacebookConversationService _facebookConversationService;

        public ThreadController(IThreadService threadService, IFacebookConversationService facebookConversationService, ICustomerService customerService, IConversationService conversationService, IBusinessService businessService, IAgentService agentService, IMessageService messageService, IFacebookService facebookService, IChannelService channelService, IOptions<AppSettings> appSettings, ILoggingService logService)
        {
            _threadService = threadService;
            _messageService = messageService;
            _appSettings = appSettings;
            _channelService = channelService;
            _facebookService = facebookService;
            _agentService = agentService;
            _businessService = businessService;
            _conversationService = conversationService;
            _customerService = customerService;
            _facebookConversationService = facebookConversationService;
            _logService = logService;
        }

        [HttpGet("unassigninactive")]
        public int UnAssignFromInActiveAgents([FromQuery]string business_id, [FromQuery]string access_token)
        {
            int count = 0;
            if (access_token != "@bazavietnam") return count;
            count = _threadService.UnAssignFromInActiveAgents(business_id, new Paging { Limit = 50 });
            return count;
        }

        [HttpGet("assigninbatch")]
        public int BatchAssignToAgents([FromQuery]string business_id, [FromQuery]string access_token)
        {
            int count = 0;
            if (access_token != "@bazavietnam") return count;
           // count = _threadService.AutoAssignToAvailableAgents(business_id, new Paging { Limit = 50 });
            return 0;
        }

        [HttpGet("autounassign")]
        public int AutoUnAssignFromAgents([FromQuery]int minutes, [FromQuery]string access_token)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;

            foreach (var b in _businessService.GetBusinesses(0, 100).Result.Where(b => b.active))
            {
                count++;
                var business_id = b.id;
                RecurringJob.AddOrUpdate<ThreadService>("AutoUnAssignFromAgentsForBusiness[" + business_id + "]", x => x.UnAssignFromInActiveAgents(business_id, new Paging { Limit = 50 }), Cron.MinuteInterval(minutes));
            }
            return count;
        }


        [HttpGet("autoassign")]
        public int AutoAssignToAgents([FromQuery]int minutes, [FromQuery]int limit, [FromQuery]string access_token)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;

            foreach (var b in _businessService.GetBusinesses(0, 100).Result.Where(b => b.active))
            {
                count++;
                var business_id = b.id;
                RecurringJob.AddOrUpdate<ThreadService>("AutoAssignToAgentsForBusiness[" + business_id + "]", x => x.AutoAssignToAvailableAgents(business_id, new Paging { Limit = limit }), Cron.MinuteInterval(minutes));
            }
            return count;
        }

        //[HttpGet("autoupdatecusomterid")]
        //public int AutoUpdateCustomerId([FromQuery]string business_id, [FromQuery]int minutes, [FromQuery]int limit, [FromQuery]string access_token)
        //{
        //    if (access_token != "@bazavietnam") return 0;

        //    RecurringJob.AddOrUpdate<ThreadService>("UpdateCustomerNullForMessagerAndThread", x => x.UpdateCustomerId(business_id,limit), Cron.MinuteInterval(minutes));
        //    return 1;
        //}

        [HttpGet("autoupdateunreadthreadcounters")]
        public int AutoUpdateUnreadThreadCounters([FromQuery]int minutes, [FromQuery]string access_token)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;

            foreach (var b in _businessService.GetBusinesses(0, 100).Result.Where(b => b.active))
            {
                count++;
                var business_id = b.id;
                RecurringJob.AddOrUpdate<ThreadService>("AutoUpdateUnreadThreadCountersForBusiness[" + business_id + "]", x => x.BatchUpdateUnreadCounters(business_id), Cron.MinuteInterval(minutes));
            }
            return count;
        }

        [HttpGet("autoupdatestatus")]
        public int AutoUpdateStatus([FromQuery]string business_id, [FromQuery]int minutes, [FromQuery]string access_token, [FromQuery]string status = "", [FromQuery]int limit = 1000)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;
            RecurringJob.AddOrUpdate<ThreadService>("AutoUpdate" + (status ?? "") + "ThreadStatusForBusiness[" + business_id + "]", x => x.UpdateThreadStatus(business_id, status, limit), Cron.MinuteInterval(minutes));
            return count;
        }

        [HttpGet("refresh/{business_id}")]
        public int Refresh(string business_id, [FromQuery]string thread_id, [FromQuery]string access_token, [FromQuery]string status = "", [FromQuery]int limit = 1000)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;
            var jobId = "";
            if (!string.IsNullOrWhiteSpace(thread_id))
            {
                _threadService.RefreshThread(business_id, thread_id, "");
            }
            else
            {
                foreach (var t in _threadService.GetThreads(business_id, "", "", status, "", "", new Paging { Limit = limit }).Result)
                {
                    count++;
                    jobId = BackgroundJob.Enqueue<ThreadService>(x => x.RefreshThread(business_id, t.id, true, true));
                }
            }
            return count;
        }

        //[HttpGet("set_user_id/{business_id}")]
        //public int UpdateUserId(string business_id, [FromQuery]string thread_id, [FromQuery]int limit, [FromQuery]string access_token)
        //{
        //    int count = 0;

        //    if (access_token != "@bazavietnam") return count;
        //    if (!string.IsNullOrWhiteSpace(thread_id))
        //    {
        //        count += _threadService.UpdateCustomerId(business_id, thread_id) ? 1 : 0;
        //    }
        //    else
        //    {
        //         count = _threadService.UpdateCustomerId(business_id, limit);
        //    }
        //    return count;
        //}

        [HttpGet("fix/{business_id}")]
        public int FixThreadData(string business_id, [FromQuery]string access_token)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;

            foreach (var t in _threadService.GetThreads(business_id, "", "", "", "", "", new Paging { Limit = 100000 }).Result)
            {
                var status = string.IsNullOrWhiteSpace(t.agent_id) ? "pending" : "active";
                var unread = t.sender_ext_id != t.channel_ext_id;
                if (t.timestamp > 9999999999 || t.status != status || t.timestamp < 99999999 || t.unread != unread)
                {
                    t.unread = unread;
                    t.status = status;
                    count++;
                    if (t.timestamp > 9999999999)
                    {
                        t.timestamp = t.timestamp / 1000;
                    }
                    if (t.timestamp < 99999999)
                    {
                        t.timestamp = t.timestamp * 1000;
                    }
                    _threadService.CreateThread(t, false);
                }

            }
            return count;
        }


        //[HttpGet("createindex/{business_id}")]
        //public int CreateIndexOnName(string business_id, [FromQuery]string access_token)
        //{
        //    int count = 0;

        //    if (access_token != "@bazavietnam") return count;

        //    FirebaseThreadRepository fb = new FirebaseThreadRepository(new FirebaseFactory(_appSettings));
        //    foreach (var t in _threadService.GetThreads(business_id, new Paging { Limit = 10 }, "", "", "", "unread").Result)
        //    //foreach (var t in _threadService.All(business_id).Result)
        //    {
        //        string st = JsonConvert.SerializeObject(t);
        //        st = st.Substring(0, st.Length - 1);
        //        if (!string.IsNullOrWhiteSpace(t.owner_name))                {
        //            string components = "";
        //            var keywords = t.owner_name.Trim().Replace("  ", " ").ToLower();
        //            int order = 0;
        //            foreach (var word in keywords.Split(' '))
        //            {
        //                order++;
        //                for (int i = 0; i < word.Length; i++)
        //                {
        //                    string key = word.Substring(0, word.Length - i);
        //                    components += "'w" + order + "l" + key.Length + "':'" + key + "',";
        //                }

        //            }
        //            if (components.Length > 1) components = components.Substring(0, components.Length - 1);
        //            components = components + "}";
        //            st += "," + components;

        //            //var stt = JsonConvert.DeserializeObject(st);
        //            Thread tt= JsonConvert.DeserializeObject<Thread>(st);
        //            tt.components = null;
        //            _threadService.CreateThread(tt);
        //        }
        //    }
        //    return count;
        //}


        [HttpGet("copyto/{business_id}")]
        public int CopToSQL(string business_id, [FromQuery]string access_token)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;
            FirebaseThreadRepository fb = new FirebaseThreadRepository(new FirebaseFactory(_appSettings));

            foreach (var t in fb.GetThreads(business_id, new Paging { Limit = 100000 }).Result)
            //foreach (var t in _threadService.All(business_id).Result)
            {
                count++;
                _threadService.CreateThread(t, false);
            }
            return count;
        }


        [HttpGet("updatethreadid/{business_id}")]
        public int UpdateThreadId(string business_id, [FromQuery]string access_token)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;
            var list = _threadService.All(business_id).Result.ToList();
            for (int i = 0; i < list.Count(); i++)
            {
                var t = list[i];
                var id = t.id;
                var d = string.IsNullOrWhiteSpace(t.id) ? null : t.id.Split('_');
                var tidn = d != null && d.Length > 0 ? d.Last() : "";
                t.id = ThreadService.FormatId(business_id, tidn);

                if (t.timestamp > 9999999999 || id != t.id || t.timestamp < 99999999)
                {
                    var job = BackgroundJob.Enqueue<ThreadService>(x => x.CreateThread(t, false));
                    count++;
                    if (t.timestamp > 9999999999)
                    {
                        t.timestamp = t.timestamp / 1000;
                    }
                    if (t.timestamp < 99999999)
                    {
                        t.timestamp = t.timestamp * 1000;
                    }
                    BackgroundJob.ContinueWith<ThreadService>(job, x => x.Delete(business_id, id));
                }
            }

            return count;
        }

        [HttpGet("updatemessageid/{business_id}")]
        public int UpdateMessageId(string business_id, [FromQuery]string access_token)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;
            var list = _messageService.All(business_id, new Paging { Limit = 10 }).Result.ToList();
            for (int i = 0; i < list.Count(); i++)
            {
                var t = list[i];
                var id = t.id;
                var tid = t.thread_id;
                t.id = MessageService.FormatId(business_id, t.ext_id);
                var d = string.IsNullOrWhiteSpace(t.thread_id) ? null : t.thread_id.Split('_');
                var tidn = d != null && d.Length > 0 ? d.Last() : "";
                t.thread_id = ThreadService.FormatId(business_id, tidn);
                if (t.timestamp > 9999999999 || id != t.id || t.timestamp < 99999999)
                {
                    count++;
                    if (t.timestamp > 9999999999)
                    {
                        t.timestamp = t.timestamp / 1000;
                    }
                    if (t.timestamp < 99999999)
                    {
                        t.timestamp = t.timestamp * 1000;
                    }

                    var job = BackgroundJob.Enqueue<MessageService>(x => x.CopyMessageToRealtimeDB(business_id, t.id, t.timestamp));

                    BackgroundJob.ContinueWith<MessageService>(job, x => x.Delete(business_id, id));

                    if (!string.IsNullOrWhiteSpace(tid))
                    {
                        BackgroundJob.ContinueWith<MessageService>(job, x => x.Delete(business_id, tid, id));
                    }
                }
            }

            return count;
        }


        [HttpGet("fixmessagetimestamp/{business_id}")]
        public int FixMessageTimestamp(string business_id, [FromQuery]string access_token)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;

            foreach (var t in _messageService.All(business_id, new Paging { Limit = 10 }).Result)
            {
                if (t.timestamp > 9999999999)
                {
                    count++;
                    t.timestamp = t.timestamp / 1000;
                    BackgroundJob.Enqueue<MessageService>(x => x.CopyMessageToRealtimeDB(business_id, t.id, t.timestamp));
                }
            }

            //foreach (var t in _threadService.All(business_id).Result)
            //{
            //    foreach (var m in _messageService.GetByThread(business_id, new Paging { Limit = 10000 }, t.id).Result)
            //    {
            //        var msg = m.Object;
            //        if (msg.timestamp > 9999999999)
            //        {
            //            count++;
            //            msg.timestamp = msg.timestamp / 1000;
            //            _messageService.SendMessageToThread(business_id, msg, t.id);
            //        }
            //    }

            //}
            return count;
        }


        [HttpGet("get/{business_id}/{id}")]
        public ThreadModel GetById(string business_id, string id)
        {
            return new ThreadModel(_threadService.GetById(business_id, id));
        }


        [HttpGet("list/{business_id}/{customer_id}")]
        public async Task<ThreadFeed> GetThreadsByCustomer(string business_id, string customer_id, [FromQuery]int limit = 2)
        {
            var data = await _threadService.GetByCustomer(business_id, customer_id, new Paging { Limit = limit });

            return new ThreadFeed { Data = data == null ? new List<ThreadModel>() : data.Select(t => new ThreadModel(t)) };
        }

        [HttpPost("search/{business_id}")]
        public dynamic SearchThreads(string business_id, [FromQuery]string keywords)
        {
            var data = _threadService.GetThreads(business_id, "", "", "", "", keywords, new Paging { Limit = 20 }).Result;
            return new { customers = data == null ? new List<ThreadModel>() : data.Select(t => new ThreadModel(t)) };
        }

        [HttpGet("list/{business_id}")]
        public ThreadFeed GetThreads(string business_id, [FromQuery]ThreadFilter filter)
        {
            if (filter.status == "unread")
            {
                filter.status = "";
                filter.flag = "unread";
            }
            if (filter.status == "nonreply")
            {
                filter.status = "";
                filter.flag = "nonreply";
            }
            var data = _threadService.GetThreads(business_id, filter.channel_id == "all" ? "" : filter.channel_id, filter.agent_id == "all" ? "" : filter.agent_id, filter.status, filter.flag, filter.search ?? "", new Paging { Limit = filter.quantity, Next = (filter.first == 0 ? 9999999999 : filter.first).ToString() }).Result;

            return new ThreadFeed { Data = data == null ? new List<ThreadModel>() : data.Select(t => new ThreadModel(t)) };
        }


        [HttpPost("last_visit")]
        public bool SaveLastVisit([FromBody]LastVisit data)
        {
            try
            {
                Task<bool>.Factory.StartNew(() =>
                {
                    _threadService.AddLastVisit(data.business_id, data.thread_id, data.url, data.agent_id);
                    return true;
                });

                return true;
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    category = "Threads",
                    link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
                    message = ex.Message,
                    name = string.Format("Add last_visit thread-{0} to agent-{1}", data.thread_id, data.agent_id)
                });
                throw ex;
            }
        }


        [HttpPost("assign/{business_id}/{agent_id}/{thread_id}")]
        public bool AssignToAgent(string business_id, string agent_id, string thread_id)
        {
            try
            {
                return _threadService.AssignToAgent(business_id, thread_id, agent_id);
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    category = "Threads",
                    link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
                    message = ex.Message,
                    name = string.Format("Assign thread-{0} to agent-{1}", thread_id, agent_id)
                });
                throw ex;
            }
        }

        [HttpPost("unassign/{business_id}/{thread_id}")]
        public bool UnAssignFromAgent(string business_id, string thread_id)
        {
            try
            {
                var thread = _threadService.UnAssignFromAgent(business_id, thread_id);
                return thread != null;
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    category = "Threads",
                    link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
                    message = ex.Message,
                    name = string.Format("UnAssign thread-{0} from agent", thread_id)
                });
                throw ex;
            }
        }

        [HttpPost("read/{business_id}/{thread_id}")]
        public ApiResponse MarkAsRead(string business_id, string thread_id, [FromQuery]string agent_id)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var thread = _threadService.MarkAsRead(business_id, thread_id, agent_id);
                if (thread != null)
                {
                    BackgroundJob.Enqueue<CustomerService>(x => x.UpdateCustomerStatusAccordingToThread(business_id, thread.customer_id, thread_id, true));
                }
                response.ok = true;
                return response;
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    category = "Threads",
                    link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
                    message = ex.Message,
                    name = string.Format("Mark thread-{0} as read", thread_id)
                });
                throw ex;
            }
        }

        [HttpPost("unread/{business_id}/{thread_id}")]
        public ApiResponse MarkAsUnRead(string business_id, string thread_id, [FromQuery]string agent_id)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var thread = _threadService.MarkAsUnRead(business_id, thread_id, agent_id);
                if (thread != null)
                {
                    BackgroundJob.Enqueue<CustomerService>(x => x.UpdateCustomerStatusAccordingToThread(business_id, thread.customer_id, thread_id, true));
                }
                response.ok = true;
                return response;
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    category = "Threads",
                    link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
                    message = ex.Message,
                    name = string.Format("Mark thread-{0} as unread", thread_id)
                });
                throw ex;
            }
        }


        [HttpGet("delete/{thread_id}")]
        public int DeleteThread(string thread_id, [FromQuery]string business_id, [FromQuery]string access_token)
        {
            int count = 0;
            if (access_token != "@bazavietnam") return count;
            if (!string.IsNullOrWhiteSpace(business_id) && !string.IsNullOrWhiteSpace(thread_id))
            {
                count = _threadService.Delete(business_id, thread_id);
            }
            return count;
        }

        private string GetInboxLink(string business_id, Thread thread)
        {
            string link = "";
            try
            {
                if (!string.IsNullOrWhiteSpace(thread.ext_id))
                {
                    string conversation_id = ConversationService.FormatId(business_id, thread.ext_id);
                    var conversation = _conversationService.GetById(business_id, conversation_id);
                    if (conversation != null && !string.IsNullOrWhiteSpace(conversation.link)) link = "https://business.facebook.com" + conversation.link;
                }
            }
            catch { }
            if (string.IsNullOrWhiteSpace(link))
            {
                link = string.Format("https://business.facebook.com/{0}/messages/?folder=inbox&section=messages", thread.channel_ext_id);
            }
            return link;
        }

        [HttpGet("link_inbox/{business_id}/{id}")]
        public ApiResponse LinkFacebookInbox(string business_id, string id)
        {
            ApiResponse response = new ApiResponse();
            string link = GetInboxLink(business_id, _threadService.GetById(business_id, id));
            response.ok = !string.IsNullOrWhiteSpace(link);
            response.data = link;

            return response;
        }


        private string GetPostLink(string business_id, string channel_id, string postId, long timestamp)
        {
            string link = "";
            //var channel = _channelService.GetById(business_id, channel_id);

            //if (!string.IsNullOrWhiteSpace(channel_id) && !string.IsNullOrWhiteSpace(postId))
            //    link = _facebookCommentService.GetFacebookPostLink(postId, channel.token);
            //if (string.IsNullOrWhiteSpace(link))
            //{
            //    link = string.Format("https://business.facebook.com/{0}", channel.ext_id);
            //}
            link = string.Format("https://business.facebook.com/{0}", postId);
            return link;
        }


        [HttpGet("link_post/{business_id}")]
        public ApiResponse OpenLink(string business_id, [FromQuery]string channel_id, [FromQuery]string postId, [FromQuery]long timestamp)
        {
            ApiResponse response = new ApiResponse();
            string link = GetPostLink(business_id, channel_id, postId, timestamp);
            response.ok = !string.IsNullOrWhiteSpace(link);
            response.data = link;

            return response;
        }

        [HttpGet("profile/{business_id}/{id}")]
        public async Task<ThreadProfileModel> GetThreadProfile(string business_id, string id)
        {
            ThreadProfileModel model = new ThreadProfileModel();
            //var key = "GetThreadProfile" + business_id + id;
            //var lst = await CacheBase.cacheManagerGet<ThreadProfileModel>(key);
            //if (lst != null)
            //    return lst;

            var thread = _threadService.GetById(business_id, id);
            var channel = _channelService.GetById(business_id, thread.channel_id);
            try
            {
                if (channel.type != "facebook")
                {
                    return model;
                }
            } catch{}

            try
            {
                model.last_visits = string.IsNullOrWhiteSpace(thread.last_visits) ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(thread.last_visits);
                model.post = null;
                try
                {
                    if (thread.link_ext_id != null)
                    {
                        var pid = thread.link_ext_id.Contains('_') ? thread.link_ext_id : string.Format("{0}_{1}", thread.channel_ext_id, thread.link_ext_id);

                        //var key1 = "GetThreadProfile_link_ext_id" + pid;
                        //var lst1 = await CacheBase.cacheManagerGet<PostModel>(key1);
                        //if (lst1 != null || (lst1 == null && await CacheBase.cacheCheckExist(key1)))
                        //    model.post = lst1;
                        //else
                        //{

                        //var options = new FindOptions<FacebookPost>();
                        ////options.Projection = "{_id: 0}";
                        //options.Limit = 1;
                        //var query = "{id:\"" + pid + "\"}";
                        //var rs = await _facebookConversationService.GetDataMongo(query, options, "Posts");
                        //FacebookPost post = null;
                        //if (rs == null || rs.Count == 0)
                        //{
                           var post = await _facebookService.GetPost(pid, channel.token);
                            //if (post != null)
                            //{
                            //    var option = new UpdateOptions { IsUpsert = true };
                            //    post._id = pid;
                            //    var filter = Builders<FacebookPost>.Filter.Where(x => x.id == pid);
                            //    _facebookConversationService.UpsertAnyMongo<FacebookPost>(post, option, filter, "Posts");
                            //}
                        //}
                        //else post = rs[0];
                        FacebookAttachment a = post.attachments != null && post.attachments.data != null && post.attachments.data.Count() > 0 && post.attachments.data.First() != null ? post.attachments.data.First() : null;

                        model.post = new PostModel
                        {
                            id = post.id,
                            link = post.link,
                            message = post.message,
                            picture = post.picture,
                            parent_id = post.parent_id,
                            permalink_url = post.permalink_url,
                            source = post.source,
                            type = post.type,
                            page_name = channel.name,
                            created_time = post.created_time,
                            updated_time = post.updated_time,
                            attachments = a != null && a.subattachments != null && a.subattachments.data != null && a.subattachments.data != null ? a.subattachments.data : new List<FacebookAttachment>()
                        };
                        //    CacheBase.cacheManagerSetForProceduce(key1, model.post, DateTime.Now.AddDays(1), null, null, false, "select", false);
                        //}
                    }
                }
                catch
                {
                }
                if (model.post == null)
                {
                    var cid = thread.owner_last_message_ext_id.Contains('_') ? thread.owner_last_message_ext_id : string.Format("{0}_{1}", thread.channel_ext_id, thread.owner_last_message_ext_id);

                    //var key2 = "GetThreadProfile_owner_last_message_ext_id" + cid;
                    //var lst2 = await CacheBase.cacheManagerGet<PostModel>(key2);
                    //if (lst2 != null || (lst2 == null && await CacheBase.cacheCheckExist(key2)))
                    //    model.post = lst2;
                    //else
                    //{

                    //var options = new FindOptions<FacebookComment>();
                    ////options.Projection = "{_id: 0}";
                    //options.Limit = 1;
                    //var query = "{id:\"" + cid + "\"}";
                    //var rs = await _facebookConversationService.GetDataMongo(query, options, "Posts");
                    //FacebookComment comment = null;
                    //if (rs == null || rs.Count == 0)
                    //{
                       var comment = await _facebookService.GetComment(cid, channel.token);
                        //if (comment != null)
                        //{
                        //    var option = new UpdateOptions { IsUpsert = true };
                        //    comment._id = cid;
                        //    var filter = Builders<FacebookComment>.Filter.Where(x => x.id == cid);
                        //    _facebookConversationService.UpsertAnyMongo<FacebookComment>(comment, option, filter, "Posts");
                        //}
                        //}
                    //}
                    //else comment = rs[0];

                    if (comment != null && comment.@object != null && !string.IsNullOrWhiteSpace(comment.@object.id))
                    {
                        var wid = comment.@object.id.Contains('_') ? comment.@object.id : string.Format("{0}_{1}", thread.channel_ext_id, comment.@object.id);
                        var website = await _facebookService.GetWebsite(wid, channel.token);
                        if (website != null)
                        {
                            model.post = new PostModel { id = comment.id, parent_id = website.id, page_name = channel.name, link = website.application.url, message = website.title, picture = website.image != null && website.image.Count() > 0 ? website.image.First().url : "", created_time = comment.created_time, updated_time = comment.created_time };
                        }
                        //}
                        //CacheBase.cacheManagerSetForProceduce(key2, model.post, DateTime.Now.AddDays(1), null, null, false, "select", false);
                    }
                }

            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Thread",
                    link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
                    details = JsonConvert.SerializeObject(ex.StackTrace),
                    name = string.Format("Get thread profile for thread id: {0}", id)
                });
            }
            // CacheBase.cacheManagerSetForProceduce(key, model, DateTime.Now.AddMinutes(10), null, null, false);

            return model;
        }


    }
}