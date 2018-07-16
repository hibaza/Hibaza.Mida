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
using Hangfire;
using Hibaza.CCP.Data.Repositories.Firebase;
using Hibaza.CCP.Data.Providers.Firebase;
using Hibaza.CCP.Service.SQL;
using Newtonsoft.Json.Linq;
using Hibaza.CCP.Service;

namespace Hibaza.CCP.Api.Controllers
{

    [Route("brands/agents")]
    public class AgentController : Controller
    {
        public IAgentService _agentService;
        public ICustomerService _customerService;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly IBusinessService _businessService;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly ILoggingService _logService;
        private readonly IThreadService _threadService;
        private readonly IFacebookService _facebookService;
        private readonly ITaskService _taskService;
        public AgentController(IAgentService agentService, ITaskService taskService, ICustomerService cusstomerService, IOptions<AppSettings> appSettings, IOptions<JwtIssuerOptions> jwtOptions, IBusinessService businessService, IFacebookService facebookService, IThreadService threadService, ILoggingService logService)
        {
            _agentService = agentService;
            _customerService = cusstomerService;
            _appSettings = appSettings;
            _logService = logService;
            _facebookService = facebookService;
            _jwtOptions = jwtOptions.Value;
            //ThrowIfInvalidOptions(_jwtOptions);
            _businessService = businessService;
            _threadService = threadService;
            _taskService = taskService;
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        [HttpGet("copyto/{business_id}")]
        public int CopToSQL(string business_id, [FromQuery]string access_token)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;
            var fb = new FirebaseAgentRepository(new FirebaseFactory(_appSettings));

            foreach (var t in fb.GetAgents(business_id, new Paging { Limit = 100 }).Result)
            {
                count++;
                _agentService.Create(t.Object);
            }
            return count;
        }

        [HttpGet("autobusy")]
        public int AutoBusy([FromQuery]int minutes, [FromQuery]string access_token)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;

            foreach (var b in _businessService.GetBusinesses(0, 100).Result.Where(b => b.active))
            {
                var business_id = b.id;
                RecurringJob.AddOrUpdate<TaskService>("AutoSetBusyAllInActivityAgentsForBusiness[" + business_id + "]", x => x.SetBusyAllInActivityAgents(business_id, minutes), Cron.MinuteInterval(minutes));
            }
            return count;
        }

        [HttpGet("autologout")]
        public async Task<int> Autologout([FromQuery]int minutes, [FromQuery]string access_token)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;

            foreach (var b in (await _businessService.GetBusinesses(0, 100)).Where(b => b.active))
            {
                var business_id = b.id;
                //await _taskService.LogoutAllInActivityAgents(business_id, minutes);

                RecurringJob.AddOrUpdate<TaskService>("AutologoutAllInActivityAgentsForBusiness[" + business_id + "]", x => x.LogoutAllInActivityAgents(business_id, minutes), Cron.MinuteInterval(minutes));
            }
            return count;
        }

        [Authorize(Policy = "AgentOrAdmin")]
        [HttpGet("check")]
        public string AuthorizeCheck([FromQuery]string access_token)
        {
            return "Authorized";
        }


        [HttpGet("list/{business_id}")]
        public async Task<AgentFeed> GetAgents(string business_id, [FromQuery]string access_token)
        {
            try
            {
                var resultData = await _agentService.GetAgents(business_id, 0, 50);
                return new AgentFeed { Data = resultData.Select(a => new AgentModel(a)) };
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Agent",
                    link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
                    details = JsonConvert.SerializeObject(ex.StackTrace),
                    name = string.Format("Get agents by business_id: {0}", business_id)
                });
            }
            return null;
        }

        [HttpPost("set_status/{id}/{status}")]
        public bool SetWorkStatus(string id, string status)
        {
            var agent = _agentService.SetWorkStatus(id, status, DateTime.UtcNow);
            // var limit = Convert.ToInt32(_appSettings.Value.LimitAssign);
            //if (agent != null) _taskService.AutoAssignToAvailableAgents(agent.business_id, new Paging { Limit = limit});
            return agent != null;
        }

        [HttpPost("set_status/{business_id}/{id}/{status}")]
        public bool SetWorkStatus(string business_id, string id, string status)
        {
            return SetWorkStatus(id, status);
        }

        [HttpPost("set_login_status/{id}/{status}")]
        public bool SetLoginStatus(string id, string status)
        {
            var agent = _agentService.SetLoginStatus(id, status, DateTime.UtcNow);

            //if (agent != null) _taskService.AutoAssignToAvailableAgents(agent.business_id, new Paging { Limit = 3 });
            return agent != null;
        }

        [HttpPost("toogle_admin/{business_id}/{id}")]
        public bool ToogleAdmin(string business_id, string id)
        {
            var resultData = "";
            resultData = _agentService.ToogleRole(business_id, id, "admin");
            return true;
        }

        [HttpPost("update/{id}")]
        public string UpdateAgent(string id, [FromForm]Agent data)
        {
            var resultData = "";
            if (string.IsNullOrWhiteSpace(id)) return resultData;
            var agent = _agentService.GetById(id);
            if (agent != null && data.password == data.password_confirmation)
            {
                agent.first_name = data.first_name;
                agent.last_name = data.last_name;
                agent.avatar = data.avatar;
                agent.password = data.password ?? agent.password;
                agent.password_confirmation = agent.password;
                agent.email = data.email;
                resultData = _agentService.Create(agent);
            }
            return resultData;
        }

        private async Task<string> UploadAvatarToFirebaseStorage(string business_id, string id, string fileName, Stream stream)
        {
            var task = new FirebaseStorage(_appSettings.Value.FirebaseDB.StorageBucket).Child(business_id).Child("avatars").Child("agents").Child(id).Child(fileName).PutAsync(stream);
            return await task;
        }


        [HttpPost("save_avatar/{id}")]
        public ApiResponse SaveAvatar(string id)
        {

            var fileUrl = "";
            var agent = _agentService.GetById(id);
            try
            {
                if (agent != null)
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
                        fileUrl = size > 0 ? UploadAvatarToFirebaseStorage(agent.business_id, agent.id, Core.Helpers.CommonHelper.GenerateDigitUniqueNumber(), stream).Result : "";
                        break;
                    }
                    if (!string.IsNullOrWhiteSpace(fileUrl))
                    {
                        agent.avatar = fileUrl;
                        _agentService.Create(agent);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new ApiResponse { ok = !string.IsNullOrWhiteSpace(fileUrl), msg = fileUrl };
        }

        [HttpPost("save_token/{id}")]
        public ApiResponse SaveToken(string id, [FromBody]string uid, [FromBody]string token)
        {
            ApiResponse response = new ApiResponse();
            if (string.IsNullOrWhiteSpace(uid))
            {
                response.msg = "UserIid required";
                return response;
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                response.msg = "Token required";
                return response;
            }

            var agent = _agentService.GetById(id);
            try
            {
                agent.ext_id = uid;
                agent.facebook_access_token = _facebookService.GetLonglivedToken(_appSettings.Value.ClientId, _appSettings.Value.ClientSecret, token) ?? token;
                response.data = _agentService.Create(agent);
                response.ok = true;
            }
            catch (Exception ex)
            {
                response.ok = false;
                response.msg = ex.Message;
                throw ex;
            }
            return response;
        }


        [HttpPost("save_name/{id}")]
        public ApiResponse SaveName(string id, [FromForm]string first_name, [FromForm]string last_name)
        {
            ApiResponse response = new ApiResponse();
            first_name = first_name ?? "";
            first_name = first_name.Trim();
            if (string.IsNullOrWhiteSpace(first_name))
            {
                response.msg = "First name required";
                return response;
            }
            last_name = last_name ?? "";
            last_name = last_name.Trim();
            if (string.IsNullOrWhiteSpace(last_name))
            {
                response.msg = "Last name required";
                return response;
            }

            var agent = _agentService.GetById(id);
            try
            {
                agent.first_name = first_name;
                agent.last_name = last_name;
                response.data = _agentService.Create(agent);
                response.ok = true;
            }
            catch (Exception ex)
            {
                response.ok = false;
                response.msg = ex.Message;
                throw ex;
            }
            return response;
        }

        [HttpPost("reset/{username}")]
        public ApiResponse ResetPassword(string username)
        {
            ApiResponse response = new ApiResponse { };
            var agent = _agentService.GetSingleOrDefaultByUserName(username);
            if (agent == null)
            {
                response.msg = "Account not existed";
                return response;
            }
            //TODO: Send reminder
            response.ok = true;
            return response;
        }

        [HttpPost("invite/{id}")]
        public ApiResponse Invite(string id, [FromBody]string emails)
        {
            ApiResponse response = new ApiResponse();
            emails = emails ?? "";
            foreach (var e in emails.Split(','))
            {
                var em = e.Trim().ToLower();

                if (Core.Helpers.CommonHelper.ValidEmail(em))
                {
                    //TODO: Send invitation
                }
            }
            response.ok = true;
            return response;
        }


        [HttpPost("register")]
        public ApiResponse RegisterAdmin([FromForm]Agent agent)
        {
            ApiResponse response = new ApiResponse();

            if (string.IsNullOrWhiteSpace(agent.password))
            {
                response.msg = "Password not provided";
                return response;
            }

            if (agent.password != agent.password_confirmation)
            {
                response.msg = "Passwords are mismatched";
                return response;
            }
            agent.name = string.IsNullOrWhiteSpace(agent.name) ? agent.last_name + " " + agent.first_name : agent.name;
            agent.business_name = agent.business_name ?? "";
            agent.business_name = agent.business_name.Trim();
            if (string.IsNullOrWhiteSpace(agent.business_name))
            {
                response.msg = "Business name required";
                return response;
            }

            agent.email = agent.email ?? "";
            agent.email = agent.email.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(agent.email) || !agent.email.Contains("@"))
            {
                response.msg = "Invalid email address";
                return response;
            }
            if (!string.IsNullOrWhiteSpace(agent.facebook_access_token))
            {
                try
                {
                    var uri = "https://graph.facebook.com/v2.10/me?fields=id,name,email,age_range,first_name,gender,last_name,picture{url},birthday,address&access_token=" + agent.facebook_access_token;
                    var c = Core.Helpers.WebHelper.HttpGetAsync<JObject>(uri).Result;
                    if (c != null && c["id"] != null)
                    {
                        agent.ext_id = (string)c["id"];
                        if (c["picture"] != null && c["picture"]["data"] != null && c["picture"]["data"]["url"] != null)
                        {
                            var imageUrl = ImagesService.UpsertImageStore((string)c["picture"]["data"]["url"], _appSettings.Value).Result;
                            agent.avatar = imageUrl;
                        }
                    }
                }
                catch (Exception ex) { }
            }
            var business = new Business
            {
                name = agent.business_name,
                type = "company",
                active = true,
                logo = agent.avatar,
                email = agent.email,
                ext_id = agent.ext_id,
                token = agent.facebook_access_token
            };

            var businessCheck = _businessService.GetByEmail(agent.email);
            if (businessCheck == null || businessCheck.id == null)
                agent.business_id = _businessService.Create(business);
            else
                agent.business_id = businessCheck.id;
            agent.username = agent.email;
            agent.login_status = "offline";
            agent.active = true;
            agent.status = "offline";
            agent.role = "admin";

            //add hotline demo
            try
            {
                var uri = _appSettings.Value.BaseUrls.ApiHotline + "api/PhoneAccounts/PhoneAccountNotUsingDemo";
                var obj = Core.Helpers.WebHelper.HttpGetAsync<dynamic>(uri).Result;
                var bson = Core.Helpers.CommonHelper.JsonToBsonDocument((string)obj);
                foreach (var item in bson)
                {
                    agent.phone_account_id = item.Name;
                    break;
                }

            }
            catch (Exception ex) { }

            response.ok = true;
            response.msg = agent.business_id;
            try
            {
                var current = _agentService.GetSingleOrDefaultByUserName(agent.username);
                if (current == null)
                {
                    response.data = _agentService.Create(agent);
                }

            }
            catch (Exception ex)
            {
                response.msg = ex.Message;
                throw ex;
            }

            return response;
        }


        [HttpPost("create/{business_id}")]
        public string CreateAgent(string business_id, [FromForm]Agent agent)
        {

            string resultData = "";
            try
            {
                agent.email = agent.email ?? "";
                agent.email = agent.email.Trim().ToLower();
                if (string.IsNullOrWhiteSpace(agent.email) || !agent.email.Contains("@")) return "Invalid email address";
                agent.username = agent.email;
                var current = _agentService.GetSingleOrDefaultByUserName(agent.username);
                if (current == null)
                {
                    if (string.IsNullOrWhiteSpace(agent.password)) return "Password not provided";
                    if (agent.password != agent.password_confirmation) return "Passwords are mismatched";
                    var business = _businessService.GetById(business_id);
                    if (business == null) return "Agent not found";
                    agent.business_name = business.name;
                    agent.business_id = business_id;
                    agent.active = false;
                    agent.login_status = "offline";
                    agent.status = "offline";
                    agent.role = "agent";
                    resultData = _agentService.Create(agent);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(current.role) || current.role != "admin")
                    {
                        current.role = "agent";
                        resultData = _agentService.Create(current);
                    }
                    else
                    {
                        return "User aldready existed";
                    }
                }

            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Agent",
                    link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
                    details = JsonConvert.SerializeObject(ex.StackTrace),
                    name = string.Format("Add agent-id: {0} to business_id: {1}", agent.username, business_id)
                });
                throw ex;
            }

            return resultData;
        }


        [HttpGet("single/{id}")]
        public AgentModel GetById(string id, [FromQuery]string access_token)
        {
            var resultData = _agentService.GetById(id);
            if (resultData != null)
            {
                return new AgentModel(resultData);
            }
            else return null;
        }

        [HttpPost("delete/{business_id}/{id}")]
        public ApiResponse Delete(string business_id, string id)
        {
            ApiResponse response = new ApiResponse();
            _threadService.UnAssignAllUnreadThreadsFromAgent(business_id, id);
            response.ok = _agentService.Delete(business_id, id);
            return response;
        }

        [Authorize(Policy = "AgentOrAdmin")]
        [HttpGet("info")]
        public object UserInformation()
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == "user_id").Value;

            // Get the name
            string name = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            return new
            {
                UserId = userId,
                Name = name
            };
        }

        [HttpGet("auth")]
        [AllowAnonymous]
        public async Task<IActionResult> Token([FromQuery]string username, [FromQuery]string password)
        {

            var json = "";
            var identity = _agentService.GetSingleOrDefaultByUserName(username);

            if (identity == null || identity.password != password)
            {

                //return BadRequest("Invalid credentials");
            }
            else
            {
                var claims = new[]
                {
        new Claim(JwtRegisteredClaimNames.Sub, username),
        new Claim(ClaimTypes.Role, identity.role),
        new Claim("user_id", identity.id),
        new Claim("name", identity.first_name + ' ' + identity.last_name),
        new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
        new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64)
                };

                // Create the JWT security token and encode it.
                var jwt = new JwtSecurityToken(
                    issuer: _jwtOptions.Issuer,
                    audience: _jwtOptions.Audience,
                    claims: claims,
                    notBefore: _jwtOptions.NotBefore,
                    expires: _jwtOptions.Expiration,
                    signingCredentials: _jwtOptions.SigningCredentials);

                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
                var response = new
                {
                    user_id = identity.id,
                    access_token = encodedJwt,
                    expires_in = (int)_jwtOptions.ValidFor.TotalSeconds
                };
                json = JsonConvert.SerializeObject(response, _serializerSettings);
            }
            //Hibaza.CCP.Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.Today.AddDays(2))
            identity.last_acted_time = DateTime.UtcNow;
            identity.last_loggedin_time = DateTime.UtcNow;
            identity.status = "busy";
            identity.login_status = "online";
            _agentService.Create(identity);

            return new OkObjectResult(json);
        }

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);


    }
}
