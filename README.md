# JsonParser
A JSON parser written in C#. It mainly consists of a Lexer and a Parser class. 

The Lexer converts the input file into a list of tokens.

The Parser converts the list of tokens into a JsonParser.Value, representing the top-level value of the JSON file.

It is possible to pass the input file directly to the Parser without having to call the Lexer manually first.
## Quick Example
Below is an example program that receives a file name via command line, passes its content to the Parser and receives the output value. For debug purposes, the built-in System.Text.Json functionality is used to serialize the output value to a readable syntax tree which is then written to the console.
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
There are three types of tokens: StringLiteral, Sequence and Symbol. Each of these is represented by a class inheriting from the Token class.
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
#### JsonParserSymbolNotFoundException
JsonParserSymbolNotFoundException is thrown whenever a specific symbol was expected at the current position, but it was not found. This can happen if e.g. two Members of an Object are not properly separated by comma.
#### JsonParserMemberNameNotFoundException
JsonParserMemberNameNotFoundException is thrown whenever the name of a Member (inside a JSON Object) could not be found.
#### JsonParserNoValueException
JsonParserNoValueException is thrown whenever no JsonParser.Value could be found at the current position and that is considered a problem. This can happen if e.g. there is a trailing comma in an Array.
#### JsonParserUnterminatedException
JsonParserUnterminatedException is thrown whenever an unterminated Object or Array is encountered, i.e. the end of the token list was reached before the closing bracket.


### System.Text.Json.Serializer Compatibility
The built-in System.Text.Json functionality can be used to serialize the output JsonParser.Value to a readable syntax tree. The following steps have been taken to ensure that the syntax tree is printed in the right way:
#### ValueOutputConverter
The ValueOutputConverter is a custom System.Text.Json.Serialization.JsonConverter. It makes sure that any JsonParser.Value is printed as object. This means that any properties of derived classes will be printed too, which is the intended outcome.
#### Property Order
The description of a JsonParser.Value ("Desc") has received the lowest JsonPropertyOrder to make sure that is is printed before any properties of derived classes. This helps with readability. 
