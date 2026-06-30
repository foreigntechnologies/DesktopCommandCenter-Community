using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Runtime.CompilerServices;

namespace DesktopCommandCenter.UI.Helpers;

/// <summary>
/// Attached property to easily translate UI elements in XAML.
/// It uses ConditionalWeakTable to track elements without keeping them alive.
/// Usage: <TextBlock helpers:Translate.Key="MyTranslationKey" />
/// </summary>
public static class Translate
{
    private static readonly ConditionalWeakTable<DependencyObject, string> _trackedElements = new();
    private static bool _isSubscribed = false;

    public static readonly DependencyProperty KeyProperty = DependencyProperty.RegisterAttached(
        "Key", typeof(string), typeof(Translate), new PropertyMetadata(null, OnKeyChanged));

    public static void SetKey(DependencyObject element, string value) => element.SetValue(KeyProperty, value);
    public static string GetKey(DependencyObject element) => (string)element.GetValue(KeyProperty);

    private static void OnKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is string key && !string.IsNullOrWhiteSpace(key))
        {
            _trackedElements.AddOrUpdate(d, key);
            UpdateElement(d, key);

            if (!_isSubscribed)
            {
                LocalizationHelper.Instance.PropertyChanged += LocalizationHelper_PropertyChanged;
                _isSubscribed = true;
            }
        }
        else if (e.NewValue == null)
        {
            _trackedElements.Remove(d);
        }
    }

    private static void LocalizationHelper_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        // Whenever language changes, we update all surviving UI elements
        foreach (var kvp in _trackedElements)
        {
            UpdateElement(kvp.Key, kvp.Value);
        }
    }

    private static void UpdateElement(DependencyObject d, string key)
    {
        var text = LocalizationHelper.Instance.GetString(key);

        if (d is TextBlock tb)
        {
            tb.Text = text;
        }
        else if (d is ButtonBase btn) // Includes Button, ToggleButton, HyperlinkButton
        {
            if (btn.Content is string || btn.Content == null)
            {
                btn.Content = text;
            }
        }
        else if (d is MenuFlyoutItem menuFlyoutItem)
        {
            menuFlyoutItem.Text = text;
        }
        else if (d is ToolTip tt)
        {
            tt.Content = text;
        }
        else if (d is RichTextBlock rtb)
        {
            rtb.Blocks.Clear();
            var p = new Microsoft.UI.Xaml.Documents.Paragraph();
            p.Inlines.Add(new Microsoft.UI.Xaml.Documents.Run { Text = text });
            rtb.Blocks.Add(p);
        }
        else if (d is ContentDialog dialog)
        {
            if (key.EndsWith("_PrimaryBtn")) dialog.PrimaryButtonText = text;
            else if (key.EndsWith("_CloseBtn")) dialog.CloseButtonText = text;
            else dialog.Title = text;
        }
        else if (d is TextBox textBox)
        {
            if (key.EndsWith("_Placeholder")) textBox.PlaceholderText = text;
            else textBox.Header = text;
        }
        else if (d is ComboBox comboBox)
        {
            comboBox.Header = text;
        }
        else if (d is ComboBoxItem comboBoxItem)
        {
            comboBoxItem.Content = text;
        }
        else if (d is ToggleSwitch toggleSwitch)
        {
            toggleSwitch.Header = text;
        }
    }
}
