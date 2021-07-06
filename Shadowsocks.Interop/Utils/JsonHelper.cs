using Shadowsocks.Utils;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shadowsocks.Interop.Utils
{
    public static class JsonHelper
    {
        public static readonly JsonSerializerOptions camelCaseJsonSerializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
        };

        public static readonly JsonSerializerOptions snakeCaseJsonSerializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy(),
            WriteIndented = true,
        };

        public static readonly JsonSerializerOptions camelCaseJsonDeserializerOptions = new()
        {
            AllowTrailingCommas = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReadCommentHandling = JsonCommentHandling.Skip,
            WriteIndented = true,
        };

        public static readonly JsonSerializerOptions snakeCaseJsonDeserializerOptions = new()
        {
            AllowTrailingCommas = true,
            PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy(),
            ReadCommentHandling = JsonCommentHandling.Skip,
            WriteIndented = true,
        };
    }
}
