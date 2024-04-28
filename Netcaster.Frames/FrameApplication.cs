using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Netcaster.Frames
{
    public static class FrameApplication
    {
        public static WebApplication Create(string[] args, Action<WebApplicationBuilder> configure = null)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseContentRoot(Directory.GetCurrentDirectory());
            builder.Services.AddHttpClient();
            configure?.Invoke(builder);
            var app = builder.Build();
            app.UseStaticFiles();
            return app;
        }
    }
}
