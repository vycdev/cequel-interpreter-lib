using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter_lib.Parser
{
    public enum ERule
    {
        ROOT, 

        EXPRESSION,
        PRIMARY,

        BITWISE_LEFT_SHIFT, SUBSEQUENT_BITWISE_LEFT_SHIFT,
        BITWISE_RIGHT_SHIFT, SUBSEQUENT_BITWISE_RIGHT_SHIFT,
        SUM, SUBSEQUENT_SUM,
        SUBTRACT, SUBSEQUENT_SUBTRACT,
        MULTIPLY, SUBSEQUENT_MULTIPLY,
        DIVIDE, SUBSEQUENT_DIVIDE,
        MODULUS, SUBSEQUENT_MODULUS,
        POWER, SUBSEQUENT_POWER,
        FLOOR,

        GROUP,
    }
}
