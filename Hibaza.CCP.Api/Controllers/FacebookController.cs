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
using System.Web.Http;
using System.Net;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Primitives;
using Hibaza.CCP.Domain.Models.Facebook;
using Firebase.Storage;
using System.IO;
using Hangfire;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Text;

namespace Hibaza.CCP.Api.Controllers
{

    [Route("facebook")]
    public class FacebookController : Controller
    {
        public IConversationService _conversationService;
        public IFacebookConversationService _fbConversationService;
        public IFacebookCommentService _fbCommentService;
        public IFacebookService _facebookService;
        public INodeService _nodeService;
        public IChannelService _channelService;
        public IMessageService _messageService;
        private readonly IThreadService _threadService;
        private readonly IBusinessService _businessService;
        private readonly ILoggingService _logService;
        private readonly AppSettings _appSettings;
        public FacebookController(IConversationService conversationService, ILoggingService logService, IFacebookService facebookService, INodeService nodeService, IBusinessService businessService, IThreadService threadService, IFacebookCommentService fbCommentService, IFacebookConversationService fbConversationService, IChannelService channelService, IMessageService messageService, IOptions<AppSettings> appSettings)
        {
            _conversationService = conversationService;
            _fbConversationService = fbConversationService;
            _fbCommentService = fbCommentService;
            _facebookService = facebookService;
            _nodeService = nodeService;
            _channelService = channelService;
            _messageService = messageService;
            _threadService = threadService;
            _businessService = businessService;
            _logService = logService;
            _appSettings = appSettings.Value;
        }

        [HttpGet("autodownload")]
        public int AutoDownloadConversations([FromQuery]string access_token, [FromQuery]int minutes = 5, [FromQuery]int offset = 10, [FromQuery]bool real_time_update = true)
        {
            int count = 0;
            if (access_token != "@bazavietnam") return count;
            foreach (var b in _businessService.GetBusinesses(0, 100).Result.Where(b => b.active))
            {
                string business_id = b.id;
                RecurringJob.AddOrUpdate<FacebookConversationService>("DownloadConversationsForBusiness[" + business_id + "]", x => x.AutoDownloadConversations(business_id, offset, real_time_update), Cron.MinuteInterval(minutes));
                count++;
            }
            return count;
        }

        [HttpGet("downloadsinglepost/{pageId}/{postId}")]
        public async System.Threading.Tasks.Task<int> GetSinglePost(string pageId, string postId, [FromQuery]bool real_time_update, [FromQuery]string access_token, [FromQuery]int limit = 100)
        {
            int count = 0;
            if (access_token != "@bazavietnam") return count;
            try
            {
                await _fbCommentService.DownloadSinglePost(pageId, postId, limit, real_time_update);
            }
            catch (Exception ex)
            {
                var e = ex;
            }
            return count;
        }

        [HttpGet("downloadposts/{business_id}")]
        public async System.Threading.Tasks.Task<int> DownloadPosts(string business_id, [FromQuery]string access_token, [FromQuery]string channel_id, [FromQuery]int skip, [FromQuery]int days, [FromQuery]bool real_time_update)
        {
            int count = 0;
            if (access_token != "@bazavietnam") return count;

            List<Channel> list = new List<Channel>();
            if (!string.IsNullOrWhiteSpace(channel_id))
            {
                list.Add(_channelService.GetById(business_id, channel_id));
            }
            else
            {
                list = _channelService.GetChannels(business_id, 0, 2000).Result.Where(a => a.active).ToList();
            }

            foreach (var bc in list)
            {
                count++;
                //count += await _fbCommentService.DownloadPosts(bc, "promotable_posts", "", "", DateTime.UtcNow.AddDays(-skip - days), DateTime.UtcNow.AddDays(-skip), real_time_update);
                BackgroundJob.Enqueue<FacebookCommentService>(x => x.DownloadPosts(bc, "promotable_posts", "", "", DateTime.UtcNow.AddDays(-skip - days), DateTime.UtcNow.AddDays(-skip), real_time_update));
            }
            return count;
        }

        [HttpGet("downloadcomments/{business_id}")]
        public async System.Threading.Tasks.Task<int> DownloadComments(string business_id, [FromQuery]string access_token, [FromQuery]string channel_id, [FromQuery]int skip, [FromQuery]int days, [FromQuery]bool real_time_update, [FromQuery]int limit = 100)
        {
            int count = 0;
            if (access_token != "@bazavietnam") return count;
            List<Channel> list = new List<Channel>();
            if (!string.IsNullOrWhiteSpace(channel_id))
            {
                list.Add(_channelService.GetById(business_id, channel_id));
            }
            else
            {
                list = _channelService.GetChannels(business_id, 0, 2000).Result.Where(a => a.active).ToList();
            }

            foreach (var bc in list)
            {
                //count += await _fbCommentService.DownloadComments(bc, "", limit, Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow.AddDays(-skip - days)), Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow.AddDays(-skip)), real_time_update);
                BackgroundJob.Enqueue<FacebookCommentService>(x => x.DownloadComments(bc, "", limit, Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow.AddDays(-skip - days)), Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow.AddDays(-skip)), real_time_update));
            }
            return count;
        }

        [HttpGet("downloadsingleconversation/{pageId}/{conversationId}")]
        public async System.Threading.Tasks.Task<int> DownloadSingleConversation(string pageId, string conversationId, [FromQuery]bool real_time_update, [FromQuery]string access_token, [FromQuery]int limit = 100)
        {
            int count = 0;
            if (access_token != "@bazavietnam") return count;
            await _fbConversationService.DownloadSingleConversation("", "", pageId, conversationId, limit, real_time_update);
            return count;
        }

        [HttpGet("downloadconversations/{business_id}")]
        public async System.Threading.Tasks.Task<int> DownloadConversations(string business_id, [FromQuery]string access_token, [FromQuery]string channel_id, [FromQuery]int skip, [FromQuery]int days, [FromQuery]bool real_time_update)
        {
            int count = 0;
            if (access_token != "@bazavietnam") return count;
            List<Channel> list = new List<Channel>();
            if (!string.IsNullOrWhiteSpace(channel_id))
            {
                list.Add(_channelService.GetById(business_id, channel_id));
            }
            else
            {
                list = _channelService.GetChannels(business_id, 0, 2000).Result.Where(a => a.active).ToList();
            }
            foreach (var bc in list)
            {
                await _fbConversationService.DownloadConversations(bc, "", "", DateTime.UtcNow.AddDays(-skip - days), DateTime.UtcNow.AddDays(-skip), real_time_update);
               // BackgroundJob.Enqueue<FacebookConversationService>(x => x.DownloadConversations(bc, "", "", DateTime.UtcNow.AddDays(-skip - days), DateTime.UtcNow.AddDays(-skip), real_time_update));
                count++;
            }
            return count;
        }

        [HttpGet("downloadmessages/{business_id}")]
        public async System.Threading.Tasks.Task<int> DownloadMessages(string business_id, [FromQuery]string access_token, [FromQuery]string channel_id, [FromQuery]int skip, [FromQuery]int days, [FromQuery]bool real_time_update, [FromQuery]int limit = 100)
        {
            int count = 0;
            if (access_token != "@bazavietnam") return count;

            List<Channel> list = new List<Channel>();
            if (!string.IsNullOrWhiteSpace(channel_id))
            {
                list.Add(_channelService.GetById(business_id, channel_id));
            }
            else
            {
                list = _channelService.GetChannels(business_id, 0, 2000).Result.Where(a => a.active).ToList();
            }

            foreach (var bc in list)
            {
                count++;
                //count += await _fbConversationService.DownloadMessages(bc, limit, Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow.AddDays(-skip - days)), Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow.AddDays(-skip)), real_time_update);
                //_fbConversationService.DownloadMessages(bc, limit, Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow.AddDays(-skip - days)), Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow.AddDays(-skip)), real_time_update);
                BackgroundJob.Enqueue<FacebookConversationService>(x => x.DownloadMessages(bc, limit, Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow.AddDays(-skip - days)), Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow.AddDays(-skip)), real_time_update));
            }

            return count;
        }

        [HttpGet("test")]
        public IActionResult Test()
        {

            //var para = "{\"object\":\"page\",\"entry\":[{\"id\":\"646263132231649\",\"changed_fields\":null,\"changes\":null,\"time\":1512986534517,\"messaging\":[{\"sender\":{\"name\":null,\"email\":null,\"phone\":null,\"avatar\":null,\"sex\":null,\"_id\":null,\"id\":\"1797471200293431\"},\"recipient\":{\"name\":null,\"email\":null,\"phone\":null,\"avatar\":null,\"sex\":null,\"_id\":null,\"id\":\"646263132231649\"},\"timestamp\":1512986534181,\"message\":{\"mid\":\"mid.$cAAIRPcmmqgRmdkupJVgRQZg7MFoe\",\"text\":\"hhhh\",\"attachments\":[{\"type\":\"fallback\",\"payload\":null}]},\"referral\":null,\"postback\":null,\"delivery\":null}]}]}";
            //var para = "{\"object\":\"page\",\"entry\":[{\"id\":\"646263132231649\",\"changed_fields\":null,\"changes\":[{\"field\":\"feed\",\"value\":{\"item\":\"comment\",\"thread_id\":null,\"page_id\":null,\"sender_name\":\"Số nhà 15 ngõ 14 đường phan đình giót hà đông hà nội\",\"photo\":null,\"comment_id\":\"1324271341015495_1402561653186463\",\"sender_id\":\"646263132231649\",\"post_id\":\"646263132231649_1324271341015495\",\"verb\":\"add\",\"parent_id\":\"1324271341015495_1402561339853161\",\"created_time\":1513009554,\"message\":\"d\"}}],\"time\":1513009556,\"messaging\":null}]}";
            //var para = "{\"object\":\"page\",\"entry\":[{\"id\":\"646263132231649\",\"changed_fields\":null,\"changes\":[{\"field\":\"feed\",\"value\":{\"item\":\"comment\",\"thread_id\":null,\"page_id\":null,\"sender_name\":\"Số nhà 15 ngõ 14 đường phan đình giót hà đông hà nội\",\"photo\":null,\"comment_id\":\"1324271341015495_1402287396547222\",\"sender_id\":\"646263132231649\",\"post_id\":\"646263132231649_1324271341015495\",\"verb\":\"hide\",\"parent_id\":\"646263132231649_1324271341015495\",\"created_time\":1512986617,\"message\":\"jjjj\"}}],\"time\":1512986617,\"messaging\":null}]}";
            //  var para = "{\"object\":\"page\",\"entry\":[{\"id\":\"481230428743675\",\"changed_fields\":null,\"changes\":null,\"time\":1528711518069,\"messaging\":[{\"sender\":{\"name\":null,\"email\":null,\"phone\":null,\"avatar\":null,\"sex\":null,\"_id\":null,\"id\":\"1701582769938724\"},\"recipient\":{\"name\":null,\"email\":null,\"phone\":null,\"avatar\":null,\"sex\":null,\"_id\":null,\"id\":\"481230428743675\"},\"timestamp\":1528711517941,\"message\":{\"mid\":\"mid.$cAAHvWLvlhdRqH5Pe31j7k6SJ2rKn8\",\"text\":\"Chị nhầm ....\",\"attachments\":null},\"referral\":null,\"postback\":null,\"delivery\":null}]}]}";
            //var para = "{\"object\":\"page\",\"entry\":[{\"id\":\"310467822386873\",\"changed_fields\":null,\"changes\":null,\"time\":1528712205400,\"messaging\":[{\"sender\":{\"name\":null,\"email\":null,\"phone\":null,\"avatar\":null,\"sex\":null,\"_id\":null,\"id\":\"310467822386873\"},\"recipient\":{\"name\":null,\"email\":null,\"phone\":null,\"avatar\":null,\"sex\":null,\"_id\":null,\"id\":\"1656222777760984\"},\"timestamp\":1528712205317,\"message\":{\"mid\":\"mid.$cAAFAqXN8BMJqH55cBFj7lkTHEfXy1\",\"text\":\"Dạ sản phẩm bên em đã giao đến anh, có thể do thứ 7, CN bưu cục họ không làm việc nên có thể anh nhận được hàng trong ngày mai, anh nhé!\",\"attachments\":null},\"referral\":null,\"postback\":null,\"delivery\":null}]}]}";
            //var para = "{\"object\":\"page\",\"entry\":[{\"id\":\"481230428743675\",\"changed_fields\":null,\"changes\":[{\"field\":\"feed\",\"value\":{\"item\":\"comment\",\"thread_id\":null,\"page_id\":null,\"sender_name\":\"Nguyen An\",\"photo\":null,\"comment_id\":\"749723241894391_758849584315090\",\"sender_id\":\"368155696945876\",\"post_id\":\"481230428743675_749723241894391\",\"verb\":\"add\",\"parent_id\":\"481230428743675_749723241894391\",\"created_time\":1511374305,\"message\":\"Xin hoi sohp áo này giá bn\"}}],\"time\":1511374307,\"messaging\":null}]}";

            //var para = "{\"object\":\"page\",\"entry\":[{\"id\":\"310467822386873\",\"changed_fields\":null,\"changes\":null,\"time\":1512004704412,\"messaging\":[{\"sender\":{\"name\":null,\"email\":null,\"phone\":null,\"avatar\":null,\"sex\":null,\"_id\":null,\"id\":\"310467822386873\"},\"recipient\":{\"name\":null,\"email\":null,\"phone\":null,\"avatar\":null,\"sex\":null,\"_id\":null,\"id\":\"1077496735686642\"},\"timestamp\":1512004704374,\"message\":{\"mid\":\"mid.$cAAFAqCbUYzBmO8YgdlgCoDXb06-I\",\"text\":\"Quần âu nam ống đứng túi sau bo viền Myknow\\r\\nMã sản phẩmKP007-06\",\"attachments\":null},\"referral\":null,\"postback\":null,\"delivery\":null}]}]}";
            //var para = "{\"object\":\"page\",\"entry\":[{\"id\":\"310467822386873\",\"changed_fields\":null,\"changes\":null,\"time\":1512222770382,\"messaging\":[{\"sender\":{\"name\":null,\"email\":null,\"phone\":null,\"avatar\":null,\"sex\":null,\"_id\":null,\"id\":\"1545899698808675\"},\"recipient\":{\"name\":null,\"email\":null,\"phone\":null,\"avatar\":null,\"sex\":null,\"_id\":null,\"id\":\"310467822386873\"},\"timestamp\":1512222770034,\"message\":{\"mid\":\"mid.$cAAFAqn7Y20pmSMWLclgF4BC6HVTq\",\"text\":null,\"attachments\":[{\"type\":\"image\",\"payload\":{\"url\":\"https://scontent.xx.fbcdn.net/v/t39.1997-6/851557_369239266556155_759568595_n.png?_nc_ad=z-m&_nc_cid=0&oh=9058fb52f628d0a6ab92f85ea310db0a&oe=5A9DAADC\",\"is_reusable\":false,\"attachment_id\":null,\"template_type\":null,\"elements\":null}}]},\"referral\":null,\"postback\":null,\"delivery\":null}]}]}";
            //var para = "{\"object\":\"page\",\"entry\":[{\"id\":\"310467822386873\",\"changed_fields\":null,\"changes\":null,\"time\":1513081039430,\"messaging\":[{\"sender\":{\"name\":null,\"email\":null,\"phone\":null,\"avatar\":null,\"sex\":null,\"_id\":null,\"id\":\"1542224662525137\"},\"recipient\":{\"name\":null,\"email\":null,\"phone\":null,\"avatar\":null,\"sex\":null,\"_id\":null,\"id\":\"310467822386873\"},\"timestamp\":1513081039107,\"message\":{\"mid\":\"mid.$cAAFAq-ws-ZZme-2xA1gSqhm01OUG\",\"text\":\"Có áo phao mình khg shop\",\"attachments\":null},\"referral\":null,\"postback\":null,\"delivery\":null}]}]}";
            // var para = "{\"object\":\"page\",\"entry\":[{\"id\":\"140691416376122\",\"changed_fields\":null,\"changes\":null,\"time\":1512672140470,\"messaging\":[{\"sender\":{\"name\":null,\"email\":null,\"phone\":null,\"avatar\":null,\"sex\":null,\"_id\":null,\"id\":\"1647625871955406\"},\"recipient\":{\"name\":null,\"email\":null,\"phone\":null,\"avatar\":null,\"sex\":null,\"_id\":null,\"id\":\"140691416376122\"},\"timestamp\":1512672139905,\"message\":{\"mid\":\"mid.$cAAAlAIK7SqFmY45igVgMkkZIt0Lt\",\"text\":\"Tôi quan tâm đến mặt hàng này.\",\"attachments\":[{\"type\":\"fallback\",\"payload\":null}]},\"referral\":null,\"postback\":null,\"delivery\":null}]}]}";
            //var para = "{\"object\":\"page\",\"entry\":[{\"id\":\"284410088612960\",\"changed_fields\":null,\"changes\":null,\"time\":1512705328862,\"messaging\":[{\"sender\":{\"name\":null,\"email\":null,\"phone\":null,\"avatar\":null,\"sex\":null,\"_id\":null,\"id\":\"1555087657940708\"},\"recipient\":{\"name\":null,\"email\":null,\"phone\":null,\"avatar\":null,\"sex\":null,\"_id\":null,\"id\":\"284410088612960\"},\"timestamp\":1512705328678,\"message\":{\"mid\":\"mid.$cAAFYXgCsgxNmZYjOJlgNENOmpULj\",\"text\":null,\"attachments\":[{\"type\":\"image\",\"payload\":{\"url\":\"https://scontent-iad3-1.xx.fbcdn.net/v/t39.1997-6/851557_369239266556155_759568595_n.png?_nc_ad=z-m&_nc_cid=0&oh=0ef8de1568b7089f8197a3edf15d8d71&oe=5AC537DC\",\"is_reusable\":false,\"attachment_id\":null,\"template_type\":null,\"elements\":null}}]},\"referral\":null,\"postback\":null,\"delivery\":null}]}]}";
            //var para = "{\"object\":\"page\",\"entry\":[{\"id\":\"646263132231649\",\"changed_fields\":null,\"changes\":null,\"time\":1512557911258,\"messaging\":[{\"sender\":{\"name\":null,\"email\":null,\"phone\":null,\"avatar\":null,\"sex\":null,\"_id\":null,\"id\":\"1797471200293431\"},\"recipient\":{\"name\":null,\"email\":null,\"phone\":null,\"avatar\":null,\"sex\":null,\"_id\":null,\"id\":\"646263132231649\"},\"timestamp\":1512557911088,\"message\":{\"mid\":\"mid.$cAAIRPcmmqgRmXL9kKlgK3oqM7myr\",\"text\":\" can nang 50 kg cao 1m70 dt l097.9354 176 yhh\",\"attachments\":null},\"referral\":null,\"postback\":null,\"delivery\":null}]}]}";
            //  var obj = JsonConvert.DeserializeObject<FacebookWebhookData>(para);
            //  _facebookService.SaveWebhookData("", "", obj, true);
            //  _fbConversationService.Test();

            var dicConfig = new Dictionary<string, string>();
            dicConfig.Add("mongoconnect", "ConnAi");
            dicConfig.Add("mongodb", "AiDb");
            dicConfig.Add("collectionname", "");
            dicConfig.Add("type", "procedure");

            var dicPara = "GetPhoneWeightHeight('150146656_1673140716067817','0979354170')";
            var json = new Dictionary<string, object>();
            //json.Add("config",dicConfig);
            //json.Add("para", dicPara);
            json.Add("config", JsonConvert.SerializeObject(dicConfig));
            json.Add("para", JsonConvert.SerializeObject(dicPara));
            var tt = JsonConvert.SerializeObject(json);
            var ts = Core.Helpers.WebHelper.HttpPostAsync("http://localhost:64834/api/procedure/execute", json);

            var client = new HttpClient();
            var check = client.PostAsync("http://localhost:64834/api/procedure/execute",
                  new StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8, "application/json")).Result;
            var contents = check.Content.ReadAsStringAsync().Result;
            if (contents != null && contents != "null" && contents != "\"[]\"")
            {
                var val = JsonConvert.DeserializeObject<dynamic>(JsonConvert.DeserializeObject<string>(contents));
                var phone = (string)val.phone;
               
            }

           

            return Ok();
        }
        public class phoneee
        {
            public string phone { get; set; }
            public string weight { get; set; }
            public string height { get; set; }
        }
        [HttpGet("fixconversationextid")]
        public async Task<int> FixConversationExtId([FromQuery]string business_id, [FromQuery]string channel_id, [FromQuery]int limit, [FromQuery]string access_token)
        {
            if (access_token != "@bazavietnam") return 0;
            return await _fbConversationService.FixConversationExtId(business_id, channel_id, limit);
        }


        [HttpGet("webhook")]
        public IActionResult WebhoookGet(FacebookHub hub)
        {
            if (hub.mode == "subscribe" && hub.verify_token == "@bazavietnam")
            {
                return Ok(hub.challenge);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("webhook")]
        public IActionResult WebhookPost([FromBody]FacebookWebhookData obj)
        {
            try
            {
                //add remove like image
                //var p = JsonConvert.SerializeObject(obj);
                //p = p.Replace(_appSettings.BaseUrls.Api, "");
                //obj = JsonConvert.DeserializeObject<FacebookWebhookData>(p);
                _logService.Create(new Log { name = "Webhook2", message = "Webhook3", details = JsonConvert.SerializeObject(obj) });
                _facebookService.SaveWebhookData("", "", obj, true);
                return Ok();
            }
            catch (Exception ex)
            {
                if (obj.@object == "page")
                {
                    DateTime now = DateTime.UtcNow;
                    Node node = new Node
                    {
                        type = "facebook.webhook",
                        created_time = now,
                        timestamp = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(now),
                        data = JsonConvert.SerializeObject(obj)
                    };
                    _nodeService.CreateNode(node);

                }
                _logService.CreateAsync(new Log { name = "Webhook calledback", message = ex.Message, details = JsonConvert.SerializeObject(ex.StackTrace) });
                throw ex;
            }
        }

        [HttpGet("downloadfromlogs/{business_id}")]
        public int DownloadFromLogs(string business_id, [FromQuery]long endAt, [FromQuery]string access_token, [FromQuery]int max, [FromQuery]int size)
        {
            int count = 0;
            if (access_token != "@bazavietnam") return count;
            string jobId = null;
            for (int i = 0; i < max; i++)
            {
                count++;
                List<FacebookWebhookData> list = new List<FacebookWebhookData>();
                var logs = _logService.GetLogs(new Paging { Limit = size, Next = endAt.ToString() });
                foreach (var bc in logs)
                {
                    try
                    {
                        FacebookWebhookData obj = JsonConvert.DeserializeObject<FacebookWebhookData>(bc.details);
                        list.Add(obj);
                    }
                    catch { }
                }
                if (string.IsNullOrWhiteSpace(jobId))
                {
                    //_facebookService.SaveWebhookData(list);
                    jobId = BackgroundJob.Enqueue<FacebookService>(x => x.SaveWebhookData(list));
                }
                else
                {
                    jobId = BackgroundJob.ContinueWith<FacebookService>(jobId, x => x.SaveWebhookData(list));
                }
                endAt = logs.Count() > 0 ? logs.LastOrDefault().key + 1 : 0;
            }

            return count;
        }


        [HttpGet("autoimportpendingnodes")]
        public int AutoImportPendingNodes([FromQuery]string access_token,  [FromQuery]string type, [FromQuery]int max, [FromQuery]int size, [FromQuery]int minutes, [FromQuery]bool real_time_update)
        {
            int count = 0;
            if (access_token != "@bazavietnam") return count;

            foreach (var b in _businessService.GetBusinesses(0, 100).Result.Where(b => b.active))
            {
                string business_id = b.id;
                RecurringJob.AddOrUpdate<FacebookService>("AutoImportPendingNodes[" + business_id + "]", x => x.BatchImportPendingNodes(business_id, "", type, max, size, real_time_update), Cron.MinuteInterval(minutes));
                count++;
            }

            return count;
        }

        [HttpGet("downloadfromnodes/{business_id}")]
        public async System.Threading.Tasks.Task<int> DownloadFromNodes(string business_id, [FromQuery]string access_token, [FromQuery]string channel_id, [FromQuery]string type, [FromQuery]int max, [FromQuery]int size, [FromQuery]bool real_time_update)
        {
            int count = 0;
            if (access_token != "@bazavietnam") return count;
            List<Channel> list = new List<Channel>();
            if (!string.IsNullOrWhiteSpace(channel_id))
            {
                list.Add(_channelService.GetById(business_id, channel_id));
            }
            else
            {
                list = _channelService.GetChannels(business_id, 0, 2000).Result.Where(a => a.active).ToList();
            }
            foreach (var bc in list)
            {
                count++;
                //_facebookService.BatchImportPendingNodes(bc.business_id, bc.id, type, max, size, real_time_update);
                BackgroundJob.Enqueue<FacebookService>(x => x.BatchImportPendingNodes(bc.business_id, bc.id, type, max, size, real_time_update));
            }
            return count;

        }

        
    }
}
