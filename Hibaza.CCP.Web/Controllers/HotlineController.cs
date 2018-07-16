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
using Newtonsoft.Json.Linq;

namespace Hibaza.CCP.Web.Controllers
{
    [Route("/")]
    public class HotlineController : Controller
    {
        private readonly IViewRenderService _viewRenderService;
        private readonly AppSettings _appSettings;

        public HotlineController(IViewRenderService viewRenderService, IOptions<AppSettings> appSettings)
        {
            _viewRenderService = viewRenderService;
            _appSettings = appSettings.Value;
        }


        [Route("hotline/get/{agent_id}")]
        public ApiResponse GetHotlineByAgentId(string agent_id)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var access_token = Request.Cookies["access_token"];

                var url = _appSettings.BaseUrls.ApiHotline + "api/Agents/InfoAgentAndPhone/" + agent_id + "/?access_token=" + access_token;

                var phone = new PhoneAccounts();
                var model = Core.Helpers.WebHelper.HttpGetAsync<dynamic>(url).Result;

                if (model != null)
                {
                    var jObject = JObject.Parse(model);
                    try
                    {
                        phone.phone_account_id = (string)jObject["phone_account_id"];
                        phone.outgoing_enable = (bool)jObject["outgoing_enable"];
                        phone.outgoing_display_name = (string)jObject["outgoing_display_name"];
                        phone.incoming_enable = (bool)jObject["incoming_enable"];
                        phone.phone_status = (string)jObject["status"];

                        ViewBag.phone_status = "Phone " + (string)jObject["status"];
                        var str = "";
                        foreach (var ar in (JArray)jObject["incoming_extention"])
                        {
                            str += ar.ToString() + ",";
                        }
                        phone.incoming_extention = str.Length > 2 ? str.Substring(0, str.Length - 1) : "";
                    }
                    catch {
                        phone.phone_account_id = "";
                        phone.outgoing_enable = false;
                        phone.outgoing_display_name = "";
                        phone.incoming_enable = false;
                        phone.phone_status = "offline";
                        phone.incoming_extention = "";
                    }
                   
                    url = _appSettings.BaseUrls.ApiHotline + "api/PhoneAccounts/PhoneAccountNotUsing/" + (string)jObject["business_id"] + "/" + agent_id + "/main/?access_token=" + access_token;
                    var notUsings = Core.Helpers.WebHelper.HttpGetAsync<dynamic>(url).Result;

                    var phone_accounts = new Dictionary<string, string>();
                    try
                    {
                        phone_accounts = JsonConvert.DeserializeObject<Dictionary<string, string>>(notUsings);
                    }
                    catch { }
                    phone.phone_account_not_using = phone_accounts;// (Array)notUsings;

                    var result = _viewRenderService.RenderToStringAsync("Account/Hotline", phone).Result;
                    response.view = result;
                    response.data = new { phone_account_id = phone.phone_account_id };
                    response.ok = true;
                    return response;

                }
            }
            catch (Exception ex) { }
            return response;
        }

        [HttpPost("hotline/upsert/{business_id}/{agent_id}")]
        public ApiResponse CustomerUpdate(string business_id, string agent_id, [FromForm]Phone hotline)
        {
            ApiResponse response = new ApiResponse();
            var access_token = Request.Cookies["access_token"];
            var url = _appSettings.BaseUrls.Api + "hotline/upsert/" + business_id + "/" + agent_id + "/?access_token=" + access_token;

            response = Core.Helpers.WebHelper.HttpPostAsync<ApiResponse>(url, hotline).Result;

            if (response.ok) return response;
            var result = _viewRenderService.RenderToStringAsync("Account/Hotline", hotline).Result;
            response.view = result;
            response.ok = false;
            return response;
        }


    }
}
