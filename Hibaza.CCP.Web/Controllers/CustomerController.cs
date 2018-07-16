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
    [Route("/")]
    public class CustomerController : Controller
    {
        private readonly IViewRenderService _viewRenderService;
        private readonly AppSettings _appSettings;

        public CustomerController(IViewRenderService viewRenderService, IOptions<AppSettings> appSettings)
        {
            _viewRenderService = viewRenderService;
            _appSettings = appSettings.Value;
        }

        [Route("{business_id}/comments/gotopost")]
        public ActionResult PostLink(string business_id, [FromQuery]string channel_id, [FromQuery]string postId, [FromQuery]long timestamp)
        {
            var access_token = Request.Cookies["access_token"];
            var url = _appSettings.BaseUrls.Api + "threads/link_post/" + business_id + "/?channel_id=" + channel_id + "&postId=" + postId + "&access_token=" + access_token;
            var response = Core.Helpers.WebHelper.HttpGetAsync<ApiResponse>(url).Result;
            return RedirectPermanent(response.data);
        }
        [Route("{business_id}/messages/inbox")]
        public ActionResult MessagesInbox(string business_id, [FromQuery]string thread_id, [FromQuery]long timestamp)
        {
            var access_token = Request.Cookies["access_token"];
            var url = _appSettings.BaseUrls.Api + "threads/link_inbox/" + business_id + "/" + thread_id + "/?access_token=" + access_token;
            var response = Core.Helpers.WebHelper.HttpGetAsync<ApiResponse>(url).Result;
            return RedirectPermanent(response.data);
        }

        [Route("{business_id}/messages/openlink")]
        public ActionResult OpenLink(string business_id, [FromQuery]string item_id)
        {
            var access_token = Request.Cookies["access_token"];
            var url = _appSettings.BaseUrls.Api + "messages/openlink/" + business_id + "/?item_id=" + item_id + "&access_token=" + access_token;
            var response = Core.Helpers.WebHelper.HttpGetAsync<ApiResponse>(url).Result;
            return RedirectPermanent(response.data);
        }

        [Route("customers/openprofile/{business_id}/{thread_id}")]
        public ActionResult Profile_Ext(string business_id, string thread_id)
        {
            var access_token = Request.Cookies["access_token"];
            var url = _appSettings.BaseUrls.Api + "customers/profile_ext_url/" + business_id + "/" + thread_id + "?access_token=" + access_token;
            var response = Core.Helpers.WebHelper.HttpGetAsync<ApiResponse>(url).Result;
            return RedirectPermanent(response.data);
        }

        [Route("customers/profile/{business_id}/{customer_id}")]
        public ApiResponse CustomerProfile(string business_id, string customer_id)
        {
            ApiResponse response = new ApiResponse();
            var access_token = Request.Cookies["access_token"];

            var url = _appSettings.BaseUrls.Api + "customers/profile/" + business_id + "/" + customer_id + "/?access_token=" + access_token;
            var model = Core.Helpers.WebHelper.HttpGetAsync<CustomerProfileModel>(url).Result;
            if (model == null) return response;
            var result = _viewRenderService.RenderToStringAsync("Customer/Profile", model).Result;
            response.view = result;
            response.data = new { customer_id = model.customer_id };
            response.ok = true;
            return response;
        }


        [HttpGet("customers/edit/{business_id}/{id}")]
        public ApiResponse CustomerEdit(string business_id, string id, [FromForm]CustomerContactInfoModel data)
        {
            ApiResponse response = new ApiResponse();
            var access_token = Request.Cookies["access_token"];
            var url = _appSettings.BaseUrls.Api + "customers/info/" + id + "/?business_id=" + business_id + "&access_token=" + access_token;
            var model = Core.Helpers.WebHelper.HttpGetAsync<CustomerContactInfoModel>(url).Result;
            if (model == null) return response;
            model.id = id;
            var result = _viewRenderService.RenderToStringAsync("Customer/Edit", model).Result;
            response.view = result;
            response.ok = true;
            return response;
        }

        [HttpPost("customers01/edit/{business_id}/{customer_id}/{name}/{phone}/{address}/{email}/{city}")]
        public ApiResponse CustomerUpdate01(string business_id, string customer_id, string name, string phone, string address, string email, string city)
        {
            ApiResponse response = new ApiResponse();
            var access_token = Request.Cookies["access_token"];
            var url = _appSettings.BaseUrls.Api + "customers/info/" + customer_id + "/?business_id=" + business_id + "&access_token=" + access_token;
            var model = Core.Helpers.WebHelper.HttpGetAsync<CustomerContactInfoModel>(url).Result;
            if (model == null) return response;

            model.name = name;
            model.phone = phone;
            model.address = address;
            model.email = email;
            model.city = city;
            url = _appSettings.BaseUrls.Api + "customers/info/" + customer_id + "/?business_id=" + business_id + "&access_token=" + access_token;
            model.id = customer_id;
            response = Core.Helpers.WebHelper.HttpPostAsync<ApiResponse>(url, model).Result;

            return response;
        }

        [HttpPost("customers/edit/{business_id}/{customer_id}")]
        public ApiResponse CustomerUpdate(string business_id, string customer_id, [FromForm]CustomerContactInfoModel model)
        {
            ApiResponse response = new ApiResponse();
            var access_token = Request.Cookies["access_token"];
            var url = _appSettings.BaseUrls.Api + "customers/info/" + customer_id + "/?business_id=" + business_id + "&access_token=" + access_token;
            model.id = customer_id;
            response = Core.Helpers.WebHelper.HttpPostAsync<ApiResponse>(url, model).Result;

            if (response.ok) return response;
            var result = _viewRenderService.RenderToStringAsync("Customer/Edit", model).Result;
            response.view = result;
            response.ok = false;
            return response;
        }

        [HttpGet("customers/profileinfo/{id}/{business_id}")]
        public ApiResponse GetProfileInfo(string id, string business_id, [FromForm]CustomerContactInfoModel data)
        {
            ApiResponse response = new ApiResponse();
            var access_token = Request.Cookies["access_token"];
            var url = _appSettings.BaseUrls.Api + "customers/profileinfo/" + id + "/" + business_id + "?access_token=" + access_token;
            var model = Core.Helpers.WebHelper.HttpGetAsync<CustomerContactInfoModel>(url).Result;
            if (model == null) return response;
            model.id = id;
            var result = _viewRenderService.RenderToStringAsync("Customer/Edit", model).Result;
            response.view = result;
            response.ok = true;
            return response;
        }


        [Route("threads/profile/{business_id}/{thread_id}")]
        public ApiResponse ThreadProfile(string business_id, string thread_id)
        {
            ApiResponse response = new ApiResponse();
            var access_token = Request.Cookies["access_token"];

            var url = _appSettings.BaseUrls.Api + "threads/profile/" + business_id + "/" + thread_id + "/?access_token=" + access_token;
            var model = Core.Helpers.WebHelper.HttpGetAsync<ThreadProfileModel>(url).Result;
            if (model == null) return response;
            var result = _viewRenderService.RenderToStringAsync("Customer/ThreadProfile", model).Result;
            response.view = result;
            response.ok = true;
            response.data = new { last_visit = model.last_visits != null && model.last_visits.Count > 0 ? model.last_visits.First() : "" };
            return response;
        }

    }
}
