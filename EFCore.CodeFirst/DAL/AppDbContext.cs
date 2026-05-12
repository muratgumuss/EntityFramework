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
        //public DbSet<ProductFeature> productFeatures { get; set; }

        //public DbSet<Person> People { get; set; }
        //public DbSet<Student> Students { get; set; }
        //public DbSet<Teacher> Teachers { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {


            Initializer.Build();
            optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information).UseSqlServer(Initializer.Configuration.GetConnectionString("SqlCon"));



        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Fluent api kullanarak tablo isimlerini değiştirebiliriz.
            //modelBuilder.Entity<Product>().ToTable("TblProducts");

            //Fluent api kullanarak kolon isimlerini değiştirebiliriz.
            //modelBuilder.Entity<Product>().HasKey(p => p.Id);

            //Fluent api kullanarak kolon özelliklerini değiştirebiliriz.
            modelBuilder.Entity<Product>().Property(p => p.Name).IsRequired();
            base.OnModelCreating(modelBuilder);
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
