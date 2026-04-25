namespace GoodHamburger.Api.Exceptions;

/// <summary>
/// Exceção lançada quando o pedido viola regras de negócio
/// (ex.: mais de um sanduíche, item inexistente no cardápio, etc.).
/// </summary>
public class InvalidOrderException : Exception
{
    public InvalidOrderException(string message) : base(message) { }
    public InvalidOrderException(string message, Exception innerException) : base(message, innerException) { }
}
