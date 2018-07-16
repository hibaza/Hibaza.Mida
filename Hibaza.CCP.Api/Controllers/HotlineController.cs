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
using Hibaza.CCP.Data.Providers.Firebase;
using Hibaza.CCP.Data.Repositories.Firebase;
using Hangfire;
using Hibaza.CCP.Service.Hotline;

namespace Hibaza.CCP.Api.Controllers
{
    [Route("hotline")]
    public class HotlineController : Controller
    {
        public IHotlineService _hotlineService;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IBusinessService _businessService;
        private readonly ILoggingService _logService;
        private readonly IThreadService _threadService;
        public HotlineController(IHotlineService hotlineService, IThreadService threadService, IOptions<AppSettings> appSettings, IBusinessService businessService, ILoggingService logService)
        {
            _hotlineService = hotlineService;
            _appSettings = appSettings;
            _logService = logService;
            _threadService = threadService;
            _businessService = businessService;
        }
        
        [HttpPost("PhoneHook")]
        public async Task<dynamic> getCustomerFormPhone([FromBody]Phone obj)
        {
            try
            {
                _logService.Create(new Log { name = "Hotline", message = "Hotline", details = JsonConvert.SerializeObject(obj) });

                return await _hotlineService.SavePhoneHookData(obj);
            }
            catch (Exception ex) {
                return new { status = false }; }
        }

        [HttpGet("test")]
        public async Task<dynamic> test()
        {
            try
            {
                var tt = "{\"id\":\"1528162537_9141\",\"agent_id\":\"150702502\",\"business_id\":\"150146656\",\"extention\":\"1011\",\"state\":\"Up\",\"ext_id\":\"1528162537.9141\",\"customer_phone\":\"0909668738\",\"trunk\":\"1528162537.9141\",\"incoming\":false,\"timestamp\":1528162764000,\"url_audio\":\"https://phone_files.hibaza.com/api/Files/Details?nameFile=20180606150004667_out-0909668738-1011-20180605-083537-1528162537.9141.WAV\",\"channel_id\":\"150146656_02473003555\"}";
                var gg = JsonConvert.DeserializeObject<Phone>(tt);
                return await _hotlineService.SavePhoneHookData(gg);
            }
            catch (Exception ex)
            {
                return new { status = false };
            }
        }

        [HttpPost("upsertCustomerCallRegistor/{customer_phone}/{customer_fullname}/{token_client}")]
        public async Task<bool> upsertCustomerCallRegistor(string customer_phone, string customer_fullname, string token_client)
        {
            try
            {
                return await upsertCustomerCallRegistor( customer_phone,  customer_fullname,  token_client);
            }
            catch (Exception ex) { return false; }
        }
    }
}
