
using CineVerse.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CineVerse.Infrastructure.Persistence.Configurations;

public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.IsActive)
            .IsRequired();
        builder.Property(x => x.CreatedOn)
            .IsRequired();
        builder.Property(x => x.LastModifiedOn)
            .IsRequired();
        builder.Property(x => x.DeletedOn)
            .IsRequired(false);
        builder.HasIndex(x => x.IsActive)
            .HasFilter("[IsActive] = 1");
        builder.HasIndex(x => x.CreatedOn);
        builder.HasQueryFilter(x => x.IsActive);

        ConfigureEntity(builder);
    }

    protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
}
