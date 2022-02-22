using CryptoBase.Abstractions.Digests;
using CryptoBase.Digests.MD5;
using System;
using System.Buffers;
using System.Text;
using System.Threading;

namespace Shadowsocks.Protocol.Shadowsocks.Crypto
{
    public static class CryptoUtils
    {
        private static readonly ThreadLocal<IHash> _md5Digest = new(() => new DefaultMD5Digest());

        public static byte[] SSKDF(string password, int keylen)
        {
            const int md5Length = 16;
            var pwMaxSize = Encoding.UTF8.GetMaxByteCount(password.Length);
            var key = new byte[keylen];

            var pwBuffer = ArrayPool<byte>.Shared.Rent(pwMaxSize);
            var resultBuffer = ArrayPool<byte>.Shared.Rent(pwMaxSize + md5Length);
            try
            {
                var pwLength = Encoding.UTF8.GetBytes(password, pwBuffer);
                var pw = pwBuffer.AsSpan(0, pwLength);
                Span<byte> md5Sum = stackalloc byte[md5Length];
                var result = resultBuffer.AsSpan(0, pwLength + md5Length);
                var i = 0;
                while (i < keylen)
                {
                    if (i == 0)
                    {
                        _md5Digest.Value!.UpdateFinal(pw, md5Sum);
                    }
                    else
                    {
                        md5Sum.CopyTo(result);
                        pw.CopyTo(result.Slice(md5Length));
                        _md5Digest.Value!.UpdateFinal(result, md5Sum);
                    }

                    var length = Math.Min(16, keylen - i);
                    md5Sum.Slice(0, length).CopyTo(key.AsSpan(i, length));

                    i += md5Length;
                }
                return key;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(pwBuffer);
                ArrayPool<byte>.Shared.Return(resultBuffer);
            }
        }
    }
}
