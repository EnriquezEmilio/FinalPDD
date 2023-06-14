using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WebBanco.Models;

namespace WebBanco.Data
{
    public class MyContext : DbContext
    {
        public DbSet<Usuario> usuarios { get; set; }
        public DbSet<CajaDeAhorro> cajas { get; set; }
        public DbSet<Movimiento> movimientos { get; set; }
        public DbSet<Pago> pagos { get; set; }
        public DbSet<Tarjeta> tarjetas { get; set; }
        public DbSet<PlazoFijo> plazoFijos { get; set; }       
        public MyContext(DbContextOptions<MyContext> optionsBuilder) : base(optionsBuilder) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();

            var connectionString = configuration.GetConnectionString("MyContext");
            optionsBuilder.UseSqlServer(connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                .ToTable("Usuarios")
                .HasKey(u => u.num_usr);

            modelBuilder.Entity<CajaDeAhorro>()
                .ToTable("Cajas")
                .HasKey(c => c.id);

            modelBuilder.Entity<Tarjeta>()
                .ToTable("Tarjetas")
                .HasKey(t => t.id);

            modelBuilder.Entity<PlazoFijo>()
                .ToTable("Plazos")
                .HasKey(p => p.id);

            modelBuilder.Entity<Pago>()
                .ToTable("Pagos")
                .HasKey(pa => pa.id);

            modelBuilder.Entity<Movimiento>()
                .ToTable("Movimientos")
                .HasKey(m => m.idm);

            // ONE TO MANY - USUARIO > TARJETAS
            modelBuilder.Entity<Tarjeta>()
                .HasOne(T => T.user)
                .WithMany(U => U.misTarjetas)
                .HasForeignKey(T => T.num_usr)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            // ONE TO MANY - USUARIO > PLAZOS FIJOS
            modelBuilder.Entity<PlazoFijo>()
                .HasOne(P => P.user)
                .WithMany(U => U.misPlazosFijos)
                .HasForeignKey(P => P.num_usr)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            // ONE TO MANY - USUARIO > PAGO
            modelBuilder.Entity<Pago>()
                .HasOne(Pg => Pg.user)
                .WithMany(U => U.misPagos)
                .HasForeignKey(Pg => Pg.num_usr)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            // ONE TO MANY - MOVIMIENTO > CAJA
            modelBuilder.Entity<Movimiento>()
                .HasOne(M => M.caja)
                .WithMany(C => C.misMovimientos)
                .HasForeignKey(M => M.id)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            /* PARA MANY TO MANY NECESITAMOS TABLAS INTERMEDIAS
               USUARIO > USUARIOCAJAS < CAJADEAHORRO
               idUsuario > idUsuario/idCaja < idCaja
               HACER CLAVE COMPUESTA ENTRE idUsuario/idCaja en USUARIOCAJAS
               NECESITAMOS ICollection PARA PODER DEFINIR LA RELACION */

            modelBuilder.Entity<Usuario>()
                .HasMany(U => U.Caja)
                .WithMany(C => C.users)
                .UsingEntity<UsuarioCaja>(
                euc => euc.HasOne(uc => uc.caja).WithMany(c => c.UserCaja).HasForeignKey(u => u.id),
                euc => euc.HasOne(uc => uc.user).WithMany(u => u.UserCaja).HasForeignKey(u => u.num_usr),
                euc => euc.HasKey(key => new { key.num_usr, key.id }));

            modelBuilder.Entity<Usuario>(
            usr =>
            {
                    usr.Property(u => u.num_usr).HasColumnType("int");
                    usr.Property(u => u.num_usr).IsRequired(true);
                    usr.Property(u => u.dni).HasColumnType("int"); 
                    usr.Property(u => u.dni).IsRequired(true);
                    usr.Property(u => u.nombre).HasColumnType("varchar(50)");
                    usr.Property(u => u.nombre).IsRequired(true);
                    usr.Property(u => u.apellido).HasColumnType("varchar(50)");
                    usr.Property(u => u.apellido).IsRequired(true);
                    usr.Property(u => u.email).HasColumnType("varchar(250)");
                    usr.Property(u => u.email).IsRequired(true);
                    usr.Property(u => u.password).HasColumnType("varchar(50)");
                    usr.Property(u => u.password).IsRequired(true);
                    usr.Property(u => u.bloqueado).HasColumnType("bit");
                    usr.Property(u => u.esADM).HasColumnType("bit");                    
                });            

            modelBuilder.Entity<CajaDeAhorro>(
                caja =>
                {
                    caja.Property(c => c.id).HasColumnType("int");
                    caja.Property(c => c.id).IsRequired(true);
                    caja.Property(c => c.Cbu).HasColumnType("varchar(50)");
                    caja.Property(c => c.Cbu).IsRequired(true);
                    caja.Property(c => c.saldo).HasColumnType("float");
                    caja.Property(c => c.saldo).IsRequired(true);

                });            

            modelBuilder.Entity<Movimiento>(
                mov =>
                {
                    mov.Property(m => m.id).HasColumnType("int");
                    mov.Property(m => m.id).IsRequired(true);
                    mov.Property(m => m.detalle).HasColumnType("varchar(250)");
                    mov.Property(m => m.detalle).IsRequired(true);
                    mov.Property(m => m.monto).HasColumnType("float");
                    mov.Property(m => m.monto).IsRequired(true);
                    mov.Property(m => m.fecha).HasColumnType("date");
                    mov.Property(m => m.fecha).IsRequired(true);
                });
            

            modelBuilder.Entity<Pago>(
                pago =>
                {
                    pago.Property(p => p.id).HasColumnType("int");
                    pago.Property(p => p.id).IsRequired(true);
                    pago.Property(p => p.nombre).HasColumnType("varchar(50)");
                    pago.Property(p => p.nombre).IsRequired(true);
                    pago.Property(p => p.monto).HasColumnType("float");
                    pago.Property(p => p.monto).IsRequired(true);
                    pago.Property(p => p.pagado).HasColumnType("bit");
                    pago.Property(p => p.pagado).IsRequired(true);
                    pago.Property(p => p.metodo).HasColumnType("varchar(50)");
                });           

            modelBuilder.Entity<Tarjeta>(
                tj =>
                {
                    tj.Property(t => t.id).HasColumnType("int");
                    tj.Property(t => t.id).IsRequired(true);
                    tj.Property(t => t.numero).HasColumnType("varchar(50)");
                    tj.Property(t => t.numero).IsRequired(true);
                    tj.Property(t => t.codigoV).HasColumnType("varchar(50)");
                    tj.Property(t => t.codigoV).IsRequired(true);
                    tj.Property(t => t.limite).HasColumnType("float");
                    tj.Property(t => t.limite).IsRequired(true);
                    tj.Property(t => t.consumos).HasColumnType("float");
                    tj.Property(t => t.consumos).IsRequired(true);
                });            

            modelBuilder.Entity<PlazoFijo>(
                pf =>
                {
                    pf.Property(pf => pf.id).HasColumnType("int");
                    pf.Property(pf => pf.id).IsRequired(true);
                    pf.Property(pf => pf.monto).HasColumnType("float");
                    pf.Property(pf => pf.monto).IsRequired(true);
                    pf.Property(pf => pf.fechaIni).HasColumnType("date");
                    pf.Property(pf => pf.fechaIni).IsRequired(true);
                    pf.Property(pf => pf.fechaFin).HasColumnType("date");
                    pf.Property(pf => pf.fechaFin).IsRequired(true);
                    pf.Property(pf => pf.tasa).HasColumnType("float");
                    pf.Property(pf => pf.tasa).IsRequired(true);
                    pf.Property(pf => pf.pagado).HasColumnType("bit");
                    pf.Property(pf => pf.pagado).IsRequired(true);
                });

            modelBuilder.Entity<Usuario>().HasData(
               new { num_usr = 1, dni = 1234, nombre = "111", apellido = "222", email = "111@111", password = "111", esADM = true, bloqueado = false, intentosFallidos = 0 },
           new { num_usr = 2, dni = 1235, nombre = "asd", apellido = "sda", email = "222@111", password = "111", esADM = false, bloqueado = false, intentosFallidos = 0 },
           new { num_usr = 3, dni = 1236, nombre = "asd", apellido = "dsa", email = "333@111", password = "111", esADM = true, bloqueado = false, intentosFallidos = 0 },
           new { num_usr = 4, dni = 1237, nombre = "asd", apellido = "ads", email = "444@111", password = "111", esADM = false, bloqueado = true, intentosFallidos = 3 });
           

            //Ignorar banco
            modelBuilder.Ignore<Banco>();          
            
        }
        public DbSet<WebBanco.Models.UsuarioCaja> UsuarioCaja { get; set; }
    }
}
