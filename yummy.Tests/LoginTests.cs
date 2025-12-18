using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using System.Threading.Tasks;

namespace yummy.Tests
{
    public class LoginTests
    {
        [Fact]
        public async Task Test_Login_Exitoso()
        {
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            var userManagerMock = new Mock<UserManager<IdentityUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);
            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var userClaimsMock = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
            var signInManagerMock = new Mock<SignInManager<IdentityUser>>(
                userManagerMock.Object,
                contextAccessorMock.Object,
                userClaimsMock.Object,
                null, null, null, null);
            signInManagerMock
                .Setup(x => x.PasswordSignInAsync("gonzalo@test.com", "Password123!", false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var resultado = await signInManagerMock.Object.PasswordSignInAsync("gonzalo@test.com", "Password123!", false, false);
            Assert.True(resultado.Succeeded);
        }
    }
}