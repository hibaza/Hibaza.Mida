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
using Hibaza.CCP.Core;
using Microsoft.Extensions.Options;
using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace Hibaza.CCP.Web.Controllers
{

    public class AccountController : Controller
    {
        private readonly IViewRenderService _viewRenderService;
        private readonly AppSettings _appSettings;
        public AccountController(IViewRenderService viewRenderService, IOptions<AppSettings> appSettings)
        {
            _viewRenderService = viewRenderService;
            _appSettings = appSettings.Value;
        }

        [Route("/accounts/settings/{user_id}")]
        public ApiResponse Settings(string user_id)
        {
            ApiResponse response = new ApiResponse();
            var access_token = Request.Cookies["access_token"] ?? "";
            AgentModel model = null;
            if (!string.IsNullOrWhiteSpace(user_id) && !string.IsNullOrWhiteSpace(access_token))
            {
                var u = _appSettings.BaseUrls.Api + "brands/agents/single/" + user_id + "/?access_token=" + access_token;
                model = Core.Helpers.WebHelper.HttpGetAsync<AgentModel>(u).Result;
            }
            if (model == null) return response;


            var result = _viewRenderService.RenderToStringAsync("Account/Settings", model).Result;
            response.view = result;
            response.ok = true;
            return response;
        }

        [Route("/accounts/hotline/{user_id}")]
        public ApiResponse Hotline(string user_id)
        {
            ApiResponse response = new ApiResponse();
            var access_token = Request.Cookies["access_token"] ?? "";
            AgentModel model = null;
            if (!string.IsNullOrWhiteSpace(user_id) && !string.IsNullOrWhiteSpace(access_token))
            {
                var u = _appSettings.BaseUrls.Api + "brands/agents/single/" + user_id + "/?access_token=" + access_token;
                model = Core.Helpers.WebHelper.HttpGetAsync<AgentModel>(u).Result;
            }
            if (model == null) return response;


            var result = _viewRenderService.RenderToStringAsync("Account/Settings", model).Result;
            response.view = result;
            response.ok = true;
            return response;
        }


        [Route("/login")]
        public IActionResult Login([FromForm]LoginViewModel user, string uid, string token)
        {
            if (!Request.Host.ToString().StartsWith(new Uri(_appSettings.BaseUrls.Web).Host))
            {
                return Redirect(_appSettings.BaseUrls.Web + "login");
            }

            var user_id = Request.Cookies["user_id"] ?? "";
            var access_token = Request.Cookies["access_token"] ?? "";

            //if (HttpContext.User.Identity.IsAuthenticated) return Redirect("/");
            ViewBag.Version = _appSettings.Version;
            ViewBag.ApiUrl = _appSettings.BaseUrls.Api;
            ViewBag.BaseUrls_Web = _appSettings.BaseUrls.Web;
            ViewBag.AppId = _appSettings.ClientId;
            if (Request.Method.Equals("POST"))
            {
                var url = _appSettings.BaseUrls.Api + "brands/agents/auth/?username=" + user.username + "&password=" + user.password;
                var r = Core.Helpers.WebHelper.HttpGetAsync<AccessToken>(url).Result;
                if (r != null && !string.IsNullOrWhiteSpace(r.access_token))
                {
                    // tam bo
                   // Framework.SetCurrentUserLoginStatus(r.user_id, r.access_token, "online", _appSettings.BaseUrls.Api);

                    Response.Cookies.Append("access_token", r.access_token, new CookieOptions { HttpOnly = true, Path = "/" });
                    Response.Cookies.Append("user_id", r.user_id, new CookieOptions { HttpOnly = true, Path = "/" });
                    return View("LoginRedirect", r);
                }
            }
            if (!string.IsNullOrWhiteSpace(user_id) && !string.IsNullOrWhiteSpace(access_token))
            {
                ViewBag.UserId = user_id;
                ViewBag.AccessToken = access_token;
                ViewBag.BaseUrls_PhoneWeb = _appSettings.BaseUrls.PhoneWeb;
                ViewBag.BaseUrls_ApiHotline = _appSettings.BaseUrls.ApiHotline;
            }
            return View();
        }

        [Route("/logout")]
        public IActionResult Logout()
        {
            var user_id = Request.Cookies["user_id"] ?? "";
            var access_token = Request.Cookies["access_token"] ?? "";
            AgentModel model = Framework.CurrentUser(user_id, access_token, _appSettings.BaseUrls.Api);
            if (model != null)
            {
                Framework.SetCurrentUserLoginStatus(user_id, access_token, "offline", _appSettings.BaseUrls.Api);
            }
            Response.Cookies.Append("access_token", "", new CookieOptions { HttpOnly = true, Path = "/" });
            Response.Cookies.Append("user_id", "", new CookieOptions { HttpOnly = true, Path = "/" });
            ViewBag.ApiUrl = _appSettings.BaseUrls.Api;
            if (!string.IsNullOrWhiteSpace(user_id) && !string.IsNullOrWhiteSpace(access_token))
            {
                ViewBag.UserId = user_id;
                ViewBag.AccessToken = access_token;
                ViewBag.BaseUrls_PhoneWeb = _appSettings.BaseUrls.PhoneWeb;
                ViewBag.BaseUrls_ApiHotline = _appSettings.BaseUrls.ApiHotline;
            }
            return Redirect("/login");
        }

        [Route("/register")]
        public IActionResult Register()
        {
            ViewBag.Version = _appSettings.Version;
            ViewBag.ApiUrl = _appSettings.BaseUrls.Api;
            return View();
        }
        [Route("/registerfb")]
        public IActionResult registerfb()
        {

            ViewBag.Version = _appSettings.Version;
            ViewBag.ApiUrl = _appSettings.BaseUrls.Api;
            return View();
        }


        [Route("/confirm/{key}")]
        public IActionResult Confirm(string key, [FromQuery]string activation_code)
        {
            //if (activation_code != "iloveyou")
            //{
            //    return Redirect("/complete/" + key);
            //}

            ViewBag.ApiUrl = _appSettings.BaseUrls.Api;
            string user_id = key;
            //string access_token = "usertoken";
            //AgentModel model = Framework.CurrentUser(user_id, access_token, _appSettings.BaseUrls.Api);
            //if (model != null)
            //{
            //    Framework.SetCurrentUserLoginStatus(user_id, access_token, "online", _appSettings.BaseUrls.Api);

            //    Response.Cookies.Append("access_token", access_token, new CookieOptions { HttpOnly = true, Path = "/" });
            //    Response.Cookies.Append("user_id", user_id, new CookieOptions { HttpOnly = true, Path = "/" });
            //}
            ViewBag.agent_id = user_id;
            ViewBag.Version = _appSettings.Version;
            return View("Setup");
            //return Redirect("/setup/" + key);
        }
        [Route("/setup/{id}")]
        public IActionResult Setup(string id)
        {
            ViewBag.agent_id = id;
            ViewBag.Version = _appSettings.Version;
            ViewBag.ApiUrl = _appSettings.BaseUrls.Api;
            return View();
        }

        [Route("/complete/{id}")]
        public IActionResult Complete(string id)
        {
            ViewBag.agent_id = id;
            ViewBag.Version = _appSettings.Version;
            ViewBag.ApiUrl = _appSettings.BaseUrls.Api;
            return View();
        }
    }
}
