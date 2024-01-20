using MeowLib.Domain.Chapter.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeowLib.DAL.Configuration;

public class ChapterEntityModelConfiguration : IEntityTypeConfiguration<ChapterEntityModel>
{
    public void Configure(EntityTypeBuilder<ChapterEntityModel> builder)
    {
        builder
            .Property(c => c.Volume)
            .HasDefaultValue(1);
    }
}