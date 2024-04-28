using System.Xml.Linq;

namespace Netcaster.Frames
{
    /// <summary>
    /// Represents a logical Farcaster frame
    /// </summary>
    /// <param name="name">Route name (not the pattern)</param>
    public class Frame(string name)
    {
        public string Name => name;

        public ImageSource ImageSource { get; set; } = default;
        public XElement? Body { get; set; } = default!;

        public Frame WithButton(Button button)
        {
            _buttons.Add(button);
            return this;
        }

        public Frame WithButton(string content, Frame target)
        {
            return WithButton(new FrameButton(content, target));
        }

        public Frame WithQueryButton(string content, Frame target, params (object, object)[] queryValues)
        {
            ArgumentNullException.ThrowIfNull(queryValues);
            return WithButton(new QueryButton(content, target, queryValues));
        }
        public Frame WithQueryButton(string content, Frame target, bool mergeQueryValues, params (object, object)[] queryValues)
        {
            ArgumentNullException.ThrowIfNull(queryValues);
            return WithButton(new QueryButton(content, target, mergeQueryValues, queryValues));
        }

        public Frame WithButton(string content, Frame target, object routeValues)
        {
            ArgumentNullException.ThrowIfNull(routeValues);
            return WithButton(new RouteButton(content, target, routeValues));
        }

        public Frame WithImage(ImageSource image)
        {
            if (ImageSource != null) throw new Exception("Frame image has already been set");

            ImageSource = image;
            return this;
        }

        /// <summary>
        /// Use this variant of <see cref="WithImage(ImageSource)"/> when you don't care to reuse the <see cref="ImageSource"/>
        /// </summary>
        /// <param name="aspectRatio">TODO</param>
        /// <param name="path">TODO</param>
        /// <returns>TODO</returns>
        public Frame WithLocalImage(AspectRatio aspectRatio, string path)
        {
            return WithImage(new LocalImageSource(aspectRatio, path));
        }

        public Frame WithRemoteImage(AspectRatio aspectRatio, Uri uri)
        {
            return WithImage(new RemoteImageSource(aspectRatio, uri));
        }

        public Frame WithDataImage(AspectRatio aspectRatio, string data)
        {
            // TODO
            throw new NotImplementedException();
        }

        public Frame WithDynamicImage(GenerateImage generate)
        {
            return WithImage(new DynamicImageSource(generate));
        }

        public Frame WithDynamicImage<TState>(Delegate generate)
        {
            return WithImage(new DynamicImageSource<TState>(generate));
        }

        private Frame WithBody(XElement body)
        {
            Body = body;
            return this;
        }

        private readonly List<Button> _buttons = [];
        public IReadOnlyList<Button> Buttons => _buttons;
    }
}
