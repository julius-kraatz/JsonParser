# JsonParser
A JSON parser written in C#.
## Quick Example
Below is an example program that receives a file name via command line, passes its content to the Parser and receives the output value. For debug purposes, the built-in System.Text.Json functions are used to serialize the output value to a readable syntax tree which is then written to the console.
```
using System.Text.Encodings.Web;
using System.Text.Json;
using JsonParser;

namespace JsonParserTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string content = File.ReadAllText(args[0]);
            Value? value = new Parser(content).Parse();
            JsonSerializerOptions options = new() { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            Console.WriteLine(JsonSerializer.Serialize(value, options));
        }
    }
}
```
## Lexer
The Lexer takes in the content of a JSON file as a string. The Lexer contains a Tokenize method that converts the string into a list of tokens.
A Token can have one of the following descriptions: StringLiteral, Sequence or Symbol.
### StringLiteral
The Lexer interprets any text enclosed in quotation marks as a StringLiteral.
### Sequence
A Sequence is not enclosed in quotation marks and may consist of so-called "sequence characters": these are alphanumeric characters as well as the dot, plus or minus sign.
Whenever a non-sequence character is encountered, the current Sequence is terminated.
(Underscores are considered sequence characters too, which is irrelevant for JSON.)
### Symbol
A Symbol may consist of a single character that is not part of a StringLiteral, not part of a Sequence, not a control character and not a space character. This type of Token is used to register structural characters such as brackets, colons and commas.
### How to use
Below is an example program that receives a file name via command line and passes its content to the Lexer:
```
using JsonParser;
namespace JsonParserTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string content = File.ReadAllText(args[0]);
            List<Token> tokens = new Lexer(content).Tokenize();
        }
    }
}
```

## Parser
The Parser takes in a list of tokens. It contains a Parse method that converts the list of tokens into a JsonParser.Value, or null if no value could be found.
It is possible to pass a string (i.e. the content of the JSON file) to the Parser constructor. The Parser will then pass on the string to the Lexer and use the resulting list of tokens.
### JsonParser.Value
There are seven classes inheriting from JsonParser.Value, representing the seven different types of JSON values: Object, Array, String, Number, True, False and Null.
### Objects/Members and Arrays/Elements
In order to represent the JSON value hierarchy, any Object contains a List of Members and any Array contains a List of Elements.
The class 'Member' and the class 'Element' do not inherit from JsonParser.Value, instead they contain an instance of JsonParser.Value as property. The class 'Member' also contains the Name string as property.
### How to use
Below is an example program that receives a file name via command line and passes its content to the Parser:
```
using JsonParser;
namespace JsonParserTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string content = File.ReadAllText(args[0]);
            Value? value = new Parser(content).Parse();
        }
    }
}
```
### Exceptions
JsonParserException is the base exception class of the Parser. It is never thrown directly. The following classes are derived from it:
#### JsonParserWrongTokenException
JsonParserWrongTokenException is thrown whenever a specific token was expected at the current position, but it was not found. This can happen if e.g. two Members of an Object are not properly separated by comma.
#### JsonParserNoValueException
JsonParserNoValueException is thrown whenever no JsonParser.Value could be found at the current position and that is considered a problem. This can happen if e.g. there is a trailing comma in an Array. If there is sufficient information (i.e. there was a specific token expected), then JsonParserWrongTokenException is thrown instead.
#### JsonParserUnterminatedException
JsonParserUnterminatedException is thrown whenever an unterminated Object or Array is encountered, i.e. the end of the token list was reached before the closing bracket.



