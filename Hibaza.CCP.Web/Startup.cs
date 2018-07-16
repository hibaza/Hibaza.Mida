using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Hibaza.CCP.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Hibaza.CCP.Core.TokenProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hibaza.CCP.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile($"hosting.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();

        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMemoryCache();
            services.AddApplicationInsightsTelemetry(Configuration);
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            //services.Configure<MvcOptions>(options =>
            //{
            //    options.Filters.Add(new RequireHttpsAttribute());
            //});

            //string hangfireConnectionString = Configuration.GetSection("AppSettings")["ConnectionStringHangfireDB"];
            //services.AddHangfire(x => x.UseSqlServerStorage(hangfireConnectionString));

            services.AddMvc();
            //services.AddMvc(config =>
            //{
            //    var policy = new AuthorizationPolicyBuilder()
            //                     .RequireAuthenticatedUser()
            //                     .Build();
            //    config.Filters.Add(new AuthorizeFilter(policy));
            //});

            //// Use policy auth.
            //services.AddAuthorization(options =>
            //{

            //    options.AddPolicy("AgentOrAdmin",
            //                      policy => policy.RequireClaim(ClaimTypes.Role, new string[] { "agent", "admin" }));

            //    options.AddPolicy("AdminOnly",
            //                      policy => policy.RequireClaim(ClaimTypes.Role, new string[] { "admin" }));

            //});

            //// Get options from app settings
            //var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            //// Configure JwtIssuerOptions
            //services.Configure<JwtIssuerOptions>(options =>
            //{
            //    options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
            //    options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
            //    options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            //});

            services.AddScoped<Controllers.IViewRenderService, Controllers.ViewRenderService>();

        }
        private const string SecretKey = "Hibaza_top_secret_key@2017";
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();



            //app.UseApplicationInsightsRequestTelemetry();

            //var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            //var tokenValidationParameters = new TokenValidationParameters
            //{
            //    ValidateIssuer = true,
            //    ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

            //    ValidateAudience = true,
            //    ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

            //    ValidateIssuerSigningKey = true,
            //    IssuerSigningKey = _signingKey,

            //    RequireExpirationTime = true,
            //    ValidateLifetime = true,

            //    ClockSkew = TimeSpan.Zero
            //};

            //app.UseJwtBearerAuthentication(new JwtBearerOptions
            //{
            //    AutomaticAuthenticate = true,
            //    AutomaticChallenge = true,
            //    TokenValidationParameters = tokenValidationParameters
            //});

            //app.UseCookieAuthentication(new CookieAuthenticationOptions
            //{
            //    LoginPath = new Microsoft.AspNetCore.Http.PathString("/login"),
            //    AutomaticAuthenticate = true,
            //    AutomaticChallenge = true,
            //    AuthenticationScheme = "Cookies",
            //    CookieName = "access_token",
            //    TicketDataFormat = new CustomJwtDataFormat(
            //        SecurityAlgorithms.HmacSha256,
            //        tokenValidationParameters)
            //});

            //app.UseCookieAuthentication(new CookieAuthenticationOptions
            //{
            //    LoginPath = new Microsoft.AspNetCore.Http.PathString("/login"),
            //    AutomaticAuthenticate = true,
            //    AutomaticChallenge = true
            //});


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            //app.UseApplicationInsightsExceptionTelemetry();
            app.UseStaticFiles(); // wwwroot inside file access. 
            app.UseDefaultFiles(); // Default startup file of app.

            //app.UseHangfireDashboard("/hangfire", new DashboardOptions
            //{
            //    Authorization = new[] { new HangfireAuthorizeFilter() }
            //});
            //app.UseHangfireServer();

            //app.Use(async (context, next) =>
            //{
            //    var request = context.Request;
            //    if (!request.IsHttps || request.Host.ToString().StartsWith("localhost", StringComparison.CurrentCultureIgnoreCase) || request.Host.ToString().StartsWith("dev", StringComparison.CurrentCultureIgnoreCase))
            //    {
            //        await next();
            //    }
            //    else
            //    {
            //        var host = new HostString(request.Host.Host);
            //        string newUrl = $"https://{host}{request.PathBase}{request.Path}{request.QueryString}";
            //        context.Response.Redirect(newUrl, true);
            //    }
            //});


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });


            //var count = Core.Helpers.WebHelper.HttpGetAsync<int>(Configuration.GetSection("AppSettings")["ApiBaseUrl"] + "brands/agents/autologout?minutes=30&access_token=@bazavietnam").Result;
        }
    }

    //public class HangfireAuthorizeFilter : IDashboardAuthorizationFilter
    //{
    //    public bool Authorize(DashboardContext context)
    //    {
    //        return true;
    //    }
    //}
}
