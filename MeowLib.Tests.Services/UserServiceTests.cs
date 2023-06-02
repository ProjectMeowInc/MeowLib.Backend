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
        _userService = new UserService(hashService, userRepository, jwtTokenService, null!);
    }

    [Test]
    public async Task SignInTestAlreadyExistUser()
    {
        // Логин пользователя, который уже занят.
        var login = "tester";
        var password = "testPassword";
        
        var signInResult = await _userService.SignInAsync(login, password);
        var _ = signInResult.Match(_ =>
        {
            Assert.Fail($"Исключение {nameof(ApiException)} не было вызвано");
            return 1;
        }, exception =>
        {
            if (exception is ApiException)
            {
                return 0;
            }

            Assert.Fail($"Было вызвано исключение {exception.GetType().Name}, а ожидалось {nameof(ApiException)}");
            return 1;
        });
    }

    [Test]
    public async Task SignInTestOk()
    {
        var login = "barsik";
        var password = "testerov";

        var createUserResult = await _userService.SignInAsync(login, password);
        var _ = createUserResult.Match(user =>
        {
            Assert.AreEqual(login, user.Login);
            return 0;
        }, exception =>
        {
            Assert.Fail($"Неожиданное исключение: {exception.Message}");
            return 1;
        });
    }
}