using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;

using LogUtility.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using RoadFlow.Business.SignalR;
using RoadFlow.Utility;
using WebCore.Areas.Controllers.Areas.Quartz.Controllers;
using Microsoft.Extensions.Logging;
using System.Reflection;
using NLog.Extensions.Logging;

namespace WebCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Config.InitUserPassword = this.Configuration.GetSection("InitUserPassword").Value;
            Config.IsDebug = "1".Equals(this.Configuration.GetSection("IsDebug").Value);
            Config.DebugUserId = this.Configuration.GetSection("DebugUserId").Value;
            Config.SingleLogin = "1".Equals(this.Configuration.GetSection("SingleLogin").Value);
            Config.ShowError = this.Configuration.GetSection("ShowError").Value.ToInt(0);
            Config.FilePath = this.Configuration.GetSection("FilePath").Value.TrimEnd('/').TrimEnd('\\');
            Config.UploadFileExtNames = this.Configuration.GetSection("FileExtName").Value;
            Config.IsLog = "1".Equals(this.Configuration.GetSection("IsLog").Value);

            Config.DatabaseType = this.Configuration.GetSection("DatabaseType").Value.ToLower();
            Config.IsLoginAddress = "1".Equals(this.Configuration.GetSection("IsLoginAddress").Value);
            Config.SystemTables = this.Configuration.GetSection("SystemTables").Value;
            Config.IsRemarks = this.Configuration.GetSection("IsRemarks").Value.ToString();
            Config.ProjectName = this.Configuration.GetSection("ProjectName").Value.ToString();
            Config.EnableDynamicStep = this.Configuration.GetSection("EnableDynamicStep").Value.ToInt(0) == 1;

            Config.CacheName = this.Configuration.GetSection("CacheName").Value;
            Config.MailWithdrawTime = Convert.ToDouble(this.Configuration.GetSection("MailWithdrawTime").Value);
            Config.ConnectionString_SqlServer = ConfigurationExtensions.GetConnectionString(this.Configuration, "RF_SqlServer");
            Config.ConnectionString_MySql = ConfigurationExtensions.GetConnectionString(this.Configuration, "RF_MySql");
            Config.ConnectionString_Oracle = ConfigurationExtensions.GetConnectionString(this.Configuration, "RF_Oracle");
            Config.ConnectionString_PostgreSql = ConfigurationExtensions.GetConnectionString(this.Configuration, "RF_PostgreSql");
            Config.ConnectionString_Sqlite = ConfigurationExtensions.GetConnectionString(this.Configuration, "RF_Sqlite");
            Config.IsFormPower = "1".Equals(this.Configuration.GetSection("IsFormPower").Value);
            Config.SystemVersion = this.Configuration.GetSection("Version").Value;

            Config.IsIntegrateIframe = "1".Equals(this.Configuration.GetSection("IsIntegrateIframe").Value);


            //日志配置
            // NLog.LogManager.Configuration.FindTargetByName<DatabaseTarget>("db").ConnectionString = ConfigurationExtensions.GetConnectionString(this.Configuration, "LogDatabase"); 

            IConfigurationSection section = this.Configuration.GetSection("EnterpriseWeiXin");
            if (section != null)
            {
                Config.Enterprise_WeiXin_AppId = section.GetSection("AppId").Value;
                Config.Enterprise_WeiXin_WebUrl = section.GetSection("WebUrl").Value;
                Config.Enterprise_WeiXin_IsUse = "1".Equals(section.GetSection("IsUse").Value);
                Config.Enterprise_WeiXin_IsSyncOrg = "1".Equals(section.GetSection("IsSyncOrg").Value);

            }
            IConfigurationSection section5 = this.Configuration.GetSection("WeiXin");
            if (section5 != null)
            {
                Config.WeiXin_IsUse = "1".Equals(section5.GetSection("IsUse").Value);
                Config.WeiXin_AppId = section5.GetSection("AppId").Value;
                Config.WeiXin_AppSecret = section5.GetSection("AppSecret").Value;
                Config.WeiXin_WebUrl = section5.GetSection("WebUrl").Value;
            }



            IConfigurationSection section3 = configuration.GetSection("EngineCenter");
            if (section3 != null)
            {
                Config.EngineCenter_IsUse = "1".Equals(section3.GetSection("IsUse").Value);
            }

            IConfigurationSection section2 = this.Configuration.GetSection("Session");
            if (section2 != null)
            {
                Config.CookieName = ConfigurationBinder.GetValue<string>(section2, "CookieName");

                Config.UserIdSessionKey = ConfigurationBinder.GetValue<string>(section2, "UserIdKey");
                Config.SessionTimeout = ConfigurationBinder.GetValue<int>(section2, "TimeOut");
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //添加NLog日志操作
           services.AddNLog();
        

            //配置上传文件配置
            services.Configure<FormOptions>(option =>
            {
                option.ValueCountLimit = 1073741822;
                option.ValueLengthLimit = 1073741822;
                option.KeyLengthLimit = 1073741822;
                option.MultipartBodyLengthLimit = 1073741822;
                option.MultipartBoundaryLengthLimit = 1073741822;

            });


            //异常过滤器
            services.AddMvc(options =>
            {
               // options.Filters.Add<ErrorHandleAttribute>();
            });

            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //});


          

            //分布式内存缓存
            services.AddDistributedMemoryCache();

            //Session会话
            services.AddSession(options =>
            {
                options.Cookie.Name = Config.CookieName;
                options.IdleTimeout = TimeSpan.FromMinutes((double)Config.SessionTimeout);
            });

            //上下文访问器
            services.AddHttpContextAccessor();


            //Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(options =>
            {
                //options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "RoadFlowCore-API",
                    Version = "v1",
                    Description = "RoadFlowCore-API. This is a sample Flow",
                });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath, true);

            });


            //添加内存缓存
            services.AddMemoryCache();


            //添加时间工作
            services.AddTimedJob();

            //添加实时通信
            services.AddSignalR();

            //添加短信服务
            RoadFlow.Utility.SMS.Extensions.SMSExtensions.AddLuoSiMao(services, "b31971de931b909e8a1e30c6df37b9ea");

            //添加定时作业
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            //添加Util基础设施服务
            services.AddUtil();

            //调用内置Json
            services.AddMvc().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

          
            services.AddControllersWithViews();
        }

       
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {

            //日志模块
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);//这是为了防止中文乱码
                                                                          //导致单体文件失败
                                                                          //NLog.LogManager.Configuration.Variables["configDir"] = this.Configuration.GetSection("FilePath").Value.TrimEnd('/').TrimEnd('\\');

          


            //#region 远程读取日志

            //if (Config.IsLog)
            //{
            //    FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();
            //    //  provider.Mappings[".log"] = "text/plain;charset=utf-8";
            //    provider.Mappings[".log"] = "text/plain;charset=utf-8";



            //    string basePath = this.Configuration.GetSection("FilePath").Value.TrimEnd('/').TrimEnd('\\');// System.IO.Path.GetDirectoryName(typeof(Program).Assembly.Location);
            //    if (!Directory.Exists(basePath + "/logs"))
            //    {
            //        Directory.CreateDirectory(basePath + "/logs");
            //    }
            //    app.UseStaticFiles(new StaticFileOptions()
            //    {
            //        FileProvider = new PhysicalFileProvider(System.IO.Path.Combine(basePath, "logs")),
            //        ServeUnknownFileTypes = true,
            //        RequestPath = new PathString("/logs"),
            //        ContentTypeProvider = provider,
            //        DefaultContentType = "application/x-msdownload", // 设置未识别的MIME类型一个默认z值

            //    });
            //    app.UseDirectoryBrowser(new DirectoryBrowserOptions()
            //    {
            //        FileProvider = new PhysicalFileProvider(System.IO.Path.Combine(basePath, "logs")),
            //        RequestPath = new PathString("/logs"),
            //    });
            //}


            //#endregion



            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }



            app.UseHttpsRedirection();

            app.UseStaticHttpContext();

            app.UseStaticFiles();



            app.UseCookiePolicy();

            app.UseSession();


            //扩展用户时间工作
            Microsoft.AspNetCore.Builder.TimedJobExtensions.UseTimedJob(app);

            app.UseWebSockets();


            //定时作业
          app.UseQuartz();




            app.UseRouting();


            app.UseSwagger().UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            //app.UseAuthentication();

            //app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<SignalRHub>("/SignalRHub");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                     name: "areas",
                     pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


            });
           
        }
    }
}
