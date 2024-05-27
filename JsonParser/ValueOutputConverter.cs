using System.Text.Json.Serialization;
using System.Text.Json;

namespace JsonParser
{
    internal class ValueOutputConverter : JsonConverter<Value>
    {
        public override Value? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
        public override void Write(Utf8JsonWriter writer, Value value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize<object?>(writer, value, options);
        }
    }
}
