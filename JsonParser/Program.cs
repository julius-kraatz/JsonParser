using System.Text.Encodings.Web;
using System.Text.Json;

namespace JsonParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            args = new[] {"test.json" };
#endif
            string content = File.ReadAllText(args[0]);
            Console.WriteLine(content);
            JsonSerializerOptions options = new() { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            List<Token> tokens = new Lexer(content).Tokenize();
            Console.WriteLine(JsonSerializer.Serialize(tokens, options));

            Value? value = new Parser(tokens).Parse();
            Console.WriteLine(JsonSerializer.Serialize<object?>(value, options));
        }
    }
}
