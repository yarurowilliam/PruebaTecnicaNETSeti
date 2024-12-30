using Moq;
using Moq.Protected;
using PruebaTecnicaNET.Models;
using PruebaTecnicaNET.Services;
using System.Net;
using System.Text;

namespace PruebaTecnicaNETTests.Services;

public class OrderServiceTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();

        var client = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://run.mocky.io")
        };

        _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
        _orderService = new OrderService(_httpClientFactoryMock.Object);
    }

    [Fact]
    public async Task ProcessOrderAsync_ValidRequest_ReturnsValidResponse()
    {
        // Arrange
        var request = new EnviarPedido
        {
            EnviarPedidoRequest = new OrderRequest
            {
                NumPedido = "75630275",
                CantidadPedido = "1",
                CodigoEAN = "00110000765191002104587",
                NombreProducto = "Armario INVAL",
                NumDocumento = "1113987400",
                Direccion = "CR 72B 45 12 APT 301"
            }
        };

        var mockResponse = @"<?xml version='1.0' encoding='UTF-8'?>
            <soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' 
                             xmlns:env='http://WSDLs/EnvioPedidos/EnvioPedidosAcme'>
                <soapenv:Header/>
                <soapenv:Body>
                    <env:EnvioPedidoAcmeResponse>
                        <EnvioPedidoResponse>
                            <Codigo>80375472</Codigo>
                            <Mensaje>Entregado exitosamente al cliente</Mensaje>
                        </EnvioPedidoResponse>
                    </env:EnvioPedidoAcmeResponse>
                </soapenv:Body>
            </soapenv:Envelope>";

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(mockResponse, Encoding.UTF8, "application/xml")
            });

        // Act
        var result = await _orderService.ProcessOrderAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.EnviarPedidoResponse);
        Assert.Equal("80375472", result.EnviarPedidoResponse.CodigoEnvio);
        Assert.Equal("Entregado exitosamente al cliente", result.EnviarPedidoResponse.Estado);
    }

    [Fact]
    public async Task ProcessOrderAsync_EmptyRequest_ReturnsEmptyResponse()
    {
        // Arrange
        var request = new EnviarPedido
        {
            EnviarPedidoRequest = new OrderRequest()
        };

        var mockResponse = @"<?xml version='1.0' encoding='UTF-8'?>
            <soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/'>
                <soapenv:Body>
                    <env:EnvioPedidoAcmeResponse xmlns:env='http://WSDLs/EnvioPedidos/EnvioPedidosAcme'>
                        <EnvioPedidoResponse>
                            <Codigo></Codigo>
                            <Mensaje></Mensaje>
                        </EnvioPedidoResponse>
                    </env:EnvioPedidoAcmeResponse>
                </soapenv:Body>
            </soapenv:Envelope>";

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(mockResponse, Encoding.UTF8, "application/xml")
            });

        // Act
        var result = await _orderService.ProcessOrderAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.EnviarPedidoResponse);
        Assert.Empty(result.EnviarPedidoResponse.CodigoEnvio ?? string.Empty);
        Assert.Empty(result.EnviarPedidoResponse.Estado ?? string.Empty);
    }
}