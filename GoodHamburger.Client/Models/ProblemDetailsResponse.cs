namespace GoodHamburger.Client.Models;

/// <summary>
/// Representa a resposta de erro no formato ProblemDetails retornado pela API.
/// </summary>
public class ProblemDetailsResponse
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Status { get; set; }
    public string Detail { get; set; } = string.Empty;
    public string TraceId { get; set; } = string.Empty;
}
