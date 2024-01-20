using MeowLib.Domain.Character.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeowLib.DAL.Configuration;

public class BookCharacterEntityModelConfiguration : IEntityTypeConfiguration<BookCharacterEntityModel>
{
    public void Configure(EntityTypeBuilder<BookCharacterEntityModel> builder)
    {
        builder
            .HasIndex(entity => new { entity.Book, entity.Character })
            .IsUnique();
    }
}