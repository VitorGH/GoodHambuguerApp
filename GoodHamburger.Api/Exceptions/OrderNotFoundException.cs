namespace GoodHamburger.Api.Exceptions;

/// <summary>
/// Exceção lançada quando um pedido não é encontrado pelo ID informado.
/// </summary>
public class OrderNotFoundException : Exception
{
    public OrderNotFoundException(Guid orderId)
        : base($"Pedido com ID '{orderId}' não foi encontrado.") { }

    public OrderNotFoundException(string message) : base(message) { }
    public OrderNotFoundException(string message, Exception innerException) : base(message, innerException) { }
}
