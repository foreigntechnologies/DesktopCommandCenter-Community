using System;
using System.Net;
using System.Threading.Tasks;

namespace DesktopCommandCenter.ProFeatures.Services;

public static class DesktopOAuthHelper
{
    public static async Task<string> WaitForRedirectAsync(string loopbackUrl = "http://127.0.0.1:5000/")
    {
        using var listener = new HttpListener();
        listener.Prefixes.Add(loopbackUrl);
        listener.Start();

        var context = await listener.GetContextAsync();
        var response = context.Response;

        // Display a message in the browser
        string responseString = "<html><body><h2>Autenticação Concluída</h2><p>Você pode fechar esta aba e voltar para o aplicativo.</p></body><script>window.close();</script></html>";
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        var output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();

        listener.Stop();

        return context.Request.Url?.ToString() ?? "";
    }
}
