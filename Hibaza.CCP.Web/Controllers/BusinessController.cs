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
using Newtonsoft.Json;

namespace Hibaza.CCP.Web.Controllers
{
    [Route("business")]
    public class BusinessController : Controller
    {
        private readonly IViewRenderService _viewRenderService;
        private readonly AppSettings _appSettings;
        public BusinessController(IViewRenderService viewRenderService, IOptions<AppSettings> appSettings)
        {
            _viewRenderService = viewRenderService;
            _appSettings = appSettings.Value;
        }

        [Route("settings/{user_id}/{id}")]
        public ApiResponse Settings(string user_id, string id)
        {
            ApiResponse response = new ApiResponse();
            var access_token = Request.Cookies["access_token"];
            var url = _appSettings.BaseUrls.Api + "brands/agents/single/" + user_id + "/?access_token=" + access_token;
            var model = Core.Helpers.WebHelper.HttpGetAsync<AgentModel>(url).Result;
            if (model == null) return response;

            url = _appSettings.BaseUrls.Api + "business/single/" + id + "/?access_token=" + access_token;
            var data = Core.Helpers.WebHelper.HttpGetAsync<Business>(url).Result;
            if (data == null) return response;

            var result = _viewRenderService.RenderToStringAsync("Business/Settings", new BusinessSettings { id = id, admin = model.admin, user_id= user_id, Data = data }).Result;

            response.view = result;
            response.ok = true;
            return response;
        }


        [Route("update/{business_id}")]
        public ApiResponse Update(string business_id, [FromForm]Business data)
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
            if (agent == null || !agent.admin) return response;

            string url;
            if (Request.Method == "POST")
            {
                url = _appSettings.BaseUrls.Api + "business/update/" + business_id + "/?access_token=" + access_token;
                response = Core.Helpers.WebHelper.HttpPostAsync<ApiResponse>(url, data).Result;

            }
            else { 
                url = _appSettings.BaseUrls.Api + "business/single/" + business_id + "/?access_token=" + access_token;
                data = Core.Helpers.WebHelper.HttpGetAsync<Business>(url).Result;
            }
            if (!response.ok)
            {
                var result = _viewRenderService.RenderToStringAsync("Business/Settings", new BusinessSettings { id = business_id, admin = agent.admin, user_id = user_id, Data = data }).Result;
                response.view = result;
            }

            return response;
        }



        [HttpGet("/shortcuts/edit/{business_id}/{id}")]
        public ApiResponse Edit(string business_id, string id)
        {
            ApiResponse response = new ApiResponse();
            var access_token = Request.Cookies["access_token"];
            ShortCutAddEdit model = new ShortCutAddEdit();

            var url = _appSettings.BaseUrls.Api + "brands/shortcuts/single/" + business_id +"/" + id + "/?access_token=" + access_token;
            var data = Core.Helpers.WebHelper.HttpGetAsync<Shortcut>(url).Result;
            if (data == null) return response;
            model.edit = true;
            model.data = data;

            var result = _viewRenderService.RenderToStringAsync("Business/AddShortcut", model).Result;
            response.data = result;
            response.ok = true;
            return response;
        }

        [HttpGet("/shortcuts/new/{business_id}")]
        public ApiResponse New(string business_id)
        {
            ApiResponse response = new ApiResponse();
            var access_token = Request.Cookies["access_token"];
            ShortCutAddEdit model = new ShortCutAddEdit();
            model.data = new Shortcut { business_id = business_id };

            var result = _viewRenderService.RenderToStringAsync("Business/AddShortcut", model).Result;
            response.data = result;
            response.ok = true;
            return response;
        }

        [Route("downloads")]
        public ApiResponse GetHotlineByAgentId()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var access_token = Request.Cookies["access_token"];
                var result = _viewRenderService.RenderToStringAsync("Business/Downloads", "").Result;
                response.view = result;
                response.data = new {};
                response.ok = true;
                return response;

            }
            catch (Exception ex) { }
            return response;
        }

        [Route("downloads/init/{channel_id}/{date_from}/{date_to}/{messages}/{comments}")]
        public async Task<ApiResponse> downloadsCreate(string channel_id,long date_from,long date_to,bool messages ,bool comments)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var access_token = Request.Cookies["access_token"];
                var business_id = Request.Cookies["business_id"];

                var uri = _appSettings.BaseUrls.Api + "facebook/downloadconversations/" + business_id;
                var from = Core.Helpers.CommonHelper.UnixTimestampToDateTime(date_from);
                var to = Core.Helpers.CommonHelper.UnixTimestampToDateTime(date_to);
                var skip = DateTime.UtcNow.Subtract(to).TotalDays;
                var days = to.Subtract(from).TotalDays;

                var para = "access_token=@bazavietnam&channel_id="+channel_id+"&skip=" + skip + "&days=" + days + "&real_time_update=false";
               
                Core.Helpers.WebHelper.HttpGetAsyncSting(uri +"?"+ para);

                var result = await _viewRenderService.RenderToStringAsync("Business/Downloads", "");
                response.view = result;
                response.data = new { };
                response.msg = "downloading...";
                response.ok = true;
                return response;

            }
            catch (Exception ex) { }
            return response;
        }


    }
}
