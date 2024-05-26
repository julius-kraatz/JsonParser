using System.Text.Json.Serialization;

namespace JsonParser
{
    [JsonConverter(typeof(ValueOutputConverter))]
    internal class Value
    {
        private readonly string desc;
        [JsonPropertyOrder(-1)]
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
}
