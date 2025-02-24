using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using S_Blazor_TDApp.Server.Entities;

namespace S_Blazor_TDApp.Server.DBContext;

public partial class DbTdappContext : DbContext
{
    public DbTdappContext()
    {
    }

    public DbTdappContext(DbContextOptions<DbTdappContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Rol> Roles { get; set; }

    public virtual DbSet<TareaDia> TareaDias { get; set; }

    public virtual DbSet<TareasCalendario> TareasCalendarios { get; set; }

    public virtual DbSet<TareasRecurrente> TareasRecurrentes { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.RolId).HasName("PK__Rol__F92302F1E5C6894E");

            entity.ToTable("Rol");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Descripcion).HasMaxLength(250);
            entity.Property(e => e.FechaActualizacion).HasColumnType("datetime");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NombreRol).HasMaxLength(50);
        });

        modelBuilder.Entity<TareaDia>(entity =>
        {
            entity.HasKey(e => e.TareaDiaId).HasName("PK__Tarea_Di__B663D2D87D3301E5");

            entity.ToTable("Tarea_Dias");

            entity.Property(e => e.Dia).HasMaxLength(20);

            entity.HasOne(d => d.TareaRecurr).WithMany(p => p.TareaDia)
                .HasForeignKey(d => d.TareaRecurrId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TareaDias_TareasRecurrentes");
        });

        modelBuilder.Entity<TareasCalendario>(entity =>
        {
            entity.HasKey(e => e.TareaId).HasName("PK__Tareas_C__5CD839916998F010");

            entity.ToTable("Tareas_Calendario");

            entity.Property(e => e.DescripcionTarea).HasMaxLength(250);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.Habilitado).HasDefaultValue(true);
            entity.Property(e => e.Hora).HasColumnType("datetime");
            entity.Property(e => e.NombreTarea).HasMaxLength(100);
        });

        modelBuilder.Entity<TareasRecurrente>(entity =>
        {
            entity.HasKey(e => e.TareaRecurrId).HasName("PK__Tareas_R__E95278B1ABC043B1");

            entity.ToTable("Tareas_Recurrentes");

            entity.Property(e => e.DescripcionTareaRecurr).HasMaxLength(100);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.HoraDesde).HasColumnType("datetime");
            entity.Property(e => e.HorasHasta).HasColumnType("datetime");
            entity.Property(e => e.NombreTareaRecurr).HasMaxLength(100);
            entity.Property(e => e.Recurrente).HasDefaultValue(true);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__Usuarios__2B3DE7B8593507B6");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Clave).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FechaActualizacion).HasColumnType("datetime");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NombreUsuario).HasMaxLength(100);

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_Rol");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}