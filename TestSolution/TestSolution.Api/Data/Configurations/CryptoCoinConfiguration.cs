using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestSolution.Api.Data.Entities;

namespace TestSolution.Api.Data.Configurations
{
    public class CryptoCoinConfiguration : IEntityTypeConfiguration<CryptoCoin>
    {
        public void Configure(EntityTypeBuilder<CryptoCoin> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.HasMany(x => x.UserCryptoCoinCollection)
                .WithOne(x => x.CryptoCoinObject)
                .HasForeignKey(x => x.CryptoCoinId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.ToTable("CryptoCoin");
        }
    }
}