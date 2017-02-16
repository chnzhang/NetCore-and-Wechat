using Microsoft.AspNetCore.Builder;
using System;


namespace Microsoft.Extensions.DependencyInjection
{
    //借鉴自yuangang007
    public static class Extensions
    {
        public static IServiceCollection AddWkMvcDI(this IServiceCollection services)
        {
            return services;
        }
        public static IApplicationBuilder UseWkMvcDI(this IApplicationBuilder builder)
        {
            DI.ServiceProvider = builder.ApplicationServices;
            return builder;
        }
    }

    public static class DI
    {
        public static IServiceProvider ServiceProvider { get; set; }
    }
}
