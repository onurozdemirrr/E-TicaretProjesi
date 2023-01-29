using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OderId { get; set; }
        public int ProductId { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        // İlişkiler
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
