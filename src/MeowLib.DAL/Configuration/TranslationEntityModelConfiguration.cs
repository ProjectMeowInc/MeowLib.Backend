using MeowLib.Domain.Translation.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeowLib.DAL.Configuration;

public class TranslationEntityModelConfiguration : IEntityTypeConfiguration<TranslationEntityModel>
{
    public void Configure(EntityTypeBuilder<TranslationEntityModel> builder)
    {
        builder
            .HasMany(t => t.Chapters)
            .WithOne(c => c.Translation)
            .OnDelete(DeleteBehavior.Cascade);
    }
}