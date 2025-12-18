using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using yummyApp.Controllers;
using yummyApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace yummy.Tests
{
    public class VentaControllerTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
        private readonly VentaController _controller;

        public VentaControllerTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(c => c["ConnectionStrings:DefaultConnection"])
                       .Returns("Server=(localdb)\\mssqllocaldb;Database=FakeDb;Trusted_Connection=True;");

            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            _controller = new VentaController(_mockConfig.Object, _mockUserManager.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "usuario-test-123")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public void ConfirmarCompra_CuandoCarritoEstaVacio_DeberiaLanzarExcepcionOError()
        {
            var carritoVacio = new List<ProductoCarrito>();
            var resultado = _controller.ConfirmarCompra(carritoVacio);
            var objectResult = Assert.IsType<ObjectResult>(resultado);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public void Test_Estructura_Carrito_Carga_Correctamente()
        {
            var producto = new ProductoCarrito
            {
                id_producto = 1,
                nombre = "Pizza",
                precio = 10.5m,
                cantidad = 2
            };
            var carrito = new List<ProductoCarrito> { producto };

            Assert.Single(carrito);
            Assert.Equal(10.5m, carrito[0].precio);
        }
    }
}
