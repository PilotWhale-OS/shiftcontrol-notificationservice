using System;
using Newtonsoft.Json;

namespace NotificationService.Converters
{
    public class UnixTimestampConverter : JsonConverter<DateTimeOffset?>
    {
        public override void WriteJson(JsonWriter writer, DateTimeOffset? value, JsonSerializer serializer)
        {
            if (value.HasValue)
            {
                writer.WriteValue(value.Value.ToUnixTimeSeconds());
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override DateTimeOffset? ReadJson(JsonReader reader, Type objectType, DateTimeOffset? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonToken.Float || reader.TokenType == JsonToken.Integer)
            {
                var value = Convert.ToDouble(reader.Value);
                return DateTimeOffset.FromUnixTimeMilliseconds((long)(value * 1000));
            }

            throw new JsonSerializationException($"Unexpected token type: {reader.TokenType}");
        }
    }
}

