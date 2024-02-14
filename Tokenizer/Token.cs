using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter_lib.Tokenizer
{
    public enum EToken
    {
        PLUS, MINUS, MULTIPLY, DIVIDE, MODULUS, POWER, LEFT_SQUARE_BRACKET, RIGHT_SQUARE_BRACKET, // arithmetic operators
        LEFT_PARENTHESIS, RIGHT_PARENTHESIS, // grouping operators
        EQUAL, NOT_EQUAL, LESS_THAN, GREATHER_THAN, LESS_THAN_EQUAL, GREATER_THAN_EQUAL, // comparison operators
        AND, OR, NOT, // logical operators

        BITWISE_AND, BITWISE_OR, BITWISE_XOR, BITWISE_NOT, BITWISE_LEFT_SHIFT, BITWISE_RIGHT_SHIFT, // bitwise operators

        READ, WRITE, // I/O operators
        ASSIGN, // assignment operator
        IDENTIFIER, // variable name
        NUMBER, // number
        STRING, // string
        COMMA, // comma

        IF, THEN, ELSE, // conditional operators
        WHILE, DO, REPEAT, UNTIL, // simple loop operators
        FOR, // complex loop operator

        END_OF_LINE, // end of line, (semicolon or end of line are both valid)
        END_OF_FILE, // end of file
        TAB, // tab
    }
    public class TokensLanguage
    {
        public string READ { get; set; } = "read";
        public string WRITE { get; set; } = "write";
        public string IF { get; set; } = "if";
        public string THEN { get; set; } = "then";
        public string ELSE { get; set; } = "else";
        public string WHILE { get; set; } = "while";
        public string DO { get; set; } = "do";
        public string REPEAT { get; set; } = "repeat";
        public string UNTIL { get; set; } = "until";
        public string FOR { get; set; } = "for";
    }

    public class Token
    {
        public EToken Type { get; private set; }
        public string Value { get; private set; }

        public Token(EToken type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}
