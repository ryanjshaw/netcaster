using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Netcaster.Frames
{
    public static class HttpContextExtensions
    {
        public static IEnumerable<KeyValuePair<string, StringValues>> QueryValues(this HttpContext httpContext)
        {
            return httpContext.Request.Query.Cast<KeyValuePair<string, StringValues>>();
        }

        public static Dictionary<string, object> QueryAndRouteValues(this HttpContext httpContext)
        {
            var combined = new Dictionary<string, object>();

            // Add items from queryValues to the combined dictionary
            foreach (var pair in httpContext.QueryValues())
            {
                // If StringValues contains more than one value, store it as an array; otherwise, store a single value
                combined[pair.Key] = pair.Value.Count > 1 ? pair.Value.ToArray() : pair.Value.ToString();
            }

            // Add items from routeValues to the combined dictionary
            foreach (var pair in httpContext.Request.RouteValues)
            {
                if (!combined.ContainsKey(pair.Key))
                {
                    combined[pair.Key] = pair.Value!;
                }
            }

            return combined;
        }
    }
}
