namespace JsonParser
{
    internal class Lexer
    {
        private const char TAB = (char)0x09;
        private enum SequenceKind
        {
            None,
            Generic
        }

        private int i;
        private int quotationMarkIndex;
        private readonly string ct;
        private int sequenceIndex;
        private SequenceKind sequenceKind;
        private int tokenLine;
        private int tokenColumn;
        private readonly List<Token> result;

        private int line;
        private int column;

        private bool HasCharRepeated(char ch)
        {
            return i > 0 && ct[i] == ch && ct[i - 1] == ch;
        }
        private bool WillCharRepeat(char ch)
        {
            return ct[i] == ch && i + 1 < ct.Length && ct[i + 1] == ch;
        }
        private bool IsNotEscapedChar(char ch)
        {
            return i > 0 ? ct[i] == ch && ct[i - 1] != '\\' : ct[i] == ch;
        }
        private bool IsQuotationMarkOpen()
        {
            return quotationMarkIndex > -1;
        }
        private void AddSymbol()
        {
            AddToResult(Token.Description.Symbol, ct[i].ToString(), line, column);
        }
        private void RegisterQuotationMarkOpen()
        {
            quotationMarkIndex = i;
            tokenColumn = column;
            tokenLine = line;
        }
        private void RegisterLiteralString()
        {
            AddToResult(Token.Description.StringLiteral, ct.Substring(quotationMarkIndex + 1, i - (quotationMarkIndex + 1)), tokenLine, tokenColumn);
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
                string sequence = ct.Substring(sequenceIndex, i - sequenceIndex);
                AddToResult(Token.Description.Sequence, sequence, tokenLine, tokenColumn);
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
            return ct[i] == '_' || ct[i] == '.' || ct[i] == '+' || ct[i] == '-' ||
                char.IsBetween(ct[i], '0', '9') || char.IsBetween(ct[i], 'A', 'Z') || char.IsBetween(ct[i], 'a', 'z');
        }
        private void AddToResult(Token.Description desc, string content, int line, int column)
        {
            result.Add(new Token(desc, content, line, column));
        }
        public Lexer(string content)
        {
            ct = content;
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
            for (i = 0; i < ct.Length; i++)
            {
                if (ct[i] == '\n')
                {
                    line++;
                    column = 0;
                }
                else if (ct[i] == TAB)
                {
                    column += 4;
                }
                else
                {
                    column++;
                }

                if (IsNotEscapedChar('\"'))
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
                    if (!char.IsControl(ct[i]) && ct[i] != ' ')
                    {
                        AddSymbol();
                    }
                }
            }
            RegisterCurrentSequence();
            return result;
        }

    }

}
