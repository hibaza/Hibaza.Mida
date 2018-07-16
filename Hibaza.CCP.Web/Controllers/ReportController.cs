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


namespace Hibaza.CCP.Web.Models
{
    public class ReportModel : BaseModel
    {
       public long DatePickerUuid { get;} = Hibaza.CCP.Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.Today);
    }
}

namespace Hibaza.CCP.Web.Controllers
{

    [Route("reports")]
    public class ReportController : Controller
    {
        private readonly IViewRenderService _viewRenderService;
        private readonly AppSettings _appSettings;

        public ReportController(IViewRenderService viewRenderService, IOptions<AppSettings> appSettings)
        {
            _viewRenderService = viewRenderService;
            _appSettings = appSettings.Value;
        }

        [Route("{business_id}")]
        public ApiResponse Profile(string business_id)
        {
            ApiResponse response = new ApiResponse();
            var access_token = Request.Cookies["access_token"];

            //var url = _appSettings.BaseUrls.Api + "brands/agents/single/" + id + "/?access_token=" + access_token;
            //var model = Core.Helpers.WebHelper.HttpGetAsync<ReportProfileModel>(url).Result;
            //if (model == null) return response;
            var model = new ReportModel { };
            var result = _viewRenderService.RenderToStringAsync("Report/Index", model).Result;
            response.data = result;
            response.ok = true;
            return response;
        }

    }
}
