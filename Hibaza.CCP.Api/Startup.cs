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
using Hibaza.CCP.Service;
using Hibaza.CCP.Data.Providers.SQLServer;
using Hibaza.CCP.Data.Repositories;
using Hibaza.CCP.Data.Infrastructure;
using Hibaza.CCP.Service.Facebook;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Hibaza.CCP.Data.Repositories.Firebase;
using Hibaza.CCP.Data.Providers.Firebase;
using Hibaza.CCP.Core.TokenProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Principal;
using Hibaza.CCP.Api.Controllers;
using Swashbuckle.AspNetCore.Swagger;
using Hangfire;
using Microsoft.AspNetCore.Mvc.Filters;
using Hangfire.Dashboard;
using Hibaza.CCP.Service.SQL;
using Hibaza.CCP.Service.Firebase;
using Hibaza.CCP.Service.Shop;
using Hangfire.SqlServer;
using Hibaza.CCP.Data.Repositories.Mongo;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.Caching.Memory;
using Hibaza.CCP.Service.Hotline;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Hangfire.Mongo;

namespace Hibaza.CCP.Api
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

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddApplicationInsightsTelemetry(Configuration);

                services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

                services.AddCors();


                //string hangfireConnectionString = Configuration.GetSection("AppSettings")["ConnectionStringHangfireDB"];
                //services.AddHangfire(x => x.UseSqlServerStorage(hangfireConnectionString));
                services.AddHangfire(config =>
                {
                    // Read DefaultConnection string from appsettings.json
                    //var connectionString = Configuration.GetConnectionString("DefaultConnection");

                    var storageOptions = new MongoStorageOptions
                    {
                        MigrationOptions = new MongoMigrationOptions
                        {
                            Strategy = MongoMigrationStrategy.Migrate,
                            BackupStrategy = MongoBackupStrategy.Collections
                        }
                    };
                    config.UseMongoStorage(Configuration["AppSettings:MongoDB:ConnectionStringHangfire"],
                        Configuration["AppSettings:MongoDB:HangfireDB"], storageOptions);
                });


                services.AddMemoryCache();
                services.AddMvc();

                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v2", new Info { Title = "Core Api", Description = "Swagger Core API" });
                });

                services.AddAuthorization(options =>
                {

                    options.AddPolicy("AgentOrAdmin",
                                      policy => policy.RequireClaim(ClaimTypes.Role, new string[] { "agent", "admin" }));

                    options.AddPolicy("AdminOnly",
                                      policy => policy.RequireClaim(ClaimTypes.Role, new string[] { "admin" }));

                });


                var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

                services.Configure<JwtIssuerOptions>(options =>
                {
                    options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                    options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                    options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
                });


                var tokenValidationParameters = new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _signingKey,
                    ValidateIssuer = true,
                    ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],
                    ValidateAudience = true,
                    ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(options =>
                {
                    options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                    options.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                    options.TokenValidationParameters = tokenValidationParameters;
                    options.SaveToken = true;
                });

                services.AddDirectoryBrowser();
                services.AddSwaggerGen();


                //services.Configure<MvcOptions>(options =>
                //{
                //    options.Filters.Add(new CorsAuthorizationFilterFactory("AllowSpecificOrigin"));
                //});

                services.AddSingleton<IConnectionFactory, ConnectionFactory>();
                //services.AddSingleton<IMessageRepository, MessageRepository>();

                services.AddSingleton<IMessageRepository, MongoMessageRepository>();

                //services.AddSingleton<IAttachmentRepository, AttachmentRepository>();
                services.AddSingleton<IAttachmentRepository, MongoAttachmentRepository>();

                //services.AddSingleton<IThreadRepository, ThreadRepository>();
                services.AddSingleton<IThreadRepository, MongoThreadRepository>();

                //services.AddSingleton<ICustomerRepository, CustomerRepository>();
                services.AddSingleton<ICustomerRepository, MongoCustomerRepository>();

                services.AddSingleton<IFirebaseFactory, FirebaseFactory>();
                services.AddSingleton<IFirebaseStorageFactory, FirebaseStorageFactory>();
                //services.AddSingleton<IChannelRepository, ChannelRepository>();
                services.AddSingleton<IChannelRepository, MongoChannelRepository>();
                //services.AddSingleton<ILinkRepository, LinkRepository>();
                services.AddSingleton<ILinkRepository, MongoLinkRepository>();
                //services.AddSingleton<INodeRepository, NodeRepository>();
                services.AddSingleton<INodeRepository, MongoNodeRepository>();
                services.AddSingleton<ITaskRepository, TaskRepository>();
                //services.AddSingleton<IAgentRepository, AgentRepository>();
                services.AddSingleton<IAgentRepository, MongoAgentRepository>();
                //services.AddSingleton<IReportRepository, ReportRepository>();
                services.AddSingleton<IReportRepository, MongoReportRepository>();

                services.AddSingleton<IUnitOfWork, UnitOfWork>();
                services.AddSingleton<ILinkService, LinkService>();
                services.AddSingleton<INodeService, NodeService>();
                services.AddSingleton<ITaskService, TaskService>();
                services.AddSingleton<IFacebookConversationService, FacebookConversationService>();
                services.AddSingleton<IZaloConversationService, ZaloConversationService>();
                services.AddSingleton<IFacebookCommentService, FacebookCommentService>();
                services.AddSingleton<IFacebookService, FacebookService>();
                services.AddSingleton<IZaloService, ZaloService>();
                services.AddSingleton<IBusinessService, BusinessService>();
                services.AddSingleton<IChannelService, ChannelService>();
                services.AddSingleton<IShortcutService, ShortcutService>();

                //services.AddSingleton<IMessageService, MessageService>();
                services.AddSingleton<IMessageService, MessageService>();

                services.AddSingleton<IProductService, ProductService>();

                //services.AddSingleton<ICustomerService, CustomerService>();
                services.AddSingleton<ICustomerService, CustomerService>();

                services.AddSingleton<IAgentService, AgentService>();

                //services.AddSingleton<IConversationService, ConversationService>();
                services.AddSingleton<IConversationService, ConversationService>();
                services.AddSingleton<ICounterService, CounterService>();
                services.AddSingleton<ICustomerCounterService, CustomerCounterService>();

                //services.AddSingleton<IReportService, ReportService>();
                services.AddSingleton<IReportService, ReportService>();

                services.AddSingleton<IReferralService, ReferralService>();
                services.AddSingleton<INoteService, NoteService>();
                services.AddSingleton<IHotlineService, HotlineService>();
                services.AddSingleton<ITicketService, TicketService>();

                //services.AddSingleton<ILoggingService, LoggingService>();
                services.AddSingleton<ILoggingService, LoggingService>();

                //services.AddSingleton<IThreadService, ThreadService>();
                services.AddSingleton<IThreadService, ThreadService>();

                //services.AddSingleton<ICustomerService, CustomerService>();
                services.AddSingleton<ICustomerService, CustomerService>();

                //services.AddSingleton<IBusinessRepository, BusinessRepository>();
                services.AddSingleton<IBusinessRepository, MongoBusinessRepository>();
                // services.AddSingleton<IShortcutRepository, ShortcutRepository>();
                services.AddSingleton<IShortcutRepository, MongoShortcutRepository>();
                //services.AddSingleton<INoteRepository, NoteRepository>();
                services.AddSingleton<INoteRepository, MongoNoteRepository>();
                services.AddSingleton<IHotlineRepository, MongoHotlineRepository>();
                //services.AddSingleton<ITicketRepository, TicketRepository>();
                services.AddSingleton<ITicketRepository, MongoTicketRepository>();
                //services.AddSingleton<IReferralRepository, IReferralRepository>();
                services.AddSingleton<IReferralRepository, MongoReferralRepository>();
                services.AddSingleton<IFirebaseTicketRepository, FirebaseTicketRepository>();

                //services.AddSingleton<IConversationRepository, ConversationRepository>();
                services.AddSingleton<IConversationRepository, MongoConversationRepository>();

                services.AddSingleton<IFirebaseAgentRepository, FirebaseAgentRepository>();
                services.AddSingleton<IFirebaseThreadRepository, FirebaseThreadRepository>();
                services.AddSingleton<IFirebaseCustomerRepository, FirebaseCustomerRepository>();
                services.AddSingleton<IFirebaseMessageRepository, FirebaseMessageRepository>();

                //services.AddSingleton<ILoggingRepository, LoggingRepository>();
                services.AddSingleton<ILoggingRepository, MongoLoggingRepository>();

                services.AddSingleton<ICounterRepository, FirebaseCounterRepository>();
                services.AddSingleton<ICustomerCounterRepository, FirebaseCustomerCounterRepository>();
                services.AddSingleton<ZaloService, ZaloService>();
                services.AddTransient<FacebookConversationService, FacebookConversationService>();
                services.AddTransient<ZaloConversationService,ZaloConversationService>();
                services.AddTransient<FacebookCommentService, FacebookCommentService>();

                //services.AddTransient<MessageService, MessageService>();
                services.AddTransient<MessageService, MessageService>();

                services.AddTransient<ReferralService, ReferralService>();
                services.AddTransient<AgentService, AgentService>();

                //services.AddTransient<ThreadService, ThreadService>();
                services.AddTransient<ThreadService, ThreadService>();

                //services.AddTransient<CustomerService, CustomerService>();
                services.AddTransient<CustomerService, CustomerService>();

                services.AddTransient<TicketService, TicketService>();
                services.AddTransient<ProductService, ProductService>();
                services.AddTransient<NoteService, NoteService>();
                services.AddTransient<HotlineService, HotlineService>();
                services.AddTransient<CounterService, CounterService>();
                services.AddTransient<CustomerCounterService, CustomerCounterService>();
                services.AddTransient<TaskService, TaskService>();
                services.AddTransient<ReportService, ReportService>();
            }
            catch (Exception ex) { }
        }

        private const string SecretKey = "Hibaza_top_secret_key@2017";
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IMemoryCache cache)
        {
            try
            {
                app.UseSecurityHeadersMiddleware();
                // app.UseSecurityHeadersMiddleware((HostingEnvironment)env);
                //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                //loggerFactory.AddDebug();


                var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                    ValidateAudience = true,
                    ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _signingKey,

                    RequireExpirationTime = true,
                    ValidateLifetime = true,

                    ClockSkew = TimeSpan.Zero
                };

                app.UseAuthentication();
                //app.UseJwtBearerAuthentication(new JwtBearerOptions
                //{
                //    AutomaticAuthenticate = true,
                //    AutomaticChallenge = true,
                //    TokenValidationParameters = tokenValidationParameters
                //});

                app.UseCors(builder =>
                          builder.AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod());

                //app.UseCors(builder =>
                //            builder.WithOrigins("http://localhost", "https://localhost", "http://hibaza.com", "https://hibaza.com", "http://api.hibaza.com", "https://api.hibaza.com", "http://hibazawebapp.azurewebsites.net"
                //            , "http://app.hibaza.com", "http://dev.hibaza.com", "https://app.hibaza.com", "https://hibazawebapp.azurewebsites.net", "http://hibaza.azurewebsites.net", "https://hibaza.azurewebsites.net", "http://localhost:10089", "http://beta_api.hibaza.com", "https://beta_api.hibaza.com", "http://beta.api.hibaza.com", "https://beta.api.hibaza.com", "http://beta.hibaza.com", "https://beta.hibaza.com")
                //                .AllowAnyHeader()
                //                .AllowAnyMethod());


                //app.UseStaticFiles(new StaticFileOptions()
                //{
                //    ServeUnknownFileTypes = true,
                //    DefaultContentType = "image/png"
                //});
                //app.UseStaticFiles(); // wwwroot inside file access. 
                //app.UseDefaultFiles(); // Default startup file of app.

                //app.UseStaticFiles(new StaticFileOptions()
                //{
                //    ServeUnknownFileTypes = true,
                //    FileProvider = new PhysicalFileProvider(
                //       Path.Combine(Directory.GetCurrentDirectory(), @"Documents", "Attachments")),
                //    RequestPath = new PathString("/Documents")
                //});

                app.UseHangfireServer();
                app.UseHangfireDashboard("/hangfire", new DashboardOptions
                {
                    Authorization = new[] { new HangfireAuthorizeFilter() }
                });

                //app.UseHangfireDashboard("/hangfire", new DashboardOptions
                //{
                //    Authorization = new[] { new HangfireAuthorizeFilter() }
                //});

                //var options = new BackgroundJobServerOptions { WorkerCount = int.Parse(Configuration.GetSection("AppSettings")["HangfireNumberOfWorkers"]) };
                //app.UseHangfireServer(options);

                //var opts = new SqlServerStorageOptions
                //{
                //    QueuePollInterval = TimeSpan.FromSeconds(1) // Default value
                //};

                //GlobalConfiguration.Configuration.UseSqlServerStorage(Configuration.GetSection("AppSettings")["ConnectionStringHangfireDB"], opts);
                app.UseMvc();
                app.UseSwagger();
                //app.UseSwaggerUi();

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "Core Api");
                    c.DefaultModelsExpandDepth(0);
                });
                //app.UseDirectoryBrowser(new DirectoryBrowserOptions()
                //{
                //    FileProvider = new PhysicalFileProvider(
                //        Path.Combine(Directory.GetCurrentDirectory(), @"Documents", "Attachments")),
                //    RequestPath = new PathString("/Documents")
                //});
                //app.UseFileServer(enableDirectoryBrowsing: true);


                app.UseDefaultFiles(); // Default startup file of app. 
                app.UseStaticFiles(); // wwwroot inside file access. 

                ////                       //app.UseWelcomePage(); // Welcome Page. 

                //var path = Path.Combine(env.ContentRootPath, "Documents", "Attachments"); // this allows for serving up contents in a folder named 'static'
                //var provider = new PhysicalFileProvider(path);

                //var options1 = new StaticFileOptions();
                //options1.RequestPath = "Documents"; // an empty string will give the *appearance* of it being served up from the root
                //                                    //options.RequestPath = "/content"; // this will use the URL path named content, but could be any made-up name you want
                //options1.FileProvider = provider;

                //app.UseStaticFiles(options1);
                try
                {
                    string dir = "", PathToFileDocuments = Configuration.GetSection("AppSettings")["PathToFileDocuments"];
                    if (PathToFileDocuments.Contains(":"))
                        dir = PathToFileDocuments;
                    else
                        dir = Path.Combine(env.ContentRootPath, PathToFileDocuments);

                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    app.UseStaticFiles(new StaticFileOptions()
                    {
                        FileProvider = new PhysicalFileProvider(dir),
                        RequestPath = new PathString("/Documents") // accessing listed directory contents. 
                    });
                    app.UseDirectoryBrowser(new DirectoryBrowserOptions()
                    {
                        FileProvider = new PhysicalFileProvider(dir),
                        RequestPath = new PathString("/Documents") // listing directory details for specified folder. 
                    });
                }
                catch (Exception ex)
                {
                    app.Run(async (context) =>
                    {
                        await context.Response.WriteAsync("Error " + ex.Message);
                    });
                    return;
                }



                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync("Running... ");
                });
            }
            catch (Exception ex) { }
        }
    }

    public class HangfireAuthorizeFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}
