using CineVerse.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CineVerse.Infrastructure.Persistence.Configurations;

public class GenreConfiguration : BaseEntityConfiguration<Genre>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Genre> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(100);
        builder.HasIndex(x => x.Name)
            .IsUnique();
        builder.HasIndex(x => x.Slug)
            .IsUnique();
    }
}
