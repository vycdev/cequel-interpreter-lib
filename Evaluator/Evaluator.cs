using Interpreter_lib.Parser;
using Interpreter_lib.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter_lib.Evaluator
{
    enum AtomType
    {
        NUMBER,
        STRING
    }

    class Atom
    {
        public AtomType Type;
        public object Value;

        public Atom(AtomType type, object value)
        {
            this.Type = type;
            this.Value = value;
        }
    }

    public class Evaluator
    {
        private static Dictionary<string, Atom> _variables = new Dictionary<string, Atom>();

        void Evaluate(Node node)
        {
            switch (node._rule)
            {
                case ERule.ROOT:
                    EvaluateRoot(node);
                    break;
                case ERule.WHILE_LOOP:
                    EvaluateWhileLoop(node);
                    break;
                case ERule.DO_WHILE_LOOP:
                    EvaluateDoWhileLoop(node);
                    break;
                case ERule.REPEAT_UNTIL_LOOP:
                    EvaluateRepeatUntilLoop(node);
                    break;
                case ERule.FOR_LOOP:
                    EvaluateForLoop(node);
                    break;
                case ERule.IF_STATEMENT:
                    EvaluateIfStatement(node);
                    break;
                case ERule.PRINT:
                    EvaluatePrint(node);
                    break;
                case ERule.READ:
                    EvaluateRead(node);
                    break;
                case ERule.ASSIGNMENT:
                    EvaluateAssignment(node);
                    break;
                default:
                    break;
            }
        }

        // ROOT 
        public void EvaluateRoot(Node node)
        {
            foreach (var n in node.GetSyntaxNodes())
            {
                if (n.GetType() == typeof(Node))
                    Evaluate((Node)n);
                else
                    throw new Exception("Invalid node type");
            }
        }

        #region NODE EVALUATION FUNCTIONS

        // WHILE_LOOP,
        void EvaluateWhileLoop(Node node)
        {

        }

        // DO_WHILE_LOOP,
        void EvaluateDoWhileLoop(Node node)
        {

        }

        // REPEAT_UNTIL_LOOP,
        void EvaluateRepeatUntilLoop(Node node)
        {

        }

        // FOR_LOOP,
        void EvaluateForLoop(Node node)
        {

        }

        // IF_STATEMENT, ELSE_STATEMENT,
        void EvaluateIfStatement(Node node)
        {

        }

        // PRINT,
        void EvaluatePrint(Node node)
        {

        }

        // READ,
        void EvaluateRead(Node node)
        {

        }

        // ASSIGNMENT,
        void EvaluateAssignment(Node node)
        {

        }

        Atom EvaluateOperator(Node node)
        {
            switch (node._rule)
            {
                case ERule.LOGICAL_OR:
                    return EvaluateLogicalOr(node);
                case ERule.LOGICAL_AND:
                    return EvaluateLogicalAnd(node);
                case ERule.BITWISE_OR:
                    return EvaluateBitwiseOr(node);
                case ERule.BITWISE_XOR:
                    return EvaluateBitwiseXor(node);
                case ERule.BITWISE_AND:
                    return EvaluateBitwiseAnd(node);
                case ERule.NOT_EQUAL:
                    return EvaluateNotEqual(node);
                case ERule.EQUAL:
                    return EvaluateEqual(node);
                case ERule.GREATER_THAN_EQUAL:
                    return EvaluateGreaterThanEqual(node);
                case ERule.GREATER_THAN:
                    return EvaluateGreaterThan(node);
                case ERule.LESS_THAN_EQUAL:
                    return EvaluateLessThanEqual(node);
                case ERule.LESS_THAN:
                    return EvaluateLessThan(node);
                case ERule.BITWISE_RIGHT_SHIFT:
                    return EvaluateBitwiseRightShift(node);
                case ERule.BITWISE_LEFT_SHIFT:
                    return EvaluateBitwiseLeftShift(node);
                case ERule.SUM:
                    return EvaluateSum(node);
                case ERule.SUBTRACT:
                    return EvaluateSubtract(node);
                case ERule.MULTIPLY:
                    return EvaluateMultiply(node);
                case ERule.DIVIDE:
                    return EvaluateDivide(node);
                case ERule.MODULUS:
                    return EvaluateModulus(node);
                case ERule.POWER:
                    return EvaluatePower(node);
                case ERule.UNARY_MINUS:
                    return EvaluateUnaryMinus(node);
                case ERule.UNARY_PLUS:
                    return EvaluateUnaryPlus(node);
                case ERule.UNARY_NOT:
                    return EvaluateUnaryNot(node);
                case ERule.UNARY_BITWISE_NOT:
                    return EvaluateUnaryBitwiseNot(node);
                case ERule.FLOOR:
                    return EvaluateFloor(node);
                case ERule.EXPRESSION:
                    return EvaluateExpression(node);
                default:
                    break;
            }

            return new Atom(AtomType.NUMBER, 0);
        }

        // EXPRESSION,
        Atom EvaluateExpression(Node node)
        {
            return new Atom(AtomType.NUMBER, 0);

        }

        // LOGICAL_OR, 
        Atom EvaluateLogicalOr(Node node)
        {
            return new Atom(AtomType.NUMBER, 0);

        }

        // LOGICAL_AND, 
        Atom EvaluateLogicalAnd(Node node)
        {
            return new Atom(AtomType.NUMBER, 0);

        }

        // BITWISE_OR,
        Atom EvaluateBitwiseOr(Node node)
        {
            return new Atom(AtomType.NUMBER, 0);

        }

        // BITWISE_XOR,
        Atom EvaluateBitwiseXor(Node node)
        {
            return new Atom(AtomType.NUMBER, 0);

        }

        // BITWISE_AND,
        Atom EvaluateBitwiseAnd(Node node)
        {
            return new Atom(AtomType.NUMBER, 0);

        }

        // NOT_EQUAL,
        Atom EvaluateNotEqual(Node node)
        {
            return new Atom(AtomType.NUMBER, 0);

        }

        // EQUAL,
        Atom EvaluateEqual(Node node)
        {
            return new Atom(AtomType.NUMBER, 0);

        }

        // GREATER_THAN_EQUAL,
        Atom EvaluateGreaterThanEqual(Node node)
        {
            return new Atom(AtomType.NUMBER, 0);

        }

        // GREATER_THAN,
        Atom EvaluateGreaterThan(Node node)
        {
            return new Atom(AtomType.NUMBER, 0);

        }

        // LESS_THAN_EQUAL,
        Atom EvaluateLessThanEqual(Node node)
        {
            return new Atom(AtomType.NUMBER, 0);

        }

        // LESS_THAN,
        Atom EvaluateLessThan(Node node)
        {
            return new Atom(AtomType.NUMBER, 0);

        }

        // BITWISE_RIGHT_SHIFT,
        Atom EvaluateBitwiseRightShift(Node node)
        {
            return new Atom(AtomType.NUMBER, 0);

        }

        // BITWISE_LEFT_SHIFT,
        Atom EvaluateBitwiseLeftShift(Node node)
        {
            return new Atom(AtomType.NUMBER, 0);

        }

        // SUM,
        Atom EvaluateSum(Node node)
        {
            return new Atom(AtomType.NUMBER, 0);

        }

        // SUBTRACT,
        Atom EvaluateSubtract(Node node)
        {
            return new Atom(AtomType.NUMBER, 0);

        }

        // MULTIPLY,
        Atom EvaluateMultiply(Node node)
        {
            return new Atom(AtomType.NUMBER, 0);

        }

        // DIVIDE,
        Atom EvaluateDivide(Node node)
        {
            return new Atom(AtomType.NUMBER, 0);

        }

        // MODULUS,
        Atom EvaluateModulus(Node node)
        {
            return new Atom(AtomType.NUMBER, 0);

        }

        // POWER,
        Atom EvaluatePower(Node node)
        {
            return new Atom(AtomType.NUMBER, 0);

        }

        // UNARY_MINUS,
        Atom EvaluateUnaryMinus(Node node)
        {
            ISyntaxNode n = node.GetSyntaxNodes()[0];

            Atom result;

            if (n.GetType() == typeof(Token))
                result = GetAtom(n);
            else
                result = EvaluateExpression((Node)n);

            if (result.Type != AtomType.NUMBER)
                throw new EvaluatorException(n, $"Invalid variable type, expected NUMBER but got {result.Type}.");

            return new Atom(AtomType.NUMBER, -(float)result.Value);
        }

        // UNARY_PLUS,
        Atom EvaluateUnaryPlus(Node node)
        {
            ISyntaxNode n = node.GetSyntaxNodes()[0];

            Atom result;

            if (n.GetType() == typeof(Token))
                result = GetAtom(n);
            else
                result = EvaluateExpression((Node)n);

            if (result.Type != AtomType.NUMBER)
                throw new EvaluatorException(n, $"Invalid variable type, expected NUMBER but got {result.Type}.");

            return new Atom(AtomType.NUMBER, +(float)result.Value);
        }

        // UNARY_NOT,
        Atom EvaluateUnaryNot(Node node)
        {
            ISyntaxNode n = node.GetSyntaxNodes()[0];

            Atom result; 

            if (n.GetType() == typeof(Token))
                result = GetAtom(n);
            else
                result = EvaluateExpression((Node)n);
           
            if (result.Type != AtomType.NUMBER)
                throw new EvaluatorException(n, $"Invalid variable type, expected NUMBER but got {result.Type}.");

            return new Atom(AtomType.NUMBER, (int)result.Value == 0 ? 1 : 0);
        }

        // UNARY_BITWISE_NOT,
        Atom EvaluateUnaryBitwiseNot(Node node)
        {
            ISyntaxNode n = node.GetSyntaxNodes()[0];

            Atom result; 

            if (n.GetType() == typeof(Token))
                result = GetAtom(n);
            else
                result = EvaluateExpression((Node)n);
           
            if (result.Type != AtomType.NUMBER)
                throw new EvaluatorException(n, $"Invalid variable type, expected NUMBER but got {result.Type}.");

            return new Atom(AtomType.NUMBER, ~(int)result.Value);
        }

        // FLOOR,
        Atom EvaluateFloor(Node node)
        {
            Atom result = EvaluateExpression((Node)node.GetSyntaxNodes()[0]);
            if (result.Type != AtomType.NUMBER)
            {
                throw new EvaluatorException(node, "Invalid token type, expected number.");
            }

            return new Atom(AtomType.NUMBER, Math.Floor((float)result.Value));
        }

        #endregion

        #region HELPER FUNCTIONS

        private static Atom GetAtom(ISyntaxNode node)
        {
            if (node.GetType() == typeof(Node))
                throw new EvaluatorException(node, "Cannot get variable of type Node.");

            Token token = (Token)node;

            if (token.Type == EToken.IDENTIFIER)
            {
                if (_variables.TryGetValue(token.Value, out Atom? value))
                {
                    return value;
                }
                else
                {
                    throw new EvaluatorException(token, "Variable was used before declaration.");
                }
            }
            else if (token.Type == EToken.STRING)
            {
                return new Atom(AtomType.STRING, token.Value);
            }
            else if (token.Type == EToken.NUMBER)
            {
                return new Atom(AtomType.NUMBER, float.Parse(token.Value));
            }
            else
            {
                throw new EvaluatorException(token, "Invalid token type when trying to obtain variable, expected IDENTIFIER, STRING or NUMBER.");
            }
        }

        #endregion
    }
}
