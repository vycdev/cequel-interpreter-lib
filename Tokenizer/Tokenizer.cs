using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter_lib.Tokenizer
{
    public class Tokenizer
    {
        private string _source;
        private int _position;
        private char _currentChar;
        private string _accumulator = string.Empty;
        private TokensLanguage _language = new();
        public List<Token> Tokens = new();

        public Tokenizer(string source, TokensLanguage? language = null)
        {
            _source = source;
            _position = 0;
            _currentChar = _source[_position];
            if (language != null)
                _language = language;

            // Generate list of tokens
            do
            {
                Tokens.Add(GetNextToken());
                Advance();
            } while (Tokens.Last().Type != Interpreter_lib.Tokenizer.EToken.END_OF_FILE);

            // Split tokens by line
            List<List<Token>> lines = new();
            List<Token> line = new();
            foreach (Token token in Tokens)
            {
                if (token.Type == EToken.END_OF_LINE || token.Type == EToken.END_OF_FILE)
                {
                    line.Add(token);
                    lines.Add(line);
                    line = new();
                }
                else
                {
                    line.Add(token);
                }
            }

            // Insert indent and dedent tokens
            List<Token> newTokens = new();
            int indentLevel = 0;

            foreach (List<Token> l in lines)
            {
                int indent = 0;
                if (l.Where(t => t.Type != EToken.TAB && t.Type != EToken.END_OF_LINE && t.Type != EToken.END_OF_FILE).Count() == 0)
                    continue; // Ignore empty lines

                // Calculate indent level
                foreach (Token token in l)
                {
                    if (token.Type == EToken.TAB)
                        indent++;
                    else
                        break;
                }

                // Insert indent or dedent tokens
                if (indent > indentLevel)
                {
                    newTokens.Add(new Token(EToken.INDENT, "INDENT"));
                    indentLevel = indent;
                }
                else if (indent < indentLevel)
                {
                    newTokens.Add(new Token(EToken.DEDENT, "DEDENT"));
                    indentLevel = indent;
                }

                // Add tokens to the new list
                newTokens.AddRange(l);
            }

            // Add dedent tokens at the end of the file
            if (indentLevel > 0)
            {
                if(newTokens.Last().Type == EToken.END_OF_LINE)
                {
                    newTokens.RemoveAt(newTokens.Count - 1);
                    newTokens.Add(new Token(EToken.END_OF_LINE, "\\n"));
                }

                for (int i = 0; i < indentLevel; i++)
                {
                    newTokens.Add(new Token(EToken.DEDENT, "DEDENT"));
                }
            }

            if (newTokens.Last().Type != EToken.END_OF_FILE)
                newTokens.Add(new Token(EToken.END_OF_FILE, "\\0"));

            Tokens = newTokens.Where(t => t.Type != EToken.TAB).ToList();
        }

        private bool Advance()
        {
            _position++;
            _currentChar = _position < _source.Length ? _source[_position] : '\0';

            return _position < _source.Length;
        }

        private bool Advance(int value)
        {
            _position += value;
            _currentChar = _position < _source.Length ? _source[_position] : '\0';

            return _position < _source.Length;
        }

        private char Peek()
        {
            return _position + 1 < _source.Length ? _source[_position + 1] : '\0';
        }

        private char Peek(int value)
        {
            return _position + value < _source.Length && _position + value >= 0 ? _source[_position + value] : '\0';
        }

        private bool PeekSequence(string sequence)
        {
            for (var i = 0; i < sequence.Length; i++)
            {
                if (Peek(i) != sequence[i])
                {
                    return false;
                }
            }

            return true;
        }

        private bool isNumber(string value)
        {
            if (value.Length == 0)
                return false;

            foreach (var c in value)
            {
                // account for floating point numbers
                if (!char.IsDigit(c) && c != '.')
                    return false;
            }

            // make sure there is only one decimal point
            return value.Count(x => x == '.') <= 1;
        }

        private Token GetNextToken()
        {
            do
            {
                switch (_currentChar)
                {
                    case '+':
                        return new Token(EToken.PLUS, _currentChar.ToString());

                    case '-':
                        return new Token(EToken.MINUS, _currentChar.ToString());

                    case '*':
                        if (Peek() == '*')
                        {
                            Advance();
                            return new Token(EToken.POWER, "**");
                        }

                        return new Token(EToken.MULTIPLY, _currentChar.ToString());

                    case '/':
                        if (Peek() == '/')
                        {
                            while (Peek() != '\n' && Peek() != '\0')
                                Advance();
                            continue;
                        }

                        return new Token(EToken.DIVIDE, _currentChar.ToString());

                    case '%':
                        return new Token(EToken.MODULUS, _currentChar.ToString());

                    case '^':
                        return new Token(EToken.BITWISE_XOR, _currentChar.ToString());

                    case '[':
                        return new Token(EToken.LEFT_SQUARE_BRACKET, _currentChar.ToString());

                    case ']':
                        return new Token(EToken.RIGHT_SQUARE_BRACKET, _currentChar.ToString());

                    case '(':
                        return new Token(EToken.LEFT_PARENTHESIS, _currentChar.ToString());

                    case ')':
                        return new Token(EToken.RIGHT_PARENTHESIS, _currentChar.ToString());

                    case ',':
                        return new Token(EToken.COMMA, _currentChar.ToString());

                    case '\r':
                        if (Peek() == '\n')
                        {
                            Advance();
                            return new Token(EToken.END_OF_LINE, "\\r\\n");
                        }
                        return new Token(EToken.END_OF_LINE, "\\r");

                    case '\n':
                        return new Token(EToken.END_OF_LINE, "\\n");

                    case ';':
                        return new Token(EToken.END_OF_LINE, _currentChar.ToString());

                    case '\0':
                        return new Token(EToken.END_OF_FILE, "\\0");

                    case '=':
                        if (Peek() == '=')
                        {
                            Advance();
                            return new Token(EToken.EQUAL, "==");
                        }

                        return new Token(EToken.ASSIGN, "=");

                    case '!':
                        if (Peek() == '=')
                        {
                            Advance();
                            return new Token(EToken.NOT_EQUAL, "!=");
                        }

                        return new Token(EToken.NOT, "!");

                    case '<':
                        if (Peek() == '=')
                        {
                            Advance();
                            return new Token(EToken.LESS_THAN_EQUAL, "<=");
                        }

                        if (Peek() == '-')
                        {
                            Advance();
                            return new Token(EToken.ASSIGN, "<-");
                        }

                        if (Peek() == '<')
                        {
                            Advance();
                            return new Token(EToken.BITWISE_LEFT_SHIFT, "<<");
                        }

                        return new Token(EToken.LESS_THAN, "<");

                    case '>':
                        if (Peek() == '=')
                        {
                            Advance();
                            return new Token(EToken.GREATER_THAN_EQUAL, ">=");
                        }

                        if (Peek() == '>')
                        {
                            Advance();
                            return new Token(EToken.BITWISE_RIGHT_SHIFT, ">>");
                        }

                        return new Token(EToken.GREATER_THAN, ">");

                    case '&':
                        if (Peek() == '&')
                        {
                            Advance();
                            return new Token(EToken.AND, "&&");
                        }

                        return new Token(EToken.BITWISE_AND, "&");

                    case '|':
                        if (Peek() == '|')
                        {
                            Advance();
                            return new Token(EToken.OR, "||");
                        }

                        return new Token(EToken.BITWISE_OR, "|");

                    case '~':
                        return new Token(EToken.BITWISE_NOT, "~");

                    case ' ':
                        if (PeekSequence("    "))
                        {
                            Advance(3);
                            return new Token(EToken.TAB, "\\t");
                        }

                        continue;

                    case '\t':
                        return new Token(EToken.TAB, "\\t");

                    case '\'':
                        _accumulator = string.Empty;

                        do
                        {
                            Advance();
                            _accumulator += _currentChar;
                        } while (!"'".Contains(Peek()) ||
                                (
                                    _currentChar.ToString() + Peek() == "\\'" ||
                                    Peek().ToString() + Peek(1) == "\\'" ||
                                    Peek(-1) + _currentChar.ToString() == "\\'"
                                )
                        );

                        Advance();

                        return new Token(EToken.STRING, _accumulator);
                    case '"':
                        _accumulator = string.Empty;

                        do
                        {
                            Advance();
                            _accumulator += _currentChar;
                        } while (!"\"".Contains(Peek()) ||
                                (
                                    _currentChar.ToString() + Peek() == "\\\"" ||
                                    Peek().ToString() + Peek(1) == "\\\"" ||
                                    Peek(-1) + _currentChar.ToString() == "\\\""
                                )
                        );

                        Advance();

                        return new Token(EToken.STRING, _accumulator);

                    default:
                        PropertyInfo[] properties = _language.GetType().GetProperties();

                        bool continueAccumulation = false;
                        foreach (PropertyInfo property in properties)
                        {
                            var propertyValue = property.GetValue(_language)?.ToString() ?? string.Empty;

                            if (propertyValue == _accumulator)
                            {
                                break;
                            }

                            if (propertyValue.Split(" ").Length > 1)
                                foreach (string word in propertyValue.Split(" "))
                                    if (_accumulator.Contains(word))
                                    {
                                        continueAccumulation = true;
                                        break;
                                    }

                            if (continueAccumulation)
                                break;
                        }

                        if (!continueAccumulation)
                            _accumulator = string.Empty;
                        else
                            _accumulator += " ";

                        do
                        {
                            _accumulator += _currentChar;
                            Advance();
                        } while (!" \n\0\r".Contains(_currentChar) && (char.IsLetterOrDigit(_currentChar) || ".".Contains(_currentChar)));

                        Advance(-1);

                        foreach (PropertyInfo property in properties)
                        {
                            var propertyValue = property.GetValue(_language)?.ToString() ?? string.Empty;

                            if (propertyValue == _accumulator && propertyValue.Split(" ").Length > 1)
                            {
                                for (int i = 1; i < propertyValue.Split(" ").Length; i++)
                                    Tokens.RemoveAt(Tokens.Count() - i);
                                break;
                            }
                        }

                        if (isNumber(_accumulator))
                            return new Token(EToken.NUMBER, _accumulator);


                        // TODO: associate the enums to the properties so you can do a foreach instead of this. 

                        if (_accumulator == _language.READ)
                            return new Token(EToken.READ, _accumulator);

                        if (_accumulator == _language.WRITE)
                            return new Token(EToken.WRITE, _accumulator);

                        if (_accumulator == _language.IF)
                            return new Token(EToken.IF, _accumulator);

                        if (_accumulator == _language.THEN)
                            return new Token(EToken.THEN, _accumulator);

                        if (_accumulator == _language.ELSE)
                            return new Token(EToken.ELSE, _accumulator);

                        if (_accumulator == _language.WHILE)
                            return new Token(EToken.WHILE, _accumulator);

                        if (_accumulator == _language.UNTIL)
                            return new Token(EToken.UNTIL, _accumulator);

                        if (_accumulator == _language.DO)
                            return new Token(EToken.DO, _accumulator);

                        if (_accumulator == _language.FOR)
                            return new Token(EToken.FOR, _accumulator);

                        if (_accumulator == _language.REPEAT)
                            return new Token(EToken.REPEAT, _accumulator);

                        if (_accumulator.Length > 0)
                            return new Token(EToken.IDENTIFIER, _accumulator);

                        break;
                }
            } while (Advance());

            return new Token(EToken.END_OF_FILE, "\\0");
        }
    }
}
