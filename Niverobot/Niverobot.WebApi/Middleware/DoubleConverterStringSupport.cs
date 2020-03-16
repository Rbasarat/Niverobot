using System;
using System.Buffers;
using System.Buffers.Text;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Niverobot.WebApi.Middleware
{
    public class DoubleConverterStringSupport : JsonConverter<double>
    {
        public override double Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String) return reader.GetDouble();

            var span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
            if (Utf8Parser.TryParse(span, out double number, out var bytesConsumed) && span.Length == bytesConsumed)
                return number;

            return reader.GetDouble();
        }

        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
