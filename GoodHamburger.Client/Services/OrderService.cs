using System.Net.Http.Json;
using System.Text.Json;
using GoodHamburger.Client.Models;

namespace GoodHamburger.Client.Services;

/// <summary>
/// Resultado de uma operação que pode conter sucesso ou erro.
/// </summary>
public class ServiceResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Serviço HTTP para consumir os endpoints de pedidos da API.
/// Trata erros ProblemDetails retornados pela API.
/// </summary>
public class OrderService
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public OrderService(HttpClient http)
    {
        _http = http;
    }

    /// <summary>
    /// Retorna todos os pedidos.
    /// </summary>
    public async Task<List<OrderResponse>> GetAllAsync()
    {
        var orders = await _http.GetFromJsonAsync<List<OrderResponse>>("api/orders");
        return orders ?? new List<OrderResponse>();
    }

    /// <summary>
    /// Retorna um pedido pelo ID.
    /// </summary>
    public async Task<ServiceResult<OrderResponse>> GetByIdAsync(Guid id)
    {
        var response = await _http.GetAsync($"api/orders/{id}");

        if (response.IsSuccessStatusCode)
        {
            var order = await response.Content.ReadFromJsonAsync<OrderResponse>();
            return new ServiceResult<OrderResponse> { Success = true, Data = order };
        }

        var error = await ExtractErrorMessageAsync(response);
        return new ServiceResult<OrderResponse> { Success = false, ErrorMessage = error };
    }

    /// <summary>
    /// Cria um novo pedido.
    /// </summary>
    public async Task<ServiceResult<OrderResponse>> CreateAsync(CreateOrderRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/orders", request);

        if (response.IsSuccessStatusCode)
        {
            var order = await response.Content.ReadFromJsonAsync<OrderResponse>();
            return new ServiceResult<OrderResponse> { Success = true, Data = order };
        }

        var error = await ExtractErrorMessageAsync(response);
        return new ServiceResult<OrderResponse> { Success = false, ErrorMessage = error };
    }

    /// <summary>
    /// Remove um pedido pelo ID.
    /// </summary>
    public async Task<ServiceResult<bool>> DeleteAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"api/orders/{id}");

        if (response.IsSuccessStatusCode)
        {
            return new ServiceResult<bool> { Success = true, Data = true };
        }

        var error = await ExtractErrorMessageAsync(response);
        return new ServiceResult<bool> { Success = false, ErrorMessage = error };
    }

    /// <summary>
    /// Extrai a mensagem de erro do ProblemDetails retornado pela API.
    /// </summary>
    private async Task<string> ExtractErrorMessageAsync(HttpResponseMessage response)
    {
        try
        {
            var content = await response.Content.ReadAsStringAsync();
            var problemDetails = JsonSerializer.Deserialize<ProblemDetailsResponse>(content, _jsonOptions);
            return problemDetails?.Detail ?? $"Erro inesperado ({(int)response.StatusCode}).";
        }
        catch
        {
            return $"Erro inesperado ({(int)response.StatusCode}).";
        }
    }
}
