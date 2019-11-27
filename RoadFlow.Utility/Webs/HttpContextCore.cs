using Microsoft.AspNetCore.Http;

namespace RoadFlow.Utility
{

    public static class HttpContextCore
    {
        public static HttpContext Current { get => AutofacHelper.GetService<Microsoft.AspNetCore.Http.IHttpContextAccessor>().HttpContext; }
    }
}
