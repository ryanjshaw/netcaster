using System.Text.Json;
using System.Text.Json.Serialization;

namespace Netcaster.Frames
{
    public abstract class ImageSource()
    {
        public abstract Task<Image> GetImage(
            IServiceProvider serviceProvider,
            FqLinkGenerator linkGenerator,
            FrameSignaturePacket? fsp,
            FrameState state
        );
    }

    /// <summary>
    /// Represents an image embedded directly into the frame HTML
    /// </summary>
    /// <param name="aspectRatio">Aspect ratio to present the image with.</param>
    /// <param name="image">Image data.</param>
    public class EmbeddedImageSource(AspectRatio aspectRatio, ImageType imageType, MemoryStream image) : ImageSource
    {
        public override Task<Image> GetImage(
            IServiceProvider serviceProvider,
            FqLinkGenerator linkGenerator,
            FrameSignaturePacket? fsp,
            FrameState state
        )
        {
            var contentType = imageType switch
            {
                ImageType.PNG => "image/png",
                ImageType.JPEG => "image/jpeg",
                ImageType.GIF => "image/gif",
                _ => throw new Exception($"Unimplemented {nameof(ImageType)} enum value: {imageType}")
            };

            var uri = $"data:{contentType};base64," + Convert.ToBase64String(image.ToArray());
            return Task.FromResult<Image>(new(aspectRatio, new Uri(uri)));
        }
    }

    /// <summary>
    /// Represents an image hosted statically on the host server
    /// </summary>
    /// <param name="aspectRatio">Aspect ratio to present the image with.</param>
    /// <param name="path">Relative path to the image on the host server. Will be expanded into a FQ URI.</param>
    public class LocalImageSource(AspectRatio aspectRatio, string path) : ImageSource
    {
        public override Task<Image> GetImage(
            IServiceProvider serviceProvider,
            FqLinkGenerator linkGenerator,
            FrameSignaturePacket? fsp,
            FrameState state
        )
        {
            var uri = linkGenerator.GetUriByPath(path);
            return Task.FromResult<Image>(new(aspectRatio, uri));
        }
    }

    /// <summary>
    /// Represents an image hosted statically on an external server
    /// </summary>
    /// <param name="aspectRatio">Aspect ratio to present the image with.</param>
    /// <param name="path">Fully qualified URI to the image</param>
    public class RemoteImageSource(AspectRatio aspectRatio, Uri uri) : ImageSource
    {
        public override Task<Image> GetImage(
            IServiceProvider serviceProvider,
            FqLinkGenerator _,
            FrameSignaturePacket? fsp,
            FrameState __
        ) => Task.FromResult<Image>(new(aspectRatio, uri));
    }

    /// <summary>
    /// Represents a dynamically generated image
    /// </summary>
    /// <param name="generate">Dynamic image generation function</param>
    public class DynamicImageSource(GenerateImage generate) : ImageSource
    {
        public override Task<Image> GetImage(
            IServiceProvider serviceProvider,
            FqLinkGenerator fq,
            FrameSignaturePacket? fsp,
            FrameState state
        )
        {
            return generate(fsp, state, fq);
        }
    }

    /// <summary>
    /// Represents a dynamically generated image
    /// </summary>
    /// <typeparam name="TState">The type to deserialize fc:frame:state into</typeparam>
    /// <param name="generate">Dynamic image generation function</param>
    public class DynamicImageSource<TState>(Delegate generate) : ImageSource
    {
        readonly static JsonSerializerOptions _jsonOptions;
        static DynamicImageSource()
        {
            _jsonOptions = new JsonSerializerOptions();
            _jsonOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public override async Task<Image> GetImage(
            IServiceProvider serviceProvider,
            FqLinkGenerator _,
            FrameSignaturePacket? fsp,
            FrameState state
        )
        {
            var json = JsonSerializer.Serialize(state);
            var tstate = JsonSerializer.Deserialize<TState>(json, _jsonOptions)
                ?? throw new Exception($"Could not deserialize frame state to {typeof(TState).FullName}");

            // HBD: we support arbitrary DI-injected parameters in the delegate, to simplify Framecaster usage
            var method = generate.Method;
            var parameters = method.GetParameters();
            var arguments = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;

                if (parameterType == typeof(FrameSignaturePacket))
                    arguments[i] = fsp!;
                else if (parameterType == typeof(FrameState))
                    arguments[i] = state;
                else if (parameterType == typeof(TState))
                    arguments[i] = tstate;
                else
                    arguments[i] = serviceProvider.GetService(parameters[i].ParameterType)
                        ?? throw new InvalidOperationException($"Service for type {parameters[i].ParameterType} not found");
            }

            var result = generate.DynamicInvoke(arguments);

            // Check if the delegate is asynchronous
            if (result is Task task)
            {
                await task.ConfigureAwait(false);
                // Handle Task<T> to extract the result
                if (task.GetType().IsGenericType)
                {
                    return (Image)((dynamic)task).Result;
                }
                throw new Exception("Missing async result");
            }

            return (Image)result; // For synchronous methods returning ImageSource

        }
    }

    public delegate Task<Image> GenerateImage(FrameSignaturePacket? fsp, FrameState state, FqLinkGenerator linkGenerator);
    public delegate Task<Image> GenerateImage<TState>(FrameSignaturePacket? fsp, TState state);

    public record Image(AspectRatio AspectRatio, Uri uri);

    public enum AspectRatio
    {
        /// <summary>
        /// 1:1
        /// </summary>
        Square = 1,

        /// <summary>
        /// 1:1.91
        /// </summary>
        Wide = 2
    }

    public enum ImageType
    {
        PNG = 1,
        JPEG = 2,
        GIF = 3
    }

}
