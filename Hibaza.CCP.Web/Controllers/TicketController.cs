using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Hibaza.CCP.Web.Models;
using Hibaza.CCP.Domain.Models;
using Hibaza.CCP.Core;
using Microsoft.Extensions.Options;
using Hibaza.CCP.Domain.Entities;
using System.Dynamic;

namespace Hibaza.CCP.Web.Controllers
{
    [Route("tickets")]
    public class TicketController : Controller
    {
        private readonly IViewRenderService _viewRenderService;
        private readonly AppSettings _appSettings;

        public TicketController(IViewRenderService viewRenderService, IOptions<AppSettings> appSettings)
        {
            _viewRenderService = viewRenderService;
            _appSettings = appSettings.Value;
        }

        [Route("profile/{business_id}/{id}")]
        public ApiResponse Profile(string business_id, string id)
        {
            ApiResponse response = new ApiResponse();
            var access_token = Request.Cookies["access_token"];

            var url = _appSettings.BaseUrls.Api + "tickets/get/" + business_id + '/' + id + "/?access_token=" + access_token;
            var model = Core.Helpers.WebHelper.HttpGetAsync<TicketModel>(url).Result;
            if (model == null) return response;
            var result = _viewRenderService.RenderToStringAsync("Ticket/Profile", model).Result;
            response.view = result;
            response.ok = true;
            return response;
        }

        [Route("last/{business_id}/{customer_id}")]
        public ApiResponse LastTicket(string business_id, string customer_id)
        {
            ApiResponse response = new ApiResponse();
                var access_token = Request.Cookies["access_token"];

                var url = _appSettings.BaseUrls.Api + "tickets/last/" + business_id + "/" + customer_id + "/?access_token=" + access_token;
                var model = Core.Helpers.WebHelper.HttpGetAsync<TicketModel>(url).Result;
                if (model == null) return response;
                var result = _viewRenderService.RenderToStringAsync("Ticket/Last", model).Result;
                response.view = result;
                response.data = new { ticket = model.id };
                response.ok = true;
            return response;
        }

        [Route("add/{business_id}/{thread_id}")]
        public ApiResponse SendTicket(string business_id, string thread_id, [FromQuery]string ticket_id, [FromBody]Ticket ticket)
        {
            ApiResponse response = new ApiResponse();
            var access_token = Request.Cookies["access_token"];
            var user_id = Request.Cookies["user_id"] ?? "";
            AgentModel agent = null;
            if (!string.IsNullOrWhiteSpace(user_id) && !string.IsNullOrWhiteSpace(access_token))
            {
                var u = _appSettings.BaseUrls.Api + "brands/agents/single/" + user_id + "/?access_token=" + access_token;
                agent = Core.Helpers.WebHelper.HttpGetAsync<AgentModel>(u).Result;
            }
            if (agent == null) return response;
            ticket.business_id = business_id;
            ticket.thread_id = thread_id;
            ticket.sender_avatar = agent.avatar;
            ticket.sender_id = agent.id;
            ticket.sender_name = agent.name;
            ticket.timestamp = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(ticket.created_time);
            if (!string.IsNullOrWhiteSpace(ticket_id))
            {
                var url = _appSettings.BaseUrls.Api + "tickets/get/" + business_id + "/" + ticket_id + "/?access_token=" + access_token;
                response = Core.Helpers.WebHelper.HttpGetAsync<ApiResponse>(url).Result;

            }
            else
            {
                ticket.id = Core.Helpers.CommonHelper.GenerateNineDigitUniqueNumber();
                var url = _appSettings.BaseUrls.Api + "tickets/add/" + "?access_token=" + access_token;
                response = Core.Helpers.WebHelper.HttpPostAsync<ApiResponse>(url, ticket).Result;

            }

            if (response == null || response.data == null) return response;
            TicketModel model = new TicketModel
            {
                id = response.data.id,
                created_time = response.data.created_time,
                timestamp = response.data.timestamp,
                sender_id = response.data.sender_id,
                sender_name = response.data.sender_name,
                sender_avatar = response.data.sender_avatar,
                short_description = response.data.short_description,
                description = response.data.featured,
                status = response.data.status,
                tags = response.data.tags,
                thread_id = response.data.thread_id,
                customer_id = response.data.customer_id,
                customer_name = response.data.customer_name,
                type = response.data.type,
                business_id = response.data.business_id,
                channel_id = response.data.channel_id
            };
            var result = _viewRenderService.RenderToStringAsync("Ticket/Add", model).Result;
            response.data = model;
            response.view = result;
            response.ok = true;
            return response;
        }


    }
}
