using System.Collections.Generic;
using System;
using System.Linq;

namespace RpnCalculate
{
    public class RpnCalculator
    {
        private ContainerOperator containerOperator;
        private Stack<string> operatorsBuffer = new Stack<string>();
        private List<string> outputBuffer = new List<string>();
        private List<char> operators;
        private List<char> separators;
        private string temp;
        private float operandRight;
        private float operandLeft;
        private string rpnFunction;
        private string resultCalculate;
        private TypeSymbol lastSymbol = TypeSymbol.Unknown;

        public string RpnFunction => rpnFunction;
        public string Result => resultCalculate;

        public RpnCalculator()
        {
            containerOperator = new ContainerOperator();
            containerOperator.AddOperators(new Operator(1, "("));
            containerOperator.AddOperators(new Operator(2, "+"));
            containerOperator.AddOperators(new Operator(2, "-"));
            containerOperator.AddOperators(new Operator(3, "*"));
            containerOperator.AddOperators(new Operator(3, "/"));
            containerOperator.AddOperators(new Operator(4, "^"));
            operators = new List<char>() { '+', '-', '*', '/', '^' };
            separators = new List<char>() { '.', ',' };
        }

        public void Calculate(string function)
        {
            ParseFunctionToRPN(function);
            CalculateFromRPN();
        }

        public void ParseFunctionToRPN(string function)
        {
            outputBuffer.Clear();
            operatorsBuffer.Clear();
            lastSymbol = TypeSymbol.Unknown;
            // Убираем все пробелы их функции
            function.Replace(" ", "");
            // Перебираем каждый симбол входной функции
            foreach (char symbol in function)
            {
                // Если число, то просто записываем
                if (char.IsDigit(symbol))
                {
                    switch (lastSymbol)
                    {
                        case TypeSymbol.Digit:
                        case TypeSymbol.Separator:
                            ChangeDigit(symbol);
                            break;
                        default:
                            AddSymbol(symbol.ToString());
                            break;
                    }
                    lastSymbol = TypeSymbol.Digit;
                    continue;
                }

                if (CheckSeparator(symbol))
                {
                    if (lastSymbol != TypeSymbol.Digit)
                        AddZero();

                    ChangeDigit(symbol);
                    lastSymbol = TypeSymbol.Separator;
                    continue;
                }

                // Если оператор, то проверяем
                if (CheckPriorityOperator(symbol))
                    continue;

                // Открывающую скобку записываем
                if (symbol == '(')
                {
                    operatorsBuffer.Push(symbol.ToString());
                    lastSymbol = TypeSymbol.Bracket;
                    continue;
                }

                // Если закрывающая скобка, то извлекаем до открывающей
                if (symbol == ')')
                {
                    while (operatorsBuffer.Peek() != "(")
                        AddSymbol(operatorsBuffer.Pop());

                    operatorsBuffer.Pop();
                }
            }

            while (operatorsBuffer.Count != 0)
                AddSymbol(operatorsBuffer.Pop());

            rpnFunction = string.Join(" ", outputBuffer);
        }

        private void CalculateFromRPN()
        {
            string op1, op2;
            foreach (string symbol in outputBuffer)
            {
                if (char.IsDigit(symbol[0]))
                {
                    operatorsBuffer.Push(symbol.ToString());
                    continue;
                }

                op1 = operatorsBuffer.Pop();
                op2 = operatorsBuffer.Pop();
                operandRight = op1.Contains(",") ?
                        (float)Convert.ToDouble(op1) :
                        float.Parse(op1, System.Globalization.CultureInfo.InvariantCulture);
                operandLeft = op2.Contains(",") ?
                        (float)Convert.ToDouble(op2) :
                        float.Parse(op2, System.Globalization.CultureInfo.InvariantCulture);

                switch (symbol)
                {
                    case "+":
                        temp = (operandLeft + operandRight).ToString();
                        break;
                    case "-":
                        temp = (operandLeft - operandRight).ToString();
                        break;
                    case "*":
                        temp = (operandLeft * operandRight).ToString();
                        break;
                    case "/":
                        temp = (operandLeft / operandRight).ToString();
                        break;
                    case "^":
                        temp = Math.Pow(operandLeft, operandRight).ToString();
                        break;
                }
                operatorsBuffer.Push(temp);
            }
            resultCalculate = operatorsBuffer.Pop();
        }

        private int GetPriority(string symbol) =>
            containerOperator.GetOperator(symbol).Priority;

        private bool CheckBufferEmptyAndPush(char symbol)
        {
            if (operatorsBuffer.Count == 0)
            {
                operatorsBuffer.Push(symbol.ToString());
                return true;
            }
            return false;
        }

        private bool CheckOperator(char symbol) =>
            operators.Any(op => op == symbol);

        private bool CheckSeparator(char symbol) =>
            separators.Any(sep => sep == symbol);

        private bool CheckPriorityOperator(char symbol)
        {
            if (CheckOperator(symbol))
            {
                if (lastSymbol == TypeSymbol.Bracket || lastSymbol == TypeSymbol.Unknown)
                    AddZero();

                lastSymbol = TypeSymbol.Operator;
                if (CheckBufferEmptyAndPush(symbol))
                    return true;

                int topStackPriority = GetPriority(operatorsBuffer.Peek());
                int symbolPriority = GetPriority(symbol.ToString());

                if (topStackPriority > symbolPriority)
                {
                    operatorsBuffer.Push(symbol.ToString());
                    return true;
                }

                while (topStackPriority >= symbolPriority && operatorsBuffer.Count != 0)
                {
                    AddSymbol(operatorsBuffer.Pop());

                    if (operatorsBuffer.Count == 0)
                        break;
                    topStackPriority = GetPriority(operatorsBuffer.Peek());
                    symbolPriority = GetPriority(symbol.ToString());
                }

                if (CheckBufferEmptyAndPush(symbol))
                    return true;

                topStackPriority = GetPriority(operatorsBuffer.Peek());
                symbolPriority = GetPriority(symbol.ToString());

                if (topStackPriority < symbolPriority)
                {
                    operatorsBuffer.Push(symbol.ToString());
                    return true;
                }
            }
            return false;
        }

        private void AddSymbol(string symbol)
        {
            outputBuffer.Add(symbol.ToString());
        }

        private void ChangeDigit(char symbol)
        {
            outputBuffer[outputBuffer.Count - 1] = outputBuffer.Last() + symbol;
        }

        private void AddZero() =>
            outputBuffer.Add("0");

        private string PopBuffer()
        {
            string symbol = outputBuffer.Last();
            outputBuffer.RemoveAt(outputBuffer.Count - 1);
            return symbol;
        }
    }
}
