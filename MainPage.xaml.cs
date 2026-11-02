
using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace HesapMakinesi
{
    public partial class MainPage : ContentPage
    {
        double firstNumber = 0;
        string currentOperator = "";
        string currentInput = "0";
        bool isNewNumber = true;

        public MainPage()
        {
            InitializeComponent();

            
            if (Content is IView root)
            {
                AttachButtonEvents(root);
            }
        }

        
        private void AttachButtonEvents(IView view)
        {
            if (view is Button button)
            {
                button.Clicked += OnButtonClicked;
            }
            else if (view is Layout layout)
            {
                foreach (var child in layout.Children)
                {
                    AttachButtonEvents(child);
                }
            }
        }

        
        private void OnButtonClicked(object sender, EventArgs e)
        {
            if (sender is not Button button)
                return;

            string value = button.Text;

            
            if ("0123456789.".Contains(value) && value.Length == 1)
            {
                if (isNewNumber)
                {
                    currentInput = value == "." ? "0." : value;
                    isNewNumber = false;
                }
                else
                {
                    if (value == "." && currentInput.Contains("."))
                        return;

                    if (currentInput == "0" && value != ".")
                        currentInput = value;
                    else
                        currentInput += value;
                }

                DisplayLabel.Text = currentInput;
                return;
            }

           
            if (value == "C")
            {
                currentInput = "0";
                firstNumber = 0;
                currentOperator = "";
                isNewNumber = true;

                DisplayLabel.Text = "0";
                HistoryLabel.Text = "";
                return;
            }

            if (value == "CE")
            {
                currentInput = "0";
                isNewNumber = true;
                DisplayLabel.Text = "0";
                return;
            }

            
            if (value == "⌫")
            {
                if (!isNewNumber && currentInput.Length > 1)
                    currentInput = currentInput.Substring(0, currentInput.Length - 1);
                else
                    currentInput = "0";

                DisplayLabel.Text = currentInput;
                return;
            }

          
            if (value == "+/-")
            {
                double number = GetCurrentNumber();
                currentInput = (-number).ToString(CultureInfo.CurrentCulture);
                DisplayLabel.Text = currentInput;
                return;
            }

            
            if (value == "%")
            {
                currentInput = (GetCurrentNumber() / 100)
                    .ToString(CultureInfo.CurrentCulture);

                DisplayLabel.Text = currentInput;
                return;
            }

            
            if (value == "1/x" || value == "x²" || value == "√x")
            {
                double number = GetCurrentNumber();
                double result;

                if (value == "1/x")
                {
                    if (number == 0)
                    {
                        ShowError("Sıfıra bölünemez!");
                        return;
                    }

                    result = 1 / number;
                }
                else if (value == "x²")
                {
                    result = number * number;
                }
                else
                {
                    if (number < 0)
                    {
                        ShowError("Geçersiz işlem!");
                        return;
                    }

                    result = Math.Sqrt(number);
                }

                currentInput = result.ToString(CultureInfo.CurrentCulture);
                DisplayLabel.Text = currentInput;
                isNewNumber = true;
                return;
            }

            
            if (value == "+" || value == "-" ||
                value == "×" || value == "÷")
            {
                if (currentOperator != "" && !isNewNumber)
                {
                    if (!CalculatePending())
                        return;
                }

                firstNumber = GetCurrentNumber();
                currentOperator = value;
                isNewNumber = true;

                HistoryLabel.Text = firstNumber + " " + value;
                return;
            }

            
            if (value == "=")
            {
                if (currentOperator == "")
                    return;

                double secondNumber = GetCurrentNumber();
                string oldOperator = currentOperator;

                HistoryLabel.Text = firstNumber + " " +
                    oldOperator + " " + secondNumber + " =";

                if (CalculatePending())
                {
                    currentOperator = "";
                }
            }
        }

       
        private double GetCurrentNumber()
        {
            double.TryParse(
                currentInput,
                NumberStyles.Float,
                CultureInfo.CurrentCulture,
                out double number);

            return number;
        }

        
        private bool CalculatePending()
        {
            double secondNumber = GetCurrentNumber();
            double result = 0;

            switch (currentOperator)
            {
                case "+":
                    result = firstNumber + secondNumber;
                    break;

                case "-":
                    result = firstNumber - secondNumber;
                    break;

                case "×":
                    result = firstNumber * secondNumber;
                    break;

                case "÷":
                    if (secondNumber == 0)
                    {
                        ShowError("Sıfıra bölünemez!");
                        return false;
                    }

                    result = firstNumber / secondNumber;
                    break;
            }

            firstNumber = result;

            currentInput = result.ToString(
                "G",
                CultureInfo.CurrentCulture);

            DisplayLabel.Text = currentInput;
            isNewNumber = true;

            return true;
        }

        
        private async void ShowError(string message)
        {
            await DisplayAlert("Hata", message, "Tamam");

            currentInput = "0";
            currentOperator = "";
            firstNumber = 0;
            isNewNumber = true;

            DisplayLabel.Text = "0";
            HistoryLabel.Text = "";
        }
    }
}
