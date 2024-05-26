using System.Text.Json.Serialization;

namespace JsonParser
{
    internal class Token(Token.Description desc, string content, int line = 0, int column = 0)
    {
        [JsonConverter(typeof(JsonStringEnumConverter<Description>))]
        public enum Description
        {
            StringLiteral,
            NumberLiteral,
            Sequence,
            Symbol
        }

        private readonly Description desc = desc;
        private readonly string content = content;
        private readonly int line = line;
        private readonly int column = column;
        public Description Desc { get { return desc; } }
        public string Content { get { return content; } }
        public int Line { get { return line; } }
        public int Column { get { return column; } }
        public override bool Equals(object? obj)
        {
            return obj is Token other && other.Desc == Desc && other.Content == content;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Desc, Content, Line, Column);
        }
    }
}
