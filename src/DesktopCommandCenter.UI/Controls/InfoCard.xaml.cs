using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DesktopCommandCenter.UI.Controls;

/// <summary>
/// InfoCard is a reusable component for displaying a section of information.
/// EN: Reusable component to display a block of information with title and icon.
/// PT: Componente reutilizável para exibir um bloco de informação com título e ícone.
/// ES: Componente reutilizable para mostrar un bloque de información con título e icono.
/// </summary>
[Microsoft.UI.Xaml.Markup.ContentProperty(Name = "InnerContent")]
public sealed partial class InfoCard : UserControl
{
    public InfoCard()
    {
        this.InitializeComponent();
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register("Title", typeof(string), typeof(InfoCard), new PropertyMetadata(string.Empty));

    public string IconGlyph
    {
        get => (string)GetValue(IconGlyphProperty);
        set => SetValue(IconGlyphProperty, value);
    }

    public static readonly DependencyProperty IconGlyphProperty =
        DependencyProperty.Register("IconGlyph", typeof(string), typeof(InfoCard), new PropertyMetadata("\xE946"));

    public object InnerContent
    {
        get => GetValue(InnerContentProperty);
        set => SetValue(InnerContentProperty, value);
    }

    public static readonly DependencyProperty InnerContentProperty =
        DependencyProperty.Register("InnerContent", typeof(object), typeof(InfoCard), new PropertyMetadata(null));
}
