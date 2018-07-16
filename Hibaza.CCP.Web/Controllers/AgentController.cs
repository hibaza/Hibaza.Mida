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
using Hibaza.CCP.Core;
using Microsoft.Extensions.Options;
using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;


namespace Hibaza.CCP.Web.Controllers
{
    [Route("agents")]
    public class AgentController : Controller
    {
        private readonly IViewRenderService _viewRenderService;
        private readonly AppSettings _appSettings;
        public AgentController(IViewRenderService viewRenderService, IOptions<AppSettings> appSettings)
        {
            _viewRenderService = viewRenderService;
            _appSettings = appSettings.Value;
        }

        [Route("settings/{user_id}/{business_id}")]
        public ApiResponse Settings(string user_id, string business_id)
        {
            ApiResponse response = new ApiResponse();
            var access_token = Request.Cookies["access_token"] ?? "";
            AgentModel model = null;
            if (!string.IsNullOrWhiteSpace(user_id) && !string.IsNullOrWhiteSpace(access_token))
            {
                var u = _appSettings.BaseUrls.Api + "brands/agents/single/" + user_id + "/?access_token=" + access_token;
                model = Core.Helpers.WebHelper.HttpGetAsync<AgentModel>(u).Result;
            }
            if (model == null) return response;

            var url = _appSettings.BaseUrls.Api + "brands/agents/list/" + business_id + "/?access_token=" + access_token;
            var rs = Core.Helpers.WebHelper.HttpGetAsync<AgentFeed>(url).Result;
            if (rs == null || rs.Data == null) return response;
            var result = _viewRenderService.RenderToStringAsync("Agent/Settings", new AgentSettings { admin = model.admin, user_id = model.id, business_id = business_id, Agents = rs.Data }).Result;
            response.view = result;
            response.ok = true;
            return response;
        }

        [Route("edit/{id}")]
        public ApiResponse Edit(string id)
        {
            ApiResponse response = new ApiResponse();
            var access_token = Request.Cookies["access_token"];
            var url = _appSettings.BaseUrls.Api + "brands/agents/single/" + id + "/?access_token=" + access_token;
            var model = Core.Helpers.WebHelper.HttpGetAsync<AgentModel>(url).Result;
            if (model == null) return response;
            var result = _viewRenderService.RenderToStringAsync("Agent/Edit", model).Result;
            response.view = result;
            response.ok = true;
            return response;
        }
        
        [Route("new/{business_id}")]
        public ApiResponse New(string business_id)
        {
            ApiResponse response = new ApiResponse();
            var access_token = Request.Cookies["access_token"];

            var model = new AgentModel(new Agent { id = "", business_id = business_id, ext_id = "" });
            var result = _viewRenderService.RenderToStringAsync("Agent/Edit", model).Result;
            response.view = result;
            response.ok = true;
            return response;
        }
    }
}
