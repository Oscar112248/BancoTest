using System;
using System.Collections.Generic;
using Banco.Infrastructure.Persistence.Entites;
using Microsoft.EntityFrameworkCore;

namespace Banco.Infrastructure.Persistence.Context;

public partial class BANCO_DbContext : DbContext
{
    public BANCO_DbContext()
    {
    }

    public BANCO_DbContext(DbContextOptions<BANCO_DbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Cuentum> Cuenta { get; set; }

    public virtual DbSet<Movimiento> Movimientos { get; set; }

    public virtual DbSet<ParametroSistema> ParametroSistemas { get; set; }

    public virtual DbSet<Persona> Personas { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=172.22.16.52\\SQLMDMQ;Database=BANCO;User Id=permisousuario;Password=123456;integrated security=False;MultipleActiveResultSets=true;TrustServerCertificate=True;Pooling=True;Connection Lifetime=0;Min Pool Size=20;Max Pool Size=200");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.ClienteIdPersona);

            entity.ToTable("Cliente", "test");

            entity.HasIndex(e => e.CodigoCliente, "UX_Cliente_Codigo_NoEliminado")
                .IsUnique()
                .HasFilter("([Estado]=(0))");

            entity.Property(e => e.ClienteIdPersona).ValueGeneratedNever();
            entity.Property(e => e.CodigoCliente)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Contrasenia).HasMaxLength(256);

            entity.HasOne(d => d.ClienteIdPersonaNavigation).WithOne(p => p.Cliente)
                .HasForeignKey<Cliente>(d => d.ClienteIdPersona)
                .HasConstraintName("FK_Cliente_Persona");
        });

        modelBuilder.Entity<Cuentum>(entity =>
        {
            entity.HasKey(e => e.CuentaId);

            entity.ToTable("Cuenta", "test");

            entity.HasIndex(e => e.NumeroCuenta, "UX_Cuenta_Numero_NoElimnado")
                .IsUnique()
                .HasFilter("([Estado]=(0))");

            entity.Property(e => e.FechaApertura)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.NumeroCuenta)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SaldoInicial).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TipoCuenta)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.ClienteIdPersonaNavigation).WithMany(p => p.Cuenta)
                .HasForeignKey(d => d.ClienteIdPersona)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cuenta_Cliente");
        });

        modelBuilder.Entity<Movimiento>(entity =>
        {
            entity.ToTable("Movimiento", "test");

            entity.HasIndex(e => new { e.CuentaId, e.Fecha }, "IX_Movimiento_Cuenta_Fecha_NoAnulado").HasFilter("([Anulado]=(0))");

            entity.Property(e => e.Fecha)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.MovimientoNeto)
                .HasComputedColumnSql("(case when [TipoMovimiento]='C' then [Valor] else  -[Valor] end)", true)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TipoMovimiento)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Valor).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Cuenta).WithMany(p => p.Movimientos)
                .HasForeignKey(d => d.CuentaId)
                .HasConstraintName("FK_Movimiento_Cuenta");
        });

        modelBuilder.Entity<ParametroSistema>(entity =>
        {
            entity.HasKey(e => e.ParametroId).HasName("PK_Parametro");

            entity.ToTable("ParametroSistema", "test");

            entity.HasIndex(e => e.Clave, "UX_Parametro_Clave").IsUnique();

            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ValorDecimal).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Persona>(entity =>
        {
            entity.ToTable("Persona", "test");

            entity.HasIndex(e => e.Identificacion, "UX_Persona_Identificacion_NoEliminada")
                .IsUnique()
                .HasFilter("([EliminadoPersona]=(0))");

            entity.Property(e => e.Direccion).HasMaxLength(250);
            entity.Property(e => e.Genero)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Identificacion)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Nombre).HasMaxLength(120);
            entity.Property(e => e.Telefono)
                .HasMaxLength(25)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
