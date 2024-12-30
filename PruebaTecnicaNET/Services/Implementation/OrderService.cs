using PruebaTecnicaNET.Models;
using System.Xml.Linq;
using System.Xml;

namespace PruebaTecnicaNET.Services;

public class OrderService : IOrderService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private const string SoapEndpoint = "https://run.mocky.io/v3/19217075-6d4e-4818-98bc-416d1feb7b84";

    public OrderService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<EnviarPedidoRespuesta> ProcessOrderAsync(EnviarPedido request)
    {
        var soapRequest = ConvertToSoapRequest(request);

        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(SoapEndpoint);
        var soapResponse = await response.Content.ReadAsStringAsync();

        return ConvertToJsonResponse(soapResponse);
    }

    private string ConvertToSoapRequest(EnviarPedido request)
    {
        var soapEnvelope = new XElement(XName.Get("Envelope", "http://schemas.xmlsoap.org/soap/envelope/"),
            new XElement(XName.Get("Header", "http://schemas.xmlsoap.org/soap/envelope/")),
            new XElement(XName.Get("Body", "http://schemas.xmlsoap.org/soap/envelope/"),
                new XElement(XName.Get("EnvioPedidoAcme", "http://WSDLs/EnvioPedidos/EnvioPedidosAcme"),
                    new XElement("EnvioPedidoRequest",
                        new XElement("pedido", request.EnviarPedidoRequest?.NumPedido),
                        new XElement("Cantidad", request.EnviarPedidoRequest?.CantidadPedido),
                        new XElement("EAN", request.EnviarPedidoRequest?.CodigoEAN),
                        new XElement("Producto", request.EnviarPedidoRequest?.NombreProducto),
                        new XElement("Cedula", request.EnviarPedidoRequest?.NumDocumento),
                        new XElement("Direccion", request.EnviarPedidoRequest?.Direccion)
                    )
                )
            )
        );

        return soapEnvelope.ToString();
    }

    private EnviarPedidoRespuesta ConvertToJsonResponse(string soapResponse)
    {
        var doc = XDocument.Parse(soapResponse);
        var nsManager = new XmlNamespaceManager(new NameTable());
        nsManager.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
        nsManager.AddNamespace("env", "http://WSDLs/EnvioPedidos/EnvioPedidosAcme");

        var codigo = doc.Descendants("Codigo").FirstOrDefault()?.Value;
        var mensaje = doc.Descendants("Mensaje").FirstOrDefault()?.Value;

        return new EnviarPedidoRespuesta
        {
            EnviarPedidoResponse = new OrderResponse
            {
                CodigoEnvio = codigo,
                Estado = mensaje
            }
        };
    }
}