using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System.Runtime.CompilerServices;

namespace DesktopCommandCenter.UI.Helpers;

/// <summary>
/// Attached property to easily translate UI elements in XAML.
/// It uses ConditionalWeakTable to track elements without keeping them alive.
/// Usage: &lt;TextBlock helpers:Translate.Key="MyTranslationKey" /&gt;
///
/// NOTE: This class must NOT be static. WinUI 3's XAML compiler (WMC0010) requires
/// that the owner type of a DependencyProperty.RegisterAttached call is an
/// instantiable (non-static) class with a default constructor.
/// All members remain static so that the attached property mechanics work correctly.
/// </summary>
public class Translate : DependencyObject
{
    // Required default constructor for WinUI 3 XAML compiler
    public Translate() { }

    private static readonly ConditionalWeakTable<DependencyObject, string> _trackedElements = new();
    private static bool _isSubscribed = false;

    private static Microsoft.UI.Dispatching.DispatcherQueue? _dispatcherQueue;

    public static readonly DependencyProperty KeyProperty = DependencyProperty.RegisterAttached(
        "Key", typeof(string), typeof(Translate), new PropertyMetadata(null, OnKeyChanged));

    public static void SetKey(DependencyObject element, string value) => element.SetValue(KeyProperty, value);
    public static string GetKey(DependencyObject element) => (string)element.GetValue(KeyProperty);

    private static void OnKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (_dispatcherQueue == null)
            _dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

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
        // Capture elements/keys before dispatching to avoid enumeration on background thread
        var snapshot = new System.Collections.Generic.List<(DependencyObject element, string key)>();
        foreach (var kvp in _trackedElements)
        {
            snapshot.Add((kvp.Key, kvp.Value));
        }

        // Dispatch all UI updates back to the UI thread to prevent WinRT COM exceptions
        if (snapshot.Count > 0 && _dispatcherQueue != null)
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                foreach (var item in snapshot)
                {
                    UpdateElement(item.element, item.key);
                }
            });
        }
        else
        {
            // Fallback: update directly (already on UI thread)
            foreach (var (element, key) in snapshot)
            {
                UpdateElement(element, key);
            }
        }
    }

    private static void UpdateElement(DependencyObject d, string key)
    {
        try
        {
            var text = LocalizationHelper.Instance.GetString(key);

            if (d is TextBlock tb)
            {
                tb.Text = text;
            }
            else if (d is Microsoft.UI.Xaml.Documents.Run run)
            {
                run.Text = text;
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
                if (key.EndsWith("_Placeholder"))
                {
                    textBox.PlaceholderText = text;
                }
                else
                {
                    textBox.Header = text;

                    // Also attempt to get placeholder if it exists (e.g., Key + "_Placeholder")
                    var placeholderKey = key + "_Placeholder";
                    var placeholderText = LocalizationHelper.Instance.GetString(placeholderKey);
                    if (!string.IsNullOrEmpty(placeholderText) && placeholderText != placeholderKey)
                    {
                        textBox.PlaceholderText = placeholderText;
                    }
                }
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

                // Localize On/Off labels using general Toggle_On / Toggle_Off keys
                var onText = LocalizationHelper.Instance.GetString("Toggle_On");
                var offText = LocalizationHelper.Instance.GetString("Toggle_Off");

                if (!string.IsNullOrEmpty(onText) && onText != "Toggle_On")
                    toggleSwitch.OnContent = onText;

                if (!string.IsNullOrEmpty(offText) && offText != "Toggle_Off")
                    toggleSwitch.OffContent = offText;
            }
        }
        catch
        {
            // Ignore disposed objects or COM exceptions
        }
    }
}
