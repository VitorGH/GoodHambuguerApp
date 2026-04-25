using System.Net;
using System.Text.Json;
using GoodHamburger.Api.Exceptions;

namespace GoodHamburger.Api.Middleware;

/// <summary>
/// Middleware global para tratamento de exceções.
/// Converte exceções conhecidas em respostas HTTP padronizadas com ProblemDetails.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = exception switch
        {
            DuplicateItemException => ((int)HttpStatusCode.Conflict, "Conflito de Itens Duplicados"),
            InvalidOrderException => ((int)HttpStatusCode.BadRequest, "Pedido Inválido"),
            OrderNotFoundException => ((int)HttpStatusCode.NotFound, "Pedido Não Encontrado"),
            _ => ((int)HttpStatusCode.InternalServerError, "Erro Interno do Servidor")
        };

        // Log de erros internos com mais detalhes
        if (statusCode == (int)HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Erro interno não tratado: {Message}", exception.Message);
        }
        else
        {
            _logger.LogWarning("Exceção de negócio: {ExceptionType} - {Message}",
                exception.GetType().Name, exception.Message);
        }

        var problemDetails = new
        {
            type = $"https://httpstatuses.com/{statusCode}",
            title,
            status = statusCode,
            detail = exception.Message,
            traceId = context.TraceIdentifier
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
    }
}
