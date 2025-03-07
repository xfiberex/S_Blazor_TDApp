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

    public virtual DbSet<DiasDisponible> DiasDisponibles { get; set; }

    public virtual DbSet<RegistroProceso> RegistroProcesos { get; set; }

    public virtual DbSet<Rol> Roles { get; set; }

    public virtual DbSet<TareaDia> TareaDias { get; set; }

    public virtual DbSet<TareasCalendario> TareasCalendarios { get; set; }

    public virtual DbSet<TareasRecurrente> TareasRecurrentes { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DiasDisponible>(entity =>
        {
            entity.HasKey(e => e.DiaId).HasName("PK__Dias_Dis__ED194C76F3466B15");

            entity.ToTable("Dias_Disponibles");

            entity.Property(e => e.NombreDia).HasMaxLength(20);
        });

        modelBuilder.Entity<RegistroProceso>(entity =>
        {
            entity.HasKey(e => e.ProcesoId).HasName("PK__Registro__1C00FFD0A8B349D5");

            entity.ToTable("Registro_Procesos");

            entity.Property(e => e.DescripcionRegistro).HasMaxLength(100);
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.RefTareaRecurr).WithMany(p => p.RegistroProcesos)
                .HasForeignKey(d => d.TareaRecurrId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Registro_Procesos_TareasRecurrentes");

            entity.HasOne(d => d.RefUsuario).WithMany(p => p.RegistroProcesos)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Registro_Procesos_Usuarios");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.RolId).HasName("PK__Rol__F92302F146F08C8A");

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
            entity.HasKey(e => e.TareaDiaId).HasName("PK__Tarea_Di__B663D2D8D9A06111");

            entity.ToTable("Tarea_Dias");

            entity.HasOne(d => d.IdDiaNavegation).WithMany(p => p.TareaDia)
                .HasForeignKey(d => d.DiaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TareaDias_DiasDisponibles");

            entity.HasOne(d => d.IdTareaRecurrNavegation).WithMany(p => p.TareaDia)
                .HasForeignKey(d => d.TareaRecurrId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TareaDias_TareasRecurrentes");
        });

        modelBuilder.Entity<TareasCalendario>(entity =>
        {
            entity.HasKey(e => e.TareaId).HasName("PK__Tareas_C__5CD8399198BF4AFB");

            entity.ToTable("Tareas_Calendario");

            entity.Property(e => e.DescripcionTarea).HasMaxLength(250);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.Habilitado).HasDefaultValue(true);
            entity.Property(e => e.Hora).HasColumnType("datetime");
            entity.Property(e => e.NombreTarea).HasMaxLength(100);
        });

        modelBuilder.Entity<TareasRecurrente>(entity =>
        {
            entity.HasKey(e => e.TareaRecurrId).HasName("PK__Tareas_R__E95278B1A55B7B91");

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
            entity.HasKey(e => e.UsuarioId).HasName("PK__Usuarios__2B3DE7B8DB60C54E");

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
