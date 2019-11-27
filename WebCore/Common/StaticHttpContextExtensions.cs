using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using RoadFlow.Business.SignalR;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCore
{
    public static class StaticHttpContextExtensions
    {
        // Methods
        public static void AddHttpContextAccessor(this IServiceCollection services)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(services);
        }

        public static IApplicationBuilder UseStaticHttpContext(this IApplicationBuilder app)
        {
            IHttpContextAccessor requiredService = ServiceProviderServiceExtensions.GetRequiredService<IHttpContextAccessor>(app.ApplicationServices);
            //2.2Core 版本
            //  IHostingEnvironment hostingEnvironment = ServiceProviderServiceExtensions.GetRequiredService<IHostingEnvironment>(app.ApplicationServices);
            IWebHostEnvironment hostingEnvironment = ServiceProviderServiceExtensions.GetRequiredService<IWebHostEnvironment>(app.ApplicationServices);
            IHubContext<SignalRHub> accessor = ServiceProviderServiceExtensions.GetRequiredService<IHubContext<SignalRHub>>(app.ApplicationServices);
            Tools.ConfigureHttpContext(requiredService);
            Tools.ConfigureHostingEnvironment(hostingEnvironment);
            SignalRHub.Configure(accessor);
            return app;
        }
    }


}
