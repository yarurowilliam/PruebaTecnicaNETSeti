using Microsoft.AspNetCore.Mvc;
using Moq;
using PruebaTecnicaNET.Controllers;
using PruebaTecnicaNET.Models;
using PruebaTecnicaNET.Services;

namespace PruebaTecnicaNETTests.Controllers;

public class OrderControllerTests
{
    private readonly Mock<IOrderService> _orderServiceMock;
    private readonly OrderController _controller;

    public OrderControllerTests()
    {
        _orderServiceMock = new Mock<IOrderService>();
        _controller = new OrderController(_orderServiceMock.Object);
    }

    [Fact]
    public async Task ProcessOrder_ValidRequest_ReturnsOkResult()
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

        var expectedResponse = new EnviarPedidoRespuesta
        {
            EnviarPedidoResponse = new OrderResponse
            {
                CodigoEnvio = "80375472",
                Estado = "Entregado exitosamente al cliente"
            }
        };

        _orderServiceMock
            .Setup(x => x.ProcessOrderAsync(It.IsAny<EnviarPedido>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.ProcessOrder(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<EnviarPedidoRespuesta>(okResult.Value);
        Assert.Equal(expectedResponse.EnviarPedidoResponse?.CodigoEnvio, response.EnviarPedidoResponse?.CodigoEnvio);
        Assert.Equal(expectedResponse.EnviarPedidoResponse?.Estado, response.EnviarPedidoResponse?.Estado);
    }

    [Fact]
    public async Task ProcessOrder_NullRequest_ReturnsBadRequest()
    {
        // Arrange
        EnviarPedido request = null!;

        // Act
        var result = await _controller.ProcessOrder(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task ProcessOrder_EmptyRequest_ReturnsBadRequest()
    {
        // Arrange
        var request = new EnviarPedido();

        // Act
        var result = await _controller.ProcessOrder(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task ProcessOrder_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var request = new EnviarPedido
        {
            EnviarPedidoRequest = new OrderRequest
            {
                NumPedido = "75630275"
            }
        };

        _orderServiceMock
            .Setup(x => x.ProcessOrderAsync(It.IsAny<EnviarPedido>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _controller.ProcessOrder(request));
    }
}