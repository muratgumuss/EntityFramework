using Microsoft.EntityFrameworkCore;

namespace EFCore.CodeFirst.DAL
{
    // Owned attribute'ü, bir varlığın başka bir varlık tarafından sahiplenildiğini belirtmek için kullanılır.
    // Bu, genellikle bir varlığın başka bir varlık tarafından tamamen kapsandığı durumlarda kullanılır.
    // Örneğin, bir Employee varlığına sahip olan bir Person varlığı olabilir.
    // Bu durumda, Person varlığı Employee tarafından sahiplenilir ve Employee tablosunda Person bilgileri de tutulur.
    [Owned]
    public class Employee 
    {
        public int Id { get; set; }

        [Precision(18, 2)]
        public decimal Salary { get; set; }

        public virtual BasedPerson People { get; set; }
    }
}
