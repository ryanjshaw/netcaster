using System.Text.Json.Serialization;
using System.Text.Json;

namespace Netcaster.Frames
{
    public class HexStringJsonConverter : JsonConverter<byte[]>
    {
        public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var hexString = reader.GetString();
            if (string.IsNullOrEmpty(hexString))
                return Array.Empty<byte>();

            return Convert.FromHexString(hexString.Substring(2));
        }

        public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
