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

            do
            {
                Tokens.Add(GetNextToken());
                Advance();
            } while (Tokens.Last().Type != Interpreter_lib.Tokenizer.EToken.END_OF_FILE);
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
                        return new Token(Interpreter_lib.Tokenizer.EToken.PLUS, _currentChar.ToString());

                    case '-':
                        return new Token(Interpreter_lib.Tokenizer.EToken.MINUS, _currentChar.ToString());

                    case '*':
                        if (Peek() == '*')
                        {
                            Advance();
                            return new Token(Interpreter_lib.Tokenizer.EToken.POWER, "**");
                        }

                        return new Token(Interpreter_lib.Tokenizer.EToken.MULTIPLY, _currentChar.ToString());

                    case '/':
                        if (Peek() == '/')
                        {
                            while (Peek() != '\n' && Peek() != '\0')
                                Advance();
                            continue;
                        }

                        return new Token(Interpreter_lib.Tokenizer.EToken.DIVIDE, _currentChar.ToString());

                    case '%':
                        return new Token(Interpreter_lib.Tokenizer.EToken.MODULUS, _currentChar.ToString());

                    case '^':
                        return new Token(Interpreter_lib.Tokenizer.EToken.BITWISE_XOR, _currentChar.ToString());

                    case '[':
                        return new Token(Interpreter_lib.Tokenizer.EToken.LEFT_SQUARE_BRACKET, _currentChar.ToString());

                    case ']':
                        return new Token(Interpreter_lib.Tokenizer.EToken.RIGHT_SQUARE_BRACKET, _currentChar.ToString());

                    case '(':
                        return new Token(Interpreter_lib.Tokenizer.EToken.LEFT_PARENTHESIS, _currentChar.ToString());

                    case ')':
                        return new Token(Interpreter_lib.Tokenizer.EToken.RIGHT_PARENTHESIS, _currentChar.ToString());

                    case ',':
                        return new Token(Interpreter_lib.Tokenizer.EToken.COMMA, _currentChar.ToString());

                    case '\r':
                        if(Peek() == '\n')
                        {
                            Advance();
                            return new Token(Interpreter_lib.Tokenizer.EToken.END_OF_LINE, "\\r\\n");
                        }
                        return new Token(Interpreter_lib.Tokenizer.EToken.END_OF_LINE, "\\r");
                    
                    case '\n':
                        return new Token(Interpreter_lib.Tokenizer.EToken.END_OF_LINE, "\\n");

                    case ';':
                        return new Token(Interpreter_lib.Tokenizer.EToken.END_OF_LINE, _currentChar.ToString());

                    case '\0':
                        return new Token(Interpreter_lib.Tokenizer.EToken.END_OF_FILE, "\\0");

                    case '=':
                        if (Peek() == '=')
                        {
                            Advance();
                            return new Token(Interpreter_lib.Tokenizer.EToken.EQUAL, "==");
                        }

                        return new Token(Interpreter_lib.Tokenizer.EToken.ASSIGN, "=");

                    case '!':
                        if (Peek() == '=')
                        {
                            Advance();
                            return new Token(Interpreter_lib.Tokenizer.EToken.NOT_EQUAL, "!=");
                        }

                        return new Token(Interpreter_lib.Tokenizer.EToken.NOT, "!");

                    case '<':
                        if (Peek() == '=')
                        {
                            Advance();
                            return new Token(Interpreter_lib.Tokenizer.EToken.LESS_THAN_EQUAL, "<=");
                        }

                        if (Peek() == '-')
                        {
                            Advance();
                            return new Token(Interpreter_lib.Tokenizer.EToken.ASSIGN, "<-");
                        }

                        if (Peek() == '<')
                        {
                            Advance();
                            return new Token(Interpreter_lib.Tokenizer.EToken.BITWISE_LEFT_SHIFT, "<<");
                        }

                        return new Token(Interpreter_lib.Tokenizer.EToken.LESS_THAN, "<");

                    case '>':
                        if (Peek() == '=')
                        {
                            Advance();
                            return new Token(Interpreter_lib.Tokenizer.EToken.GREATER_THAN_EQUAL, ">=");
                        }

                        if (Peek() == '>')
                        {
                            Advance();
                            return new Token(Interpreter_lib.Tokenizer.EToken.BITWISE_RIGHT_SHIFT, ">>");
                        }

                        return new Token(Interpreter_lib.Tokenizer.EToken.GREATER_THAN, ">");

                    case '&':
                        if (Peek() == '&')
                        {
                            Advance();
                            return new Token(Interpreter_lib.Tokenizer.EToken.AND, "&&");
                        }

                        return new Token(Interpreter_lib.Tokenizer.EToken.BITWISE_AND, "&");

                    case '|':
                        if (Peek() == '|')
                        {
                            Advance();
                            return new Token(Interpreter_lib.Tokenizer.EToken.OR, "||");
                        }

                        return new Token(Interpreter_lib.Tokenizer.EToken.BITWISE_OR, "|");

                    case '~':
                        return new Token(Interpreter_lib.Tokenizer.EToken.BITWISE_NOT, "~");

                    case ' ':
                        if (PeekSequence("    "))
                        {
                            Advance(3);
                            return new Token(Interpreter_lib.Tokenizer.EToken.TAB, "\\t");
                        }

                        continue;

                    case '\t':
                        return new Token(Interpreter_lib.Tokenizer.EToken.TAB, "\\t");

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

                        return new Token(Interpreter_lib.Tokenizer.EToken.STRING, _accumulator);
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
                        
                        return new Token(Interpreter_lib.Tokenizer.EToken.STRING, _accumulator);

                    default:
                        PropertyInfo[] properties = _language.GetType().GetProperties();

                        bool continueAccumulation = false;
                        foreach (PropertyInfo property in properties)
                        {
                            var propertyValue = property.GetValue(_language)?.ToString() ?? string.Empty;
                         
                            if(propertyValue == _accumulator)
                            {
                                break;
                            }

                            if (propertyValue.Split(" ").Length > 1)
                                foreach (string word in propertyValue.Split(" "))
                                    if(_accumulator.Contains(word))
                                    {
                                        continueAccumulation = true;
                                        break;
                                    }

                            if(continueAccumulation)
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
                            return new Token(Interpreter_lib.Tokenizer.EToken.NUMBER, _accumulator);


                        // TODO: associate the enums to the properties so you can do a foreach instead of this. 

                        if (_accumulator == _language.READ)
                            return new Token(Interpreter_lib.Tokenizer.EToken.READ, _accumulator);

                        if (_accumulator == _language.WRITE)
                            return new Token(Interpreter_lib.Tokenizer.EToken.WRITE, _accumulator);

                        if (_accumulator == _language.IF)
                            return new Token(Interpreter_lib.Tokenizer.EToken.IF, _accumulator);

                        if (_accumulator == _language.THEN)
                            return new Token(Interpreter_lib.Tokenizer.EToken.THEN, _accumulator);

                        if (_accumulator == _language.ELSE)
                            return new Token(Interpreter_lib.Tokenizer.EToken.ELSE, _accumulator);

                        if (_accumulator == _language.WHILE)
                            return new Token(Interpreter_lib.Tokenizer.EToken.WHILE, _accumulator);

                        if (_accumulator == _language.UNTIL)
                            return new Token(Interpreter_lib.Tokenizer.EToken.UNTIL, _accumulator);

                        if (_accumulator == _language.DO)
                            return new Token(Interpreter_lib.Tokenizer.EToken.DO, _accumulator);

                        if (_accumulator == _language.FOR)
                            return new Token(Interpreter_lib.Tokenizer.EToken.FOR, _accumulator);

                        if (_accumulator == _language.REPEAT)
                            return new Token(Interpreter_lib.Tokenizer.EToken.REPEAT, _accumulator);

                        if (_accumulator.Length > 0)
                            return new Token(Interpreter_lib.Tokenizer.EToken.IDENTIFIER, _accumulator);

                        break;
                }
            } while (Advance());

            return new Token(Interpreter_lib.Tokenizer.EToken.END_OF_FILE, "\\0");
        }
    }
}
