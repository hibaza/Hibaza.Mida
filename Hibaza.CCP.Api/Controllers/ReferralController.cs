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
using Hangfire;

namespace Hibaza.CCP.Api.Controllers
{


    [Route("referrals")]
    public class ReferralController : Controller
    {
        public IReferralService _referralService;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IBusinessService _businessService;
        private readonly ILoggingService _logService;

        public ReferralController(IReferralService referralService, IOptions<AppSettings> appSettings,IBusinessService businessService, ILoggingService logService)
        {
            _referralService = referralService;
            _appSettings = appSettings;
            _logService = logService;
            _businessService = businessService;
        }

        [HttpGet("copyto/{business_id}")]
        public int CopToSQL(string business_id, [FromQuery]string access_token)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;
            var fb = new FirebaseReferralRepository(new FirebaseFactory(_appSettings));

            foreach (var t in fb.GetAll(business_id, new Paging { Limit = 10 }))
            {
                count++;
                _referralService.Create(t);
            }
            return count;
        }

        [HttpGet("autoupdatecusomterid")]
        public int AutoUpdateCustomerId([FromQuery]int minutes, [FromQuery]string access_token)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;

            RecurringJob.AddOrUpdate<ReferralService>("AutoUpdateCustomerIdForAllReferrals", x => x.UpdateCustomerId(), Cron.MinuteInterval(minutes));
            return count;
        }

        [HttpGet("list/{business_id}/{thread_id}")]
        public async Task<ApiResponse> GetReferrals(string business_id, string thread_id, string access_token)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var resultData = _referralService.GetReferrals(business_id, thread_id, new Paging { Limit = 50 }).Result.Select(r => new ReferralModel(r));
                response.ok = true;
                response.data = resultData;
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Referral",
                    link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
                    details = JsonConvert.SerializeObject(ex.StackTrace),
                    name = string.Format("Get referrals by business_id: {0}", business_id)
                });
            }
            return response;
        }


        [HttpGet("single/{business_id}/{id}")]
        public Referral GetById(string id, string business_id)
        {
            var resultData = _referralService.GetById(business_id, id);
            if (resultData != null)
            {
                return resultData;
            }
            else return null;
        }


        
    }
}
