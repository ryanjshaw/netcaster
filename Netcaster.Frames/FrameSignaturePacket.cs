namespace Netcaster.Frames
{
    public class FrameSignaturePacket
    {
        public UntrustedData untrustedData { get; set; } = default!;
        public TrustedData trustedData { get; set; } = default!;

        public class UntrustedData
        {
            public long fid { get; set; }
            public string url { get; set; } = default!;
            public byte[] messageHash { get; set; } = default!;
            public long timestamp { get; set; }
            public long network { get; set; }
            public int buttonIndex { get; set; }
            public string inputText { get; set; } = default!;
            public string state { get; set; } = default!;
            public byte[] transactionId { get; set; } = default!;
            public byte[] address { get; set; } = default!;
            public CastId castId { get; set; } = default!;
        }

        public class TrustedData
        {
            public byte[] messageBytes { get; set; } = default!;
        }

        public class CastId
        {
            public long fid { get; set; }
            public byte[] hash { get; set; } = default!;
        }
    }
}