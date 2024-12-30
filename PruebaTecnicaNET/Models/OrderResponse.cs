namespace PruebaTecnicaNET.Models;

public class OrderResponse
{
    public string? CodigoEnvio { get; set; }
    public string? Estado { get; set; }
}

public class EnviarPedidoRespuesta
{
    public OrderResponse? EnviarPedidoResponse { get; set; }
}