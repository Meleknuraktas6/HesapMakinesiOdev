
using System;
using System.Globalization;

namespace HesapMakinesi
{
    public partial class ScientificPage : ContentPage
    {
        double firstNumber = 0;
        double secondNumber = 0;
        string currentOperator = "";
        string currentInput = "0";
        bool isNewNumber = true;

        public ScientificPage()
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

                ScientificDisplayLabel.Text = currentInput;
                return;
            }

          
            if (value == "C")
            {
                currentInput = "0";
                firstNumber = 0;
                secondNumber = 0;
                currentOperator = "";
                isNewNumber = true;

                ScientificDisplayLabel.Text = "0";
                ScientificHistoryLabel.Text = "";
                return;
            }

            if (value == "CE")
            {
                currentInput = "0";
                isNewNumber = true;
                ScientificDisplayLabel.Text = "0";
                return;
            }

            if (value == "⌫")
            {
                if (!isNewNumber && currentInput.Length > 1)
                    currentInput = currentInput.Substring(0, currentInput.Length - 1);
                else
                    currentInput = "0";

                ScientificDisplayLabel.Text = currentInput;
                return;
            }

           
            if (value == "+/-")
            {
                currentInput = (-GetCurrentNumber())
                    .ToString(CultureInfo.CurrentCulture);

                ScientificDisplayLabel.Text = currentInput;
                return;
            }

            
            if (value == "xʸ")
            {
                firstNumber = GetCurrentNumber();
                currentOperator = "^";
                isNewNumber = true;

                ScientificHistoryLabel.Text = firstNumber + " ^";
                return;
            }

            
            double number = GetCurrentNumber();
            double result = 0;

            switch (value)
            {
                case "sin":
                    result = Math.Sin(number * Math.PI / 180);
                    break;

                case "cos":
                    result = Math.Cos(number * Math.PI / 180);
                    break;

                case "tan":
                    if (Math.Abs(Math.Cos(number * Math.PI / 180)) < 0.0000001)
                    {
                        ShowError("Bu açı için tanjant tanımsızdır!");
                        return;
                    }

                    result = Math.Tan(number * Math.PI / 180);
                    break;

                case "log":
                    if (number <= 0)
                    {
                        ShowError("Logaritma için sayı sıfırdan büyük olmalıdır!");
                        return;
                    }

                    result = Math.Log10(number);
                    break;

                case "ln":
                    if (number <= 0)
                    {
                        ShowError("Doğal logaritma için sayı sıfırdan büyük olmalıdır!");
                        return;
                    }

                    result = Math.Log(number);
                    break;

                case "√x":
                    if (number < 0)
                    {
                        ShowError("Negatif sayının karekökü alınamaz!");
                        return;
                    }

                    result = Math.Sqrt(number);
                    break;

                case "x²":
                    result = number * number;
                    break;

                case "+":
                case "-":
                case "×":
                case "÷":
                    firstNumber = number;
                    currentOperator = value;
                    isNewNumber = true;

                    ScientificHistoryLabel.Text = firstNumber + " " + value;
                    return;

                case "=":
                    if (currentOperator == "")
                        return;

                    secondNumber = number;

                    if (currentOperator == "+")
                        result = firstNumber + secondNumber;
                    else if (currentOperator == "-")
                        result = firstNumber - secondNumber;
                    else if (currentOperator == "×")
                        result = firstNumber * secondNumber;
                    else if (currentOperator == "÷")
                    {
                        if (secondNumber == 0)
                        {
                            ShowError("Sıfıra bölünemez!");
                            return;
                        }

                        result = firstNumber / secondNumber;
                    }
                    else if (currentOperator == "^")
                        result = Math.Pow(firstNumber, secondNumber);

                    ScientificHistoryLabel.Text =
                        firstNumber + " " + currentOperator +
                        " " + secondNumber + " =";

                    currentOperator = "";
                    break;

                default:
                    return;
            }

           
            if (double.IsNaN(result) || double.IsInfinity(result))
            {
                ShowError("Geçersiz işlem sonucu!");
                return;
            }

            currentInput = result.ToString(
                "G",
                CultureInfo.CurrentCulture);

            ScientificDisplayLabel.Text = currentInput;
            isNewNumber = true;
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

        
        private async void ShowError(string message)
        {
            await DisplayAlert("Hata", message, "Tamam");

            currentInput = "0";
            firstNumber = 0;
            secondNumber = 0;
            currentOperator = "";
            isNewNumber = true;

            ScientificDisplayLabel.Text = "0";
            ScientificHistoryLabel.Text = "";
        }
    }
}
