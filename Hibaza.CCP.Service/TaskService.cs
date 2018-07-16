using Hibaza.CCP.Data.Infrastructure;
using Hibaza.CCP.Data.Repositories;
using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hibaza.CCP.Domain.Models;
using Hangfire;

namespace Hibaza.CCP.Service
{
    public class TaskService : ITaskService
    {
        private readonly IConversationService _conversationService;
        private readonly IAgentService _agentService;
        private readonly ICustomerService _customerService;
        private readonly IChannelService _channelService;
        public TaskService(ICustomerService customerService, IChannelService channelService, IConversationService conversationService, IAgentService agentService)
        {
            _customerService = customerService;
            _conversationService = conversationService;
            _channelService = channelService;
            _agentService = agentService;
        }

        public async Task<int> AutoAssignToAvailableAgents(string business_id, Paging page)
        {
            var count = await _customerService.AutoAssignToAvailableAgents(business_id, page);
            BackgroundJob.Enqueue<CustomerService>(x => x.BatchUpdateUnreadCounters(business_id));
            return count;
        }


        public async Task<int> SetBusyAllInActivityAgents(string business_id, int minutes)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                _agentService.SetBusyAllInActivityAgents(business_id, minutes);                
            });
            return 0;
           // await AutoAssignToAvailableAgents(business_id,null);
           
        }

        public async Task<int> LogoutAllInActivityAgents(string business_id, int minutes)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                _agentService.LogoutAllInActivityAgents(business_id, minutes);
            });
            //await AutoAssignToAvailableAgents(business_id, null);
            return 0;

        }
    }
}
