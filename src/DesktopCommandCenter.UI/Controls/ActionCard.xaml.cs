using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DesktopCommandCenter.UI.Controls;

/// <summary>
/// ActionCard is a reusable component for triggering actions with a modern button layout.
/// EN: Reusable component to trigger actions with a title, description and icon.
/// PT: Componente reutilizável para disparar ações com título, descrição e ícone.
/// ES: Componente reutilizable para disparar acciones con título, descripción e icono.
/// </summary>
public sealed partial class ActionCard : UserControl
{
    public ActionCard()
    {
        this.InitializeComponent();
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register("Title", typeof(string), typeof(ActionCard), new PropertyMetadata(string.Empty));

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register("Description", typeof(string), typeof(ActionCard), new PropertyMetadata(string.Empty));

    public string IconGlyph
    {
        get => (string)GetValue(IconGlyphProperty);
        set => SetValue(IconGlyphProperty, value);
    }

    public static readonly DependencyProperty IconGlyphProperty =
        DependencyProperty.Register("IconGlyph", typeof(string), typeof(ActionCard), new PropertyMetadata("\xE8A7"));

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register("Command", typeof(ICommand), typeof(ActionCard), new PropertyMetadata(null));

    public event RoutedEventHandler Click
    {
        add => RootButton.Click += value;
        remove => RootButton.Click -= value;
    }
}
