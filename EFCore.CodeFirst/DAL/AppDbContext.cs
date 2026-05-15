using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFCore.CodeFirst.DAL
{
    public class AppDbContext : DbContext
    {

        public AppDbContext()
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductFeature> productFeatures { get; set; }

        public DbSet<Person> People { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Initializer.Build();
            optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information).UseLazyLoadingProxies().UseSqlServer(Initializer.Configuration.GetConnectionString("SqlCon"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Fluent api kullanarak tablo isimlerini değiştirebiliriz.
            //modelBuilder.Entity<Product>().ToTable("TblProducts");

            //Fluent api kullanarak kolon isimlerini değiştirebiliriz.
            //modelBuilder.Entity<Product>().HasKey(p => p.Id);

            //Fluent api kullanarak kolon özelliklerini değiştirebiliriz.
            modelBuilder.Entity<Product>().Property(p => p.Name).IsRequired();

            //Fluent api kullanarak ilişkileri tanımlayabiliriz.
            modelBuilder.Entity<Category>().HasMany(c => c.Products).WithOne(p => p.Category).HasForeignKey(p => p.CategoryId);
            base.OnModelCreating(modelBuilder);

            // one to many
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId);

            // one to one
            modelBuilder.Entity<Product>()
                .HasOne(p => p.ProductFeature)
                .WithOne(pf => pf.Product)
                .HasForeignKey<ProductFeature>(pf => pf.Id);

            // many to many
            modelBuilder.Entity<Student>()
                .HasMany(s => s.Teachers)
                .WithMany(t => t.Students)
                .UsingEntity(j => j.ToTable("StudentTeacherManyToMany"));

            // cascade delete -- bir kategoriyi silerken o kategoriye ait ürünlerinde silinmesini sağlar.
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .OnDelete(DeleteBehavior.Cascade);

            // restrict delete -- bir kategoriyi silerken o kategoriye ait ürünlerin silinmesini engeller.
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .OnDelete(DeleteBehavior.Restrict);
            // no action delete -- bir kategoriyi silerken o kategoriye ait ürünlerin silinmesini engeller ve hata vermez.
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .OnDelete(DeleteBehavior.NoAction);

            // set null delete -- bir kategoriyi silerken o kategoriye ait ürünlerin CategoryId kolonunu null yapar.
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .OnDelete(DeleteBehavior.SetNull);

            // computed column -- veritabanında oluşturulma tarihini otomatik olarak ekleyebiliriz. Update yaparken bu kolonun güncellenmesini engelleyebiliriz.
            modelBuilder.Entity<Product>()
                .Property(p => p.CreatedDate)
                .HasComputedColumnSql("GETDATE()");
            // none -- Id property'sinin değerini manuel olarak gireceğimiz anlamına gelir. Veritabanı bu değeri otomatik olarak oluşturmaz.
            //modelBuilder.Entity<Product>().
            //    Property(p => p.Id)
            //    .ValueGeneratedOnAdd();

            // Identity -- Id property'sinin değerini veritabanının otomatik olarak oluşturacağı anlamına gelir.
            //modelBuilder.Entity<Product>()
            //    .Property(p => p.Id)
            //    .ValueGeneratedOnAdd();
        }

        public override int SaveChanges()
        {
            ChangeTracker.Entries().ToList().ForEach(e =>
            {
                if (e.Entity is Product p)
                {
                    if (e.State == EntityState.Added)
                    {
                        p.CreatedDate = DateTime.Now;
                    }
                }
            });

            return base.SaveChanges();
        }
    }
}
