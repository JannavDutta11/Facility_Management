
using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Facility_Management.Converters
{
    
    public class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        
        private const string OutputFormat = "yyyy-MM-dd HH:mm";

       
        private static readonly string[] InputFormats =
        {
            "yyyy-MM-dd HH:mm",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-ddTHH:mm",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-dd",                
        };

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
           
            if (reader.TokenType == JsonTokenType.String)
            {
                var s = reader.GetString();
                if (string.IsNullOrWhiteSpace(s))
                    return default;

                if (DateTime.TryParseExact(
                        s,
                        InputFormats,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeLocal | DateTimeStyles.AllowWhiteSpaces,
                        out var parsed))
                {
                    return parsed;
                }

                if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out parsed))
                    return parsed;

                throw new JsonException($"Invalid date/time format: '{s}'. Allowed: {string.Join(", ", InputFormats)}");
            }

            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt64(out var ms))
            {
                return DateTimeOffset.FromUnixTimeMilliseconds(ms).LocalDateTime;
            }

            throw new JsonException($"Unexpected token parsing DateTime. Token: {reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
           
            writer.WriteStringValue(value.ToString(OutputFormat, CultureInfo.InvariantCulture));
        }
    }
}

