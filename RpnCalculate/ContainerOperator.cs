using System.Collections.Generic;
using System.Linq;

namespace RpnCalculate
{
    internal class ContainerOperator
    {
        private List<Operator> operators = new List<Operator>();

        public void AddOperators(Operator op)
        {
            if (!operators.Contains(op))
                operators.Add(op);
        }

        public Operator GetOperator(string symbol)
        {
            return operators.FirstOrDefault(op => op.Equals(symbol));
        }
    }
}
