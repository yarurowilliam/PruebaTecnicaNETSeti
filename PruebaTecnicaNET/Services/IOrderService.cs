using PruebaTecnicaNET.Models;

namespace PruebaTecnicaNET.Services;

public interface IOrderService
{
    Task<EnviarPedidoRespuesta> ProcessOrderAsync(EnviarPedido request);
}
