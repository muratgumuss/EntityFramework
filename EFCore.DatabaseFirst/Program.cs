

using EFCore.DatabaseFirst.DAL;
using Microsoft.EntityFrameworkCore;

DbContextInitializer.Build();

using (var _context = new AppDbContext(DbContextInitializer.OptionsBuilder.Options))
{
    var products = await _context.Products.ToListAsync();

    products.ForEach(p =>
    {
        Console.WriteLine($"Id: {p.Id} Name: {p.Name} Price: {p.Price} Stock: {p.Stock}");
    });
}