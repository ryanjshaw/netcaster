using Microsoft.AspNetCore.Routing;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Netcaster.Frames;

namespace Netcaster.Frames
{
    public static class WebApplicationExtensions
    {
        public static Frame MapFrameGet(this IEndpointRouteBuilder endpoints, string pattern, string name)
        {
            var frame = new Frame(name);
            MapGet(endpoints, pattern, frame);
            return frame;
        }

        public static Frame MapFramePost(this IEndpointRouteBuilder endpoints, string pattern, string name)
        {
            var frame = new Frame(name);
            MapPost(endpoints, pattern, frame);
            return frame;
        }

        public static IEndpointConventionBuilder MapGet(this IEndpointRouteBuilder endpoints, string pattern, Frame frame)
        {
            return endpoints.MapGet(pattern,
                (IServiceProvider serviceProvider, HttpContext httpContext, LinkGenerator linkGenerator) =>
                    HandleRequest(serviceProvider, httpContext, linkGenerator, frame)
            ).WithName(frame.Name);
        }

        public static IEndpointConventionBuilder MapPost(this IEndpointRouteBuilder endpoints, string pattern, Frame frame)
        {
            return endpoints.MapPost(pattern,
                (IServiceProvider serviceProvider, HttpContext httpContext, LinkGenerator linkGenerator) =>
                    HandleRequest(serviceProvider, httpContext, linkGenerator, frame)
            ).WithName(frame.Name);
        }

        private static async Task<IResult> HandleRequest(
            IServiceProvider serviceProvider,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            Frame frame
        )
        {
            var fqLinkGenerator = new FqLinkGenerator(httpContext.Request);

            // TODO support frame validator APIs: (1) direct to hub (2) Airstack Frames Validator

            var fsp = httpContext.Request.HasJsonContentType()
                    ? await FrameSignaturePacketSerializer.ReadAsync(httpContext.Request.Body)
                    : null;

            // Update state by loading fc:frame:state and merging query/route values into it
            var state = FrameState.Read(fsp?.untrustedData.state);
            state.Merge(httpContext.QueryAndRouteValues());

            // TODO support additional state properties on Frame, e.g. as part of active processing

            var image = await frame.ImageSource.GetImage(serviceProvider, fqLinkGenerator, fsp, state);

            var html = FrameToHtmlAdapter.Adapt(httpContext, linkGenerator, fqLinkGenerator, fsp, frame, state, image);

            return Results.Text(html.ToString(), "text/html", statusCode: (int)HttpStatusCode.OK);
        }
    }
}
