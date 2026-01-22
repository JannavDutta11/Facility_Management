
using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Facility_Management.Converters
{
    /// <summary>
    /// Global JSON converter for DateTime to allow human-friendly input formats
    /// and enforce a consistent output format.
    /// </summary>
    public class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        // Output format used for all DateTime in API responses
        private const string OutputFormat = "yyyy-MM-dd HH:mm";

        // Accepted input formats for requests (add/remove as needed)
        private static readonly string[] InputFormats =
        {
            "yyyy-MM-dd HH:mm",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-ddTHH:mm",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-dd",                 // date-only
            // Optional extras (uncomment if you want to accept these too):
            // "dd-MM-yyyy HH:mm",
            // "dd/MM/yyyy HH:mm",
            // "MM/dd/yyyy HH:mm",
        };

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Only handle string values (and fail fast for others)
            if (reader.TokenType == JsonTokenType.String)
            {
                var s = reader.GetString();
                if (string.IsNullOrWhiteSpace(s))
                    return default;

                // Try exact formats first
                if (DateTime.TryParseExact(
                        s,
                        InputFormats,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeLocal | DateTimeStyles.AllowWhiteSpaces,
                        out var parsed))
                {
                    return parsed;
                }

                // Fallback general parse with InvariantCulture
                if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out parsed))
                    return parsed;

                throw new JsonException($"Invalid date/time format: '{s}'. Allowed: {string.Join(", ", InputFormats)}");
            }

            // Optional: support numeric Unix epoch milliseconds
            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt64(out var ms))
            {
                return DateTimeOffset.FromUnixTimeMilliseconds(ms).LocalDateTime;
            }

            throw new JsonException($"Unexpected token parsing DateTime. Token: {reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // Always output in a consistent, readable format
            writer.WriteStringValue(value.ToString(OutputFormat, CultureInfo.InvariantCulture));
        }
    }
}

