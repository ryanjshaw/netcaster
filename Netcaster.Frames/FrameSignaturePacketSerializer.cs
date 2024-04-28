using System.Text.Json;

namespace Netcaster.Frames
{
    public class FrameSignaturePacketSerializer
    {
        private static readonly JsonSerializerOptions _serializerOptions;
        static FrameSignaturePacketSerializer()
        {
            _serializerOptions = new JsonSerializerOptions();
            _serializerOptions.Converters.Add(new HexStringJsonConverter());
        }

        public static async ValueTask<FrameSignaturePacket> ReadAsync(Stream stream)
        {
            return await JsonSerializer.DeserializeAsync<FrameSignaturePacket>(stream, _serializerOptions)
                ?? throw new Exception($"Failed to deserialize {nameof(FrameSignaturePacket)}");
        }
    }
}
