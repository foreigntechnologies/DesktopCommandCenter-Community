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
        if (_isNewEntry)
        {
            CurrentEntry = digit;
            _isNewEntry = false;
        }
        else
        {
            if (digit == "," && CurrentEntry.Contains(",")) return;
            CurrentEntry = CurrentEntry == "0" && digit != "," ? digit : CurrentEntry + digit;
        }
    }

    [RelayCommand]
    private void Operator(string op)
    {
        if (!_isNewEntry && !string.IsNullOrEmpty(_currentOperator))
        {
            Evaluate();
        }

        if (double.TryParse(CurrentEntry, out var val))
        {
            _leftOperand = val;
        }

        _currentOperator = op;
        ExpressionDisplay = $"{_leftOperand} {op}";
        _isNewEntry = true;
    }

    [RelayCommand]
    private void Evaluate()
    {
        if (string.IsNullOrEmpty(_currentOperator) || _isNewEntry) return;

        if (double.TryParse(CurrentEntry, out var rightOperand))
        {
            double result = 0;
            switch (_currentOperator)
            {
                case "+": result = _leftOperand + rightOperand; break;
                case "-": result = _leftOperand - rightOperand; break;
                case "*": result = _leftOperand * rightOperand; break;
                case "/": 
                    if (rightOperand == 0) { CurrentEntry = "Erro"; _isNewEntry = true; return; }
                    result = _leftOperand / rightOperand; 
                    break;
            }

            ExpressionDisplay = $"{_leftOperand} {_currentOperator} {rightOperand} =";
            CurrentEntry = result.ToString();
            _leftOperand = result;
            _currentOperator = "";
            _isNewEntry = true;
        }
    }

    [RelayCommand]
    private void ClearAll()
    {
        CurrentEntry = "0";
        ExpressionDisplay = "";
        _currentOperator = "";
        _leftOperand = 0;
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
        if (double.TryParse(CurrentEntry, out var val))
        {
            CurrentEntry = (-val).ToString();
        }
    }

    [RelayCommand]
    private void CalculatePercent()
    {
        if (double.TryParse(CurrentEntry, out var val))
        {
            CurrentEntry = (val / 100).ToString();
            _isNewEntry = true;
        }
    }

    [RelayCommand]
    private void ScientificFunction(string funcName)
    {
        if (double.TryParse(CurrentEntry, out var val))
        {
            double result = 0;
            switch (funcName)
            {
                case "sin": result = Math.Sin(val); break;
                case "cos": result = Math.Cos(val); break;
                case "tan": result = Math.Tan(val); break;
                case "sqrt": result = Math.Sqrt(val); break;
                case "log": result = Math.Log10(val); break;
                case "ln": result = Math.Log(val); break;
                case "pi": CurrentEntry = Math.PI.ToString(); _isNewEntry = true; return;
                case "e": CurrentEntry = Math.E.ToString(); _isNewEntry = true; return;
                case "sqr": result = Math.Pow(val, 2); break;
            }
            ExpressionDisplay = $"{funcName}({val}) =";
            CurrentEntry = result.ToString();
            _isNewEntry = true;
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
