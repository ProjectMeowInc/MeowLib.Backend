using MeowLib.Domain.Exceptions;
using MeowLib.WebApi.DAL.Repository.Implementation.Tests;
using MeowLIb.WebApi.Services.Implementation.Production;
using MeowLIb.WebApi.Services.Interface;
using NUnit.Framework;

namespace MeowLib.Tests.Services;

[TestFixture]
public class UserServiceTests
{
    private static IUserService _userService = null!;

    [SetUp]
    public void SetUp()
    {
        var hashService = new HashService();
        var userRepository = new UserTestRepository();
        var jwtTokenService = new JwtTokensService(null!);
        _userService = new UserService(hashService, userRepository, jwtTokenService);
    }

    [Test]
    public async Task SignInTestAlreadyExistUser()
    {
        // Логин пользователя, который уже занят.
        var login = "tester";
        var password = "testPassword";

        var signInResult = await _userService.SignInAsync(login, password);
        if (!signInResult.IsFailure)
        {
            Assert.Fail($"Исключение не было вызвано");
        }

        var exception = signInResult.GetError();
        if (exception is not ApiException)
        {
            Assert.Fail($"Было вызвано исключение {exception.GetType().Name}, а ожидалось {nameof(ApiException)}");
        }
    }

    [Test]
    public async Task SignInTestOk()
    {
        var login = "barsik";
        var password = "testerov";

        var createUserResult = await _userService.SignInAsync(login, password);
        if (createUserResult.IsFailure)
        {
            var exception = createUserResult.GetError();
            Assert.Fail($"Неожиданное исключение: {exception.Message}");
        }

        var user = createUserResult.GetResult();
        Assert.AreEqual(login, user.Login);
    }
}