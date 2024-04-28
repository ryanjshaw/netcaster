using System.Text.Json;

namespace Netcaster.Frames
{
    public class FrameState : Dictionary<string, object>
    {
        public FrameState(IEnumerable<KeyValuePair<string, object>> collection)
            : base(collection)
        { }

        public static FrameState Read(string? state)
        {
            var data =
                state != null
                ? JsonSerializer.Deserialize<Dictionary<string, object>>(state)
                    ?? throw new Exception($"Unable to deserialize FrameSignaturePacket.state")
                : [];

            return new FrameState(data);
        }

        public void Merge(IEnumerable<KeyValuePair<string, object>> collection)
        {
            foreach (var (key, value) in collection)
                Add(key, value);
        }
    }
}
