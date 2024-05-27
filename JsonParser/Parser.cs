using System.Globalization;

namespace JsonParser
{
    [Serializable]
    public class JsonParserException : Exception
    {
        public JsonParserException() : base() { }
        public JsonParserException(string message) : base(message) { }
        public JsonParserException(string message, Exception inner) : base(message, inner) { }
        public JsonParserException(int line, int column, string expected, string found)
            : this("Line " + line + ", Column " + column + ": Expected " + expected + ", found '" + found + "'") { }
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
            if (!CheckToken(Token.Description.Symbol, "{"))
            {
                return result;
            }
            List<Member> members = new List<Member>();
            bool foundEnd = false;
            while (true)
            {
                if (CheckToken(Token.Description.Symbol, "}"))
                {
                    foundEnd = true;
                    break;
                }
                if (members.Count > 0)
                {
                    if (!CheckToken(Token.Description.Symbol, ","))
                    {
                        ErrorExpectedToken(Token.Description.Symbol, ",");
                        break;
                    }
                }
                Member? member = ParseMember();
                if (member != null)
                {
                    members.Add(member);
                }
                else
                {
                    ErrorExpectedJsonValue();
                }
            }
            if (!foundEnd)
            {
                ErrorExpectedToken(Token.Description.Symbol, "}");
                return result;
            }
            result = new Object(members);
            return result;
        }
        private Member? ParseMember()
        {
            Member? member = null;
            String? memberName = ParseString();
            if (memberName == null)
            {
                ErrorExpectedToken(Token.Description.StringLiteral);
                return member;
            }
            if (!CheckToken(Token.Description.Symbol, ":"))
            {
                ErrorExpectedToken(Token.Description.Symbol, ":");
                return member;
            }
            Value? memberValue = ParseValue();
            if (memberValue == null)
            {
                return member;
            }
            return new Member(memberName.Content, memberValue);
        }
        private Array? ParseArray()
        {
            Array? result = null;
            if (!CheckToken(Token.Description.Symbol, "["))
            {
                return result;
            }
            List<Element> elements = new List<Element>();
            bool foundEnd = false;
            while (true)
            {
                if (CheckToken(Token.Description.Symbol, "]"))
                {
                    foundEnd = true;
                    break;
                }
                if (elements.Count > 0)
                {
                    if (!CheckToken(Token.Description.Symbol, ","))
                    {
                        ErrorExpectedToken(Token.Description.Symbol, ",");
                        break;
                    }
                }
                Value? elementValue = ParseValue();
                if (elementValue != null)
                {
                    elements.Add(new Element(elementValue));
                }
                else
                {
                    ErrorExpectedJsonValue();
                }
                    
            }
            if (!foundEnd)
            {
                ErrorExpectedToken(Token.Description.Symbol, "]");
                return result;
            }
            result = new Array(elements);
            return result;
        }
        private JsonParser.String? ParseString()
        {
            JsonParser.String? result = null;
            string content = tokens[tokenIndex].Content;
            if (CheckToken(Token.Description.StringLiteral))
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
                if(CheckToken(Token.Description.Sequence))
                {
                    result = new Number(i);
                }
            }
            return result;
        }
        private True? ParseTrue()
        {
            True? result = null;
            if (CheckToken(Token.Description.Sequence, "true"))
            {
                result = new True();
            }
            return result;
        }
        private False? ParseFalse()
        {
            False? result = null;
            if (CheckToken(Token.Description.Sequence, "false"))
            {
                result = new False();
            }
            return result;
        }
        private Null? ParseNull()
        {
            Null? result = null;
            if (CheckToken(Token.Description.Sequence, "null"))
            {
                result = new Null();
            }
            return result;
        }
        private bool CheckToken(Token.Description desc, string? content = null)
        {
            if(tokenIndex >= tokens.Count)
            {
                ErrorReachedEndOfTokens();
                return false;
            }
            bool result;
            if (content == null)
            {
                result = desc == tokens[tokenIndex].Desc;
            }
            else
            {
                result = desc == tokens[tokenIndex].Desc && content == tokens[tokenIndex].Content;
            }
            if(result)
            {
                tokenIndex++;
            }
            return result;
        }
        private void ErrorExpectedToken(Token.Description expectedTokenDesc, string expectedTokenContent = "")
        {
            throw new JsonParserException(tokens[tokenIndex].Line, tokens[tokenIndex].Column,
                (Enum.GetName(expectedTokenDesc) ?? "") + " " + expectedTokenContent, tokens[tokenIndex].Content);
        }
        private void ErrorExpectedJsonValue()
        {
            throw new JsonParserException(tokens[tokenIndex].Line, tokens[tokenIndex].Column, "a JSON value", tokens[tokenIndex].Content);
        }
        private void ErrorReachedEndOfTokens()
        {
            throw new JsonParserException("Unterminated object or array: reached end of file at Line " + 
                tokens[^1].Line + ", Column " + tokens[^1].Column);
        }
    }
}
