using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading;
using System.Globalization;
using System.Collections.ObjectModel;
using System.IO;

namespace _1_Calculator
{
    public partial class MainWindow : Window
    {
        private Collection<string> computationHistory; 
        public MainWindow()
        { 
            InitializeComponent();
            computationHistory = new Collection<string>();
            Calculator.InputRestriction = 12;
            Calculator.ComputationEnded += (s, b) =>
            {
                computationHistory.Add(Calculator.getTheLastComputation());
                //MessageBox.Show(Calculator.getTheLastComputation());
            };

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            string cont = b.Content.ToString();

            switch (cont)
            {
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                case "0":
                    Calculator.EnterNumber(Int32.Parse(cont));
                    break;
                case ".":
                    Calculator.EnterDot();
                    break;
                case "π":
                    Calculator.EnterPi();
                    break;
                case "⟵":
                    Calculator.EraseLast();
                    break;
                case "C":
                    Calculator.Clear();
                    break;
                case "CE":
                    Calculator.ClearEntry();
                    break;
                case "+":
                    Calculator.ExecuteOperation(Calculator.CalculatorOperationType.Addition);
                    break;
                case "-":
                    Calculator.ExecuteOperation(Calculator.CalculatorOperationType.Substraction);
                    break;
                case "*":
                    Calculator.ExecuteOperation(Calculator.CalculatorOperationType.Multiplying);
                    break;
                case "/":
                    Calculator.ExecuteOperation(Calculator.CalculatorOperationType.Dividing);
                    break;
                case "=":
                    Calculator.Equal();
                    break;
                case "±":
                    Calculator.ChangeSign();
                    break;
                case "n!":
                    Calculator.Factorial();
                    break;
                case "√":
                    Calculator.SquareRoot();
                    break;
                case "x²":
                    Calculator.ToTheSquare();
                    break;
                case "sin":
                    Calculator.Sin();
                    break;
                case "cos":
                    Calculator.Cos();
                    break;
                case "tan":
                    Calculator.Tan();
                    break;
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Calculator.ChangeMeasureMode();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key) {
                case Key.D8:
                    if (Keyboard.IsKeyDown(Key.LeftShift)) Calculator.ExecuteOperation(Calculator.CalculatorOperationType.Multiplying);
                    else Calculator.EnterNumber(8);
                    break;
                case Key.D0:
                case Key.D1:
                case Key.D2:
                case Key.D3:
                case Key.D4:
                case Key.D5:
                case Key.D6:
                case Key.D7:
                case Key.D9:
                    Calculator.EnterNumber((int)e.Key - 34);
                    break;
                case Key.NumPad0:
                case Key.NumPad1:
                case Key.NumPad2:
                case Key.NumPad3:
                case Key.NumPad4:
                case Key.NumPad5:
                case Key.NumPad6:
                case Key.NumPad7:
                case Key.NumPad8:
                case Key.NumPad9:
                    if(Keyboard.IsKeyToggled(Key.NumLock)) Calculator.EnterNumber((int)e.Key - 74);
                    break;
                case Key.OemMinus:
                    if (!Keyboard.IsKeyDown(Key.LeftShift)) Calculator.ExecuteOperation(Calculator.CalculatorOperationType.Substraction);
                    break;
                case Key.OemPlus:
                    if (Keyboard.IsKeyDown(Key.LeftShift)) Calculator.ExecuteOperation(Calculator.CalculatorOperationType.Addition);
                    else Calculator.Equal();
                    break;
                case Key.Back:
                    Calculator.EraseLast();
                    break;
                case Key.Delete:
                    Calculator.ClearEntry();
                    break;
                case Key.Oem2:
                    if (InputLanguageManager.Current.CurrentInputLanguage.ThreeLetterISOLanguageName == "eng" && !Keyboard.IsKeyDown(Key.LeftShift)) Calculator.ExecuteOperation(Calculator.CalculatorOperationType.Dividing);
                    else if (InputLanguageManager.Current.CurrentInputLanguage.ThreeLetterISOLanguageName == "rus" && !Keyboard.IsKeyDown(Key.LeftShift)) Calculator.EnterDot();
                    break;
                case Key.Oem5:
                    if (InputLanguageManager.Current.CurrentInputLanguage.ThreeLetterISOLanguageName == "rus" && Keyboard.IsKeyDown(Key.LeftShift)) Calculator.ExecuteOperation(Calculator.CalculatorOperationType.Dividing);
                    break;
                case Key.OemPeriod:
                    if (InputLanguageManager.Current.CurrentInputLanguage.ThreeLetterISOLanguageName == "eng" && !Keyboard.IsKeyDown(Key.LeftShift)) Calculator.EnterDot();
                    break;
                case Key.Multiply:
                    if (Keyboard.IsKeyToggled(Key.NumLock)) Calculator.ExecuteOperation(Calculator.CalculatorOperationType.Multiplying);
                    break;
                case Key.Divide:
                    if (Keyboard.IsKeyToggled(Key.NumLock)) Calculator.ExecuteOperation(Calculator.CalculatorOperationType.Dividing);
                    break;
                case Key.Add:
                    if (Keyboard.IsKeyToggled(Key.NumLock)) Calculator.ExecuteOperation(Calculator.CalculatorOperationType.Addition);
                    break;
                case Key.Subtract:
                    if (Keyboard.IsKeyToggled(Key.NumLock)) Calculator.ExecuteOperation(Calculator.CalculatorOperationType.Substraction);
                    break;
                case Key.Decimal:
                    if (Keyboard.IsKeyToggled(Key.NumLock)) Calculator.EnterDot();
                    break;
            }
        }
        private void Copy_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetText(tbOut.Text);
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (computationHistory.Count != 0) e.CanExecute = true;
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string logFilePath = Directory.GetCurrentDirectory() + @"\" + "Session_" + DateTime.Now.ToString("dd.MM.yyyy") + 'T' + DateTime.Now.ToString("hh.mm.ss") + ".txt";
            using (StreamWriter streamWriter = new StreamWriter(logFilePath))
            {
                int i = 0;
                foreach (string computation in computationHistory) {
                    i++;
                    streamWriter.WriteLine(i.ToString() + ") " + computation);
                }
                MessageBox.Show("History of computations has been saved in the file: " + logFilePath);
                    
            }
        }
    }
}
