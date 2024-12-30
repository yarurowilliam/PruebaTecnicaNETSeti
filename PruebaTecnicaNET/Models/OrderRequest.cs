namespace PruebaTecnicaNET.Models;

public class OrderRequest
{
    public string? NumPedido { get; set; }
    public string? CantidadPedido { get; set; }
    public string? CodigoEAN { get; set; }
    public string? NombreProducto { get; set; }
    public string? NumDocumento { get; set; }
    public string? Direccion { get; set; }
}

public class EnviarPedido
{
    public OrderRequest? EnviarPedidoRequest { get; set; }
}