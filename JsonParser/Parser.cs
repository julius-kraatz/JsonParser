using System.Globalization;

namespace JsonParser
{
    internal class Value
    {
        private readonly string desc;
        private string? error;
        public string Desc { get { return desc; } }
        public Value(string desc, string? error = null)
        {
            this.desc = desc;
            this.error = error;
        }
    }
    internal class Member
    {
        private readonly string name;
        public string Name { get { return name; } }
        private readonly Value value;
        public Value Value { get { return value; } }
        public Member(string name, Value value)
        {
            this.name = name;
            this.value = value;
        }
    }
    internal class Element
    {
        private readonly Value value;
        public Value Value { get { return value; } }
        public Element(Value value)
        {
            this.value = value;
        }
    }

    internal class Object : Value
    {
        private readonly List<Member> members;
        public List<Member> Members { get { return members; } }

        public Object(List<Member> members) : base("object")
        {
            this.members = members;
        }
    }
    internal class Array : Value
    {
        private readonly List<Element> elements;
        public List<Element> Elements { get { return elements; } }

        public Array(List<Element> elements) : base("array")
        {
            this.elements = elements;
        }
    }

    internal class String : Value
    {
        private string content;
        public string Content { get { return content; } set { content = value; } }

        public String(string content, string? error = null) : base("string", error)
        {
            this.content = content;
        }
    }

    internal class Number : Value
    {
        private double content;
        public double Content { get { return content; } set { content = value; } }
        public Number(double content) : base("number")
        {
            this.content = content;
        }
    }

    internal class True : Value
    {
        public True() : base("true") { }
    }

    internal class False : Value
    {
        public False() : base("false") { }
    }

    internal class Null : Value
    {
        public Null() : base("null") { }
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
        public Value? Parse()
        {
            return ParseValue(tokens.Count - 1);
        }
        private Value? ParseValue(int endIndex)
        {
            Value? value = null;

            value = ParseObject(endIndex);
            if (value != null)
            {
                return value;
            }
            value = ParseArray(endIndex);
            if (value != null)
            {
                return value;
            }
            value = ParseString();
            if (value != null)
            {
                return value;
            }
            value = ParseNumber();
            if (value != null)
            {
                return value;
            }
            value = ParseTrue();
            if (value != null)
            {
                return value;
            }
            value = ParseFalse();
            if (value != null)
            {
                return value;
            }
            value = ParseNull();
            if (value != null)
            {
                return value;
            }
            return value;
        }
        private Object? ParseObject(int endIndex)
        {
            Object? result = null;
            if (tokenIndex >= endIndex)
            {
                return result;
            }
            if (!CheckToken(tokenIndex, Token.Description.Symbol, "{"))
            {
                return result;
            }
            tokenIndex++;
            if (!CheckToken(endIndex, Token.Description.Symbol, "}"))
            {
                return result;
            }
            result = new Object(ParseMembers(endIndex - 1));
            tokenIndex++;
            return result;
        }
        private List<Member> ParseMembers(int endIndex)
        {
            List<Member> members = new List<Member>();
            while (tokenIndex <= endIndex)
            {
                if (members.Count > 0)
                {
                    if(!CheckToken(tokenIndex, Token.Description.Symbol, ","))
                    {
                        break;
                    }
                    tokenIndex++;
                }

                Member? member = ParseMember(endIndex);
                if (member != null)
                {
                    members.Add(member);
                }
            }
            return members;
        }
        private Member? ParseMember(int endIndex)
        {
            Member? member = null;
            if (tokenIndex >= endIndex)
            {
                return member;
            }
            String? memberName = ParseString();
            if (memberName == null)
            {
                return member;
            }
            tokenIndex++;
            if (!CheckToken(tokenIndex, Token.Description.Symbol, ":"))
            {
                return member;
            }
            tokenIndex++;
            Value? memberValue = ParseValue(endIndex);
            if (memberValue == null)
            {
                return member;
            }
            tokenIndex++;
            return new Member(memberName.Content, memberValue);
        }
        private Array? ParseArray(int endIndex)
        {
            Array? result = null;
            if (tokenIndex >= endIndex)
            {
                return result;
            }
            if (!CheckToken(tokenIndex, Token.Description.Symbol, "["))
            {
                return result;
            }
            tokenIndex++;
            if (!CheckToken(endIndex, Token.Description.Symbol, "]"))
            {
                return result;
            }
            result = new Array(ParseElements(endIndex - 1));
            tokenIndex++;
            return result;
        }
        private List<Element> ParseElements(int endIndex)
        {
            List<Element> elements = new List<Element>();
            while (tokenIndex <= endIndex)
            {
                if (elements.Count > 0)
                {
                    if (!CheckToken(tokenIndex, Token.Description.Symbol, ","))
                    {
                        break;
                    }
                    tokenIndex++;
                }

                Value? elementValue = ParseValue(endIndex);
                if (elementValue != null)
                {
                    elements.Add(new Element(elementValue));
                }
            }
            return elements;
        }
        private string? getJsonStringError(string input)
        {
            return null;
        }
        private JsonParser.String? ParseString()
        {
            JsonParser.String? result = null;
            if (CheckToken(tokenIndex, Token.Description.StringLiteral))
            {
                string? error = getJsonStringError(tokens[tokenIndex].Content);
                result = new JsonParser.String(tokens[tokenIndex].Content, error);               
            }
            return result;
        }
        private Number? ParseNumber()
        {
            bool success = double.TryParse(tokens[tokenIndex].Content, NumberStyles.Any, CultureInfo.InvariantCulture, out double i);
            if (!success)
            {
                return null;
            }
            return new Number(i);
        }
        private True? ParseTrue()
        {
            True? result = null;
            if (CheckToken(tokenIndex, Token.Description.Sequence, "true"))
            {
                result = new True();
            }
            return result;
        }
        private False? ParseFalse()
        {
            False? result = null;
            if (CheckToken(tokenIndex, Token.Description.Sequence, "false"))
            {
                result = new False();
            }
            return result;
        }
        private Null? ParseNull()
        {
            Null? result = null;
            if (CheckToken(tokenIndex, Token.Description.Sequence, "null"))
            {
                result = new Null();
            }
            return result;
        }
        private bool CheckToken(int index, Token.Description desc, string? content = null)
        {
            bool result;
            if (content == null)
            {
                result = desc == tokens[index].Desc;
            }
            else
            {
                result = desc == tokens[index].Desc && content == tokens[index].Content;
            }
            return result;
        }
    }
}
