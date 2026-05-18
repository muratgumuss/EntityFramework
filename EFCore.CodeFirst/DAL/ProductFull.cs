using Microsoft.EntityFrameworkCore;

namespace EFCore.CodeFirst.DAL
{
    // Keyless -- Bu sınıfın birincil anahtarı olmadığını belirtir. Genellikle veritabanında bir tabloya karşılık gelmeyen,
    // sadece sorgulama sonuçlarını tutmak için kullanılan sınıflar için kullanılır.
    [Keyless]
    public class ProductFull
    {
        public int Product_Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int Stock { get; set; }


    }
}
