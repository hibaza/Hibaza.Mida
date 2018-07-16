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

namespace Hibaza.CCP.Api.Controllers
{

    [Route("jobs")]
    public class JobController : Controller
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
        public JobController(IThreadService threadService, IFacebookConversationService facebookConversationService, ICustomerService customerService, IConversationService conversationService, IBusinessService businessService, IAgentService agentService, IMessageService messageService, IFacebookService facebookService, IChannelService channelService, IOptions<AppSettings> appSettings, ILoggingService logService)
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

        [HttpGet("autoupdatecusomteridnull")]
        public int AutoUpdateCustomerIdNull([FromQuery]int minutes, [FromQuery]string access_token)
        {
            if (access_token != "@bazavietnam") return 0;
         //_facebookConversationService.AutoUpdateCustomerIdNull();
            RecurringJob.AddOrUpdate<FacebookConversationService>("AutoUpdateCustomerIdNull", x => x.AutoUpdateCustomerIdNull(), Cron.MinuteInterval(minutes));
            return 1;
        }

        [HttpGet("autoUpdateReferralsCusomterIdNull")]
        public int autoUpdateReferralsCusomterIdNull([FromQuery]int minutes, [FromQuery]string access_token)
        {
            if (access_token != "@bazavietnam") return 0;
            // _facebookConversationService.AutoUpdateCustomerIdNull(business_id, limit);
            RecurringJob.AddOrUpdate<FacebookConversationService>("autoUpdateReferralsCusomterIdNull", x => x.autoUpdateReferralsCusomterIdNull(), Cron.MinuteInterval(minutes));
            return 1;
        }

        [HttpGet("autoUpdateThreadsCusomterIdNull")]
        public int autoUpdateThreadsCusomterIdNull([FromQuery]int minutes,[FromQuery]string access_token)
        {
            if (access_token != "@bazavietnam") return 0;
             //_facebookConversationService.autoUpdateThreadsCusomterIdNull();
            RecurringJob.AddOrUpdate<FacebookConversationService>("autoUpdateThreadsCusomterIdNull", x => x.autoUpdateThreadsCusomterIdNull(), Cron.MinuteInterval(minutes));
            return 1;
        }
        

        [HttpGet("autoCallProcedureMinutes")]
        public int autoCallProcedureMinutes([FromQuery]string procedure, [FromQuery]string jobName, [FromQuery]int minutes, [FromQuery]string access_token)
        {
            if (access_token != "@bazavietnam") return 0;
            RecurringJob.AddOrUpdate<ThreadService>(jobName, x => x.Job_AutoCallProcedure(procedure), Cron.MinuteInterval(minutes));
            return 1;
        }

        //[HttpGet("autoFixAllTicket")]
        //public int autoFixAllTicket([FromQuery]string procedure, [FromQuery]string jobName, [FromQuery]int minutes, [FromQuery]string access_token)
        //{
        //    if (access_token != "@bazavietnam") return 0;
        //    _
        //    return 1;
        //}
    }
}