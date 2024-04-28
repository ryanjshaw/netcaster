using Microsoft.AspNetCore.Http;

namespace Netcaster.Frames
{
    public class FqLinkGenerator(HttpRequest request)
    {
        /// <summary>
        /// Produces a fully qualified URI
        /// </summary>
        /// <param name="path">A local path on the server</param>
        /// <returns>A fully qualified path, based on the request URI</returns>
        public Uri GetUriByPath(string path)
        {
            var defaultPort = request.Scheme switch
            {
                "https" => 443,
                "http" => 80,
                _ => throw new NotImplementedException($"Unknown defaultPort for scheme {request.Scheme}")
            };

            var uriBuilder = new UriBuilder
            {
                Scheme = request.Scheme,
                Host = request.Host.Host,
                Port = request.Host.Port ?? defaultPort,
                Path = path
            };

            return uriBuilder.Uri;
        }
    }
}
