
namespace JsonParser
{
    internal class Token(string desc, string content, int line = 0, int column = 0)
    {
        private readonly string desc = desc;
        private readonly string content = content;
        private readonly int line = line;
        private readonly int column = column;
        public string Desc { get { return desc; } }
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

        public bool CheckToken<T>(string? contentToCompare = null)
        {
            bool result;
            if (contentToCompare == null)
            {
                result = typeof(T) == GetType();
            }
            else
            {
                result = typeof(T) == GetType() && content == contentToCompare;
            }
            return result;
        }
    }

    internal class StringLiteral : Token
    {
        public StringLiteral(string content, int line = 0, int column = 0) : base("StringLiteral", content, line, column)
        {
        }
    }
    internal class Sequence : Token
    {
        public Sequence(string content, int line = 0, int column = 0) : base("Sequence", content, line, column)
        {
        }
    }
    internal class Symbol : Token
    {
        public Symbol(string content, int line = 0, int column = 0) : base("Symbol", content, line, column)
        {
        }
    }
}
