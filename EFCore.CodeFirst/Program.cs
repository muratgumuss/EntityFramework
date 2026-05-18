using EFCore.CodeFirst.DAL;
using Microsoft.EntityFrameworkCore;

using (var context = new AppDbContext())
{
    var category = new Category()
    {
        Name = "Telefon"
    };
    var newProduct = new Product
    {
        Name = "Iphone 15 Pro Max",
        Price = 30000,
        DiscountPrice = 25000,
        Stock = 100,
        Barcode = 123456789,
        Category = category

    };
    context.Products.Add(newProduct);
    context.SaveChanges();

    var category2 = new Category()
    {
        Name = "Bilgisayar"
    };

    category2.Products.Add(new Product
    {
        Name = "Macbook Pro M2",
        Price = 40000,
        DiscountPrice = 35000,
        Stock = 50,
        Barcode = 987654321
    });
    context.Categories.Add(category2);
    context.SaveChanges();

    var product = await context.Products.FirstOrDefaultAsync(p => p.Id == 1);

    Console.WriteLine($"Before Adding - State: {context.Entry(newProduct).State}");
    //await context.AddAsync(newProduct);
    // context.Entry(newProduct).State = EntityState.Added;
    //context.Remove(product);
    await context.SaveChangesAsync();
    Console.WriteLine($"After Adding - State: {context.Entry(newProduct).State}");


    Console.WriteLine($"After Save - State: {context.Entry(newProduct).State}");

    var products = await context.Products.AsNoTracking().ToListAsync();

    products.ForEach(p =>
    {
        var state = context.Entry(p).State;
        Console.WriteLine($"Id: {p.Id} - Name: {p.Name} - Price: {p.Price} - State: {state}");
    });

    context.ChangeTracker.Entries().ToList().ForEach(e =>
    {
        if (e.Entity is Product p)
        {
            Console.WriteLine($"Id: {p.Id} - Name: {p.Name} - Price: {p.Price} - State: {e.State}");
        }
    });

    context.Products.Add(new Product
    {
        Name = "Iphone 15 Pro Max",
        Price = 30000,
        DiscountPrice = 25000,
        Stock = 100,
        Barcode = 123456789,
        CategoryId = 1
    });
    context.ChangeTracker.Entries().ToList().ForEach(e =>
    {
        if (e.Entity is Product p)
        {
            if (e.State == EntityState.Added)
            {
                p.CreatedDate = DateTime.Now;
            }
        }
    });
    context.SaveChanges();

    // eager loading -- bir kategoriyi çektiğimizde o kategoriye ait ürünlerinde gelmesini sağlar. Include metodu
    // then include metodu ile ürünlerin özelliklerini de çekebiliriz.
    // then include vs include -- then include metodu ile birden fazla ilişkiyi çekebiliriz. Include metodu ile sadece bir ilişkiyi çekebiliriz.
    var categoriesWithProducts = await context.Categories
        .Include(c => c.Products)
        .ThenInclude(p => p.ProductFeature)
        .ToListAsync();

    // explicit loading -- bir kategoriyi çektiğimizde o kategoriye ait ürünlerin gelmesini sağlar. Load metodu
    context.Entry(category).Collection(c => c.Products).Load();

    category.Products.ForEach(p =>
    {
        Console.WriteLine($"Id: {p.Id} - Name: {p.Name} - Price: {p.Price}");
    });

    // lazy loading -- bir kategoriyi çektiğimizde o kategoriye ait ürünlerin gelmesini sağlar.
    // virtual keywordü ile lazy loading özelliğini aktif ederiz. Lazy loading özelliği aktif olduğunda,
    // bir kategoriyi çektiğimizde o kategoriye ait ürünler gelmez.
    // Ürünlere erişmeye çalıştığımızda, o ürünler veritabanından çekilir ve gelir.
    var categoryLazyLoading = await context.Categories.FirstOrDefaultAsync(c => c.Id == 1);

    /*
     Buradaki “virtual” ifadesi, Category sınıfındaki navigation property’ler (örneğin, Products gibi) için kullanılır. Örneğin:
    public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual ICollection<Product> Products { get; set; }
}
    Burada virtual anahtar kelimesi, EF Core’un proxy oluşturup lazy loading (tembel yükleme) yapabilmesini sağlar.
    Yani, categoryLazyLoading.Products’a eriştiğinizde, EF Core veritabanından ilgili ürünleri otomatik olarak çeker.
    virtual anahtar kelimesi, navigation property’lerde (ör. Products) lazy loading’i etkinleştirir.

    Doğru, verdiğiniz örnekte doğrudan Products koleksiyonuna erişmiyorsunuz, sadece bir kategori çekiyorsunuz:
    var categoryLazyLoading = await context.Categories.FirstOrDefaultAsync(c => c.Id == 1);
    Ancak, lazy loading’in devreye girmesi için navigation property’ye (örneğin, categoryLazyLoading.Products) erişmeniz gerekir. 
    Sadece kategoriyi çekmek lazy loading’i tetiklemez. Aşağıdaki gibi bir erişim yaptığınızda lazy loading çalışır:

    var products = categoryLazyLoading.Products; // Burada veritabanından ürünler çekilir

    Özet:
    virtual anahtar kelimesi, navigation property’lerde lazy loading’i mümkün kılar. 
    Ancak lazy loading’in çalışması için o property’ye erişmeniz gerekir; sadece ana nesneyi çekmek yeterli değildir.

    Evet, EF Core’da lazy loading özelliğini kullanmak için ayrıca Microsoft.EntityFrameworkCore.Proxies NuGet paketini eklemeniz gerekir. 
    Ayrıca, DbContext’te proxy kullanımını etkinleştirmelisiniz:
     */

    // lazy loading n+1 problem -- bir kategoriyi çektiğimizde o kategoriye ait ürünlerin gelmesini sağlar. Ancak, her ürün için ayrı bir sorgu atılır. Bu da performans sorunlarına yol açar.
    // n+1 kod örneği:
    // n+1 çözümü -- bir kategoriyi çektiğimizde o kategoriye ait ürünlerin gelmesini sağlar. Ancak, her ürün için ayrı bir sorgu atılır.
    // Bu da performans sorunlarına yol açar. Bunu çözmek için eager loading kullanabiliriz.
    var categoriesLazyLoading = await context.Categories.ToListAsync();
    categoriesLazyLoading.ForEach(category =>
    {
        var products = category.Products;
    });

    // Tbh örneği -- bir kategoriyi çektiğimizde o kategoriye ait ürünlerin gelmesini sağlar. Ancak, her ürün için ayrı bir sorgu atılır. Bu da performans sorunlarına yol açar.

    //var employees = await context.Employees.ToListAsync();

    var productFulls = context.ProductFulls.FromSqlRaw(@"SELECT p.Id AS Product_Id, p.Name, p.Price, c.Name AS CategoryName, p.Stock 
    FROM Products p INNER JOIN Categories c ON p.CategoryId = c.Id").ToList();


}