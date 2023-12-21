using MeowLib.DAL;
using MeowLib.Domain.DbModels.AuthorEntity;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Services.Implementation.Production;
using Moq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MeowLib.Services.Tests.AuthService;

public class CreateAuthorAsyncTest
{
    [Fact]
    public async Task CreateAuthorAsync_WithEmptyName_ShouldReturnValidationError()
    {
        // Arrange
        var mockSet = new Mock<DbSet<AuthorEntityModel>>();
        var mockContext = new Mock<ApplicationDbContext>();
        mockContext.Setup(m => m.Authors).Returns(mockSet.Object);
        var service = new AuthorService(mockContext.Object);

        // Act
        var result = await service.CreateAuthorAsync("");

        // Assert
        Assert.False(result.IsFailure);
        var exception = Assert.IsType<ValidationException>(result.GetError());
        Assert.Single(exception.ValidationErrors);
    }

    [Fact]
    public async Task CreateAuthorAsync_WithValidName_ShouldAddAuthor()
    {
        // Arrange
        var data = new List<AuthorEntityModel>().AsQueryable();
        var mockSet = new Mock<DbSet<AuthorEntityModel>>();
        mockSet.As<IQueryable<AuthorEntityModel>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<AuthorEntityModel>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<AuthorEntityModel>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<AuthorEntityModel>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        var mockContext = new Mock<ApplicationDbContext>();
        mockContext.Setup(m => m.Authors).Returns(mockSet.Object);
        mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

        var service = new AuthorService(mockContext.Object);

        // Act
        var result = await service.CreateAuthorAsync("Достоевский");

        // Assert
        Assert.True(!result.IsFailure);
        mockSet.Verify(m => m.AddAsync(It.Is<AuthorEntityModel>(a => a.Name == "Достоевский"), default), Times.Once);
        mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
    }
}