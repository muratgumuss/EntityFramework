namespace EFCore.CodeFirst.DAL
{
    public class Teacher
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual List<Student> Students { get; set; } = new();
    }
}
