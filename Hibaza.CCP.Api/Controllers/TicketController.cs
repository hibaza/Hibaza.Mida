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
    [Route("tickets")]
    public class TicketController : Controller
    {
        public ITicketService _ticketService;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IBusinessService _businessService;
        private readonly ILoggingService _logService;
        private readonly IThreadService _threadService;
        private readonly ICustomerService _customerService;
        public TicketController(ITicketService ticketService, ICustomerService customerService, IThreadService threadService, IOptions<AppSettings> appSettings,IBusinessService businessService, ILoggingService logService)
        {
            _ticketService = ticketService;
            _appSettings = appSettings;
            _logService = logService;
            _threadService = threadService;
            _customerService = customerService;
            _businessService = businessService;
        }

        [HttpGet("copyto/{business_id}")]
        public int CopToSQL(string business_id, [FromQuery]string access_token)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;
            var fb = new FirebaseTicketRepository(new FirebaseFactory(_appSettings));

            foreach (var t in fb.GetAll(business_id, new Paging { Limit = 10 }))
            {
                count++;
                _ticketService.Create(t);
            }
            return count;
        }

        [HttpGet("autoupdatecusomterid")]
        public int AutoUpdateCustomerId([FromQuery]int minutes, [FromQuery]string access_token)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;

            RecurringJob.AddOrUpdate<TicketService>("AutoUpdateCustomerIdForAllTickets", x => x.UpdateCustomerId(), Cron.MinuteInterval(minutes));
            return count;
        }


        [HttpGet("list/{business_id}/{customer_id}")]
        public ApiResponse GetCustomerTickets(string business_id, string customer_id, string access_token)
        {
            ApiResponse response = new ApiResponse();
            IEnumerable<Ticket> resultData = null;
            try
            {
                resultData = _ticketService.GetCustomerTickets(business_id,customer_id, new Paging { Limit = 100 });
                response.ok = resultData != null;
                response.data = resultData;
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Ticket",
                    link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
                    details = JsonConvert.SerializeObject(ex.StackTrace),
                    name = string.Format("Get tickets by business_id: {0}", business_id)
                });
            }
            return response;
        }

        [HttpPost("set_short_description/{id}")]
        public ApiResponse SetShortDescription(string id, [FromForm]string business_id, [FromForm]string short_description)
        {
            ApiResponse response = new ApiResponse();
            short_description = short_description ?? "";
            short_description = short_description.Trim();
            if (string.IsNullOrWhiteSpace(short_description))
            {
                response.msg += "Short description cannot be empty " + id;
                return response;
            }
            Ticket ticket = _ticketService.GetById(business_id, id);
            ticket.short_description = short_description;
            _ticketService.Create(ticket);
            response.ok = true;
            return response;
        }

        [HttpPost("set_description/{id}")]
        public ApiResponse SetDescription(string id, [FromForm]string business_id, [FromForm]string description)
        {
            ApiResponse response = new ApiResponse();
            description = description ?? "";
            description = description.Trim();
            if (string.IsNullOrWhiteSpace(description))
            {
                response.msg += "Description cannot be empty " + id;
                return response;
            }
            Ticket ticket = _ticketService.GetById(business_id, id);
            ticket.description = description;
            _ticketService.Create(ticket);
            response.ok = true;
            return response;
        }

        [HttpPost("edit_tags")]
        public ApiResponse EditTags([FromForm]TagModel data)
        {
            ApiResponse response = new ApiResponse();
            data.tags = data.tags ?? "";
            data.tags = data.tags.Trim();
            Ticket ticket = _ticketService.GetById(data.owner_id);
            ticket.tags = data.tags;
            _ticketService.Create(ticket);
            response.ok = true;
            return response;
        }

        [HttpPost("set_status/{id}/{status}")]
        public ApiResponse SetStatus(string id, int status, [FromQuery]string business_id)
        {
            ApiResponse response = new ApiResponse();
            if (status < 0 || status > 4)
            {
                response.msg += "Invalid status " + id + " : " + status;
                return response;
            }
            Ticket ticket = _ticketService.GetById(id);
            ticket.status = status;
            _ticketService.Create(ticket);
            _customerService.UpdateCustomerLastTicket(ticket.business_id, ticket.customer_id, true);
            response.ok = true;
            return response;
        }

        [HttpPost("add")]
        public ApiResponse Create([FromBody]Ticket ticket)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                ticket.short_description = ticket.short_description ?? "";
                ticket.short_description = ticket.short_description.Trim();
                response.data = new TicketModel(ticket);
                try
                {
                    if (string.IsNullOrWhiteSpace(ticket.short_description)) { response.msg += "Short description cannot be empty " + ticket.thread_id; }

                    if (!string.IsNullOrWhiteSpace(ticket.business_id) && !string.IsNullOrWhiteSpace(ticket.short_description))
                    {
                        ticket.customer_id = string.IsNullOrWhiteSpace(ticket.customer_id) ? _threadService.GetCustomerId(ticket.business_id, ticket.thread_id) : ticket.customer_id;
                        _ticketService.Create(ticket);
                        _customerService.UpdateCustomerLastTicket(ticket.business_id, ticket.customer_id, true);
                        response.ok = true;
                    }

                }
                catch (Exception ex)
                {
                    _logService.Create(new Log
                    {
                        message = ex.Message,
                        category = "Ticket",
                        link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
                        details = JsonConvert.SerializeObject(ex.StackTrace),
                        name = string.Format("Add ticket: {0} to business_id: {1}", ticket.id, ticket.business_id)
                    });
                }
            }
            catch { }
            return response;
        }

        [HttpGet("last/{business_id}/{customer_id}")]
        public async Task<Ticket> GetLastForCustomer(string business_id, string customer_id, [FromQuery]string access_token)
        {
            var ticket = await _ticketService.GetCustomerLastActiveTicket(business_id, customer_id);
            if (ticket == null) return null;
            return ticket;
        }


        [HttpGet("get/{business_id}/{id}")]
        public dynamic GetById(string business_id, string id)
        {
            var ticket =_ticketService.GetById(business_id, id);
            if (ticket == null) return null;
            return new TicketModel(ticket);
        }
      

        [HttpPost("delete/{business_id}/{id}")]
        public bool Delete(string business_id, string id)
        {
            var resultData = _ticketService.Delete(business_id, id);
            return resultData;
        }


        [HttpGet("fixTicket/")]
        public void fixTicket(string business_id, string id)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                var page = new Paging();
                page.Limit = 1000;
                business_id = "150146656";
                var tickets = _ticketService.GetTickets(business_id, page).Result;
                if (tickets == null) return;
                foreach (var t in tickets)
                {
                    if (!string.IsNullOrWhiteSpace(t.customer_id))
                    {
                        var cus = _customerService.GetById(business_id, t.customer_id);
                        if (cus != null)
                        {
                            cus.active_ticket = JsonConvert.SerializeObject(_ticketService.GetCustomerLastActiveTicket(business_id, cus.id).Result);
                            _customerService.CreateCustomer(cus, false);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(t.thread_id))
                        {
                            var p = new Paging();
                            p.Limit = 1;
                            var cus = _customerService.GetCustomersActiveThreadLikeThread(business_id, t.thread_id, p).Result;
                            if (cus != null && cus.Count > 0)
                            {
                                var c = cus[0];
                                cus[0].active_ticket = JsonConvert.SerializeObject(_ticketService.GetCustomerLastActiveTicket(business_id, c.id).Result);
                                _customerService.CreateCustomer(c, false);
                            }
                        }
                    }
                }
            });
        }

    }
}
