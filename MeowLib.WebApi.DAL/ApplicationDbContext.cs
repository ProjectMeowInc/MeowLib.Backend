using MeowLib.Domain.DbModels.UserEntity;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.WebApi.DAL;

public class ApplicationDbContext : DbContext
{
    public DbSet<UserEntityModel> Users { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
}