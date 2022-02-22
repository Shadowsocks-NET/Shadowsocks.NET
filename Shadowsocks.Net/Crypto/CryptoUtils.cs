using CryptoBase.Abstractions.Digests;
using CryptoBase.Digests.MD5;
using System;
using System.Threading;

namespace Shadowsocks.Net.Crypto
{
    public static class CryptoUtils
    {
        private static readonly ThreadLocal<IHash> _md5Digest = new(() => new DefaultMD5Digest());
        private static readonly ThreadLocal<IHash> _md5DigestFast = new(() => new Fast440MD5Digest());

        public static byte[] MD5(byte[] b)
        {
            var hash = new byte[CryptoBase.MD5Length];
            _md5Digest.Value!.UpdateFinal(b, hash);
            return hash;
        }
        public static void Fast440(in ReadOnlySpan<byte> origin, Span<byte> destination)
        {
            _md5DigestFast.Value!.UpdateFinal(origin, destination);
        }
    }
}
