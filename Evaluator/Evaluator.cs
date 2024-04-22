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

            throw new EvaluatorException(node, "Invalid node type.");
        }

        // ROOT 
        public void EvaluateRoot(Node node)
        {
            foreach (var n in node.GetSyntaxNodes())
            {
                if (n.GetType() == typeof(Node))
                    Evaluate((Node)n);
                else
                    throw new EvaluatorException(node, "Invalid node type, received token instead.");
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
                    return EvaluateBinaryOperator<int>(node, (x, y) => (x == 1 || y == 1) ? 1 : 0);
                case ERule.LOGICAL_AND:
                    return EvaluateBinaryOperator<int>(node, (x, y) => (x == 1 && y == 1) ? 1 : 0);
                case ERule.BITWISE_OR:
                    return EvaluateBinaryOperator<int>(node, (x, y) => x | y);
                case ERule.BITWISE_XOR:
                    return EvaluateBinaryOperator<int>(node, (x, y) => x ^ y);
                case ERule.BITWISE_AND:
                    return EvaluateBinaryOperator<int>(node, (x, y) => x & y);
                case ERule.NOT_EQUAL:
                    return EvaluateBinaryOperator<float>(node, (x, y) => (x != y) ? 1 : 0);
                case ERule.EQUAL:
                    return EvaluateBinaryOperator<float>(node, (x, y) => (x == y) ? 1 : 0);
                case ERule.GREATER_THAN_EQUAL:
                    return EvaluateBinaryOperator<float>(node, (x, y) => (x >= y) ? 1 : 0);
                case ERule.GREATER_THAN:
                    return EvaluateBinaryOperator<float>(node, (x, y) => (x > y) ? 1 : 0);
                case ERule.LESS_THAN_EQUAL:
                    return EvaluateBinaryOperator<float>(node, (x, y) => (x <= y) ? 1 : 0);
                case ERule.LESS_THAN:
                    return EvaluateBinaryOperator<float>(node, (x, y) => (x < y) ? 1 : 0);
                case ERule.BITWISE_RIGHT_SHIFT:
                    return EvaluateBinaryOperator<int>(node, (x, y) => x >> y);
                case ERule.BITWISE_LEFT_SHIFT:
                    return EvaluateBinaryOperator<int>(node, (x, y) => x << y);
                case ERule.SUM:
                    return EvaluateSum(node);
                case ERule.SUBTRACT:
                    return EvaluateBinaryOperator<float>(node, (x, y) => x - y);
                case ERule.MULTIPLY:
                    return EvaluateBinaryOperator<float>(node, (x, y) => x * y);
                case ERule.DIVIDE:
                    return EvaluateBinaryOperator<float>(node, (x, y) => x / y);
                case ERule.MODULUS:
                    return EvaluateBinaryOperator<float>(node, (x, y) => x % y);
                case ERule.POWER:
                    return EvaluatePower(node);
                case ERule.UNARY_MINUS:
                    return EvaluateUnaryOperator<float>(node, x => -x);
                case ERule.UNARY_PLUS:
                    return EvaluateUnaryOperator<float>(node, x => +x);
                case ERule.UNARY_NOT:
                    return EvaluateUnaryOperator<int>(node, x => x == 0 ? 1 : 0);
                case ERule.UNARY_BITWISE_NOT:
                    return EvaluateUnaryOperator<int>(node, x => ~x);
                case ERule.FLOOR:
                    return EvaluateUnaryOperator<float>(node, x => (int)Math.Floor(x));
                case ERule.EXPRESSION:
                    return EvaluateExpression(node);
                default:
                    break;
            }

            throw new EvaluatorException(node, "Invalid operator.");
        }

        // EXPRESSION,
        Atom EvaluateExpression(Node node)
        {
            List<ISyntaxNode> nodes = node.GetSyntaxNodes();
            if (nodes.Count != 1)
                throw new EvaluatorException(node, $"Invalid number of arguments for operator {node._rule}.");

            ISyntaxNode n = nodes[0];

            if (n.GetType() == typeof(Token))
                return GetAtom(n);
            else
                return EvaluateOperator((Node)n);
        }

        // SUM,
        Atom EvaluateSum(Node node)
        {
            List<ISyntaxNode> n = node.GetSyntaxNodes();
            if (n.Count < 2)
                throw new EvaluatorException(node, "Invalid number of arguments for operator POWER.");

            Atom result;
            Atom acc;

            // Get first element
            if (n[0].GetType() == typeof(Token))
                result = GetAtom(n[0]);
            else
                result = EvaluateOperator((Node)n[0]);

            acc = result;

            // Iterate through the rest of the elements
            for (int i = 1; i < n.Count; i++)
            {
                if (n[i].GetType() == typeof(Token))
                    result = GetAtom(n[i]);
                else
                    result = EvaluateOperator((Node)n[i]);

                if (result.Type == AtomType.NUMBER)
                    if(acc.Type == AtomType.NUMBER)
                        acc = new Atom(AtomType.NUMBER, (float)acc.Value + (float)result.Value);
                    else 
                        acc = new Atom(AtomType.STRING, (string)acc.Value + (float)result.Value);
                else
                    if(acc.Type == AtomType.NUMBER)
                        acc = new Atom(AtomType.STRING, (float)acc.Value + (string)result.Value);
                    else
                        acc = new Atom(AtomType.STRING, (string)acc.Value + (string)result.Value);
            }

            return acc;
        }

        // POWER,
        Atom EvaluatePower(Node node)
        {
            List<ISyntaxNode> n = node.GetSyntaxNodes();
            if(n.Count < 2)
                throw new EvaluatorException(node, $"Invalid number of arguments for operator {node._rule}.");

            Atom result;
            Atom acc; 

            // Get last element
            if (n[n.Count - 1].GetType() == typeof(Token))
                result = GetAtom(n[n.Count - 1]);
            else
                result = EvaluateOperator((Node)n[n.Count - 1]);

            if (result.Type != AtomType.NUMBER)
                throw new EvaluatorException(n[n.Count - 1], $"Invalid variable type, expected NUMBER but got {result.Type}.");

            acc = result;

            // Iterate through the rest of the elements
            for (int i = n.Count - 2; i >= 0; i--)
            {
                if (n[i].GetType() == typeof(Token))
                    result = GetAtom(n[i]);
                else
                    result = EvaluateOperator((Node)n[i]);

                if (result.Type != AtomType.NUMBER)
                    throw new EvaluatorException(n[i], $"Invalid variable type, expected NUMBER but got {result.Type}.");
                
                acc = new Atom(AtomType.NUMBER, Math.Pow((float)result.Value, (float)acc.Value));
            }

            return acc;
        }

        private Atom EvaluateBinaryOperator<T>(Node node, Func<T, T, T> action)
        {
            List<ISyntaxNode> n = node.GetSyntaxNodes();
            if (n.Count < 2)
                throw new EvaluatorException(node, $"Invalid number of arguments for operator {node._rule}.");

            Atom result;
            Atom acc;

            // Get first element
            if (n[0].GetType() == typeof(Token))
                result = GetAtom(n[0]);
            else
                result = EvaluateOperator((Node)n[0]);

            if (result.Type != AtomType.NUMBER)
                throw new EvaluatorException(n[0], $"Invalid variable type, expected NUMBER but got {result.Type}.");

            acc = result;

            // Iterate through the rest of the elements
            for (int i = 1; i < n.Count; i++)
            {
                if (n[i].GetType() == typeof(Token))
                    result = GetAtom(n[i]);
                else
                    result = EvaluateOperator((Node)n[i]);

                if (result.Type != AtomType.NUMBER)
                    throw new EvaluatorException(n[i], $"Invalid variable type, expected NUMBER but got {result.Type}.");

                acc = new Atom(AtomType.NUMBER, action((T)acc.Value, (T)result.Value));
            }

            return acc;
        }

        private Atom EvaluateUnaryOperator<T>(Node node, Func<T, T> action)
        {
            List<ISyntaxNode> nodes = node.GetSyntaxNodes();
            if(nodes.Count != 1)
                throw new EvaluatorException(node, $"Invalid number of arguments for operator {node._rule}.");
            
            ISyntaxNode n = nodes[0];

            Atom result;

            if (n.GetType() == typeof(Token))
                result = GetAtom(n);
            else
                result = EvaluateOperator((Node)n);

            if (result.Type != AtomType.NUMBER)
                throw new EvaluatorException(n, $"Invalid variable type, expected NUMBER but got {result.Type}.");

            return new Atom(AtomType.NUMBER, action((T)result.Value));
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
