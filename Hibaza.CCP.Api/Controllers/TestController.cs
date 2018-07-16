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

namespace Hibaza.CCP.Api.Controllers
{

    [Route("test")]
    public class TestController : Controller
    {
        public IConversationService _conversationService;
        public IFacebookConversationService _fbConversationService;
        public IFacebookCommentService _fbCommentService;
        public IFacebookService _facebookService;
        public INodeService _nodeService;
        public IChannelService _channelService;
        public IMessageService _messageService;
        public ICustomerService _custormerService;

        private readonly IThreadService _threadService;
        private readonly IBusinessService _businessService;
        private readonly ILoggingService _logService;
        private readonly AppSettings _appSettings;
        public TestController( IConversationService conversationService, ILoggingService logService, IFacebookService facebookService, INodeService nodeService, IBusinessService businessService, IThreadService threadService, IFacebookCommentService fbCommentService, IFacebookConversationService fbConversationService, IChannelService channelService, IMessageService messageService, IOptions<AppSettings> appSettings, ICustomerService customerService)
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
            _custormerService = customerService;
        }

        [HttpGet("baza/{next}")]
        public int AutoDownloadConversations(string next)
        {
            long timestamp = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
            //
            var test = "{\"object\":\"page\",\"entry\":[{\"id\":\"310467822386873\",\"changed_fields\":null,\"changes\":null,\"time\":152214248394"+next+ ",\"messaging\":[{\"sender\":{\"name\":null,\"email\":null,\"phone\":null,\"avatar\":null,\"sex\":null,\"_id\":null,\"id\":\"1817333971652111\"},\"recipient\":{\"name\":null,\"email\":null,\"phone\":null,\"avatar\":null,\"sex\":null,\"_id\":null,\"id\":\"310467822386873\"},\"timestamp\":152214248385" + next + ",\"message\":{\"mid\":\"mid.$cAAFAqhyGva1omAhdjliZsMVEMIw_" + next + "\",\"text\":\"Loại này nhúng nước và chống xước đúng không" + next + "\",\"attachments\":null},\"referral\":null,\"postback\":null,\"delivery\":null}]}]}";
            //var test = "{\"object\":\"page\",\"entry\":[{\"id\":\"646263132231649\",\"changed_fields\":null,\"changes\":null,\"time\":"+(timestamp)+ ",\"messaging\":[{\"sender\":{\"name\":null,\"email\":null,\"phone\":null,\"avatar\":null,\"id\":\"1373352442742848\"},\"recipient\":{\"name\":null,\"email\":null,\"phone\":null,\"avatar\":null,\"id\":\"646263132231649\"},\"timestamp\":" + (timestamp) +",\"message\":{\"mid\":\"mid.$cAAIRNhUxkKNjmUqZpldaAcAdja3_"+timestamp+"\",\"text\":\""+next+"\",\"attachments\":null},\"referral\":null,\"postback\":null,\"delivery\":null}]}]}";

            //   var test = "{\"object\":\"page\",\"entry\":[{\"id\":\"646263132231649\",\"changed_fields\":null,\"changes\":null,\"time\":1501816950918,\"messaging\":[{\"sender\":{\"name\":null,\"email\":null,\"phone\":null,\"avatar\":null,\"id\":\"1373352442742848\"},\"recipient\":{\"name\":null,\"email\":null,\"phone\":null,\"avatar\":null,\"id\":\"646263132231649\"},\"timestamp\":1501816950918,\"message\":null,\"referral\":null,\"postback\":{\"payload\":\"GN003-03 \n Xanh than, size 3XL \n 499.000đ \n    \",\"referral\":null},\"delivery\":null}]}]}";

            var thu = JsonConvert.DeserializeObject<FacebookWebhookData>(test);
            _facebookService.SaveWebhookData("", "", thu, true);
            return 1;
        }

        [HttpGet("batchupdateunreadcounters")]
        public void BatchUpdateUnreadCounterss()
        {
         _custormerService.BatchUpdateUnreadCounters("150146656");
        }
        

    }
}
