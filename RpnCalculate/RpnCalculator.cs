using System.Collections.Generic;
using System;
using System.Linq;

namespace RpnCalculate
{
    public class RpnCalculator
    {
        private ContainerOperator containerOperator;
        private string inputString = "";
        private Stack<string> operatorsBuffer = new Stack<string>();
        private List<char> operators;
        private string temp;
        private int operandRight;
        private int operandLeft;
        private string rpnFunction;
        private string resultCalculate;

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
        }

        public void Calculate(string function)
        {
            ParseFunctionToRPN(function);
            CalculateFromRPN();
        }

        public void ParseFunctionToRPN(string function)
        {
            inputString = "";
            // Убираем все пробелы их функции
            function.Replace(" ", "");
            // Перебираем каждый симбол входной функции
            foreach (char symbol in function)
            {
                // Если число, то просто записываем
                if (char.IsDigit(symbol))
                {
                    inputString += symbol;
                    continue;
                }

                // Если оператор, то проверяем
                if (CheckOperator(symbol))
                {
                    if (CheckBufferEmpty(symbol))
                        continue;

                    int topStackPriority = GetPriority(operatorsBuffer.Peek());
                    int symbolPriority = GetPriority(symbol.ToString());

                    if (topStackPriority > symbolPriority)
                    {
                        operatorsBuffer.Push(symbol.ToString());
                        continue;
                    }

                    while (topStackPriority >= symbolPriority && operatorsBuffer.Count != 0)
                    {
                        inputString += operatorsBuffer.Pop();

                        if (operatorsBuffer.Count == 0)
                            break;
                        topStackPriority = GetPriority(operatorsBuffer.Peek());
                        symbolPriority = GetPriority(symbol.ToString());
                    }

                    if (CheckBufferEmpty(symbol))
                        continue;

                    topStackPriority = GetPriority(operatorsBuffer.Peek());
                    symbolPriority = GetPriority(symbol.ToString());

                    if (topStackPriority < symbolPriority)
                    {
                        operatorsBuffer.Push(symbol.ToString());
                        continue;
                    }
                }

                // Открывающую скобку записываем
                if (symbol == '(')
                {
                    operatorsBuffer.Push(symbol.ToString());
                    continue;
                }

                // Если закрывающая скобка, то извлекаем до открывающей
                if (symbol == ')')
                {
                    while (operatorsBuffer.Peek() != "(")
                        inputString += operatorsBuffer.Pop();

                    operatorsBuffer.Pop();
                }
            }

            while (operatorsBuffer.Count != 0)
                inputString += operatorsBuffer.Pop();

            rpnFunction = inputString;
        }

        private void CalculateFromRPN()
        {
            foreach (char symbol in inputString)
            {
                if (char.IsDigit(symbol))
                {
                    operatorsBuffer.Push(symbol.ToString());
                    continue;
                }

                switch (symbol)
                {
                    case '+':
                        temp = (Int32.Parse(operatorsBuffer.Pop()) + Int32.Parse(operatorsBuffer.Pop())).ToString();
                        break;
                    case '-':
                        operandRight = Int32.Parse(operatorsBuffer.Pop());
                        operandLeft = Int32.Parse(operatorsBuffer.Pop());
                        temp = (operandLeft - operandRight).ToString();
                        break;
                    case '*':
                        temp = (Int32.Parse(operatorsBuffer.Pop()) * Int32.Parse(operatorsBuffer.Pop())).ToString();
                        break;
                    case '/':
                        operandRight = Int32.Parse(operatorsBuffer.Pop());
                        operandLeft = Int32.Parse(operatorsBuffer.Pop());
                        temp = (operandLeft / operandRight).ToString();
                        break;
                    case '^':
                        operandRight = Int32.Parse(operatorsBuffer.Pop());
                        operandLeft = Int32.Parse(operatorsBuffer.Pop());
                        temp = Math.Pow(operandLeft, operandRight).ToString();
                        break;
                }
                operatorsBuffer.Push(temp);
            }
            resultCalculate = operatorsBuffer.Pop();
        }

        private int GetPriority(string symbol) =>
            containerOperator.GetOperator(symbol).Priority;

        private bool CheckBufferEmpty(char symbol)
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
    }
}
