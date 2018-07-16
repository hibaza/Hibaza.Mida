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
using System.Net.Http;
using Microsoft.Net.Http.Headers;
using System.IO;
using Firebase.Storage;

namespace Hibaza.CCP.Api.Controllers
{
    [Route("files")]
    public class FileController : Controller
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ILoggingService _logService;
        private static HttpClient Client { get; } = new HttpClient();

        public FileController(IOptions<AppSettings> appSettings, ILoggingService logService)
        {
            _appSettings = appSettings;
            _logService = logService;
        }

    }

}
