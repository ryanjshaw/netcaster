using Microsoft.AspNetCore.Routing;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System.Text.Json;

namespace Netcaster.Frames
{
    public class FrameToHtmlAdapter
    {
        public static XDocument Adapt(
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            FqLinkGenerator fqLinkGenerator,
            FrameSignaturePacket? fsp,
            Frame frame,
            FrameState state,
            Image image
        )
        {
            if (frame.ImageSource == null)
                throw new ValidationException("Frame must specify an image");

            if (frame.Buttons.Count > 4)
                throw new ValidationException("Frame can't have more than 4 buttons");

            var html =
                new XDocument(
                    new XElement("html",
                        new XElement("head", metas()),
                        new XElement("body", frame.Body)
                    )
                );

            return html;

            IEnumerable<XElement> metas()
            {
                yield return XMeta("fc:frame", "vNext");
                ;
                var (aspectRatio, imageContent) = AdaptImage(image);
                yield return XMeta("fc:frame:image", imageContent);
                yield return XMeta("fc:frame:image:aspect_ratio", aspectRatio);
                // TODO og fallback

                for (var i = 0; i < frame.Buttons.Count; i++)
                {
                    var property = $"fc:frame:button:{i + 1}";

                    var button = frame.Buttons[i];
                    yield return XMeta(property, button.Content);
                    yield return XMeta(property, "action", button.Action);

                    var (postUrl, target) = AdaptButton(httpContext, linkGenerator, fqLinkGenerator, button);
                    if (postUrl != null)
                        yield return XMeta(property, "post_url", postUrl);
                    if (target != null)
                        yield return XMeta(property, "target", target);
                }

                // Populate fc:frame:state if there's any state available
                if (state.Count > 0)
                {
                    var stateJson = JsonSerializer.Serialize(state);
                    yield return XMeta("fc:frame:state", stateJson);
                }
            }
        }

        internal static XElement XMeta(string property, object content)
        {
            return new XElement("meta",
                new XAttribute("property", property),
                new XAttribute("content", content)
            );
        }

        internal static XElement XMeta(string property, string subProperty, object content) =>
            XMeta(property + ":" + subProperty, content);

        internal static (string, string) AdaptImage(Image image)
        {
            var aspectRatio = image.AspectRatio switch
            {
                AspectRatio.Square => "1:1",
                AspectRatio.Wide => "1:1.91",
                _ => throw new NotImplementedException($"Unknown aspect ratio: {image.AspectRatio}")
            };

            return (aspectRatio, image.uri.ToString());
        }

        // TODO this logic probably shouldn't be here
        internal static (string? postUrl, string? target) AdaptButton(
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            FqLinkGenerator fqLinkGenerator,
            Button button
        )
        {
            // TODO should existing route/query values be supported by either button type?
            switch (button)
            {
                case RouteButton routeButton:
                    {
                        // Merging route values from the current frame into the new frame query is mandatory
                        var routeValues =
                            new RouteValueDictionary(httpContext.Request.RouteValues.Concat(
                                new RouteValueDictionary(routeButton.RouteValues)
                            ));

                        return getPathByName(routeButton.Destination, routeValues, null);
                    }

                case QueryButton queryButton:
                    {
                        // Merging query values from the current frame into the new frame query is optional
                        var currentQueryValues =
                            queryButton.MergeQueryValues
                            ? httpContext.QueryValues()
                            : [];

                        var nextQueryValues =
                            from kv in queryButton.QueryValues
                            select KeyValuePair.Create(kv.Item1, kv.Item2);

                        var queryValues = currentQueryValues.Concat(nextQueryValues);

                        return getPathByName(queryButton.Destination, null, queryValues);
                    }

                case FrameButton frameButton:
                    {
                        return getPathByName(frameButton.Destination, null, null);
                    };

                default:
                    throw new NotImplementedException($"Unimplemented button type: {button.GetType().Name}");
            };

            (string? postUrl, string? target) getPathByName(
                Frame destination,
                RouteValueDictionary? routeValues,
                IEnumerable<KeyValuePair<string, StringValues>>? queryValues
            )
            {
                var path = linkGenerator.GetPathByName(destination.Name, routeValues)
                    ?? throw new Exception($"Button target route name {destination.Name} is not registered");

                var target = fqLinkGenerator.GetUriByPath(path).ToString();

                if (queryValues != null)
                    target = QueryHelpers.AddQueryString(target, queryValues);

                return (null, target);
            }
        }
    }
}
