using System;
using System.Collections.Generic;
using System.Linq;
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

            do
            {
                Tokens.Add(GetNextToken());
            } while (Tokens.Last().Type != Interpreter_lib.Tokenizer.Tokens.END_OF_FILE);
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
            while (Advance())
            {
                switch (_currentChar)
                {
                    case '+':
                        return new Token(Interpreter_lib.Tokenizer.Tokens.PLUS, _currentChar.ToString());

                    case '-':
                        return new Token(Interpreter_lib.Tokenizer.Tokens.MINUS, _currentChar.ToString());

                    case '*':
                        if (Peek() == '*')
                        {
                            Advance();
                            return new Token(Interpreter_lib.Tokenizer.Tokens.POWER, "**");
                        }

                        return new Token(Interpreter_lib.Tokenizer.Tokens.MULTIPLY, _currentChar.ToString());

                    case '/':
                        if (Peek() == '/')
                        {
                            while (Peek() != '\n' && Peek() != '\0')
                                Advance();
                            continue;
                        }

                        return new Token(Interpreter_lib.Tokenizer.Tokens.DIVIDE, _currentChar.ToString());

                    case '%':
                        return new Token(Interpreter_lib.Tokenizer.Tokens.MODULUS, _currentChar.ToString());

                    case '^':
                        return new Token(Interpreter_lib.Tokenizer.Tokens.BITWISE_XOR, _currentChar.ToString());

                    case '[':
                        return new Token(Interpreter_lib.Tokenizer.Tokens.LEFT_SQUARE_BRACKET, _currentChar.ToString());

                    case ']':
                        return new Token(Interpreter_lib.Tokenizer.Tokens.RIGHT_SQUARE_BRACKET, _currentChar.ToString());

                    case '(':
                        return new Token(Interpreter_lib.Tokenizer.Tokens.LEFT_PARENTHESIS, _currentChar.ToString());

                    case ')':
                        return new Token(Interpreter_lib.Tokenizer.Tokens.RIGHT_PARENTHESIS, _currentChar.ToString());

                    case ',':
                        return new Token(Interpreter_lib.Tokenizer.Tokens.COMMA, _currentChar.ToString());

                    case '\n':
                    case ';':
                        return new Token(Interpreter_lib.Tokenizer.Tokens.END_OF_LINE, _currentChar.ToString());

                    case '\0':
                        return new Token(Interpreter_lib.Tokenizer.Tokens.END_OF_FILE, string.Empty);

                    case '=':
                        if (Peek() == '=')
                        {
                            Advance();
                            return new Token(Interpreter_lib.Tokenizer.Tokens.EQUAL, "==");
                        }

                        return new Token(Interpreter_lib.Tokenizer.Tokens.ASSIGN, "=");

                    case '!':
                        if (Peek() == '=')
                        {
                            Advance();
                            return new Token(Interpreter_lib.Tokenizer.Tokens.NOT_EQUAL, "!=");
                        }

                        return new Token(Interpreter_lib.Tokenizer.Tokens.NOT, "!");

                    case '<':
                        if (Peek() == '=')
                        {
                            Advance();
                            return new Token(Interpreter_lib.Tokenizer.Tokens.LESS_THAN_EQUAL, "<=");
                        }

                        if (Peek() == '-')
                        {
                            Advance();
                            return new Token(Interpreter_lib.Tokenizer.Tokens.ASSIGN, "<-");
                        }

                        if (Peek() == '<')
                        {
                            Advance();
                            return new Token(Interpreter_lib.Tokenizer.Tokens.BITWISE_LEFT_SHIFT, "<<");
                        }

                        return new Token(Interpreter_lib.Tokenizer.Tokens.LESS_THAN, "<");

                    case '>':
                        if (Peek() == '=')
                        {
                            Advance();
                            return new Token(Interpreter_lib.Tokenizer.Tokens.GREATER_THAN_EQUAL, ">=");
                        }

                        if (Peek() == '>')
                        {
                            Advance();
                            return new Token(Interpreter_lib.Tokenizer.Tokens.BITWISE_RIGHT_SHIFT, ">>");
                        }

                        return new Token(Interpreter_lib.Tokenizer.Tokens.GREATHER_THAN, ">");

                    case '&':
                        if (Peek() == '&')
                        {
                            Advance();
                            return new Token(Interpreter_lib.Tokenizer.Tokens.AND, "&&");
                        }

                        return new Token(Interpreter_lib.Tokenizer.Tokens.BITWISE_AND, "&");

                    case '|':
                        if (Peek() == '|')
                        {
                            Advance();
                            return new Token(Interpreter_lib.Tokenizer.Tokens.OR, "||");
                        }

                        return new Token(Interpreter_lib.Tokenizer.Tokens.BITWISE_OR, "|");

                    case '~':
                        return new Token(Interpreter_lib.Tokenizer.Tokens.BITWISE_NOT, "~");

                    case ' ':
                        if (PeekSequence("    "))
                        {
                            Advance(3);
                            return new Token(Interpreter_lib.Tokenizer.Tokens.TAB, "    ");
                        }

                        continue;

                    case '\t':
                        return new Token(Interpreter_lib.Tokenizer.Tokens.TAB, "\t");

                    case '"':
                        _accumulator += _currentChar;

                        while (Peek() != '"' && _currentChar != '\\')
                        {
                            Advance();
                            _accumulator += _currentChar;
                        }

                        return new Token(Interpreter_lib.Tokenizer.Tokens.STRING, _accumulator);

                    default:
                        while (Peek() != ' ' && Peek() != '\n' && Peek() != '\0')
                        {
                            _accumulator += _currentChar;
                            Advance();
                        }

                        if (isNumber(_accumulator))
                        {
                            _accumulator = string.Empty;
                            return new Token(Interpreter_lib.Tokenizer.Tokens.NUMBER, _accumulator);
                        }

                        if (_accumulator == _language.READ)
                        {
                            _accumulator = string.Empty;
                            return new Token(Interpreter_lib.Tokenizer.Tokens.READ, _accumulator);
                        }

                        if (_accumulator == _language.WRITE)
                        {
                            _accumulator = string.Empty;
                            return new Token(Interpreter_lib.Tokenizer.Tokens.WRITE, _accumulator);
                        }

                        if (_accumulator == _language.IF)
                        {
                            _accumulator = string.Empty;
                            return new Token(Interpreter_lib.Tokenizer.Tokens.IF, _accumulator);
                        }

                        if (_accumulator == _language.THEN)
                        {
                            _accumulator = string.Empty;
                            return new Token(Interpreter_lib.Tokenizer.Tokens.THEN, _accumulator);
                        }

                        if (_accumulator == _language.ELSE)
                        {
                            _accumulator = string.Empty;
                            return new Token(Interpreter_lib.Tokenizer.Tokens.ELSE, _accumulator);
                        }

                        if (_accumulator == _language.WHILE)
                        {
                            _accumulator = string.Empty;
                            return new Token(Interpreter_lib.Tokenizer.Tokens.WHILE, _accumulator);
                        }

                        if (_accumulator == _language.UNTIL)
                        {
                            _accumulator = string.Empty;
                            return new Token(Interpreter_lib.Tokenizer.Tokens.UNTIL, _accumulator);
                        }

                        if (_accumulator == _language.DO)
                        {
                            _accumulator = string.Empty;
                            return new Token(Interpreter_lib.Tokenizer.Tokens.DO, _accumulator);
                        }

                        if (_accumulator == _language.FOR)
                        {
                            _accumulator = string.Empty;
                            return new Token(Interpreter_lib.Tokenizer.Tokens.FOR, _accumulator);
                        }

                        if (_accumulator.Length > 0)
                        {
                            _accumulator = string.Empty;
                            return new Token(Interpreter_lib.Tokenizer.Tokens.IDENTIFIER, _accumulator);
                        }

                        break;
                }
            }

            return new Token(Interpreter_lib.Tokenizer.Tokens.END_OF_FILE, string.Empty);
        }

    }
}
