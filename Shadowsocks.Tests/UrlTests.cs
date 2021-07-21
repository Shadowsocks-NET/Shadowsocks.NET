using Shadowsocks.Models;
using Xunit;

namespace Shadowsocks.Tests
{
    public class UrlTests
    {
        [Theory]
        [InlineData("chacha20-ietf-poly1305", "kf!V!TFzgeNd93GE", "Y2hhY2hhMjAtaWV0Zi1wb2x5MTMwNTprZiFWIVRGemdlTmQ5M0dF")]
        [InlineData("aes-256-gcm", "ymghiR#75TNqpa", "YWVzLTI1Ni1nY206eW1naGlSIzc1VE5xcGE")]
        [InlineData("aes-128-gcm", "tK*sk!9N8@86:UVm", "YWVzLTEyOC1nY206dEsqc2shOU44QDg2OlVWbQ")]
        public void Utilities_Base64Url_Encode(string method, string password, string expectedUserinfoBase64url)
        {
            var userinfoBase64url = Utils.Base64Url.Encode($"{method}:{password}");

            Assert.Equal(expectedUserinfoBase64url, userinfoBase64url);
        }

        [Theory]
        [InlineData("Y2hhY2hhMjAtaWV0Zi1wb2x5MTMwNTo2JW04RDlhTUI1YkElYTQl", "chacha20-ietf-poly1305:6%m8D9aMB5bA%a4%")]
        [InlineData("YWVzLTI1Ni1nY206YnBOZ2sqSjNrYUFZeXhIRQ", "aes-256-gcm:bpNgk*J3kaAYyxHE")]
        [InlineData("YWVzLTEyOC1nY206dkFBbiY4a1I6JGlBRTQ", "aes-128-gcm:vAAn&8kR:$iAE4")]
        public void Utilities_Base64Url_Decode(string userinfoBase64url, string expectedUserinfo)
        {
            var userinfo = Utils.Base64Url.DecodeToString(userinfoBase64url);

            Assert.Equal(expectedUserinfo, userinfo);
        }

        [Theory]
        [InlineData("aes-256-gcm", "wLhN2STZ", "github.com", 443, "", null, null, null, null, "ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@github.com:443/")] // domain name
        [InlineData("aes-256-gcm", "wLhN2STZ", "1.1.1.1", 853, "", null, null, null, null, "ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@1.1.1.1:853/")] // IPv4
        [InlineData("aes-256-gcm", "wLhN2STZ", "2001:db8:85a3::8a2e:370:7334", 8388, "", null, null, null, null, "ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@[2001:db8:85a3::8a2e:370:7334]:8388/")] // IPv6
        [InlineData("aes-256-gcm", "wLhN2STZ", "github.com", 443, "GitHub", null, null, null, null, "ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@github.com:443/#GitHub")] // fragment
        [InlineData("aes-256-gcm", "wLhN2STZ", "github.com", 443, "👩‍💻", null, null, null, null, "ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@github.com:443/#%F0%9F%91%A9%E2%80%8D%F0%9F%92%BB")] // fragment
        [InlineData("aes-256-gcm", "wLhN2STZ", "github.com", 443, "", "v2ray-plugin", null, null, null, "ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@github.com:443/?plugin=v2ray-plugin")] // pluginName
        [InlineData("aes-256-gcm", "wLhN2STZ", "github.com", 443, "", null, "1.0", null, null, "ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@github.com:443/")] // pluginVersion
        [InlineData("aes-256-gcm", "wLhN2STZ", "github.com", 443, "", null, null, "server;tls;host=github.com", null, "ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@github.com:443/")] // pluginOptions
        [InlineData("aes-256-gcm", "wLhN2STZ", "github.com", 443, "", null, null, null, "-vvvvvv", "ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@github.com:443/")] // pluginArguments
        [InlineData("aes-256-gcm", "wLhN2STZ", "github.com", 443, "", "v2ray-plugin", "1.0", null, null, "ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@github.com:443/?plugin=v2ray-plugin&pluginVersion=1.0")] // pluginName + pluginVersion
        [InlineData("aes-256-gcm", "wLhN2STZ", "github.com", 443, "", "v2ray-plugin", null, "server;tls;host=github.com", null, "ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@github.com:443/?plugin=v2ray-plugin%3Bserver%3Btls%3Bhost%3Dgithub.com")] // pluginName + pluginOptions
        [InlineData("aes-256-gcm", "wLhN2STZ", "github.com", 443, "", "v2ray-plugin", null, null, "-vvvvvv", "ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@github.com:443/?plugin=v2ray-plugin&pluginArguments=-vvvvvv")] // pluginName + pluginArguments
        [InlineData("aes-256-gcm", "wLhN2STZ", "github.com", 443, "", "v2ray-plugin", "1.0", "server;tls;host=github.com", null, "ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@github.com:443/?plugin=v2ray-plugin%3Bserver%3Btls%3Bhost%3Dgithub.com&pluginVersion=1.0")] // pluginName + pluginVersion + pluginOptions
        [InlineData("aes-256-gcm", "wLhN2STZ", "github.com", 443, "", "v2ray-plugin", "1.0", null, "-vvvvvv", "ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@github.com:443/?plugin=v2ray-plugin&pluginVersion=1.0&pluginArguments=-vvvvvv")] // pluginName + pluginVersion + pluginArguments
        [InlineData("aes-256-gcm", "wLhN2STZ", "github.com", 443, "GitHub", "v2ray-plugin", "1.0", "server;tls;host=github.com", "-vvvvvv", "ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@github.com:443/?plugin=v2ray-plugin%3Bserver%3Btls%3Bhost%3Dgithub.com&pluginVersion=1.0&pluginArguments=-vvvvvv#GitHub")] // fragment + pluginName + pluginVersion + pluginOptions + pluginArguments
        public void Server_ToUrl(string method, string password, string host, int port, string fragment, string? pluginName, string? pluginVersion, string? pluginOptions, string? pluginArguments, string expectedSSUri)
        {
            IServer server = new Server()
            {
                Password = password,
                Method = method,
                Host = host,
                Port = port,
                Name = fragment,
                PluginName = pluginName,
                PluginVersion = pluginVersion,
                PluginOptions = pluginOptions,
                PluginArguments = pluginArguments,
            };

            var ssUriString = server.ToUrl().AbsoluteUri;

            Assert.Equal(expectedSSUri, ssUriString);
        }

        [Theory]
        [InlineData("ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@github.com:443/", true, "aes-256-gcm", "wLhN2STZ", "github.com", 443, "", null, null, null, null)] // domain name
        [InlineData("ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@1.1.1.1:853/", true, "aes-256-gcm", "wLhN2STZ", "1.1.1.1", 853, "", null, null, null, null)] // IPv4
        [InlineData("ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@[2001:db8:85a3::8a2e:370:7334]:8388/", true, "aes-256-gcm", "wLhN2STZ", "2001:db8:85a3::8a2e:370:7334", 8388, "", null, null, null, null)] // IPv6
        [InlineData("ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@github.com:443/#GitHub", true, "aes-256-gcm", "wLhN2STZ", "github.com", 443, "GitHub", null, null, null, null)] // fragment
        [InlineData("ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@github.com:443/#%F0%9F%91%A9%E2%80%8D%F0%9F%92%BB", true, "aes-256-gcm", "wLhN2STZ", "github.com", 443, "👩‍💻", null, null, null, null)] // fragment
        [InlineData("ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@github.com:443/?plugin=v2ray-plugin", true, "aes-256-gcm", "wLhN2STZ", "github.com", 443, "", "v2ray-plugin", null, null, null)] // pluginName
        [InlineData("ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@github.com:443/?pluginVersion=1.0", true, "aes-256-gcm", "wLhN2STZ", "github.com", 443, "", null, null, null, null)] // pluginVersion
        [InlineData("ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@github.com:443/?pluginArguments=-vvvvvv", true, "aes-256-gcm", "wLhN2STZ", "github.com", 443, "", null, null, null, null)] // pluginArguments
        [InlineData("ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@github.com:443/?plugin=v2ray-plugin&pluginVersion=1.0", true, "aes-256-gcm", "wLhN2STZ", "github.com", 443, "", "v2ray-plugin", "1.0", null, null)] // pluginName + pluginVersion
        [InlineData("ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@github.com:443/?plugin=v2ray-plugin%3Bserver%3Btls%3Bhost%3Dgithub.com", true, "aes-256-gcm", "wLhN2STZ", "github.com", 443, "", "v2ray-plugin", null, "server;tls;host=github.com", null)] // pluginName + pluginOptions
        [InlineData("ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@github.com:443/?plugin=v2ray-plugin&pluginArguments=-vvvvvv", true, "aes-256-gcm", "wLhN2STZ", "github.com", 443, "", "v2ray-plugin", null, null, "-vvvvvv")] // pluginName + pluginArguments
        [InlineData("ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@github.com:443/?plugin=v2ray-plugin%3Bserver%3Btls%3Bhost%3Dgithub.com&pluginVersion=1.0", true, "aes-256-gcm", "wLhN2STZ", "github.com", 443, "", "v2ray-plugin", "1.0", "server;tls;host=github.com", null)] // pluginName + pluginVersion + pluginOptions
        [InlineData("ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@github.com:443/?plugin=v2ray-plugin&pluginVersion=1.0&pluginArguments=-vvvvvv", true, "aes-256-gcm", "wLhN2STZ", "github.com", 443, "", "v2ray-plugin", "1.0", null, "-vvvvvv")] // pluginName + pluginVersion + pluginArguments
        [InlineData("ss://YWVzLTI1Ni1nY206d0xoTjJTVFo@github.com:443/?plugin=v2ray-plugin%3Bserver%3Btls%3Bhost%3Dgithub.com&pluginVersion=1.0&pluginArguments=-vvvvvv#GitHub", true, "aes-256-gcm", "wLhN2STZ", "github.com", 443, "GitHub", "v2ray-plugin", "1.0", "server;tls;host=github.com", "-vvvvvv")] // fragment + pluginName + pluginVersion + pluginOptions + pluginArguments
        [InlineData("ss://Y2hhY2hhMjAtaWV0Zi1wb2x5MTMwNTo2JW04RDlhTUI1YkElYTQl@github.com:443/", true, "chacha20-ietf-poly1305", "6%m8D9aMB5bA%a4%", "github.com", 443, "", null, null, null, null)] // userinfo parsing
        [InlineData("ss://YWVzLTI1Ni1nY206YnBOZ2sqSjNrYUFZeXhIRQ@github.com:443/", true, "aes-256-gcm", "bpNgk*J3kaAYyxHE", "github.com", 443, "", null, null, null, null)] // userinfo parsing
        [InlineData("ss://YWVzLTEyOC1nY206dkFBbiY4a1I6JGlBRTQ@github.com:443/", true, "aes-128-gcm", "vAAn&8kR:$iAE4", "github.com", 443, "", null, null, null, null)] // userinfo parsing
        [InlineData("ss://YWVzLTI1Ni1nY206d0xoTjJTVFpAZ2l0aHViLmNvbTo0NDM", false, "", "", "", 0, "", null, null, null, null)] // unsupported legacy URL
        [InlineData("ss://YWVzLTI1Ni1nY206d0xoTjJTVFpAZ2l0aHViLmNvbTo0NDM#some-legacy-url", false, "", "", "", 0, "", null, null, null, null)] // unsupported legacy URL with fragment
        [InlineData("https://github.com/", false, "", "", "", 0, "", null, null, null, null)] // non-Shadowsocks URL
        public void Server_TryParse(string ssUrl, bool expectedResult, string expectedMethod, string expectedPassword, string expectedHost, int expectedPort, string expectedFragment, string? expectedPluginName, string? expectedPluginVersion, string? expectedPluginOptions, string? expectedPluginArguments)
        {
            var result = Server.TryParse(ssUrl, out var server);

            Assert.Equal(expectedResult, result);
            if (result)
            {
                Assert.Equal(expectedPassword, server!.Password);
                Assert.Equal(expectedMethod, server.Method);
                Assert.Equal(expectedHost, server.Host);
                Assert.Equal(expectedPort, server.Port);
                Assert.Equal(expectedFragment, server.Name);
                Assert.Equal(expectedPluginName, server.PluginName);
                Assert.Equal(expectedPluginVersion, server.PluginVersion);
                Assert.Equal(expectedPluginOptions, server.PluginOptions);
                Assert.Equal(expectedPluginArguments, server.PluginArguments);
            }
        }
    }
}
