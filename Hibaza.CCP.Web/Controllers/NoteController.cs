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
using Hibaza.CCP.Domain.Models;
using Hibaza.CCP.Core;
using Microsoft.Extensions.Options;
using Hibaza.CCP.Domain.Entities;
using System.Dynamic;
using Newtonsoft.Json;

namespace Hibaza.CCP.Web.Controllers
{
    [Route("notes")]
    public class NoteController : Controller
    {
        private readonly IViewRenderService _viewRenderService;
        private readonly AppSettings _appSettings;

        public NoteController(IViewRenderService viewRenderService, IOptions<AppSettings> appSettings)
        {
            _viewRenderService = viewRenderService;
            _appSettings = appSettings.Value;
        }


        [Route("send_{type}_note/{business_id}/{thread_id}")]
        public ApiResponse SendNote(string type, string business_id, string thread_id, [FromQuery]string note_id, [FromForm]string text, [FromForm]string featured)
        {
            ApiResponse response = new ApiResponse();
            var access_token = Request.Cookies["access_token"];
            var user_id = Request.Cookies["user_id"] ?? "";
            AgentModel agent = null;
            if (!string.IsNullOrWhiteSpace(user_id) && !string.IsNullOrWhiteSpace(access_token))
            {
                var u = _appSettings.BaseUrls.Api + "brands/agents/single/" + user_id + "/?access_token=" + access_token;
                agent = Core.Helpers.WebHelper.HttpGetAsync<AgentModel>(u).Result;
            }
            if (agent == null) return response;


            if (!string.IsNullOrWhiteSpace(note_id))
            {
                var url = _appSettings.BaseUrls.Api + "notes/get/" + type + "/" + business_id + "/" + note_id + "/?access_token=" + access_token;
                response = Core.Helpers.WebHelper.HttpGetAsync<ApiResponse>(url).Result;

            }
            else
            {
                var url = _appSettings.BaseUrls.Api + "notes/send/" + "?access_token=" + access_token;
                response = Core.Helpers.WebHelper.HttpPostAsync<ApiResponse>(url, new Note { type = type, business_id = business_id, thread_id = thread_id, text = text, featured = featured == "on", sender_id = agent.id, sender_name = agent.name, sender_avatar = agent.avatar }).Result;

            }

            if (response == null || response.data == null) return response;
            NoteModel model = new NoteModel
            {
                id = response.data.id,
                created_time = response.data.created_time,
                sender_id = response.data.sender_id,
                sender_name = response.data.sender_name,
                sender_avatar = response.data.sender_avatar,
                text = response.data.text,
                featured = response.data.featured,
                thread_id =response.data.thread_id,
                customer_id = response.data.customer_id,
                type = response.data.type
            };
            var result = _viewRenderService.RenderToStringAsync("Note/Send", model).Result;
            response.view = result;
            response.ok = true;
            return response;
        }

    }
}
