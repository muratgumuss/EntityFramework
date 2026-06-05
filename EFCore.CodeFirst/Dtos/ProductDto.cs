namespace EFCore.CodeFirst.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // #######.##
        public decimal Price { get; set; }

        public decimal DiscountPrice { get; set; }

        public int Stock { get; set; }

    }
}
