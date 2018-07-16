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
using Firebase.Auth;
using Newtonsoft.Json;

namespace Hibaza.CCP.Web.Controllers
{

    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string viewName, object model);
    }

    public class ViewRenderService : IViewRenderService
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public ViewRenderService(IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> RenderToStringAsync(string viewName, object model)
        {
            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            using (var sw = new StringWriter())
            {
                var viewResult = _razorViewEngine.FindView(actionContext, viewName, false);

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewName} does not match any available view");
                }

                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }
    }

    public class HomeController : Controller
    {
        private readonly IViewRenderService _viewRenderService;
        private readonly AppSettings _appSettings;
        public HomeController(IViewRenderService viewRenderService, IOptions<AppSettings> appSettings)
        {
            _viewRenderService = viewRenderService;
            _appSettings = appSettings.Value;
        }

        [Route("/")]
        public IActionResult Index()
        {
            var user_id = Request.Cookies["user_id"] ?? "";
            var access_token = Request.Cookies["access_token"] ?? "";
            AgentModel model = Framework.CurrentUser(user_id, access_token, _appSettings.BaseUrls.Api);
            if (model == null) return Redirect("/login");

            if (model.locked) return Redirect("/complete/" + user_id);

            Response.Cookies.Append("business_id", model.business_id, new CookieOptions { HttpOnly = true, Path = "/" });

            ViewBag.Version = _appSettings.Version;
            ViewBag.AccessToken = access_token;
            ViewBag.BusinessId = model.business_id;
            ViewBag.BusinessName = model.business_name;
            ViewBag.UserId = model.id;
            ViewBag.UserRole = model.role;
            ViewBag.UserName = model.name;
            ViewBag.UserActive = model.status == "online";
            ViewBag.UserStatus = model.status; //online, offline, busy
            ViewBag.ApiUrl = _appSettings.BaseUrls.Api;
            ViewBag.AppUrl = _appSettings.BaseUrls.Web;
            dynamic firebase = new ExpandoObject();
            firebase.ApiKey = _appSettings.FirebaseDB.APIKey;
            firebase.AuthDomain = _appSettings.FirebaseDB.AuthDomain;
            firebase.DatabaseURL = _appSettings.FirebaseDB.DatabaseURL;
            firebase.StorageBucket = _appSettings.FirebaseDB.StorageBucket;

            //var authProvider = new FirebaseAuthProvider(new FirebaseConfig(_appSettings.FirebaseDB.APIKey));
            //var auth = authProvider.SignInWithEmailAndPasswordAsync("truongvuhung@yahoo.com", "Vaza@d4a2");
            firebase.Token = "";
            firebase.MessagingSenderId = _appSettings.FirebaseDB.MessagingSenderId;
            ViewBag.FirebaseConfig = firebase;
            ViewBag.AccessToken = access_token;
            ViewBag.BusinessName = model.business_name;
            var url = _appSettings.BaseUrls.Api + "brands/agents/list/" + model.business_id + "/?access_token=" + access_token;
            var rs = Core.Helpers.WebHelper.HttpGetAsync<AgentFeed>(url).Result;
            ViewBag.Agents = (rs != null && rs.Data != null) ? rs.Data : new List<AgentModel>();

            url = _appSettings.BaseUrls.Api + "brands/channels/list/" + model.business_id + "/?access_token=" + access_token;
            var rs1 = Core.Helpers.WebHelper.HttpGetAsync<ChannelFeed>(url).Result;
            ViewBag.Channels = (rs1 != null && rs1.Data != null) ? rs1.Data : new List<Channel>();

            ViewBag.BaseUrls_Api = _appSettings.BaseUrls.Api;
            ViewBag.BaseUrls_ApiAi = _appSettings.BaseUrls.ApiAi;
            ViewBag.BaseUrls_ApiOrder = _appSettings.BaseUrls.ApiOrder;
            ViewBag.BaseUrls_ApiHotline = _appSettings.BaseUrls.ApiHotline;
            ViewBag.BaseUrls_PhoneWeb = _appSettings.BaseUrls.PhoneWeb;
            ViewBag.BaseUrls_Web = _appSettings.BaseUrls.Web;

            return View();
        }

        

        [Route("/error")]
        public IActionResult Error()
        {
            return Content("Error!");
        }

    }
}
