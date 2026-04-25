namespace GoodHamburger.Api.Exceptions;

/// <summary>
/// Exceção lançada quando o pedido contém itens duplicados.
/// </summary>
public class DuplicateItemException : Exception
{
    public DuplicateItemException(string message) : base(message) { }
    public DuplicateItemException(string message, Exception innerException) : base(message, innerException) { }
}
