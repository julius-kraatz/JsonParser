namespace JsonParser
{
    internal class Lexer
    {
        private const char TAB = (char)0x09;
        private const char NEWLINE = '\n';
        private enum SequenceKind
        {
            None,
            Generic
        }

        private int i;
        private int quotationMarkIndex;
        private readonly string content;
        private int sequenceIndex;
        private SequenceKind sequenceKind;
        private int tokenLine;
        private int tokenColumn;
        private readonly List<Token> result;

        private int line;
        private int column;

        public Lexer(string content)
        {
            this.content = content;
            i = -1;
            quotationMarkIndex = -1;
            sequenceIndex = -1;
            sequenceKind = SequenceKind.None;
            result = new List<Token>();
            line = 1;
            column = 0;
        }
        public List<Token> Tokenize()
        {
            for (i = 0; i < content.Length; i++)
            {
                if (content[i] == NEWLINE)
                {
                    line++;
                    column = 0;
                }
                else if (content[i] == TAB)
                {
                    column += 4;
                }
                else
                {
                    column++;
                }

                if (IsUnescapedQuotationMark())
                {
                    if (IsQuotationMarkOpen())
                    {
                        RegisterLiteralString();
                    }
                    else
                    {
                        RegisterQuotationMarkOpen();
                    }
                    continue;
                }

                if (IsQuotationMarkOpen()) { continue; }

                if (IsSequenceChar())
                {
                    SwitchSequence(SequenceKind.Generic);
                }
                else
                {
                    SwitchSequence(SequenceKind.None);
                    if (!char.IsControl(content[i]) && content[i] != ' ')
                    {
                        AddSymbol();
                    }
                }
            }
            RegisterCurrentSequence();
            return result;
        }

        private bool HasCharRepeated(char ch)
        {
            return i > 0 && content[i] == ch && content[i - 1] == ch;
        }
        private bool WillCharRepeat(char ch)
        {
            return content[i] == ch && i + 1 < content.Length && content[i + 1] == ch;
        }
        private bool IsNotEscapedChar(char ch)
        {
            return i > 0 ? content[i] == ch && content[i - 1] != '\\' : content[i] == ch;
        }
        private bool IsUnescapedQuotationMark()
        {
            return IsNotEscapedChar('\"');
        }
        private bool IsQuotationMarkOpen()
        {
            return quotationMarkIndex > -1;
        }
        private void AddSymbol()
        {
            result.Add(new Symbol(content[i].ToString(), line, column));
        }
        private void RegisterQuotationMarkOpen()
        {
            quotationMarkIndex = i;
            tokenColumn = column;
            tokenLine = line;
        }
        private void RegisterLiteralString()
        {
            string substring = content.Substring(quotationMarkIndex + 1, i - (quotationMarkIndex + 1));
            result.Add(new StringLiteral(substring, tokenLine, tokenColumn));
            quotationMarkIndex = -1;
        }
        private void RegisterSequenceStart(SequenceKind kind)
        {
            if (kind != SequenceKind.None)
            {
                sequenceIndex = i;
                sequenceKind = kind;
                tokenColumn = column;
                tokenLine = line;
            }
        }
        private void RegisterCurrentSequence()
        {
            if (sequenceKind != SequenceKind.None)
            {
                string sequence = content.Substring(sequenceIndex, i - sequenceIndex);
                result.Add(new Sequence(sequence, tokenLine, tokenColumn));
                sequenceIndex = -1;
                sequenceKind = SequenceKind.None;
            }
        }
        private void SwitchSequence(SequenceKind kind)
        {
            if (sequenceKind != kind)
            {
                RegisterCurrentSequence();
                RegisterSequenceStart(kind);
            }
        }
        private bool IsSequenceChar()
        {
            return content[i] == '_' || content[i] == '.' || content[i] == '+' || content[i] == '-' ||
                char.IsBetween(content[i], '0', '9') || char.IsBetween(content[i], 'A', 'Z') || char.IsBetween(content[i], 'a', 'z');
        }
    }

}
