using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Mapping;

public sealed class CartaoMapping : IEntityTypeConfiguration<Cartao>
{
    public void Configure(EntityTypeBuilder<Cartao> builder)
    {
        builder.ToTable("cartoes");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ClienteId).IsRequired();
        builder.Property(x => x.PropostaId).IsRequired();
        builder.Property(x => x.Sequencia).IsRequired();
        builder.HasIndex(x => new { x.PropostaId, x.Sequencia }).IsUnique();
        builder.HasIndex(x => x.ClienteId);

        builder.Property(x => x.UltimosQuatro).HasMaxLength(4).IsRequired();
        builder.Property(x => x.NumeroMascarado).HasMaxLength(32).IsRequired();
        builder.Property(x => x.Limite).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.EmitidoEmUtc).IsRequired();
    }
}
