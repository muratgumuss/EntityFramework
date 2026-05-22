using EFCore.CodeFirst.Models;
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

        public DbSet<Person> Person { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }

        // TPH (Table Per Hierarchy) -- miras alınan sınıfların tek bir tabloda tutulmasıdır.
        // Discriminator kolonu ile hangi sınıfa ait olduğunu belirleriz.
        // TPT (Table Per Type) -- miras alınan sınıfların ayrı tablolarda tutulmasıdır. Tablolar arasında ilişki kurulur.
        //public DbSet<BasedPerson> BasedPeople { get; set; }
        public DbSet<Manager> Manager { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public DbSet<ProductFull> ProductFulls { get; set; }

        public DbSet<ProductEssential> ProductEssentials { get; set; }

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

            // Keyless -- ProductFull sınıfının birincil anahtarı olmadığını belirtir. Genellikle veritabanında bir tabloya karşılık gelmeyen,
            modelBuilder.Entity<ProductFull>().HasNoKey();

            // NotMapped -- Barcode property’sinin veritabanında bir kolona karşılık gelmediğini belirtir.
            // Yani, bu property veritabanında oluşturulmaz ve sorgulanmaz. Sadece uygulama içinde kullanılır.
            modelBuilder.Entity<Product>().Ignore(p => p.Barcode);

            // unicode -- Unicode karakterleri destekleyen bir string türü olduğunu belirtir. Bu, veritabanında bu kolona Unicode karakterlerin saklanabileceği anlamına gelir. Eğer Unicode karakter desteği istemiyorsanız,
            modelBuilder.Entity<Product>().Property(p => p.Name).IsUnicode(false);

            // kolon tipini değiştirebiliriz. Bu örnekte Name kolonunu varchar(100) olarak belirledik. Varsayılan olarak string türündeki kolonlar nvarchar olarak oluşturulur.
            modelBuilder.Entity<Product>().Property(p => p.Name).HasColumnType("varchar(100)");

            // HasIndex -- Name kolonuna index ekler. Bu, Name kolonundaki verilere daha hızlı erişim sağlar. Ancak, index eklemek veritabanında ekstra depolama alanı kullanır ve veri ekleme, güncelleme ve silme işlemlerini yavaşlatabilir.
            // IsUnique = true -- Name kolonundaki değerlerin benzersiz olmasını sağlar. Aynı değere sahip iki ürün eklenemez.
            modelBuilder.Entity<Product>().HasIndex(p => p.Name).IsUnique();

            // Name ve Price kolonlarına composite index ekler. Bu, Name ve Price kolonlarındaki verilere daha hızlı erişim sağlar. Ancak, index eklemek veritabanında ekstra depolama alanı kullanır ve veri ekleme, güncelleme ve silme işlemlerini yavaşlatabilir.
            modelBuilder.Entity<Product>().HasIndex(p => new { p.Name, p.Price });

            // IncludeProperties -- index oluştururken hangi kolonların indexe dahil edileceğini belirtir. Bu, indexin daha verimli kullanılmasını sağlar. Ancak, index eklemek veritabanında ekstra depolama alanı kullanır ve veri ekleme, güncelleme ve silme işlemlerini yavaşlatabilir.
            modelBuilder.Entity<Product>().HasIndex(p =>p.Name).IncludeProperties(p => p.Price);

            // Check Constraint -- Price kolonunun DiscountPrice kolonundan büyük olmasını sağlar. Bu, veritabanında veri bütünlüğünü sağlar. Ancak, check constraint eklemek veritabanında ekstra işlem yapar ve veri ekleme, güncelleme ve silme işlemlerini yavaşlatabilir.
            // CK_Product_Price -- check constraint'in adıdır. [Price] > [DiscountPrice] -- check constraint'in koşuludur. Price kolonunun DiscountPrice kolonundan büyük olmasını sağlar.
            // Check Constraint -- Price kolonunun DiscountPrice kolonundan büyük olmasını sağlar. Bu, veritabanında veri bütünlüğünü sağlar. Ancak, check constraint eklemek veritabanında ekstra işlem yapar ve veri ekleme, güncelleme ve silme işlemlerini yavaşlatabilir.
            modelBuilder.Entity<Product>().HasCheckConstraint("CK_Product_Price", "[Price] > [DiscountPrice]");


            // Vw_ProductEssentials adında bir view oluşturduk. Bu view, Product tablosundaki Name ve Price kolonlarını içerir. ProductEssential sınıfı bu view'a karşılık gelir. Bu sınıfın birincil anahtarı olmadığını belirtmek için HasNoKey() metodunu kullanırız.
            modelBuilder.Entity<ProductEssential>().ToView("Vw_ProductEssentials").HasNoKey();

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
