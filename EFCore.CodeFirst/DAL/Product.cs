using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCore.CodeFirst.DAL
{

    // Data Annotations kullanarak tablo ismini değiştirebiliriz.
    //[Table("Products")]
    // Name kolonuna unique index ekler. Bu, Name kolonundaki değerlerin benzersiz olmasını sağlar. Aynı değere sahip iki ürün eklenemez.
    // IsUnique = true -- Name kolonundaki değerlerin benzersiz olmasını sağlar. Aynı değere sahip iki ürün eklenemez.
    // IsUnique = false -- Name kolonundaki değerlerin benzersiz olmasını engeller. Aynı değere sahip iki ürün eklenebilir.
    // Index -- Name kolonuna index ekler. Bu, Name kolonundaki verilere daha hızlı erişim sağlar. Ancak, index eklemek veritabanında ekstra depolama alanı kullanır ve veri ekleme, güncelleme ve silme işlemlerini yavaşlatabilir.
    [Index(nameof(Name), IsUnique = true)]
    // Name ve Price kolonlarına composite index ekler. Bu, Name ve Price kolonlarındaki verilere daha hızlı erişim sağlar. Ancak, index eklemek veritabanında ekstra depolama alanı kullanır ve veri ekleme, güncelleme ve silme işlemlerini yavaşlatabilir.
    [Index(nameof(Name), nameof(Price))]
    public class Product
    {
        // None -- Id property'sinin değerini manuel olarak gireceğimiz anlamına gelir. Veritabanı bu değeri otomatik olarak oluşturmaz.
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        //Solution Nullable Reference Types özelliği açık olduğu için string türündeki Name property'si nullable olarak tanımlanmıştır.
        //Bu, Name property'sinin null değer alabileceği anlamına gelir. Eğer Name property'sinin null olmasını istemiyorsanız, nullable olmayan bir string türü kullanabilirsiniz:
        // Unicode -- Unicode karakterleri destekleyen bir string türü olduğunu belirtir. Bu, veritabanında bu kolona Unicode karakterlerin saklanabileceği anlamına gelir. Eğer Unicode karakter desteği istemiyorsanız,
        // Unicode(false) olarak belirtebilirsiniz. vchar olarak saklanır. Unicode(true) olarak belirtebilirsiniz. nvarchar olarak saklanır.
        [Unicode(false)]
        [Column(TypeName = "varchar(100)")] // kolon tipini değiştirebiliriz. Bu örnekte Name kolonunu varchar(100) olarak belirledik. Varsayılan olarak string türündeki kolonlar nvarchar olarak oluşturulur.
        public string? Name { get; set; }
        // #######.##
        [Precision(9, 2)]
        public decimal Price { get; set; }

        [Precision(9, 2)]
        public decimal DiscountPrice { get; set; }

        // Data Annotations kullanarak kolon ismini değiştirebiliriz.
        //[Column("StockAmount")]
        public int Stock { get; set; }

        // NotMapped -- Bu property’nin veritabanında bir kolona karşılık gelmediğini belirtir. Yani, bu property veritabanında oluşturulmaz ve sorgulanmaz. Sadece uygulama içinde kullanılır.
        // Örneğin, Barcode property’si veritabanında bir kolona karşılık gelmez, sadece uygulama içinde kullanılır.
        //[NotMapped]
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
