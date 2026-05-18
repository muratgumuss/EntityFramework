using Microsoft.EntityFrameworkCore;

namespace EFCore.CodeFirst.DAL
{
    [Owned]
    public class Manager 
    {
        public int Id { get; set; }
        public int Grade { get; set; }

        public virtual BasedPerson People { get; set; }
    }
}
