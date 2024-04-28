using Microsoft.AspNetCore.Builder;

namespace Netcaster.Frames
{
    public static class FrameExtensions
    {
        /// <summary>
        /// Convenience method equivalent to <code>app.MapGet(pattern, frame)</code>
        /// </summary>
        public static Frame WithGetUrl(this Frame frame, WebApplication app, string pattern)
        {
            app.MapGet(pattern, frame);
            return frame;
        }

        /// <summary>
        /// Convenience method equivalent to <code>app.MapPost(pattern, frame)</code>
        /// </summary>
        public static Frame WithPostUrl(this Frame frame, WebApplication app, string pattern)
        {
            app.MapPost(pattern, frame);
            return frame;
        }
    }
}
