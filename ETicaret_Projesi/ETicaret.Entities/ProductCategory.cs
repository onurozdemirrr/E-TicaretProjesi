using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.Entities
{
    public class ProductCategory
    {
        // Bu Classın amacı bir product birden çok Category'ye karşılık gelebilir. Aynı zamanda bir category birden fazla product'a karşılık gelebilir. Yani Çoktan çoğa bir ilişki olacak. Bunu da ilişki kurulacak classın dışında üçüncü bir class tanımlayarak oluşturabiliyoruz.

        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
