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
using Hibaza.CCP.Domain.Models.Facebook;
using Firebase.Storage;
using Microsoft.Net.Http.Headers;
using System.IO;
using Newtonsoft.Json;
using Hangfire;
using Hibaza.CCP.Service.Firebase;
using Hibaza.CCP.Data.Repositories.Firebase;
using Hibaza.CCP.Data.Providers.Firebase;
using Hibaza.CCP.Service.SQL;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text;
using Hibaza.CCP.Data;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace Hibaza.CCP.Api.Controllers
{

    public class CustomerFilter
    {
        public long first { get; set; }
        public int quantity { get; set; }
        public string agent_id { get; set; }
        public string channel_id { get; set; }
        public string status { get; set; }
        public string flag { get; set; }
    }


    [Route("customers")]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IMessageService _messageService;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ILoggingService _logService;
        private readonly IFacebookService _facebookService;
        private readonly IChannelService _channelService;
        private readonly IAgentService _agentService;
        private readonly IThreadService _threadService;
        private readonly IReferralService _referralService;
        private readonly INoteService _noteService;
        private readonly ITicketService _ticketService;
        private readonly IBusinessService _businessService;
        private readonly IFacebookConversationService _facebookConversationService;
        Data.Common _cm = new Data.Common();

        public CustomerController(ICustomerService customerService, IFacebookConversationService facebookConversationService, IBusinessService businessService, ITicketService ticketService, INoteService noteService, IReferralService referralService, IThreadService threadService, IAgentService agentService, IMessageService messageService, IFacebookService facebookService, IChannelService channelService, IOptions<AppSettings> appSettings, ILoggingService logService)
        {
            _customerService = customerService;
            _messageService = messageService;
            _appSettings = appSettings;
            _channelService = channelService;
            _facebookService = facebookService;
            _agentService = agentService;
            _threadService = threadService;
            _referralService = referralService;
            _noteService = noteService;
            _ticketService = ticketService;
            _businessService = businessService;
            _facebookConversationService = facebookConversationService;
            _logService = logService;
        }

        [HttpGet("autoassign")]
        public int AutoAssignToAgents([FromQuery]int minutes, [FromQuery]int limit, [FromQuery]string access_token)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;

            foreach (var b in _businessService.GetBusinesses(0, 100).Result.Where(b => b.active))
            {
                count++;
                var business_id = b.id;
                // _customerService.AutoAssignToAvailableAgents(business_id, new Paging { Limit = limit });
                RecurringJob.AddOrUpdate<CustomerService>("AutoAssignToAgentsForBusiness[" + business_id + "]", x => x.AutoAssignToAvailableAgents(business_id, new Paging { Limit = limit }), Cron.MinuteInterval(minutes));
            }
            return count;
        }

        //[HttpGet("copyto/{business_id}")]
        //public int CopToSQL(string business_id, [FromQuery]string access_token)
        //{
        //    int count = 0;

        //    if (access_token != "@bazavietnam") return count;

        //    var fb = new FirebaseCustomerRepository(new FirebaseFactory(_appSettings));

        //    foreach (var t in fb.GetCustomers(business_id, new Paging { Limit = 1000000 }).Result)
        //    {
        //        count++;
        //        _customerService.CreateCustomer(t.Object);
        //    }
        //    return count;
        //}

        //[HttpGet("copyfromthreads/{business_id}")]
        //public int CreateFromThread(string business_id, [FromQuery]string access_token)
        //{
        //    int count = 0;

        //    if (access_token != "@bazavietnam") return count;


        //    foreach (var t in _threadService.GetThreads(business_id, "", "", "", "", "", new Paging { Limit = 1000000 }).Result)
        //        if (!string.IsNullOrWhiteSpace(t.customer_id))
        //        {
        //            count++;
        //            Customer c = new Customer
        //            {
        //                id = t.customer_id,
        //                app_id = t.owner_app_id,
        //                ext_id = t.owner_ext_id,
        //                avatar = t.owner_avatar
        //            };
        //            _customerService.CreateCustomer(c);
        //        }
        //    return count;
        //}

        [HttpGet("findphonenumber/{business_id}")]
        public int FindPhoneNumber(string business_id, [FromQuery]string customer_id, [FromQuery]string access_token, [FromQuery]int limit = 100)
        {

            int count = 0;
            if (access_token != "@bazavietnam") return count;
            if (!string.IsNullOrWhiteSpace(customer_id))
            {
                var customer = _customerService.GetById(business_id, customer_id);
                if (customer != null)
                {
                    List<string> phoneList = customer.phone_list == null ? new List<string>() : customer.phone_list;
                    foreach (var t in _messageService.GetByCustomer(business_id, customer_id, new Paging { Limit = limit }).Result.OrderBy(m => m.timestamp))
                        if (!string.IsNullOrWhiteSpace(t.message) && t.sender_ext_id != t.channel_ext_id)
                        {
                            phoneList.AddRange(Core.Helpers.CommonHelper.FindPhoneNumbers(t.message));
                        }
                    var lastPhoneNumber = phoneList.Count() > 0 ? phoneList.Last() : customer.phone;
                    if (!string.IsNullOrWhiteSpace(lastPhoneNumber))
                    {
                        count++;
                        _customerService.UpdatePhoneNumber(business_id, customer_id, phoneList, lastPhoneNumber);
                    }

                }
            }
            else
            {
                var customers = _customerService.SearchCustomers(business_id, "", "", "", "", "", new Paging { Limit = limit }).Result.ToDictionary(a => a.id, b => b.phone_list == null ? new List<string>() : b.phone_list);

                foreach (var t in _messageService.All(business_id, new Paging { Limit = limit }).Result)
                    if (t.sender_ext_id != t.channel_ext_id && !string.IsNullOrWhiteSpace(t.customer_id) && !string.IsNullOrWhiteSpace(t.message) && customers.ContainsKey(t.customer_id))
                    {
                        var newPhoneList = Core.Helpers.CommonHelper.FindPhoneNumbers(t.message);
                        if (newPhoneList.Count() > 0)
                        {
                            //tam bo
                            // customers[t.customer_id] = customers[t.customer_id].Union(newPhoneList);
                        }

                    }
                foreach (var c in customers)
                    if (c.Value != null && c.Value.Count() > 0)
                    {
                        count++;
                        var lastPhoneNumber = c.Value.Last();
                        _customerService.UpdatePhoneNumber(business_id, c.Key, c.Value, lastPhoneNumber);
                        //var jobId = BackgroundJob.Enqueue<CustomerService>(x => x.UpdatePhoneNumber(business_id, customer_id, c.Value, lastPhoneNumber));
                    }
            }
            return count;
        }

        [HttpGet("refresh/{business_id}")]
        public int Refresh(string business_id, [FromQuery]string customer_id, [FromQuery]string access_token, [FromQuery]string status = "", [FromQuery]int limit = 100)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;
            var jobId = "";
            if (!string.IsNullOrWhiteSpace(customer_id))
            {
                _customerService.RefreshCustomer(business_id, customer_id);

            }
            else
            {
                foreach (var t in _customerService.SearchCustomers(business_id, "", "", status, "", "", new Paging { Limit = limit }).Result)
                {
                    count++;
                    //_customerService.RefreshCustomer(business_id, t.id);
                    jobId = BackgroundJob.Enqueue<CustomerService>(x => x.RefreshCustomer(business_id, t.id));
                }

                //foreach (var t in _threadService.GetThreads(business_id, "", "", status, "", "", new Paging { Limit = limit }).Result)
                //    if (!string.IsNullOrWhiteSpace(t.customer_id))
                //    {
                //        count++;
                //        //_customerService.RefreshCustomer(business_id, t.customer_id, t);
                //        jobId = BackgroundJob.Enqueue<CustomerService>(x => x.RefreshCustomer(business_id, t.customer_id, t));
                //    }
            }
            return count;
        }

        [HttpGet("autoupdateunreadcustomercounters")]
        public int AutoUpdateUnreadCustomerCounters([FromQuery]int minutes, [FromQuery]string access_token)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;

            foreach (var b in _businessService.GetBusinesses(0, 100).Result.Where(b => b.active))
            {
                count++;
                var business_id = b.id;
                RecurringJob.AddOrUpdate<CustomerService>("AutoUpdateUnreadCustomerCountersForBusiness[" + business_id + "]", x => x.BatchUpdateUnreadCounters(business_id), Cron.MinuteInterval(minutes));
            }
            return count;
        }

        [HttpGet("autoupdatestatus")]
        public int AutoUpdateStatus([FromQuery]string business_id, [FromQuery]int minutes, [FromQuery]string access_token, [FromQuery]string status = "", [FromQuery]int limit = 1000)
        {
            int count = 0;
            if (access_token != "@bazavietnam") return count;
            RecurringJob.AddOrUpdate<CustomerService>("AutoUpdate" + (status ?? "") + "CustomerStatusForBusiness[" + business_id + "]", x => x.UpdateCustomerStatus(business_id, status, limit), Cron.MinuteInterval(minutes));
            return count;
        }

        //[HttpGet("set_user_id/{business_id}")]
        //public int UpdateUserId(string business_id, [FromQuery]string thread_id, [FromQuery]int limit, [FromQuery]string access_token)
        //{
        //    int count = 0;

        //    if (access_token != "@bazavietnam") return count;
        //    if (!string.IsNullOrWhiteSpace(thread_id))
        //    {

        //    }
        //    else
        //    {
        //        count = _customerService.UpdateUserId(business_id, limit);
        //    }
        //    return count;
        //}

        [HttpGet("getidsforpages")]
        public async Task<int> GetIdForPages([FromQuery]string business_id, [FromQuery]int limit, [FromQuery]string end = "9999999999")
        {
            int count = 0;
            foreach (var customer in await _customerService.GetCustomersWhereExtIdIsNull(business_id, new Paging { Limit = limit, Next = end }))
                if (!string.IsNullOrWhiteSpace(customer.app_id))
                {
                    var channel = _channelService.GetById(business_id, customer.channel_id);
                    if (channel != null)
                    {
                        var psid = await _facebookConversationService.GetPageScopedId(customer.app_id, channel.ext_id);
                        if (!string.IsNullOrWhiteSpace(psid))
                        {
                            var c = _customerService.GetById(business_id, customer.id);
                            c.ext_id = psid;
                            _customerService.CreateCustomer(c, false);
                        }
                    }
                }
            return count;
        }


        [HttpGet("profile_ext_url/{business_id}/{thread_id}")]
        public ApiResponse GetExtProfileUrl(string business_id, string thread_id)
        {
            ApiResponse response = new ApiResponse();
            string app_id = null;
            var thread = _threadService.GetById(business_id, thread_id);
            if (thread != null)
            {
                app_id = thread.owner_app_id;
                if (string.IsNullOrWhiteSpace(app_id))
                {
                    var customer = _customerService.GetById(business_id, thread.customer_id);
                    if (customer != null)
                    {
                        app_id = customer.app_id;
                    }

                }
            }
            response.ok = !string.IsNullOrWhiteSpace(app_id);
            response.data = "https://facebook.com/" + (app_id ?? "");

            return response;
        }


        [HttpGet("profile/{business_id}/{id}")]
        public async Task<CustomerProfileModel> GetCustomerProfile(string business_id, string id)
        {
            try
            {
                List<System.Threading.Tasks.Task> tasks = new List<System.Threading.Tasks.Task>();
                CustomerProfileModel model = new CustomerProfileModel();
                //var key = "GetCustomerProfile" + business_id + id;
                //var lst = await CacheBase.cacheManagerGet<CustomerProfileModel>(key);
                //if (lst != null)
                //    return lst;
                Customer c = null;
                FacebookUserProfile profile = null;
                var date1 = DateTime.UtcNow;
                Thread thread = null;
                //tasks.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
                //{
                c = _customerService.GetById(business_id, id);
                if (c == null)
                {
                    thread = _threadService.GetById(business_id, id);
                    if (thread != null)
                    {
                        c = _customerService.GetById(business_id, thread.customer_id);
                    }
                }
                //}));

                //System.Threading.Tasks.Task.WaitAll(tasks.ToArray());

                if (c == null) return model;
                if (c != null)
                    if (c.active_thread == null)
                    {
                        thread = _threadService.GetByIdFromCustomerId(business_id, c.id);
                        if (thread != null)
                        {
                            c.active_thread = JsonConvert.SerializeObject(thread);
                            System.Threading.Tasks.Task.Factory.StartNew(() =>
                             {
                                 _customerService.CreateCustomer(c, false);
                             });
                        }
                    }

                var sp = c.channel_id.Split('_');
                var channel_ext_id = sp[sp.Length - 1];

                #region lay ten that neu da tao don hang
                if (string.IsNullOrWhiteSpace(c.real_name))
                {
                    c.real_name = c.name;
                    _customerService.UpdateRealName(c.business_id, c.id, c.name);
                }
                if (c.real_name == c.name && !string.IsNullOrWhiteSpace(c.phone))
                {
                    try
                    {
                        System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            var uri = _appSettings.Value.BaseUrls.ApiOrder + "api/order/list?logonId=" + c.phone + "&imei=&token=@bazavietnam";
                            var orders = Core.Helpers.WebHelper.HttpGetAsyncSting(uri).Result;
                            if (orders.Length > 10)
                            {
                                var orderBs = JsonConvert.DeserializeObject(orders);
                                var array = (JArray)orderBs;
                                var orderItem = array[array.Count - 1];
                                {
                                    if (orderItem["OrderId"].ToString() != "")
                                    {
                                        var urlDetail = _appSettings.Value.BaseUrls.ApiOrder + "api/order/detail2?id=" + orderItem["OrderId"] + "&logonId=" + c.phone + "&imei=&token=@bazavietnam";
                                        var orderDetail = Core.Helpers.WebHelper.HttpGetAsyncSting(urlDetail).Result;

                                        var obj = JsonConvert.DeserializeObject<JToken>(orderDetail);
                                        if (obj != null)
                                        {
                                            var real_name = (string)obj["Fullname"];
                                            if (!string.IsNullOrWhiteSpace(real_name))
                                                _customerService.UpdateRealName(c.business_id, c.id, real_name);
                                        }

                                    }
                                }
                            }
                        });
                    }
                    catch (Exception ex) { }
                }
                #endregion

                var customer = new CustomerModel(c);

                var customer_id1 = "";
                var customer_id2 = customer.id;
                model.id = customer.id;
                model.customer_id = customer.id;

                if (string.IsNullOrWhiteSpace(customer.sex) || (!string.IsNullOrWhiteSpace(customer.avatar) && customer.avatar.IndexOf(_appSettings.Value.BaseUrls.ApiSaveImage)<0))
                {
                    tasks.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            var channel = _channelService.GetById(business_id, customer.channel_id);
                            profile = _facebookConversationService.GetProfile(customer.active_thread.ext_id, channel.token);
                            if (profile != null && !string.IsNullOrWhiteSpace(profile.sex))
                            {
                                c.sex = profile.sex;
                                if (string.IsNullOrWhiteSpace(customer.avatar) || (!string.IsNullOrWhiteSpace(customer.avatar) && customer.avatar.IndexOf(_appSettings.Value.BaseUrls.ApiSaveImage) < 0))
                                    c.avatar =  ImagesService.UpsertImageStore(profile.avatar, _appSettings.Value).Result;
                                _customerService.CreateCustomer(c, false);
                            }
                        }
                        catch { }
                    }));
                }

                tasks.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                model.referrals = new List<ReferralModel>();
                if (!string.IsNullOrWhiteSpace(customer_id1))
                {
                    foreach (var item in _referralService.GetReferralsByCustomer(business_id, customer_id1, new Paging { Limit = 5 }).Result)
                    {
                        model.referrals.Add(new ReferralModel(item));
                    }
                }
                if (customer_id2 != customer_id1 && !string.IsNullOrWhiteSpace(customer_id2))
                {
                    foreach (var item in _referralService.GetReferralsByCustomer(business_id, customer_id2, new Paging { Limit = 5 }).Result)
                    {
                        model.referrals.Add(new ReferralModel(item));
                    }
                    model.referrals = model.referrals.OrderByDescending(r => r.created_time).Take(5).ToList();
                }
            }));
                tasks.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    model.id = id;
                    model.name = customer.name;
                    model.blocked = customer.blocked;
                    model.phone = !string.IsNullOrWhiteSpace(customer.phone) ? customer.phone : "";
                    model.avatar = customer.avatar != null && customer.avatar.IndexOf("http://") < 0 && customer.avatar.IndexOf("https://") < 0 ? _appSettings.Value.BaseUrls.Api + customer.avatar : customer.avatar;
                    model.profile_ext_url = "/customers/openprofile/" + business_id + "/" + id;
                    model.openlink = "/" + business_id + "/messages/openlink/?thread_id=" + customer.active_thread.id;
                    model.last_contacted_since = Core.Helpers.CommonHelper.UnixTimestampToDateTime(customer.timestamp).ToLocalTime().ToString("dd/MM/yyyy");
                    model.last_visits = customer.active_thread.last_visits;
                    model.sex = !string.IsNullOrWhiteSpace(customer.sex) ? customer.sex : "";
                    model.weight = customer.weight;
                    model.height = customer.height;
                    model.address = !string.IsNullOrWhiteSpace(customer.address) ? customer.address : "";
                    model.phone_list = c.phone_list;
                }));

                tasks.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    model.notes = new List<NoteModel>();
                    if (!string.IsNullOrWhiteSpace(customer_id1))
                    {
                        foreach (var item in _noteService.GetCustomerNotes(business_id, customer_id1, new Paging { Limit = 10 }))
                        {
                            model.notes.Add(new NoteModel(item));
                        }
                    }
                    if (customer_id2 != customer_id1 && !string.IsNullOrWhiteSpace(customer_id2))
                    {
                        foreach (var item in _noteService.GetCustomerNotes(business_id, customer_id2, new Paging { Limit = 10 }))
                        {
                            model.notes.Add(new NoteModel(item));
                        }
                        model.notes = model.notes.OrderByDescending(n => n.created_time).Take(10).ToList();
                    }
                }));

                tasks.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    model.tickets = new List<TicketModel>();
                    if (!string.IsNullOrWhiteSpace(customer_id1))
                    {
                        foreach (var item in _ticketService.GetCustomerTickets(business_id, customer_id1, new Paging { Limit = 10 }))
                        {
                            model.tickets.Add(new TicketModel(item));
                        }
                    }
                    if (customer_id2 != customer_id1 && !string.IsNullOrWhiteSpace(customer_id2))
                    {
                        foreach (var item in _ticketService.GetCustomerTickets(business_id, customer_id2, new Paging { Limit = 10 }))
                        {
                            model.tickets.Add(new TicketModel(item));
                        }
                        model.tickets = model.tickets.OrderByDescending(t => t.created_time).Take(10).ToList();
                    }
                }));

                tasks.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    model.starred_messages = new List<MessageModel>();
                    if (!string.IsNullOrWhiteSpace(customer_id1))
                    {
                        foreach (var item in _messageService.GetStarredMesagesByCustomer(business_id, new Paging { Limit = 10 }, customer_id1).Result)
                        {
                            model.starred_messages.Add(new MessageModel(item));
                        }
                    }

                    if (customer_id2 != customer_id1 && !string.IsNullOrWhiteSpace(customer_id2))
                    {
                        foreach (var item in _messageService.GetStarredMesagesByCustomer(business_id, new Paging { Limit = 10 }, customer_id2).Result)
                        {
                            model.starred_messages.Add(new MessageModel(item));
                        }
                        model.starred_messages = model.starred_messages.OrderByDescending(m => m.timestamp).Take(10).ToList();
                    }
                }));


                tasks.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
               {
                   model.last_messages = new List<MessageModel>();
                   if (!string.IsNullOrWhiteSpace(customer_id1))
                   {
                       foreach (var item in _messageService.GetByCustomerExcludeCurrentThread(business_id, customer_id1, channel_ext_id, new Paging { Limit = 10, Next = "9999999999" }).Result)
                       {
                           item.sender_name = item.sender_ext_id == item.channel_ext_id ? item.sender_name : customer.name;
                           model.last_messages.Add(new MessageModel(item));
                       }
                   }

                   if (customer_id2 != customer_id1 && !string.IsNullOrWhiteSpace(customer_id2))
                   {
                       foreach (var item in _messageService.GetByCustomerExcludeCurrentThread(business_id, customer_id2, channel_ext_id, new Paging { Limit = 10, Next = "9999999999" }).Result)
                       {
                           item.sender_name = item.sender_ext_id == item.channel_ext_id ? item.sender_name : customer.name;
                           model.last_messages.Add(new MessageModel(item));
                       }
                       model.last_messages = model.last_messages.OrderByDescending(m => m.timestamp).Take(10).ToList();
                   }
               }));

                System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
                model.sex = string.IsNullOrWhiteSpace(model.sex) && profile != null && !string.IsNullOrWhiteSpace(profile.sex) ? profile.sex : model.sex;
                model.avatar = string.IsNullOrWhiteSpace(model.avatar) && profile != null && !string.IsNullOrWhiteSpace(profile.avatar) ? profile.avatar : model.avatar;

                //  CacheBase.cacheManagerSetForProceduce(key, model, DateTime.Now.AddMinutes(10), null, null, false);
                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [Authorize(Policy = "AgentOrAdmin")]
        [HttpPost("assign/{business_id}/{agent_id}/{customer_id}")]
        public bool AssignToAgent(string business_id, string agent_id, string customer_id)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == "user_id").Value;
                return _customerService.AssignToAgent(business_id, customer_id, agent_id, userId);
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    category = "Customers",
                    link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
                    message = ex.Message,
                    name = string.Format("Assign customer-{0} to agent-{1}", customer_id, agent_id)
                });
                throw ex;
            }
        }

        [Authorize(Policy = "AgentOrAdmin")]
        [HttpPost("unassign/{business_id}/{customer_id}")]
        public bool UnAssignFromAgent(string business_id, string customer_id)
        {
            try
            {
                var customer = _customerService.UnAssignFromAgent(business_id, customer_id);
                return customer != null;
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    category = "Customers",
                    link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
                    message = ex.Message,
                    name = string.Format("UnAssign customer-{0} from agent", customer_id)
                });
                throw ex;
            }
        }



        [HttpGet("info/{id}")]
        public CustomerContactInfoModel Info(string id, [FromQuery]string business_id)
        {
            return new CustomerContactInfoModel(_customerService.GetById(business_id, id));
        }

        [HttpGet("profileinfo/{id}/{business_id}")]
        public async Task<CustomerContactInfoModel> profileinfo(string id, string business_id)
        {
            return new CustomerContactInfoModel(await _customerService.GetCustomerId(business_id, id));
        }


        [HttpPost("info/{id}")]
        public ApiResponse UpdateContactInfo(string id, [FromBody]CustomerContactInfoModel data, [FromQuery]string business_id)
        {
            ApiResponse response = new ApiResponse();
            data.real_name = data.real_name ?? "";
            data.real_name = data.real_name.Trim();

            if (string.IsNullOrWhiteSpace(data.real_name)) { response.msg = "Real name cannot be empty"; return response; }

            data.name = data.name ?? "";
            data.name = data.name.Trim();
            if (string.IsNullOrWhiteSpace(data.name)) { response.msg = "Nick name cannot be empty"; return response; }

            data.phone = data.phone ?? "";
            data.phone = data.phone.Trim().ToLower();
            if (!string.IsNullOrWhiteSpace(data.phone))
            {
                data.phone = Core.Helpers.CommonHelper.FindPhoneNumbers(data.phone).SingleOrDefault();
                if (string.IsNullOrWhiteSpace(data.phone)) { response.msg = "Phone number is invalid"; return response; }
            }

            data.email = data.email ?? "";
            data.email = data.email.Trim().ToLower();
            //if (!string.IsNullOrWhiteSpace(data.email))
            //{
            //if (Core.Helpers.CommonHelper.ValidEmail(data.email))
            //{
            //   response.msg = "Email address is invalid"; return response;
            //}
            // }
            _customerService.UpdateContactInfo(business_id, id, data);
            response.ok = true;
            return response;
        }

        [HttpGet("get/{id}")]
        public CustomerModel GetById(string id, [FromQuery]string business_id)
        {

            return new CustomerModel(_customerService.GetById(business_id, id));
        }

        [HttpPost("block/{business_id}/{id}")]
        public bool Block(string business_id, string id)
        {
            var customer = _customerService.GetById(business_id, id);
            bool blocked = !customer.blocked;
            if (blocked)
            {
                foreach (var bc in _channelService.GetChannels(business_id, 0, 100).Result.Where(c => c.active))
                {
                    _facebookService.BlockUserFromPage(bc.ext_id, customer.app_id, bc.token);
                }
                _customerService.Block(business_id, id, blocked);
                return true;
            }
            return false;
        }

        [HttpPost("search/{business_id}")]
        public dynamic SearchCustomers(string business_id, [FromQuery]string keywords)
        {
            var data = _customerService.SearchCustomers(business_id, "", "", "", "", keywords, new Paging { Limit = 20 }).Result;
            return new { customers = data == null ? new List<CustomerModel>() : data.Select(t => new CustomerModel(t)) };
        }

        [Authorize(Policy = "AgentOrAdmin")]
        [HttpGet("list/{business_id}")]
        public async Task<CustomerFeed> GetCustomers(string business_id, [FromQuery]ThreadFilter filter)
        {
            IEnumerable<Customer> data = null;

            filter.channel_id = filter.channel_id == "all" ? "" : filter.channel_id;
            filter.agent_id = filter.agent_id == "all" ? "" : filter.agent_id;
            filter.search = filter.search ?? "";
            filter.first = filter.first == 0 ? 999999999999999 : filter.first;

            // var now = DateTime.Now.ToString("dd-MM-yyyy");
            if (!string.IsNullOrWhiteSpace(filter.from_date) && !string.IsNullOrWhiteSpace(filter.to_date))
            {
                var time = Convert.ToInt64(filter.to_date.Substring(0, 10));
                if (filter.first >= time)
                {
                    filter.first = time;
                }
            }

            switch (filter.status)
            {
                case "unread":
                    filter.status = "";
                    filter.flag = "unread";
                    break;

                case "nonreply":
                    filter.status = "";
                    filter.flag = "nonreply";
                    break;
                case "open":
                    filter.status = "";
                    filter.flag = "open";
                    break;
                default:
                    break;
            }

            if (string.IsNullOrWhiteSpace(filter.search))
            {
                if (string.IsNullOrWhiteSpace(filter.status))
                {
                    switch (filter.flag)
                    {
                        case "unread":
                            if (string.IsNullOrWhiteSpace(filter.channel_id))
                            {
                                if (string.IsNullOrWhiteSpace(filter.agent_id))
                                {
                                    data = await _customerService.GetUnreadCustomers(business_id, new Paging { Limit = filter.quantity, Next = (filter.first).ToString() });
                                }
                                else
                                {
                                    data = await _customerService.GetUnreadCustomersByAgent(business_id, filter.agent_id, new Paging { Limit = filter.quantity, Next = (filter.first).ToString() });
                                }
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(filter.agent_id))
                                {
                                    data = await _customerService.GetUnreadCustomersByChannel(business_id, filter.channel_id, new Paging { Limit = filter.quantity, Next = (filter.first).ToString() });
                                }
                                else
                                {
                                    data = await _customerService.GetUnreadCustomersByChannelAndAgent(business_id, filter.channel_id, filter.agent_id, new Paging { Limit = filter.quantity, Next = (filter.first).ToString() });
                                }
                            }
                            break;
                        case "nonreply":
                            if (string.IsNullOrWhiteSpace(filter.channel_id))
                            {
                                if (string.IsNullOrWhiteSpace(filter.agent_id))
                                {
                                    data = await _customerService.GetNonReplyCustomers(business_id, new Paging { Limit = filter.quantity, Next = (filter.first).ToString() });
                                }
                                else
                                {
                                    data = await _customerService.GetNonReplyCustomersByAgent(business_id, filter.agent_id, new Paging { Limit = filter.quantity, Next = (filter.first).ToString() });
                                }
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(filter.agent_id))
                                {
                                    data = await _customerService.GetNonReplyCustomersByChannel(business_id, filter.channel_id, new Paging { Limit = filter.quantity, Next = (filter.first).ToString() });
                                }
                                else
                                {
                                    data = await _customerService.GetNonReplyCustomersByChannelAndAgent(business_id, filter.channel_id, filter.agent_id, new Paging { Limit = filter.quantity, Next = (filter.first).ToString() });
                                }
                            }
                            break;
                        case "open":
                            if (string.IsNullOrWhiteSpace(filter.channel_id))
                            {
                                if (string.IsNullOrWhiteSpace(filter.agent_id))
                                {
                                    data = await _customerService.GetOpenCustomers(business_id, new Paging { Limit = filter.quantity, Next = (filter.first).ToString() });
                                }
                                else
                                {
                                    data = await _customerService.GetOpenCustomersByAgent(business_id, filter.agent_id, new Paging { Limit = filter.quantity, Next = (filter.first).ToString() });
                                }
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(filter.agent_id))
                                {
                                    data = await _customerService.GetOpenCustomersByChannel(business_id, filter.channel_id, new Paging { Limit = filter.quantity, Next = (filter.first).ToString() });
                                }
                                else
                                {
                                    data = await _customerService.GetOpenCustomersByChannelAndAgent(business_id, filter.channel_id, filter.agent_id, new Paging { Limit = filter.quantity, Next = (filter.first).ToString() });
                                }
                            }
                            break;
                        default:
                            if (string.IsNullOrWhiteSpace(filter.channel_id))
                            {
                                if (string.IsNullOrWhiteSpace(filter.agent_id))
                                {
                                    data = await _customerService.GetAllCustomers(business_id, new Paging { Limit = filter.quantity, Next = (filter.first).ToString() });
                                }
                                else
                                {
                                    data = await _customerService.GetAllCustomersByAgent(business_id, filter.agent_id, new Paging { Limit = filter.quantity, Next = (filter.first).ToString() });
                                }
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(filter.agent_id))
                                {
                                    data = await _customerService.GetAllCustomersByChannel(business_id, filter.channel_id, new Paging { Limit = filter.quantity, Next = (filter.first).ToString() });
                                }
                                else
                                {
                                    data = await _customerService.GetAllCustomersByChannelAndAgent(business_id, filter.channel_id, filter.agent_id, new Paging { Limit = filter.quantity, Next = (filter.first).ToString() });
                                }
                            }
                            break;
                    }
                }
            }
            if (data == null)
            {
                data = _customerService.SearchCustomers(business_id, filter.channel_id, filter.agent_id, filter.status, filter.flag, filter.search, new Paging { Limit = filter.quantity, Next = (filter.first).ToString() }).Result;
                if (data != null)
                    foreach (var v in data)
                    {
                        if (v.active_thread == null)
                        {
                            var thread = _threadService.GetByIdFromCustomerId(business_id, v.id);
                            if (thread == null)
                            {
                                var page = new Paging();
                                page.Limit = 1;
                                var mess = await _messageService.GetByCustomer(v.business_id, v.id, page);
                                if (mess != null && mess.Count > 0)
                                {
                                    thread = _threadService.GetById(mess[0].business_id, mess[0].thread_id);
                                }
                            }
                            if (thread != null)
                            {
                                v.active_thread = JsonConvert.SerializeObject(thread);
                                _customerService.CreateCustomer(v, false);
                            }

                        }
                    }
            }
            return new CustomerFeed { Data = data == null ? new List<CustomerModel>() : data.Where(t => !string.IsNullOrWhiteSpace(t.active_thread)).Select(t => new CustomerModel(t)) };
        }


    }
}