using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCore.CodeFirst.DAL
{

    // Data Annotations kullanarak tablo ismini değiştirebiliriz.
    //[Table("Products")]
    public class Product
    {
        // None -- Id property'sinin değerini manuel olarak gireceğimiz anlamına gelir. Veritabanı bu değeri otomatik olarak oluşturmaz.
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        //Solution Nullable Reference Types özelliği açık olduğu için string türündeki Name property'si nullable olarak tanımlanmıştır.
        //Bu, Name property'sinin null değer alabileceği anlamına gelir. Eğer Name property'sinin null olmasını istemiyorsanız, nullable olmayan bir string türü kullanabilirsiniz:
        public string? Name { get; set; }
        // #######.##
        [Precision(9, 2)]
        public decimal Price { get; set; }

        [Precision(9, 2)]
        public decimal DiscountPrice { get; set; }

        // Data Annotations kullanarak kolon ismini değiştirebiliriz.
        //[Column("StockAmount")]
        public int Stock { get; set; }

        public int Barcode { get; set; }

        public int CategoryId { get; set; }

        public bool IsDeleted { get; set; }

        // Data Annotations kullanarak oluşturulma tarihini otomatik olarak ekleyebiliriz. Update yaparken bu kolonun güncellenmesini engelleyebiliriz.
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        // Computed -- veritabanında oluşturulma tarihini otomatik olarak ekleyebiliriz. Update yaparken bu kolonun güncellenmesini engelleyebiliriz.
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreatedDate { get; set; }

        //navigation property
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        public virtual ProductFeature ProductFeature { get; set; }
    }
}
