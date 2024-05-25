﻿using System.Globalization;

namespace JsonParser
{
    [Serializable]
    public class JsonParserException : Exception
    {
        public JsonParserException() : base() { }
        public JsonParserException(string message) : base(message) { }
        public JsonParserException(string message, Exception inner) : base(message, inner) { }
    }
    [Serializable]
    public class JsonParserEofException : Exception
    {
        public JsonParserEofException() : base() { }
        public JsonParserEofException(string message) : base(message) { }
        public JsonParserEofException(string message, Exception inner) : base(message, inner) { }
    }
    internal class Value
    {
        private readonly string desc;
        public string Desc { get { return desc; } }
        public Value(string desc)
        {
            this.desc = desc;
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

        public String(string content) : base("string")
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
        private readonly int endIndex;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
            tokenIndex = 0;
            endIndex = tokens.Count;
        }
        public Parser(string content) : this(new Lexer(content).Tokenize())
        {
        }
        public Value? Parse()
        {
            Value? value = null;
            try
            {
                value = ParseValue();
            }
            catch (JsonParserEofException)
            {
                Console.WriteLine("Reached end of file!");
            }
            return value;
        }
        private Value? ParseValue()
        {
            Value? value = null;

            value = ParseObject();
            if (value != null)
            {
                return value;
            }
            value = ParseArray();
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
                        Error(Token.Description.Symbol, ",");
                        break;
                    }
                }
                Member? member = ParseMember();
                if (member != null)
                {
                    members.Add(member);
                }
            }
            if (!foundEnd)
            {
                Error(Token.Description.Symbol, "}");
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
                Error(Token.Description.StringLiteral);
                return member;
            }
            if (!CheckToken(Token.Description.Symbol, ":"))
            {
                Error(Token.Description.Symbol, ":");
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
                        Error(Token.Description.Symbol, ",");
                        break;
                    }
                }
                Value? elementValue = ParseValue();
                if (elementValue != null)
                {
                    elements.Add(new Element(elementValue));
                }
            }
            if (!foundEnd)
            {
                Error(Token.Description.Symbol, "]");
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
                throw new JsonParserEofException();
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
        private void Error(Token.Description expectedTokenDesc, string expectedTokenContent = "")
        {
            throw new JsonParserException("Line " + tokens[tokenIndex].Line + ", Column " + tokens[tokenIndex].Column + ": Expected "
                + Enum.GetName(expectedTokenDesc) + " " + expectedTokenContent);
        }
    }
}