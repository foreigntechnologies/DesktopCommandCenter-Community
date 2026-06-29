using DesktopCommandCenter.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DesktopCommandCenter.Infrastructure.Services;

/// <summary>
/// Web search service that always returns grounded, up-to-date results.
/// Strategy 1: Bing HTML scraping (primary - most reliable)
/// Strategy 2: DuckDuckGo Instant Answer API + HTML (fallback)
/// </summary>
public class DuckDuckGoSearchService : IWebSearchService
{
    private static readonly HttpClient _http;

    static DuckDuckGoSearchService()
    {
        _http = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
        _http.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36 Edg/125.0.0.0");
        _http.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
        _http.DefaultRequestHeaders.Add("Accept-Language", "pt-BR,pt;q=0.9,en-US;q=0.8,en;q=0.7");
        _http.DefaultRequestHeaders.Add("Accept-Encoding", "identity");
    }

    public async Task<List<WebSearchResult>> SearchAsync(string query, int maxResults = 6, CancellationToken cancellationToken = default)
    {
        var results = new List<WebSearchResult>();

        // Strategy 1: Yahoo
        try
        {
            var yahooResults = await SearchYahooAsync(query, maxResults, cancellationToken);
            results.AddRange(yahooResults);
            System.Diagnostics.Debug.WriteLine($"[Search] Yahoo: {yahooResults.Count} results");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Search] Yahoo error: {ex.Message}");
        }

        // Strategy 2: DuckDuckGo (if Yahoo insufficient)
        if (results.Count < 3)
        {
            try
            {
                var ddgResults = await SearchDuckDuckGoAsync(query, maxResults, cancellationToken);
                foreach (var r in ddgResults)
                    if (!results.Any(x => SimilarText(x.Snippet, r.Snippet)))
                        results.Add(r);
                System.Diagnostics.Debug.WriteLine($"[Search] DDG added: {ddgResults.Count} results");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Search] DDG error: {ex.Message}");
            }
        }

        return results.Where(r => !string.IsNullOrWhiteSpace(r.Snippet) && r.Snippet.Length > 15)
                      .Take(maxResults)
                      .ToList();
    }

    // ──────────────────────────────────────────────────────────────
    // YAHOO SCRAPER
    // ──────────────────────────────────────────────────────────────
    private async Task<List<WebSearchResult>> SearchYahooAsync(string query, int maxResults, CancellationToken ct)
    {
        var results = new List<WebSearchResult>();
        var encoded = Uri.EscapeDataString(query).Replace("%20", "+");
        var url = $"https://search.yahoo.com/search?p={encoded}";

        string html;
        try { html = await _http.GetStringAsync(url, ct); }
        catch { return results; }

        System.Diagnostics.Debug.WriteLine($"[Yahoo] HTML size: {html.Length} chars");

        // Extract regular search results
        ExtractYahooResults(html, results, maxResults);

        return results.Take(maxResults).ToList();
    }

    private static void ExtractYahooResults(string html, List<WebSearchResult> results, int maxResults)
    {
        // Yahoo results are typically within compTitle and compText classes
        var matches = Regex.Matches(html, @"class=""compTitle[^>]*>.*?<a[^>]+href=""([^""]+)""[^>]*>([\s\S]+?)</a>.*?<div class=""compText[^>]*>([\s\S]+?)</div>", RegexOptions.IgnoreCase);

        foreach (Match match in matches)
        {
            if (results.Count >= maxResults) break;

            var href = HttpUtility.HtmlDecode(match.Groups[1].Value.Trim());
            if (!href.StartsWith("http") || href.Contains("yahoo.com/search")) continue;

            var title = HtmlToText(match.Groups[2].Value);
            var snippet = HtmlToText(match.Groups[3].Value);

            if (string.IsNullOrWhiteSpace(snippet)) continue;

            results.Add(new WebSearchResult { Title = title, Snippet = snippet, Url = href });
        }
    }
    // ──────────────────────────────────────────────────────────────
    // DUCKDUCKGO FALLBACK
    // ──────────────────────────────────────────────────────────────
    private async Task<List<WebSearchResult>> SearchDuckDuckGoAsync(string query, int maxResults, CancellationToken ct)
    {
        var results = new List<WebSearchResult>();
        var encoded = Uri.EscapeDataString(query);

        // DDG Instant Answer API
        try
        {
            var json = await _http.GetStringAsync(
                $"https://api.duckduckgo.com/?q={encoded}&format=json&no_html=1&skip_disambig=1&no_redirect=1", ct);
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var abstractText = GetProp(root, "AbstractText");
            var abstractUrl  = GetProp(root, "AbstractURL");
            var heading      = GetProp(root, "Heading");
            var answer       = GetProp(root, "Answer");

            if (!string.IsNullOrWhiteSpace(answer))
                results.Add(new WebSearchResult { Title = "Resposta Direta", Snippet = answer, Url = "" });

            if (!string.IsNullOrWhiteSpace(abstractText) && abstractText.Length > 20)
                results.Add(new WebSearchResult
                {
                    Title = string.IsNullOrEmpty(heading) ? query : heading,
                    Snippet = abstractText,
                    Url = abstractUrl ?? ""
                });

            if (root.TryGetProperty("RelatedTopics", out var topics))
            {
                foreach (var topic in topics.EnumerateArray())
                {
                    if (results.Count >= maxResults) break;
                    var txt  = GetProp(topic, "Text");
                    var furl = GetProp(topic, "FirstURL");
                    if (txt.Length > 15)
                        results.Add(new WebSearchResult { Title = TitleFromUrl(furl), Snippet = txt, Url = furl });
                }
            }
        }
        catch { /* ignore */ }

        // DDG HTML search
        if (results.Count < 2)
        {
            try
            {
                var html = await _http.GetStringAsync(
                    $"https://html.duckduckgo.com/html/?q={encoded}&kl=br-pt", ct);

                var titleMatches  = Regex.Matches(html, @"class=""result__a""[^>]*href=""([^""]+)""[^>]*>([\s\S]+?)</a>", RegexOptions.IgnoreCase);
                var snippetMatches = Regex.Matches(html, @"class=""result__snippet""[^>]*>([\s\S]+?)</a>", RegexOptions.IgnoreCase);

                for (int i = 0; i < Math.Min(titleMatches.Count, snippetMatches.Count) && results.Count < maxResults; i++)
                {
                    var snip = HtmlToText(snippetMatches[i].Groups[1].Value);
                    if (snip.Length < 15) continue;
                    results.Add(new WebSearchResult
                    {
                        Title = HtmlToText(titleMatches[i].Groups[2].Value),
                        Snippet = snip,
                        Url = HttpUtility.HtmlDecode(titleMatches[i].Groups[1].Value)
                    });
                }
            }
            catch { /* ignore */ }
        }

        return results;
    }

    // ──────────────────────────────────────────────────────────────
    // HELPERS
    // ──────────────────────────────────────────────────────────────
    private static string HtmlToText(string html)
    {
        if (string.IsNullOrEmpty(html)) return "";
        html = Regex.Replace(html, @"<script[\s\S]*?</script>|<style[\s\S]*?</style>", "", RegexOptions.IgnoreCase);
        html = Regex.Replace(html, @"<br\s*/?>|</p>|</li>|</div>|</h\d>", " ", RegexOptions.IgnoreCase);
        html = Regex.Replace(html, @"<[^>]+>", "");
        html = HttpUtility.HtmlDecode(html);
        html = Regex.Replace(html, @"\s{2,}", " ");
        return html.Trim();
    }

    private static string GetProp(JsonElement el, string key) =>
        el.TryGetProperty(key, out var v) ? v.GetString() ?? "" : "";

    private static string TitleFromUrl(string url)
    {
        if (string.IsNullOrEmpty(url)) return "Resultado";
        try
        {
            var uri = new Uri(url);
            return uri.Host.Replace("www.", "");
        }
        catch { return "Resultado"; }
    }

    private static bool SimilarText(string a, string b)
    {
        if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return false;
        var short1 = a.Length > 40 ? a[..40] : a;
        return b.Contains(short1, StringComparison.OrdinalIgnoreCase);
    }
}
