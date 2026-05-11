namespace EFCore.CodeFirst.DAL
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        public List<Teacher> Teachers { get; set; } = new();
    }
}
