using EFCore.CodeFirst.DAL;
using Microsoft.EntityFrameworkCore;

using (var context = new AppDbContext())
{
    var newProduct = new Product
    {
        Name = "Iphone 15 Pro Max",
        Price = 30000,
        DiscountPrice = 25000,
        Stock = 100,
        Barcode = 123456789,
        CategoryId = 1
    };

    var product = await context.Products.FirstOrDefaultAsync(p => p.Id == 10);

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
        if(e.Entity is Product p)
        {
            Console.WriteLine($"Id: {p.Id} - Name: {p.Name} - Price: {p.Price} - State: {e.State}");
        }
    }); 

    context.Products.Add(new Product     {
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
}