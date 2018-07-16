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
using Newtonsoft.Json;
using Hibaza.CCP.Domain.Models.Facebook;
using Microsoft.Net.Http.Headers;
using Firebase.Storage;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Hibaza.CCP.Core.TokenProviders;
using Microsoft.Extensions.Logging;
using Hibaza.CCP.Data.Repositories.Firebase;
using Hibaza.CCP.Data.Providers.Firebase;

namespace Hibaza.CCP.Api.Controllers
{

    public class ShortCutModel
    {
        public string business_id { get; set; }
        public string name { get; set; }
        public string shortcut { get; set; }
        public string created_by { get; set; }
    }

    [Route("brands/shortcuts")]
    public class ShortcutController : Controller
    {
        public IShortcutService _shortcutService;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IBusinessService _businessService;
        private readonly ILoggingService _logService;

        public ShortcutController(IShortcutService shortcutService, IOptions<AppSettings> appSettings,IBusinessService businessService, ILoggingService logService)
        {
            _shortcutService = shortcutService;
            _appSettings = appSettings;
            _logService = logService;
            _businessService = businessService;
        }

        [HttpGet("copyto/{business_id}")]
        public int CopToSQL(string business_id, [FromQuery]string access_token)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;
            var fb = new FirebaseShortcutRepository(new FirebaseFactory(_appSettings));

            foreach (var t in fb.GetShortcuts(business_id, new Paging { Limit = 10 }).Result)
            {
                count++;
                _shortcutService.Create(t.Object);
            }
            return count;
        }

        [HttpGet("list/{business_id}")]
        public async Task<ApiResponse> GetShortcuts(string business_id, string access_token)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var resultData = await _shortcutService.GetShortcuts(business_id, 0, 10);
                response.ok = true;
                response.data = resultData;
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Shortcut",
                    link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
                    details = JsonConvert.SerializeObject(ex.StackTrace),
                    name = string.Format("Get shortcuts by business_id: {0}", business_id)
                });
            }
            return response;
        }

        [HttpGet("get-by-agent/{business_id}/{agent_id}")]
        public async Task<ApiResponse> GetByAgent(string business_id, string agent_id, string access_token)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var resultData = await _shortcutService.GetByAgent(business_id, agent_id);
                response.ok = true;
                response.data = resultData;
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Shortcut",
                    link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
                    details = JsonConvert.SerializeObject(ex.StackTrace),
                    name = string.Format("Get shortcuts by business_id: {0}", business_id)
                });
            }
            return response;
        }
        

          //[HttpPost("update/form/{shortcut_id}")]
          //public string UpdateFromForm(string shortcut_id, [FromForm]Shortcut data)
          //{

          //    string resultData = "";
          //    try
          //    {
          //        data.message = data.message ?? "";
          //        data.message = data.message.Trim().ToLower();
          //        if (string.IsNullOrWhiteSpace(data.message)) return "Cannot be empty";

          //        data.shortcut = data.shortcut ?? "";
          //        data.shortcut = data.shortcut.Trim().ToLower();
          //        if (string.IsNullOrWhiteSpace(data.shortcut)) return "Cannot be empty";

          //        if (!string.IsNullOrWhiteSpace(shortcut_id))
          //        {
          //            Shortcut shortcut = GetById(data.business_id, shortcut_id);
          //            if (shortcut != null)
          //            {
          //                shortcut.message = data.message;
          //                shortcut.shortcut = data.shortcut;
          //                resultData = _shortcutService.Create(data);
          //            }
          //        }

          //    }
          //    catch (Exception ex)
          //    {
          //        _logService.Create(new Log
          //        {
          //            message = ex.Message,
          //            category = "Shortcut",
          //            link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
          //            details = JsonConvert.SerializeObject(ex.StackTrace),
          //            name = string.Format("Update shortcut: {0}", shortcut_id)
          //        });
          //    }

          //    return resultData;
          //}

          [HttpPost("update/{shortcut_id}")]
        public ApiResponse Update(string shortcut_id, [FromBody]ShortCutModel data)
        {
            ApiResponse response = new ApiResponse();
     
            try
            {
                data.shortcut = data.shortcut ?? "";
                data.shortcut = data.shortcut.Trim();
                if (string.IsNullOrWhiteSpace(data.shortcut)) { response.msg += "Shortcut cannot be empty" + shortcut_id; }


                data.name = data.name ?? "";
                data.name = data.name.Trim();
                if (string.IsNullOrWhiteSpace(data.name)) { response.msg += "Name cannot be empty" + shortcut_id; }

                var shortcut = _shortcutService.GetById(data.business_id, shortcut_id);
                if (shortcut!=null && !string.IsNullOrWhiteSpace(data.name) && !string.IsNullOrWhiteSpace(data.shortcut))
                {
                    shortcut.name = data.name;
                    shortcut.shortcut = data.shortcut;
                    response.data = _shortcutService.Create(shortcut);
                    response.ok = true;
                }

            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Shortcut",
                    link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
                    details = JsonConvert.SerializeObject(ex.StackTrace),
                    name = string.Format("Update shortcut: {0}", data.name)
                });
            }

            return response;

        }

        [HttpPost("create")]
        public ApiResponse Create([FromBody]ShortCutModel model)
        {
            ApiResponse response = new ApiResponse();
            Shortcut sc = new Shortcut { name = model.name, shortcut = model.shortcut, business_id = model.business_id,created_by = model.created_by };

            try
            {
                sc.shortcut = sc.shortcut ?? "";
                sc.shortcut = sc.shortcut.Trim();
                if (string.IsNullOrWhiteSpace(sc.shortcut)) { response.msg += "Shortcut cannot be empty" + model.business_id; }


                sc.name = sc.name ?? "";
                sc.name = sc.name.Trim();
                if (string.IsNullOrWhiteSpace(sc.name)) { response.msg += "Name cannot be empty" +model.business_id; }

                if (!string.IsNullOrWhiteSpace(model.business_id) && !string.IsNullOrWhiteSpace(sc.name) && !string.IsNullOrWhiteSpace(sc.shortcut))
                {
                    sc.id = Core.Helpers.CommonHelper.GenerateDigitUniqueNumber();
                    sc.business_id = model.business_id;
                    response.data = _shortcutService.Create(sc);
                    response.ok = true;
                }

            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Shortcut",
                    link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
                    details = JsonConvert.SerializeObject(ex.StackTrace),
                    name = string.Format("Add shortcut: {0} to business_id: {1}", sc.name, model.business_id)
                });
            }

            return response;
        }


        //[HttpPost("create/form/{business_id}")]
        //public string CreateFromForm(string business_id, [FromForm]ShortCutModel shortcut)
        //{
        //    Shortcut sc = new Shortcut { message = shortcut.name, shortcut = shortcut.shortcut, business_id = business_id };

        //    string resultData = "";
        //    try
        //    {
        //        sc.shortcut = sc.shortcut ?? "";
        //        sc.shortcut = sc.shortcut.Trim();
        //        if (string.IsNullOrWhiteSpace(sc.shortcut)) return "Shortcut Cannot be empty" + business_id + sc.business_id;


        //        sc.message = sc.message ?? "";
        //        sc.message = sc.message.Trim();
        //        if (string.IsNullOrWhiteSpace(sc.message)) return "Name cannot be empty" + business_id + sc.business_id;

        //        if (!string.IsNullOrWhiteSpace(business_id))
        //        {
        //            sc.id = Core.Helpers.CommonHelper.GenerateDigitUniqueNumber();
        //            sc.business_id = business_id;
        //            resultData = _shortcutService.Create(sc);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logService.Create(new Log
        //        {
        //            message = ex.Message,
        //            category = "Shortcut",
        //            link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
        //            details = JsonConvert.SerializeObject(ex.StackTrace),
        //            name = string.Format("Add shortcut: {0} to business_id: {1}", sc.message, business_id)
        //        });
        //    }

        //    return resultData;
        //}


        [HttpGet("single/{business_id}/{id}")]
        public Shortcut GetById(string id, string business_id)
        {
            var resultData = _shortcutService.GetById(business_id, id);
            if (resultData != null)
            {
                return resultData;
            }
            else return null;
        }

        [HttpPost("delete/{business_id}/{id}")]
        public bool Delete(string business_id, string id)
        {
            var resultData = _shortcutService.Delete(business_id, id);
            return resultData;
        }

        
    }
}
