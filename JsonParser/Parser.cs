using System.Globalization;

namespace JsonParser
{
    [Serializable]
    internal class JsonParserException : Exception
    {
        public JsonParserException(string message) : base(message) { }
        public JsonParserException(int line, int column, string expected, string found)
            : this("Line " + line + ", Column " + column + ": Expected " + expected + ", found '" + found + "'") { }
    }
    internal class JsonParserSymbolNotFoundException : JsonParserException
    {
        public JsonParserSymbolNotFoundException(Token currentToken, string expectedSymbolContent)
            : base(currentToken.Line, currentToken.Column,
                "Symbol " + expectedSymbolContent, currentToken.Content) { }
    }
    internal class JsonParserMemberNameNotFoundException : JsonParserException
    {
        public JsonParserMemberNameNotFoundException(Token currentToken)
            : base(currentToken.Line, currentToken.Column, "StringLiteral", currentToken.Content) { }
    }
    internal class JsonParserNoValueException : JsonParserException
    {
        public JsonParserNoValueException(Token currentToken)
            : base(currentToken.Line, currentToken.Column, "a JSON value", currentToken.Content) { }
    }
    internal class JsonParserUnterminatedException : JsonParserException
    {
        public JsonParserUnterminatedException(List<Token> tokens)
            : base("Unterminated object or array: Line " + tokens[^1].Line + ", Column " + tokens[^1].Column) { }
    }
    internal class Parser
    {
        private readonly List<Token> tokens;
        private int tokenIndex;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
            tokenIndex = 0;
        }
        public Parser(string content) : this(new Lexer(content).Tokenize())
        {
        }
        public Value? Parse()
        {
            return ParseValue();
        }
        private Value? ParseValue()
        {
            Value? value = ParseObject();
            value ??= ParseArray();
            value ??= ParseNumber();
            value ??= ParseString();
            value ??= ParseTrue();
            value ??= ParseFalse();
            value ??= ParseNull();
            return value;
        }
        private Object? ParseObject()
        {
            Object? result = null;
            if (!CheckToken<Symbol>("{"))
            {
                return result;
            }
            List<Member> members = new List<Member>();
            while (true)
            {
                if (CheckToken<Symbol>("}"))
                {
                    break;
                }
                if (members.Count > 0)
                {
                    if (!CheckToken<Symbol>(","))
                    {
                        throw new JsonParserSymbolNotFoundException(tokens[tokenIndex], ",");
                    }
                }
                members.Add(ParseMember());
            }
            result = new Object(members);
            return result;
        }
        private Member ParseMember()
        {
            String? memberName = ParseString();
            if (memberName == null)
            {
                throw new JsonParserMemberNameNotFoundException(tokens[tokenIndex]);
            }
            if (!CheckToken<Symbol>(":"))
            {
                throw new JsonParserSymbolNotFoundException(tokens[tokenIndex], ":");
            }
            Value? memberValue = ParseValue();
            if (memberValue == null)
            {
                throw new JsonParserNoValueException(tokens[tokenIndex]);
            }
            return new Member(memberName.Content, memberValue);
        }
        private Array? ParseArray()
        {
            Array? result = null;
            if (!CheckToken<Symbol>("["))
            {
                return result;
            }
            List<Element> elements = new List<Element>();
            while (true)
            {
                if (CheckToken<Symbol>("]"))
                {
                    break;
                }
                if (elements.Count > 0)
                {
                    if (!CheckToken<Symbol>(","))
                    {
                        throw new JsonParserSymbolNotFoundException(tokens[tokenIndex], ",");
                    }
                }
                elements.Add(ParseElement());              
            }
            result = new Array(elements);
            return result;
        }
        private Element ParseElement()
        {
            Value? elementValue = ParseValue();
            if (elementValue == null)
            {
                throw new JsonParserNoValueException(tokens[tokenIndex]);
            }
            return new Element(elementValue);
        }
        private JsonParser.String? ParseString()
        {
            JsonParser.String? result = null;
            string content = tokens[tokenIndex].Content;
            if (CheckToken<StringLiteral>())
            {
                result = new JsonParser.String(content);               
            }
            return result;
        }
        private Number? ParseNumber()
        {
            Number? result = null;
            bool success = double.TryParse(tokens[tokenIndex].Content, NumberStyles.Any, CultureInfo.InvariantCulture, out double i);
            if (success)
            {
                if(CheckToken<Sequence>())
                {
                    result = new Number(i);
                }
            }
            return result;
        }
        private True? ParseTrue()
        {
            True? result = null;
            if (CheckToken<Sequence>("true"))
            {
                result = new True();
            }
            return result;
        }
        private False? ParseFalse()
        {
            False? result = null;
            if (CheckToken<Sequence>("false"))
            {
                result = new False();
            }
            return result;
        }
        private Null? ParseNull()
        {
            Null? result = null;
            if (CheckToken<Sequence>("null"))
            {
                result = new Null();
            }
            return result;
        }
        private bool CheckToken<T>(string? content = null)
        {
            if (tokenIndex >= tokens.Count)
            {
                throw new JsonParserUnterminatedException(tokens);
            }
            bool result = tokens[tokenIndex].CheckToken<T>(content);
            if (result)
            {
                tokenIndex++;
            }
            return result;
        }
    }
}
