using JsonParser;

namespace JsonTests
{
    [TestClass]
    public class JsonTests
    {
        [TestMethod]
        public void ParserCanParseTrueValue()
        {
            List<Token> tokens = new Lexer("true").Tokenize();
            JsonParser.True? value = new Parser(tokens).Parse() as JsonParser.True;
            Assert.IsNotNull(value);
            Assert.AreEqual("true", value.Desc);
        }
        [TestMethod]
        public void ParserCanParseFalseValue()
        {
            List<Token> tokens = new Lexer("false").Tokenize();
            JsonParser.False? value = new Parser(tokens).Parse() as JsonParser.False; 
            Assert.IsNotNull(value);
            Assert.AreEqual("false", value.Desc);
        }
        [TestMethod]
        public void ParserCanParseNullValue()
        {
            List<Token> tokens = new Lexer("null").Tokenize();
            JsonParser.Null? value = new Parser(tokens).Parse() as JsonParser.Null;
            Assert.IsNotNull(value);
            Assert.AreEqual("null", value.Desc);
        }
        [TestMethod]
        public void ParserCanParseStringValue()
        {
            List<Token> tokens = new Lexer("\"Hello\"").Tokenize();
            JsonParser.String? value = new Parser(tokens).Parse() as JsonParser.String;
            Assert.IsNotNull(value);
            Assert.AreEqual("string", value.Desc);
            Assert.AreEqual("Hello", value.Content);
        }
        [TestMethod]
        public void ParserCanParseNumberValue()
        {
            List<Token> tokens = new Lexer("9").Tokenize();
            JsonParser.Number? value = new Parser(tokens).Parse() as JsonParser.Number;
            Assert.IsNotNull(value);
            Assert.AreEqual("number", value.Desc);
            Assert.AreEqual(9, value.Content);
        }
        [TestMethod]
        public void ParserCanParseNumberValueWithFraction()
        {
            List<Token> tokens = new Lexer("373.5939").Tokenize();
            JsonParser.Number? value = new Parser(tokens).Parse() as JsonParser.Number;
            Assert.IsNotNull(value);
            Assert.AreEqual("number", value.Desc);
            Assert.AreEqual(373.5939, value.Content);
        }
        [TestMethod]
        public void ParserCanParseNumberValueWithFraction2()
        {
            List<Token> tokens = new Lexer("27.4").Tokenize();
            JsonParser.Number? value = new Parser(tokens).Parse() as JsonParser.Number;
            Assert.IsNotNull(value);
            Assert.AreEqual("number", value.Desc);
            Assert.AreEqual(27.4, value.Content);
        }
        [TestMethod]
        public void ParserCanParseNumberValueWithMinusSign()
        {
            List<Token> tokens = new Lexer("-13745").Tokenize();
            JsonParser.Number? value = new Parser(tokens).Parse() as JsonParser.Number;
            Assert.IsNotNull(value);
            Assert.AreEqual("number", value.Desc);
            Assert.AreEqual(-13745, value.Content);
        }
        [TestMethod]
        public void ParserCanParseNumberValueWithPlusSign()
        {
            List<Token> tokens = new Lexer("+13745").Tokenize();
            JsonParser.Number? value = new Parser(tokens).Parse() as JsonParser.Number;
            Assert.IsNotNull(value);
            Assert.AreEqual("number", value.Desc);
            Assert.AreEqual(13745, value.Content);
        }
        [TestMethod]
        public void ParserCanParseNumberValueWithExponent()
        {
            List<Token> tokens = new Lexer("+13745e-003").Tokenize();
            JsonParser.Number? value = new Parser(tokens).Parse() as JsonParser.Number;
            Assert.IsNotNull(value);
            Assert.AreEqual("number", value.Desc);
            Assert.AreEqual(13.745, value.Content);
        }
        [TestMethod]
        public void ParserCanParseNumberValueWithFractionAndExponent()
        {
            List<Token> tokens = new Lexer("-65.323e00001").Tokenize();
            JsonParser.Number? value = new Parser(tokens).Parse() as JsonParser.Number;
            Assert.IsNotNull(value);
            Assert.AreEqual("number", value.Desc);
            Assert.AreEqual(-653.23, value.Content);
        }
        [TestMethod]
        public void ParserCanParseNumberValueWithFractionAndExponent2()
        {
            List<Token> tokens = new Lexer("375.3484E7").Tokenize();
            JsonParser.Number? value = new Parser(tokens).Parse() as JsonParser.Number;
            Assert.IsNotNull(value);
            Assert.AreEqual("number", value.Desc);
            Assert.AreEqual(3753484000, value.Content);
        }
        [TestMethod]
        public void ParserCannotParseNumberValueWithPrefix0x()
        {
            List<Token> tokens = new Lexer("0x456").Tokenize();
            JsonParser.Number? value = new Parser(tokens).Parse() as JsonParser.Number;
            Assert.IsNull(value);
        }
        [TestMethod]
        public void ParserCanParseEmptyObject()
        {
            List<Token> tokens = new Lexer("{}").Tokenize();
            JsonParser.Object? value = new Parser(tokens).Parse() as JsonParser.Object;
            Assert.IsNotNull(value);
            Assert.AreEqual("object", value.Desc);
        }
        [TestMethod]
        public void ParserCanParseEmptyArray()
        {
            List<Token> tokens = new Lexer("[]").Tokenize();
            JsonParser.Array? value = new Parser(tokens).Parse() as JsonParser.Array;
            Assert.IsNotNull(value);
            Assert.AreEqual("array", value.Desc);
        }
        [TestMethod]
        public void ParserCanParseObjectWithMember()
        {
            string content = File.ReadAllText("testfiles/testObjectWithMember.json");
            List<Token> tokens = new Lexer(content).Tokenize();
            JsonParser.Object? value = new Parser(tokens).Parse() as JsonParser.Object;
            Assert.IsNotNull(value);
            Assert.AreEqual("object", value.Desc);
            Assert.AreEqual(1, value.Members.Count);
            Assert.AreEqual("Test", value.Members[0].Name);

            JsonParser.Number? memberValue = value.Members[0].Value as JsonParser.Number;
            Assert.IsNotNull(memberValue);
            Assert.AreEqual("number", memberValue.Desc);
            Assert.AreEqual(9, memberValue.Content);
        }
        [TestMethod]
        public void ParserCanParseObjectWithMembers()
        {
            string content = File.ReadAllText("testfiles/testObjectWithMembers.json");
            List<Token> tokens = new Lexer(content).Tokenize();
            JsonParser.Object? value = new Parser(tokens).Parse() as JsonParser.Object;
            Assert.IsNotNull(value);
            Assert.AreEqual("object", value.Desc);
            Assert.AreEqual(3, value.Members.Count);
            Assert.AreEqual("Test", value.Members[0].Name);
            Assert.AreEqual("Name", value.Members[1].Name);
            Assert.AreEqual("Pets", value.Members[2].Name);

            JsonParser.Number? memberValue0 = value.Members[0].Value as JsonParser.Number;
            Assert.IsNotNull(memberValue0);
            Assert.AreEqual("number", memberValue0.Desc);
            Assert.AreEqual(9, memberValue0.Content);

            JsonParser.String? memberValue1 = value.Members[1].Value as JsonParser.String;
            Assert.IsNotNull(memberValue1);
            Assert.AreEqual("string", memberValue1.Desc);
            Assert.AreEqual("Frank", memberValue1.Content);

            JsonParser.Null? memberValue2 = value.Members[2].Value as JsonParser.Null;
            Assert.IsNotNull(memberValue2);
            Assert.AreEqual("null", memberValue2.Desc);
            
        }
        [TestMethod]
        public void ParserCanParseArrayWithElement()
        {
            string content = File.ReadAllText("testfiles/testArrayWithMember.json");
            List<Token> tokens = new Lexer(content).Tokenize();
            JsonParser.Array? value = new Parser(tokens).Parse() as JsonParser.Array;
            Assert.IsNotNull(value);
            Assert.AreEqual("array", value.Desc);
            Assert.AreEqual(1, value.Elements.Count);

            JsonParser.String? elementValue0 = value.Elements[0].Value as JsonParser.String;
            Assert.IsNotNull(elementValue0);
            Assert.AreEqual("string", elementValue0.Desc);
            Assert.AreEqual("Welcome", elementValue0.Content);
        }
        [TestMethod]
        public void ParserCanParseArrayWithElements()
        {
            string content = File.ReadAllText("testfiles/testArrayWithMembers.json");
            List<Token> tokens = new Lexer(content).Tokenize();
            JsonParser.Array? value = new Parser(tokens).Parse() as JsonParser.Array;
            Assert.IsNotNull(value);
            Assert.AreEqual("array", value.Desc);
            Assert.AreEqual(4, value.Elements.Count);

            JsonParser.Number? elementValue0 = value.Elements[0].Value as JsonParser.Number;
            Assert.IsNotNull(elementValue0);
            Assert.AreEqual("number", elementValue0.Desc);
            Assert.AreEqual(9.45, elementValue0.Content);

            JsonParser.Number? elementValue1 = value.Elements[1].Value as JsonParser.Number;
            Assert.IsNotNull(elementValue1);
            Assert.AreEqual("number", elementValue1.Desc);
            Assert.AreEqual(13, elementValue1.Content);

            JsonParser.Number? elementValue2 = value.Elements[2].Value as JsonParser.Number;
            Assert.IsNotNull(elementValue2);
            Assert.AreEqual("number", elementValue2.Desc);
            Assert.AreEqual(-67500, elementValue2.Content);

            JsonParser.Number? elementValue3 = value.Elements[3].Value as JsonParser.Number;
            Assert.IsNotNull(elementValue3);
            Assert.AreEqual("number", elementValue3.Desc);
            Assert.AreEqual(1000, elementValue3.Content);
        }
        [TestMethod]
        public void ParserCanParseNestedObjectWithObjectInside()
        {
            string content = File.ReadAllText("testfiles/testNestedObjectWithObject.json");
            List<Token> tokens = new Lexer(content).Tokenize();
            JsonParser.Object? value = new Parser(tokens).Parse() as JsonParser.Object;
            Assert.IsNotNull(value);
            Assert.AreEqual("object", value.Desc);
            Assert.AreEqual(2, value.Members.Count);
            Assert.AreEqual("Test", value.Members[0].Name);
            Assert.AreEqual("Object", value.Members[1].Name);

            JsonParser.Number? memberValue0 = value.Members[0].Value as JsonParser.Number;
            Assert.IsNotNull(memberValue0);
            Assert.AreEqual("number", memberValue0.Desc);
            Assert.AreEqual(9, memberValue0.Content);

            JsonParser.Object? nestedObjectValue = value.Members[1].Value as JsonParser.Object;
            Assert.IsNotNull(nestedObjectValue);
            Assert.AreEqual("object", nestedObjectValue.Desc);
            Assert.AreEqual(2, nestedObjectValue.Members.Count);
            Assert.AreEqual("Name", nestedObjectValue.Members[0].Name);
            Assert.AreEqual("IsArtist", nestedObjectValue.Members[1].Name);

            JsonParser.String? nestedObjectValue0 = nestedObjectValue.Members[0].Value as JsonParser.String;
            Assert.IsNotNull (nestedObjectValue0);
            Assert.AreEqual ("string", nestedObjectValue0.Desc);
            Assert.AreEqual("Leonardo", nestedObjectValue0.Content);
        }
        [TestMethod]
        public void ParserCanParseNestedObjectWithArrayInside()
        {
            string content = File.ReadAllText("testfiles/testNestedObjectWithArray.json");
            List<Token> tokens = new Lexer(content).Tokenize();
            JsonParser.Object? value = new Parser(tokens).Parse() as JsonParser.Object;
            Assert.IsNotNull(value);
            Assert.AreEqual("object", value.Desc);
            Assert.AreEqual(2, value.Members.Count);
            Assert.AreEqual("Test", value.Members[0].Name);
            Assert.AreEqual("Array", value.Members[1].Name);

            JsonParser.Number? memberValue0 = value.Members[0].Value as JsonParser.Number;
            Assert.IsNotNull(memberValue0);
            Assert.AreEqual("number", memberValue0.Desc);
            Assert.AreEqual(9, memberValue0.Content);

            JsonParser.Array? nestedArrayValue = value.Members[1].Value as JsonParser.Array;
            Assert.IsNotNull(nestedArrayValue);
            Assert.AreEqual("array", nestedArrayValue.Desc);
            Assert.AreEqual(4, nestedArrayValue.Elements.Count);

            JsonParser.Number? elementValue0 = nestedArrayValue.Elements[0].Value as JsonParser.Number;
            Assert.IsNotNull(elementValue0);
            Assert.AreEqual("number", elementValue0.Desc);
            Assert.AreEqual(9.45, elementValue0.Content);

            JsonParser.Number? elementValue1 = nestedArrayValue.Elements[1].Value as JsonParser.Number;
            Assert.IsNotNull(elementValue1);
            Assert.AreEqual("number", elementValue1.Desc);
            Assert.AreEqual(13, elementValue1.Content);

            JsonParser.Number? elementValue2 = nestedArrayValue.Elements[2].Value as JsonParser.Number;
            Assert.IsNotNull(elementValue2);
            Assert.AreEqual("number", elementValue2.Desc);
            Assert.AreEqual(-67500, elementValue2.Content);

            JsonParser.Number? elementValue3 = nestedArrayValue.Elements[3].Value as JsonParser.Number;
            Assert.IsNotNull(elementValue3);
            Assert.AreEqual("number", elementValue3.Desc);
            Assert.AreEqual(1000, elementValue3.Content);
        }
        [TestMethod]
        public void ParserCanParseNestedArrayWithArrayInside()
        {
            string content = File.ReadAllText("testfiles/testNestedArrayWithArray.json");
            List<Token> tokens = new Lexer(content).Tokenize();
            JsonParser.Array? value = new Parser(tokens).Parse() as JsonParser.Array;
            Assert.IsNotNull(value);
            Assert.AreEqual("array", value.Desc);
            Assert.AreEqual(3, value.Elements.Count);

            JsonParser.Number? elementValue0 = value.Elements[0].Value as JsonParser.Number;
            Assert.IsNotNull(elementValue0);
            Assert.AreEqual("number", elementValue0.Desc);
            Assert.AreEqual(9.45, elementValue0.Content);

            JsonParser.Array? nestedArrayValue = value.Elements[1].Value as JsonParser.Array;
            Assert.IsNotNull(nestedArrayValue);
            Assert.AreEqual("array", nestedArrayValue.Desc);
            Assert.AreEqual(3, nestedArrayValue.Elements.Count);

            JsonParser.Number? elementValue2 = value.Elements[2].Value as JsonParser.Number;
            Assert.IsNotNull(elementValue2);
            Assert.AreEqual("number", elementValue2.Desc);
            Assert.AreEqual(13.34, elementValue2.Content);

            JsonParser.Number? nestedArrayElementValue0 = nestedArrayValue.Elements[0].Value as JsonParser.Number;
            Assert.IsNotNull(nestedArrayElementValue0);
            Assert.AreEqual("number", nestedArrayElementValue0.Desc);
            Assert.AreEqual(3, nestedArrayElementValue0.Content);

            JsonParser.Number? nestedArrayElementValue1 = nestedArrayValue.Elements[1].Value as JsonParser.Number;
            Assert.IsNotNull(nestedArrayElementValue1);
            Assert.AreEqual("number", nestedArrayElementValue1.Desc);
            Assert.AreEqual(4, nestedArrayElementValue1.Content);

            JsonParser.Number? nestedArrayElementValue2 = nestedArrayValue.Elements[2].Value as JsonParser.Number;
            Assert.IsNotNull(nestedArrayElementValue2);
            Assert.AreEqual("number", nestedArrayElementValue2.Desc);
            Assert.AreEqual(5, nestedArrayElementValue2.Content);
        }
        [TestMethod]
        public void ParserCanParseNestedArrayWithObjectInside()
        {
            string content = File.ReadAllText("testfiles/testNestedArrayWithObject.json");
            List<Token> tokens = new Lexer(content).Tokenize();
            JsonParser.Array? value = new Parser(tokens).Parse() as JsonParser.Array;
            Assert.IsNotNull(value);
            Assert.AreEqual("array", value.Desc);
            Assert.AreEqual(2, value.Elements.Count);

            JsonParser.Number? elementValue0 = value.Elements[0].Value as JsonParser.Number;
            Assert.IsNotNull(elementValue0);
            Assert.AreEqual("number", elementValue0.Desc);
            Assert.AreEqual(9.45, elementValue0.Content);

            JsonParser.Object? nestedObjectValue = value.Elements[1].Value as JsonParser.Object;
            Assert.IsNotNull(nestedObjectValue);
            Assert.AreEqual("object", nestedObjectValue.Desc);
            Assert.AreEqual(2, nestedObjectValue.Members.Count);
            Assert.AreEqual("Number", nestedObjectValue.Members[0].Name);
            Assert.AreEqual("Name", nestedObjectValue.Members[1].Name);

            JsonParser.Number? nestedObjectMemberValue0 = nestedObjectValue.Members[0].Value as JsonParser.Number;
            Assert.IsNotNull(nestedObjectMemberValue0);
            Assert.AreEqual("number", nestedObjectMemberValue0.Desc);
            Assert.AreEqual(9, nestedObjectMemberValue0.Content);

            JsonParser.String? nestedObjectMemberValue1 = nestedObjectValue.Members[1].Value as JsonParser.String;
            Assert.IsNotNull(nestedObjectMemberValue1);
            Assert.AreEqual("string", nestedObjectMemberValue1.Desc);
            Assert.AreEqual("Donald", nestedObjectMemberValue1.Content);
        }
        [TestMethod]
        public void LexerReturnsExpectedResultWithStandardInput()
        {
            string content = File.ReadAllText("testfiles/completeTest.json");
            List<Token> actual = new Lexer(content).Tokenize();

            List<Token> expected = [
                new (Token.Description.Symbol, "{"),
                new (Token.Description.StringLiteral, "first_name"),
                new (Token.Description.Symbol, ":"),
                new (Token.Description.StringLiteral, "John"),
                new(Token.Description.Symbol, ","),
                new (Token.Description.StringLiteral, "is_alive"),
                new(Token.Description.Symbol, ":"),
                new(Token.Description.Sequence, "true"),
                new(Token.Description.Symbol, ","),
                new(Token.Description.StringLiteral, "is_stupid"),
                new(Token.Description.Symbol, ":"),
                new(Token.Description.Sequence, "false"),
                new(Token.Description.Symbol, ","),
                new(Token.Description.StringLiteral, "age"),
                new(Token.Description.Symbol, ":"),
                new(Token.Description.Sequence, "27.4"),
                new(Token.Description.Symbol, ","),
                new(Token.Description.StringLiteral, "spouse"),
                new(Token.Description.Symbol, ":"),
                new(Token.Description.Sequence, "null"),
                new(Token.Description.Symbol, ","),
                new(Token.Description.StringLiteral, "address"),
                new(Token.Description.Symbol, ":"),
                new(Token.Description.Symbol, "{"),
                new(Token.Description.StringLiteral, "city"),
                new(Token.Description.Symbol, ":"),
                new(Token.Description.StringLiteral, "New York"),
                new(Token.Description.Symbol, ","),
                new(Token.Description.StringLiteral, "state"),
                new(Token.Description.Symbol, ":"),
                new(Token.Description.StringLiteral, "NY"),
                new(Token.Description.Symbol, ","),
                new(Token.Description.StringLiteral, "postal_code"),
                new(Token.Description.Symbol, ":"),
                new(Token.Description.StringLiteral, "645657z"),
                new(Token.Description.Symbol, "}"),
                new(Token.Description.Symbol, ","),
                new(Token.Description.StringLiteral, "phone_numbers"),
                new(Token.Description.Symbol, ":"),
                new(Token.Description.Symbol, "["),
                new(Token.Description.Symbol, "{"),
                new(Token.Description.StringLiteral, "type"),
                new(Token.Description.Symbol, ":"),
                new(Token.Description.StringLiteral, "home"),
                new(Token.Description.Symbol, ","),
                new(Token.Description.StringLiteral, "number"),
                new(Token.Description.Symbol, ":"),
                new(Token.Description.StringLiteral, "1337-001"),
                new(Token.Description.Symbol, "}"),
                new(Token.Description.Symbol, ","),
                new(Token.Description.Symbol, "{"),
                new(Token.Description.StringLiteral, "type"),
                new(Token.Description.Symbol, ":"),
                new(Token.Description.StringLiteral, "office"),
                new(Token.Description.Symbol, ","),
                new(Token.Description.StringLiteral, "number"),
                new(Token.Description.Symbol, ":"),
                new(Token.Description.Sequence, "null"),
                new(Token.Description.Symbol, "}"),
                new(Token.Description.Symbol, "]"),
                new(Token.Description.Symbol, ","),
                new(Token.Description.StringLiteral, "children"),
                new(Token.Description.Symbol, ":"),
                new(Token.Description.Symbol, "["),
                new(Token.Description.StringLiteral, "Catherine"),
                new(Token.Description.Symbol, ","),
                new(Token.Description.StringLiteral, "Thomas"),
                new(Token.Description.Symbol, ","),
                new(Token.Description.StringLiteral, "Trevor"),
                new(Token.Description.Symbol, "]"),
                new(Token.Description.Symbol, "}")
            ];

            try
            {
                Assert.IsTrue(expected.Count == actual.Count);
            }
            catch(AssertFailedException)
            {
                Assert.Fail("expected Count is " + expected.Count + ", but actual Count is " + actual.Count + "!");
            }

            int i = 0;
            try
            {
                for (i = 0; i < expected.Count; i++)
                {
                    Assert.IsTrue(expected[i].Equals(actual[i]));
                }
            }
            catch (AssertFailedException)
            { 
                Assert.Fail("Unexpected token at index " + i + ": expected " + expected[i].Desc + " "
                    + expected[i].Content + ", actual " + actual[i].Desc + " " + actual[i].Content);
            }

        }
    }
}