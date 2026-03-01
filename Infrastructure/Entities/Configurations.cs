using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CleanApi.Infrastructure.Entities;

namespace CleanApi.Infrastructure.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<CustomerEntity>
{
    public void Configure(EntityTypeBuilder<CustomerEntity> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("CustomerId");
    }
}

public class StatusConfiguration : IEntityTypeConfiguration<OrderStatusLookupEntity>
{
    public void Configure(EntityTypeBuilder<OrderStatusLookupEntity> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("StatusId");
    }
}

public class OrderConfiguration : IEntityTypeConfiguration<OrderEntity>
{
    public void Configure(EntityTypeBuilder<OrderEntity> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasColumnName("OrderId");
        
        builder.HasOne(o => o.Customer)
               .WithMany(c => c.Orders)
               .HasForeignKey(o => o.CustomerId);

        builder.HasOne(o => o.Status)
               .WithMany()
               .HasForeignKey(o => o.StatusId);
    }
}