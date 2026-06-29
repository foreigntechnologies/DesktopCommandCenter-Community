using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Windows.UI;

namespace DesktopCommandCenter.UI.Controls;

/// <summary>
/// A lightweight WinUI 3 UserControl that renders basic Markdown using RichTextBlock.
/// Supports: bold (**), italic (*), inline code (`), code blocks (```), headers (#), bullet lists.
/// </summary>
public sealed partial class MarkdownTextBlock : UserControl
{
    private readonly RichTextBlock _rtb;

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text), typeof(string), typeof(MarkdownTextBlock),
        new PropertyMetadata(string.Empty, OnTextChanged));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public MarkdownTextBlock()
    {
        _rtb = new RichTextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            IsTextSelectionEnabled = true,
            LineHeight = 22,
        };
        Content = _rtb;
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is MarkdownTextBlock block)
            block.Render(e.NewValue as string ?? "");
    }

    private void Render(string markdown)
    {
        _rtb.Blocks.Clear();
        if (string.IsNullOrEmpty(markdown)) return;

        var lines = markdown.Replace("\r\n", "\n").Split('\n');
        bool inCodeBlock = false;
        string codeBlockLang = "";
        var codeLines = new List<string>();

        void FlushCodeBlock()
        {
            if (codeLines.Count == 0 && string.IsNullOrEmpty(codeBlockLang)) return;
            var codePara = new Paragraph { Margin = new Thickness(0, 6, 0, 6) };

            if (!string.IsNullOrEmpty(codeBlockLang))
            {
                codePara.Inlines.Add(new Run
                {
                    Text = codeBlockLang + "\n",
                    FontSize = 10.5,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 86, 210, 166)),
                });
            }

            codePara.Inlines.Add(new Run
            {
                Text = string.Join("\n", codeLines),
                FontFamily = new FontFamily("Consolas, Courier New, monospace"),
                FontSize = 12,
            });

            _rtb.Blocks.Add(codePara);
            codeLines.Clear();
            codeBlockLang = "";
        }

        foreach (var rawLine in lines)
        {
            var trimmed = rawLine.TrimStart();

            // Detect code fence ```
            if (trimmed.StartsWith("```"))
            {
                if (!inCodeBlock)
                {
                    inCodeBlock = true;
                    codeBlockLang = trimmed.Substring(3).Trim();
                }
                else
                {
                    inCodeBlock = false;
                    FlushCodeBlock();
                }
                continue;
            }

            if (inCodeBlock)
            {
                codeLines.Add(rawLine);
                continue;
            }

            var para = new Paragraph { Margin = new Thickness(0, 2, 0, 2) };
            string line = rawLine;

            // Heading detection
            int hLevel = 0;
            while (hLevel < line.Length && line[hLevel] == '#') hLevel++;
            if (hLevel > 0 && hLevel < 7 && hLevel < line.Length && line[hLevel] == ' ')
            {
                line = line.Substring(hLevel + 1);
                para.Margin = new Thickness(0, hLevel <= 2 ? 14 : 8, 0, 4);
                double fontSize = hLevel == 1 ? 22 : hLevel == 2 ? 18 : hLevel == 3 ? 15 : 13;
                var fw = hLevel <= 2 ? FontWeights.Bold : FontWeights.SemiBold;
                foreach (var inl in ParseInline(line))
                {
                    if (inl is Run r) { r.FontSize = fontSize; r.FontWeight = fw; }
                    para.Inlines.Add(inl);
                }
                _rtb.Blocks.Add(para);
                continue;
            }

            // Horizontal rule ---
            if (trimmed == "---" || trimmed == "***" || trimmed == "___")
            {
                // Add a spacer paragraph
                _rtb.Blocks.Add(new Paragraph { Margin = new Thickness(0, 8, 0, 8) });
                continue;
            }

            // Bullet list
            if (trimmed.StartsWith("- ") || trimmed.StartsWith("* ") || trimmed.StartsWith("• "))
            {
                var indent = line.Length - trimmed.Length;
                line = new string(' ', indent * 2) + "•  " + trimmed.Substring(2);
            }

            // Numbered list
            var numMatch = Regex.Match(line, @"^(\s*)(\d+)\.\s(.+)");
            if (numMatch.Success)
            {
                line = numMatch.Groups[1].Value + numMatch.Groups[2].Value + ". " + numMatch.Groups[3].Value;
            }

            foreach (var inl in ParseInline(line))
                para.Inlines.Add(inl);

            if (para.Inlines.Count == 0)
                para.Inlines.Add(new Run { Text = " " });

            _rtb.Blocks.Add(para);
        }

        if (inCodeBlock) FlushCodeBlock();
    }

    private static List<Inline> ParseInline(string text)
    {
        var result = new List<Inline>();
        if (string.IsNullOrEmpty(text))
        {
            result.Add(new Run { Text = " " });
            return result;
        }

        // Match: ***bold+italic***, **bold**, *italic*, `code`, ~~strikethrough~~
        const string pattern = @"(\*\*\*(.+?)\*\*\*|\*\*(.+?)\*\*|\*(.+?)\*|`([^`\n]+)`|~~(.+?)~~)";
        int last = 0;

        foreach (Match m in Regex.Matches(text, pattern, RegexOptions.Singleline))
        {
            if (m.Index > last)
                result.Add(new Run { Text = text.Substring(last, m.Index - last) });

            if (m.Groups[2].Success) // ***bold+italic***
                result.Add(new Run { Text = m.Groups[2].Value, FontWeight = FontWeights.Bold, FontStyle = Windows.UI.Text.FontStyle.Italic });
            else if (m.Groups[3].Success) // **bold**
                result.Add(new Run { Text = m.Groups[3].Value, FontWeight = FontWeights.Bold });
            else if (m.Groups[4].Success) // *italic*
                result.Add(new Run { Text = m.Groups[4].Value, FontStyle = Windows.UI.Text.FontStyle.Italic });
            else if (m.Groups[5].Success) // `code`
                result.Add(new Run
                {
                    Text = m.Groups[5].Value,
                    FontFamily = new FontFamily("Consolas, Courier New, monospace"),
                    FontSize = 12.5,
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 86, 210, 166)),
                });
            else if (m.Groups[6].Success) // ~~strikethrough~~
                result.Add(new Run { Text = m.Groups[6].Value }); // WinUI doesn't have native strikethrough on Run, just show as normal

            last = m.Index + m.Length;
        }

        if (last < text.Length)
            result.Add(new Run { Text = text.Substring(last) });

        if (result.Count == 0)
            result.Add(new Run { Text = text });

        return result;
    }
}
