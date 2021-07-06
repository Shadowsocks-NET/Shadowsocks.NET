using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shadowsocks.Utils
{
    public class DateTimeOffsetUnixTimeSecondsConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64());

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
            => writer.WriteNumberValue(value.ToUnixTimeSeconds());
    }
}
