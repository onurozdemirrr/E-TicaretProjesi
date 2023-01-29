using ETicaret.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ETicaret.WebUI.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        [Display(Name = "Kategori Adı", Prompt ="Kategori Adını giriniz.")]
        [Required(ErrorMessage ="Kategori Adı zorunlu bir alandır, boş geçilemez.")]
        [StringLength(50, MinimumLength =3, ErrorMessage ="Kategori Adı 3-50 karakter arasında olmalıdır.")]
        public string Name { get; set; }

        [Display(Name ="Kategori Linki", Prompt ="Kategori Linkini giriniz.")]
        [Required(ErrorMessage = "Kategori Linki zorunlu bir alandır, boş geçilemez.")]
        public string Url { get; set; }

        public List<Product> Products { get; set; }
        public CategoryModel()
        {
            Products = new List<Product>();
        }
    }
}
