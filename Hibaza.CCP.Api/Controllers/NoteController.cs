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

namespace Hibaza.CCP.Api.Controllers
{
    [Route("notes")]
    public class NoteController : Controller
    {
        public INoteService _noteService;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IBusinessService _businessService;
        private readonly ILoggingService _logService;
        private readonly IThreadService _threadService;
        public NoteController(INoteService noteService, IThreadService threadService, IOptions<AppSettings> appSettings,IBusinessService businessService, ILoggingService logService)
        {
            _noteService = noteService;
            _appSettings = appSettings;
            _logService = logService;
            _threadService = threadService;
            _businessService = businessService;
        }

        [HttpGet("copyto/{business_id}")]
        public int CopToSQL(string business_id, [FromQuery]string access_token)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;
            var fb = new FirebaseNoteRepository(new FirebaseFactory(_appSettings));

            foreach (var t in fb.GetAll(business_id, new Paging { Limit = 10 }))
            {
                count++;
                _noteService.Create(t);
            }
            return count;
        }


        [HttpGet("list/{type}/{business_id}/{customer_id}")]
        public ApiResponse GetCustomerNotes(string type, string business_id, string customer_id, string access_token)
        {
            ApiResponse response = new ApiResponse();
            IEnumerable<Note> resultData = null;
            try
            {
                switch (type) {
                    case "customer":
                        resultData = _noteService.GetCustomerNotes(business_id, customer_id, new Paging { Limit = 100 });
                        break;
                }
                response.ok = resultData != null;
                response.data = resultData;
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    message = ex.Message,
                    category = "Note",
                    link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
                    details = JsonConvert.SerializeObject(ex.StackTrace),
                    name = string.Format("Get notes by business_id: {0}", business_id)
                });
            }
            return response;
        }
        
        [HttpPost("send")]
        public ApiResponse Create([FromBody]Note note)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                if (note.customer_id == null)
                {
                    var threads = _threadService.GetById(note.business_id, note.thread_id);
                    if (threads != null)
                    {
                        note.customer_id = threads.customer_id;
                    }
                }

                note.text = note.text ?? "";
                note.text = note.text.Trim();
                response.data = new NoteModel(note);
                try
                {
                    if (string.IsNullOrWhiteSpace(note.text)) { response.msg += "Note cannot be empty " + note.business_id; }

                    if (!string.IsNullOrWhiteSpace(note.business_id) && !string.IsNullOrWhiteSpace(note.text))
                    {
                        note.customer_id = string.IsNullOrWhiteSpace(note.customer_id) ? _threadService.GetCustomerId(note.business_id, note.thread_id) : note.customer_id;
                        _noteService.Create(note);
                        response.ok = true;
                    }

                }
                catch (Exception ex)
                {
                    _logService.Create(new Log
                    {
                        message = ex.Message,
                        category = "Note",
                        link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
                        details = JsonConvert.SerializeObject(ex.StackTrace),
                        name = string.Format("Add note: {0} to business_id: {1}", note.id, note.business_id)
                    });
                }
            }catch{ }
            return response;
        }

        [HttpGet("autoupdatecusomterid")]
        public int AutoUpdateCustomerId([FromQuery]int minutes, [FromQuery]string access_token)
        {
            int count = 0;

            if (access_token != "@bazavietnam") return count;

            RecurringJob.AddOrUpdate<NoteService>("AutoUpdateCustomerIdForAllNotes", x => x.UpdateCustomerId(), Cron.MinuteInterval(minutes));
            return count;
        }

        [HttpGet("get/{type}/{business_id}/{id}")]
        public dynamic GetById(string type, string business_id, string id)
        {
            var note =_noteService.GetById(business_id, id);
            if (note == null) return null;
            switch (type)
            {
                case "customer":
                    return new NoteModel(note);
            }
            return null;
        }
      

        [HttpPost("delete/{type}/{business_id}/{id}")]
        public bool Delete(string business_id, string id)
        {
            var resultData = _noteService.Delete(business_id, id);
            return resultData;
        }
        
    }
}
