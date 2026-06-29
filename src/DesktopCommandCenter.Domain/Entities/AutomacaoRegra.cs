using System;
using System.Text.Json.Serialization;

namespace DesktopCommandCenter.Domain.Entities;

public class AutomacaoRegra : EntityBase
{
    public string Gatilho { get; set; } = string.Empty;
    public string Acao { get; set; } = string.Empty;
    public string GatilhoParametro { get; set; } = string.Empty;
    public string AcaoParametro { get; set; } = string.Empty;
    public bool IsAtivo { get; set; } = true;

    // Construtor padrão para uso interno do app
    public AutomacaoRegra()
    {
    }

    // Construtor para desserialização do JSON preservando as propriedades do EntityBase
    [JsonConstructor]
    public AutomacaoRegra(Guid id, DateTime createdAt, DateTime? updatedAt, string gatilho, string acao, string gatilhoParametro, string acaoParametro, bool isAtivo)
    {
        Id = id;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        Gatilho = gatilho;
        Acao = acao;
        GatilhoParametro = gatilhoParametro;
        AcaoParametro = acaoParametro;
        IsAtivo = isAtivo;
    }
}
