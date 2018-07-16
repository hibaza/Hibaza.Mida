using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Data.Repositories;
using Hibaza.CCP.Service;
using Hibaza.CCP.Service.Facebook;
using Hibaza.CCP.Core;
using Microsoft.Extensions.Options;
using Hibaza.CCP.Domain.Models;
using Newtonsoft.Json;
using Hibaza.CCP.Domain.Models.Facebook;
using Hangfire;
using Hibaza.CCP.Data.Repositories.Firebase;
using Hibaza.CCP.Data.Providers.Firebase;

namespace Hibaza.CCP.Api.Controllers
{

    [Route("brands/channels")]
    public class ChannelController : Controller
    {
        private readonly IChannelService _channelService;
        private readonly IMessageService _messageService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerService _userService;
        private readonly IThreadService _threadService;
        private readonly ILoggingService _logService;
        private readonly IAgentService _agentService;
        private readonly ICounterService _counterService;
        private readonly IFacebookService _facebookService;
        private readonly INodeService _nodeService;
        private readonly IOptions<AppSettings> _appSettings;
        private IBackgroundJobClient _backgroundJobClient;
        public ChannelController(IChannelService channelService, INodeService nodeService, IThreadService threadService, ICounterService counterService, ICustomerService userService, ICustomerService cusstomerService, IMessageService messageService, IFacebookService facebookService, IAgentService agentService, IOptions<AppSettings> appSettings, IBackgroundJobClient backgroundJobClient, ILoggingService logService)
        {
            _channelService = channelService;
            _threadService = threadService;
            _customerService = cusstomerService;
            _messageService = messageService;
            _facebookService = facebookService;
            _nodeService = nodeService;
            _appSettings = appSettings;
            _agentService = agentService;
            _userService = userService;
            _counterService = counterService;
            _backgroundJobClient = backgroundJobClient;
            _logService = logService;
        }

        [HttpGet("copyto/{business_id}")]
        public int CopToSQL(string business_id, [FromQuery]string access_token)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;
            var fb = new FirebaseChannelRepository(new FirebaseFactory(_appSettings));

            foreach (var t in fb.GetChannels(business_id, new Paging { Limit = 100 }).Result)
            {
                count++;
                _channelService.Create(t.Object);
            }
            return count;
        }


        [HttpGet("list/{business_id}")]
        public async System.Threading.Tasks.Task<ChannelFeed> GetChannels(string business_id, string access_token)
        {
            try
            {
                var resultData = await _channelService.GetChannels(business_id, 0, 50);
                return new ChannelFeed { Data = resultData };
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Agent",
                    link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
                    details = JsonConvert.SerializeObject(ex.StackTrace),
                    name = string.Format("Get channels by business_id: {0}", business_id)
                });
            }
            return null;
        }

        [HttpPost("update/{business_id}/{id}")]
        public string UpdateChannel(string business_id, string id, [FromForm]string page_name, [FromForm]string token)
        {
            var resultData = "";
            var data = _channelService.GetById(business_id, id);
            if (data != null)
            {

                var url = string.Format("https://graph.facebook.com/v2.8/oauth/access_token?grant_type=fb_exchange_token&client_id={0}&client_secret={1}&fb_exchange_token={2}", _appSettings.Value.ClientId, _appSettings.Value.ClientSecret, token);

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
                        category = "Channels",
                        name = "Get long lived token",
                        link = url
                    });
                }

                _facebookService.SubscribeToAppWebhook(data.ext_id, data.token);

                data.name = page_name ?? data.type + "-" + data.ext_id;
                data.token = longLivedToken ?? token;
                resultData = _channelService.Create(data);
            }
            return resultData;
        }


        [HttpPost("create/{type}/{business_id}")]
        public string CreateChannel(string type, string business_id, [FromForm]string page_id, [FromForm]string page_name, [FromForm]string token)
        {
            try
            {
                var list =  _channelService.GetChannelsByExtId(page_id).Result.Where(b => b.active).ToList();
                if (list == null || list.Count == 0)
                {
                    string longLivedToken = _facebookService.GetLonglivedToken(_appSettings.Value.ClientId, _appSettings.Value.ClientSecret, token);
                    page_name = Core.Helpers.CommonHelper.EnsureMaximumLength(page_name, 200);
                    Channel data = new Channel { id = ChannelService.FormatId(business_id, page_id), business_id = business_id, ext_id = page_id, name = string.IsNullOrWhiteSpace(page_name) ? type + page_id : page_name, active = true, type = type, token = string.IsNullOrWhiteSpace(longLivedToken) ? token : longLivedToken };


                    var resultData = _channelService.Create(data);

                    _facebookService.SubscribeToAppWebhook(page_id, data.token);
                    
                }
                else
                {
                    var data = list[0];
                    var url = string.Format("https://graph.facebook.com/v2.8/oauth/access_token?grant_type=fb_exchange_token&client_id={0}&client_secret={1}&fb_exchange_token={2}", _appSettings.Value.ClientId, _appSettings.Value.ClientSecret, token);

                    string longLivedToken = "";
                    try
                    {
                        longLivedToken = Core.Helpers.WebHelper.HttpGetAsync<dynamic>(url).Result.access_token;
                    }
                    catch(Exception ex)
                    {
                        _logService.Create(new Log
                        {
                            message = ex.Message,
                            category = "Channels",
                            name = "Get long lived token",
                            link = url
                        });
                    }
                    _facebookService.SubscribeToAppWebhook(data.ext_id, data.token);
                    data.name = page_name ?? data.type + "-" + data.ext_id;
                    data.token = longLivedToken ?? token;
                    return _channelService.Create(data);
                   // throw new Exception("Channel already added");
                }
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    category = "Channels",
                    link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
                    message = ex.Message,
                    details = ex.StackTrace,
                    name = string.Format("Create channel for facebook page-{0} of business-{1} use token={2}", page_id, business_id, token)
                });
                throw ex;
            }
            return page_id; 
        }

        [HttpGet("single/{business_id}/{id}")]
        public Channel GetById(string business_id, string id)
        {
            var resultData = _channelService.GetById(business_id, id);
            return resultData;
        }

        [HttpPost("delete/{business_id}/{id}")]
        public bool Delete(string business_id, string id)
        {
            Channel channel = _channelService.GetById(business_id, id);
            _facebookService.UnSubscribeToAppWebhook(channel.ext_id, channel.token);
            var resultData = _channelService.Delete(business_id, id);
            return resultData;
        }


        // GET api/facebook/conversations/pageid
        [HttpGet("GetFacebookConversations/{id}")]
        public Channel GetFacebookConversations(string id)
        {
            //_facebookConversationService.GetFacebookConversations(id, "", _appSettings.UserAccessToken, _appSettings.PageAccessToken);
            return null;
        }

        //// GET api/facebook/links/pageid
        //[HttpGet("GetFacebookLinks/{id}")]
        //public Channel GetFacebookLinks(string id)
        //{
        //    _facebookCommentService.GetFacebookLinks(id, "", _appSettings.Value.PageAccessToken);
        //    return null;
        //}

        //// GET api/facebook/comments/pageid
        //[HttpGet("GetFacebookComments/{id}")]
        //public Channel GetFacebookComments(string id)
        //{
        //    //_facebookCommentService.InsertFacebookComments(id, "", _appSettings.PageAccessToken);
        //    return null;
        //}



        //[HttpGet("webhook")]
        //public IActionResult WebhoookGet(FacebookHub hub)
        //{
        //    if (hub.mode == "subscribe" && hub.verify_token == "@bazavietnam")
        //    {
        //        return Ok(hub.challenge);
        //    }
        //    else
        //    {
        //        return NotFound();
        //    }
        //}

        //[HttpPost("webhook")]
        //public IActionResult WebhookPost([FromBody]FacebookWebhookData obj)
        //{
        //    try
        //    {
        //        _facebookService.SaveWebhookData("", "", obj, true);
        //    }
        //    catch (Exception ex)
        //    {
        //        if (obj.@object == "page")
        //        {
        //            DateTime now = DateTime.UtcNow;
        //            Node node = new Node
        //            {
        //                type = "facebook.webhook",
        //                created_time = now,
        //                timestamp = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(now),
        //                data = JsonConvert.SerializeObject(obj)
        //            };
        //            _nodeService.CreateNode(node);

        //        }
        //        _logService.CreateAsync(new Log { name = "Webhook calledback", message = ex.Message, details = JsonConvert.SerializeObject(ex.StackTrace) });
        //    }

        //    _logService.CreateAsync(new Log { name = "Webhook calledback", message = "Webhook", details = JsonConvert.SerializeObject(obj).ToString() });

        //    return Ok();
        //}


    }
}
