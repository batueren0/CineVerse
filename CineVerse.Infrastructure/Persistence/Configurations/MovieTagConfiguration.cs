
using CineVerse.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CineVerse.Infrastructure.Persistence.Configurations;

public class MovieTagConfiguration : BaseEntityConfiguration<MovieTag>
{
    protected override void ConfigureEntity(EntityTypeBuilder<MovieTag> builder)
    {
        builder.HasOne(mt => mt.Movie)
            .WithMany(m => m.Tags)
            .HasForeignKey(mt => mt.MovieId);
        builder.HasOne(mt => mt.Tag)
            .WithMany(t => t.Movies)
            .HasForeignKey(mt => mt.TagId);
        builder.HasIndex(x => new { x.MovieId, x.TagId })
            .IsUnique()
            .HasFilter("[IsActive] = 1");
    }
}
