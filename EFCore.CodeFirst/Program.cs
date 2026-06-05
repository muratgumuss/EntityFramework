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

    // join işlemi -- bir kategoriyi çektiğimizde o kategoriye ait ürünlerin gelmesini sağlar. Ancak, her ürün için ayrı bir sorgu atılır. Bu da performans sorunlarına yol açar. Bunu çözmek için eager loading kullanabiliriz.

    var joinResult = await (from p in context.Products
                            join c in context.Categories on p.CategoryId equals c.Id
                            select new
                            {
                                ProductId = p.Id,
                                ProductName = p.Name,
                                CategoryName = c.Name
                            }).ToListAsync();



    var joinResult2 = await context.Products
        .Join(context.Categories,
            p => p.CategoryId,
            c => c.Id,
            (p, c) => new
            {
                ProductId = p.Id,
                ProductName = p.Name,
                CategoryName = c.Name
            }).ToListAsync();

    // 3 lü join örneği
    var joinResult3 = await (from p in context.Products
                             join c in context.Categories on p.CategoryId equals c.Id
                             join pf in context.productFeatures on p.Id equals pf.Id
                             select new
                             {
                                 ProductId = p.Id,
                                 ProductName = p.Name,
                                 CategoryName = c.Name,
                                 Color = pf.Color,
                                 Height = pf.Height,
                                 Width = pf.Width
                             }).ToListAsync();

    // 3 lü join örneği
    var joinResult4 = await context.Products
        .Join(context.Categories,
            p => p.CategoryId,
            c => c.Id,
            (p, c) => new
            {
                ProductId = p.Id,
                ProductName = p.Name,
                CategoryName = c.Name,
                ProductFeature = context.productFeatures.FirstOrDefault(pf => pf.Id == p.Id)
            }).ToListAsync();

    // left join örneği -- into kullanarak left join yapabiliriz. Left join, bir tablodaki tüm kayıtları getirirken, diğer tablodaki eşleşen kayıtları getirir. Eşleşmeyen kayıtlar için null değer döner.
    var leftJoinResult = await (from p in context.Products
                                join c in context.Categories on p.CategoryId equals c.Id into pc
                                from c in pc.DefaultIfEmpty()
                                select new
                                {
                                    ProductId = p.Id,
                                    ProductName = p.Name,
                                    CategoryName = c != null ? c.Name : "No Category"
                                }).ToListAsync();

    // left join örneği -- group join kullanarak left join yapabiliriz. Group join, bir tablodaki tüm kayıtları getirirken, diğer tablodaki eşleşen kayıtları gruplar halinde getirir. Eşleşmeyen kayıtlar için boş bir koleksiyon döner.
    var leftJoinResult2 = await context.Products
        .GroupJoin(context.Categories,
            p => p.CategoryId,
            c => c.Id,
            (p, c) => new
            {
                ProductId = p.Id,
                ProductName = p.Name,
                CategoryName = c.FirstOrDefault() != null ? c.FirstOrDefault().Name : "No Category"
            }).ToListAsync();

    // right join örneği -- right join, bir tablodaki tüm kayıtları getirirken, diğer tablodaki eşleşen kayıtları getirir. Eşleşmeyen kayıtlar için null değer döner. EF Core’da right join doğrudan desteklenmez, ancak left join kullanarak right join yapabilirsiniz.
    var rightJoinResult = await (from c in context.Categories
                                 join p in context.Products on c.Id equals p.CategoryId into cp
                                 from p in cp.DefaultIfEmpty()
                                 select new
                                 {
                                     ProductId = p != null ? p.Id : 0,
                                     ProductName = p != null ? p.Name : "No Product",
                                     CategoryName = c.Name
                                 }).ToListAsync();

    var rightJoinResult2 = await context.Categories
        .GroupJoin(context.Products,
            c => c.Id,
            p => p.CategoryId,
            (c, p) => new
            {
                ProductId = p.FirstOrDefault() != null ? p.FirstOrDefault().Id : 0,
                ProductName = p.FirstOrDefault() != null ? p.FirstOrDefault().Name : "No Product",
                CategoryName = c.Name
            }).ToListAsync();


    // full outer join örneği -- full outer join, her iki tablodaki tüm kayıtları getirirken, eşleşen kayıtları birleştirir. Eşleşmeyen kayıtlar için null değer döner. EF Core’da full outer join doğrudan desteklenmez, ancak left join ve right join kullanarak full outer join yapabilirsiniz.
    // union -- 
    var fullOuterJoinResult = await (from p in context.Products
                                     join c in context.Categories on p.CategoryId equals c.Id into pc
                                     from c in pc.DefaultIfEmpty()
                                     select new
                                     {
                                         ProductId = p.Id,
                                         ProductName = p.Name,
                                         CategoryName = c != null ? c.Name : "No Category"
                                     }).Union(
                                  from c in context.Categories
                                  join p in context.Products on c.Id equals p.CategoryId into cp
                                  from p in cp.DefaultIfEmpty()
                                  select new
                                  {
                                      ProductId = p != null ? p.Id : 0,
                                      ProductName = p != null ? p.Name : "No Product",
                                      CategoryName = c.Name
                                  }).ToListAsync();

    // full outer join örneği 2
    var fullOuterJoinResult2 = await context.Products
        .GroupJoin(context.Categories,
            p => p.CategoryId,
            c => c.Id,
            (p, c) => new
            {
                ProductId = p.Id,
                ProductName = p.Name,
                CategoryName = c.FirstOrDefault() != null ? c.FirstOrDefault().Name : "No Category"
            }).Union(
        context.Categories
        .GroupJoin(context.Products,
            c => c.Id,
            p => p.CategoryId,
            (c, p) => new
            {
                ProductId = p.FirstOrDefault() != null ? p.FirstOrDefault().Id : 0,
                ProductName = p.FirstOrDefault() != null ? p.FirstOrDefault().Name : "No Product",
                CategoryName = c.Name
            })).ToListAsync();


    // raw sql örneği -- raw sql, SQL sorgularını doğrudan kullanarak veri çekmenizi sağlar. Ancak, raw sql kullanırken SQL injection saldırılarına karşı dikkatli olmalısınız. Parametreli sorgular kullanarak bu saldırılardan korunabilirsiniz.
    var rawSqlResult = await context.Products
        .FromSqlRaw("SELECT * FROM Products WHERE Price > {0}", 20000)
        .ToListAsync();

    // fromsqlinterpolated örneği -- fromsqlinterpolated, SQL sorgularını doğrudan kullanarak veri çekmenizi sağlar. Ancak, fromsqlinterpolated kullanırken SQL injection saldırılarına karşı dikkatli olmalısınız. Parametreli sorgular kullanarak bu saldırılardan korunabilirsiniz.
    var fromSqlInterpolatedResult = await context.Products
        .FromSqlInterpolated($"SELECT * FROM Products WHERE Price > {20000}")
        .ToListAsync();

    // custom query örneği -- custom query, SQL sorgularını doğrudan kullanarak veri çekmenizi sağlar. Ancak, custom query kullanırken SQL injection saldırılarına karşı dikkatli olmalısınız. Parametreli sorgular kullanarak bu saldırılardan korunabilirsiniz.
    var customQueryResult = await context.ProductEssentials
        .FromSqlRaw("SELECT Id, Name, Price FROM Products WHERE Price > {0}", 20000)
        .ToListAsync();

    // tosqlquery örneği -- tosqlquery, LINQ sorgularını SQL sorgularına dönüştürmenizi sağlar. Bu, sorgularınızın veritabanında nasıl çalıştığını görmenizi sağlar. Ancak, tosqlquery kullanırken performans sorunlarına dikkat etmelisiniz, çünkü bazı LINQ sorguları veritabanında verimsiz SQL sorgularına dönüşebilir.
    var toSqlQueryResult = context.Products
        .Where(p => p.Price > 20000)
        .ToQueryString();

    // ToView örneği -- toview, bir sorgunun sonucunu bir view olarak kaydetmenizi sağlar. Bu, sorgularınızın performansını artırabilir, çünkü view’lar veritabanında önceden hesaplanmış sonuçları saklar. Ancak, toview kullanırken view’ların güncellenmesi gerektiğini unutmayın, çünkü view’lar veritabanında saklandığı için veriler değiştiğinde view’ların da güncellenmesi gerekir.
    var toViewResult = await context.ProductEssentials.ToListAsync();

    // Pagenation - Take & Skip -- pagenation, büyük veri setlerini sayfalara bölerek kullanıcıya sunmanızı sağlar. Take metodu, belirli bir sayıda kayıt almanızı sağlar, Skip metodu ise belirli bir sayıda kaydı atlamanızı sağlar. Bu iki metodu birlikte kullanarak sayfalama yapabilirsiniz.
    int pageSize = 10;
    int pageNumber = 1;
    var paginatedResult = await context.Products
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    // Global query filters -- global query filters, belirli bir koşula göre tüm sorgulara otomatik olarak uygulanan filtrelerdir. Örneğin, bir ürünün stokta olup olmadığını kontrol etmek için global query filter kullanabilirsiniz. Bu sayede, stokta olmayan ürünler sorgularınızda otomatik olarak filtrelenir ve kullanıcıya gösterilmez.
    var productsWithGlobalFilter = await context.Products.ToListAsync();
    // ignore query filters -- ignore query filters, global query filters’ı geçersiz kılarak sorgularınızda tüm kayıtları getirmenizi sağlar. Örneğin, stokta olmayan ürünleri de göstermek istediğinizde ignore query filters kullanabilirsiniz.
    var productsWithoutGlobalFilter = await context.Products.IgnoreQueryFilters().ToListAsync();

    // Query tags -- query tags, sorgularınıza açıklama eklemenizi sağlar. Bu açıklamalar, sorgularınızın veritabanında nasıl çalıştığını anlamanıza yardımcı olabilir. Ayrıca, query tags kullanarak sorgularınızı gruplandırabilir ve performans analizleri yapabilirsiniz.
    var taggedQueryResult = await context.Products
        .TagWith("This is a tagged query")
        .Where(p => p.Price > 20000)
        .ToListAsync();

    // Global Tracking / No Tracking -- global tracking, tüm sorgularınızda değişiklik takibini etkinleştirir. Bu, sorgularınızın sonucunda dönen nesnelerin değişikliklerini takip etmenizi sağlar. No tracking ise, sorgularınızda değişiklik takibini devre dışı bırakır. Bu, sorgularınızın sonucunda dönen nesnelerin değişikliklerini takip etmenizi engeller ve performansı artırabilir.
    var trackedProducts = await context.Products.ToListAsync();
    var untrackedProducts = await context.Products.AsNoTracking().ToListAsync();

    // Stored Procedures -- stored procedures, veritabanında önceden tanımlanmış SQL sorgularıdır. Bu sorgular, belirli bir işlemi gerçekleştirmek için kullanılır. EF Core’da stored procedures kullanarak veri çekebilir, ekleyebilir, güncelleyebilir ve silebilirsiniz. Ancak, stored procedures kullanırken performans sorunlarına dikkat etmelisiniz, çünkü bazı stored procedures veritabanında verimsiz çalışabilir.
    var storedProcedureResult = await context.Products
        .FromSqlRaw("EXEC sp_GetProductsByPrice @Price = {0}", 20000)
        .ToListAsync();


    // sp_insert_product stored procedure'ünü kullanarak yeni bir ürün ekleyelim. Bu stored procedure, ürünün adını, fiyatını ve kategori ID'sini parametre olarak alır ve yeni bir ürün ekler.
    var newProductName = "Samsung Galaxy S23";
    var newProductPrice = 25000;
    var newProductCategoryId = 1;
    await context.Database.ExecuteSqlRawAsync("EXEC sp_insert_product @Name = {0}, @Price = {1}, @CategoryId = {2}",
        newProductName, newProductPrice, newProductCategoryId);

    // function mapping -- function mapping, veritabanında tanımlı bir fonksiyonu EF Core’da kullanmanızı sağlar. Bu, veritabanında tanımlı bir fonksiyonu LINQ sorgularınızda kullanarak veri çekmenizi sağlar. Ancak, function mapping kullanırken performans sorunlarına dikkat etmelisiniz, çünkü bazı fonksiyonlar veritabanında verimsiz çalışabilir.
    var functionMappingResult = await context.Products
        .Where(p => EF.Functions.Like(p.Name, "%Iphone%"))
        .ToListAsync();
    // function table mapping -- function table mapping, veritabanında tanımlı bir fonksiyonu EF Core’da bir tablo gibi kullanmanızı sağlar. Bu, veritabanında tanımlı bir fonksiyonu LINQ sorgularınızda kullanarak veri çekmenizi sağlar. Ancak, function table mapping kullanırken performans sorunlarına dikkat etmelisiniz, çünkü bazı fonksiyonlar veritabanında verimsiz çalışabilir.
    var functionTableMappingResult = await context.Products
        .FromSqlRaw("SELECT * FROM dbo.GetProductsByPrice({0})", 20000)
        .ToListAsync();

    // table valued function
    var tableValuedFunctionResult = await context.Products
        .FromSqlRaw("SELECT * FROM dbo.GetProductsByPrice({0})", 20000)
        .ToListAsync();

    // scalar valued function -- scalar valued function, veritabanında tanımlı bir fonksiyonu EF Core’da bir değer olarak kullanmanızı sağlar. Bu, veritabanında tanımlı bir fonksiyonu LINQ sorgularınızda kullanarak veri çekmenizi sağlar. Ancak, scalar valued function kullanırken performans sorunlarına dikkat etmelisiniz, çünkü bazı fonksiyonlar veritabanında verimsiz çalışabilir.








}