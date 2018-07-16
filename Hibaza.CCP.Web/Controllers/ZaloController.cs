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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Caching.Memory;
using System.Dynamic;

namespace Hibaza.CCP.Web.Controllers
{

    public class ZaloController : Controller
    {
        private readonly IViewRenderService _viewRenderService;
        private readonly AppSettings _appSettings;
        //Zalo3rdAppInfo appInfo;
        //Zalo3rdAppClient appClient;
        //ZaloOaClient oaClient;
        //string KEY_OA_ZALO = "zaloOaClient";
        string TOKEN_ZALO = "tokenZalo";
        string UID_ZALO = "uidZalo";

        public static MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        public ZaloController(IViewRenderService viewRenderService, IOptions<AppSettings> appSettings)
        {
            _viewRenderService = viewRenderService;
            _appSettings = appSettings.Value;
        }
        //[Route("zalos/token")]
        //public ApiResponse token()
        //{
        //    ApiResponse response = new ApiResponse();
        //    try
        //    {
        //        var page_name = Request.Cookies["id_zalo_name"];
        //        var page_id = Request.Cookies["edit_zalo_id"];

        //        response.msg = "Add channel error";
        //        if (string.IsNullOrWhiteSpace(page_name))
        //            return response;
        //        var business_id = Request.Cookies["business_id"];
        //        if (string.IsNullOrWhiteSpace(business_id))
        //            return response;

        //        var token = Request.Cookies[TOKEN_ZALO];
        //        var userId = Request.Cookies[UID_ZALO];

        //        Response.Cookies.Delete(TOKEN_ZALO);
        //        Response.Cookies.Delete(UID_ZALO);
        //        Response.Cookies.Delete("id_zalo_name");
        //        Response.Cookies.Delete("edit_zalo_id");

        //        //Zalo3rdAppInfo appInfo = new Zalo3rdAppInfo(_appSettings.Zalos.App_Id, _appSettings.Zalos.App_Secret, _appSettings.BaseUrls.Web+ "zaloauth");
        //        //Zalo3rdAppClient appClient = new Zalo3rdAppClient(appInfo);
        //        //JObject profile = appClient.getProfile(token, "id,name");

        //        //ZaloOaInfo zaloOaInfo = new ZaloOaInfo(_appSettings.Zalos.OA_Id, _appSettings.Zalos.OA_Secret);
        //        //ZaloOaClient oaClient = new ZaloOaClient(zaloOaInfo);
        //        //oaClient.getProfile(Convert.ToInt64(userId));

                

        //        //edit
        //        if (!string.IsNullOrWhiteSpace(page_id))
        //        {
        //            var rs = Core.Helpers.WebHelper.HttpPostAsync(_appSettings.BaseUrls.Api + "brands/zalos/update/" + business_id + "/" + page_id + "/" + page_name + "/" + token , null).Result;

        //            response.msg = "Update channel ok";
        //            response.ok = true;
        //            return response;
        //        }

        //        // create
        //        if (string.IsNullOrWhiteSpace(page_id) && !string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(token))
        //        {
        //            //create/{type}/{business_id}/{page_id}/{page_name}/{token}/{ext_id}")]
        //            page_id = userId;
        //            var rs= Core.Helpers.WebHelper.HttpPostAsync(_appSettings.BaseUrls.Api + "brands/zalos/create/zalo/" + business_id + "/" + page_id + "/" + page_name + "/" + token + "/" + userId, null).Result;

        //            response.msg = "Add channel ok";
        //            response.ok = true;
        //            return response;
        //        }
               
        //    }
        //    catch (Exception ex)
        //    {  
        //        response.msg = "Add channel error " + ex.Message;
        //        response.ok = false;
        //        return response;
        //    }
        //    response.msg = "Add channel error";
        //    response.ok = false;
        //    return response;
        //}



        //[Route("zalos/token")]
        //public ApiResponse token()
        //{
        //    ApiResponse response = new ApiResponse();
        //    try
        //    {
        //        var obj = _cache.Get(ZALOKEY);
        //        if (obj == null)
        //            return response;

        //        var phone = Request.Cookies["id_zalo_phone"];
        //        var page_name = Request.Cookies["id_zalo_name"];
        //        if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(page_name))
        //            return response;

        //        appClient = (Zalo3rdAppClient)obj;
        //        var zaloCode = Request.Cookies[ZALOCODE];
        //        JObject tokenObj = appClient.getAccessToken(zaloCode);
        //        Response.Cookies.Delete(ZALOCODE);
        //        Response.Cookies.Delete("id_zalo_phone");
        //        Response.Cookies.Delete("id_zalo_name");

        //        var token = (string)tokenObj["access_token"];
        //        var profile = appClient.getProfile(token, "id,name,birthday,gender,picture");
        //        if (!string.IsNullOrWhiteSpace((string)profile["id"]))
        //        {
        //            //type, string business_id,string page_id, string page_name,string token,string phone
        //            var type = "zalo";
        //            var business_id = Request.Cookies["business_id"];

        //           var rs= Core.Helpers.WebHelper.HttpPostAsync<ApiResponse>(_appSettings.BaseUrls.Api+ "brands/zalos/create/"+ type+"/"+ business_id+"/"+ profile["id"]+ "/"+ page_name+"/"+ token+"/"+ phone, null).Result;

        //            response.msg = "Add channel ok";
        //            response.ok = true;
        //            return response;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.msg = "Add channel error " + ex.Message;
        //        response.ok = false;
        //        return response;
        //    }
        //    response.msg = "Add channel error";
        //    response.ok = false;
        //    return response;
        //}

        //[Route("/zalos/new")]
        //public ApiResponse New()
        //{
        //    ApiResponse response = new ApiResponse();
        //    var obj = _cache.Get(ZALOKEY);
        //    if (obj == null)
        //    {
        //        appInfo = new Zalo3rdAppInfo(_appSettings.Zalos.App_Id, _appSettings.Zalos.App_Secret, _appSettings.BaseUrls.Web + "zaloauth");
        //        appClient = new Zalo3rdAppClient(appInfo);
        //        _cache.Set(ZALOKEY, appClient, DateTime.Now.AddDays(1));
        //    }
        //    else
        //        appClient = (Zalo3rdAppClient)obj;

        //    string loginUrl = appClient.getLoginUrl();

        //    var model = new ChannelAddEdit { Phone = "", PageName="" ,Token = loginUrl};
        //    var result = _viewRenderService.RenderToStringAsync("Channel/Zalo", model).Result;
        //    response.data = result;
        //    response.ok = true;
        //    return response;
        //}

        //[HttpGet("/zalos/edit/{business_id}/{id}")]
        //public ApiResponse Edit(string business_id, string id)
        //{
        //    ApiResponse response = new ApiResponse();

        //    var obj = _cache.Get(ZALOKEY);
        //    if (obj == null)
        //    {
        //        appInfo = new Zalo3rdAppInfo(_appSettings.Zalos.App_Id, _appSettings.Zalos.App_Secret, _appSettings.BaseUrls.Web + "zaloauth");
        //        appClient = new Zalo3rdAppClient(appInfo);
        //        _cache.Set(ZALOKEY, appClient, DateTime.Now.AddDays(1));
        //    }
        //    else
        //        appClient = (Zalo3rdAppClient)obj;

        //    string loginUrl = appClient.getLoginUrl();

        //    var access_token = Request.Cookies["access_token"];
        //    var url = _appSettings.BaseUrls.Api + "brands/channels/single/" + business_id + '/' + id + "/?access_token=" + access_token;
        //    var data = Core.Helpers.WebHelper.HttpGetAsync<Channel>(url).Result;
        //    if (data == null) return response;

        //    //url = _appSettings.BaseUrls.Api + "brands/channels/list/" + business_id + "/?access_token=" + access_token;
        //    //var rs = Core.Helpers.WebHelper.HttpGetAsync<ChannelFeed>(url).Result;
        //    //if (rs == null || rs.Data == null) rs = new ChannelFeed { Data = new List<Channel>() };

        //    var model = new ChannelAddEdit { AppId = _appSettings.ClientId, BusinessId = business_id, ChannelId = id, PageId = data.ext_id, PageName = data.name, Phone = data.phones[0], Token = loginUrl };
        //    var result = _viewRenderService.RenderToStringAsync("Channel/Zalo", model).Result;
        //    response.data = result;
        //    response.ok = true;
        //    return response;
        //}

        [HttpGet("/zalos/test")]
        public ApiResponse Edit1()
        {
            //var appInfo = new Zalo3rdAppInfo(4195900329485784724, "3RJs6hbXkNSScG5VZI76", _appSettings.BaseUrls.Web + "zaloauth");
            //var appClient = new Zalo3rdAppClient(appInfo);

            //var list = new List<long>();
            //list.Add(84979354170);
            //var token = "CwMjOaAYtYiwewPhHgdP9JQlf6vAiuLQPTIf5qwVWbzPzPraTeY3Jmdx-brqYES9Qz_r8dU-sofSvkiYSxt4EKYsnm9VphaUOBU-A57gWHHIWR4QO_6DEGQHhWmsp8u_J9Fn8qNiw3PCZPGQVT6r1L-LqYfSplORVxhJE4E-monQzCmoM_7F7oBon1u4ZyGH1_NcEt-0rX1-nFeALC3572k-zoOfwUik8-cM0o6qgH00mx8i3vYXF3JDdp4hjhSf4FUJ0Zpmb38xyUuA0gNjIo7Ourz8CafvIBdJAW";
            //JObject sendAppRequest = appClient.sendAppRequest(token, list, "test moi ket ban");


            //var id_zalo_name = Request.Cookies["id_zalo_name"];
            //var timestame =Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow);


            //var templatedata = new Dictionary<string, string>();
            //templatedata.Add("DAY", "20");
            //templatedata.Add("TIME_24HR", "24h");

            //var data = new Dictionary<string,object>();
            //data.Add("phone", 841638500000);
            //data.Add("templateid", "c8754d9b71de9880c1cf");
            //data.Add("templatedata",JsonConvert.SerializeObject(templatedata));
            //data.Add("callbackdata", "https://fbwebhook.hibaza.com/brands/zalos/webhook");

            //var mac = Core.Helpers.CommonHelper.sha256("1282263163182093624" + JsonConvert.SerializeObject(data) + timestame + "37TU121BboZ5DXNyExHx");
            //var uri = "https://openapi.zaloapp.com/oa/v1/sendmessage/phone/invite_v2?oaid=1282263163182093624&data=" + JsonConvert.SerializeObject(data)+ "&timestamp=" + timestame + "&mac="+mac;
            //var rsss = Core.Helpers.WebHelper.HttpPostAsync<JObject>(uri,null).Result;

            //var mac = Core.Helpers.CommonHelper.sha256(_appSettings.Zalos.OA_Id + "84979354170" + timestame + _appSettings.Zalos.OA_Secret);
            ////var uri = "https://openapi.zaloapp.com/oa/v1/getprofile?oaid=" + _appSettings.Zalos.OA_Id + "&uid=84979354170&timestamp=" + timestame + "&mac=" + mac;
            //var uri = "https://openapi.zaloapp.com/oa/v1/getprofile?oaid=" + _appSettings.Zalos.OA_Id + "&uid=84979354170&timestamp=" + timestame + "&mac=" + mac;
            //var rsss = Core.Helpers.WebHelper.HttpGetAsync<JObject>(uri).Result;
            //var tt = (string)rsss["data"]["userId"];

            //mac = Core.Helpers.CommonHelper.sha256(_appSettings.Zalos.OA_Id + "{\"message\":\"test send text mess by zalo api\",\"uid\":\"" + tt + "\"}" + timestame + _appSettings.Zalos.OA_Secret);

            ////https://openapi.zaloapp.com/oa/v1/sendmessage/text?oaid=3017580706414181376&data={'message':'test send text mess by zalo api','uid':5314099839299520168}&timestamp=1525773021533&mac=9e21551935125b2327f40e0366c9e6cc561f6c99326f4959280de625a55cd489
            //uri = "https://openapi.zaloapp.com/oa/v1/sendmessage/text?oaid="+_appSettings.Zalos.OA_Id+"&data={\"message\":\"test send text mess by zalo api\",\"uid\":\"" + tt + "\"}&timestamp="+ timestame + "&mac="+mac;

            //var tts = Core.Helpers.WebHelper.HttpPostAsync<JObject>(uri,null).Result;
            ////ZaloOaInfo info = new ZaloOaInfo(1014541367050827072, "P6YTW5qGqq64w6yjtCkt");
            ////ZaloOaClient oaClient = new ZaloOaClient(info);

            ////long userId = 84982745193L; // user id or phone number;
            ////var profile = oaClient.getProfile(userId);
            return null;
        }
    }
}
