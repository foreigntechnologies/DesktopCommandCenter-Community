using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class CalculadoraViewModel : ObservableObject
{
    [ObservableProperty]
    private int _selectedTabIndex = 0;

    [ObservableProperty]
    private string _currentEntry = "0";

    [ObservableProperty]
    private string _expressionDisplay = "";

    private string _currentOperator = "";
    private double _leftOperand = 0;
    private bool _isNewEntry = true;
    
    // Scientific Calculator properties
    [ObservableProperty]
    private string _scientificEntry = "0";

    [ObservableProperty]
    private string _scientificExpressionDisplay = "";

    // Physics Calculator properties
    [ObservableProperty]
    private int _selectedPhysicsFormulaIndex = 0;
    [ObservableProperty]
    private string _physicsParam1 = "";
    [ObservableProperty]
    private string _physicsParam2 = "";
    [ObservableProperty]
    private string _physicsResult = "";

    // Chemistry Calculator properties
    [ObservableProperty]
    private int _selectedChemistryFormulaIndex = 0;
    [ObservableProperty]
    private string _chemParam1 = "";
    [ObservableProperty]
    private string _chemParam2 = "";
    [ObservableProperty]
    private string _chemResult = "";

    public ObservableCollection<string> ScientificFunctions { get; } = new()
    {
        "Fórmula de Bhaskara",
        "Trigonometria (Sen, Cos, Tan)",
        "Logaritmos",
        "Fatoriais"
    };

    public ObservableCollection<string> PhysicsFormulas { get; } = new()
    {
        "Velocidade Média (v = Δs/Δt)",
        "Segunda Lei de Newton (F = m.a)",
        "Equivalência Massa-Energia (E = mc²)",
        "Energia Cinética (Ec = mv²/2)"
    };

    public ObservableCollection<string> ChemistryFormulas { get; } = new()
    {
        "Cálculo Estequiométrico",
        "Massa Molar",
        "Lei dos Gases Ideais (PV=nRT)",
        "Tabela Periódica Interativa"
    };

    public CalculadoraViewModel()
    {
        // Initialization logic for the calculator
    }

    [RelayCommand]
    private void Digit(string digit)
    {
        if (_isNewEntry && digit != "(" && digit != ")")
        {
            CurrentEntry = digit;
            _isNewEntry = false;
        }
        else
        {
            CurrentEntry = CurrentEntry == "0" && digit != "," && digit != "(" && digit != ")" ? digit : CurrentEntry + digit;
            _isNewEntry = false;
        }
    }

    [RelayCommand]
    private void Operator(string op)
    {
        if (_isNewEntry) _isNewEntry = false;
        CurrentEntry += op;
    }

    [RelayCommand]
    private void Evaluate()
    {
        try
        {
            var parser = new MathParser();
            double result = parser.Parse(CurrentEntry);
            ExpressionDisplay = CurrentEntry + " =";
            CurrentEntry = result.ToString();
            _isNewEntry = true;
        }
        catch
        {
            CurrentEntry = "Erro";
            _isNewEntry = true;
        }
    }

    [RelayCommand]
    private void ClearAll()
    {
        CurrentEntry = "0";
        ExpressionDisplay = "";
        _isNewEntry = true;
    }

    [RelayCommand]
    private void ClearEntry()
    {
        CurrentEntry = "0";
        _isNewEntry = true;
    }

    [RelayCommand]
    private void Backspace()
    {
        if (_isNewEntry || CurrentEntry.Length <= 1)
        {
            CurrentEntry = "0";
            _isNewEntry = true;
        }
        else
        {
            CurrentEntry = CurrentEntry.Substring(0, CurrentEntry.Length - 1);
        }
    }

    [RelayCommand]
    private void Negate()
    {
        if (_isNewEntry) _isNewEntry = false;
        if (CurrentEntry.StartsWith("-"))
            CurrentEntry = CurrentEntry.Substring(1);
        else if (CurrentEntry != "0")
            CurrentEntry = "-" + CurrentEntry;
    }

    [RelayCommand]
    private void CalculatePercent()
    {
        try
        {
            var parser = new MathParser();
            double result = parser.Parse(CurrentEntry) / 100.0;
            CurrentEntry = result.ToString();
            _isNewEntry = true;
        }
        catch { }
    }

    [RelayCommand]
    private void ScientificFunction(string funcName)
    {
        if (_isNewEntry)
        {
            CurrentEntry = "";
            _isNewEntry = false;
        }
        if (CurrentEntry == "0") CurrentEntry = "";
        
        switch (funcName)
        {
            case "sin": CurrentEntry += "sin("; break;
            case "cos": CurrentEntry += "cos("; break;
            case "tan": CurrentEntry += "tan("; break;
            case "sqrt": CurrentEntry += "sqrt("; break;
            case "log": CurrentEntry += "log("; break;
            case "ln": CurrentEntry += "ln("; break;
            case "pi": CurrentEntry += "pi"; break;
            case "e": CurrentEntry += "e"; break;
            case "sqr": CurrentEntry += "^2"; break;
        }
    }

    public class MathParser
    {
        private string _expr = "";
        private int _pos;

        public double Parse(string expression)
        {
            _expr = expression.Replace(" ", "").Replace(",", ".").Replace("×", "*").Replace("÷", "/");
            _pos = 0;
            return ParseExpression();
        }

        private double ParseExpression()
        {
            double result = ParseTerm();
            while (_pos < _expr.Length)
            {
                char op = _expr[_pos];
                if (op != '+' && op != '-') break;
                _pos++;
                double term = ParseTerm();
                if (op == '+') result += term;
                else result -= term;
            }
            return result;
        }

        private double ParseTerm()
        {
            double result = ParsePower();
            while (_pos < _expr.Length)
            {
                char op = _expr[_pos];
                if (op != '*' && op != '/') break;
                _pos++;
                double factor = ParsePower();
                if (op == '*') result *= factor;
                else result /= factor;
            }
            return result;
        }

        private double ParsePower()
        {
            double result = ParseFactor();
            while (_pos < _expr.Length && _expr[_pos] == '^')
            {
                _pos++;
                double exponent = ParseFactor();
                result = System.Math.Pow(result, exponent);
            }
            return result;
        }

        private double ParseFactor()
        {
            if (_pos >= _expr.Length) return 0;
            int startPos = _pos;
            if (_expr[_pos] == '+' || _expr[_pos] == '-')
            {
                _pos++;
                double factor = ParseFactor();
                return _expr[startPos] == '+' ? factor : -factor;
            }

            if (_expr[_pos] == '(')
            {
                _pos++;
                double result = ParseExpression();
                if (_pos < _expr.Length && _expr[_pos] == ')') _pos++;
                return result;
            }

            if (char.IsLetter(_expr[_pos]))
            {
                while (_pos < _expr.Length && char.IsLetter(_expr[_pos])) _pos++;
                string func = _expr.Substring(startPos, _pos - startPos);
                if (func.ToLower() == "pi") return System.Math.PI;
                if (func.ToLower() == "e") return System.Math.E;
                
                double arg = ParseFactor();
                return func.ToLower() switch
                {
                    "sin" => System.Math.Sin(arg),
                    "cos" => System.Math.Cos(arg),
                    "tan" => System.Math.Tan(arg),
                    "log" => System.Math.Log10(arg),
                    "ln" => System.Math.Log(arg),
                    "sqrt" => System.Math.Sqrt(arg),
                    _ => 0
                };
            }

            while (_pos < _expr.Length && (char.IsDigit(_expr[_pos]) || _expr[_pos] == '.')) _pos++;
            if (_pos > startPos)
            {
                string numStr = _expr.Substring(startPos, _pos - startPos);
                double.TryParse(numStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double result);
                return result;
            }
            return 0;
        }
    }

    [RelayCommand]
    private void CalculatePhysics()
    {
        try
        {
            if (SelectedPhysicsFormulaIndex == 0) // Velocidade Media
            {
                var ds = double.Parse(PhysicsParam1);
                var dt = double.Parse(PhysicsParam2);
                PhysicsResult = $"{ds / dt:F2} m/s";
            }
            else if (SelectedPhysicsFormulaIndex == 1) // F = m.a
            {
                var m = double.Parse(PhysicsParam1);
                var a = double.Parse(PhysicsParam2);
                PhysicsResult = $"{m * a:F2} N";
            }
            else if (SelectedPhysicsFormulaIndex == 2) // E = mc2
            {
                var m = double.Parse(PhysicsParam1);
                var c = 299792458; // m/s
                PhysicsResult = $"{m * Math.Pow(c, 2):E2} Joules";
            }
            else if (SelectedPhysicsFormulaIndex == 3) // Ec = mv2/2
            {
                var m = double.Parse(PhysicsParam1);
                var v = double.Parse(PhysicsParam2);
                PhysicsResult = $"{(m * Math.Pow(v, 2)) / 2:F2} Joules";
            }
        }
        catch { PhysicsResult = "Entrada Inválida"; }
    }

    [RelayCommand]
    private void CalculateChemistry()
    {
        try
        {
            if (SelectedChemistryFormulaIndex == 0) // Massa Molar Basica
            {
                PhysicsResult = "Módulo de Tabela Periódica em breve";
            }
            else if (SelectedChemistryFormulaIndex == 2) // PV=nRT (Calcula P)
            {
                var v = double.Parse(ChemParam1);
                var t = double.Parse(ChemParam2);
                var r = 0.082; // atm L / mol K
                var n = 1; // 1 mol
                ChemResult = $"{(n * r * t) / v:F2} atm";
            }
        }
        catch { ChemResult = "Entrada Inválida"; }
    }
}
