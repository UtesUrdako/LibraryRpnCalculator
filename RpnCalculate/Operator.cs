namespace RpnCalculate
{
    internal class Operator
    {
        private int priority;
        private string symbol;

        public int Priority => priority;

        public Operator(int priority, string symbol)
        {
            this.priority = priority;
            this.symbol = symbol;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is string symbol)) return false;
            return symbol.Equals(this.symbol);
        }
    }
}
