using Microsoft.EntityFrameworkCore;

namespace EFCore.CodeFirst.DAL
{
    public class Employee : BasedPerson
    {
        [Precision(18, 2)]
        public decimal Salary { get; set; }
    }
}
