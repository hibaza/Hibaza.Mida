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
using ZaloCSharpSDK;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Hibaza.CCP.Web.Controllers
{


    [Route("channels")]
    public class ChannelController : Controller
    {
     
        private readonly IViewRenderService _viewRenderService;
        private readonly AppSettings _appSettings;
        public ChannelController(IViewRenderService viewRenderService, IOptions<AppSettings> appSettings)
        {
            _viewRenderService = viewRenderService;
            _appSettings = appSettings.Value;
        }

        [Route("settings/{user_id}/{business_id}")]
        public ApiResponse Settings(string user_id, string business_id)
        {
            //var timestame = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow);

            //var templatedata = new Dictionary<string, string>();
            //templatedata.Add("content", "alo thử quan tâm");

            //var data = new Dictionary<string, object>();
            //data.Add("phone", 84982745193L);
            //data.Add("templateid", "c8754d9b71de9880c1cf");
            //data.Add("templatedata", templatedata);
            //data.Add("callbackdata", "https://fbwebhook.hibaza.com/brands/zalos/webhook");

            //var t = JsonConvert.SerializeObject(data);
            //var mac = Core.Helpers.CommonHelper.sha256("1282263163182093624" + JsonConvert.SerializeObject(data) +
            //    timestame + "37TU121BboZ5DXNyExHx");
            //var uri = "https://openapi.zaloapp.com/oa/v1/sendmessage/phone/invite_v2?oaid=1282263163182093624&data=" +
            //    JsonConvert.SerializeObject(data) + "&timestamp=" + timestame + "&mac=" + mac;
            //var rsss = Core.Helpers.WebHelper.HttpPostAsync<JObject>(uri, null).Result;





            //ZaloOaInfo info = new ZaloOaInfo(1282263163182093624, "37TU121BboZ5DXNyExHx");
            //ZaloOaClient oaClient = new ZaloOaClient(info);

            //long userId = 84982745193L; // user id or phone number;
            //var profile = oaClient.getProfile(userId);

            // var uid = (string)profile["data"]["userId"];


            //            templatedata = {
            //                "content":"Chào bạn, Chúc bạn một ngày vui vẻ!"
            //}
            //            data = {
            //                'phone': phone,
            //    'templateid': template_id,
            //    'templatedata': templatedata
            //            }
            //params = {
            //                'data': data
            //}

            //var appInfo = new Zalo3rdAppInfo(4195900329485784724, "3RJs6hbXkNSScG5VZI76", _appSettings.BaseUrls.Web + "zaloauth");
            //var appClient = new Zalo3rdAppClient(appInfo);

            //var list = new List<long>();
            //list.Add(84982745193);
            //var token = "yJJ8ROH8S27uCiDaW4DtB_G-mM6CMITlZKh3Vg1b4qMkNDfCjqCVURnwgbgqGKTDWcto1RWLAJUb6z5xZ2GTTl8_oJBq90GWw0d2PUjmCJJtN-9zpaKyRF5WrY6CGXyKXMAcAfXzOM2YNzDQhI4A7z4QsbEjAIX0qqt4SgfA45QpOQ5wiciHJSfrm4QtPZPJltdn5eTsHWogQ8WAh11b6fKkYIge3MK_gHA96OaDNGkoDAGFcWrr9kOqmZFdCImvaYZQT-vH5aBoKF1Ttsb34ELuWpjCIO5yZbDz80";
                       
            //JObject sendAppRequest = appClient.sendAppRequest(token, list, "test moi ket ban");


            ApiResponse response = new ApiResponse();
            var access_token = Request.Cookies["access_token"] ?? "";
            AgentModel model = null;
            if (!string.IsNullOrWhiteSpace(user_id) && !string.IsNullOrWhiteSpace(access_token))
            {
                var u = _appSettings.BaseUrls.Api + "brands/agents/single/" + user_id + "/?access_token=" + access_token;
                model = Core.Helpers.WebHelper.HttpGetAsync<AgentModel>(u).Result;
            }
            if (model == null) return response;

            var url = _appSettings.BaseUrls.Api + "brands/channels/list/" + business_id + "/?access_token=" + access_token;

            var rs = Core.Helpers.WebHelper.HttpGetAsync<ChannelFeed>(url).Result;
            if (rs == null || rs.Data == null) rs = new ChannelFeed { Data = new List<Channel>() };
            var result = _viewRenderService.RenderToStringAsync("Channel/Settings", new ChannelSettings { business_id = business_id, Channels = rs.Data,ZaloAuth=""}).Result;

            response.view = result;
            response.ok = true;
            Response.Cookies.Delete("edit_zalo_id");
            return response;
        }

        [HttpGet("edit/{business_id}/{id}")]
        public ApiResponse Edit(string business_id, string id)
        {
            ApiResponse response = new ApiResponse();
            var access_token = Request.Cookies["access_token"];
            var url = _appSettings.BaseUrls.Api + "brands/channels/single/" + business_id + '/' + id + "/?access_token=" + access_token;
            var data = Core.Helpers.WebHelper.HttpGetAsync<Channel>(url).Result;
            if (data == null) return response;

            url = _appSettings.BaseUrls.Api + "brands/channels/list/" + business_id + "/?access_token=" + access_token;
            var rs = Core.Helpers.WebHelper.HttpGetAsync<ChannelFeed>(url).Result;
            if (rs == null || rs.Data == null) rs = new ChannelFeed { Data = new List<Channel>() };

            var model = new ChannelAddEdit { AppId = _appSettings.ClientId, BusinessId = business_id, Channels = rs.Data, ChannelId = id, PageId = data.ext_id, PageName = data.name ,Secret = data.secret};
            var result = _viewRenderService.RenderToStringAsync("Channel/Edit", model).Result;
            response.data = result;
            response.ok = true;
            return response;
        }

        [Route("new/facebook/{business_id}")]
        public ApiResponse New(string business_id)
        {
            ApiResponse response = new ApiResponse();
            var access_token = Request.Cookies["access_token"];
            var url = _appSettings.BaseUrls.Api + "brands/channels/list/" + business_id + "/?access_token=" + access_token;
            var rs = Core.Helpers.WebHelper.HttpGetAsync<ChannelFeed>(url).Result;
            if (rs == null || rs.Data == null) rs = new ChannelFeed { Data = new List<Channel>() };
            var model = new ChannelAddEdit { AppId = _appSettings.ClientId, BusinessId = business_id, Channels = rs.Data };
            var result = _viewRenderService.RenderToStringAsync("Channel/Edit", model).Result;
            response.data = result;
            response.ok = true;
            return response;
        }

        [Route("new/zalo_page/{business_id}")]
        public ApiResponse NewZalo(string business_id)
        {
            ApiResponse response = new ApiResponse();
            var access_token = Request.Cookies["access_token"];
            //var url = _appSettings.BaseUrls.Api + "brands/channels/list/" + business_id + "/?access_token=" + access_token;
            //var rs = Core.Helpers.WebHelper.HttpGetAsync<ChannelFeed>(url).Result;
            //if (rs == null || rs.Data == null) rs = new ChannelFeed { Data = new List<Channel>() };
            var model = new ChannelAddEdit { AppId = _appSettings.ClientId, BusinessId = business_id, Channels = null,PageName= "", Token="",ChannelId = "",PageId ="" ,TemplateId ="",Secret = ""};
            var result = _viewRenderService.RenderToStringAsync("Channel/ZaloEdit", model).Result;
            response.data = result;
            response.ok = true;
            return response;
        }

        [HttpGet("EditPageZalo/{business_id}/{id}")]
        public ApiResponse EditZalo(string business_id, string id)
        {
            ApiResponse response = new ApiResponse();
            var access_token = Request.Cookies["access_token"];
            var url = _appSettings.BaseUrls.Api + "brands/channels/single/" + business_id + '/' + id + "/?access_token=" + access_token;
            var data = Core.Helpers.WebHelper.HttpGetAsync<Channel>(url).Result;
            if (data == null) return response;

            var model = new ChannelAddEdit { AppId ="", BusinessId = business_id,  ChannelId = id, PageId = data.ext_id, PageName = data.name ,Token = data.token, TemplateId = data.template_id ,Secret = data.secret};
            var result = _viewRenderService.RenderToStringAsync("Channel/ZaloEdit", model).Result;
            response.data = result;
            response.ok = true;
            return response;
        }


        [Route("new/zalo_personal/{business_id}")]
        public ApiResponse NewZaloPersonal(string business_id)
        {
            ApiResponse response = new ApiResponse();
            var access_token = Request.Cookies["access_token"];
            //var url = _appSettings.BaseUrls.Api + "brands/channels/list/" + business_id + "/?access_token=" + access_token;
            //var rs = Core.Helpers.WebHelper.HttpGetAsync<ChannelFeed>(url).Result;
            //if (rs == null || rs.Data == null) rs = new ChannelFeed { Data = new List<Channel>() };
            var model = new ChannelAddEdit { AppId = _appSettings.ClientId, BusinessId = business_id, Channels = null, PageName = "", Token = "", ChannelId = "", PageId = "",TemplateId ="" ,Secret = ""};
            var result = _viewRenderService.RenderToStringAsync("Channel/ZaloPersonal", model).Result;
            response.data = result;
            response.ok = true;
            return response;
        }

        [HttpGet("EditPersonalZalo/{business_id}/{id}")]
        public ApiResponse EditPersonalZalo(string business_id, string id)
        {
            ApiResponse response = new ApiResponse();
            var access_token = Request.Cookies["access_token"];
            var url = _appSettings.BaseUrls.Api + "brands/channels/single/" + business_id + '/' + id + "/?access_token=" + access_token;
            var data = Core.Helpers.WebHelper.HttpGetAsync<Channel>(url).Result;
            if (data == null) return response;

            var model = new ChannelAddEdit { AppId = "", BusinessId = business_id, ChannelId = id, PageId = data.ext_id, PageName = data.name, Token = data.token, TemplateId = data.template_id ,Secret = data.secret};
            var result = _viewRenderService.RenderToStringAsync("Channel/ZaloPersonal", model).Result;
            response.data = result;
            response.ok = true;
            return response;
        }

        //[HttpGet("EditZaloPersonal/{business_id}/{id}")]
        //public ApiResponse EditZaloPersonal(string business_id, string id)
        //{
        //    ApiResponse response = new ApiResponse();

        //    var obj = _cache.Get(APP_ZALO);
        //    if (obj == null)
        //    {
        //       var appInfo = new Zalo3rdAppInfo(_appSettings.Zalos.App_Id, _appSettings.Zalos.App_Secret, _appSettings.BaseUrls.Web + "zaloauth");
        //        appClient = new Zalo3rdAppClient(appInfo);
        //        _cache.Set(APP_ZALO, appClient, DateTime.Now.AddDays(1));
        //    }
        //    else
        //        appClient = (Zalo3rdAppClient)obj;

        //    string loginUrl = appClient.getLoginUrl();


        //    var access_token = Request.Cookies["access_token"];
        //    var url = _appSettings.BaseUrls.Api + "brands/channels/single/" + business_id + '/' + id + "/?access_token=" + access_token;
        //    var data = Core.Helpers.WebHelper.HttpGetAsync<Channel>(url).Result;
        //    if (data == null) return response;

        //    var model = new ChannelAddEdit { AppId = "", BusinessId = business_id, ChannelId = id, PageId = data.ext_id, PageName = data.name, Token = data.token, TemplateId = data.template_id };
        //    var result = _viewRenderService.RenderToStringAsync("Channel/ZaloPersonal", model).Result;
        //    response.data = result;
        //    response.ok = true;
        //    return response;
        //}

        // [HttpPost("upsert/{type}/{business_id}/{channel_id}/{oaid}/{oasecret}/{page_name}")]
        //  public string CreateChannel(string type, string business_id, string channel_id, string oaid, string oasecret, string page_name)
        [Route("UpsertPageZalo/{user_id}/{business_id}/{channel_id}/{oaid}/{oasecret}/{page_name}/{template_id}")]
        public ApiResponse ZaloUpsert(string user_id, string business_id, string channel_id, string oaid, string oasecret, string page_name,string template_id)
        {
            ApiResponse response = new ApiResponse();
            response.msg = "Add channel error";

            var access_token = Request.Cookies["access_token"] ?? "";
            
            Channel para = new Channel();
            para.business_id = business_id;
            para.ext_id = oaid;
            para.name = page_name;
            para.id = channel_id;
            para.template_id =template_id;
            para.token = oasecret;
            para.type = "zalo_page";
            para.secret = oasecret;

            var rs1 = Core.Helpers.WebHelper.HttpPostAsync(_appSettings.BaseUrls.Api + "brands/zalos/upsert", para).Result;

            if (!rs1)
                return response;

            var url = _appSettings.BaseUrls.Api + "brands/channels/list/" + business_id + "/?access_token=" + access_token;

           var rs = Core.Helpers.WebHelper.HttpGetAsync<ChannelFeed>(url).Result;
            if (rs == null || rs.Data == null) rs = new ChannelFeed { Data = new List<Channel>() };
            var result = _viewRenderService.RenderToStringAsync("Channel/Settings", new ChannelSettings { business_id = business_id, Channels = rs.Data, ZaloAuth = "" }).Result;

            response.view = result;
            response.msg = "Add channel ok";
            response.ok = true;
            return response;
        }

        [Route("/ZaloAuth")]
        public IActionResult ZaloAuth()
        {
            return View();
        }

        [Route("/FBAuth")]
        public IActionResult FbAuth()
        {
            return View();
        }


        [Route("UpsertPersonalZalo")]
        public ApiResponse UpsertPersonalZalo()
        {
            ApiResponse response = new ApiResponse();
            response.msg = "Add channel error";
            try
            {
                var id_zalo_appid = Request.Cookies["id_zalo_appid"];
                var id_zalo_channelid = Request.Cookies["id_zalo_channelid"];
                var id_zalo_business_id = Request.Cookies["id_zalo_business_id"];
                var id_zalo_appsecret = Request.Cookies["id_zalo_appsecret"];
                var id_zalo_name = Request.Cookies["id_zalo_name"];
                var id_zalo_templateid = Request.Cookies["id_zalo_templateid"];
                var codeZalo = Request.Cookies["codeZalo"];
                               
                if (string.IsNullOrWhiteSpace(codeZalo)|| string.IsNullOrWhiteSpace(id_zalo_business_id))
                    return response;

                var appInfo = new Zalo3rdAppInfo(Convert.ToInt64(id_zalo_appid), id_zalo_appsecret, _appSettings.BaseUrls.Web + "zaloauth");
               var appClient = new Zalo3rdAppClient(appInfo);               
                JObject tokenObj = appClient.getAccessToken(codeZalo);

                var token = (string)tokenObj["access_token"];
                if (token == null)
                    return response;
                var profile = appClient.getProfile(token, "id,name,birthday,gender,picture");
                var profile_id = (string)profile["id"];
                if (!string.IsNullOrWhiteSpace(profile_id))
                {
                    // upsert
                    Channel para = new Channel();
                    para.business_id = id_zalo_business_id;
                    para.ext_id = id_zalo_appid;
                    para.name = id_zalo_name;
                    para.id = id_zalo_channelid;
                    para.template_id = id_zalo_templateid;
                    para.token = token;
                    para.type = "zalo_personal";
                    para.secret = id_zalo_appsecret;

                    var rs1 = Core.Helpers.WebHelper.HttpPostAsync(_appSettings.BaseUrls.Api + "brands/zalos/upsert", para).Result;
                    if (!rs1)
                        return response;
                    //  var rs1 = Core.Helpers.WebHelper.HttpPostAsync(_appSettings.BaseUrls.Api + "brands/zalos/upsert/zalo-personal/" + id_zalo_business_id + "/" + id_zalo_channelid + "/" + profile_id + "/" + token + "/" + id_zalo_name + "/" + id_zalo_templateid, null).Result;

                    var access_token = Request.Cookies["access_token"] ?? "";
                    var url = _appSettings.BaseUrls.Api + "brands/channels/list/" + id_zalo_business_id + "/?access_token=" + access_token;

                    var rs = Core.Helpers.WebHelper.HttpGetAsync<ChannelFeed>(url).Result;
                    if (rs == null || rs.Data == null) rs = new ChannelFeed { Data = new List<Channel>() };
                    var result = _viewRenderService.RenderToStringAsync("Channel/Settings", new ChannelSettings { business_id = id_zalo_business_id, Channels = rs.Data, ZaloAuth = "" }).Result;
                    response.msg = "Add channel ok";
                    response.view = result;
                    response.ok = true;

                    Response.Cookies.Delete("id_zalo_appid");
                    Response.Cookies.Delete("id_zalo_channelid");
                    Response.Cookies.Delete("id_zalo_business_id");
                    Response.Cookies.Delete("id_zalo_appsecret");
                    Response.Cookies.Delete("id_zalo_name");
                    Response.Cookies.Delete("id_zalo_templateid");
                    Response.Cookies.Delete("codeZalo");

                    return response;
                }
                return response;

            }
            catch (Exception ex)
            {
                response.msg = "Add channel error " + ex.Message;
                response.ok = false;
                return response;
            }
        }

    }
}
