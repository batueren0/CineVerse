
using CineVerse.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CineVerse.Infrastructure.Persistence.Configurations;

public class MovieConfiguration : BaseEntityConfiguration<Movie>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Movie> builder)
    {
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(256);
        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(256);
        builder.Property(x => x.Director)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(x => x.ReleaseYear)
            .IsRequired();
        builder.Property(x => x.PosterUrl)
            .IsRequired(false)
            .HasMaxLength(512);
        builder.Property(x => x.Overview)
            .IsRequired();
        builder.HasIndex(x => x.Title)
            .IsUnique();
        builder.HasIndex(x => x.Slug)
            .IsUnique();

        builder.HasOne(m => m.Genre)
            .WithMany(g => g.Movies)
            .HasForeignKey(m => m.GenreId);
    }
}
