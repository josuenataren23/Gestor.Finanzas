using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Gestor.Finanzas.Models
{
    public partial class GestorFinanzasModel : DbContext
    {
        public GestorFinanzasModel()
            : base("name=GestorFinanzasModel")
        {
        }

        public virtual DbSet<Categoria> Categorias { get; set; }
        public virtual DbSet<Meta> Metas { get; set; }
        public virtual DbSet<Presupuesto> Presupuestos { get; set; }
        public virtual DbSet<Transaccione> Transacciones { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categoria>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Categoria>()
                .Property(e => e.descripcion)
                .IsUnicode(false);

            modelBuilder.Entity<Categoria>()
                .HasMany(e => e.Presupuestos)
                .WithOptional(e => e.Categoria)
                .HasForeignKey(e => e.categoria_id);

            modelBuilder.Entity<Categoria>()
                .HasMany(e => e.Transacciones)
                .WithOptional(e => e.Categoria)
                .HasForeignKey(e => e.categoria_id);

            modelBuilder.Entity<Meta>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Meta>()
                .Property(e => e.monto_objetivo)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Meta>()
                .Property(e => e.monto_actual)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Presupuesto>()
                .Property(e => e.monto_objetivo)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Presupuesto>()
                .Property(e => e.monto_gastado)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Transaccione>()
                .Property(e => e.monto)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Transaccione>()
                .Property(e => e.descripcion)
                .IsUnicode(false);

            modelBuilder.Entity<Usuario>()
                .Property(e => e.Nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Usuario>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Usuario>()
                .Property(e => e.FotoPerfilUrl)
                .IsUnicode(false);

            modelBuilder.Entity<Usuario>()
                .HasMany(e => e.Metas)
                .WithOptional(e => e.Usuario)
                .HasForeignKey(e => e.usuario_id);

            modelBuilder.Entity<Usuario>()
                .HasMany(e => e.Presupuestos)
                .WithOptional(e => e.Usuario)
                .HasForeignKey(e => e.usuario_id);

            modelBuilder.Entity<Usuario>()
                .HasMany(e => e.Transacciones)
                .WithOptional(e => e.Usuario)
                .HasForeignKey(e => e.usuario_id);
        }
    }
}
