using Interpreter_lib.Parser;
using Interpreter_lib.Tokenizer;
using System.Diagnostics;

namespace Interpreter_lib.Evaluator;

public enum AtomType
{
    NUMBER,
    STRING
}

public class Atom(AtomType type, object value)
{
    public AtomType Type = type;
    public object Value = value;
}

public class Evaluator(DateTime? StartDateTime = null, TimeSpan? TimeSpanLimit = null)
{
    public Dictionary<string, Atom> Variables { get; set; } = [];
    public string Output { get; set; } = string.Empty;

    public void Evaluate(Node node)
    {
        CheckTimeLimit(node);

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
                throw new EvaluatorException(node, "Invalid node type.");
        }
    }

    // ROOT 
    public void EvaluateRoot(Node node)
    {
        CheckTimeLimit(node);

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
        // Get all nodes
        List<ISyntaxNode> nodes = node.GetSyntaxNodes();

        // Get the first node
        Node condition = (Node)nodes[0];

        // Loop until the condition is false
        while ((float)(EvaluateOperator(condition).Value) == 1)
        {
            CheckTimeLimit(node);

            foreach (var n in nodes.Skip(1))
            {
                if (n.GetType() == typeof(Node))
                    Evaluate((Node)n);
                else
                    throw new EvaluatorException(node, "Invalid node type, received token instead.");
            }
        }
    }

    // DO_WHILE_LOOP,
    void EvaluateDoWhileLoop(Node node)
    {
        // Get all nodes
        List<ISyntaxNode> nodes = node.GetSyntaxNodes();

        // Get last node
        Node condition = (Node)nodes[^1];

        // Loop until the condition is false
        do
        {
            CheckTimeLimit(node);

            foreach (var n in nodes.Take(nodes.Count - 1))
            {
                if (n.GetType() == typeof(Node))
                    Evaluate((Node)n);
                else
                    throw new EvaluatorException(node, "Invalid node type, received token instead.");
            }
        } while ((float)(EvaluateOperator(condition).Value) == 1);
    }

    // REPEAT_UNTIL_LOOP,
    void EvaluateRepeatUntilLoop(Node node)
    {
        // Get all nodes
        List<ISyntaxNode> nodes = node.GetSyntaxNodes();

        // Get last node
        Node condition = (Node)nodes[^1];

        // Loop until condition is true
        do
        {
            CheckTimeLimit(node);

            foreach (ISyntaxNode? n in nodes.Take(nodes.Count - 1))
            {
                if (n.GetType() == typeof(Node))
                    Evaluate((Node)n);
                else
                    throw new EvaluatorException(node, "Invalid node type, received token instead.");
            }
        } while ((float)(EvaluateOperator(condition).Value) == 0);
    }

    // FOR_LOOP,
    void EvaluateForLoop(Node node)
    {
        // Get all nodes
        List<ISyntaxNode> nodes = node.GetSyntaxNodes();

        // Get first node as an assignment
        Node assignment = (Node)nodes[0];

        // Get second node as a condition
        Node finalExpression = (Node)nodes[1];

        // Check if third node is a step
        Node? stepExpresssion = null;
        if (nodes.Count >= 3 && ((Node)nodes[2])._rule == ERule.FOR_LOOP_STEP)
            stepExpresssion = (Node)((Node)nodes[2]).GetSyntaxNodes()[0];

        // Evaluate the assignment
        string variableKey = EvaluateAssignment(assignment);

        // Get the value of the variable
        if (!Variables.TryGetValue(variableKey, out Atom? value))
            throw new EvaluatorException(node, "Variable was used before declaration.");

        // Evaluate the step if it exists
        Atom? firstStep = null;
        if (stepExpresssion != null)
        {
            firstStep = EvaluateOperator(stepExpresssion);
            if (firstStep.Type != AtomType.NUMBER)
                throw new EvaluatorException(node, "Invalid step type, expected NUMBER but got STRING.");
        }


        // Loop until the condition is false
        while (firstStep == null || (float)firstStep.Value > 0
            ? (float)(EvaluateOperator(finalExpression).Value) > (float)Variables[variableKey].Value
            : (float)(EvaluateOperator(finalExpression).Value) < (float)Variables[variableKey].Value)
        {
            CheckTimeLimit(node);

            foreach (var n in nodes.Skip(stepExpresssion == null ? 2 : 3))
            {
                if (n.GetType() == typeof(Node))
                    Evaluate((Node)n);
                else
                    throw new EvaluatorException(node, "Invalid node type, received token instead.");
            }

            // Evaluate the step
            if (stepExpresssion != null)
            {
                Atom result = EvaluateOperator(stepExpresssion);
                if (result.Type != AtomType.NUMBER)
                    throw new EvaluatorException(node, "Invalid step type, expected NUMBER but got STRING.");

                // Get the value of the variable
                if (value.Type == AtomType.NUMBER)
                {
                    Variables[variableKey] = new Atom(AtomType.NUMBER, (float)Variables[variableKey].Value + (float)result.Value);
                }
                else
                {
                    throw new EvaluatorException(node, "Invalid variable type, expected NUMBER but got STRING.");
                }
            }
            else
            {
                // Get the value of the variable
                if (value.Type == AtomType.NUMBER)
                {
                    Variables[variableKey] = new Atom(AtomType.NUMBER, (float)Variables[variableKey].Value + 1);
                }
                else
                {
                    throw new EvaluatorException(node, "Invalid variable type, expected NUMBER but got STRING.");
                }
            }
        }
    }

    // IF_STATEMENT, ELSE_STATEMENT,
    void EvaluateIfStatement(Node node)
    {
        // Get all nodes
        List<ISyntaxNode> nodes = node.GetSyntaxNodes();

        // Get the first node which is the condition
        Node condition = (Node)nodes[0];

        // Get the last node and check if it's and else statement
        Node? elseStatement = null;
        if (nodes.Count > 1 && ((Node)nodes[^1])._rule == ERule.ELSE_STATEMENT)
            elseStatement = (Node)nodes[^1];

        // Check if the condition is true
        if ((float)(EvaluateOperator(condition).Value) == 1)
        {
            // Evaluate the if statement
            foreach (var n in nodes.Skip(1).Take(elseStatement == null ? nodes.Count - 1 : nodes.Count - 2))
            {
                if (n.GetType() == typeof(Node))
                    Evaluate((Node)n);
                else
                    throw new EvaluatorException(node, "Invalid node type, received token instead.");
            }
        }
        else if (elseStatement != null)
        {
            // Evaluate the else statement
            foreach (var n in elseStatement.GetSyntaxNodes())
            {
                if (n.GetType() == typeof(Node))
                    Evaluate((Node)n);
                else
                    throw new EvaluatorException(node, "Invalid node type, received token instead.");
            }
        }
    }

    // PRINT,
    void EvaluatePrint(Node node)
    {
        // Get the nodes
        List<ISyntaxNode> nodes = node.GetSyntaxNodes();

        // Evaluate each node and print the result
        if (nodes.Count > 0)
            Write("\n");

        foreach (var n in nodes)
        {
            Atom result = EvaluateOperator((Node)n);

            Write(result.Value.ToString() ?? string.Empty);
        }
    }

    // READ,
    void EvaluateRead(Node node)
    {
        throw new EvaluatorException(node, "Read instruction has not been implemented yet.");
    }

    // ASSIGNMENT,
    string EvaluateAssignment(Node node)
    {
        // Get the nodes
        List<ISyntaxNode> nodes = node.GetSyntaxNodes();

        // Get the first node which is a token
        Token variable = (Token)nodes[0];

        // Get the second node which is an expression
        Node expression = (Node)nodes[1];

        // Evaluate the expression
        Atom result = EvaluateOperator(expression);

        // Add the variable to the dictionary
        Variables[variable.Value] = result;

        return variable.Value;
    }

    Atom EvaluateOperator(Node node)
    {
        CheckTimeLimit(node);

        return node._rule switch
        {
            ERule.LOGICAL_OR => EvaluateBinaryOperator(node, (x, y) => (x == 1 || y == 1) ? 1 : 0),
            ERule.LOGICAL_AND => EvaluateBinaryOperator(node, (x, y) => (x == 1 && y == 1) ? 1 : 0),
            ERule.BITWISE_OR => EvaluateBinaryOperator(node, (x, y) => (int)x | (int)y),
            ERule.BITWISE_XOR => EvaluateBinaryOperator(node, (x, y) => (int)x ^ (int)y),
            ERule.BITWISE_AND => EvaluateBinaryOperator(node, (x, y) => (int)x & (int)y),
            ERule.NOT_EQUAL => EvaluateBinaryOperator(node, (x, y) => (x != y) ? 1 : 0),
            ERule.EQUAL => EvaluateBinaryOperator(node, (x, y) => (x == y) ? 1 : 0),
            ERule.GREATER_THAN_EQUAL => EvaluateBinaryOperator(node, (x, y) => (x >= y) ? 1 : 0),
            ERule.GREATER_THAN => EvaluateBinaryOperator(node, (x, y) => (x > y) ? 1 : 0),
            ERule.LESS_THAN_EQUAL => EvaluateBinaryOperator(node, (x, y) => (x <= y) ? 1 : 0),
            ERule.LESS_THAN => EvaluateBinaryOperator(node, (x, y) => (x < y) ? 1 : 0),
            ERule.BITWISE_RIGHT_SHIFT => EvaluateBinaryOperator(node, (x, y) => (int)x >> (int)y),
            ERule.BITWISE_LEFT_SHIFT => EvaluateBinaryOperator(node, (x, y) => (int)x << (int)y),
            ERule.SUM => EvaluateSum(node),
            ERule.SUBTRACT => EvaluateBinaryOperator(node, (x, y) => x - y),
            ERule.MULTIPLY => EvaluateBinaryOperator(node, (x, y) => x * y),
            ERule.DIVIDE => EvaluateBinaryOperator(node, (x, y) => x / y),
            ERule.MODULUS => EvaluateBinaryOperator(node, (x, y) => x % y),
            ERule.POWER => EvaluatePower(node),
            ERule.UNARY_MINUS => EvaluateUnaryOperator(node, x => -x),
            ERule.UNARY_PLUS => EvaluateUnaryOperator(node, x => +x),
            ERule.UNARY_NOT => EvaluateUnaryOperator(node, x => x == 0 ? 1 : 0),
            ERule.UNARY_BITWISE_NOT => EvaluateUnaryOperator(node, x => ~(int)x),
            ERule.FLOOR => EvaluateUnaryOperator(node, x => (int)Math.Floor(x)),
            ERule.EXPRESSION => EvaluateExpression(node),
            _ => throw new EvaluatorException(node, "Invalid operator."),
        };
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
                if (acc.Type == AtomType.NUMBER)
                    acc = new Atom(AtomType.NUMBER, (float)acc.Value + (float)result.Value);
                else
                    acc = new Atom(AtomType.STRING, (string)acc.Value + (float)result.Value);
            else
                if (acc.Type == AtomType.NUMBER)
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
        if (n.Count < 2)
            throw new EvaluatorException(node, $"Invalid number of arguments for operator {node._rule}.");

        Atom result;
        Atom acc;

        // Get last element
        if (n[^1].GetType() == typeof(Token))
            result = GetAtom(n[^1]);
        else
            result = EvaluateOperator((Node)n[^1]);

        if (result.Type != AtomType.NUMBER)
            throw new EvaluatorException(n[^1], $"Invalid variable type, expected NUMBER but got {result.Type}.");

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

            acc = new Atom(AtomType.NUMBER, (float)Math.Pow((float)result.Value, (float)acc.Value));
        }

        return acc;
    }

    private Atom EvaluateBinaryOperator(Node node, Func<float, float, float> action)
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

            acc = new Atom(AtomType.NUMBER, action((float)acc.Value, (float)result.Value));
        }

        return acc;
    }

    private Atom EvaluateUnaryOperator(Node node, Func<float, float> action)
    {
        List<ISyntaxNode> nodes = node.GetSyntaxNodes();
        if (nodes.Count != 1)
            throw new EvaluatorException(node, $"Invalid number of arguments for operator {node._rule}.");

        ISyntaxNode n = nodes[0];

        Atom result;

        if (n.GetType() == typeof(Token))
            result = GetAtom(n);
        else
            result = EvaluateOperator((Node)n);

        if (result.Type != AtomType.NUMBER)
            throw new EvaluatorException(n, $"Invalid variable type, expected NUMBER but got {result.Type}.");

        return new Atom(AtomType.NUMBER, action((float)result.Value));
    }

    #endregion

    #region HELPER FUNCTIONS

    private Atom GetAtom(ISyntaxNode node)
    {
        if (node.GetType() == typeof(Node))
            throw new EvaluatorException(node, "Cannot get variable of type Node.");

        Token token = (Token)node;

        if (token.Type == EToken.IDENTIFIER)
        {
            if (Variables.TryGetValue(token.Value, out Atom? value))
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

    private void Write(string text)
    {
        if (Debugger.IsAttached)
            Console.Write(text);

        Output += text;
    }

    private void CheckTimeLimit(ISyntaxNode node)
    {
        if (StartDateTime == null || TimeSpanLimit == null)
            return;

        if (DateTime.Now - StartDateTime > TimeSpanLimit)
            throw new EvaluatorException(node, $"Time limit of {TimeSpanLimit} exceeded.");
    }

    #endregion
}

