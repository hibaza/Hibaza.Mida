using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using Hibaza.CCP.Core.TokenProviders;
using Hibaza.CCP.Core;
using Hibaza.CCP.Service;
using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Data.Repositories.Firebase;
using Hibaza.CCP.Data.Providers.Firebase;
using Hibaza.CCP.Domain.Models;
using Microsoft.Net.Http.Headers;
using Firebase.Storage;
using System.IO;

namespace Hibaza.CCP.Api.Controllers
{
    [Route("business")]
    public class BusinessController : Controller
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IBusinessService _businessService;
        private readonly ILoggingService _logService;
        public BusinessController(IOptions<AppSettings> appSettings, IBusinessService businessService, ILoggingService logService)
        {
            _appSettings = appSettings;
            _logService = logService;
            _businessService = businessService;
        }

        [HttpGet("copyto")]
        public int CopToSQL([FromQuery]string access_token)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;
            var fb = new FirebaseBusinessRepository(new FirebaseFactory(_appSettings));

            foreach (var t in fb.GetBusinesses(new Paging { Limit = 100 }))
            {
                count++;
                _businessService.Create(t);
            }
            return count;
        }

        [HttpGet("fb_reset/{id}")]
        public void DeleteFirebaseDataById(string id, [FromQuery]string access_token)
        {
            if (access_token != "@bazavietnam") return;
            var fb = new FirebaseBusinessRepository(new FirebaseFactory(_appSettings));
            fb.DeleteData(id);
        }

        private async Task<string> UploadBusinessLogoToFirebaseStorage(string business_id, string id, Stream stream)
        {
            var task = new FirebaseStorage(_appSettings.Value.FirebaseDB.StorageBucket).Child(business_id).Child("logos").Child(id).PutAsync(stream);
            return await task;
        }

        [HttpPost("update/{business_id}")]
        public ApiResponse UpdateBusiness(string business_id, [FromBody]Business data)
        {
            ApiResponse response = new ApiResponse();
            if (string.IsNullOrWhiteSpace(business_id)) return response;
            var business = _businessService.GetById(business_id);
            if (business != null)
            {
                business.name = data.name;
                business.phone = data.phone;
                business.email = data.email;
                business.zip = data.zip;
                business.city = data.city;
                business.country = data.country;
                business.address = data.address;
                business.updated_time = DateTime.UtcNow;
                business.auto_assign = data.auto_assign;
                business.auto_hide = data.auto_hide;
                business.auto_like = data.auto_like;
                business.auto_message = data.auto_message;
                business.auto_ticket = data.auto_ticket;
                _businessService.Create(business);
                response.ok = true;
                response.data = business;
            }
            return response;
        }

        [HttpPost("save_logo/{id}")]
        public ApiResponse SaveLogo(string id)
        {

            var fileUrl = "";
            var business = _businessService.GetById(id);
            try
            {
                if (business != null)
                {
                    var files = Request.Form.Files;
                    long size = 0;
                    foreach (var file in files)
                    {
                        var filename = ContentDispositionHeaderValue
                                        .Parse(file.ContentDisposition)
                                        .FileName;
                        size += file.Length;
                        var stream = file.OpenReadStream();
                        fileUrl = size > 0 ? UploadBusinessLogoToFirebaseStorage(business.id, Core.Helpers.CommonHelper.GenerateDigitUniqueNumber() + filename, stream).Result : "";
                        break;
                    }
                    if (!string.IsNullOrWhiteSpace(fileUrl))
                    {
                        business.logo = fileUrl;
                        _businessService.Create(business);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new ApiResponse { ok = !string.IsNullOrWhiteSpace(fileUrl), msg = fileUrl };
        }


        [HttpGet("single/{id}")]
        public Business GetById(string id)
        {
            var resultData =  _businessService.GetById(id);
            if (resultData != null)
            {
                return resultData;
            }
            else return null;
        }
    }
}
