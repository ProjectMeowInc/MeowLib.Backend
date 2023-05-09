using MeowLib.Domain.Exceptions;
using MeowLib.WebApi.DAL.Repository.Implementation.Tests;
using MeowLIb.WebApi.Services.Implementation.Production;
using MeowLIb.WebApi.Services.Interface;
using NUnit.Framework;

namespace MeowLib.Tests.Services;

[TestFixture]
public class UserServiceTests
{
    private static IUserService _userService;

    [SetUp]
    public void SetUp()
    {
        var hashService = new HashService();
        var userRepository = new UserTestRepository();
        var jwtTokenService = new JwtTokensService();
        _userService = new UserService(hashService, userRepository, jwtTokenService);
    }

    [Test]
    public void SignInTestAlreadyExistUser()
    {
        // Логин пользователя, который уже занят.
        var login = "tester";
        var password = "testPassword";
        
        Assert.CatchAsync<ApiException>(async () =>
        {
            await _userService.SignInAsync(login, password);
        }, $"Исключение {nameof(ApiException)} не было вызвано");
    }

    [Test]
    public async Task SignInTestOk()
    {
        var login = "barsik";
        var password = "testerov";

        var createUser = await _userService.SignInAsync(login, password);

        Assert.AreEqual(login, createUser.Login);
    }
}