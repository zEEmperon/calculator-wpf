﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace _1_Calculator
{

    public class Calculator
    {
        public enum CalculatorOperationType { Addition, Substraction, Dividing, Multiplying, None };
        static Calculator()
        {
            InputBuffer = "0";
            OutputBuffer = string.Empty;
            InputRestriction = -1;

            isFirstNumValueSet = false;
            isBlocked = false;
            doesTheInputContainResOrPrevNum = false;
            doesTheInputContainConstantValue = false;
            isUnaryOperation = false;
            doesTheLastOutputContainsParentheses = false;
            isRadians = false;

            firstNumValue = 0;
            secondNumValue = 0;
            unaryBuffer = string.Empty;

            nextOperationType = CalculatorOperationType.None;
            prevOperationType = CalculatorOperationType.None;
        }

        //fields
        private static string _inputBuffer;
        private static string _outputBuffer;
        private static string unaryBuffer;
        private static int _inputRestriction;

        private static bool isFirstNumValueSet;
        private static bool doesTheInputContainResOrPrevNum;
        private static bool doesTheInputContainConstantValue;
        private static bool isBlocked;
        private static bool isUnaryOperation;
        private static bool doesTheLastOutputContainsParentheses;
        private static bool isRadians;

        private static double firstNumValue;
        private static double secondNumValue;
        
        public static event EventHandler InputBufferChanged;
        public static event EventHandler OutputBufferChanged;

        private static CalculatorOperationType nextOperationType;
        private static CalculatorOperationType prevOperationType;

        //private const double PI = 3.1415926535897932384626433832795;
        private const double PI = Math.PI;

        //properties
        public static string InputBuffer
        {
            get
            {
                return _inputBuffer;
            }
            private set
            {
                if (doesTheInputContainResOrPrevNum || doesTheInputContainConstantValue)
                {
                    if (_inputBuffer != string.Empty) value = value.Remove(0, _inputBuffer.Length);
                    if (value != string.Empty && value[0] == '.') value = value.Insert(0, "0");
                    doesTheInputContainResOrPrevNum = false;
                    doesTheInputContainConstantValue = false;
                }

                if (value.Length > 1 && value[0] == '0')
                {
                    if (value[1] != '.')
                        value = value.Substring(1);
                }
                _inputBuffer = value;
                OnInputBufferChanged(EventArgs.Empty);
            }
        }
        public static string OutputBuffer
        {
            get
            {
                return _outputBuffer;
            }
            private set
            {   if (value.Length >= 40) value = "..." + value.Substring(value.Length-40, 40) ;
                _outputBuffer = value;
                OnOutputBufferChanged(EventArgs.Empty);
            }
        }

        //No restriction = -1
        public static int InputRestriction {
            get { return _inputRestriction; }
            set {
                if (value < -1) value = Math.Abs(value);
                _inputRestriction = value;
            }
        }

        //methods
        public static void EnterNumber(int num)
        {
            if (isInputAllowed()) {
                if (isUnaryOperation) {
                    EraseUnaryOperation();
                    InputBuffer = num.ToString();
                } 
                else InputBuffer += num.ToString();
            } 
        }
        public static void EnterDot()
        {
            if (isInputAllowed() && (!InputBuffer.Contains(".") || doesTheInputContainResOrPrevNum || doesTheInputContainConstantValue))
            {
                if (isUnaryOperation)
                {
                    EraseUnaryOperation();
                    InputBuffer = "0.";
                }
                else InputBuffer += '.';
            }
        }
        public static void EnterPi()
        {
            if (isInputAllowed()) {
                if (isUnaryOperation) EraseUnaryOperation();
                doesTheInputContainResOrPrevNum = true;
                if (InputRestriction == -1 || PI.ToString().Length <= InputRestriction) InputBuffer += PI.ToString();
                else InputBuffer += PI.ToString().Substring(0, InputRestriction-1);
                doesTheInputContainConstantValue = true;
            }

        }
        public static void EraseLast()
        {
            if (!doesTheInputContainResOrPrevNum && !isBlocked && !doesTheInputContainConstantValue && !isUnaryOperation)
            { 
                InputBuffer = InputBuffer.Remove(InputBuffer.Length - 1);
                if (InputBuffer.Length == 0 || (InputBuffer.Length == 1 && InputBuffer[0] == '-')) InputBuffer = "0";
            }
        }
        public static void ClearEntry()
        {
            doesTheInputContainConstantValue = false;
            doesTheInputContainResOrPrevNum = false;
            if (isUnaryOperation) EraseUnaryOperation();
            InputBuffer = "0";
            if (isBlocked) {
                OutputBuffer = string.Empty;
                unaryBuffer = string.Empty;
                isBlocked = false;
            }
            
        }
        public static void Clear()
        {

            doesTheInputContainResOrPrevNum = false;
            ClearOutputBuffer();

            isBlocked = false;
            InputBuffer = "0";


        }
        public static void ChangeMeasureMode()
        {
            isRadians = !isRadians;
        }
        public static void Equal() {
            if (!isBlocked) {
                if (doesTheInputContainResOrPrevNum || (nextOperationType == CalculatorOperationType.None && isUnaryOperation)) {

                    doesTheInputContainResOrPrevNum = true;
                    ClearOutputBuffer();

                }
                else if ((prevOperationType != CalculatorOperationType.None || (prevOperationType == CalculatorOperationType.None && nextOperationType != CalculatorOperationType.None)))
                {
                    if (Double.TryParse(InputBuffer, out secondNumValue))
                    {
                        ExecuteOperationImpl(nextOperationType);
                        ClearOutputBuffer();

                    }
                }
            }
        }
        public static void Factorial() {
            if (!isBlocked && !InputBuffer.Contains(".")) {
                int num;
                if (Int32.TryParse(InputBuffer, out num)) {
                    int res = num;
                    if (num > 0)
                    {
                        for (int i = num - 1; i > 0; i--) {
                            res *= i;
                        }
                    }
                    else if (num == 0) {
                        res = 1;
                    }
                    else if (num < 0)
                    {
                        InvokeError();
                        return;
                    }

                    PrintUnaryOperation(num, "factorial");
                    InputBuffer = res.ToString();
                }
            
            }
        }
        public static void ChangeSign() {

            if (!isBlocked)
            {
                double num;
                if (Double.TryParse(InputBuffer, out num))
                {
                    PrintUnaryOperation(num, "negate");
                    InputBuffer = (num*(-1)).ToString();

                }
            }
        }
        public static void SquareRoot() {
            if (!isBlocked) {
                double num;
                if (Double.TryParse(InputBuffer, out num)) {
                    if (num >= 0)
                    {
                        PrintUnaryOperation(num, "sqrt");
                        InputBuffer = Math.Sqrt(num).ToString();
                    }
                    else {
                        InvokeError();
                        return;
                    }
                }
            }
        }
        public static void ToTheSquare() {
            if (!isBlocked) {
                double num;
                if (Double.TryParse(InputBuffer, out num))
                {
                    PrintUnaryOperation(num, "sqr");
                    InputBuffer = Math.Pow(num,4).ToString();
                }
            }
        }
        public static void Sin() {
            if (!isBlocked) {
                double num;
                if (Double.TryParse(InputBuffer, out num))
                {
                    PrintUnaryOperation(num, "sin");
                    if (!isRadians) num = PI * (num / 180);
                    if(doesTheInputContainConstantValue)doesTheInputContainConstantValue = false;
                    InputBuffer = Math.Round(Math.Sin(num), 4).ToString();
                }
            }
        }
        public static void Cos() {
            if (!isBlocked) {
                double num;
                if (Double.TryParse(InputBuffer, out num))
                {
                    PrintUnaryOperation(num, "cos");
                    if (!isRadians) num = PI * (num / 180);
                    if (doesTheInputContainConstantValue) doesTheInputContainConstantValue = false;
                    InputBuffer = Math.Round(Math.Cos(num), 4).ToString();
                }
            }
        }
        public static void Tan() {
            if (!isBlocked) {
                if (!isBlocked)
                {
                    double num;
                    if (Double.TryParse(InputBuffer, out num))
                    {
                        PrintUnaryOperation(num, "tan");
                        if (!isRadians) {
                            num = PI * (num / 180);
                        }
                        num = Math.Tan(num);
                        if (num > 1)
                        {
                            InvokeError();
                            return;
                        }
                        if (doesTheInputContainConstantValue) doesTheInputContainConstantValue = false;
                        InputBuffer = Math.Round(num, 4).ToString();
                    }
                }
            }
        }

        private static void AddNextOperation(in CalculatorOperationType operation, char sign)
        {
            double num;
            if (Double.TryParse(InputBuffer, out num))
            {
                if (!isFirstNumValueSet)
                {
                    firstNumValue = num;
                    isFirstNumValueSet = true;
                }
                else secondNumValue = num;

                string outputValueStr;

                if (nextOperationType != CalculatorOperationType.None && (!doesTheInputContainResOrPrevNum || doesTheInputContainConstantValue))
                {
                    ExecuteOperationImpl(nextOperationType);
                    if (isBlocked) return;

                    if (!isUnaryOperation)
                    {
                        outputValueStr = secondNumValue.ToString();
                    }
                    else {
                        outputValueStr = string.Empty;
                        isUnaryOperation = false;
                        unaryBuffer = string.Empty;
                    }

                    if ((operation == CalculatorOperationType.Multiplying || operation == CalculatorOperationType.Dividing)
                        && (prevOperationType == CalculatorOperationType.Addition || prevOperationType == CalculatorOperationType.Substraction)
                        && !doesTheLastOutputContainsParentheses)
                    {
                        OutputBuffer = "(" + OutputBuffer + outputValueStr + ")" + ' ' + sign + ' ';
                        doesTheLastOutputContainsParentheses = true;
                    }
                    else OutputBuffer += outputValueStr + ' ' + sign + ' ';

                }
                else if (!doesTheInputContainResOrPrevNum || doesTheInputContainConstantValue)
                {
                    if (!isUnaryOperation) OutputBuffer += firstNumValue.ToString() + ' ' + sign + ' ';
                    else {
                        OutputBuffer += " " + sign + " ";
                        isUnaryOperation = false;
                        unaryBuffer = string.Empty;
                    } 

                    if (InputBuffer[InputBuffer.Length - 1] == '.' || (InputBuffer.Length > 1 && InputBuffer[InputBuffer.Length - 2] == '.' && InputBuffer[InputBuffer.Length - 1] == '0')) InputBuffer = firstNumValue.ToString();
                }
                else {
                    if (prevOperationType != CalculatorOperationType.None
                        && (operation == CalculatorOperationType.Substraction || operation == CalculatorOperationType.Addition)
                        && doesTheLastOutputContainsParentheses)
                    {
                        OutputBuffer = (OutputBuffer.Remove(OutputBuffer.Length - 4, 4) + ' ' + sign + ' ').Remove(0, 1);
                        doesTheLastOutputContainsParentheses = false;
                    }
                    else if (prevOperationType != CalculatorOperationType.None
                        && (operation == CalculatorOperationType.Multiplying || operation == CalculatorOperationType.Dividing)
                        && !doesTheLastOutputContainsParentheses)
                    {
                        OutputBuffer = "(" + OutputBuffer.Remove(OutputBuffer.Length - 3, 3) + ")" + ' ' + sign + ' ';
                        doesTheLastOutputContainsParentheses = true;
                    }
                    else {
                        if (OutputBuffer == string.Empty && doesTheInputContainResOrPrevNum) OutputBuffer = firstNumValue.ToString() + " " + sign + " ";
                        else OutputBuffer = OutputBuffer.Remove(OutputBuffer.Length - 2, 2) + sign + ' ';
                    }
                }
               
                nextOperationType = operation;
                doesTheInputContainResOrPrevNum = true;
                doesTheInputContainConstantValue = false;


            }
        }
        private static void ExecuteOperationImpl(CalculatorOperationType op) {
            switch (op) {
                case CalculatorOperationType.Addition:
                    firstNumValue += secondNumValue;
                    break;
                case CalculatorOperationType.Substraction:
                    firstNumValue -= secondNumValue;
                    break;
                case CalculatorOperationType.Dividing:
                    if (secondNumValue == 0)
                    {
                        InvokeError();
                        return;                      
                    }
                    else {
                        firstNumValue /= secondNumValue;
                    }
                    break;
                case CalculatorOperationType.Multiplying:
                    firstNumValue *= secondNumValue;
                    break;
            }
            prevOperationType = nextOperationType;
            nextOperationType = CalculatorOperationType.None;
            unaryBuffer = string.Empty;
            doesTheInputContainConstantValue = false;
            InputBuffer = firstNumValue.ToString();
            doesTheInputContainResOrPrevNum = true;

        }
        public static void ExecuteOperation(CalculatorOperationType op) {
            if (op != CalculatorOperationType.None && !isBlocked)
            {
                switch (op)
                {
                    case CalculatorOperationType.Addition:
                        AddNextOperation(CalculatorOperationType.Addition, '+');
                        break;
                    case CalculatorOperationType.Substraction:
                        AddNextOperation(CalculatorOperationType.Substraction, '-');
                        break;
                    case CalculatorOperationType.Dividing:
                        AddNextOperation(CalculatorOperationType.Dividing, '/');
                        break;
                    case CalculatorOperationType.Multiplying:
                        AddNextOperation(CalculatorOperationType.Multiplying, '*');
                        break;
                }
            }
        }
        private static bool isInputAllowed() {
            return (!isBlocked && (doesTheInputContainResOrPrevNum || (InputRestriction == -1 || InputBuffer.Length+1 <= InputRestriction) || isUnaryOperation));
        }
        private static void EraseUnaryOperation() {
            if (OutputBuffer != string.Empty && nextOperationType != CalculatorOperationType.None) {

                if (unaryBuffer != string.Empty) {
                    OutputBuffer = OutputBuffer.Substring(0, OutputBuffer.Length - (unaryBuffer.Length + 1)) + ' ';
                }
            } 
            else OutputBuffer = string.Empty;
            isUnaryOperation = false;
            unaryBuffer = string.Empty;
        }

        private static void PrintUnaryOperation(double num, string functionName) {
            if (unaryBuffer != string.Empty)
            {
                string tempBuffer = unaryBuffer;
                EraseUnaryOperation();
                unaryBuffer = functionName + "(" + tempBuffer + ")";
            }
            else unaryBuffer = functionName + "(" + num.ToString() + ")";

            doesTheInputContainResOrPrevNum = false;
            isUnaryOperation = true;
            OutputBuffer += unaryBuffer;
        }

        private static void InvokeError() {
            prevOperationType = CalculatorOperationType.None;
            nextOperationType = CalculatorOperationType.None;
            doesTheInputContainResOrPrevNum = false;
            isFirstNumValueSet = false;
            isBlocked = true;
            InputBuffer = "Error";
        }

        private static void ClearOutputBuffer() {
            isFirstNumValueSet = false;
            doesTheInputContainConstantValue = false;
            isUnaryOperation = false;
            OutputBuffer = string.Empty;
            unaryBuffer = string.Empty;
            doesTheLastOutputContainsParentheses = false;
            nextOperationType = CalculatorOperationType.None;
            prevOperationType = CalculatorOperationType.None;
        }
        protected static void OnInputBufferChanged(EventArgs e) => InputBufferChanged?.Invoke(null, e);
        protected static void OnOutputBufferChanged(EventArgs e) => OutputBufferChanged?.Invoke(null, e);
       
    }
}
